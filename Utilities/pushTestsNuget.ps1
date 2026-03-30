param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$ApiKey
)

$originalPath = $PSScriptRoot
$releasePath = Join-Path $PSScriptRoot "../Tests/bin/Release"

if (-not (Test-Path $releasePath)) {
    Write-Error "Bin folder not found."
    exit 1
}

Set-Location $releasePath

$packageName = "eliseubatista99-dotnet-nugets-tests.$Version.nupkg"

if (-not (Test-Path $packageName)) {
    Write-Error "Package $packageName not found."
    exit 1
}

dotnet nuget push $packageName `
    --api-key $ApiKey `
    --source https://api.nuget.org/v3/index.json

Set-Location $originalPath
