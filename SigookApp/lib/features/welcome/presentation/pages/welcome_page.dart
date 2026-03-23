import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_svg/flutter_svg.dart';
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
  late Animation<double> _panelsFade;
  late Animation<double> _taglineFade;
  late Animation<double> _buttonsFade;
  late Animation<Offset> _buttonsSlide;
  late Animation<Offset> _panelsSlide;
  late Animation<Offset> _legalSlide;
  late Animation<double> _circlesFade;

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

    // Image panels fade in first
    _panelsFade = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.0, 0.5, curve: Curves.easeOut),
      ),
    );

    // Tagline appears after a short delay
    _taglineFade = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.15, 0.55, curve: Curves.easeOut),
      ),
    );

    // Buttons fade and slide in
    _buttonsFade = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.3, 0.7, curve: Curves.easeOut),
      ),
    );

    _buttonsSlide =
        Tween<Offset>(begin: const Offset(0, 0.15), end: Offset.zero).animate(
          CurvedAnimation(
            parent: _controller,
            curve: const Interval(0.3, 0.7, curve: Curves.easeOutCubic),
          ),
        );

    // Panels slide in from slightly above
    _panelsSlide = Tween<Offset>(begin: const Offset(0, -40), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _controller,
            curve: const Interval(0.0, 0.6, curve: Curves.easeOutCubic),
          ),
        );

    // Legal icon slides in from the right
    _legalSlide = Tween<Offset>(begin: const Offset(60, 0), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _controller,
            curve: const Interval(0.3, 0.85, curve: Curves.easeOutCubic),
          ),
        );

    // Decorative circles fade in
    _circlesFade = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.2, 0.7, curve: Curves.easeOut),
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
              leading: Icon(
                Icons.privacy_tip_outlined,
                color: AppTheme.secondaryRed,
              ),
              title: const Text('Privacy Policy'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () {
                Navigator.pop(context);
                context.push(AppRoutes.privacyPolicy);
              },
            ),
            ListTile(
              leading: Icon(
                Icons.description_outlined,
                color: AppTheme.secondaryRed,
              ),
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
          // ── Full red background (matches splash, visible before panels animate in)
          Container(color: AppTheme.secondaryRed),

          // ── Top hero section — vertical split image panels
          Positioned(
            top: 0,
            left: 0,
            right: 0,
            height: size.height * 0.44,
            child: FadeTransition(
              opacity: _panelsFade,
              child: AnimatedBuilder(
                animation: _panelsSlide,
                builder: (context, child) => Transform.translate(
                  offset: _panelsSlide.value,
                  child: child!,
                ),
                child: Row(
                  children: [
                    // Welcome Image
                    Expanded(
                      child: Image.asset(
                        'assets/images/welcome-screen/welcome-page.png',
                        height: double.infinity,
                        fit: BoxFit.fill,
                        alignment: const Alignment(0.0, 0.0),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),

          // ── Gradient fade — blends the hero image smoothly into the red section
          Positioned(
            top: size.height * 0.10,
            left: 0,
            right: 0,
            height: size.height * 0.38,
            child: IgnorePointer(
              child: Container(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    stops: const [0.0, 0.15, 0.50, 0.85, 1.0],
                    colors: [
                      AppTheme.secondaryRed.withValues(alpha: 0.0),
                      AppTheme.secondaryRed.withValues(alpha: 0.15),
                      AppTheme.secondaryRed.withValues(alpha: 0.60),
                      AppTheme.secondaryRed.withValues(alpha: 0.99),
                      AppTheme.secondaryRed,
                    ],
                  ),
                ),
              ),
            ),
          ),

          // ── Bottom solid red section
          Positioned(
            top: size.height * 0.45,
            left: 0,
            right: 0,
            bottom: 0,
            child: Container(color: AppTheme.secondaryRed),
          ),

          // ── Decorative circles — bottom left (larger, more visible)
          Positioned(
            bottom: -45,
            left: -35,
            child: IgnorePointer(
              child: FadeTransition(
                opacity: _circlesFade,
                child: SizedBox(
                  width: 170,
                  height: 170,
                  child: Stack(
                    clipBehavior: Clip.none,
                    children: [
                      // Large ring from SVG
                      Opacity(
                        opacity: 0.90,
                        child: SvgPicture.asset(
                          'assets/images/welcome-screen/circles.svg',
                          width: 170,
                          height: 170,
                        ),
                      ),
                      // Small filled translucent circle (top-right of the ring)
                      Positioned(
                        top: 0,
                        left: 100,
                        child: Container(
                          width: 48,
                          height: 48,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            color: Colors.white.withValues(alpha: 0.25),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),

          // ── Legal dots-rectangle button — bottom right
          Positioned(
            bottom: 20,
            right: 16,
            child: Opacity(
              opacity: 0.750,
              child: FadeTransition(
                opacity: _panelsFade,
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
                      child: SvgPicture.asset(
                        'assets/images/welcome-screen/dots-rectangle.svg',
                        width: 50,
                        height: 20,
                      ),
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
                // Push content down to match splash logo position
                SizedBox(height: size.height * 0.36),

                // Logo — Hero-animated from splash screen
                Hero(
                  tag: 'app-logo',
                  child: Image.asset(
                    'assets/images/logo/sigook-logo.png',
                    color: Colors.white,
                    width: 270,
                    colorBlendMode: BlendMode.srcIn,
                  ),
                ),
                const SizedBox(height: 12),
                // Tagline fades in after logo lands
                FadeTransition(
                  opacity: _taglineFade,
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

                // Buttons — right below the tagline
                Transform.translate(
                  offset: const Offset(0, 20),
                  child: FadeTransition(
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
                              width: 300,
                              height: 56,
                              child: ElevatedButton(
                                onPressed: _navigateToRegistration,
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: Colors.white,
                                  foregroundColor: AppTheme.secondaryRed,
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(14),
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
                            const SizedBox(height: 7),
                            // Already have an account row
                            SizedBox(
                              width: 300,
                              child: Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  SizedBox(width: 10),
                                  Text(
                                    'Already have an account?',
                                    style: TextStyle(
                                      color: Colors.white.withValues(
                                        alpha: 0.85,
                                      ),
                                      fontSize: 16,
                                    ),
                                  ),
                                  Spacer(),
                                  TextButton(
                                    onPressed: authState.isLoading
                                        ? null
                                        : _signIn,
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
                                  SizedBox(width: 10),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                ),

                const Spacer(),

                // App version — pinned at the very bottom
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
