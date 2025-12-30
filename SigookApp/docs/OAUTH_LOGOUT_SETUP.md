# OAuth Logout Setup Guide

This document explains how the logout flow works and how to configure deep links for post-logout redirect handling.

## Overview

The app uses IdentityServer's standard end-session endpoint for logout. When a user logs out:

1. The app constructs a logout URL with the ID token and post-logout redirect URI
2. The URL is opened in the system browser (external application)
3. IdentityServer clears the session and redirects back to the configured redirect URI
4. The app can optionally handle the redirect to confirm logout completion

## Logout Flow

```
App → Browser → IdentityServer → Redirect Back → App (optional)
```

### Logout URL Format

```
https://your-identityserver-domain/connect/endsession?id_token_hint={id_token}&post_logout_redirect_uri={redirect_uri}
```

## Configuration

### 1. Environment Variables

In your `.env.staging` or `.env.production` file:

```env
POST_LOGOUT_REDIRECT_URI=sigookcallback://logout
```

**Note:** The redirect URI must be registered in your IdentityServer client's `PostLogoutRedirectUris` configuration.

### 2. Android Deep Link Configuration

To handle the post-logout redirect on Android, add an intent filter to `android/app/src/main/AndroidManifest.xml`:

```xml
<!-- Post-logout redirect handler (optional) -->
<intent-filter android:autoVerify="true">
    <action android:name="android.intent.action.VIEW"/>
    <category android:name="android.intent.category.DEFAULT"/>
    <category android:name="android.intent.category.BROWSABLE"/>
    <data android:scheme="sigookcallback" />
    <data android:host="logout" />
</intent-filter>
```

Add this inside the `<activity>` tag for `MainActivity`.

### 3. iOS Deep Link Configuration

For iOS, add the URL scheme to `ios/Runner/Info.plist`:

```xml
<key>CFBundleURLTypes</key>
<array>
    <dict>
        <key>CFBundleTypeRole</key>
        <string>Editor</string>
        <key>CFBundleURLName</key>
        <string>com.sigook.auth</string>
        <key>CFBundleURLSchemes</key>
        <array>
            <string>sigookcallback</string>
        </array>
    </dict>
</array>
```

## Implementation Details

### AuthRemoteDataSource

The logout method in `AuthRemoteDataSourceImpl` uses FlutterAppAuth's `endSession` method:

```dart
Future<void> logout(String idToken) async {
  final request = EndSessionRequest(
    idTokenHint: idToken,
    postLogoutRedirectUrl: EnvironmentConfig.postLogoutRedirectUri,
    issuer: EnvironmentConfig.authority,
  );

  await appAuth.endSession(request);
}
```

### Key Points

- **ID Token Required**: The logout method requires the ID token obtained during authentication
- **FlutterAppAuth Integration**: Uses the OAuth library's native `endSession` method for proper OIDC logout
- **External Browser**: Logout happens in the system browser, not in-app, for security
- **Session Cleanup**: The app clears local token storage after initiating logout
- **Redirect Handling**: The deep link configuration is optional but recommended for a complete flow
- **Dependency Injection**: FlutterAppAuth is properly injected, making the code testable and following Clean Architecture

## Testing

1. **Login** to the app to obtain tokens
2. **Logout** - the system browser should open with the IdentityServer logout page
3. **Verify** that you're logged out of IdentityServer
4. **Check** if the app receives the redirect (if deep link is configured)
5. **Confirm** that local tokens are cleared

## Troubleshooting

### Redirect Not Working

- Verify the `POST_LOGOUT_REDIRECT_URI` is registered in IdentityServer
- Check that the intent filter/URL scheme matches the redirect URI
- Ensure the redirect URI is properly URL-encoded

### Logout Not Clearing Session

- Verify the ID token is valid and not expired
- Check IdentityServer logs for any errors
- Ensure the end-session endpoint is `/connect/endsession`

## Security Considerations

- Always use HTTPS for the IdentityServer authority in production
- The ID token is passed as a query parameter - this is standard OAuth 2.0 practice
- Using `LaunchMode.externalApplication` ensures logout happens in a secure browser context
- Local token storage is cleared immediately upon logout initiation

## Related Files

- `lib/features/auth/data/datasources/auth_remote_datasource.dart` - Logout implementation
- `lib/features/auth/data/repositories/auth_repository_impl.dart` - Repository that calls logout
- `lib/core/config/environment.dart` - Environment configuration
- `android/app/src/main/AndroidManifest.xml` - Android deep link configuration
- `ios/Runner/Info.plist` - iOS URL scheme configuration
