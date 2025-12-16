# Registration API Migration to Multipart/Form-Data

## Overview
Migrated the worker registration endpoint from JSON-only to multipart/form-data to align with the backend API changes.

## Backend API Specification
```csharp
[HttpPost]
[Consumes("multipart/form-data")]
public async Task<IActionResult> CreateWorkerProfile([FromQuery] int? orderId)
```

The backend expects:
- **`data` field**: JSON string containing worker profile data (WorkerProfileCreateModel)
- **File fields**: Separate multipart file uploads for documents and images

## Changes Made

### 1. Created `WorkerProfileData` Model
**File**: `lib/features/registration/data/models/worker_profile_data.dart`

This model contains only the worker profile data (no files):
- Personal information (name, birthday, gender)
- Identification numbers and types (no files)
- Contact information
- Location data
- Availability preferences
- Professional data (languages, skills)
- Account credentials

**Purpose**: Serialize to JSON for the `data` field in multipart request.

### 2. Updated `WorkerRegistrationRequest`
**File**: `lib/features/registration/data/models/worker_registration_request.dart`

Added method:
```dart
WorkerProfileData toWorkerProfileData()
```

This extracts all non-file data from the registration request.

### 3. Updated `RegistrationRemoteDataSource`
**File**: `lib/features/registration/data/datasources/registration_remote_datasource.dart`

**Before**: Sent entire request as JSON
```dart
await apiClient.post('/WorkerProfile', data: jsonData);
```

**After**: Uses FormData with multipart
```dart
final formData = FormData();
formData.fields.add(MapEntry('data', jsonEncode(jsonData)));
// Add files separately
formData.files.add(MapEntry('profileImage', ...));
formData.files.add(MapEntry('identificationType1File', ...));
// etc.
await apiClient.post('/WorkerProfile', data: formData);
```

## File Handling

### ProfileImage
- Uses `pathFile` property (local file path)
- Filename from `fileName` property
- Field name: `profileImage`

### Identification Documents
- Uses `filePath` property from `UploadedFile`
- Field names: `identificationType1File`, `identificationType2File`
- Only sent if file path exists

### Resume
- Uses `filePath` property from `UploadedFile`
- Field name: `resume`
- Only sent if file path exists

## Request Structure

### Multipart Form-Data Structure:
```
POST /api/WorkerProfile
Content-Type: multipart/form-data

Parts:
1. data: {
     "firstName": "John",
     "lastName": "Doe",
     "email": "john@example.com",
     ...
   }
2. profileImage: [binary file data]
3. identificationType1File: [binary file data]
4. identificationType2File: [binary file data] (optional)
5. resume: [binary file data] (optional)
```

## Key Points

1. **Worker data sent as JSON string** in the `data` field
2. **Files sent as separate multipart attachments**
3. **File fields are optional** - only included if files exist
4. **Proper null checks** before accessing file paths
5. **Debug logging** to track which files are attached

## Testing Checklist

- [ ] Registration with profile image
- [ ] Registration with 1 identification document
- [ ] Registration with 2 identification documents
- [ ] Registration with resume
- [ ] Registration without optional files
- [ ] File path validation
- [ ] Error handling for missing files
- [ ] Network retry on failure

## Benefits

1. **Aligned with backend API** - Matches expected format
2. **Efficient file upload** - Files sent as binary, not base64
3. **Better separation of concerns** - Data vs files
4. **Easier debugging** - Can see file count in logs
5. **Maintains retry logic** - Multipart works with existing retry mechanism
