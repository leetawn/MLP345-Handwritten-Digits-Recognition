# PowerShell script to download optdigits.tra and optdigits.tes into data/raw
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$targetDir = Join-Path $repoRoot "data\raw"

if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

$baseUrl = "https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits"
$fallbackUrl = "https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits"
$files = @("optdigits.tra", "optdigits.tes")

foreach ($file in $files) {
    $dest = Join-Path $targetDir $file
    if (Test-Path $dest) {
        Write-Host "File already exists, skipping: $dest"
        continue
    }

    $url = "$baseUrl/$file"
    Write-Host "Downloading $file from $url..."
    try {
        Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing
        $lines = (Get-Content $dest).Count
        Write-Host "Successfully downloaded $file ($lines lines)."
    }
    catch {
        Write-Warning "Failed to download $file from $url."
        Write-Host "Please download manually from: $fallbackUrl"
        if (Test-Path $dest) {
            Remove-Item $dest -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host "Data download completed."
