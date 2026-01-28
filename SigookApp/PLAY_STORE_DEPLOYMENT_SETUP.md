# Play Store Deployment Setup Guide

## ✅ What's Already Done (In Codebase)

- ✅ Pipeline updated with `Deploy_PlayStore` stage in `.azure-pipelines/sigookapp-pipeline.yml`
- ✅ Production AAB build configured
- ✅ Android signing with keystore
- ✅ Artifact publishing

---

## 🔧 Required Azure DevOps Setup (Outside Codebase)

### 1. Upload Service Account JSON to Secure Files

**Steps:**
1. Go to Azure DevOps → Your Project
2. Navigate to: `Pipelines` → `Library` → `Secure files`
3. Click `+ Secure file`
4. Upload your Google Play service account JSON file
5. **Important:** Name it exactly: `google-play-service-account.json`
   - The pipeline expects this exact filename
   - If you use a different name, update line 515 in the pipeline YAML

**What is this file?**
- It's the JSON key you downloaded when creating the service account in Google Cloud Console
- Contains credentials for the pipeline to authenticate with Google Play

---

### 2. Create Environment for Approval Gates

**Steps:**
1. Go to: `Pipelines` → `Environments`
2. Click `New environment`
3. Name: `PlayStore-Production` (must match exactly)
4. Description: "Google Play Store production deployment"
5. Click `Create`

**Optional but Recommended - Add Approvals:**
1. Click on the `PlayStore-Production` environment
2. Click the `...` menu → `Approvals and checks`
3. Click `+` → `Approvals`
4. Add yourself and/or team members as approvers
5. This prevents accidental deployments - pipeline will wait for manual approval

---

### 3. Verify Google Play Console Setup

**Ensure you've completed these in Google Play Console:**

1. **Service Account Permissions:**
   - Go to: Google Play Console → Setup → API access
   - Your service account should be listed
   - Required permissions:
     - ✅ View app information and download bulk reports
     - ✅ Create and edit draft apps
     - ✅ Release apps to testing tracks
     - ✅ Release apps to production

2. **App Created:**
   - App must exist in Play Console (even if not published)
   - Package name: `com.example.sigook_app_flutter`
   - If using a different package name, see "Codebase Changes" below

3. **Internal Testing Track:**
   - Go to: Release → Testing → Internal testing
   - Create the track if it doesn't exist
   - Add at least one internal tester (can be yourself)

---

## 📝 Required Codebase Changes

### Update Application ID (If Needed)

**Current package name:** `com.example.sigook_app_flutter`

If you need to change this (recommended for production):

#### 1. Update `android/app/build.gradle.kts`:
```kotlin
defaultConfig {
    applicationId = "com.sigook.app"  // Change from com.example.sigook_app_flutter
    // ... rest of config
}
```

#### 2. Update pipeline YAML line 522:
```yaml
applicationId: 'com.sigook.app'  # Must match build.gradle.kts
```

#### 3. Update Android namespace in `android/app/build.gradle.kts` line 20:
```kotlin
namespace = "com.sigook.app"  // Change from com.example.sigook_app_flutter
```

#### 4. Update manifestPlaceholders in `android/app/build.gradle.kts` line 64:
```kotlin
manifestPlaceholders["appAuthRedirectScheme"] = "sigookcallback"  // Keep or update based on OAuth config
```

---

## 🚀 Testing the Pipeline

### First Deployment (Internal Track):

1. **Trigger the pipeline:**
   - Push to `main` branch
   - Pipeline will run automatically

2. **Monitor stages:**
   - Stage 1: Validate & Test
   - Stage 2: Build Android (Production)
   - Stage 3: Deploy to Play Store
     - Will wait for approval if you set it up
     - Click "Review" → "Approve" to proceed

3. **Verify in Play Console:**
   - Go to: Google Play Console → Release → Testing → Internal testing
   - You should see a new release with your build number
   - It may take 5-10 minutes to appear

---

## 📊 Deployment Tracks Strategy

The pipeline is currently set to deploy to **Internal** track. Here's the recommended progression:

### Track Progression:

1. **Internal** (current setting)
   - File: `.azure-pipelines/sigookapp-pipeline.yml` line 525
   - Value: `track: 'internal'`
   - Who: Internal team members only
   - Use for: Initial testing, QA

2. **Alpha** (when ready for wider testing)
   - Change line 525 to: `track: 'alpha'`
   - Who: External alpha testers
   - Use for: Pre-release testing with select users

3. **Beta** (public testing)
   - Change line 525 to: `track: 'beta'`
   - Who: Public beta testers (opt-in via Play Store)
   - Use for: Wider audience testing before production

4. **Production** (public release)
   - Change line 525 to: `track: 'production'`
   - Also add: `rolloutPercentage: 10`
   - Gradually increase percentage: 10% → 25% → 50% → 100%
   - Use for: Live app in Play Store

---

## 🔍 Troubleshooting

### Common Issues:

#### 1. "Service account not found"
- Verify the JSON file is uploaded to Secure Files
- Check the filename matches exactly: `google-play-service-account.json`

#### 2. "App not found" or "Package name mismatch"
- Ensure `applicationId` in pipeline matches the app in Play Console
- App must exist in Play Console before first deployment

#### 3. "Insufficient permissions"
- Check service account permissions in Play Console
- May need to wait 24 hours after granting permissions

#### 4. "Release already exists"
- Each release needs a unique version code
- Version code auto-increments based on `versionCode` in build.gradle.kts

#### 5. Pipeline waiting indefinitely
- Check if approval is required in the `PlayStore-Production` environment
- Go to pipeline run → Click "Review" → "Approve"

---

## 📌 Important Notes

1. **Version Management:**
   - Version code is managed in `pubspec.yaml`
   - Format: `version: 1.0.0+1` (the `+1` is the version code)
   - Must increment for each Play Store release

2. **First Upload:**
   - The first AAB upload to a new app may require manual intervention in Play Console
   - Subsequent automated uploads should work smoothly

3. **Release Notes:**
   - Currently not included in pipeline
   - Can add via `changeLogFile` parameter in GooglePlayRelease task
   - Create `CHANGELOG.md` in SigookApp folder if needed

4. **Rollback:**
   - Play Store doesn't support automatic rollbacks
   - Keep previous AABs as artifacts in Azure DevOps
   - Can manually upload previous version if needed

---

## ✅ Final Checklist

Before running the pipeline:

- [ ] Service account JSON uploaded to Azure DevOps Secure Files
- [ ] `PlayStore-Production` environment created
- [ ] Approval gates configured (optional but recommended)
- [ ] Service account has correct permissions in Play Console
- [ ] App exists in Play Console with matching package name
- [ ] Internal testing track created in Play Console
- [ ] Application ID updated in code (if changed from example package)
- [ ] Application ID updated in pipeline YAML (if changed)
- [ ] Version code incremented in `pubspec.yaml` if redeploying

Once all items are checked, push to `main` branch and the pipeline will deploy to Play Store! 🚀
