# Analytics & Crash Reporting Setup Guide

## Overview

This document provides instructions for setting up Firebase Analytics and Crashlytics in the Sigook application.

**Status:** Infrastructure ready, Firebase integration pending

---

## Current Implementation

### ✅ What's Already Done

1. **Analytics Service Interface**
   - Location: `lib/core/services/analytics_service.dart`
   - Abstract interface for analytics operations
   - Mock implementation that logs to console in debug mode

2. **Crash Reporting Service Interface**
   - Location: `lib/core/services/crash_reporting_service.dart`
   - Abstract interface for crash reporting
   - Mock implementation that logs to console in debug mode
   - Flutter error handler integration

3. **Riverpod Providers**
   - Location: `lib/core/providers/analytics_providers.dart`
   - Dependency injection ready

4. **Job Analytics Events**
   - Pre-built event helpers for job-related analytics
   - `logJobViewed`, `logJobApplied`, `logJobSearch`

---

## Firebase Setup (To Be Completed)

### Step 1: Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click "Add Project"
3. Enter project name: "Sigook"
4. Enable Google Analytics (recommended)
5. Select or create Analytics account
6. Click "Create Project"

### Step 2: Add Firebase to Flutter App

#### iOS Setup
1. In Firebase Console, click iOS icon
2. Enter iOS bundle ID (from `ios/Runner/Info.plist`)
3. Download `GoogleService-Info.plist`
4. Add to `ios/Runner/` directory in Xcode
5. Follow Firebase setup instructions

#### Android Setup
1. In Firebase Console, click Android icon
2. Enter Android package name (from `android/app/build.gradle`)
3. Download `google-services.json`
4. Place in `android/app/` directory
5. Follow Firebase setup instructions

### Step 3: Add Dependencies

Add to `pubspec.yaml`:

```yaml
dependencies:
  # Firebase Core (required)
  firebase_core: ^3.0.0
  
  # Analytics
  firebase_analytics: ^11.0.0
  
  # Crash Reporting
  firebase_crashlytics: ^4.0.0
```

Run:
```bash
flutter pub get
```

### Step 4: Initialize Firebase

Update `lib/main_common.dart`:

```dart
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_crashlytics/firebase_crashlytics.dart';
import 'firebase_options.dart'; // Generated file

Future<void> mainCommon(Environment env) async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Initialize Firebase
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  
  // Setup Crashlytics
  FlutterError.onError = FirebaseCrashlytics.instance.recordFlutterFatalError;
  
  PlatformDispatcher.instance.onError = (error, stack) {
    FirebaseCrashlytics.instance.recordError(error, stack, fatal: true);
    return true;
  };
  
  // ... rest of initialization
}
```

### Step 5: Update Analytics Service

Replace mock implementation in `analytics_service.dart`:

```dart
import 'package:firebase_analytics/firebase_analytics.dart';

class AnalyticsServiceImpl implements AnalyticsService {
  final FirebaseAnalytics _analytics = FirebaseAnalytics.instance;

  @override
  Future<void> logEvent({
    required String name,
    Map<String, dynamic>? parameters,
  }) async {
    await _analytics.logEvent(
      name: name,
      parameters: parameters,
    );
  }

  @override
  Future<void> logScreenView({
    required String screenName,
    String? screenClass,
  }) async {
    await _analytics.logScreenView(
      screenName: screenName,
      screenClass: screenClass,
    );
  }

  @override
  Future<void> setUserId(String? userId) async {
    await _analytics.setUserId(id: userId);
  }

  @override
  Future<void> setUserProperty({
    required String name,
    required String value,
  }) async {
    await _analytics.setUserProperty(
      name: name,
      value: value,
    );
  }

  // ... implement other methods
}
```

### Step 6: Update Crash Reporting Service

Replace mock implementation in `crash_reporting_service.dart`:

```dart
import 'package:firebase_crashlytics/firebase_crashlytics.dart';

class CrashReportingServiceImpl implements CrashReportingService {
  final FirebaseCrashlytics _crashlytics = FirebaseCrashlytics.instance;

  @override
  Future<void> recordError(
    dynamic exception,
    StackTrace? stackTrace, {
    dynamic reason,
    bool fatal = false,
    Map<String, dynamic>? information,
  }) async {
    await _crashlytics.recordError(
      exception,
      stackTrace,
      reason: reason,
      fatal: fatal,
      information: information?.map((k, v) => MapEntry(k, v.toString())),
    );
  }

  @override
  Future<void> recordFlutterError(FlutterErrorDetails details) async {
    await _crashlytics.recordFlutterError(details);
  }

  @override
  Future<void> setCustomKey(String key, dynamic value) async {
    await _crashlytics.setCustomKey(key, value);
  }

  @override
  Future<void> setUserId(String userId) async {
    await _crashlytics.setUserIdentifier(userId);
  }

  @override
  Future<void> log(String message) async {
    await _crashlytics.log(message);
  }
}
```

---

## Usage Examples

### Screen Tracking

```dart
class MyPage extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final analytics = ref.read(analyticsServiceProvider);
    
    useEffect(() {
      analytics.logScreenView(
        screenName: 'My Page',
        screenClass: 'MyPage',
      );
      return null;
    }, []);

    return Scaffold(...);
  }
}
```

### Event Tracking

```dart
// Track button click
await ref.read(analyticsServiceProvider).logEvent(
  name: 'button_clicked',
  parameters: {
    'button_name': 'apply_job',
    'job_id': jobId,
  },
);

// Track job application
await JobAnalyticsEvents.logJobApplied(
  ref.read(analyticsServiceProvider),
  jobId: job.id,
  jobTitle: job.title,
);
```

### Error Tracking

```dart
try {
  await riskyOperation();
} catch (error, stackTrace) {
  await ref.read(crashReportingServiceProvider).recordError(
    error,
    stackTrace,
    reason: 'Failed to complete risky operation',
    information: {
      'user_id': currentUserId,
      'operation_type': 'risky',
    },
  );
  
  // Show user-friendly error
  showErrorDialog();
}
```

### User Properties

```dart
// Set user properties after login
final analytics = ref.read(analyticsServiceProvider);
await analytics.setUserId(user.id);
await analytics.setUserProperty(
  name: 'user_type',
  value: 'worker',
);
await analytics.setUserProperty(
  name: 'signup_date',
  value: user.createdAt.toIso8601String(),
);
```

---

## Recommended Events to Track

### Authentication
- `sign_up` - User creates account
- `login` - User logs in
- `logout` - User logs out

### Jobs
- `job_search` - User searches for jobs
- `job_viewed` - User views job details
- `job_applied` - User applies for job
- `job_favorite` - User favorites a job

### Profile
- `profile_viewed` - User views their profile
- `profile_updated` - User updates profile
- `document_uploaded` - User uploads document

### Registration
- `registration_started` - User begins registration
- `registration_step_completed` - User completes a step
- `registration_completed` - User completes registration

### Navigation
- `screen_view` - User views a screen
- `tab_changed` - User changes tab

---

## Testing

### Debug View (iOS)

```bash
adb shell setprop debug.firebase.analytics.app <package_name>
```

### Debug View (Android)

```bash
adb shell setprop debug.firebase.analytics.app <package_name>
```

### Verify Events in Firebase Console

1. Go to Firebase Console
2. Navigate to Analytics > Events
3. Enable Debug View
4. Run app in debug mode
5. Verify events appear in real-time

### Test Crashlytics

```dart
// Force a crash (testing only!)
FirebaseCrashlytics.instance.crash();

// Or send test error
throw Exception('This is a test crash');
```

---

## Privacy & Compliance

### GDPR Compliance

```dart
// Disable analytics for users who opt out
await FirebaseAnalytics.instance.setAnalyticsCollectionEnabled(false);

// Disable crashlytics
await FirebaseCrashlytics.instance.setCrashlyticsCollectionEnabled(false);
```

### Data Retention

Configure in Firebase Console:
1. Analytics > Data Settings > Data Retention
2. Set retention period (14 months recommended)
3. Enable "Reset data on new activity"

---

## Monitoring & Alerts

### Crashlytics Alerts

1. Go to Crashlytics in Firebase Console
2. Set up alerts for:
   - New crashes
   - Crash velocity alerts
   - ANR (Application Not Responding)

### Analytics Alerts

1. Go to Analytics in Firebase Console
2. Set up custom alerts for:
   - Sudden drop in users
   - Increase in errors
   - Conversion funnel drops

---

## Performance Considerations

### Batch Events

Analytics automatically batches events. Don't worry about calling too frequently.

### Async Operations

All analytics calls are async and non-blocking. They won't slow down your app.

### Error Handling

Wrap analytics calls in try-catch to prevent analytics failures from affecting app:

```dart
try {
  await analytics.logEvent(...);
} catch (e) {
  // Analytics failure shouldn't crash app
  debugPrint('Analytics error: $e');
}
```

---

## Next Steps

1. ✅ Infrastructure created (complete)
2. ⏳ Create Firebase project
3. ⏳ Add Firebase to iOS/Android
4. ⏳ Install Firebase dependencies
5. ⏳ Replace mock implementations
6. ⏳ Test in debug mode
7. ⏳ Deploy to production
8. ⏳ Monitor in Firebase Console

---

**Document Version:** 1.0  
**Last Updated:** December 2024  
**Owner:** Sigook Development Team
