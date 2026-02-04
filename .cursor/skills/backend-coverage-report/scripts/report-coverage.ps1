# ReportGenerator: per test project take only the latest coverage.cobertura.xml, then merge into one HTML report
$ErrorActionPreference = "Stop"
# Script lives under .cursor/skills/coverage-report/scripts/ -> workspace root is 4 levels up
$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..\..")).Path
$testRoot = Join-Path $workspaceRoot "test"
$destDir = Join-Path $workspaceRoot "coveragereport"

$allFiles = Get-ChildItem -Path $testRoot -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue
if (-not $allFiles -or $allFiles.Count -eq 0) {
    Write-Warning "No coverage.cobertura.xml found under test/. Run task: test:coverage:report"
    exit 1
}

# One latest file per test project (path: test/ProjectName/TestResults/guid/coverage.cobertura.xml)
$files = $allFiles | Group-Object { $_.Directory.Parent.Parent.FullName } | ForEach-Object {
    $_.Group | Sort-Object LastWriteTime -Descending | Select-Object -First 1
}

$reportGenerator = "reportgenerator"
if (-not (Get-Command $reportGenerator -ErrorAction SilentlyContinue)) {
    Write-Host "Install ReportGenerator first: dotnet tool install -g dotnet-reportgenerator-globaltool"
    exit 1
}

$reportPaths = @($files | ForEach-Object { $_.FullName })
$reportsArg = $reportPaths -join ";"
& $reportGenerator "-reports:$reportsArg" "-targetdir:$destDir" "-reporttypes:Html"
$indexPath = Join-Path $destDir "index.html"
Write-Host "Report generated ($($reportPaths.Count) test project(s)): $indexPath"
