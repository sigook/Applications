import 'package:flutter/material.dart';

/// Application theme configuration
/// Centralized theme management for consistent styling across the app
class AppTheme {
  // Private constructor to prevent instantiation
  AppTheme._();

  // Color Palette
  static const Color primaryBlue = Color(0xFF1565C0); // Deep Blue
  static const Color secondaryRed = Color(0xFFE53935); // Vibrant Red
  static const Color tertiaryBlue = Color(0xFF0277BD); // Light Blue
  static const Color surfaceGrey = Color(0xFFF5F7FA); // Subtle blue-grey
  static const Color errorRed = Color(0xFFD32F2F); // Error red
  static const Color successGreen = Color(0xFF43A047); // Success green
  static const Color textDark = Color(0xFF212121); // Dark grey text
  static const Color textLight = Color(0xFF757575); // Light grey text

  /// Light theme configuration
  static ThemeData get lightTheme {
    return ThemeData(
      // Color Scheme
      colorScheme: ColorScheme.fromSeed(
        seedColor: primaryBlue,
        primary: primaryBlue,
        secondary: secondaryRed,
        tertiary: tertiaryBlue,
        surface: surfaceGrey,
        error: errorRed,
        onPrimary: Colors.white,
        onSecondary: Colors.white,
        onSurface: textDark,
        brightness: Brightness.light,
      ),
      
      scaffoldBackgroundColor: surfaceGrey,
      useMaterial3: true,
      fontFamily: 'Roboto',
      
      // AppBar Theme
      appBarTheme: const AppBarTheme(
        centerTitle: false,
        elevation: 0,
        backgroundColor: primaryBlue,
        foregroundColor: Colors.white,
        iconTheme: IconThemeData(color: Colors.white),
        titleTextStyle: TextStyle(
          color: Colors.white,
          fontSize: 20,
          fontWeight: FontWeight.w600,
        ),
      ),
      
      // Elevated Button Theme
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: primaryBlue,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 16),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 2,
          shadowColor: primaryBlue.withValues(alpha: 0.3),
        ),
      ),
      
      // Text Button Theme
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: primaryBlue,
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        ),
      ),
      
      // Outlined Button Theme
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: primaryBlue,
          side: const BorderSide(color: primaryBlue, width: 1.5),
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
        ),
      ),
      
      // Input Decoration Theme
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Color(0xFFE0E0E0)),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Color(0xFFE0E0E0)),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: primaryBlue, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: secondaryRed, width: 1.5),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: secondaryRed, width: 2),
        ),
      ),
      
      // Card Theme
      cardTheme: CardThemeData(
        elevation: 2,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
        ),
        color: Colors.white,
        shadowColor: primaryBlue.withValues(alpha: 0.08),
        surfaceTintColor: primaryBlue.withValues(alpha: 0.02),
      ),
      
      // Chip Theme
      chipTheme: ChipThemeData(
        backgroundColor: const Color(0xFFF5F5F5),
        selectedColor: primaryBlue.withValues(alpha: 0.15),
        secondarySelectedColor: secondaryRed.withValues(alpha: 0.15),
        labelStyle: const TextStyle(fontSize: 14, color: textDark),
        secondaryLabelStyle: const TextStyle(fontSize: 14, color: textDark),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        checkmarkColor: primaryBlue,
      ),
      
      // Progress Indicator Theme
      progressIndicatorTheme: ProgressIndicatorThemeData(
        color: secondaryRed, // RED ACCENT!
        linearTrackColor: secondaryRed.withValues(alpha: 0.2),
        circularTrackColor: secondaryRed.withValues(alpha: 0.2),
      ),
      
      // Floating Action Button Theme
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: secondaryRed, // RED ACCENT!
        foregroundColor: Colors.white,
        elevation: 4,
      ),
      
      // Snackbar Theme
      snackBarTheme: SnackBarThemeData(
        backgroundColor: textDark,
        contentTextStyle: const TextStyle(color: Colors.white),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        behavior: SnackBarBehavior.floating,
      ),
      
      // Divider Theme
      dividerTheme: const DividerThemeData(
        color: Color(0xFFE0E0E0),
        thickness: 1,
      ),
      
      // Icon Theme
      iconTheme: const IconThemeData(
        color: textDark,
        size: 24,
      ),
    );
  }

  /// Dark theme configuration (for future use)
  static ThemeData get darkTheme {
    return ThemeData(
      colorScheme: ColorScheme.fromSeed(
        seedColor: primaryBlue,
        brightness: Brightness.dark,
        primary: primaryBlue,
        secondary: secondaryRed,
      ),
      useMaterial3: true,
    );
  }
}
