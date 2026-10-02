#requires -Version 7.0
$ErrorActionPreference = 'Stop'
$containerName = 'crc-foundation-' + [Guid]::NewGuid().ToString('N')
$oldPassword = $env:POSTGRES_PASSWORD
$oldConnection = $env:CRC_TEST_POSTGRES_ADMIN
$oldDisposable = $env:CRC_TEST_DISPOSABLE
$oldEvidence = $env:CRC_TEST_EVIDENCE_DIR
try {
    $env:POSTGRES_PASSWORD = [Convert]::ToHexString([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
    docker run --detach --rm --name $containerName --env POSTGRES_PASSWORD --publish 127.0.0.1::5432 postgres:17@sha256:d74eeac9a635390a49bc21bd49fccd973de707e2a53a76ac49b552b8712ec46f
    if ($LASTEXITCODE -ne 0) { throw 'PostgreSQL fixture start failed.' }
    $ready = $false
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        docker exec $containerName pg_isready -U postgres *> $null
        if ($LASTEXITCODE -eq 0) { $ready = $true; break }
        Start-Sleep -Seconds 1
    }
    if (!$ready) { throw 'PostgreSQL fixture did not become ready.' }
    $mapping = docker port $containerName 5432/tcp
    $port = ($mapping -split ':')[-1]
    $env:CRC_TEST_POSTGRES_ADMIN = "Host=127.0.0.1;Port=$port;Database=postgres;Username=postgres;Password=$env:POSTGRES_PASSWORD;Pooling=false"
    $env:CRC_TEST_DISPOSABLE = '1'
    $env:CRC_TEST_EVIDENCE_DIR = Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/evidence'
    dotnet test CRC.sln -c Release --no-build --logger trx --results-directory artifacts/evidence/tests
    if ($LASTEXITCODE -ne 0) { throw 'Foundation tests failed.' }
}
finally {
    docker stop $containerName *> $null
    $env:POSTGRES_PASSWORD = $oldPassword
    $env:CRC_TEST_POSTGRES_ADMIN = $oldConnection
    $env:CRC_TEST_DISPOSABLE = $oldDisposable
    $env:CRC_TEST_EVIDENCE_DIR = $oldEvidence
}
