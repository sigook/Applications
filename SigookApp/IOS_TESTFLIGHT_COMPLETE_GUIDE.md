# iOS TestFlight Deployment - Complete Guide

## ✅ What's Already Done (In Codebase)

### 1. Pipeline Configuration ✅
- **Stage 5:** iOS Staging Build (dev branch)
- **Stage 6:** iOS Production Build (main/SigookApp branch)
- **Stage 7:** TestFlight Deployment (automatic upload)

### 2. Codebase Updates ✅
- ✅ iOS bundle identifier: `com.sigook.beta`
- ✅ ExportOptions-Staging.plist configured
- ✅ ExportOptions-Production.plist configured
- ✅ Auto-incrementing build number using `Build.BuildId`
- ✅ Flavor-specific builds (staging, production)

### 3. Version Management ✅
- Same as Android - version code auto-increments
- `pubspec.yaml` stays at `1.0.0+1`
- Pipeline overrides with `--build-number=$(Build.BuildId)`

---

## 🔧 Required Setup (Outside Codebase)

### **STEP 1: Apple Developer Account Requirements**

#### A. Create App ID in Apple Developer Portal
1. Go to: [developer.apple.com](https://developer.apple.com)
2. Navigate to: **Certificates, Identifiers & Profiles**
3. Click: **Identifiers** → **+** (Add)
4. Select: **App IDs** → **Continue**
5. Configure:
   - **Bundle ID:** `com.sigook.beta`
   - **Description:** Sigook App
   - **Capabilities:** Enable what you need:
     - ✅ Push Notifications
     - ✅ Sign in with Apple (if using)
     - ✅ Associated Domains
     - ✅ Background Modes
     - ✅ Any other needed capabilities
6. Click **Continue** → **Register**

#### B. Create Distribution Certificate (.p12 file)
This certificate signs your app for distribution.

**On Mac (required):**

1. **Open Keychain Access**
   - Applications → Utilities → Keychain Access

2. **Request Certificate:**
   - Menu: Keychain Access → Certificate Assistant → Request a Certificate from a Certificate Authority
   - **User Email:** Your Apple ID email
   - **Common Name:** Your name or company
   - **CA Email:** Leave blank
   - **Request:** Saved to disk
   - Click **Continue** → Save the `.certSigningRequest` file

3. **Create Distribution Certificate:**
   - Go to: [developer.apple.com](https://developer.apple.com) → Certificates
   - Click **+** (Add)
   - Select: **Apple Distribution**
   - Click **Continue**
   - Upload your `.certSigningRequest` file
   - Click **Continue** → **Download** the certificate

4. **Install Certificate:**
   - Double-click downloaded certificate (.cer file)
   - It installs in Keychain Access

5. **Export as .p12:**
   - In Keychain Access → **My Certificates**
   - Find: **Apple Distribution: [Your Name]**
   - **Right-click** → **Export**
   - Format: **Personal Information Exchange (.p12)**
   - Save with a **strong password** (you'll need this!)
   - **Remember this password** - needed for Azure DevOps

**Important:** Keep the .p12 file and password secure!

#### C. Create Provisioning Profiles

You need **2 provisioning profiles:**
1. **Staging** (Ad Hoc) - for internal testing
2. **Production** (App Store) - for TestFlight/App Store

**For Each Profile:**

1. Go to: [developer.apple.com](https://developer.apple.com) → **Profiles**
2. Click **+** (Add)

**Staging Profile (Ad Hoc):**
- Select: **Ad Hoc**
- App ID: `com.sigook.beta`
- Certificate: Select your Distribution certificate
- Devices: Select test devices (must register device UDIDs first)
- Profile Name: `Sigook App Staging Ad Hoc`
- Download the `.mobileprovision` file

**Production Profile (App Store):**
- Select: **App Store**
- App ID: `com.sigook.beta`
- Certificate: Select your Distribution certificate
- Profile Name: `Sigook App App Store`
- Download the `.mobileprovision` file

---

### **STEP 2: App Store Connect Setup**

#### A. Create App in App Store Connect
1. Go to: [appstoreconnect.apple.com](https://appstoreconnect.apple.com)
2. Click: **My Apps** → **+** → **New App**
3. Fill in:
   - **Platform:** iOS
   - **Name:** Sigook
   - **Primary Language:** English
   - **Bundle ID:** Select `com.sigook.beta`
   - **SKU:** `com.sigook.beta` (or unique identifier)
   - **User Access:** Full Access
4. Click **Create**

#### B. Configure App Information
Go to: App Store Connect → Your App → **App Information**

**Required Fields:**
- **Category:** Choose appropriate (e.g., Business, Productivity)
- **Content Rights:** Declare if needed
- **Age Rating:** Complete questionnaire
- **Privacy Policy URL:** (required before submission)
- **Support URL:** (required before submission)

#### C. Create App Store Connect API Key

This allows Azure Pipeline to upload to TestFlight automatically.

1. Go to: [appstoreconnect.apple.com](https://appstoreconnect.apple.com)
2. Click: **Users and Access** → **Keys** tab
3. Click **+** (Generate API Key)
4. Fill in:
   - **Name:** `Azure DevOps CI/CD`
   - **Access:** **App Manager** (or higher)
5. Click **Generate**
6. **Download API Key** (`.p8` file) - **can only download once!**
7. **Copy these values** (needed for Azure DevOps):
   - **Issuer ID** (at top of page)
   - **Key ID** (in the list)
   - **Key file content** (open `.p8` file, copy text)

**Warning:** The .p8 key can only be downloaded once! Keep it secure.

---

### **STEP 3: Azure DevOps Configuration**

#### A. Upload Secure Files

Go to: Azure DevOps → Pipelines → Library → **Secure files**

Upload these files:

1. **Distribution Certificate (.p12)**
   - File name: e.g., `sigook-distribution.p12`
   - Note the exact filename for later

2. **Provisioning Profile - Staging (.mobileprovision)**
   - File name: e.g., `sigook-staging.mobileprovision`
   - Note the exact filename

3. **Provisioning Profile - Production (.mobileprovision)**
   - File name: e.g., `sigook-production.mobileprovision`
   - Note the exact filename

#### B. Create Variable Group: `SigookApp-iOS-Signing`

Go to: Pipelines → Library → **Variable groups** → **+ Variable group**

**Group name:** `SigookApp-iOS-Signing`

**Add these variables:**

| Variable Name | Value | Secret? |
|--------------|-------|---------|
| `P12_FILE_NAME` | `sigook-distribution.p12` | No |
| `P12_PASSWORD` | [Your .p12 password] | **Yes** 🔒 |
| `PROVISIONING_PROFILE_STAGING` | `sigook-staging.mobileprovision` | No |
| `PROVISIONING_PROFILE_PRODUCTION` | `sigook-production.mobileprovision` | No |
| `APP_STORE_CONNECT_API_KEY_ID` | [Key ID from Step 2C] | No |
| `APP_STORE_CONNECT_ISSUER_ID` | [Issuer ID from Step 2C] | No |
| `APP_STORE_CONNECT_API_KEY_CONTENT` | [Content of .p8 file] | **Yes** 🔒 |
| `APPLE_TEAM_ID` | [Your Team ID] | No |
| `APPLE_TEAM_NAME` | [Your Team Name] | No |

**How to find Team ID and Name:**
- Team ID: developer.apple.com → Account → Membership → Team ID
- Team Name: Same page, Team Name field

**Click Save**

#### C. Create Environment: `TestFlight-Production`

Go to: Pipelines → **Environments** → **New environment**

**Configuration:**
- **Name:** `TestFlight-Production` (exact match)
- **Description:** iOS TestFlight production deployment
- **Resource:** None
- Click **Create**

**Optional - Add Approvals:**
1. Click environment → **⋯** → **Approvals and checks**
2. Add **Approvals** check
3. Add yourself as approver
4. This creates manual approval gate before TestFlight upload

---

### **STEP 4: Update ExportOptions Plist Files**

Already updated in codebase, but verify:

**`ios/ExportOptions-Staging.plist`:**
- Bundle ID: `com.sigook.beta` ✅
- Team ID: Update line 12 with your actual Team ID

**`ios/ExportOptions-Production.plist`:**
- Bundle ID: `com.sigook.beta` ✅
- Team ID: Update line 12 with your actual Team ID

**Update Team ID:**
```xml
<key>teamID</key>
<string>YOUR_ACTUAL_TEAM_ID</string>
```

Replace `NGXFWU4H7Z` with your Apple Team ID from developer.apple.com.

---

### **STEP 5: Register Test Devices (For Staging)**

To install staging builds on physical devices:

1. Get device UDID:
   - Connect iPhone to Mac
   - Open Finder → Device → Click device name
   - Copy **Serial Number** → **Cmd+V** reveals UDID

2. Register in Apple Developer Portal:
   - developer.apple.com → **Devices** → **+**
   - Name: e.g., "John's iPhone"
   - UDID: Paste UDID
   - Click **Continue** → **Register**

3. **Regenerate Staging Provisioning Profile:**
   - After adding devices, you must regenerate the profile
   - developer.apple.com → Profiles → Staging profile
   - Click **Edit** → Select new devices → **Generate**
   - Download new `.mobileprovision` file
   - **Re-upload to Azure DevOps Secure Files** (replace old one)

---

## 📋 Codebase Changes Needed

### ✅ Already Done:
- ✅ Bundle identifier updated to `com.sigook.beta`
- ✅ ExportOptions files configured
- ✅ Pipeline stages added

### 🔧 **TO DO - Update Team IDs:**

**File: `ios/ExportOptions-Staging.plist` (Line 12)**
```xml
<key>teamID</key>
<string>YOUR_TEAM_ID_HERE</string>
```

**File: `ios/ExportOptions-Production.plist` (Line 12)**
```xml
<key>teamID</key>
<string>YOUR_TEAM_ID_HERE</string>
```

Replace with your actual Apple Team ID.

**That's the only codebase change needed!**

---

## 🚀 Testing iOS Deployment

### First Build Test (Staging - Dev Branch)

1. **Commit and push to `dev` branch:**
   ```powershell
   git checkout dev
   git add .
   git commit -m "Configure iOS pipeline"
   git push
   ```

2. **Monitor pipeline:**
   - Azure DevOps → Pipelines → SigookApp
   - Watch stages:
     1. Validate & Test ✅
     2. Build Android Staging ✅
     3. **Build iOS Staging** ⏳ (~15-20 min on macOS agent)

3. **Download IPA:**
   - Pipeline run → Artifacts → `sigookapp-ios-staging`
   - Download IPA file

4. **Install on test device:**
   - Use TestFlight, Diawi, or direct install
   - Device UDID must be in provisioning profile

### Production Deploy (main/SigookApp Branch)

1. **Push to `main` or `SigookApp`:**
   ```powershell
   git checkout main  # or SigookApp
   git push
   ```

2. **Pipeline runs:**
   - Stage 1: Validate & Test
   - Stage 2: Build Android Production
   - Stage 3: Deploy to Play Store
   - Stage 4: **Build iOS Production**
   - Stage 5: ⏸️ **Wait for approval** (if configured)
   - Stage 6: **Deploy to TestFlight** ✅

3. **Verify in App Store Connect:**
   - Wait 5-15 minutes for processing
   - App Store Connect → TestFlight
   - Build should appear with build number = Build.BuildId

---

## 🎯 Complete Setup Checklist

### Apple Developer Portal
- [ ] App ID created: `com.sigook.beta`
- [ ] Distribution certificate created (.cer)
- [ ] Certificate exported as .p12 with password
- [ ] Staging provisioning profile created (Ad Hoc)
- [ ] Production provisioning profile created (App Store)
- [ ] Test devices registered (for staging)

### App Store Connect
- [ ] App created with bundle ID: `com.sigook.beta`
- [ ] App information filled
- [ ] API Key generated (.p8 file)
- [ ] API Key ID copied
- [ ] Issuer ID copied
- [ ] Team ID and Team Name noted

### Azure DevOps - Secure Files
- [ ] Distribution certificate (.p12) uploaded
- [ ] Staging provisioning profile (.mobileprovision) uploaded
- [ ] Production provisioning profile (.mobileprovision) uploaded

### Azure DevOps - Variable Groups
- [ ] `SigookApp-iOS-Signing` variable group created
- [ ] All 9 variables configured (see STEP 3B)
- [ ] Secrets marked as secret (P12_PASSWORD, API_KEY_CONTENT)

### Azure DevOps - Environments
- [ ] `TestFlight-Production` environment created
- [ ] Approval gates configured (optional but recommended)

### Codebase
- [ ] Team ID updated in `ios/ExportOptions-Staging.plist`
- [ ] Team ID updated in `ios/ExportOptions-Production.plist`
- [ ] Changes committed and pushed

### Testing
- [ ] Staging build tested (dev branch)
- [ ] IPA successfully generated
- [ ] Production build tested (main/SigookApp branch)
- [ ] TestFlight upload successful
- [ ] Build appears in App Store Connect

---

## 🔍 Common iOS Issues & Solutions

### Issue 1: "Certificate not found in keychain"
**Cause:** .p12 file or password incorrect in Azure DevOps  
**Solution:**
- Verify `P12_PASSWORD` in variable group
- Re-export .p12 from Keychain Access
- Re-upload to Secure Files

### Issue 2: "No matching provisioning profile"
**Cause:** Profile doesn't match bundle ID or certificate  
**Solution:**
- Verify bundle ID in profile matches `com.sigook.beta`
- Ensure profile uses correct distribution certificate
- Re-download and re-upload .mobileprovision file

### Issue 3: "Device not registered" (Staging)
**Cause:** Device UDID not in provisioning profile  
**Solution:**
- Register device UDID in Apple Developer Portal
- **Regenerate** staging provisioning profile
- Re-upload to Azure DevOps

### Issue 4: "Export failed: Archive not found"
**Cause:** Flutter build failed but wasn't caught  
**Solution:**
- Check Flutter build logs
- Look for Swift compilation errors
- Check for plugin compatibility issues

### Issue 5: "API Key invalid" (TestFlight upload)
**Cause:** API Key variables incorrect in Azure DevOps  
**Solution:**
- Verify `APP_STORE_CONNECT_API_KEY_ID`
- Verify `APP_STORE_CONNECT_ISSUER_ID`
- Verify `APP_STORE_CONNECT_API_KEY_CONTENT` (full .p8 content)
- Ensure key has "App Manager" access

### Issue 6: "Build processing stuck in App Store Connect"
**Cause:** Normal - Apple processes builds  
**Solution:**
- Wait 5-30 minutes (sometimes longer)
- Check email for errors from Apple
- Refresh App Store Connect periodically

### Issue 7: "Signing certificate expired"
**Cause:** Distribution certificate expired (1 year validity)  
**Solution:**
- Create new distribution certificate
- Update all provisioning profiles
- Export new .p12
- Upload to Azure DevOps

---

## 📊 iOS vs Android Comparison

| Aspect | Android (Play Store) | iOS (TestFlight/App Store) |
|--------|---------------------|---------------------------|
| **Account Cost** | $25 one-time | $99/year |
| **Build Agent** | ubuntu-latest | macos-latest |
| **Build Time** | ~10-11 min | ~15-20 min |
| **Signing** | Keystore (.keystore) | Certificate (.p12) + Profile (.mobileprovision) |
| **Distribution** | AAB only | IPA |
| **Testing Track** | Internal → Alpha → Beta → Production | TestFlight → App Store |
| **Upload Task** | GooglePlayRelease@4 | AppStoreRelease@1 |
| **API Auth** | Service Account JSON | API Key (.p8) |
| **Auto-deploy** | ✅ Yes | ✅ Yes |
| **Approval Gates** | ✅ PlayStore-Production | ✅ TestFlight-Production |
| **Version Management** | Auto Build.BuildId | Auto Build.BuildId |

---

## 🎯 Quick Reference

### Pipeline Triggers

| Branch | Android | iOS | Deployment |
|--------|---------|-----|-----------|
| `dev` | Staging APK | Staging IPA | None |
| `main` | Production AAB | Production IPA | Play Store + TestFlight |
| `SigookApp` | Production AAB | Production IPA | Play Store + TestFlight |
| PR to `dev` | Validation only | Validation only | None |

### Variable Groups Needed

1. **SigookApp-Staging** (Android + iOS env vars)
2. **SigookApp-Production** (Android + iOS env vars)
3. **SigookApp-Android-Signing** (keystore credentials)
4. **SigookApp-iOS-Signing** (certificates + API keys) ← **NEW**

### Environments

1. **PlayStore-Production** (Android deployment)
2. **TestFlight-Production** (iOS deployment) ← **NEW**

### Secure Files Needed

**Android:**
- `sigook-release.keystore`
- `playstore-service-account.json`

**iOS (NEW):**
- `sigook-distribution.p12`
- `sigook-staging.mobileprovision`
- `sigook-production.mobileprovision`

---

## 💡 Pro Tips

### 1. Certificate Management
- Distribution certificates expire after **1 year**
- Set calendar reminder 1 month before expiration
- Keep backup of .p12 file in secure location
- Document the password in password manager

### 2. Provisioning Profile Updates
- Profiles expire with certificate
- Adding new devices requires profile regeneration
- Always test after updating profiles

### 3. TestFlight Testing
- Can have up to 10,000 external testers
- Internal testers (up to 100) get builds immediately
- External testers require app review for first build
- Subsequent builds skip review if no major changes

### 4. Build Numbers
- iOS and Android share same build number (Build.BuildId)
- Starts at 1, auto-increments
- Never reuse build numbers

### 5. Fastlane Alternative
Current pipeline uses native Azure tasks. If you need more control:
- Consider Fastlane for both iOS and Android
- More complex but more flexible
- Better for advanced scenarios

---

## ✅ Summary

### What's Complete in Pipeline:
1. ✅ iOS staging builds (dev branch)
2. ✅ iOS production builds (main/SigookApp branch)
3. ✅ Auto-incrementing build numbers
4. ✅ TestFlight automatic upload
5. ✅ Approval gates support
6. ✅ Parallel with Android pipeline

### What You Need to Do:
1. **Apple Developer Portal:** Create certificates, profiles (30-60 min)
2. **App Store Connect:** Create app, API key (15-30 min)
3. **Azure DevOps:** Upload files, configure variables (15 min)
4. **Codebase:** Update Team IDs in ExportOptions files (2 min)
5. **Test:** Push to dev, verify builds work

**Total Setup Time: 2-3 hours** (mostly Apple account setup)

### After Setup:
- Push to `dev` → Builds staging APK + IPA
- Push to `main`/`SigookApp` → Builds production, deploys to Play Store + TestFlight
- Fully automated mobile CI/CD! 🎉

---

**Need Help?** Check Azure Pipeline logs for specific errors. Most issues are certificate/provisioning profile related.
