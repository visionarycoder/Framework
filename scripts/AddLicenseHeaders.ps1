# Add MIT License Headers to C# Files
# Usage: .\AddLicenseHeaders.ps1

$licenseHeader = @"
// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

"@

function Add-LicenseHeader {
    param(
        [string]$FilePath
    )
    
    $content = Get-Content $FilePath -Raw
    
    # Check if license header already exists
    if ($content -match "Copyright.*VisionaryCoder") {
        Write-Host "License header already exists in: $FilePath" -ForegroundColor Yellow
        return
    }
    
    # Add license header at the beginning
    $newContent = $licenseHeader + $content
    Set-Content -Path $FilePath -Value $newContent -NoNewline
    Write-Host "Added license header to: $FilePath" -ForegroundColor Green
}

# Find all C# files in src directory
$csFiles = Get-ChildItem -Path "src" -Filter "*.cs" -Recurse

Write-Host "Found $($csFiles.Count) C# files" -ForegroundColor Cyan

foreach ($file in $csFiles) {
    Add-LicenseHeader -FilePath $file.FullName
}

Write-Host "`nLicense header addition complete!" -ForegroundColor Green