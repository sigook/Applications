import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../../../core/routing/app_router.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../auth/presentation/pages/logout_webview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../account_settings/presentation/viewmodels/account_settings_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';

class AccountSettingsSectionCard extends ConsumerStatefulWidget {
  const AccountSettingsSectionCard({super.key});

  @override
  ConsumerState<AccountSettingsSectionCard> createState() =>
      _AccountSettingsSectionCardState();
}

class _AccountSettingsSectionCardState
    extends ConsumerState<AccountSettingsSectionCard> {
  final _newEmailController = TextEditingController();
  final _confirmEmailController = TextEditingController();

  @override
  void dispose() {
    _newEmailController.dispose();
    _confirmEmailController.dispose();
    super.dispose();
  }

  void _clearEmailForm() {
    _newEmailController.clear();
    _confirmEmailController.clear();
  }

  Future<void> _submitEmailChange() async {
    final newEmail = _newEmailController.text.trim();
    final confirmEmail = _confirmEmailController.text.trim();
    if (newEmail.isEmpty) {
      showProfileError(context, 'New email is required');
      return;
    }
    if (confirmEmail != newEmail) {
      showProfileError(context, 'Emails do not match');
      return;
    }
    await ref.read(accountSettingsViewModelProvider.notifier).changeEmail(
          newEmail: newEmail,
          confirmNewEmail: confirmEmail,
        );
  }

  Future<void> _showPasswordInfoDialog() async {
    await showDialog<void>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Change Password'),
        content: const Text(
          'To change your password, use the "Forgot Password" option on the '
          'login screen. A reset link will be sent to your registered email address.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Got it'),
          ),
        ],
      ),
    );
  }

  Future<void> _showDeactivateDialog() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        titlePadding: const EdgeInsets.fromLTRB(24, 24, 24, 8),
        contentPadding: const EdgeInsets.fromLTRB(24, 0, 24, 24),
        title: Row(
          children: [
            Icon(Icons.warning_amber_rounded, color: Colors.red.shade600, size: 28),
            const SizedBox(width: 10),
            Text(
              'Are you sure?',
              style: TextStyle(
                fontSize: 22,
                fontWeight: FontWeight.bold,
                color: Colors.red.shade700,
              ),
            ),
          ],
        ),
        content: const Text(
          'This action will deactivate your account. You will be signed out '
          'and will no longer be able to access the platform. Do you want to proceed?',
          style: TextStyle(fontSize: 14, height: 1.5),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red.shade600,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8)),
            ),
            child: const Text('Deactivate'),
          ),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;

    await ref.read(authViewModelProvider.notifier).deactivateAccount();

    if (!mounted) return;
    final error = ref.read(authViewModelProvider).error;
    if (error != null) {
      showProfileError(context, error);
    } else {
      context.go(AppRoutes.welcome);
    }
  }

  Future<void> _showLogoutDialog() async {
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

    if (shouldLogout != true || !mounted) return;

    final idToken = ref.read(authViewModelProvider).token?.idToken;
    final notifier = ref.read(authViewModelProvider.notifier);

    await Navigator.of(context).push(
      MaterialPageRoute<bool>(
        builder: (_) => LogoutWebviewPage(idToken: idToken),
      ),
    );

    await notifier.logout();
    if (mounted) context.go(AppRoutes.welcome);
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(accountSettingsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen<AccountSettingsState>(accountSettingsViewModelProvider,
        (prev, next) {
      if (!mounted) return;
      if (next.justChangedEmail && !(prev?.justChangedEmail ?? false)) {
        _clearEmailForm();
        showProfileSuccess(context, 'Email updated successfully!');
      }
      if (next.emailError != null && next.emailError != prev?.emailError) {
        showProfileError(context, next.emailError!);
      }
    });

    return ProfileSectionCard(
      title: 'Account Settings',
      icon: Icons.manage_accounts_outlined,
      iconGradient: const [Color(0xFF37474F), Color(0xFF78909C)],
      children: [
        // ── Email ──────────────────────────────────────────────────────────
        if (!vm.isChangingEmail)
          Row(
            children: [
              Icon(Icons.email_outlined, size: 20, color: Colors.grey.shade600),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Email',
                      style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade500,
                          fontWeight: FontWeight.w500),
                    ),
                    Text(
                      profile?.email ?? '—',
                      style: const TextStyle(fontSize: 14),
                    ),
                  ],
                ),
              ),
              IconButton(
                onPressed: () => ref
                    .read(accountSettingsViewModelProvider.notifier)
                    .startChangingEmail(),
                icon: Icon(Icons.edit_outlined,
                    size: 18, color: AppTheme.primaryBlue),
                tooltip: 'Change Email',
                padding: EdgeInsets.zero,
                constraints: const BoxConstraints(),
              ),
            ],
          )
        else ...[
          Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: Text(
              'Change Email',
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey.shade600,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          TextField(
            controller: _newEmailController,
            keyboardType: TextInputType.emailAddress,
            decoration: InputDecoration(
              labelText: 'New Email',
              prefixIcon: const Icon(Icons.email_outlined, size: 20),
              border:
                  OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
              contentPadding:
                  const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              isDense: true,
            ),
          ),
          const SizedBox(height: 10),
          TextField(
            controller: _confirmEmailController,
            keyboardType: TextInputType.emailAddress,
            decoration: InputDecoration(
              labelText: 'Confirm New Email',
              prefixIcon: const Icon(Icons.email_outlined, size: 20),
              border:
                  OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
              contentPadding:
                  const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              isDense: true,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: ElevatedButton(
                  onPressed: vm.isSavingEmail ? null : _submitEmailChange,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppTheme.primaryBlue,
                    foregroundColor: Colors.white,
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12)),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                  child: vm.isSavingEmail
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(
                              strokeWidth: 2, color: Colors.white),
                        )
                      : const Text('Save Email'),
                ),
              ),
              const SizedBox(width: 8),
              IconButton(
                onPressed: vm.isSavingEmail
                    ? null
                    : () {
                        _clearEmailForm();
                        ref
                            .read(accountSettingsViewModelProvider.notifier)
                            .cancelChangingEmail();
                      },
                icon: Icon(Icons.cancel_outlined,
                    color: Colors.grey.shade500, size: 22),
                tooltip: 'Cancel',
              ),
            ],
          ),
        ],

        const Divider(height: 24),

        // ── Password ───────────────────────────────────────────────────────
        Row(
          children: [
            Icon(Icons.lock_outline, size: 20, color: Colors.grey.shade600),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Password',
                      style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade500,
                          fontWeight: FontWeight.w500)),
                  const Text('••••••••',
                      style: TextStyle(fontSize: 14, letterSpacing: 2)),
                ],
              ),
            ),
            TextButton(
              onPressed: _showPasswordInfoDialog,
              style: TextButton.styleFrom(
                foregroundColor: AppTheme.primaryBlue,
                padding: const EdgeInsets.symmetric(horizontal: 8),
              ),
              child: const Text('Change'),
            ),
          ],
        ),

        const Divider(height: 28),

        // ── Deactivate Account ─────────────────────────────────────────────
        Padding(
          padding: const EdgeInsets.only(bottom: 8),
          child: Text(
            'Deactivate Account',
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: Colors.grey.shade800,
            ),
          ),
        ),
        Text(
          'Deactivating your account will prevent you from accessing the platform. '
          'Your data will be retained as required by law, but you will no longer '
          'be able to sign in or apply to jobs.',
          style: TextStyle(fontSize: 13, color: Colors.grey.shade600, height: 1.45),
        ),
        const SizedBox(height: 10),
        _BulletPoint('You will be signed out immediately'),
        _BulletPoint('You will no longer be able to sign in to your account'),
        _BulletPoint('You will not be able to apply to new job requests'),
        const SizedBox(height: 14),
        SizedBox(
          width: double.infinity,
          child: OutlinedButton.icon(
            onPressed: _showDeactivateDialog,
            icon: const Icon(Icons.no_accounts_outlined, size: 20),
            label: const Text('Deactivate Account'),
            style: OutlinedButton.styleFrom(
              foregroundColor: Colors.red.shade600,
              side: BorderSide(color: Colors.red.shade400),
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
        ),

        const Divider(height: 28),

        // ── Logout ─────────────────────────────────────────────────────────
        SizedBox(
          width: double.infinity,
          child: OutlinedButton.icon(
            onPressed: _showLogoutDialog,
            icon: const Icon(Icons.logout, size: 20),
            label: const Text('Logout'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppTheme.secondaryRed,
              side: const BorderSide(color: AppTheme.secondaryRed, width: 1.5),
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
        ),
      ],
    );
  }
}

class _BulletPoint extends StatelessWidget {
  final String text;
  const _BulletPoint(this.text);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 5, right: 8),
            child: CircleAvatar(
              radius: 3,
              backgroundColor: Colors.grey.shade500,
            ),
          ),
          Expanded(
            child: Text(
              text,
              style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
            ),
          ),
        ],
      ),
    );
  }
}
