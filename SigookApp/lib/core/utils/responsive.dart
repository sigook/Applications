import 'package:flutter/material.dart';

/// Responsive utility class for adaptive layouts
/// Provides methods to handle different screen sizes and orientations
class Responsive {
  final BuildContext context;

  Responsive(this.context);

  /// Get screen width
  double get width => MediaQuery.of(context).size.width;

  /// Get screen height
  double get height => MediaQuery.of(context).size.height;

  /// Check if device is mobile (width < 600)
  bool get isMobile => width < 600;

  /// Check if device is tablet (width >= 600 && width < 1024)
  bool get isTablet => width >= 600 && width < 1024;

  /// Check if device is desktop (width >= 1024)
  bool get isDesktop => width >= 1024;

  /// Get responsive padding
  double get horizontalPadding => isMobile ? 16 : (isTablet ? 24 : 32);

  /// Get responsive font size
  double sp(double size) {
    if (isMobile) return size;
    if (isTablet) return size * 1.1;
    return size * 1.2;
  }

  /// Get responsive width (percentage of screen width)
  double wp(double percentage) => width * (percentage / 100);

  /// Get responsive height (percentage of screen height)
  double hp(double percentage) => height * (percentage / 100);

  /// Get keyboard height
  double get keyboardHeight => MediaQuery.of(context).viewInsets.bottom;

  /// Check if keyboard is visible
  bool get isKeyboardVisible => keyboardHeight > 0;
}

/// Extension on BuildContext for easy access to Responsive
extension ResponsiveContext on BuildContext {
  Responsive get responsive => Responsive(this);
}

/// Responsive widget wrapper
class ResponsiveBuilder extends StatelessWidget {
  final Widget Function(BuildContext context, Responsive responsive) builder;

  const ResponsiveBuilder({super.key, required this.builder});

  @override
  Widget build(BuildContext context) {
    return builder(context, Responsive(context));
  }
}
