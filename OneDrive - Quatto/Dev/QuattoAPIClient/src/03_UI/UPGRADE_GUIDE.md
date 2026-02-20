# Upgrade Guide - Quatto API Client for SSIS

> How to upgrade from previous versions to the latest release

---

## 📋 Quick Links

- [Upgrade from v0.x](#upgrade-from-v0x) - Initial migration
- [Upgrade from v1.0.x](#upgrade-from-v10x) - Minor version updates
- [Backup & Rollback](#backup--rollback) - Safety procedures
- [Troubleshooting](#troubleshooting) - Common issues

---

## ⚠️ Important Notes

### Requirements Before Upgrade

- ✅ SQL Server 2022 with SSIS v17.100
- ✅ Visual Studio 2022 18.3.1+
- ✅ .NET Framework 4.7.2+
- ✅ Administrator access to SSIS folder
- ✅ Backup of existing SSIS packages

### Compatibility Matrix

| Version | SSIS Version | SQL Server | .NET Framework | Status |
|---------|------------|-----------|----------------|--------|
| 1.0.0 | v17.100 | 2022 | 4.7.2+ | ✅ Current |
| 1.1.0 | v17.100 | 2022 | 4.7.2+ | ⏳ Planned |
| 2.0.0 | TBD | 2024+ | .NET 6+ | ⏳ Future |

---

## Upgrade from v0.x

### Phase 1: Prepare for Migration (30 minutes)

#### Step 1: Backup Current Configuration
```powershell
# Backup existing DLLs
$backupPath = "C:\Backups\SSIS\v0.x-$(Get-Date -Format 'yyyy-MM-dd-HHmmss')"
mkdir -Force $backupPath | Out-Null

$ssisBinn = "C:\Program Files\Microsoft SQL Server\160\DTS\Binn"
Copy-Item "$ssisBinn\QuattoAPIClient.*.dll" -Destination $backupPath -Force

Write-Host "✅ Backup created at: $backupPath"
```

#### Step 2: Document Current Configuration
```powershell
# Export SSIS packages configuration
# Take screenshots of all pipeline configurations
# Export any custom settings or scripts

# Document in migration checklist
```

#### Step 3: Review Breaking Changes

**Major Changes from v0.x to v1.0.0:**
- ✅ SSIS version now v17.100 (was v15.0.0)
- ✅ Logging architecture changed to Microsoft.Extensions.Logging
- ✅ New classes: LoggerFactory, LogScope
- ✅ New properties in components (may require reconfiguration)
- ✅ Authentication changes (OAuth2 refresh now automatic)

### Phase 2: Install v1.0.0 (15 minutes)

#### Step 1: Download Latest Version
```powershell
# Download from GitHub Releases or deployment
$version = "1.0.0"
$downloadPath = "C:\Downloads\QuattoAPIClient-$version.zip"
```

#### Step 2: Stop SSIS Service
```powershell
Stop-Service "MsDtsServer" -Force
Start-Sleep -Seconds 5
```

#### Step 3: Remove Old Version
```powershell
$ssisBinn = "C:\Program Files\Microsoft SQL Server\160\DTS\Binn"

# Remove old DLLs
Remove-Item "$ssisBinn\QuattoAPIClient.UI.dll" -Force -ErrorAction SilentlyContinue
Remove-Item "$ssisBinn\QuattoAPIClient.ConnectionManager.dll" -Force -ErrorAction SilentlyContinue
Remove-Item "$ssisBinn\QuattoAPIClient.Logging.dll" -Force -ErrorAction SilentlyContinue

Write-Host "✅ Old version removed"
```

#### Step 4: Install New Version
```powershell
# Extract new DLLs
Expand-Archive $downloadPath -DestinationPath "C:\Temp\QuattoAPIClient"

# Copy new DLLs
Copy-Item "C:\Temp\QuattoAPIClient\*.dll" -Destination $ssisBinn -Force

Write-Host "✅ New version installed"
```

#### Step 5: Start SSIS Service
```powershell
Start-Service "MsDtsServer"
Start-Sleep -Seconds 10

# Verify service started
Get-Service "MsDtsServer" | Select-Object Status
```

### Phase 3: Migrate Packages (1-2 hours)

#### Step 1: Open Packages in VS
```
1. Open SQL Server Data Tools (SSDT)
2. Open existing SSIS packages
3. For each package with Quatto components:
   - Right-click component
   - Select "Reset ComponentId" (if prompted)
   - Reconfigure component properties
```

#### Step 2: Reconfigure Components

**For Each Quatto Component:**

```
1. Double-click to open editor
2. Verify all properties are set:
   ✓ Connection Manager selection
   ✓ Base URL
   ✓ Endpoint
   ✓ Authentication type
   ✓ Page size
   ✓ Timeout
   ✓ Watermark (if incremental)
3. Click OK to save
```

#### Step 3: Test Packages

```powershell
# Run each package in VS
1. Right-click package → Execute Package
2. Verify successful execution
3. Check output data
4. Verify logs (check Output window)
```

#### Step 4: Deploy Updated Packages

```powershell
# Deploy to production
# Use SSIS Project deployment or Legacy package deployment
```

---

## Upgrade from v1.0.x

### For Minor Version Updates (e.g., 1.0.0 → 1.0.1)

#### Simple Upgrade Process

```powershell
# 1. Backup current DLLs
$backupPath = "C:\Backups\SSIS\v1.0.x-backup"
Copy-Item "C:\SSIS\Components\*.dll" -Destination $backupPath -Force

# 2. Stop SSIS service
Stop-Service "MsDtsServer" -Force
Start-Sleep -Seconds 5

# 3. Copy new DLLs
Copy-Item "C:\Downloads\QuattoAPIClient-1.0.1\*.dll" `
  -Destination "C:\Program Files\Microsoft SQL Server\160\DTS\Binn" -Force

# 4. Start SSIS service
Start-Service "MsDtsServer"
Start-Sleep -Seconds 5

# 5. Test
Get-Service "MsDtsServer" | Select-Object Status
```

**No package reconfiguration needed for minor versions!**

---

## Backup & Rollback

### Creating Backups

```powershell
# Comprehensive backup strategy

# 1. Backup DLLs
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmmss"
$backupDir = "C:\Backups\SSIS\$timestamp"
mkdir -Force $backupDir

Copy-Item "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient.*.dll" `
  -Destination $backupDir -Force

# 2. Backup SSIS packages
Copy-Item "C:\Program Files\Microsoft SQL Server\MSDB" `
  -Destination "$backupDir\MSDB" -Recurse -Force

# 3. Export registry settings
reg export "HKLM\Software\Microsoft\SSIS" `
  "$backupDir\SSIS-Registry.reg"

Write-Host "✅ Complete backup at: $backupDir"
```

### Rollback Procedure

```powershell
# If upgrade fails, rollback to previous version

# 1. Stop SSIS
Stop-Service "MsDtsServer" -Force

# 2. Remove current version
$ssisBinn = "C:\Program Files\Microsoft SQL Server\160\DTS\Binn"
Remove-Item "$ssisBinn\QuattoAPIClient.*.dll" -Force

# 3. Restore backup
$backupDir = "C:\Backups\SSIS\previous-version"
Copy-Item "$backupDir\*.dll" -Destination $ssisBinn -Force

# 4. Restart SSIS
Start-Service "MsDtsServer"
Start-Sleep -Seconds 10

Write-Host "✅ Rollback complete"
```

---

## Verification Checklist

After upgrade, verify:

- [ ] SSIS Service running
- [ ] Visual Studio opens without errors
- [ ] Quatto components appear in Toolbox
- [ ] Old packages open correctly
- [ ] Components can be added to pipelines
- [ ] Properties can be configured
- [ ] Test package executes successfully
- [ ] Logging output appears
- [ ] Data is correctly retrieved

---

## Troubleshooting

### Component Not Appearing in Toolbox

**Solution:**
```powershell
# 1. Clear SSIS cache
Remove-Item "$env:APPDATA\Microsoft\DataTransformationServices" -Recurse -Force

# 2. Restart Visual Studio
# 3. Rebuild SSIS project
# 4. Close and reopen Solution
```

### "DLL Not Found" Error

**Solution:**
```powershell
# 1. Verify DLLs are in correct location
Test-Path "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient.UI.dll"

# 2. Check file permissions
Get-Acl "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient.UI.dll"

# 3. Re-register if necessary
gacutil /i "C:\Program Files\Microsoft SQL Server\160\DTS\Binn\QuattoAPIClient.UI.dll"
```

### Package Compatibility Issues

**Solution:**
```
1. Right-click package → Upgrade
2. Visual Studio will attempt automatic migration
3. If issues remain:
   - Right-click component → Reset ComponentId
   - Reconfigure component
   - Save and test
```

---

## Getting Help

If you encounter upgrade issues:

1. **Check Documentation**
   - Review TROUBLESHOOTING.md
   - See INSTALLATION.md for setup issues

2. **Contact Support**
   - 📧 support@quatto.com.br
   - 🐛 GitHub Issues
   - 💬 GitHub Discussions

3. **Rollback**
   - If critical issues arise, use rollback procedure above
   - Report issue to support team

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for detailed version history.

---

**Last Updated:** 2026-02-20  
**Version:** 1.0.0

