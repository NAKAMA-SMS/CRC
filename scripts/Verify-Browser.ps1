#requires -Version 7.0
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
$data=Join-Path ([IO.Path]::GetTempPath()) ('crc-browser-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $data | Out-Null
$names=@('Environment','Profile','Provider','DataRoot','ConnectionString','Port','BindAddress','BROWSER_BASE_URL')
$previous=@{}
foreach ($name in $names) { $previous[$name]=[Environment]::GetEnvironmentVariable("CRC_$name") }
$process=$null
try {
    if ($IsLinux) { chmod 700 $data; if ($LASTEXITCODE -ne 0) { throw 'Fixture protection failed.' } }
    $env:CRC_Environment='Test'; $env:CRC_Profile='Local'; $env:CRC_Provider='Sqlite'; $env:CRC_ConnectionString=''
    $env:CRC_DataRoot=$data; $env:CRC_BindAddress='127.0.0.1'
    $listener=[Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback,0)
    $listener.Start(); $env:CRC_Port="$($listener.LocalEndpoint.Port)"; $listener.Stop()
    dotnet run --project src/CRC.DbMigrator -c Release --no-build -- apply
    if ($LASTEXITCODE -ne 0) { throw 'Fixture migration failed.' }
    $start=[Diagnostics.ProcessStartInfo]::new('dotnet')
    $start.ArgumentList.Add((Join-Path $repo 'src/CRC.Api/bin/Release/net10.0/CRC.Api.dll'))
    $start.UseShellExecute=$false; $start.CreateNoWindow=$true
    $process=[Diagnostics.Process]::Start($start)
    $env:CRC_BROWSER_BASE_URL="http://127.0.0.1:$env:CRC_Port"
    $ready=$false
    for ($attempt=0; $attempt -lt 50; $attempt++) {
        if ($process.HasExited) { throw 'Fixture host exited.' }
        try {
            if ((Invoke-RestMethod "$env:CRC_BROWSER_BASE_URL/health/ready").data.status -eq 'ready') { $ready=$true; break }
        } catch { Start-Sleep -Milliseconds 200 }
    }
    if (!$ready) { throw 'Fixture readiness timed out.' }
    npm run test:e2e
    if ($LASTEXITCODE -ne 0) { throw 'Browser tests failed.' }
} finally {
    if ($process -and !$process.HasExited) { $process.Kill(); $process.WaitForExit() }
    foreach ($name in $names) { [Environment]::SetEnvironmentVariable("CRC_$name",$previous[$name]) }
    $resolved=[IO.Path]::GetFullPath($data)
    if (!$resolved.StartsWith([IO.Path]::GetFullPath([IO.Path]::GetTempPath()),[StringComparison]::Ordinal) -or (Split-Path $resolved -Leaf) -notlike 'crc-browser-*') { throw 'Unsafe fixture cleanup path.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
