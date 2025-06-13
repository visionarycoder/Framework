<#
.SYNOPSIS
  Recursively adds all .csproj files in a directory tree to a .sln
.PARAMETER SolutionPath
  Path to the .sln file (alias: -s)
.PARAMETER RootDir
  Root folder to scan (defaults to current directory) (alias: -d)
#>
param(
    [Parameter(Mandatory)]
    [Alias('s')]
    [string]$SolutionPath,

    [Alias('d')]
    [string]$RootDir = (Get-Location)
)

Write-Host "SolutionPath: $SolutionPath"
Write-Host "RootDir:      $RootDir"
Write-Host ''

function Add-Projects {
    param(
        [string]$solutionPath,
        [string]$rootDir
    )

    Get-ChildItem -Path $rootDir -Recurse -Filter *.csproj |
        ForEach-Object {
            Write-Host "Adding project:" $_.FullName
            dotnet sln $solutionPath add $_.FullName
        }
}

Add-Projects -solutionPath $SolutionPath -rootDir $RootDir
