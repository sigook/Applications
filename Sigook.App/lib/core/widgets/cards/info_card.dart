import 'package:flutter/material.dart';

class InfoCard extends StatelessWidget {
  final String? title;
  final Widget child;
  final EdgeInsets? margin;
  final EdgeInsets? padding;

  const InfoCard({
    super.key,
    this.title,
    required this.child,
    this.margin,
    this.padding,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: margin ?? const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: padding ?? const EdgeInsets.all(20),
        child: title != null
            ? Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title!,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 16),
                  child,
                ],
              )
            : child,
      ),
    );
  }
}
