import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

class AppTabItem {
  final IconData icon;
  final String label;

  const AppTabItem({required this.icon, required this.label});
}

class AppTabBar extends StatelessWidget implements PreferredSizeWidget {
  static const double height = 60;
  static const double _indicatorWeight = 2;

  final List<AppTabItem> tabs;
  final TabController? controller;

  const AppTabBar({super.key, required this.tabs, this.controller});

  @override
  Size get preferredSize => const Size.fromHeight(height + _indicatorWeight);

  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.white,
      child: TabBar(
        controller: controller,
        labelPadding: const EdgeInsets.symmetric(horizontal: 4),
        indicatorWeight: _indicatorWeight,
        indicator: const UnderlineTabIndicator(
          borderSide: BorderSide(color: AppTheme.primaryBlue, width: 3),
          insets: EdgeInsets.symmetric(horizontal: 16),
        ),
        labelColor: AppTheme.primaryBlue,
        unselectedLabelColor: Colors.grey.shade600,
        labelStyle: const TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.bold,
          letterSpacing: 0.8,
        ),
        unselectedLabelStyle: const TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.w500,
          letterSpacing: 0.5,
        ),
        tabs: [
          for (final tab in tabs)
            Tab(
              height: height,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(tab.icon, size: 24),
                  const SizedBox(height: 4),
                  FittedBox(
                    fit: BoxFit.scaleDown,
                    child: Text(tab.label, maxLines: 1),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }
}
