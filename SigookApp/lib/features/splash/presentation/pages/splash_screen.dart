import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../auth/presentation/providers/auth_providers.dart';

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

    // Auto-navigate after 3 seconds
    Future.delayed(const Duration(seconds: 3), () {
      if (mounted) _onLogoTapped();
    });
  }

  Future<void> _onLogoTapped() async {
    if (_isProcessing || _hasNavigated) return;

    setState(() {
      _isProcessing = true;
    });

    debugPrint('🔐 [SPLASH] Logo tapped, starting authentication check...');

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

    debugPrint('🔐 [SPLASH] Token found, checking user role...');
    await _checkRoleAndNavigate(token.accessToken!);
  }

  Future<void> _checkRoleAndNavigate(String accessToken) async {
    final authRepo = ref.read(authRepositoryProvider);
    final roleResult = await authRepo.getUserRole(accessToken);

    if (!mounted || _hasNavigated) return;

    roleResult.fold(
      (failure) {
        debugPrint(
            '🔐 [SPLASH] Failed to fetch user role: ${failure.message}');
        _navigateToJobs();
      },
      (role) {
        if (role.toLowerCase() == 'worker') {
          debugPrint('🔐 [SPLASH] User role is worker - access granted');
          _navigateToJobs();
        } else {
          debugPrint('🔐 [SPLASH] User role is "$role" - access denied');
          ref.read(authViewModelProvider.notifier).logout();
          _navigateToWelcome();
        }
      },
    );
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
                FadeTransition(
                  opacity: _logoFadeAnimation,
                  child: ScaleTransition(
                    scale: _logoScaleAnimation,
                    child: Image.asset(
                      'assets/images/logo/sigook_logo.png',
                      width: 270,
                      color: Colors.white,
                      colorBlendMode: BlendMode.srcIn,
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
