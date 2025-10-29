# PowerShell script to properly add all documentation and configuration directories to the solution

$solutionFile = "VisionaryCoder.Framework.sln"

# Create a backup of the solution file
Copy-Item $solutionFile "$solutionFile.backup2"

Write-Host "Adding directories and files to solution..." -ForegroundColor Green

# Function to get all files recursively in a directory with correct paths
function Get-FilesRecursively {
    param(
        [string]$RootPath
    )
    
    $files = @()
    
    if (Test-Path $RootPath) {
        Get-ChildItem -Path $RootPath -Recurse -File | ForEach-Object {
            $relativePath = $_.FullName.Substring((Get-Location).Path.Length + 1)
            $files += $relativePath
        }
    }
    
    return $files | Sort-Object
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

# Generate new GUIDs for solution folders
function New-Guid {
    return [System.Guid]::NewGuid().ToString("B").ToUpper()
}

$copilotFolderGuid = New-Guid
$nugetFolderGuid = New-Guid
$docsFolderGuid = New-Guid
$scriptsFolderGuid = New-Guid

# Generate solution folder content with proper file references
function Generate-SolutionFolder {
    param(
        [string]$FolderName,
        [string]$FolderGuid,
        [string[]]$Files
    )
    
    $fileEntries = ($Files | ForEach-Object { "`t`t$_ = $_" }) -join "`r`n"
    
    return @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "$FolderName", "$FolderName", "$FolderGuid"
	ProjectSection(SolutionItems) = preProject
$fileEntries
	EndProjectSection
EndProject
"@
}

# Create the solution file content manually to ensure proper formatting
$solutionHeader = @"

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 18
VisualStudioVersion = 18.0.11018.127
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{CF68B68C-8A91-4020-AA05-C6862858DAB7}"
	ProjectSection(SolutionItems) = preProject
		.copilotignore = .copilotignore
		.gitignore = .gitignore
		Directory.Build.props = Directory.Build.props
		Directory.Build.targets = Directory.Build.targets
		Directory.Packages.props = Directory.Packages.props
		global.json = global.json
		LICENSE = LICENSE
		NuGet.config = NuGet.config
		README.md = README.md
		VisionaryCoder.Framework.COMPLETE.md = VisionaryCoder.Framework.COMPLETE.md
		VisionaryCoder.Framework.README.md = VisionaryCoder.Framework.README.md
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{94FEF38A-DA45-4CF1-A0DD-EA337586A1AF}"
EndProject
"@

# Add the directory solution folders
$bestPracticesFolder = Generate-SolutionFolder ".best-practices" "{C2B33938-AE71-AF10-05E6-67F4873F4C49}" $bestPracticesFiles
$copilotFolder = Generate-SolutionFolder ".copilot" $copilotFolderGuid $copilotFiles
$githubFolder = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = ".github", ".github", "{A87D5213-6DF1-4E17-83D1-FCBB76750022}"
	ProjectSection(SolutionItems) = preProject
$((($githubFiles | Where-Object { $_ -notmatch "instructions|workflows" }) | ForEach-Object { "`t`t$_ = $_" }) -join "`r`n")
	EndProjectSection
EndProject
"@

$nugetFolder = Generate-SolutionFolder ".nuget" $nugetFolderGuid $nugetFiles
$docsFolder = Generate-SolutionFolder "docs" $docsFolderGuid $docsFiles
$scriptsFolder = Generate-SolutionFolder "scripts" $scriptsFolderGuid $scriptsFiles

Write-Host "Restoring solution from backup and applying proper formatting..." -ForegroundColor Yellow

# Restore from backup and rebuild properly
if (Test-Path "VisionaryCoder.Framework.sln.backup") {
    Copy-Item "VisionaryCoder.Framework.sln.backup" $solutionFile
}

# Now let me just add the missing solution folders to the existing solution file using a simpler approach
$content = Get-Content $solutionFile -Raw

# Find the last project entry before Global section
$lastProjectPattern = 'EndProject\s*(?=Global)'
$insertionPoint = [regex]::Match($content, $lastProjectPattern).Index + 10 # After "EndProject"

# Create the additional folders text
$additionalFolders = @"

$copilotFolder
$nugetFolder
$docsFolder
$scriptsFolder

"@

# Insert the additional folders
$newContent = $content.Insert($insertionPoint, $additionalFolders)

# Write the updated content
Set-Content -Path $solutionFile -Value $newContent -NoNewline

Write-Host "Solution file updated successfully!" -ForegroundColor Green