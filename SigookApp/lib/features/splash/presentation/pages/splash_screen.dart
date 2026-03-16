import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
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
    with TickerProviderStateMixin {
  late AnimationController _logoController;
  late AnimationController _exitController;
  late Animation<double> _logoFadeAnimation;
  late Animation<double> _logoScaleAnimation;
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

    // Runs when navigating away — drives circle scatter + text fade-out.
    _exitController = AnimationController(
      duration: const Duration(milliseconds: 380),
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

    _logoScaleAnimation = Tween<double>(
      begin: 0.80,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _logoController,
      curve: const Interval(0.0, 0.7, curve: Curves.easeOutBack),
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

    debugPrint('🔐 [SPLASH] Token found, checking user role...');
    await _checkRoleAndNavigate(token.accessToken!);
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

  // Starts the scatter exit animation, then navigates after circles are in motion.
  void _navigateToJobs() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    _exitController.forward();
    Future.delayed(const Duration(milliseconds: 280), () {
      if (mounted) context.go(AppRoutes.jobs);
    });
  }

  void _navigateToWelcome() {
    if (!mounted || _hasNavigated) return;
    _hasNavigated = true;
    _exitController.forward();
    Future.delayed(const Duration(milliseconds: 280), () {
      if (mounted) context.go(AppRoutes.welcome);
    });
  }

  @override
  void dispose() {
    _logoController.dispose();
    _exitController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        fit: StackFit.expand,
        children: [
          // ── Full red background
          Container(color: AppTheme.secondaryRed),

          // ── Main content
          SafeArea(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                const Spacer(flex: 3),

                // Logo: scale-in + fade-in entrance; Hero flies it to welcome page
                FadeTransition(
                  opacity: _logoFadeAnimation,
                  child: ScaleTransition(
                    scale: _logoScaleAnimation,
                    child: Hero(
                      tag: 'sigook_logo',
                      child: Image.asset(
                        'assets/images/logo/sigook_logo.png',
                        width: 215,
                        color: Colors.white,
                        colorBlendMode: BlendMode.srcIn,
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 10),

                // Tagline — drifts upward and fades out on exit
                AnimatedBuilder(
                  animation: _exitController,
                  builder: (context, child) => Opacity(
                    opacity: (1.0 - _exitController.value).clamp(0.0, 1.0),
                    child: Transform.translate(
                      offset: Offset(0, -20 * _exitController.value),
                      child: child!,
                    ),
                  ),
                  child: FadeTransition(
                    opacity: _textFadeAnimation,
                    child: const Text(
                      'Find Work That Fits Your Life',
                      style: TextStyle(
                        fontSize: 15,
                        fontStyle: FontStyle.italic,
                        color: Colors.white,
                        fontWeight: FontWeight.w400,
                        letterSpacing: 0.3,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ),

                const Spacer(flex: 4),

                // Loading indicator — fades out on exit
                AnimatedBuilder(
                  animation: _exitController,
                  builder: (context, child) => Opacity(
                    opacity: (1.0 - _exitController.value).clamp(0.0, 1.0),
                    child: child!,
                  ),
                  child: FadeTransition(
                    opacity: _textFadeAnimation,
                    child: Padding(
                      padding: const EdgeInsets.only(bottom: 16),
                      child: _isProcessing
                          ? const SizedBox(
                              width: 22,
                              height: 22,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                valueColor: AlwaysStoppedAnimation<Color>(
                                  Colors.white54,
                                ),
                              ),
                            )
                          : const SizedBox(height: 22),
                    ),
                  ),
                ),

                // Version — fades out on exit
                AnimatedBuilder(
                  animation: _exitController,
                  builder: (context, child) => Opacity(
                    opacity: (1.0 - _exitController.value).clamp(0.0, 1.0),
                    child: child!,
                  ),
                  child: FadeTransition(
                    opacity: _textFadeAnimation,
                    child: Padding(
                      padding: const EdgeInsets.only(bottom: 20),
                      child: Text(
                        _appVersion,
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.white.withValues(alpha: 0.40),
                        ),
                        textAlign: TextAlign.center,
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
