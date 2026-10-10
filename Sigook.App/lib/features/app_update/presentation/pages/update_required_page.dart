import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/theme/app_theme.dart';
import '../../domain/entities/app_update_check.dart';

class UpdateRequiredPage extends ConsumerStatefulWidget {
  final AppUpdateCheck check;

  const UpdateRequiredPage({super.key, required this.check});

  @override
  ConsumerState<UpdateRequiredPage> createState() => _UpdateRequiredPageState();
}

class _UpdateRequiredPageState extends ConsumerState<UpdateRequiredPage> {
  bool _opening = false;
  bool _openFailed = false;

  String get _storeUrl => Platform.isIOS
      ? widget.check.requirement.iosStoreUrl
      : widget.check.requirement.androidStoreUrl;

  @override
  void initState() {
    super.initState();
    ref.read(analyticsServiceProvider).logEvent(
      name: 'app_update_required',
      parameters: {
        'current_version': widget.check.currentVersion,
        'minimum_version': widget.check.requirement.minimumVersion,
      },
    );
  }

  Future<void> _openStore() async {
    final uri = Uri.tryParse(_storeUrl);
    if (uri == null || _opening) return;
    setState(() {
      _opening = true;
      _openFailed = false;
    });
    var opened = false;
    try {
      opened = await launchUrl(uri, mode: LaunchMode.externalApplication);
    } catch (_) {
      opened = false;
    }
    if (!mounted) return;
    setState(() {
      _opening = false;
      _openFailed = !opened;
    });
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: false,
      child: Scaffold(
        backgroundColor: const Color(0xFFF5F7FA),
        body: SafeArea(
          child: Center(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 32),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Image.asset('assets/images/logo/sigook-logo.png', width: 180),
                  const SizedBox(height: 48),
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(24),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withValues(alpha: 0.06),
                          blurRadius: 16,
                          offset: const Offset(0, 4),
                        ),
                      ],
                    ),
                    child: Column(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: AppTheme.secondaryRed.withValues(alpha: 0.1),
                            shape: BoxShape.circle,
                          ),
                          child: const Icon(
                            Icons.system_update_rounded,
                            size: 32,
                            color: AppTheme.secondaryRed,
                          ),
                        ),
                        const SizedBox(height: 16),
                        const Text(
                          'Update required',
                          style: TextStyle(
                            fontSize: 22,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.textDark,
                          ),
                        ),
                        const SizedBox(height: 8),
                        const Text(
                          'A new version of Sigook is available.\n'
                          'Please update the app to keep using it.',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontSize: 14,
                            color: AppTheme.textMedium,
                            height: 1.5,
                          ),
                        ),
                        if (_openFailed) ...[
                          const SizedBox(height: 12),
                          Text(
                            'Could not open the store. Please update Sigook from '
                            '${Platform.isIOS ? 'the App Store' : 'Google Play'}.',
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 13,
                              color: AppTheme.errorRed,
                            ),
                          ),
                        ],
                      ],
                    ),
                  ),
                  const SizedBox(height: 32),
                  SizedBox(
                    width: double.infinity,
                    height: 52,
                    child: ElevatedButton(
                      onPressed: _opening ? null : _openStore,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.secondaryRed,
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(16),
                        ),
                        elevation: 2,
                      ),
                      child: _opening
                          ? const SizedBox(
                              width: 22,
                              height: 22,
                              child: CircularProgressIndicator(
                                strokeWidth: 2.5,
                                color: Colors.white,
                              ),
                            )
                          : const Text(
                              'Update now',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
