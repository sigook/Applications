import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'dart:async';
import '../../../../core/routing/app_router.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../../auth/domain/usecases/validate_token.dart';

class SplashScreen extends ConsumerStatefulWidget {
  const SplashScreen({super.key});

  @override
  ConsumerState<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends ConsumerState<SplashScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _logoController;
  late Animation<double> _logoFadeAnimation;

  bool _hasNavigated = false;

  @override
  void initState() {
    super.initState();

    _logoController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _logoFadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(parent: _logoController, curve: Curves.easeOut));

    Future.delayed(const Duration(milliseconds: 120), () {
      if (mounted) _logoController.forward();
    });

    _checkAuthAndNavigate();
  }

  Future<void> _checkAuthAndNavigate() async {
    await Future.delayed(const Duration(milliseconds: 3000));

    if (!mounted || _hasNavigated) return;

    debugPrint('🔐 [SPLASH] Starting authentication check...');

    int attempts = 0;
    const maxAttempts = 50;

    bool isInitialized = false;
    while (!isInitialized && attempts < maxAttempts && mounted) {
      await Future.delayed(const Duration(milliseconds: 100));
      attempts++;
      final currentNotifier = ref.read(authViewModelProvider.notifier);
      isInitialized = currentNotifier.isInitialized;
    }

    debugPrint('🔐 [SPLASH] Token loading completed after ${attempts * 100}ms');

    if (!mounted || _hasNavigated) return;

    final authState = ref.read(authViewModelProvider);
    final token = authState.token;

    debugPrint(
      '🔐 [SPLASH] Auth state - isAuthenticated: ${authState.isAuthenticated}, token present: ${token != null}',
    );

    if (token == null ||
        token.accessToken == null ||
        token.accessToken!.isEmpty) {
      debugPrint('🔐 [SPLASH] No token found, redirecting to welcome');
      _navigateToWelcome();
      return;
    }

    final expirationDateTime = token.expirationDateTime;
    final isExpired =
        expirationDateTime != null &&
        DateTime.now().isAfter(expirationDateTime);

    if (isExpired) {
      debugPrint('🔐 [SPLASH] Token is expired, validating with backend...');
    } else {
      debugPrint(
        '🔐 [SPLASH] Token not expired locally, validating with backend...',
      );
    }

    final validateTokenUseCase = ref.read(validateTokenProvider);
    final validationResult = await validateTokenUseCase(
      ValidateTokenParams(accessToken: token.accessToken!),
    );

    if (!mounted || _hasNavigated) return;

    validationResult.fold(
      (failure) {
        debugPrint('🔐 [SPLASH] Token validation failed: ${failure.message}');
        debugPrint('🔐 [SPLASH] Redirecting to welcome for re-authentication');
        _navigateToWelcome();
      },
      (isValid) {
        if (isValid) {
          debugPrint('🔐 [SPLASH] Token is valid! Navigating to jobs');
          _navigateToJobs();
        } else {
          debugPrint(
            '🔐 [SPLASH] Token validation returned false, redirecting to welcome',
          );
          _navigateToWelcome();
        }
      },
    );
  }

  void _navigateToJobs() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.go(AppRoutes.jobs);
    });
  }

  void _navigateToWelcome() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.go(AppRoutes.welcome);
    });
  }

  @override
  void dispose() {
    _logoController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    ref.listen<AuthState>(authViewModelProvider, (previous, next) {
      if (_hasNavigated || !mounted) return;

      if (next.isAuthenticated &&
          next.token != null &&
          next.token!.accessToken != null &&
          next.token!.accessToken!.isNotEmpty) {
        debugPrint(
          '🔐 [SPLASH] Auth state changed - valid token detected, navigating to jobs',
        );
        _navigateToJobs();
      }
    });

    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: FadeTransition(
          opacity: _logoFadeAnimation,
          child: Image.asset(
            'assets/images/logo/sigook-logo.png',
            width: 220,
          ),
        ),
      ),
    );
  }
}
