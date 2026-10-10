# Widget Usage Guide

## Overview
This guide provides best practices and examples for using the standardized widget library in the Sigook application.

## Core Widgets

### 1. LoadingIndicator & LoadingOverlay

**Purpose:** Display loading states consistently across the app.

**Usage:**
```dart
// Simple loading indicator with message
LoadingIndicator(message: 'Loading profile...')

// Full-screen loading overlay
LoadingOverlay(
  isLoading: isLoading,
  message: 'Processing...',
  child: YourContent(),
)
```

**When to use:**
- Data fetching operations
- API calls
- File uploads
- Any async operation that requires user feedback

---

### 2. ErrorStateWidget

**Purpose:** Display error states with retry functionality.

**Usage:**
```dart
ErrorStateWidget(
  title: 'Failed to load data',
  message: 'Unable to retrieve information',
  onRetry: () => ref.refresh(dataProvider),
)
```

**When to use:**
- API call failures
- Network errors
- Data loading failures
- Permission denied scenarios

---

### 3. EmptyStateWidget

**Purpose:** Display when no data is available.

**Usage:**
```dart
EmptyStateWidget(
  icon: Icons.inbox,
  title: 'No items found',
  message: 'Try adjusting your filters',
  actionLabel: 'Reset Filters',
  onAction: () => resetFilters(),
)
```

**When to use:**
- Empty lists
- No search results
- Cleared filters
- First-time user experiences

---

### 4. CustomTextField

**Purpose:** Standardized text input with theme styling.

**Usage:**
```dart
CustomTextField(
  label: 'Email',
  hint: 'example@email.com',
  controller: emailController,
  errorText: emailError,
  keyboardType: TextInputType.emailAddress,
  prefixIcon: Icons.email,
)
```

**Features:**
- Automatic AppTheme styling
- Built-in validation display
- Icon support
- Keyboard type handling

---

### 5. CustomDropdown

**Purpose:** Standardized dropdown selection.

**Usage:**
```dart
CustomDropdown<String>(
  label: 'Country',
  hint: 'Select country',
  items: countries,
  value: selectedCountry,
  onChanged: (value) => setState(() => selectedCountry = value),
  errorText: countryError,
)
```

**When to use:**
- Fixed option selections
- Category choices
- Status selections

---

### 6. CustomCard

**Purpose:** Consistent card styling.

**Usage:**
```dart
CustomCard(
  padding: EdgeInsets.all(16),
  child: YourContent(),
)
```

**When to use:**
- Content grouping
- List items
- Section containers

---

### 7. PageScaffold

**Purpose:** Standard page wrapper with AppBar.

**Usage:**
```dart
PageScaffold(
  title: 'Page Title',
  actions: [IconButton(...)],
  child: YourContent(),
)
```

**When to use:**
- Standard pages with AppBar
- Consistent navigation structure

---

## Theme Constants

### Colors
```dart
AppTheme.primaryBlue
AppTheme.secondaryRed
AppTheme.surfaceGrey
AppTheme.errorRed
AppTheme.successGreen
AppTheme.textDark
AppTheme.textMedium
AppTheme.textLight
AppTheme.borderLight
```

### Spacing
```dart
AppTheme.spacing4   // 4px
AppTheme.spacing8   // 8px
AppTheme.spacing12  // 12px
AppTheme.spacing16  // 16px
AppTheme.spacing24  // 24px
AppTheme.spacing32  // 32px
AppTheme.spacing48  // 48px
AppTheme.spacing64  // 64px
```

### Border Radius
```dart
AppTheme.radiusSmall   // 8px
AppTheme.radiusMedium  // 12px
AppTheme.radiusLarge   // 16px
AppTheme.radiusXLarge  // 20px
AppTheme.radiusRound   // 999px (fully rounded)
```

### Text Styles
```dart
AppTheme.heading1  // 32px, bold
AppTheme.heading2  // 24px, bold
AppTheme.heading3  // 20px, w600
AppTheme.heading4  // 18px, w600
AppTheme.bodyLarge // 16px, normal
AppTheme.bodyMedium // 14px, normal
AppTheme.bodySmall  // 12px, normal
AppTheme.caption    // 12px, light
AppTheme.button     // 16px, w600
AppTheme.label      // 14px, w500
```

---

## Widget Creation Guidelines

### When to Create a Core Widget

Create a widget in `lib/core/widgets/` if it meets **ALL** criteria:

1. **Reusable** - Used in 3+ different features
2. **Generic** - No feature-specific logic
3. **Standalone** - Self-contained functionality
4. **Styled** - Uses AppTheme constants

**Examples:**
- ✅ LoadingIndicator (used everywhere)
- ✅ ErrorStateWidget (common pattern)
- ✅ CustomTextField (reusable input)
- ❌ JobCard (job-specific)
- ❌ ProfileHeader (profile-specific)

### When to Create a Feature Widget

Create a widget in `lib/features/[feature]/presentation/widgets/` if:

1. **Feature-specific** - Contains domain logic
2. **Limited scope** - Used in 1-2 places within feature
3. **Business logic** - Tied to specific entities

**Examples:**
- ✅ JobCard (job details display)
- ✅ ProfileHeader (profile-specific)
- ✅ FileUploadModal (registration flow)

---

## Best Practices

### 1. Always Use Theme Constants

❌ **Bad:**
```dart
Color(0xFF1565C0)
fontSize: 16
padding: EdgeInsets.all(12)
```

✅ **Good:**
```dart
AppTheme.primaryBlue
AppTheme.bodyLarge.fontSize
EdgeInsets.all(AppTheme.spacing12)
```

### 2. Prefer Core Widgets Over Custom

❌ **Bad:**
```dart
Center(
  child: Column(
    children: [
      CircularProgressIndicator(),
      Text('Loading...'),
    ],
  ),
)
```

✅ **Good:**
```dart
LoadingIndicator(message: 'Loading...')
```

### 3. Extract Reusable Sections

If you're copying/pasting UI code, consider extracting to a widget.

❌ **Bad:**
```dart
// Same code in 5 different files
Container(
  decoration: BoxDecoration(...),
  child: Column(...),
)
```

✅ **Good:**
```dart
// Create InfoCard widget
InfoCard(title: title, content: content)
```

### 4. Use Const Constructors

```dart
const LoadingIndicator(message: 'Loading...')
const SizedBox(height: AppTheme.spacing16)
const Text('Hello', style: AppTheme.bodyMedium)
```

---

## Migration Checklist

When refactoring existing widgets:

- [ ] Replace hardcoded colors with AppTheme constants
- [ ] Replace hardcoded spacing with AppTheme spacing
- [ ] Replace custom loading with LoadingIndicator
- [ ] Replace custom errors with ErrorStateWidget
- [ ] Use CustomTextField instead of raw TextField
- [ ] Apply consistent border radius
- [ ] Use theme text styles

---

## Error Handling Pattern

```dart
// In ViewModel/Notifier
state = state.copyWith(
  isLoading: false,
  error: ErrorMessages.fromException(exception),
);

// In UI
if (state.error != null)
  ErrorStateWidget(
    title: 'Operation Failed',
    message: state.error!,
    onRetry: () => retry(),
  )
```

---

## Examples

### Complete Page Example

```dart
class MyPage extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(myProvider);
    
    return PageScaffold(
      title: 'My Page',
      child: state.isLoading
        ? const LoadingIndicator(message: 'Loading data...')
        : state.error != null
          ? ErrorStateWidget(
              title: 'Failed to load',
              message: state.error!,
              onRetry: () => ref.refresh(myProvider),
            )
          : state.items.isEmpty
            ? const EmptyStateWidget(
                icon: Icons.inbox,
                title: 'No items',
                message: 'Add your first item',
              )
            : ListView.builder(
                itemCount: state.items.length,
                itemBuilder: (context, index) {
                  return CustomCard(
                    child: ListTile(
                      title: Text(state.items[index].name),
                    ),
                  );
                },
              ),
    );
  }
}
```

---

## Resources

- **Theme File:** `lib/core/theme/app_theme.dart`
- **Core Widgets:** `lib/core/widgets/`
- **Error Messages:** `lib/core/constants/error_messages.dart`
- **i18n Files:** `assets/i18n/`

---

**Last Updated:** December 2024
**Maintained By:** Sigook Development Team
