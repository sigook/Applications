# Azure DevOps Complete Setup Checklist
## SigookApp CI/CD Pipeline - Ready to Deploy

## ✅ Pipeline Status: COMPLETE

All 7 stages are fully implemented in the pipeline:
- ✅ **Stage 1:** Validate & Test (Flutter analyze, tests)
- ✅ **Stage 2:** Build Android Staging (APK for dev branch)
- ✅ **Stage 3:** Build Android Production (AAB for main branch)
- ✅ **Stage 4:** Deploy to Play Store (Automatic upload)
- ✅ **Stage 5:** Build iOS Staging (IPA for dev branch)
- ✅ **Stage 6:** Build iOS Production (IPA for main branch)
- ✅ **Stage 7:** Deploy to TestFlight (Automatic upload)

**No TODO items remain in the pipeline YAML file.**

---

## 🔧 What You Need to Configure in Azure DevOps

The pipeline code is complete, but you need to configure Azure DevOps with your credentials and files.

### **Step 1: Create Variable Groups**

#### Variable Group 1: `SigookApp-Staging`
Go to: **Pipelines → Library → Variable groups → + Variable group**

| Variable Name | Example Value | Secret? |
|--------------|---------------|---------|
| `AUTH_AUTHORITY` | `https://auth.staging.sigook.com` | No |
| `API_BASE_URL` | `https://api.staging.sigook.com` | No |
| `CLIENT_ID` | `your-staging-client-id` | No |
| `REDIRECT_URI` | `com.sigook.staging://oauth/callback` | No |
| `POST_LOGOUT_REDIRECT_URI` | `com.sigook.staging://logout` | No |
| `SCOPES` | `openid profile email offline_access api` | No |
| `APP_NAME` | `Sigook Staging` | No |

#### Variable Group 2: `SigookApp-Production`
Same structure as staging, but with production values:

| Variable Name | Example Value | Secret? |
|--------------|---------------|---------|
| `AUTH_AUTHORITY` | `https://auth.sigook.com` | No |
| `API_BASE_URL` | `https://api.sigook.com` | No |
| `CLIENT_ID` | `your-production-client-id` | No |
| `REDIRECT_URI` | `com.sigook.beta://oauth/callback` | No |
| `POST_LOGOUT_REDIRECT_URI` | `com.sigook.beta://logout` | No |
| `SCOPES` | `openid profile email offline_access api` | No |
| `APP_NAME` | `Sigook` | No |

#### Variable Group 3: `SigookApp-Android-Signing`

| Variable Name | Value | Secret? |
|--------------|-------|---------|
| `KEYSTORE_PASSWORD` | Your keystore password | **Yes** 🔒 |
| `KEY_PASSWORD` | Your key password | **Yes** 🔒 |
| `KEY_ALIAS` | Your key alias (e.g., `sigook-key`) | No |

#### Variable Group 4: `SigookApp-iOS-Signing`

| Variable Name | Value | Secret? |
|--------------|-------|---------|
| `P12_FILE_NAME` | `sigook-distribution.p12` | No |
| `P12_PASSWORD` | Your .p12 password | **Yes** 🔒 |
| `PROVISIONING_PROFILE_STAGING` | `sigook-staging.mobileprovision` | No |
| `PROVISIONING_PROFILE_PRODUCTION` | `sigook-production.mobileprovision` | No |
| `APP_STORE_CONNECT_API_KEY_ID` | Your API Key ID | No |
| `APP_STORE_CONNECT_ISSUER_ID` | Your Issuer ID | No |
| `APP_STORE_CONNECT_API_KEY_CONTENT` | Content of .p8 file | **Yes** 🔒 |
| `APPLE_TEAM_ID` | Your Team ID | No |
| `APPLE_TEAM_NAME` | Your Team Name | No |

---

### **Step 2: Upload Secure Files**

Go to: **Pipelines → Library → Secure files**

Upload these files:

#### Android Files:
1. **`sigook-release.keystore`**
   - Your Android keystore file
   - Used for signing APK/AAB

#### iOS Files:
2. **`sigook-distribution.p12`** (or your chosen filename)
   - Apple Distribution certificate
   - Export from Keychain Access on Mac

3. **`sigook-staging.mobileprovision`** (or your chosen filename)
   - Ad Hoc provisioning profile for staging

4. **`sigook-production.mobileprovision`** (or your chosen filename)
   - App Store provisioning profile for production

#### Play Store File:
5. **`playstore-service-account.json`**
   - Google Play Console service account JSON
   - For automated Play Store uploads

**After uploading:** Authorize each secure file for your pipeline:
- Click each file → **Pipeline permissions** → Add `sigookapp-pipeline`

---

### **Step 3: Create Environments**

Go to: **Pipelines → Environments → New environment**

#### Environment 1: `PlayStore-Production`
- **Name:** `PlayStore-Production` (exact match)
- **Description:** Android Play Store deployment
- **Optional:** Add approval checks

#### Environment 2: `TestFlight-Production`
- **Name:** `TestFlight-Production` (exact match)
- **Description:** iOS TestFlight deployment
- **Optional:** Add approval checks

**Why environments?** They provide:
- Deployment history tracking
- Manual approval gates (optional)
- Environment-specific variables
- Audit logs

---

### **Step 4: Create or Verify Service Connections** (Optional)

If using service connections instead of secure files:

Go to: **Project Settings → Service connections**

#### For Google Play:
- Type: **Google Play**
- Connection name: `GooglePlay-SigookApp`
- Upload service account JSON

#### For App Store:
- Type: **App Store**
- Connection name: `AppStore-SigookApp`
- Configure API key

*Note: Current pipeline uses secure files directly, so service connections are optional.*

---

## 📋 Quick Setup Checklist

### Azure DevOps Configuration:
- [ ] Variable Group `SigookApp-Staging` created with 7 variables
- [ ] Variable Group `SigookApp-Production` created with 7 variables
- [ ] Variable Group `SigookApp-Android-Signing` created with 3 variables
- [ ] Variable Group `SigookApp-iOS-Signing` created with 9 variables
- [ ] Secure file: `sigook-release.keystore` uploaded
- [ ] Secure file: `sigook-distribution.p12` uploaded
- [ ] Secure file: `sigook-staging.mobileprovision` uploaded
- [ ] Secure file: `sigook-production.mobileprovision` uploaded
- [ ] Secure file: `playstore-service-account.json` uploaded
- [ ] All secure files authorized for pipeline
- [ ] Environment `PlayStore-Production` created
- [ ] Environment `TestFlight-Production` created

### Apple Prerequisites:
- [ ] Apple Developer account active
- [ ] App ID `com.sigook.beta` created
- [ ] Distribution certificate generated
- [ ] Provisioning profiles created (staging + production)
- [ ] App created in App Store Connect
- [ ] App Store Connect API key generated (.p8 file)
- [ ] Test devices registered (for staging builds)
- [ ] Team ID updated in ExportOptions plist files

### Google Play Prerequisites:
- [ ] Google Play Console account active
- [ ] App created with application ID `com.sigook.beta`
- [ ] Service account created with API access
- [ ] Service account JSON downloaded
- [ ] Internal testing track enabled

---

## 🚀 How to Deploy

### For Staging (Dev Branch):
```bash
git checkout dev
git add .
git commit -m "Your changes"
git push origin dev
```

**Pipeline runs:**
1. Validate & Test
2. Build Android Staging → APK artifact
3. Build iOS Staging → IPA artifact

**Artifacts available for download and manual distribution.**

---

### For Production (Main Branch):
```bash
git checkout main
git merge dev
git push origin main
```

**Pipeline runs:**
1. Validate & Test
2. Build Android Production → AAB artifact
3. **Deploy to Play Store** (Internal track) → Automatic
4. Build iOS Production → IPA artifact
5. **Deploy to TestFlight** → Automatic

**Apps automatically uploaded to Play Store (internal track) and TestFlight.**

---

## 🎯 Pipeline Flow Summary

### Dev Branch Push:
```
Validate & Test
    ↓
Build Android Staging (APK)
    ↓
Build iOS Staging (IPA)
    ↓
Download artifacts manually
```

### Main Branch Push:
```
Validate & Test
    ↓
Build Android Production (AAB)
    ↓
Deploy to Play Store (automatic)
    ↓
Build iOS Production (IPA)
    ↓
Deploy to TestFlight (automatic)
    ↓
Apps available for testing!
```

---

## 🔍 Verify Everything Works

### Test 1: Dev Branch (Staging)
1. Push to `dev` branch
2. Check pipeline runs Stages 1, 2, 5
3. Download APK and IPA artifacts
4. Install on test devices manually

### Test 2: Main Branch (Production)
1. Push to `main` branch
2. Check pipeline runs all 7 stages
3. Verify Play Store upload (check Google Play Console)
4. Verify TestFlight upload (check App Store Connect)
5. Test builds from Play Store and TestFlight

---

## 📖 Related Documentation

- **iOS Setup Details:** `IOS_TESTFLIGHT_COMPLETE_GUIDE.md`
- **Android Setup Details:** `PLAYSTORE_COMPLETE_GUIDE.md`
- **Pipeline File:** `.azure-pipelines/sigookapp-pipeline.yml`

---

## ❓ Common Issues

### Issue: "Secure file not found"
**Solution:** 
- Verify file name in variable group matches uploaded file name exactly
- Verify pipeline has authorization to access secure file

### Issue: "Variable group not found"
**Solution:**
- Verify variable group name spelling is exact
- Verify variable group exists in Library

### Issue: "Environment not found"
**Solution:**
- Create environment with exact name (case-sensitive)
- `PlayStore-Production` and `TestFlight-Production`

### Issue: "Certificate installation failed"
**Solution:**
- Verify P12_PASSWORD is correct
- Verify .p12 file is valid Apple Distribution certificate
- Re-export certificate from Keychain if needed

### Issue: "App Store Connect API error"
**Solution:**
- Verify API Key ID, Issuer ID, and Key Content are correct
- Verify API key has "App Manager" access or higher
- Check .p8 file content is complete (including BEGIN/END lines)

---

## 🎉 You're Ready!

The pipeline code is **100% complete** with all 7 stages implemented. You just need to configure Azure DevOps with your credentials, files, and variables. Follow the checklist above, and you'll have automated deployments to both Play Store and TestFlight!

**Total Setup Time:** ~2-3 hours (mostly waiting for Apple/Google approvals)

Good luck! 🚀
