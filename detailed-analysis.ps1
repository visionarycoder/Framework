# Additional analysis to find potentially redundant projects
Write-Host "=== DETAILED PROJECT ANALYSIS FOR CONSOLIDATION ===" -ForegroundColor Yellow

# Analyze projects with Abstractions patterns
$abstractionProjects = @(
    'VisionaryCoder.Framework.Proxy.Interceptors.Security.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Correlation.Abstractions', 
    'VisionaryCoder.Framework.Proxy.Interceptors.Caching.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Auditing.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Logging.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Resilience.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Retry.Abstractions',
    'VisionaryCoder.Framework.Proxy.Interceptors.Telemetry.Abstractions',
    'VisionaryCoder.Framework.Secrets.Abstractions',
    'VisionaryCoder.Framework.Data.Abstractions',
    'VisionaryCoder.Framework.Services.Abstractions',
    'VisionaryCoder.Framework.Proxy.Abstractions'
)

Write-Host "`nAbstraction Projects Analysis:" -ForegroundColor Cyan
foreach ($project in $abstractionProjects) {
    $projectPath = "c:\Dev\Azure\temp\ifx\src\$project"
    if (Test-Path $projectPath) {
        $files = Get-ChildItem -Path $projectPath -Filter "*.cs" | Measure-Object
        Write-Host "  $project`: $($files.Count) files" -ForegroundColor White
        
        # Check if there's a corresponding implementation project
        $implProject = $project -replace '\.Abstractions$', ''
        $implPath = "c:\Dev\Azure\temp\ifx\src\$implProject"
        if (Test-Path $implPath -and $implProject -ne $project) {
            $implFiles = Get-ChildItem -Path $implPath -Filter "*.cs" | Measure-Object
            Write-Host "    -> Implementation: $implProject`: $($implFiles.Count) files" -ForegroundColor Gray
        }
    }
}

# Look for projects with similar names (potential duplicates)
Write-Host "`n=== PROJECTS WITH SIMILAR FUNCTIONALITY ===" -ForegroundColor Yellow

$allProjects = Get-ChildItem -Path "c:\Dev\Azure\temp\ifx\src" -Directory | Select-Object -ExpandProperty Name

# Group projects by base functionality
$projectGroups = @{
    'Configuration' = $allProjects | Where-Object { $_ -like "*Configuration*" }
    'KeyVault' = $allProjects | Where-Object { $_ -like "*KeyVault*" }
    'Security' = $allProjects | Where-Object { $_ -like "*Security*" }
    'Caching' = $allProjects | Where-Object { $_ -like "*Caching*" }
    'Logging' = $allProjects | Where-Object { $_ -like "*Logging*" }
    'Auditing' = $allProjects | Where-Object { $_ -like "*Auditing*" }
    'Correlation' = $allProjects | Where-Object { $_ -like "*Correlation*" }
    'Extensions' = $allProjects | Where-Object { $_ -like "*Extensions*" -and $_ -notlike "*Primitives*" }
    'Primitives' = $allProjects | Where-Object { $_ -like "*Primitives*" }
    'Secrets' = $allProjects | Where-Object { $_ -like "*Secrets*" }
    'Azure' = $allProjects | Where-Object { $_ -like "*Azure*" }
}

foreach ($group in $projectGroups.GetEnumerator()) {
    if ($group.Value.Count -gt 1) {
        Write-Host "`n$($group.Key) Related Projects:" -ForegroundColor Cyan
        foreach ($project in $group.Value) {
            $projectPath = "c:\Dev\Azure\temp\ifx\src\$project"
            $files = Get-ChildItem -Path $projectPath -Filter "*.cs" | Measure-Object
            Write-Host "  $project`: $($files.Count) files" -ForegroundColor White
        }
    }
}

# Check for empty or very small projects
Write-Host "`n=== SMALL PROJECTS (POTENTIAL CANDIDATES FOR CONSOLIDATION) ===" -ForegroundColor Yellow
$allProjects | ForEach-Object {
    $projectPath = "c:\Dev\Azure\temp\ifx\src\$_"
    $files = Get-ChildItem -Path $projectPath -Filter "*.cs"
    if ($files.Count -le 3) {
        Write-Host "$_`: $($files.Count) files" -ForegroundColor Red
        $files | ForEach-Object { Write-Host "    - $($_.Name)" -ForegroundColor Gray }
    }
}

Write-Host "`n=== RECOMMENDATIONS ===" -ForegroundColor Green
Write-Host "1. HIGH PRIORITY - Consolidate duplicate interfaces:" -ForegroundColor Yellow
Write-Host "   • Move all proxy-related abstractions to VisionaryCoder.Framework.Proxy.Abstractions" -ForegroundColor White
Write-Host "   • Eliminate individual .Abstractions projects for interceptors" -ForegroundColor White
Write-Host ""
Write-Host "2. MEDIUM PRIORITY - Secret provider consolidation:" -ForegroundColor Yellow
Write-Host "   • Choose between VisionaryCoder.Framework.Extensions.Configuration and VisionaryCoder.Framework.Secrets.Abstractions" -ForegroundColor White
Write-Host "   • Move ConnectionString to a single location" -ForegroundColor White
Write-Host ""
Write-Host "3. CONSIDER - Configuration project merge:" -ForegroundColor Yellow
Write-Host "   • VisionaryCoder.Framework.Extensions.Configuration and VisionaryCoder.Framework.Data.Configuration have overlapping purposes" -ForegroundColor White