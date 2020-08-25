Param([switch]$Init, [switch]$Run, [string]$PackagePath)
$currentDir = (Convert-Path .)
$astyle = "C:\Program Files (x86)\AStyle\bin\AStyle.exe"

function Init($packagePath) {
    Copy-Item $packagePath/pre-commit.git $currentDir/.git/hooks/pre-commit
    Copy-Item $packagePath/.astylerc $currentDir/
    Write-Host "[Init] SUCCESS: File copied."
}

function Run() {
    Write-Host "--- running astyle ---"
    Get-ChildItem -Path $currentDir/Assets -Filter *.cs -Recurse | ForEach-Object {
        $fullname = $_.FullName
        $p = Start-Process $astyle -ArgumentList "--options=$currentDir/.astylerc $fullname"
    }
    Write-Host "--- running astyle done ---"
}

if ($Init) {
    Init $PackagePath
} elseif ($Run) {
    Run
}