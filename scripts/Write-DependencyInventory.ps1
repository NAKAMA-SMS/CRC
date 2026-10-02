#requires -Version 7.0
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
$output=Join-Path $repo 'artifacts/evidence'
New-Item -ItemType Directory -Force $output | Out-Null
$inventory=[Collections.Generic.List[object]]::new()
$lock=Get-Content -Raw (Join-Path $repo 'package-lock.json') | ConvertFrom-Json -AsHashtable
foreach ($entry in $lock.packages.GetEnumerator()) {
    if ($entry.Key -notlike '*node_modules/*' -or $entry.Value.link) { continue }
    $inventory.Add([pscustomobject]@{ Ecosystem='npm'; Package=$entry.Key; Version=$entry.Value.version; License=$entry.Value.license })
}
$nugetRoot=if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path ([Environment]::GetFolderPath('UserProfile')) '.nuget/packages' }
$seen=@{}
foreach ($file in Get-ChildItem (Join-Path $repo 'src'),(Join-Path $repo 'tests') -Recurse -Filter packages.lock.json) {
    $packages=Get-Content -Raw $file.FullName | ConvertFrom-Json -AsHashtable
    foreach ($target in $packages.dependencies.Values) {
        foreach ($entry in $target.GetEnumerator()) {
            if ($entry.Value.type -eq 'Project') { continue }
            $name=$entry.Key.ToLowerInvariant(); $version=$entry.Value.resolved
            $key="$name/$version"
            if ($seen.ContainsKey($key)) { continue }; $seen[$key]=$true
            $spec=Join-Path $nugetRoot "$key/$name.nuspec"
            if (!(Test-Path $spec)) { throw "Restore missing for inventory package $name." }
            [xml]$xml=Get-Content -Raw $spec
            $license=$xml.SelectSingleNode("//*[local-name()='metadata']/*[local-name()='license']")
            $inventory.Add([pscustomobject]@{ Ecosystem='NuGet'; Package=$name; Version=$version; License=if ($license) { $license.InnerText } else { 'REVIEW_REQUIRED' } })
        }
    }
}
$inventory | Sort-Object Ecosystem,Package | Export-Csv (Join-Path $output 'dependency-licenses.csv') -NoTypeInformation
$inventory | Group-Object License | Select-Object Count,Name | Format-Table
if ($inventory | Where-Object { !$_.License -or $_.License -eq 'REVIEW_REQUIRED' }) { throw 'A dependency license requires explicit review.' }
