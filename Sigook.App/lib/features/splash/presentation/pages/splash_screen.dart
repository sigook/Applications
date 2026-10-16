import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';

class SplashScreen extends ConsumerStatefulWidget {
  const SplashScreen({super.key});

  @override
  ConsumerState<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends ConsumerState<SplashScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _logoController;
  late Animation<double> _logoFadeAnimation;
  late Animation<double> _logoScaleAnimation;

  bool _hasNavigated = false;
  bool _isProcessing = false;

  @override
  void initState() {
    super.initState();

    _logoController = AnimationController(
      duration: const Duration(milliseconds: 1200),
      vsync: this,
    );

    _logoFadeAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _logoController,
        curve: const Interval(0.0, 0.6, curve: Curves.easeOut),
      ),
    );

    _logoScaleAnimation = Tween<double>(begin: 0.80, end: 1.0).animate(
      CurvedAnimation(
        parent: _logoController,
        curve: const Interval(0.0, 0.7, curve: Curves.easeOutBack),
      ),
    );

    Future.delayed(const Duration(milliseconds: 120), () {
      if (mounted) _logoController.forward();
    });

    Future.delayed(const Duration(seconds: 3), () {
      if (mounted) _restoreSessionAndNavigate();
    });
  }

  Future<void> _restoreSessionAndNavigate() async {
    if (_isProcessing || _hasNavigated) return;
    _isProcessing = true;

    debugPrint('🔐 [SPLASH] Waiting for session restore...');
    final result = await ref.read(authViewModelProvider.notifier).sessionRestore;

    if (!mounted || _hasNavigated) return;
    debugPrint('🔐 [SPLASH] Session restore result: ${result.name}');

    switch (result) {
      case SessionRestoreResult.authenticated:
      case SessionRestoreResult.refreshDeferred:
        await _checkRoleAndNavigate();
      case SessionRestoreResult.unauthenticated:
      case SessionRestoreResult.sessionExpired:
        _navigateToWelcome();
    }
  }

  Future<void> _checkRoleAndNavigate() async {
    final accessToken = ref.read(authViewModelProvider).token?.accessToken;
    if (accessToken == null || accessToken.isEmpty) {
      _navigateToWelcome();
      return;
    }

    final roleResult = await ref
        .read(authRepositoryProvider)
        .getUserRole(accessToken);

    if (!mounted || _hasNavigated) return;

    roleResult.fold(
      (failure) {
        final rejected =
            failure is ServerFailure &&
            (failure.statusCode == 401 || failure.statusCode == 403);
        if (rejected) {
          debugPrint(
            '🔐 [SPLASH] Token rejected by server (${failure.statusCode}), clearing session',
          );
          _signOutAndNavigateToWelcome();
        } else {
          debugPrint(
            '🔐 [SPLASH] Role check unavailable (${failure.message}), continuing',
          );
          _navigateToJobs();
        }
      },
      (role) {
        if (role.toLowerCase() == 'worker') {
          debugPrint('🔐 [SPLASH] User role is worker - access granted');
          _navigateToJobs();
        } else {
          debugPrint('🔐 [SPLASH] User role is "$role" - access denied');
          _signOutAndNavigateToWelcome();
        }
      },
    );
  }

  void _signOutAndNavigateToWelcome() {
    unawaited(ref.read(authViewModelProvider.notifier).logout());
    _navigateToWelcome();
  }

  void _navigateToJobs() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    context.go(AppRoutes.jobs);
  }

  void _navigateToWelcome() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    context.go(AppRoutes.welcome);
  }

  @override
  void dispose() {
    _logoController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;

    return Scaffold(
      body: Stack(
        fit: StackFit.expand,
        children: [
          Container(color: AppTheme.secondaryRed),
          SafeArea(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                SizedBox(height: size.height * 0.36),
                Transform.translate(
                  offset: const Offset(0, -32),
                  child: FadeTransition(
                    opacity: _logoFadeAnimation,
                    child: ScaleTransition(
                      scale: _logoScaleAnimation,
                      child: Image.asset(
                        'assets/images/logo/sigook-logo.png',
                        width: 270,
                        color: Colors.white,
                        colorBlendMode: BlendMode.srcIn,
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
