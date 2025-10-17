# Analyze C# files for duplicate class names across different namespaces
$results = @{}
$duplicates = @()

Write-Host "Scanning C# files for class definitions..." -ForegroundColor Green

Get-ChildItem -Path "c:\Dev\Azure\temp\ifx\src" -Recurse -Filter "*.cs" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
    if ($content) {
        # Extract namespace
        $namespaceMatch = [regex]::Match($content, 'namespace\s+([^\s\{\r\n]+)')
        $namespace = if ($namespaceMatch.Success) { $namespaceMatch.Groups[1].Value.Trim() } else { "Global" }
        
        # Extract class, interface, record, struct declarations (more precise regex)
        $typePattern = '(?:^|\s)(?:public\s+|internal\s+|private\s+|protected\s+)?(?:static\s+|sealed\s+|abstract\s+|partial\s+)*(?:class|interface|record|struct|enum)\s+(\w+)(?:\s*<[^>]*>)?(?:\s*:\s*[^\{\r\n]*)?(?:\s*where[^\{\r\n]*)*\s*\{'
        $typeMatches = [regex]::Matches($content, $typePattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)
        
        foreach ($match in $typeMatches) {
            $typeName = $match.Groups[1].Value
            
            # Skip common keywords that might be matched incorrectly
            if ($typeName -notmatch '^(for|while|if|else|using|var|return|new|this|base|get|set|value|where|select|from|to|in|on|by|into|join|let|orderby|group|with|async|await|yield|when|case|default|try|catch|finally|throw|typeof|sizeof|is|as|namespace|class|interface|struct|record|enum|delegate|event|operator|explicit|implicit|override|virtual|abstract|sealed|static|readonly|const|volatile|extern|unsafe|fixed|lock|checked|unchecked|stackalloc)$') {
                
                if (-not $results.ContainsKey($typeName)) {
                    $results[$typeName] = @()
                }
                
                $results[$typeName] += [PSCustomObject]@{
                    Namespace = $namespace
                    FilePath = $_.FullName
                    FileName = $_.Name
                    ProjectName = ($_.Directory.Name -replace '\.cs$', '')
                }
            }
        }
    }
}

Write-Host "`nAnalyzing for duplicates..." -ForegroundColor Green

# Find duplicates (classes with same name in different namespaces/projects)
foreach ($className in $results.Keys) {
    $occurrences = $results[$className]
    if ($occurrences.Count -gt 1) {
        $uniqueNamespaces = ($occurrences | Select-Object -Property Namespace -Unique).Count
        $uniqueProjects = ($occurrences | Select-Object -Property ProjectName -Unique).Count
        
        if ($uniqueNamespaces -gt 1 -or $uniqueProjects -gt 1) {
            $duplicates += [PSCustomObject]@{
                ClassName = $className
                OccurrenceCount = $occurrences.Count
                UniqueNamespaces = $uniqueNamespaces
                UniqueProjects = $uniqueProjects
                Occurrences = $occurrences
            }
        }
    }
}

# Display results
Write-Host "`n=== DUPLICATE CLASS NAMES ANALYSIS ===" -ForegroundColor Yellow
Write-Host "Found $($duplicates.Count) classes with duplicate names across different namespaces/projects`n" -ForegroundColor Cyan

$duplicates | Sort-Object ClassName | ForEach-Object {
    Write-Host "Class: $($_.ClassName)" -ForegroundColor Red
    Write-Host "  Total Occurrences: $($_.OccurrenceCount)" -ForegroundColor White
    Write-Host "  Across $($_.UniqueNamespaces) namespace(s) and $($_.UniqueProjects) project(s)" -ForegroundColor White
    
    $_.Occurrences | ForEach-Object {
        $projectName = $_.ProjectName
        Write-Host "    • $($_.Namespace) -> $projectName ($($_.FileName))" -ForegroundColor Gray
    }
    Write-Host ""
}

# Summary for consolidation planning
Write-Host "=== CONSOLIDATION CANDIDATES ===" -ForegroundColor Yellow
$consolidationCandidates = $duplicates | Where-Object { $_.UniqueProjects -gt 1 } | Sort-Object @{Expression={$_.UniqueProjects}; Descending=$true}

if ($consolidationCandidates.Count -gt 0) {
    Write-Host "Classes that appear in multiple projects (highest priority for consolidation):`n" -ForegroundColor Cyan
    
    $consolidationCandidates | ForEach-Object {
        Write-Host "$($_.ClassName): appears in $($_.UniqueProjects) projects" -ForegroundColor Red
        $projects = ($_.Occurrences | Select-Object -Property ProjectName -Unique | Sort-Object ProjectName).ProjectName
        Write-Host "  Projects: $($projects -join ', ')" -ForegroundColor Gray
        Write-Host ""
    }
} else {
    Write-Host "No classes found in multiple projects. Duplicates are within same project but different namespaces." -ForegroundColor Green
}

# Projects with most duplicates
Write-Host "=== PROJECTS WITH MOST INTERNAL DUPLICATES ===" -ForegroundColor Yellow
$projectDuplicates = @{}
foreach ($duplicate in $duplicates) {
    foreach ($occurrence in $duplicate.Occurrences) {
        if (-not $projectDuplicates.ContainsKey($occurrence.ProjectName)) {
            $projectDuplicates[$occurrence.ProjectName] = 0
        }
        $projectDuplicates[$occurrence.ProjectName]++
    }
}

$projectDuplicates.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
    if ($_.Value -gt 1) {
        Write-Host "$($_.Key): $($_.Value) duplicate class instances" -ForegroundColor Cyan
    }
}