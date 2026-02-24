import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
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
    final size = MediaQuery.of(context).size;

    return Scaffold(
      body: Stack(
        fit: StackFit.expand,
        children: [
          // ── Full blue background
          Container(color: AppTheme.primaryBlue),

          // ── Background decorative circles — spread across the whole screen

          // Top-right: large blue ring (partially off-screen)
          Positioned(
            top: -50,
            right: -60,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: _RingCircle(
                diameter: 210,
                color: const Color(0xFF90CAF9).withValues(alpha: 0.22),
                strokeWidth: 20,
              ),
            ),
          ),
          // Top-left: medium red filled circle
          Positioned(
            top: 50,
            left: -28,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: Container(
                width: 95,
                height: 95,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: const Color(0xFFEF9A9A).withValues(alpha: 0.22),
                ),
              ),
            ),
          ),
          // Top-right area: small blue filled circle
          Positioned(
            top: 90,
            right: 28,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: Container(
                width: 38,
                height: 38,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: const Color(0xFF90CAF9).withValues(alpha: 0.28),
                ),
              ),
            ),
          ),
          // Mid-left: tiny red circle
          Positioned(
            top: size.height * 0.46,
            left: 18,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: Container(
                width: 24,
                height: 24,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: const Color(0xFFEF9A9A).withValues(alpha: 0.30),
                ),
              ),
            ),
          ),
          // Mid-right: small blue ring
          Positioned(
            top: size.height * 0.52,
            right: -18,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: _RingCircle(
                diameter: 80,
                color: const Color(0xFF90CAF9).withValues(alpha: 0.20),
                strokeWidth: 10,
              ),
            ),
          ),
          // Bottom-left: red ring + blue ring
          Positioned(
            bottom: -55,
            left: -65,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: _RingCircle(
                diameter: 190,
                color: const Color(0xFFEF9A9A).withValues(alpha: 0.40),
                strokeWidth: 24,
              ),
            ),
          ),
          Positioned(
            bottom: -85,
            left: -95,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: _RingCircle(
                diameter: 250,
                color: const Color(0xFF90CAF9).withValues(alpha: 0.22),
                strokeWidth: 18,
              ),
            ),
          ),
          // Bottom-right: small red filled circle
          Positioned(
            bottom: 85,
            right: 28,
            child: FadeTransition(
              opacity: _logoFadeAnimation,
              child: Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: const Color(0xFFEF9A9A).withValues(alpha: 0.28),
                ),
              ),
            ),
          ),

          // ── Main content
          SafeArea(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                const Spacer(flex: 3),

                // Logo: scale-in + fade-in entrance
                FadeTransition(
                  opacity: _logoFadeAnimation,
                  child: ScaleTransition(
                    scale: _logoScaleAnimation,
                    child: SizedBox(
                      width: 260,
                      height: 200,
                      child: Stack(
                        clipBehavior: Clip.none,
                        alignment: Alignment.center,
                        children: [
                          Positioned(
                            top: 8,
                            left: 12,
                            child: Container(
                              width: 62,
                              height: 62,
                              decoration: BoxDecoration(
                                shape: BoxShape.circle,
                                color: const Color(0xFF90CAF9).withValues(alpha: 0.45),
                              ),
                            ),
                          ),
                          Positioned(
                            bottom: 10,
                            right: 14,
                            child: Container(
                              width: 52,
                              height: 52,
                              decoration: BoxDecoration(
                                shape: BoxShape.circle,
                                color: const Color(0xFFEF9A9A).withValues(alpha: 0.50),
                              ),
                            ),
                          ),
                          Positioned(
                            top: 18,
                            right: 8,
                            child: Container(
                              width: 32,
                              height: 32,
                              decoration: BoxDecoration(
                                shape: BoxShape.circle,
                                color: const Color(0xFF90CAF9).withValues(alpha: 0.35),
                              ),
                            ),
                          ),
                          Positioned(
                            bottom: 18,
                            left: 18,
                            child: Container(
                              width: 26,
                              height: 26,
                              decoration: BoxDecoration(
                                shape: BoxShape.circle,
                                color: const Color(0xFFEF9A9A).withValues(alpha: 0.40),
                              ),
                            ),
                          ),
                          Image.asset(
                            'assets/images/logo/sigook_logo.png',
                            width: 215,
                            color: Colors.white,
                            colorBlendMode: BlendMode.srcIn,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 10),

                // Tagline
                FadeTransition(
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

                const Spacer(flex: 4),

                // Loading indicator (only while checking auth)
                FadeTransition(
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

                // Version
                FadeTransition(
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
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _RingCircle extends StatelessWidget {
  final double diameter;
  final Color color;
  final double strokeWidth;

  const _RingCircle({
    required this.diameter,
    required this.color,
    required this.strokeWidth,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: diameter,
      height: diameter,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(color: color, width: strokeWidth),
      ),
    );
  }
}
