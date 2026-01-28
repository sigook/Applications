# Deployment Approval Configuration Guide

## ✅ Changes Implemented

### 1. **SigookApp Branch = Production Deployment**
- Both `main` AND `SigookApp` branches now trigger production builds + Play Store deployment
- When you push to either branch, the full production pipeline runs

### 2. **Gradle Optimizations** ✅
Added to `android/gradle.properties`:
```properties
org.gradle.caching=true          # Reuse build outputs
org.gradle.parallel=true         # Build modules in parallel
org.gradle.configureondemand=true # Only configure needed projects
org.gradle.daemon=false          # Disable daemon in CI (saves memory)
```

**Expected build time reduction:** ~5-7 minutes (from 18 min → 10-11 min)

### 3. **APK Build Removed** ✅
- Production pipeline now only builds AAB (Play Store format)
- APK removed because:
  - Play Store doesn't need it
  - Saves ~7 minutes of build time
  - AAB is required for Play Store anyway

**For local testing with APK:**
```powershell
flutter build apk --flavor production -t lib/main_production.dart
```

### 4. **Auto-Incrementing Version Code** ✅
- Version code now auto-increments using Azure Pipeline Build ID
- Each build gets unique version code automatically
- **No more manual `pubspec.yaml` edits needed!**

**How it works:**
- Pipeline adds: `--build-number=$(Build.BuildId)`
- Build ID is sequential: 1, 2, 3, 4...
- Each Play Store upload automatically has higher version code

**pubspec.yaml stays at:**
```yaml
version: 1.0.0+1  # Keep this as-is, pipeline overrides the +1
```

---

## 🔐 Deployment Approval Configuration

### Yes, You Need to Configure Approvals in the Environment

The pipeline references `environment: 'PlayStore-Production'` but approvals are **NOT automatic**.

### Step-by-Step: Add Approval Gates

#### 1. Navigate to Environments
1. Open Azure DevOps
2. Go to your project: **SigookApp**
3. Click: **Pipelines** → **Environments**
4. Click on: **PlayStore-Production**

#### 2. Add Approvals
1. Click the **3-dot menu (⋯)** in the top right
2. Select: **Approvals and checks**
3. Click: **+ (Add check)**
4. Select: **Approvals**

#### 3. Configure Approval Settings

**Approvers:**
- Add yourself: Type your email/name
- Add team members if needed
- You can add multiple approvers

**Approval Type:**
- **Any one user** (default) - any listed approver can approve
- **All listed users** - everyone must approve
- **Specific number of users** - custom threshold

**Advanced Options:**
- **Timeout:** How long to wait before auto-rejecting (default: 30 days)
- **Retry policy:** Allow approvers to retry failed deployments
- **Instructions for approvers:** Add custom message (optional)

**Recommended Settings:**
```
Approvers: You + 1 team member (if available)
Approval type: Any one user
Timeout: 7 days
Instructions: "Review build logs and test results before approving Play Store deployment"
```

#### 4. Save Configuration
- Click **Create** or **Save**
- Approval gate is now active!

---

## 🎯 How Approvals Work in Pipeline

### Without Approvals (Current Default)
```
Push to main/SigookApp
  ↓
Validate & Test (auto)
  ↓
Build Production (auto)
  ↓
Deploy to Play Store (auto) ← Deploys immediately
  ↓
Done
```

### With Approvals (After Configuration)
```
Push to main/SigookApp
  ↓
Validate & Test (auto)
  ↓
Build Production (auto)
  ↓
⏸️ WAITING FOR APPROVAL ← Pipeline pauses here
  ↓
(You click "Review" → "Approve")
  ↓
Deploy to Play Store (auto)
  ↓
Done
```

---

## 📧 Approval Notifications

### What Happens When Pipeline Waits for Approval?

1. **Email Notification:**
   - You receive email: "Approval needed for PlayStore-Production"
   - Contains link to pipeline run
   - Shows: Branch, Commit, Build number

2. **Azure DevOps UI:**
   - Pipeline shows yellow "Waiting" status
   - Big **Review** button appears
   - Can see build artifacts before approving

3. **Approving the Deployment:**
   - Click **Review**
   - See deployment summary
   - Add optional comment
   - Click **Approve** or **Reject**

4. **After Approval:**
   - Pipeline continues automatically
   - AAB uploads to Play Store
   - Deployment marked as complete

---

## 🛡️ Additional Security Checks (Optional)

### Other Useful Environment Checks

Beyond approvals, you can add:

#### 1. **Business Hours Check**
- Only allow deployments during work hours
- Example: Mon-Fri, 9am-5pm EST

**How to add:**
1. Environments → PlayStore-Production
2. Approvals and checks → Add check
3. Select: **Business hours**
4. Configure timezone and hours

#### 2. **Branch Protection**
- Only allow deployments from specific branches
- Already configured via `condition: eq(variables['isMain'], true)`

#### 3. **Required Templates**
- Enforce pipeline security templates
- Advanced - usually not needed

#### 4. **Exclusive Lock**
- Prevent multiple simultaneous deployments
- Useful if you have multiple pipelines

---

## 📋 Quick Setup Checklist

**To enable deployment approvals:**

- [ ] Go to: Pipelines → Environments → PlayStore-Production
- [ ] Click: ⋯ menu → Approvals and checks
- [ ] Add: Approvals check
- [ ] Configure: Add your email as approver
- [ ] Set: Approval type = "Any one user"
- [ ] Save configuration

**Test it:**
- [ ] Push a commit to `main` or `SigookApp` branch
- [ ] Watch pipeline pause at "Deploy to Play Store"
- [ ] Check email for approval notification
- [ ] Click "Review" → "Approve" in Azure DevOps
- [ ] Verify deployment completes

---

## 🎯 Recommended Approval Workflow

### For Production Deployments

1. **Before Pushing to main/SigookApp:**
   - Test locally: `flutter run --flavor production`
   - Review code changes
   - Update release notes (optional)

2. **After Pipeline Builds:**
   - Check build logs for warnings
   - Verify version code is correct
   - Review deployment summary

3. **Before Approving:**
   - Confirm test stage passed
   - Check AAB size is reasonable
   - Verify environment variables are correct

4. **After Approving:**
   - Wait 5-10 minutes for Play Store processing
   - Check Google Play Console for release
   - Test internal track installation

---

## 🔄 Rollback Strategy

### If Something Goes Wrong After Approval

1. **Reject next deployment:**
   - Fix the issue in code
   - Push new commit
   - Let pipeline build new AAB
   - Approve the fixed version

2. **Play Store doesn't support rollbacks:**
   - Can't undo a release automatically
   - Can manually upload previous AAB in Play Console
   - Can halt rollout (if using staged rollouts)

3. **Keep artifacts:**
   - Azure Pipelines keeps previous AABs
   - Download from: Pipelines → Runs → Artifacts
   - Can manually upload to Play Store if needed

---

## 💡 Pro Tips

### 1. **Multiple Approvers for Safety**
```
Approvers: Developer + QA Lead + Product Owner
Approval type: Any 2 users
```
Ensures at least 2 people review before production.

### 2. **Different Approvals for Different Tracks**

Future enhancement - create separate environments:
- `PlayStore-Internal` (auto-deploy, no approval)
- `PlayStore-Beta` (1 approver)
- `PlayStore-Production` (2 approvers)

### 3. **Approval Comments**
When approving, add comments like:
- "Tested locally, looks good"
- "QA approved, deploying"
- "Hotfix for critical bug #123"

Comments appear in deployment history.

### 4. **Timeout Settings**
- Don't set timeout too short (< 1 day)
- Approvers might be in different timezones
- 7 days is reasonable for weekly releases

---

## ❓ FAQ

### Q: What if no one approves?
**A:** Pipeline waits until timeout (default 30 days), then fails. You can manually cancel or retry.

### Q: Can I skip approvals for hotfixes?
**A:** No, but you can:
1. Approve quickly (< 5 minutes)
2. Create separate environment for hotfixes
3. Use manual pipeline trigger with approval bypass (advanced)

### Q: What if I approve by accident?
**A:** Can't undo. Play Store will receive the AAB. You can:
1. Immediately push a new fixed version
2. Halt rollout in Play Console (if using staged rollout)

### Q: Can I test the AAB before approving?
**A:** Yes! Download artifact from pipeline:
1. Click on pipeline run
2. Artifacts → sigookapp-android-production
3. Download AAB
4. Install locally: `adb install app.aab` (requires bundletool)

### Q: Do approvals work offline?
**A:** No. Must approve via Azure DevOps (web or mobile app).

---

## ✅ Summary

**What Changed:**
1. ✅ `SigookApp` branch now deploys to production (same as `main`)
2. ✅ Gradle optimized for faster builds (~10-11 min instead of 18 min)
3. ✅ APK build removed (AAB only) - saves ~7 minutes
4. ✅ Version code auto-increments using Build ID (no manual edits!)

**What You Need to Do:**
1. ✅ Configure approvals in `PlayStore-Production` environment (see steps above)
2. ✅ Test by pushing to `main` or `SigookApp`
3. ✅ Approve when pipeline waits
4. ✅ Verify deployment in Play Console

**Approval Setup:** 5 minutes to configure, then works automatically for all future deployments! 🎉
