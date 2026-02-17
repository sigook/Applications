import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
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
  late Animation<double> _textFadeAnimation;

  bool _hasNavigated = false;
  bool _isProcessing = false;
  String _appVersion = '';

  @override
  void initState() {
    super.initState();

    _logoController = AnimationController(
      duration: const Duration(milliseconds: 1200),
      vsync: this,
    );

    _logoFadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _logoController,
      curve: const Interval(0.0, 0.6, curve: Curves.easeOut),
    ));

    _textFadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _logoController,
      curve: const Interval(0.5, 1.0, curve: Curves.easeOut),
    ));

    Future.delayed(const Duration(milliseconds: 120), () {
      if (mounted) _logoController.forward();
    });

    _loadAppVersion();

    // Auto-navigate after 3 seconds
    Future.delayed(const Duration(seconds: 3), () {
      if (mounted) _onLogoTapped();
    });
  }

  Future<void> _loadAppVersion() async {
    final packageInfo = await PackageInfo.fromPlatform();
    if (mounted) {
      setState(() {
        _appVersion = 'v${packageInfo.version} (${packageInfo.buildNumber})';
      });
    }
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
      (isValid) async {
        if (isValid) {
          debugPrint('🔐 [SPLASH] Token is valid! Checking user role...');
          await _checkRoleAndNavigate(token.accessToken!);
        } else {
          debugPrint(
            '🔐 [SPLASH] Token validation returned false, redirecting to welcome',
          );
          _navigateToWelcome();
        }
      },
    );
  }

  Future<void> _checkRoleAndNavigate(String accessToken) async {
    final authRepo = ref.read(authRepositoryProvider);
    final roleResult = await authRepo.getUserRole(accessToken);

    if (!mounted || _hasNavigated) return;

    roleResult.fold(
      (failure) {
        debugPrint('🔐 [SPLASH] Failed to fetch user role: ${failure.message}');
        // Allow access if role check fails (graceful degradation)
        _navigateToJobs();
      },
      (role) {
        if (role.toLowerCase() == 'worker') {
          debugPrint('🔐 [SPLASH] User role is worker - access granted');
          _navigateToJobs();
        } else {
          debugPrint('🔐 [SPLASH] User role is "$role" - access denied');
          // Logout and redirect to welcome
          ref.read(authViewModelProvider.notifier).logout();
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
    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Stack(
          children: [
            Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  FadeTransition(
                    opacity: _logoFadeAnimation,
                    child: Image.asset(
                      'assets/images/logo/sigook-logo.png',
                      width: 220,
                    ),
                  ),
                  const SizedBox(height: 40),
                  FadeTransition(
                    opacity: _textFadeAnimation,
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 40),
                      child: Column(
                        children: [
                          Text(
                            'Welcome to Sigook!',
                            style: TextStyle(
                              fontSize: 24,
                              fontWeight: FontWeight.bold,
                              color: Theme.of(context).primaryColor,
                            ),
                            textAlign: TextAlign.center,
                          ),
                          if (_isProcessing) ...[
                            const SizedBox(height: 24),
                            const SizedBox(
                              width: 24,
                              height: 24,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
            // Version display at bottom
            Positioned(
              bottom: 20,
              left: 0,
              right: 0,
              child: FadeTransition(
                opacity: _textFadeAnimation,
                child: Text(
                  _appVersion,
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey[400],
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
