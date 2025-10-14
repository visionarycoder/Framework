# PowerShell script to rename all VisionaryCoder projects to VisionaryCoder.Framework

# Define the project mappings
$projectMappings = @{
    "VisionaryCoder.Extensions.Configuration" = "VisionaryCoder.Framework.Extensions.Configuration"
    "VisionaryCoder.Extensions.Logging" = "VisionaryCoder.Framework.Extensions.Logging" 
    "VisionaryCoder.Extensions.Pagination" = "VisionaryCoder.Framework.Extensions.Pagination"
    "VisionaryCoder.Extensions.Primitives" = "VisionaryCoder.Framework.Extensions.Primitives"
    "VisionaryCoder.Extensions.Primitives.AspNetCore" = "VisionaryCoder.Framework.Extensions.Primitives.AspNetCore"
    "VisionaryCoder.Extensions.Primitives.EFCore" = "VisionaryCoder.Framework.Extensions.Primitives.EFCore"
    "VisionaryCoder.Extensions.Querying" = "VisionaryCoder.Framework.Extensions.Querying"
    "VisionaryCoder.Proxy" = "VisionaryCoder.Framework.Proxy"
    "VisionaryCoder.Proxy.Caching" = "VisionaryCoder.Framework.Proxy.Caching"
    "VisionaryCoder.Proxy.DependencyInjection" = "VisionaryCoder.Framework.Proxy.DependencyInjection"
    "VisionaryCoder.Proxy.Interceptors" = "VisionaryCoder.Framework.Proxy.Interceptors"
}

foreach ($oldName in $projectMappings.Keys) {
    $newName = $projectMappings[$oldName]
    $oldPath = "src/$oldName"
    $newPath = "src/$newName"
    
    if (Test-Path $oldPath) {
        Write-Host "Renaming: $oldName -> $newName" -ForegroundColor Green
        
        # Create new directory and copy content
        if (!(Test-Path $newPath)) {
            New-Item -ItemType Directory -Path $newPath -Force | Out-Null
        }
        Copy-Item -Path "$oldPath/*" -Destination $newPath -Recurse -Force
        
        # Rename the .csproj file
        $oldProjFile = "$newPath/$oldName.csproj"
        $newProjFile = "$newPath/$newName.csproj"
        
        if (Test-Path $oldProjFile) {
            Rename-Item -Path $oldProjFile -NewName "$newName.csproj"
            Write-Host "  Renamed project file: $newName.csproj" -ForegroundColor Yellow
        }
        
        # Update RootNamespace in the project file if it exists
        if (Test-Path $newProjFile) {
            $projContent = Get-Content -Path $newProjFile -Raw
            
            # Add RootNamespace if it doesn't exist, or update it if it does
            if ($projContent -match '<RootNamespace>') {
                $projContent = $projContent -replace '<RootNamespace>.*?</RootNamespace>', "<RootNamespace>$newName</RootNamespace>"
            } elseif ($projContent -match '<PropertyGroup>') {
                $projContent = $projContent -replace '(<PropertyGroup>\s*\r?\n)', "`$1    <RootNamespace>$newName</RootNamespace>`r`n"
            }
            
            # Update PackageId if it exists
            if ($projContent -match '<PackageId>') {
                $projContent = $projContent -replace '<PackageId>.*?</PackageId>', "<PackageId>$newName</PackageId>"
            }
            
            Set-Content -Path $newProjFile -Value $projContent -NoNewline
            Write-Host "  Updated project file properties" -ForegroundColor Yellow
        }
        
        # Update all C# files with namespace changes
        $csFiles = Get-ChildItem -Path $newPath -Filter "*.cs" -Recurse
        foreach ($file in $csFiles) {
            $content = Get-Content -Path $file.FullName -Raw
            $originalContent = $content
            
            # Replace namespace declarations
            $content = $content -replace "namespace $([regex]::Escape($oldName))", "namespace $newName"
            
            # Replace using statements 
            $content = $content -replace "using $([regex]::Escape($oldName))", "using $newName"
            
            if ($content -ne $originalContent) {
                Set-Content -Path $file.FullName -Value $content -NoNewline
                Write-Host "    Updated namespace in: $($file.Name)" -ForegroundColor Cyan
            }
        }
    } else {
        Write-Host "Source path not found: $oldPath" -ForegroundColor Red
    }
}

Write-Host "`nAll projects renamed to VisionaryCoder.Framework.* pattern!" -ForegroundColor Green