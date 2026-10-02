#requires -Version 7.0
param([switch]$SkipBuild)
$ErrorActionPreference = 'Stop'
if (!$IsWindows) { throw 'Windows publication smoke must run on Windows.' }
function Invoke-Checked([string]$Command, [string[]]$Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command failed ($LASTEXITCODE)." }
}
$repo = Split-Path $PSScriptRoot -Parent
$publish = Join-Path $repo 'artifacts/Windows Release'
$data = Join-Path ([IO.Path]::GetTempPath()) ('crc-publish-' + [Guid]::NewGuid().ToString('N'))
$evidence = Join-Path $repo 'artifacts/evidence'
New-Item -ItemType Directory -Force $data, $evidence | Out-Null
$variables = @('Environment','Profile','Provider','DataRoot','ConnectionString','BindAddress','Port')
$previous = @{}
$previousBrowser = $env:CRC_BROWSER_BASE_URL
foreach ($name in $variables) { $previous[$name] = [Environment]::GetEnvironmentVariable("CRC_$name") }
$process = $null
try {
    if (!$SkipBuild) {
        Invoke-Checked npm @('run','build','--workspace','apps/web')
        Invoke-Checked dotnet @('publish','src/CRC.Api','-c','Release','-r','win-x64','--self-contained','true','-p:RestoreLockedMode=true','-p:PublishTrimmed=false','-p:PublishSingleFile=false','-o',"$publish/api")
        Invoke-Checked dotnet @('publish','src/CRC.DbMigrator','-c','Release','-r','win-x64','--self-contained','true','-p:RestoreLockedMode=true','-p:PublishTrimmed=false','-p:PublishSingleFile=false','-o',"$publish/migrator")
    }
    # Restrict mutable fixture data to this identity; no service/firewall changes.
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent().Name
    icacls $data /inheritance:r /grant:r "${identity}:(OI)(CI)F" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Fixture ACL failed.' }
    $env:CRC_Environment='Test'; $env:CRC_Profile='Local'; $env:CRC_Provider='Sqlite'
    $env:CRC_DataRoot=$data; $env:CRC_ConnectionString=''; $env:CRC_BindAddress='127.0.0.1'
    $listener=[Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback,0)
    $listener.Start(); $port=$listener.LocalEndpoint.Port; $listener.Stop()
    $env:CRC_Port="$port"
    $migrators=@()
    try {
        for ($index=0; $index -lt 2; $index++) {
            $migrators += Start-Process -WindowStyle Hidden -FilePath "$publish/migrator/CRC.DbMigrator.exe" -ArgumentList 'apply' -PassThru -RedirectStandardOutput "$evidence/migrator-$index.stdout.txt" -RedirectStandardError "$evidence/migrator-$index.stderr.txt"
        }
        foreach ($migrator in $migrators) {
            if (!$migrator.WaitForExit(45000)) { throw 'Concurrent migrator timed out.' }
            if ($migrator.ExitCode -ne 0) { throw 'Concurrent migrator failed.' }
        }
    } finally {
        foreach ($migrator in $migrators) { if (!$migrator.HasExited) { Stop-Process -Id $migrator.Id } }
    }
    Invoke-Checked "$publish/migrator/CRC.DbMigrator.exe" @('apply')
    Invoke-Checked "$publish/migrator/CRC.DbMigrator.exe" @('status')
    $databaseHash = (Get-FileHash "$data/crc.db").Hash
    for ($cycle=0; $cycle -lt 2; $cycle++) {
        $process=Start-Process -WindowStyle Hidden -FilePath "$publish/api/CRC.Api.exe" -WorkingDirectory $publish -PassThru -RedirectStandardOutput "$evidence/windows-$cycle.stdout.jsonl" -RedirectStandardError "$evidence/windows-$cycle.stderr.txt"
        $ready=$false
        for ($attempt=0; $attempt -lt 50; $attempt++) {
            if ($process.HasExited) { throw 'Published host exited before readiness.' }
            try {
                $response=Invoke-RestMethod "http://127.0.0.1:$port/health/ready"
                if ($response.data.status -eq 'ready') { $ready=$true; break }
            } catch { Start-Sleep -Milliseconds 200 }
        }
        if (!$ready) { throw 'Published host readiness timed out.' }
        $bindings=Get-NetTCPConnection -OwningProcess $process.Id -State Listen
        if (@($bindings | Where-Object LocalAddress -ne '127.0.0.1').Count -ne 0) { throw 'Unexpected listener.' }
        if ($cycle -eq 0) {
            $env:CRC_BROWSER_BASE_URL="http://127.0.0.1:$port"
            Invoke-Checked npm @('run','test:e2e')
        }
        # Deliberate abrupt restart check. Graceful Ctrl+C and clean-VM behavior
        # remain separate manual evidence, never claimed by this smoke.
        Stop-Process -Id $process.Id
        $process.WaitForExit(); $process=$null
        if ((Get-FileHash "$data/crc.db").Hash -ne $databaseHash) { throw 'Probe/startup mutated baseline database.' }
    }
    Get-ChildItem $publish -Recurse -File | Get-FileHash -Algorithm SHA256 | Export-Csv "$evidence/windows-artifact-sha256.csv" -NoTypeInformation
    'PASS: self-contained publication, explicit migrations, loopback browser smoke and abrupt restart. Clean VM/graceful stop not asserted.' | Set-Content "$evidence/windows-smoke.txt"
}
finally {
    if ($process -and !$process.HasExited) { Stop-Process -Id $process.Id }
    foreach ($name in $variables) { [Environment]::SetEnvironmentVariable("CRC_$name", $previous[$name]) }
    $env:CRC_BROWSER_BASE_URL=$previousBrowser
    $resolved=[IO.Path]::GetFullPath($data)
    $temp=[IO.Path]::GetFullPath([IO.Path]::GetTempPath())
    if (!$resolved.StartsWith($temp, [StringComparison]::OrdinalIgnoreCase) -or (Split-Path $resolved -Leaf) -notlike 'crc-publish-*') { throw 'Refusing fixture cleanup outside temporary directory.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
