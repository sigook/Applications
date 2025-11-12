# Registration Form Refactoring Status

## 🎯 Objective
Reorganize the registration form from 5 sections to 4 sections with new field arrangement.

---

## ✅ Completed Work

### 1. **New Domain Entities Created**

#### Basic Info (Section 1)
- ✅ Created `lib/features/registration/domain/entities/basic_info.dart`
  - Combines: Personal info + Location + Mobile number
  - Fields: firstName, lastName, dateOfBirth, gender, country, province, city, address, zipCode, mobileNumber

#### Preferences Info (Section 2)
- ✅ Created `lib/features/registration/domain/entities/preferences_info.dart`
  - Combines: Availability + Physical capabilities + Professional skills
  - Fields: availabilityType, availableTimes, availableDays, liftingCapacity, hasVehicle, languages, skills
- ✅ Created `lib/features/registration/domain/entities/lifting_capacity.dart`
  - Enum for lifting capacity: 10-20 lbs, 20-40 lbs, 40-60 lbs

#### Documents Info (Section 3)
- ✅ Created `lib/features/registration/domain/entities/documents_info.dart`
  - Fields: documents[], licenses[], certificates[], resume

#### Account Info (Section 4)
- ✅ Created `lib/features/registration/domain/entities/account_info.dart`
  - Fields: email, password, confirmPassword, termsAccepted

### 2. **Existing Entities Updated**
- ✅ Updated `address_info.dart` - Added `zipCode` field
- ✅ Updated `registration_form.dart` - Changed to new 4-section structure

### 3. **New UI Pages Created**
- ✅ Created `lib/features/registration/presentation/pages/basic_info_page.dart`
- ✅ Created `lib/features/registration/presentation/pages/preferences_page.dart`
- ✅ Created `lib/features/registration/presentation/pages/documents_page.dart`
- ✅ Created `lib/features/registration/presentation/pages/account_page.dart`

---

## 🔧 Work In Progress / Pending

### 1. **Update Registration ViewModel**
File: `lib/features/registration/presentation/viewmodels/registration_viewmodel.dart`

**Needed Changes:**
- Replace old update methods with new ones:
  - ❌ `updatePersonalInfo()` → ✅ `updateBasicInfo()`
  - ❌ `updateContactInfo()` → ✅ `updateAccountInfo()`
  - ❌ `updateAddressInfo()` → (merged into BasicInfo)
  - ❌ `updateAvailabilityInfo()` → (merged into PreferencesInfo)
  - ❌ `updateProfessionalInfo()` → (merged into PreferencesInfo)
  - ✅ ADD: `updatePreferencesInfo()`
  - ✅ ADD: `updateDocumentsInfo()`

### 2. **Update Worker Registration Request**
File: `lib/features/registration/data/models/worker_registration_request.dart`

**Needed Changes:**
- Update `fromEntity()` to map from new 4-section structure
- Add fields:
  - zipCode (from BasicInfo)
  - liftingCapacity (from PreferencesInfo)
  - hasVehicle (from PreferencesInfo)
  - confirmPassword handling
  - termsAccepted

### 3. **Update Registration Screen**
File: `lib/features/registration/presentation/pages/registration_screen.dart`

**Needed Changes:**
- Update stepper from 5 steps to 4 steps:
  1. Basic Information
  2. Preferences
  3. Documents
  4. Account Setup
- Update step validation checks
- Update imports to use new pages

### 4. **Update/Create Missing Widgets**

**Need to create/update:**
- ✅ `AvailabilityTypeSelector` widget (may already exist)
- ✅ `AvailabilityTimeSelector` widget (may already exist)
- ✅ `DaySelector` widget (may already exist)
- Note: Language and Skill autocomplete widgets already exist

### 5. **Update Data Models**

**Files to update:**
- `address_info_model.dart` - Add zipCode field
- `registration_form_model.dart` - Update to new 4-section structure
- Create new models:
  - `basic_info_model.dart`
  - `preferences_info_model.dart`
  - `documents_info_model.dart`
  - `account_info_model.dart`

### 6. **Fix Existing Pages**
These old pages still reference the old structure and need updates/removal:
- `personal_info_page.dart` - Can be deleted (merged into basic_info_page)
- `contact_info_page.dart` - Can be deleted (merged into account_page)
- `address_info_page.dart` - Can be deleted (merged into basic_info_page)
- `availability_info_page.dart` - Can be deleted (merged into preferences_page)
- `professional_info_page.dart` - Can be deleted (merged into preferences_page)

### 7. **Update Form State Provider**
File: `lib/features/registration/presentation/providers/registration_providers.dart`

**Needed Changes:**
- Update step count from 5 to 4 in `RegistrationFormStateNotifier`
- Update step validation logic

---

## 📋 New Form Structure

### Section 1: Basic Information
```
✅ First Name
✅ Last Name
✅ Date of Birth
✅ Gender
✅ Country (dropdown - pre-loaded)
✅ Province (dropdown - loaded when country selected)
✅ City (dropdown - loaded when province selected)
✅ Address
✅ ZIP Code
✅ Mobile Number
```

### Section 2: Preferences
```
✅ Availability Type
✅ Available Time
✅ Available Days
✅ Lifting Capacity (10-20 / 20-40 / 40-60 lbs) - Radio buttons
✅ Has Vehicle - Checkbox
✅ Languages
✅ Skills
```

### Section 3: Documents
```
✅ Documents - File upload (optional, multiple)
✅ Licenses - File upload (optional, multiple)
✅ Certificates - File upload (optional, multiple)
✅ Resume - File upload (REQUIRED, single)
```

### Section 4: Account Setup
```
✅ Email
✅ Password
✅ Confirm Password
✅ Terms & Conditions Checkbox
```

---

## ⚠️ Current Errors

### Critical Errors to Fix:
1. **RegistrationViewModel** - Missing new update methods
2. **Worker Registration Request** - Referencing old structure
3. **Registration Screen** - Still using 5-step layout
4. **Data Models** - Not updated for new structure
5. **Old Pages** - Still referencing old structure

### Build Errors Count: ~60+
Most errors are due to:
- Old pages trying to access removed properties
- ViewModel missing new methods
- Data models not updated

---

## 🚀 Next Steps (Priority Order)

1. **Update RegistrationViewModel** - Add new methods, remove old ones
2. **Create missing widget files** - availability selectors, day selector
3. **Update registration_screen.dart** - Change to 4-step layout
4. **Update worker_registration_request.dart** - Map from new structure
5. **Create data models** for new entities
6. **Run build_runner** to generate freezed/json files
7. **Delete old pages** after confirming new ones work
8. **Test each section** independently
9. **End-to-end testing** of full registration flow

---

## 📝 Notes

- All catalog data (countries, genders, etc.) is pre-loaded before form display
- Province and city data loads dynamically based on parent selection
- File upload functionality is placeholder for now (buttons do nothing except resume validation)
- Terms checkbox must be checked to proceed
- Password confirmation must match password

---

## 💡 Design Decisions

1. **BasicInfo** consolidates all identifying and location information
2. **PreferencesInfo** groups work-related preferences and capabilities
3. **DocumentsInfo** centralizes all file upload requirements
4. **AccountInfo** handles authentication and legal agreement
5. **Lifting capacity** implemented as enum for type safety
6. **Vehicle ownership** simple boolean (not optional)
7. **Resume** is the only required document

---

*Last Updated: During refactoring session*
*Status: Domain entities complete, UI pages complete, integration pending*
