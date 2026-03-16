import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';

class WelcomePage extends ConsumerStatefulWidget {
  const WelcomePage({super.key});

  @override
  ConsumerState<WelcomePage> createState() => _WelcomePageState();
}

class _WelcomePageState extends ConsumerState<WelcomePage>
    with TickerProviderStateMixin {
  late AnimationController _controller;
  late AnimationController _exitController;
  late Animation<double> _fadeAnimation;
  late Animation<double> _logoScale;
  late Animation<double> _buttonsFade;
  late Animation<Offset> _buttonsSlide;
  late Animation<Offset> _panelsSlide;
  late Animation<Offset> _legalSlide;

  bool _isNavigating = false;
  String _appVersion = '';

  @override
  void initState() {
    super.initState();

    _controller = AnimationController(
      duration: const Duration(milliseconds: 1200),
      vsync: this,
    );

    _exitController = AnimationController(
      duration: const Duration(milliseconds: 380),
      vsync: this,
    );

    _fadeAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.0, 0.5, curve: Curves.easeOut),
      ),
    );

    _logoScale = Tween<double>(begin: 0.8, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.0, 0.6, curve: Curves.easeOutBack),
      ),
    );

    _buttonsFade = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.4, 0.8, curve: Curves.easeOut),
      ),
    );

    _buttonsSlide = Tween<Offset>(
      begin: const Offset(0, 0.15),
      end: Offset.zero,
    ).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.4, 0.8, curve: Curves.easeOutCubic),
      ),
    );

    // Panels slide in from slightly above
    _panelsSlide = Tween<Offset>(
      begin: const Offset(0, -40),
      end: Offset.zero,
    ).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.0, 0.6, curve: Curves.easeOutCubic),
      ),
    );

    // Legal icon slides in from the right
    _legalSlide = Tween<Offset>(
      begin: const Offset(60, 0),
      end: Offset.zero,
    ).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.3, 0.85, curve: Curves.easeOutCubic),
      ),
    );

    _controller.forward();
    _loadAppVersion();
  }

  Future<void> _loadAppVersion() async {
    final packageInfo = await PackageInfo.fromPlatform();
    if (mounted) {
      setState(() {
        _appVersion = 'v${packageInfo.version} (${packageInfo.buildNumber})';
      });
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    _exitController.dispose();
    super.dispose();
  }

  Future<void> _navigateWithExit(String route) async {
    if (_isNavigating || !mounted) return;
    _isNavigating = true;
    _exitController.forward();
    await Future.delayed(const Duration(milliseconds: 280));
    if (mounted) context.go(route);
  }

  void _navigateToRegistration() {
    _navigateWithExit(AppRoutes.registration);
  }

  Future<void> _signIn() async {
    await ref.read(authViewModelProvider.notifier).signIn();
  }

  void _showLegalModal() {
    showModalBottomSheet<void>(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (_) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        ),
        padding: const EdgeInsets.fromLTRB(24, 12, 24, 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Drag handle
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: Colors.grey[300],
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'Legal',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 8),
            const Divider(),
            ListTile(
              leading: Icon(Icons.privacy_tip_outlined, color: AppTheme.secondaryRed),
              title: const Text('Privacy Policy'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () {
                Navigator.pop(context);
                context.push(AppRoutes.privacyPolicy);
              },
            ),
            ListTile(
              leading: Icon(Icons.description_outlined, color: AppTheme.secondaryRed),
              title: const Text('Terms & Conditions'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () {
                Navigator.pop(context);
                context.push(AppRoutes.terms);
              },
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final authState = ref.watch(authViewModelProvider);
    final size = MediaQuery.of(context).size;

    ref.listen(authViewModelProvider, (previous, next) {
      if (next.error != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(next.error!),
            backgroundColor: AppTheme.errorRed,
          ),
        );
      }

      if (previous?.isAuthenticated != true &&
          next.isAuthenticated &&
          next.token != null &&
          next.token!.accessToken != null &&
          next.token!.accessToken!.isNotEmpty) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Sign in successful!'),
            backgroundColor: AppTheme.successGreen,
          ),
        );
        _navigateWithExit(AppRoutes.jobs);
      }
    });

    return Scaffold(
      body: Stack(
        fit: StackFit.expand,
        children: [
          // ── Top hero section — diagonal split image panels
          Positioned(
            top: 0,
            left: 0,
            right: 0,
            height: size.height * 0.44,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: AnimatedBuilder(
                animation: _panelsSlide,
                builder: (context, child) => Transform.translate(
                  offset: _panelsSlide.value,
                  child: child!,
                ),
                child: Stack(
                  fit: StackFit.expand,
                  children: [
                    // Right panel — family image (full width, base layer)
                    Image.asset(
                      'assets/images/welcome-screen/family.png',
                      width: double.infinity,
                      height: double.infinity,
                      fit: BoxFit.cover,
                      alignment: Alignment.topRight,
                    ),
                    // Left panel — worker image clipped to diagonal shape
                    ClipPath(
                      clipper: const _DiagonalLeftClipper(),
                      child: Image.asset(
                        'assets/images/welcome-screen/worker.png',
                        width: double.infinity,
                        height: double.infinity,
                        fit: BoxFit.cover,
                        alignment: Alignment.topLeft,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),

          // ── Gradient fade — blends the hero image into the red section below
          Positioned(
            top: size.height * 0.18,
            left: 0,
            right: 0,
            height: size.height * 0.28,
            child: IgnorePointer(
              child: Container(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    stops: const [0.0, 0.55, 1.0],
                    colors: [
                      AppTheme.secondaryRed.withValues(alpha: 0.0),
                      AppTheme.secondaryRed.withValues(alpha: 0.55),
                      AppTheme.secondaryRed,
                    ],
                  ),
                ),
              ),
            ),
          ),

          // ── Bottom solid red section
          Positioned(
            top: size.height * 0.43,
            left: 0,
            right: 0,
            bottom: 0,
            child: Container(color: AppTheme.secondaryRed),
          ),

          // ── Decorative circles — bottom left
          // Ring circle (border only, behind)
          Positioned(
            bottom: -28,
            left: -42,
            child: IgnorePointer(
              child: FadeTransition(
                opacity: _fadeAnimation,
                child: _RingCircle(
                  diameter: 140,
                  color: Colors.white.withValues(alpha: 0.30),
                  strokeWidth: 2.0,
                ),
              ),
            ),
          ),
          // Filled translucent circle (in front)
          Positioned(
            bottom: 4,
            left: -22,
            child: IgnorePointer(
              child: FadeTransition(
                opacity: _fadeAnimation,
                child: Container(
                  width: 72,
                  height: 72,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: const Color(0xFFFFCDD2).withValues(alpha: 0.55),
                  ),
                ),
              ),
            ),
          ),

          // ── Legal dot-grid button — slides in from the right
          Positioned(
            bottom: 24,
            right: 20,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: AnimatedBuilder(
                animation: _legalSlide,
                builder: (context, child) => Transform.translate(
                  offset: _legalSlide.value,
                  child: child!,
                ),
                child: GestureDetector(
                  onTap: _showLegalModal,
                  behavior: HitTestBehavior.opaque,
                  child: Padding(
                    padding: const EdgeInsets.all(8),
                    child: CustomPaint(
                      size: const Size(50, 26),
                      painter: const _DotGridPainter(),
                    ),
                  ),
                ),
              ),
            ),
          ),

          // ── Main content layer
          SafeArea(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                // Push content down to the boundary between the two sections
                SizedBox(height: size.height * 0.36),

                // Logo — Hero sits outside ScaleTransition so its landing
                // position is stable and the flight path is clean.
                FadeTransition(
                  opacity: _fadeAnimation,
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

                // Tagline — scale-in entrance independent of the Hero
                ScaleTransition(
                  scale: _logoScale,
                  child: FadeTransition(
                    opacity: _fadeAnimation,
                    child: const Padding(
                      padding: EdgeInsets.only(top: 10),
                      child: Text(
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
                ),

                const Spacer(),

                // Buttons
                FadeTransition(
                  opacity: _buttonsFade,
                  child: SlideTransition(
                    position: _buttonsSlide,
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 28),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          // Get Started — white fill, red text
                          SizedBox(
                            width: double.infinity,
                            height: 56,
                            child: ElevatedButton(
                              onPressed: _navigateToRegistration,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.white,
                                foregroundColor: AppTheme.secondaryRed,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(32),
                                ),
                                elevation: 0,
                              ),
                              child: const Text(
                                'Get Started',
                                style: TextStyle(
                                  fontSize: 17,
                                  fontWeight: FontWeight.w700,
                                  letterSpacing: 0.3,
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(height: 14),
                          // Already have an account row
                          Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Text(
                                'Already have an account?',
                                style: TextStyle(
                                  color: Colors.white.withValues(alpha: 0.85),
                                  fontSize: 14,
                                ),
                              ),
                              TextButton(
                                onPressed: authState.isLoading ? null : _signIn,
                                style: TextButton.styleFrom(
                                  foregroundColor: Colors.white,
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 8,
                                    vertical: 4,
                                  ),
                                  minimumSize: Size.zero,
                                  tapTargetSize:
                                      MaterialTapTargetSize.shrinkWrap,
                                ),
                                child: authState.isLoading
                                    ? const SizedBox(
                                        width: 18,
                                        height: 18,
                                        child: CircularProgressIndicator(
                                          strokeWidth: 2,
                                          valueColor:
                                              AlwaysStoppedAnimation<Color>(
                                            Colors.white,
                                          ),
                                        ),
                                      )
                                    : const Text(
                                        'Sign In',
                                        style: TextStyle(
                                          fontSize: 14,
                                          fontWeight: FontWeight.w700,
                                        ),
                                      ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 4),

                // App version
                FadeTransition(
                  opacity: _buttonsFade,
                  child: Padding(
                    padding: const EdgeInsets.only(bottom: 20),
                    child: Text(
                      _appVersion,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 11,
                        color: Colors.white.withValues(alpha: 0.4),
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

/// Clips the worker (left) image panel with a diagonal right edge.
/// Diagonal runs from (75% width, top) down to (25% width, bottom).
class _DiagonalLeftClipper extends CustomClipper<Path> {
  const _DiagonalLeftClipper();

  @override
  Path getClip(Size size) {
    final path = Path();
    path.moveTo(0, 0);
    path.lineTo(size.width * 0.62, 0);           // top of diagonal
    path.lineTo(size.width * 0.38, size.height); // bottom of diagonal
    path.lineTo(0, size.height);
    path.close();
    return path;
  }

  @override
  bool shouldReclip(_DiagonalLeftClipper old) => false;
}


/// Hollow ring used as a decorative accent.
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

/// Draws a 4×3 grid of small rounded squares — the legal/menu trigger icon.
class _DotGridPainter extends CustomPainter {
  const _DotGridPainter();

  @override
  void paint(Canvas canvas, Size size) {
    const cols = 6;
    const rows = 3;
    const dotSize = 4.5;
    const radius = 1.2;
    final color = Colors.white.withValues(alpha: 0.65);
    final paint = Paint()..color = color;

    final hSpacing = size.width / cols;
    final vSpacing = size.height / rows;

    for (int r = 0; r < rows; r++) {
      for (int c = 0; c < cols; c++) {
        final cx = hSpacing * c + hSpacing / 2 - dotSize / 2;
        final cy = vSpacing * r + vSpacing / 2 - dotSize / 2;
        canvas.drawRRect(
          RRect.fromRectAndRadius(
            Rect.fromLTWH(cx, cy, dotSize, dotSize),
            const Radius.circular(radius),
          ),
          paint,
        );
      }
    }
  }

  @override
  bool shouldRepaint(_DotGridPainter old) => false;
}
