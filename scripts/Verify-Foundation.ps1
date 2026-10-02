#requires -Version 7.0
$ErrorActionPreference='Stop'
function Invoke-Checked([string]$Command, [string[]]$Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command failed ($LASTEXITCODE)." }
}
Invoke-Checked dotnet @('tool','restore')
Invoke-Checked dotnet @('restore','CRC.sln','--locked-mode')
Invoke-Checked npm @('ci')
Invoke-Checked dotnet @('format','CRC.sln','--verify-no-changes','--no-restore')
Invoke-Checked npm @('run','check')
Invoke-Checked npm @('run','build','--workspace','apps/web')
Invoke-Checked dotnet @('build','CRC.sln','-c','Release','--no-restore')
& "$PSScriptRoot/Test-Postgres.ps1"
Invoke-Checked dotnet @('list','CRC.sln','package','--vulnerable','--include-transitive')
Invoke-Checked npm @('audit')
& "$PSScriptRoot/Write-DependencyInventory.ps1"
Invoke-Checked gitleaks @('git','.','--redact')
Invoke-Checked gitleaks @('dir','.','--redact')
if ($IsWindows) { & "$PSScriptRoot/Verify-WindowsPublish.ps1" }
else { & "$PSScriptRoot/Verify-Browser.ps1" }
