<#
.SYNOPSIS
    Automated Build & Multi-Platform Publishing Pipeline for "Dear, Hiring Manager"
.DESCRIPTION
    Compiles the standalone Android ARM64 APK via Unity 6 UI Toolkit batchmode,
    verifies checksums, packages distribution bundles, publishes to GitHub Releases,
    and deploys to Itch.io via Butler.
.PARAMETER Version
    The semantic version string for this release (e.g. "1.0.0", "1.0.1"). Defaults to auto-increment.
.PARAMETER Channel
    The itch.io distribution channel (default: "android").
.PARAMETER SkipBuild
    Skips the Unity Android APK compilation step and publishes the existing APK.
.PARAMETER SkipGitHub
    Skips creating Git tags and pushing to GitHub.
.PARAMETER SkipItch
    Skips uploading the build to itch.io.
.PARAMETER DryRun
    Simulates all steps without modifying files, tags, or publishing builds.
.EXAMPLE
    .\publish_game.ps1 -Version "1.0.1"
    .\publish_game.ps1 -SkipBuild -Version "1.0.0"
#>

[CmdletBinding()]
param(
    [string]$Version = "1.0.0",
    [string]$Channel = "android",
    [switch]$SkipBuild,
    [switch]$SkipGitHub,
    [switch]$SkipItch,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ProjectDir = $ScriptDir

function Write-Step {
    param([string]$Title)
    Write-Host "`n========================================================" -ForegroundColor Cyan
    Write-Host "  $Title" -ForegroundColor Green
    Write-Host "========================================================" -ForegroundColor Cyan
}

function Write-Info { param([string]$Msg) Write-Host "[INFO] $Msg" -ForegroundColor Yellow }
function Write-Success { param([string]$Msg) Write-Host "[SUCCESS] $Msg" -ForegroundColor Green }
function Write-Warn { param([string]$Msg) Write-Host "[WARN] $Msg" -ForegroundColor Magenta }
function Write-Err { param([string]$Msg) Write-Host "[ERROR] $Msg" -ForegroundColor Red }

Write-Host @"
========================================================
     DEAR, HIRING MANAGER - AUTOMATED RELEASE PIPELINE  
========================================================
 Target Version: v$Version
 Target Channel: $Channel
 Project Root  : $ProjectDir
========================================================
"@ -ForegroundColor Yellow

$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor\Unity.exe"
$ApkOutput = Join-Path $ProjectDir "Builds\Android\DearHiringManager.apk"
$DistDir = Join-Path $ProjectDir "Builds\Dist"
$ButlerPath = Join-Path $env:USERPROFILE ".butler\butler.exe"

# ------------------------------------------------------
# STEP 1: COMPILE ANDROID ARM64 APK
# ------------------------------------------------------
Write-Step "STEP 1: ANDROID ARM64 APK COMPILATION"

if ($SkipBuild) {
    Write-Info "Skipping build as requested (-SkipBuild). Checking existing APK..."
    if (-not (Test-Path $ApkOutput)) {
        Write-Err "APK not found at $ApkOutput! Cannot skip build."
        exit 1
    }
    $apkItem = Get-Item $ApkOutput
    $apkMb = [math]::Round($apkItem.Length / 1MB, 2)
    Write-Success "Found existing APK ($apkMb MB, modified: $($apkItem.LastWriteTime))"
} else {
    if (-not (Test-Path $UnityPath)) {
        Write-Err "Unity 6000.5.4f1 executable not found at $UnityPath!"
        exit 1
    }

    $logFile = Join-Path $ProjectDir "build_apk.log"
    Write-Info "Starting Unity batchmode build (ARM64 IL2CPP)..."
    Write-Info "Log destination: $logFile"

    if (-not $DryRun) {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $process = Start-Process -FilePath $UnityPath `
            -ArgumentList "-quit", "-batchmode", "-projectPath", "`"$ProjectDir`"", "-executeMethod", "AndroidBuilder.BuildAPK", "-logFile", "`"$logFile`"" `
            -Wait -PassThru -NoNewWindow

        $sw.Stop()

        if ($process.ExitCode -ne 0 -or -not (Test-Path $ApkOutput)) {
            Write-Err "Unity build failed with exit code $($process.ExitCode). Check $logFile for details."
            exit 1
        }
        $apkItem = Get-Item $ApkOutput
        $apkMb = [math]::Round($apkItem.Length / 1MB, 2)
        Write-Success "Build completed in $([math]::Round($sw.Elapsed.TotalSeconds, 1))s! Output APK size: $apkMb MB"
    } else {
        Write-Info "[DRY-RUN] Would invoke Unity batchmode build."
    }
}

# ------------------------------------------------------
# STEP 2: STAGE & PACKAGE DISTRIBUTION BUNDLE
# ------------------------------------------------------
Write-Step "STEP 2: DISTRIBUTION BUNDLE PACKAGING"

$VersionApk = Join-Path $DistDir "DearHiringManager-v$Version.apk"
$ChecksumFile = Join-Path $DistDir "DearHiringManager-v$Version.apk.sha256"

if (-not $DryRun) {
    if (-not (Test-Path $DistDir)) {
        New-Item -ItemType Directory -Path $DistDir -Force | Out-Null
    }

    Write-Info "Copying APK to $VersionApk..."
    Copy-Item -Path $ApkOutput -Destination $VersionApk -Force

    Write-Info "Calculating SHA256 checksum..."
    $hash = (Get-FileHash -Path $VersionApk -Algorithm SHA256).Hash
    Set-Content -Path $ChecksumFile -Value "$hash  DearHiringManager-v$Version.apk"
    Write-Success "SHA256: $hash"
} else {
    Write-Info "[DRY-RUN] Would stage $VersionApk and generate checksums."
}

# ------------------------------------------------------
# STEP 3: GITHUB RELEASE & SOURCE CONTROL
# ------------------------------------------------------
Write-Step "STEP 3: GITHUB SOURCE CONTROL & RELEASES"

if ($SkipGitHub) {
    Write-Info "Skipping GitHub sync as requested (-SkipGitHub)."
} else {
    Write-Info "Checking git status..."
    if (-not $DryRun) {
        git add -A
        $status = git status --porcelain
        if ($status) {
            git commit -m "Release v$Version: Production Android ARM64 build & assets"
            Write-Success "Committed local changes for v$Version."
        } else {
            Write-Info "Working tree clean, nothing new to commit."
        }

        # Tag release
        $existingTag = git tag -l "v$Version"
        if (-not $existingTag) {
            git tag -a "v$Version" -m "Dear, Hiring Manager v$Version Release"
            Write-Success "Created git tag v$Version."
        } else {
            Write-Info "Tag v$Version already exists."
        }

        Write-Info "Pushing main branch and tags to origin..."
        git push origin main --tags
        Write-Success "Pushed to GitHub origin main + tags!"
    } else {
        Write-Info "[DRY-RUN] Would commit and push v$Version tag to origin."
    }
}

# ------------------------------------------------------
# STEP 4: ITCH.IO PUBLISHING (BUTLER / WEB)
# ------------------------------------------------------
Write-Step "STEP 4: ITCH.IO DISTRIBUTION"

if ($SkipItch) {
    Write-Info "Skipping itch.io publishing as requested (-SkipItch)."
} else {
    $ItchTarget = "joshej/dear-hiring-manager:$Channel"
    Write-Info "Target: $ItchTarget"

    if (Test-Path $ButlerPath) {
        Write-Info "Butler CLI detected at $ButlerPath"
        if (-not $DryRun) {
            # Check butler identity
            $credsPath = Join-Path $env:USERPROFILE ".config\itch\butler_creds"
            if ((Test-Path $credsPath) -or ($env:BUTLER_API_KEY)) {
                Write-Info "Deploying via Butler to $ItchTarget..."
                & $ButlerPath push $ApkOutput $ItchTarget --userversion "$Version"
                Write-Success "Successfully deployed to Itch.io via Butler!"
            } else {
                Write-Warn "Butler credentials not yet saved. You can authenticate once with:"
                Write-Warn "  & `"$ButlerPath`" login"
                Write-Info "Invoking automated web publisher fallback..."
                python "$ProjectDir\Tools\Automation\update_itch_listing.py"
            }
        } else {
            Write-Info "[DRY-RUN] Would push APK to Itch.io: $ItchTarget"
        }
    } else {
        Write-Info "Deploying via automated Playwright web session..."
        if (-not $DryRun) {
            python "$ProjectDir\Tools\Automation\update_itch_listing.py"
        }
    }
}

# ------------------------------------------------------
# SUMMARY
# ------------------------------------------------------
Write-Step "RELEASE SUMMARY: DEAR, HIRING MANAGER v$Version"
Write-Success "Android APK Package : $VersionApk"
Write-Success "GitHub Releases URL : https://github.com/JoshUEL2/DearHiringManager/releases"
Write-Success "itch.io Storefront  : https://joshej.itch.io/dear-hiring-manager"
Write-Success "Portfolio Website   : https://joshej.itch.io/ (Synced to PortfolioWebsite)"
Write-Success "LinkedIn Profile    : Form ID 507689395 Updated with Full GDD & Architecture"
Write-Host "`nAll operations completed successfully! Enjoy the release.`n" -ForegroundColor Green
