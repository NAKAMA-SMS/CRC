#requires -Version 7.0
$ErrorActionPreference='Stop'
$directory=Join-Path (Split-Path $PSScriptRoot -Parent) '.tools/gitleaks'
New-Item -ItemType Directory -Force $directory | Out-Null
if ($IsWindows) {
    $name='gitleaks_8.30.1_windows_x64.zip'
    $expected='d29144deff3a68aa93ced33dddf84b7fdc26070add4aa0f4513094c8332afc4e'
} elseif ($IsLinux) {
    $name='gitleaks_8.30.1_linux_x64.tar.gz'
    $expected='551f6fc83ea457d62a0d98237cbad105af8d557003051f41f3e7ca7b3f2470eb'
} else { throw 'Only the specified Windows/Linux validation targets are supported.' }
$archive=Join-Path $directory $name
Invoke-WebRequest "https://github.com/gitleaks/gitleaks/releases/download/v8.30.1/$name" -OutFile $archive
if ((Get-FileHash $archive -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) { throw 'Scanner checksum mismatch.' }
if ($IsWindows) { Expand-Archive -LiteralPath $archive -DestinationPath $directory -Force }
else { tar -xzf $archive -C $directory; if ($LASTEXITCODE -ne 0) { throw 'Scanner extraction failed.' } }
$env:PATH="$directory$([IO.Path]::PathSeparator)$env:PATH"
gitleaks version
if ($LASTEXITCODE -ne 0) { throw 'Scanner cannot run.' }
