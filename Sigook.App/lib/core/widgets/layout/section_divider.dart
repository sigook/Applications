import 'package:flutter/material.dart';

class SectionDivider extends StatelessWidget {
  final double height;
  final Color? color;

  const SectionDivider({super.key, this.height = 12, this.color});

  @override
  Widget build(BuildContext context) {
    return SizedBox(height: height);
  }
}
