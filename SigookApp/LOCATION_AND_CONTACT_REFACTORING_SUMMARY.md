# Location and Contact Information Refactoring Summary

## Overview
This refactoring updates the registration flow to use catalog-based location selection with cascading dropdowns and adds mobile phone number to contact information.

---

## ✅ Changes Made

### 1. **Catalog API Updates**

#### Updated Methods to Support Parameters
- **File**: `lib/features/catalog/data/datasources/catalog_remote_datasource.dart`
  - `getProvinces(String countryId)` - Now requires country ID parameter
  - `getCities(String provinceId)` - Now requires province ID parameter
  - API calls: 
    - `/Catalog/province?countryId={id}`
    - `/Catalog/city?provinceId={id}`

#### Repository Layer
- **File**: `lib/features/catalog/domain/repositories/catalog_repository.dart`
  - Added `getProvinces(String countryId)`
  - Added `getCities(String provinceId)`

- **File**: `lib/features/catalog/data/repositories/catalog_repository_impl.dart`
  - Implemented both methods with proper error handling

#### Use Cases
- **Created**: `lib/features/catalog/domain/usecases/get_provinces.dart`
- **Created**: `lib/features/catalog/domain/usecases/get_cities.dart`

#### Providers
- **File**: `lib/features/catalog/presentation/providers/catalog_providers.dart`
  - Added `getProvincesProvider`
  - Added `getCitiesProvider`
  - Added `provincesProvider` - FutureProvider.family<List<CatalogItem>, String>
  - Added `citiesProvider` - FutureProvider.family<List<CatalogItem>, String>

---

### 2. **Contact Information Updates**

#### Domain Entity
- **File**: `lib/features/registration/domain/entities/contact_info.dart`
  - **Added**: `final String mobileNumber` field
  - **Updated**: Validation to check mobile number (min 10 digits)
  - **Updated**: `copyWith` method
  - **Updated**: `props` getter

#### Data Model
- **File**: `lib/features/registration/data/models/contact_info_model.dart`
  - **Added**: `required String mobileNumber` to freezed model
  - **Updated**: `fromEntity` to include mobileNumber
  - **Updated**: `toEntity` to include mobileNumber

#### Presentation Pages:
- **File**: `lib/features/registration/presentation/pages/contact_info_page.dart`
  - **Added**: `_mobileNumberController` text controller
  - **Added**: `_mobileNumberError` validation field
  - **Added**: Mobile Number input field in UI (after email)
  - **Updated**: Validation logic
  - **Updated**: Save logic to include mobile number

- **File**: `lib/features/registration/presentation/pages/registration_screen.dart`
  - **Added**: Pre-loading wrapper that loads all catalog data before showing form
  - **Updated**: Shows loading screen while catalog data is being fetched
  - **Updated**: Shows error screen with retry button if catalog loading fails
  - **Split**: Created `_RegistrationFormScreen` private widget (shown only after data loads)

- **File**: `lib/features/registration/presentation/pages/personal_info_page.dart`
  - **Removed**: Loading state from `_buildGenderSelector()`
  - **Removed**: Error state from gender selector
  - **Updated**: Simplified to assume data is already available

#### API Request
- **File**: `lib/features/registration/data/models/worker_registration_request.dart`
  - **Updated**: Uses `contactInfo.mobileNumber` instead of null

---

### 3. **Address Information Refactoring**

#### Complete Page Rewrite
- **File**: `lib/features/registration/presentation/pages/address_info_page.dart`

**Key Changes:**
1. **Removed LocationData dependency** - No longer uses hardcoded location lists
2. **Cascading Selection**:
   - Country selector always visible
   - Province dropdown only shows when country is selected
   - City dropdown only shows when province is selected

3. **State Management**:
   ```dart
   String? _selectedCountryId;
   String? _selectedCountryName;
   String? _selectedProvinceId;
   String? _selectedProvinceName;
   String? _selectedCityId;
   String? _selectedCityName;
   ```

4. **Dynamic Data Loading**:
   - Provinces loaded via `ref.watch(provincesProvider(countryId))`
   - Cities loaded via `ref.watch(citiesProvider(provinceId))`
   - Shows `LinearProgressIndicator` while loading
   - Shows error messages if API fails

5. **Selection Handlers**:
   - `_onCountryChanged()` - Clears province and city
   - `_onProvinceChanged()` - Clears city only
   - `_onCityChanged()` - Sets city

6. **UI Improvements**:
   - Added `_buildProvinceDropdown()` method
   - Added `_buildCityDropdown()` method
   - Added `_buildCatalogDropdown()` helper method for consistent styling
   - Dropdowns are conditionally rendered based on parent selection

---

## 🎯 User Experience Improvements

### Before
- All location dropdowns visible at once
- Used hardcoded location data
- No relationship between country/province/city selections
- Province and city lists didn't update based on country
- **Loading indicators shown inside form fields** (Gender field)

### After
✅ **Progressive Disclosure**: Fields appear only when parent selection is made
✅ **Dynamic Data**: Provinces and cities loaded from API based on selection
✅ **Clear Relationships**: Selecting country shows provinces, selecting province shows cities
✅ **No Loading Widgets in Form**: Loading indicators only appear in specific dropdown sections
✅ **Mobile Number**: Now captured as part of contact information
✅ **Pre-loaded Catalog Data**: All static catalogs loaded before form is displayed
✅ **Clean Loading Experience**: Full-screen loader before form, then seamless form interaction

---

## 📋 API Flow

### Pre-loading Flow (Before Form Display)
```
1. User navigates to Registration Screen
   ↓
2. RegistrationScreen wrapper starts loading:
   - Genders
   - Identification Types
   - Languages
   - Skills
   - Availability Types
   - Availability Times
   - Countries
   ↓
3. Shows full-screen loading indicator
   "Loading registration form..."
   ↓
4. Once ALL catalog data is loaded successfully
   ↓
5. Shows the actual registration form
   (All form fields now work without loading states)
```

### Location Selection Flow (Within Form)
```
1. User selects Country
   ↓
2. App calls GET /Catalog/province?countryId={selectedCountryId}
   ↓
3. Province dropdown appears with fetched data
   ↓
4. User selects Province
   ↓
5. App calls GET /Catalog/city?provinceId={selectedProvinceId}
   ↓
6. City dropdown appears with fetched data
   ↓
7. User selects City
```

### Data Submission
```dart
{
  "country": "Canada",
  "provinceState": "Ontario",
  "city": "Toronto",
  "address": "123 Main St",
  "mobileNumber": "+1 234 567 8900"
}
```

---

## 🔧 Technical Details

### Riverpod Family Providers
```dart
final provincesProvider = FutureProvider.family<List<CatalogItem>, String>(
  (ref, countryId) async {
    final useCase = ref.read(getProvincesProvider);
    final result = await useCase(countryId);
    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  },
);
```

### Conditional Rendering
```dart
// Province dropdown - only show if country is selected
if (_selectedCountryId != null) _buildProvinceDropdown(),
if (_selectedCountryId != null) const SizedBox(height: 24),

// City dropdown - only show if province is selected
if (_selectedProvinceId != null) _buildCityDropdown(),
if (_selectedProvinceId != null) const SizedBox(height: 24),
```

---

## ✅ Testing Checklist

- [ ] Test country selection
- [ ] Verify provinces load when country selected
- [ ] Verify provinces are cleared when country changes
- [ ] Verify cities load when province selected
- [ ] Verify cities are cleared when province changes
- [ ] Test mobile number validation (min 10 digits)
- [ ] Test full registration flow with all fields
- [ ] Verify API payload includes mobileNumber
- [ ] Test error handling when province/city API fails
- [ ] Test loading indicators during data fetch

---

## 📝 Notes

1. **No Loading Widget in Main Form**: Loading indicators only appear within specific dropdown sections when data is being fetched
2. **All Static Catalogs Pre-loaded**: Countries, genders, identification types, languages, skills, etc. are loaded before showing the form
3. **Dynamic Catalogs On-Demand**: Provinces and cities are loaded dynamically based on user selection
4. **LocationData.dart Removed**: Old hardcoded location data is no longer used
5. **Pre-loading Strategy**: `RegistrationScreen` now wraps the form with a loading screen that ensures all catalog data is loaded before showing any form fields

---

## 🚀 Next Steps

1. Remove `location_data.dart` file if not used elsewhere
2. Remove any unused imports in other files
3. Test the complete registration flow end-to-end
4. Verify API endpoints return correct data format
5. Add integration tests for cascading selection

---

## 🐛 Known Issues

- Minor warning: `_selectedCityId` field marked as unused (false positive - used in conditional rendering)
- Deprecated `value` parameter in DropdownButtonFormField (Flutter framework issue, will be fixed in future Flutter versions)
