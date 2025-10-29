# Get all C# files
$files = Get-ChildItem -Path "c:\Dev\GitHub\VisionaryCoder\vc\src" -Recurse -Filter "*.cs"

foreach ($file in $files) {
    # Get the relative path from src folder
    $relativePath = $file.FullName.Substring((Resolve-Path "c:\Dev\GitHub\VisionaryCoder\vc\src").Path.Length + 1)
    
    # Convert file path to expected namespace
    $pathParts = $relativePath -split "\\"
    
    # Remove the filename part and join with dots
    $expectedNamespace = ($pathParts[0..($pathParts.Length-2)] -join ".").Replace("/", ".")
    
    # Read the file content
    $content = Get-Content $file.FullName -Raw
    
    if ($content -match "^\s*namespace\s+([^;\s]+)") {
        $currentNamespace = $matches[1]
        
        # Only update if namespace doesn't match expected
        if ($currentNamespace -ne $expectedNamespace -and $expectedNamespace -ne "") {
            Write-Host "File: $($file.FullName)"
            Write-Host "  Current: $currentNamespace"
            Write-Host "  Expected: $expectedNamespace"
            Write-Host ""
            
            # Replace the namespace
            $newContent = $content -replace "^(\s*)namespace\s+[^;\s]+", "`$1namespace $expectedNamespace"
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
        }
    }
}
