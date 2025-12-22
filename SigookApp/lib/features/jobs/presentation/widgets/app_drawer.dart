import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';

class AppDrawer extends ConsumerWidget {
  final String currentRoute;

  const AppDrawer({super.key, required this.currentRoute});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profileAsync = ref.watch(cachedWorkerProfileProvider);

    return Drawer(
      backgroundColor: Colors.white,
      child: Column(
        children: [
          profileAsync.when(
            data: (profile) => _buildProfileHeader(
              context,
              profile?.fullName ?? 'User',
              profile?.email ?? '',
              profile?.profilePhoto,
            ),
            loading: () => _buildProfileHeader(context, 'Loading...', '', null),
            error: (_, __) => _buildProfileHeader(context, 'User', '', null),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: ListView(
              padding: EdgeInsets.zero,
              children: [
                _buildMenuItem(
                  context: context,
                  icon: Icons.work_outline,
                  title: 'Jobs',
                  route: AppRoutes.jobs,
                  isSelected: currentRoute == AppRoutes.jobs,
                ),
                _buildMenuItem(
                  context: context,
                  icon: Icons.history,
                  title: 'History',
                  route: '/history',
                  isSelected: currentRoute == '/history',
                ),
                _buildMenuItem(
                  context: context,
                  icon: Icons.account_balance_wallet_outlined,
                  title: 'Payroll',
                  route: '/payroll',
                  isSelected: currentRoute == '/payroll',
                ),
                _buildMenuItem(
                  context: context,
                  icon: Icons.info_outline,
                  title: 'About',
                  route: '/about',
                  isSelected: currentRoute == '/about',
                ),
                const Divider(height: 32, thickness: 1),
                _buildMenuItem(
                  context: context,
                  icon: Icons.logout,
                  title: 'Logout',
                  route: null,
                  isSelected: false,
                  isLogout: true,
                  onTap: () => _handleLogout(context, ref),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildProfileHeader(
    BuildContext context,
    String? displayName,
    String? email,
    String? profilePhoto,
  ) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.fromLTRB(24, 60, 24, 24),
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
        ),
      ),
      child: InkWell(
        onTap: () => _navigateToProfile(context),
        borderRadius: BorderRadius.circular(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                CircleAvatar(
                  radius: 40,
                  backgroundColor: Colors.white,
                  backgroundImage: profilePhoto != null
                      ? NetworkImage(profilePhoto)
                      : null,
                  child: profilePhoto == null
                      ? Text(
                          _getInitials(displayName),
                          style: const TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.primaryBlue,
                          ),
                        )
                      : null,
                ),
                Positioned(
                  right: 0,
                  bottom: 0,
                  child: Container(
                    padding: const EdgeInsets.all(6),
                    decoration: BoxDecoration(
                      color: AppTheme.primaryBlue,
                      shape: BoxShape.circle,
                      border: Border.all(color: Colors.white, width: 2),
                    ),
                    child: const Icon(
                      Icons.edit,
                      size: 14,
                      color: Colors.white,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Text(
              displayName ?? 'User',
              style: const TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.w600,
                color: Colors.white,
              ),
            ),
            if (email != null && email.isNotEmpty) ...[
              const SizedBox(height: 4),
              Text(
                email,
                style: TextStyle(
                  fontSize: 14,
                  color: Colors.white.withValues(alpha: 0.9),
                ),
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildMenuItem({
    required BuildContext context,
    required IconData icon,
    required String title,
    String? route,
    required bool isSelected,
    bool isLogout = false,
    VoidCallback? onTap,
  }) {
    final iconColor = isSelected
        ? AppTheme.primaryBlue
        : isLogout
        ? AppTheme.secondaryRed
        : AppTheme.primaryBlue;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: isSelected
            ? AppTheme.primaryBlue.withValues(alpha: 0.1)
            : Colors.transparent,
        borderRadius: BorderRadius.circular(12),
      ),
      child: ListTile(
        leading: Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: isLogout
                  ? [AppTheme.secondaryRed, AppTheme.errorRed]
                  : isSelected
                  ? [AppTheme.primaryBlue, AppTheme.tertiaryBlue]
                  : [
                      iconColor.withValues(alpha: 0.1),
                      iconColor.withValues(alpha: 0.05),
                    ],
            ),
            borderRadius: BorderRadius.circular(10),
            boxShadow: isSelected || isLogout
                ? [
                    BoxShadow(
                      color:
                          (isLogout
                                  ? AppTheme.secondaryRed
                                  : AppTheme.primaryBlue)
                              .withValues(alpha: 0.3),
                      blurRadius: 8,
                      offset: const Offset(0, 2),
                    ),
                  ]
                : null,
          ),
          child: Icon(
            icon,
            color: isSelected || isLogout ? Colors.white : iconColor,
            size: 22,
          ),
        ),
        title: Text(
          title,
          style: TextStyle(
            fontSize: 16,
            fontWeight: isSelected ? FontWeight.w600 : FontWeight.w400,
            color: isSelected
                ? AppTheme.primaryBlue
                : isLogout
                ? AppTheme.secondaryRed
                : AppTheme.textDark,
          ),
        ),
        trailing: isSelected
            ? Container(
                width: 4,
                height: 24,
                decoration: BoxDecoration(
                  color: AppTheme.primaryBlue,
                  borderRadius: BorderRadius.circular(2),
                ),
              )
            : null,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        onTap:
            onTap ??
            () {
              Navigator.of(context).pop();
              if (route != null && !isSelected) {
                context.go(route);
              }
            },
      ),
    );
  }

  String _getInitials(String? name) {
    if (name == null || name.isEmpty) return 'U';

    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return name[0].toUpperCase();
  }

  void _navigateToProfile(BuildContext context) {
    Navigator.of(context).pop();
    context.push(AppRoutes.profile);
  }

  Future<void> _handleLogout(BuildContext context, WidgetRef ref) async {
    final shouldLogout = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Logout'),
        content: const Text('Are you sure you want to logout?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.secondaryRed,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );

    if (shouldLogout == true && context.mounted) {
      await ref.read(authViewModelProvider.notifier).logout();
      if (context.mounted) {
        context.go(AppRoutes.signIn);
      }
    }
  }
}
