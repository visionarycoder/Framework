# PowerShell script to rename all VisionaryCoder.* namespaces to VisionaryCoder.Framework.*

# Get all C# files in the VisionaryCoder.Framework projects
$frameworkProjects = Get-ChildItem -Path "src" -Directory | Where-Object { $_.Name -like "VisionaryCoder.Framework.*" }

foreach ($project in $frameworkProjects) {
    Write-Host "Processing project: $($project.Name)" -ForegroundColor Green
    
    $csFiles = Get-ChildItem -Path $project.FullName -Filter "*.cs" -Recurse
    
    foreach ($file in $csFiles) {
        $content = Get-Content -Path $file.FullName -Raw
        $originalContent = $content
        
        # Replace old namespace patterns with new Framework patterns
        $content = $content -replace 'namespace VisionaryCoder\.Extensions', 'namespace VisionaryCoder.Framework.Extensions'
        $content = $content -replace 'namespace VisionaryCoder\.Core', 'namespace VisionaryCoder.Framework.Core'
        $content = $content -replace 'namespace VisionaryCoder\.Proxy', 'namespace VisionaryCoder.Framework.Proxy'
        
        # Replace using statements too
        $content = $content -replace 'using VisionaryCoder\.Extensions', 'using VisionaryCoder.Framework.Extensions'
        $content = $content -replace 'using VisionaryCoder\.Core', 'using VisionaryCoder.Framework.Core'
        $content = $content -replace 'using VisionaryCoder\.Proxy', 'using VisionaryCoder.Framework.Proxy'
        
        if ($content -ne $originalContent) {
            Set-Content -Path $file.FullName -Value $content -NoNewline
            Write-Host "  Updated: $($file.Name)" -ForegroundColor Yellow
        }
    }
}

Write-Host "Namespace update completed!" -ForegroundColor Green