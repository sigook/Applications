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
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _fadeAnimation;
  late Animation<double> _logoScale;
  late Animation<double> _buttonsFade;
  late Animation<Offset> _buttonsSlide;

  String _appVersion = '';

  @override
  void initState() {
    super.initState();

    _controller = AnimationController(
      duration: const Duration(milliseconds: 1200),
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
    super.dispose();
  }

  void _navigateToRegistration() {
    context.go(AppRoutes.registration);
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
              leading: Icon(Icons.privacy_tip_outlined, color: AppTheme.primaryBlue),
              title: const Text('Privacy Policy'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () {
                // TODO: navigate to Privacy Policy URL
                Navigator.pop(context);
              },
            ),
            ListTile(
              leading: Icon(Icons.description_outlined, color: AppTheme.primaryBlue),
              title: const Text('Terms & Conditions'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () {
                // TODO: navigate to Terms & Conditions URL
                Navigator.pop(context);
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

      if (next.isAuthenticated &&
          next.token != null &&
          next.token!.accessToken != null &&
          next.token!.accessToken!.isNotEmpty) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Sign in successful!'),
            backgroundColor: AppTheme.successGreen,
          ),
        );
        context.go(AppRoutes.jobs);
      }
    });

    return Scaffold(
      body: Stack(
        fit: StackFit.expand,
        children: [
          // ── Top hero section — two gradient panels separated by a white line
          Positioned(
            top: 0,
            left: 0,
            right: 0,
            height: size.height * 0.44,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: Row(
                children: [
                  // Left panel — dark blue
                  Expanded(
                    child: Container(
                      decoration: const BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                          colors: [Color(0xFF0D47A1), Color(0xFF1565C0)],
                        ),
                      ),
                      child: const Center(
                        child: Icon(
                          Icons.engineering_outlined,
                          size: 90,
                          color: Colors.white12,
                        ),
                      ),
                    ),
                  ),
                  // White vertical divider
                  Container(width: 2, color: Colors.white),
                  // Right panel — medium blue
                  Expanded(
                    child: Container(
                      decoration: const BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topRight,
                          end: Alignment.bottomLeft,
                          colors: [Color(0xFF1E88E5), Color(0xFF1565C0)],
                        ),
                      ),
                      child: const Center(
                        child: Icon(
                          Icons.people_alt_outlined,
                          size: 90,
                          color: Colors.white12,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),

          // ── Gradient fade — blends the hero panels into the blue section below
          Positioned(
            top: size.height * 0.30,
            left: 0,
            right: 0,
            height: size.height * 0.16,
            child: IgnorePointer(
              child: Container(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [
                      AppTheme.primaryBlue.withValues(alpha: 0.0),
                      AppTheme.primaryBlue,
                    ],
                  ),
                ),
              ),
            ),
          ),

          // ── Bottom solid blue section
          Positioned(
            top: size.height * 0.43,
            left: 0,
            right: 0,
            bottom: 0,
            child: Container(color: AppTheme.primaryBlue),
          ),

          // ── Decorative ring circles — bottom left (inner: soft red, outer: soft blue)
          Positioned(
            bottom: -55,
            left: -65,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: _RingCircle(
                diameter: 190,
                color: const Color(0xFFEF9A9A).withValues(alpha: 0.40), // Red[200]
                strokeWidth: 24,
              ),
            ),
          ),
          Positioned(
            bottom: -85,
            left: -95,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: _RingCircle(
                diameter: 250,
                color: const Color(0xFF90CAF9).withValues(alpha: 0.22), // Blue[300]
                strokeWidth: 18,
              ),
            ),
          ),

          // ── Legal pill button — bottom right
          Positioned(
            bottom: 32,
            right: 20,
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: OutlinedButton.icon(
                onPressed: _showLegalModal,
                icon: const Icon(Icons.shield_outlined, size: 14),
                label: const Text('Legal'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: Colors.white.withValues(alpha: 0.65),
                  side: BorderSide(
                    color: Colors.white.withValues(alpha: 0.30),
                    width: 1.0,
                  ),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 8,
                  ),
                  shape: const StadiumBorder(),
                  minimumSize: Size.zero,
                  tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  textStyle: const TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w500,
                    letterSpacing: 0.3,
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

                // Logo + tagline
                FadeTransition(
                  opacity: _fadeAnimation,
                  child: ScaleTransition(
                    scale: _logoScale,
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        // Logo with scattered decorative circles in the background
                        SizedBox(
                          width: 260,
                          height: 200,
                          child: Stack(
                            clipBehavior: Clip.none,
                            alignment: Alignment.center,
                            children: [
                              // Upper-left — soft blue filled circle
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
                              // Lower-right — soft red filled circle
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
                              // Upper-right — small blue circle
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
                              // Lower-left — small red circle
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
                              // White logo centred
                              Image.asset(
                                'assets/images/logo/sigook_logo.png',
                                width: 215,
                                color: Colors.white,
                                colorBlendMode: BlendMode.srcIn,
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 10),
                        const Text(
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
                      ],
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
                          // Get Started — white fill, blue text
                          SizedBox(
                            width: double.infinity,
                            height: 56,
                            child: ElevatedButton(
                              onPressed: _navigateToRegistration,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.white,
                                foregroundColor: AppTheme.primaryBlue,
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

/// Hollow ring circle used as a decorative accent.
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

