import 'package:flutter/material.dart';

class Responsive {
  final BuildContext context;

  Responsive(this.context);

  double get width => MediaQuery.of(context).size.width;

  double get height => MediaQuery.of(context).size.height;

  bool get isMobile => width < 600;

  bool get isTablet => width >= 600 && width < 1024;

  bool get isDesktop => width >= 1024;

  double get horizontalPadding => isMobile ? 16 : (isTablet ? 24 : 32);

  double sp(double size) {
    if (isMobile) return size;
    if (isTablet) return size * 1.1;
    return size * 1.2;
  }

  double wp(double percentage) => width * (percentage / 100);

  double hp(double percentage) => height * (percentage / 100);

  double get keyboardHeight => MediaQuery.of(context).viewInsets.bottom;

  bool get isKeyboardVisible => keyboardHeight > 0;
}

extension ResponsiveContext on BuildContext {
  Responsive get responsive => Responsive(this);
}

class ResponsiveBuilder extends StatelessWidget {
  final Widget Function(BuildContext context, Responsive responsive) builder;

  const ResponsiveBuilder({super.key, required this.builder});

  @override
  Widget build(BuildContext context) {
    return builder(context, Responsive(context));
  }
}
