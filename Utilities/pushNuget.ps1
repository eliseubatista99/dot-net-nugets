param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$ApiKey
)

$releasePath = Join-Path $PSScriptRoot "../bin/Release"

if (-not (Test-Path $releasePath)) {
    Write-Error "Bin folder not found."
    exit 1
}

Set-Location $releasePath

$packageName = "database-postgresql.$Version.nupkg"

if (-not (Test-Path $packageName)) {
    Write-Error "Package $packageName not found."
    exit 1
}

dotnet nuget push $packageName `
    --api-key $ApiKey `
    --source https://api.nuget.org/v3/index.json
