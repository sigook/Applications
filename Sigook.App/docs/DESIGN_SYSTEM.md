# Sigook Design System

## Introduction

This document defines the design system for the Sigook application, ensuring consistency across all features and components.

---

## Color Palette

### Primary Colors

| Color | Hex | Usage |
|-------|-----|-------|
| **Primary Blue** | `#1565C0` | Primary actions, headers, links |
| **Secondary Red** | `#E53935` | Secondary actions, alerts |
| **Tertiary Blue** | `#0277BD` | Accent elements, highlights |

### Semantic Colors

| Color | Hex | Usage |
|-------|-----|-------|
| **Error Red** | `#D32F2F` | Error states, validation failures |
| **Success Green** | `#43A047` | Success messages, confirmations |
| **Surface Grey** | `#F5F7FA` | Background, surfaces |

### Text Colors

| Color | Hex | Usage |
|-------|-----|-------|
| **Text Dark** | `#212121` | Primary text, headings |
| **Text Medium** | `#757575` | Secondary text, labels |
| **Text Light** | `#757575` | Tertiary text, placeholders |

### Border Colors

| Color | Hex | Usage |
|-------|-----|-------|
| **Border Light** | `#E0E0E0` | Input borders, dividers |
| **Slate 800** | `#1E293B` | Dark text alternative |

---

## Typography

### Headings

| Style | Size | Weight | Usage |
|-------|------|--------|-------|
| **Heading 1** | 32px | Bold (700) | Page titles |
| **Heading 2** | 24px | Bold (700) | Section titles |
| **Heading 3** | 20px | Semi-bold (600) | Subsection titles |
| **Heading 4** | 18px | Semi-bold (600) | Card titles |

### Body Text

| Style | Size | Weight | Usage |
|-------|------|--------|-------|
| **Body Large** | 16px | Normal (400) | Large body text |
| **Body Medium** | 14px | Normal (400) | Default body text |
| **Body Small** | 12px | Normal (400) | Small body text |

### Special

| Style | Size | Weight | Usage |
|-------|------|--------|-------|
| **Caption** | 12px | Normal (400) | Captions, hints |
| **Button** | 16px | Semi-bold (600) | Button labels |
| **Label** | 14px | Medium (500) | Form labels |

**Font Family:** Roboto (system default)

---

## Spacing

### Scale

Our spacing follows an 4px base grid:

| Token | Value | Usage |
|-------|-------|-------|
| `spacing4` | 4px | Micro spacing |
| `spacing8` | 8px | Small spacing |
| `spacing12` | 12px | Medium-small spacing |
| `spacing16` | 16px | Standard spacing |
| `spacing24` | 24px | Large spacing |
| `spacing32` | 32px | Section spacing |
| `spacing48` | 48px | Major section spacing |
| `spacing64` | 64px | Page-level spacing |

### Usage Guidelines

- **Padding:** Use `spacing16` for standard padding
- **Margins:** Use `spacing12` between related items
- **Section gaps:** Use `spacing24` or `spacing32`
- **Page margins:** Use `spacing16` on mobile, `spacing24` on tablet+

---

## Border Radius

| Token | Value | Usage |
|-------|-------|-------|
| `radiusSmall` | 8px | Chips, tags |
| `radiusMedium` | 12px | Inputs, buttons |
| `radiusLarge` | 16px | Cards, modals |
| `radiusXLarge` | 20px | Large cards |
| `radiusRound` | 999px | Circular elements |

---

## Elevation & Shadows

### Card Elevation

**Standard Card:**
```dart
elevation: 2
shadowColor: AppTheme.primaryBlue.withValues(alpha: 0.08)
```

**Elevated Card:**
```dart
elevation: 4
shadowColor: AppTheme.primaryBlue.withValues(alpha: 0.12)
```

**Floating Action Button:**
```dart
elevation: 6
shadowColor: AppTheme.primaryBlue.withValues(alpha: 0.3)
```

---

## Components

### Buttons

#### Primary Button
- **Background:** `primaryBlue`
- **Text:** White
- **Padding:** 32px horizontal, 16px vertical
- **Border Radius:** `radiusMedium` (12px)
- **Elevation:** 2

#### Secondary Button (Outlined)
- **Border:** `primaryBlue`, 1.5px
- **Text:** `primaryBlue`
- **Background:** Transparent
- **Padding:** 24px horizontal, 16px vertical
- **Border Radius:** `radiusMedium` (12px)

#### Text Button
- **Text:** `primaryBlue`
- **Background:** Transparent
- **Padding:** 16px horizontal, 12px vertical

### Input Fields

#### Text Field
- **Background:** White
- **Border:** `borderLight` (1px)
- **Focused Border:** `primaryBlue` (2px)
- **Error Border:** `errorRed` (1.5px)
- **Border Radius:** `radiusMedium` (12px)
- **Padding:** 16px horizontal, 16px vertical

#### Dropdown
- Same styling as Text Field
- **Icon:** Down arrow, `textMedium`

### Cards

#### Standard Card
- **Background:** White
- **Elevation:** 2
- **Border Radius:** `radiusLarge` (16px)
- **Padding:** 16-24px

#### Info Card
- **Background:** White
- **Border:** 1px solid `borderLight`
- **Border Radius:** `radiusMedium` (12px)
- **Padding:** 16px

### Chips

#### Filter Chip
- **Background:** `#F5F5F5` (unselected)
- **Selected Background:** `primaryBlue.withValues(alpha: 0.15)`
- **Text:** `textDark`
- **Border Radius:** `radiusSmall` (8px)
- **Padding:** 12px horizontal, 8px vertical

---

## Icons

### Sizes

| Size | Value | Usage |
|------|-------|-------|
| Small | 16px | Inline icons |
| Medium | 20px | Button icons |
| Large | 24px | Section icons |
| XLarge | 32px | Feature icons |
| XXLarge | 64px | Empty/error states |

### Color

- **Primary icons:** `textDark` (#212121)
- **Secondary icons:** `textMedium` (#757575)
- **Disabled icons:** `textLight` with opacity
- **Accent icons:** `primaryBlue` (#1565C0)

---

## Layout

### Breakpoints

| Breakpoint | Value | Usage |
|------------|-------|-------|
| Mobile | < 600px | Phone portrait |
| Tablet | 600-900px | Phone landscape, small tablets |
| Desktop | > 900px | Tablets landscape, desktop |

### Responsive Padding

```dart
EdgeInsets.all(
  MediaQuery.of(context).size.width < 600
    ? AppTheme.spacing12  // Mobile
    : AppTheme.spacing24  // Tablet+
)
```

### Max Width

- **Forms:** 600px
- **Content:** 1200px
- **Modals:** 500px

---

## State Patterns

### Loading States

Use `LoadingIndicator` with message:
```dart
LoadingIndicator(message: 'Loading...')
```

### Error States

Use `ErrorStateWidget`:
```dart
ErrorStateWidget(
  title: 'Error title',
  message: 'Error description',
  onRetry: () => retry(),
)
```

### Empty States

Use `EmptyStateWidget`:
```dart
EmptyStateWidget(
  icon: Icons.inbox,
  title: 'No items',
  message: 'Description',
  actionLabel: 'Action',
  onAction: () => action(),
)
```

---

## Animations

### Duration

| Duration | Value | Usage |
|----------|-------|-------|
| Fast | 150ms | Micro-interactions |
| Normal | 300ms | Standard transitions |
| Slow | 500ms | Page transitions |

### Curves

- **Ease Out:** `Curves.easeOut` - Exiting animations
- **Ease In:** `Curves.easeIn` - Entering animations
- **Ease In Out:** `Curves.easeInOut` - Bi-directional

---

## Accessibility

### Color Contrast

All text must meet WCAG AA standards:
- **Normal text:** 4.5:1 minimum
- **Large text:** 3:1 minimum
- **Icons:** 3:1 minimum

### Touch Targets

- **Minimum size:** 44x44px (iOS) / 48x48px (Android)
- **Spacing:** 8px minimum between targets

### Semantic Labels

Always provide semantic labels for:
- Buttons
- Input fields
- Icons
- Images

---

## Implementation

### Using the Design System

```dart
import 'package:sigook_app_flutter/core/theme/app_theme.dart';

// Colors
color: AppTheme.primaryBlue

// Spacing
padding: EdgeInsets.all(AppTheme.spacing16)

// Border Radius
borderRadius: BorderRadius.circular(AppTheme.radiusMedium)

// Text Styles
style: AppTheme.heading2
```

### Custom Theming

If you need custom variations, extend from base:

```dart
AppTheme.heading2.copyWith(
  color: AppTheme.successGreen,
)
```

---

## Resources

- **Figma Design Files:** [Link to Figma]
- **Theme Implementation:** `lib/core/theme/app_theme.dart`
- **Widget Library:** `lib/core/widgets/`
- **Example Usage:** See `WIDGET_USAGE_GUIDE.md`

---

**Version:** 1.0
**Last Updated:** December 2024
**Maintained By:** Sigook Design Team
