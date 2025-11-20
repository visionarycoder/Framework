<#
Script: enforce_cs_style.ps1
Purpose: Traverse solution directories, for each .cs file collapse multi-line method/constructor parameter lists into a single line.
It groups files in batches (default 12 files per group), writes changes after each group and pauses 2s between groups larger than 12 files.

Usage: Run from repository root in PowerShell 7+: `pwsh ./tools/enforce_cs_style.ps1`
#>

param(
    [int]$BatchSize = 12,
    [switch]$DryRun
)

Write-Host "Starting enforcement script..."

$root = Get-Location
$csFiles = Get-ChildItem -Path $root -Recurse -Include *.cs -File | Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\" } | Sort-Object FullName

if ($csFiles.Count -eq 0) {
    Write-Host "No .cs files found. Exiting."
    exit 0
}

Write-Host ("Found {0} .cs files. Processing in batches of {1}..." -f $csFiles.Count, $BatchSize)

function Collapse-ParameterLines {
    param(
        [string[]]$lines
    )

    $i = 0
    $changed = $false
    while ($i -lt $lines.Count) {
        $line = $lines[$i]
        # Quick filter: interested in lines that contain '(' but not ')' and likely are method/constructor signatures
        if ($line -match "\(" -and $line -notmatch "\)" -and $line -match "\b(public|private|protected|internal|static|override|async|sealed|partial)\b") {
            $start = $i
            $acc = $line
            $j = $i + 1
            while ($j -lt $lines.Count -and $lines[$j] -notmatch "\)") {
                $acc += " " + $lines[$j].Trim()
                $j++
            }
            if ($j -lt $lines.Count) {
                # include the line that has the closing )
                $acc += " " + $lines[$j].Trim()
                # extract everything between first '(' and last ')'
                $firstParen = $acc.IndexOf('(')
                $lastParen = $acc.LastIndexOf(')')
                if ($firstParen -ge 0 -and $lastParen -gt $firstParen) {
                    $between = $acc.Substring($firstParen + 1, $lastParen - $firstParen - 1)
                    # If between contains newlines or multiple line breaks or parameter formatting, collapse
                    if ($between -match "\n|\r|,\s*\n") {
                        $collapsed = $between -replace '\s+', ' '
                        # Normalize spaces around commas
                        $collapsed = $collapsed -replace '\s*,\s*', ', '
                        # Rebuild the new single-line signature
                        $before = $acc.Substring(0, $firstParen + 1)
                        $after = $acc.Substring($lastParen)
                        $newLine = ($before + $collapsed + $after).Trim()
                        # Replace lines from start..j with newLine
                        $pre = @()
                        if ($start -gt 0) { $pre = $lines[0..($start - 1)] }
                        $post = @()
                        if ($j + 1 -le $lines.Count - 1) { $post = $lines[($j + 1)..($lines.Count - 1)] }
                        $lines = $pre + $newLine + $post
                        $changed = $true
                        # Move i to next line after the replaced one
                        $i = $start + 1
                        continue
                    }
                }
            }
        }
        $i++
    }
    return @{ Lines = $lines; Changed = $changed }
}

$groupIndex = 0
$total = $csFiles.Count
for ($groupStart = 0; $groupStart -lt $total; $groupStart += $BatchSize) {
    $groupIndex++
    $endIndex = [Math]::Min($groupStart + $BatchSize - 1, $total - 1)
    $group = $csFiles[$groupStart..$endIndex]
    Write-Host ("Processing group {0}: {1} files..." -f $groupIndex, $group.Count)

    foreach ($file in $group) {
        $path = $file.FullName
        Write-Host ("Processing {0}" -f $path)
        try {
            $orig = Get-Content -Raw -Encoding UTF8 -Path $path
        } catch {
            Write-Host ("Failed to read {0}: {1}" -f $path, $_.Exception.Message)
            continue
        }
        $lines = $orig -split "\r?\n"
        $result = Collapse-ParameterLines -lines $lines
        if ($result.Changed) {
            if ($DryRun) {
                Write-Host ("[DryRun] Would modify {0}" -f $path)
            } else {
                try {
                    # Do not create backups; user relies on source control
                    $result.Lines -join "`n" | Out-File -FilePath $path -Encoding UTF8
                    Write-Host ("Modified and saved {0}" -f $path)
                } catch {
                    Write-Host ("Failed to write {0}: {1}" -f $path, $_.Exception.Message)
                }
            }
        }
    }

    # After processing group, simulate "Save file groups"
    Write-Host ("Completed group {0}. Saved changes for this group." -f $groupIndex)

    # Pause 2 seconds between groups larger than 12 files
    if ($group.Count -gt 12) {
        Write-Host "Group size is greater than 12. Pausing 2 seconds..."
        Start-Sleep -Seconds 2
    }
}

Write-Host "All groups processed."
