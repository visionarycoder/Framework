# PowerShell script to add all documentation and configuration directories to the solution

$solutionFile = "VisionaryCoder.Framework.sln"

# Create a backup of the solution file
Copy-Item $solutionFile "$solutionFile.backup"

Write-Host "Adding directories and files to solution..." -ForegroundColor Green

# First, let's get the contents of each directory and add them systematically

# Function to get all files recursively in a directory
function Get-FilesRecursively {
    param(
        [string]$Path,
        [string]$RelativePath = ""
    )
    
    $files = @()
    
    if (Test-Path $Path) {
        Get-ChildItem -Path $Path -File | ForEach-Object {
            $relativePath = if ($RelativePath) { "$RelativePath\$($_.Name)" } else { $_.Name }
            $files += @{
                FullPath = $_.FullName
                RelativePath = "$Path\$relativePath"
            }
        }
        
        Get-ChildItem -Path $Path -Directory | ForEach-Object {
            $subRelativePath = if ($RelativePath) { "$RelativePath\$($_.Name)" } else { $_.Name }
            $files += Get-FilesRecursively -Path $_.FullName -RelativePath $subRelativePath
        }
    }
    
    return $files
}

# Get all files for each directory
$bestPracticesFiles = Get-FilesRecursively ".best-practices"
$copilotFiles = Get-FilesRecursively ".copilot"
$githubFiles = Get-FilesRecursively ".github"
$nugetFiles = Get-FilesRecursively ".nuget"
$docsFiles = Get-FilesRecursively "docs"
$scriptsFiles = Get-FilesRecursively "scripts"

Write-Host "Found files:" -ForegroundColor Yellow
Write-Host "  .best-practices: $($bestPracticesFiles.Count) files" -ForegroundColor Cyan
Write-Host "  .copilot: $($copilotFiles.Count) files" -ForegroundColor Cyan
Write-Host "  .github: $($githubFiles.Count) files" -ForegroundColor Cyan
Write-Host "  .nuget: $($nugetFiles.Count) files" -ForegroundColor Cyan
Write-Host "  docs: $($docsFiles.Count) files" -ForegroundColor Cyan
Write-Host "  scripts: $($scriptsFiles.Count) files" -ForegroundColor Cyan

# Read the current solution content
$content = Get-Content $solutionFile -Raw

# Generate new GUIDs for solution folders
function New-Guid {
    return [System.Guid]::NewGuid().ToString("B").ToUpper()
}

$copilotFolderGuid = New-Guid
$nugetFolderGuid = New-Guid
$docsFolderGuid = New-Guid
$scriptsFolderGuid = New-Guid

# Find the insertion point (before the Global section)
$globalSectionIndex = $content.IndexOf("Global")

# Generate the new solution folder entries
$newFolders = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = ".copilot", ".copilot", "$copilotFolderGuid"
	ProjectSection(SolutionItems) = preProject
$(($copilotFiles | ForEach-Object { "`t`t$($_.RelativePath) = $($_.RelativePath)" }) -join "`r`n")
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = ".nuget", ".nuget", "$nugetFolderGuid"
	ProjectSection(SolutionItems) = preProject
$(($nugetFiles | ForEach-Object { "`t`t$($_.RelativePath) = $($_.RelativePath)" }) -join "`r`n")
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "docs", "docs", "$docsFolderGuid"
	ProjectSection(SolutionItems) = preProject
$(($docsFiles | ForEach-Object { "`t`t$($_.RelativePath) = $($_.RelativePath)" }) -join "`r`n")
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "scripts", "scripts", "$scriptsFolderGuid"
	ProjectSection(SolutionItems) = preProject
$(($scriptsFiles | ForEach-Object { "`t`t$($_.RelativePath) = $($_.RelativePath)" }) -join "`r`n")
	EndProjectSection
EndProject

"@

# Find the existing .best-practices folder and add files to it
$bestPracticesFolderPattern = 'Project\("\{2150E333-8FDC-42A3-9474-1A3956D46DE8\}"\) = "\.best-practices"[^E]*EndProject'
$bestPracticesMatch = [regex]::Match($content, $bestPracticesFolderPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)

if ($bestPracticesMatch.Success) {
    Write-Host "Updating existing .best-practices folder..." -ForegroundColor Yellow
    
    $newBestPracticesSection = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = ".best-practices", ".best-practices", "{C2B33938-AE71-AF10-05E6-67F4873F4C49}"
	ProjectSection(SolutionItems) = preProject
$(($bestPracticesFiles | ForEach-Object { "`t`t$($_.RelativePath) = $($_.RelativePath)" }) -join "`r`n")
	EndProjectSection
EndProject
"@
    
    $content = $content -replace $bestPracticesFolderPattern, $newBestPracticesSection
}

# Insert the new folders before Global section
$beforeGlobal = $content.Substring(0, $globalSectionIndex)
$afterGlobal = $content.Substring($globalSectionIndex)

$newContent = $beforeGlobal + $newFolders + $afterGlobal

# Write the updated solution file
Set-Content -Path $solutionFile -Value $newContent -NoNewline

Write-Host "Solution file updated successfully!" -ForegroundColor Green
Write-Host "Backup saved as: $solutionFile.backup" -ForegroundColor Yellow