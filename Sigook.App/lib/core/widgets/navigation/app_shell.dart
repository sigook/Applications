import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../theme/app_theme.dart';
import 'navbar_logo.dart';

class AppShell extends StatelessWidget {
  final StatefulNavigationShell navigationShell;
  final Widget profileIcon;
  final Widget profileSelectedIcon;

  const AppShell({
    super.key,
    required this.navigationShell,
    this.profileIcon = const Icon(Icons.person_outline),
    this.profileSelectedIcon = const Icon(Icons.person),
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: navigationShell,
      bottomNavigationBar: DecoratedBox(
        decoration: const BoxDecoration(
          border: Border(top: BorderSide(color: AppTheme.borderLight)),
        ),
        child: NavigationBarTheme(
          data: NavigationBarThemeData(
            backgroundColor: Colors.white,
            surfaceTintColor: Colors.transparent,
            indicatorColor: AppTheme.primaryBlue.withValues(alpha: 0.1),
            iconTheme: WidgetStateProperty.resolveWith(
              (states) => IconThemeData(
                color: states.contains(WidgetState.selected)
                    ? AppTheme.tertiaryBlue
                    : Colors.grey.shade700,
              ),
            ),
            labelTextStyle: WidgetStateProperty.resolveWith(
              (states) => TextStyle(
                fontSize: 12,
                fontWeight: states.contains(WidgetState.selected)
                    ? FontWeight.w700
                    : FontWeight.w500,
                color: states.contains(WidgetState.selected)
                    ? AppTheme.tertiaryBlue
                    : Colors.grey.shade700,
              ),
            ),
          ),
          child: NavigationBar(
            height: 64,
            selectedIndex: navigationShell.currentIndex,
            onDestinationSelected: (index) {
              notifyLogoFlash();
              navigationShell.goBranch(
                index,
                initialLocation: index == navigationShell.currentIndex,
              );
            },
            destinations: [
              const NavigationDestination(
                icon: Icon(Icons.work_outline),
                selectedIcon: Icon(Icons.work),
                label: 'Jobs',
              ),
              const NavigationDestination(
                icon: Icon(Icons.event_note_outlined),
                selectedIcon: Icon(Icons.event_note),
                label: 'Activity',
              ),
              NavigationDestination(
                icon: profileIcon,
                selectedIcon: profileSelectedIcon,
                label: 'Profile',
              ),
            ],
          ),
        ),
      ),
    );
  }
}
