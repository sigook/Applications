import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'dart:async';
import 'dart:math' as math;
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
    with TickerProviderStateMixin {
  late AnimationController _logoController;
  late AnimationController _blobController;
  late AnimationController _loadingController;

  late Animation<double> _logoFadeAnimation;
  late Animation<double> _logoScaleAnimation;
  late Animation<double> _blobAnimation;

  bool _hasNavigated = false;

  @override
  void initState() {
    super.initState();

    _logoController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _blobController = AnimationController(
      duration: const Duration(milliseconds: 9000),
      vsync: this,
    );

    _loadingController = AnimationController(
      duration: const Duration(milliseconds: 1400),
      vsync: this,
    );

    _logoFadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(parent: _logoController, curve: Curves.easeOut));

    _logoScaleAnimation = Tween<double>(begin: 0.7, end: 1.0).animate(
      CurvedAnimation(parent: _logoController, curve: Curves.easeOutBack),
    );

    _blobAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(parent: _blobController, curve: Curves.easeInOut),
    );

    _startAnimationSequence();

    _checkAuthAndNavigate();
  }

  void _startAnimationSequence() async {
    await Future.delayed(const Duration(milliseconds: 120));
    if (!mounted) return;
    _logoController.forward();

    await Future.delayed(const Duration(milliseconds: 380));
    if (!mounted) return;
    _blobController.repeat();

    await Future.delayed(const Duration(milliseconds: 220));
    if (!mounted) return;
    _loadingController.repeat();
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
    _blobController.dispose();
    _loadingController.dispose();
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
      backgroundColor: const Color(0xFF070A16),
      body: Stack(
        children: [
          Positioned.fill(
            child: DecoratedBox(
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    const Color(0xFF070A16),
                    AppTheme.primaryBlue.withValues(alpha: 0.25),
                    const Color(0xFF070A16),
                  ],
                  stops: const [0.0, 0.55, 1.0],
                ),
              ),
            ),
          ),
          Positioned.fill(
            child: AnimatedBuilder(
              animation: _blobController,
              builder: (context, child) {
                return CustomPaint(
                  painter: _BlobPainter(twinkle: _blobAnimation.value),
                );
              },
            ),
          ),
          SafeArea(
            child: Center(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    FadeTransition(
                      opacity: _logoFadeAnimation,
                      child: ScaleTransition(
                        scale: _logoScaleAnimation,
                        child: _buildWordmark(),
                      ),
                    ),
                    const SizedBox(height: 44),
                    AnimatedBuilder(
                      animation: _loadingController,
                      builder: (context, child) {
                        final t = _loadingController.value * math.pi * 2;
                        return Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            _LoadingDot(v: math.sin(t)),
                            const SizedBox(width: 10),
                            _LoadingDot(v: math.sin(t - 2.1)),
                            const SizedBox(width: 10),
                            _LoadingDot(v: math.sin(t - 4.2)),
                          ],
                        );
                      },
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildWordmark() {
    const baseStyle = TextStyle(
      fontSize: 84,
      fontWeight: FontWeight.w800,
      height: 1.0,
      letterSpacing: -2,
    );

    final sigPaint = Paint()
      ..shader = const LinearGradient(
        begin: Alignment.topCenter,
        end: Alignment.bottomCenter,
        colors: [Color(0xFF7DD3FC), Color(0xFF2563EB)],
      ).createShader(const Rect.fromLTWH(0, 0, 320, 120));

    final ookPaint = Paint()
      ..shader = const LinearGradient(
        begin: Alignment.topCenter,
        end: Alignment.bottomCenter,
        colors: [Color(0xFFFFB4B4), Color(0xFFE53935)],
      ).createShader(const Rect.fromLTWH(0, 0, 320, 120));

    return RichText(
      textAlign: TextAlign.center,
      text: TextSpan(
        style: baseStyle,
        children: [
          TextSpan(
            text: 'sig',
            style: baseStyle.copyWith(
              foreground: sigPaint,
              shadows: [
                Shadow(
                  color: Colors.black.withValues(alpha: 0.4),
                  blurRadius: 24,
                  offset: const Offset(0, 16),
                ),
              ],
            ),
          ),
          TextSpan(
            text: 'ook',
            style: baseStyle.copyWith(
              foreground: ookPaint,
              shadows: [
                Shadow(
                  color: Colors.black.withValues(alpha: 0.35),
                  blurRadius: 24,
                  offset: const Offset(0, 16),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _LoadingDot extends StatelessWidget {
  final double v;

  const _LoadingDot({required this.v});

  @override
  Widget build(BuildContext context) {
    final t = (v + 1) / 2;
    final scale = 0.75 + 0.35 * t;
    final opacity = 0.35 + 0.55 * t;
    return Opacity(
      opacity: opacity,
      child: Transform.scale(
        scale: scale,
        child: Container(
          width: 8,
          height: 8,
          decoration: BoxDecoration(
            color: Colors.white,
            shape: BoxShape.circle,
            boxShadow: [
              BoxShadow(
                color: Colors.white.withValues(alpha: 0.15),
                blurRadius: 10,
                spreadRadius: 1,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _BlobPainter extends CustomPainter {
  final double twinkle;

  _BlobPainter({required this.twinkle});

  @override
  void paint(Canvas canvas, Size size) {
    final t = twinkle * math.pi * 2;

    final blobPaint = Paint()..style = PaintingStyle.fill;
    blobPaint.maskFilter = const MaskFilter.blur(BlurStyle.normal, 60);

    final b1 = Offset(
      size.width * 0.18 + math.sin(t) * 22,
      size.height * 0.25 + math.cos(t) * 18,
    );
    blobPaint.color = AppTheme.primaryBlue.withValues(alpha: 0.28);
    canvas.drawCircle(b1, size.shortestSide * 0.28, blobPaint);

    final b2 = Offset(
      size.width * 0.88 + math.sin(t + 1.4) * 26,
      size.height * 0.35 + math.cos(t + 1.4) * 22,
    );
    blobPaint.color = AppTheme.tertiaryBlue.withValues(alpha: 0.22);
    canvas.drawCircle(b2, size.shortestSide * 0.22, blobPaint);

    final b3 = Offset(
      size.width * 0.62 + math.sin(t + 2.7) * 18,
      size.height * 0.86 + math.cos(t + 2.7) * 16,
    );
    blobPaint.color = AppTheme.secondaryRed.withValues(alpha: 0.12);
    canvas.drawCircle(b3, size.shortestSide * 0.26, blobPaint);
  }

  @override
  bool shouldRepaint(covariant _BlobPainter oldDelegate) {
    return oldDelegate.twinkle != twinkle;
  }
}
