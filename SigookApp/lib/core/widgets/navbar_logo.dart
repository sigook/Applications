import 'package:flutter/material.dart';

/// Global notifier that fires whenever any tap occurs in the app.
/// NavbarLogo listens to this to trigger its flash animation.
final globalTapNotifier = ValueNotifier<int>(0);

/// A navbar logo widget that displays the white header logo by default
/// and briefly cross-fades to the colored logo whenever a tap occurs
/// anywhere in the app (via [globalTapNotifier]).
class NavbarLogo extends StatefulWidget {
  final double height;

  const NavbarLogo({
    super.key,
    this.height = 32,
  });

  @override
  State<NavbarLogo> createState() => _NavbarLogoState();
}

class _NavbarLogoState extends State<NavbarLogo>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _animation;
  double _opacity = 0.0;
  bool _isFlashing = false;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(milliseconds: 350),
      vsync: this,
    );
    _animation = CurvedAnimation(
      parent: _controller,
      curve: Curves.easeOut,
      reverseCurve: Curves.easeIn,
    );
    _controller.addListener(() {
      setState(() {
        _opacity = _animation.value;
      });
    });
    globalTapNotifier.addListener(_onGlobalTap);
  }

  void _onGlobalTap() {
    if (!mounted) return;
    _flash();
  }

  void _flash() {
    if (_isFlashing) return;
    _isFlashing = true;
    _controller.forward().then((_) {
      if (!mounted) return;
      Future.delayed(const Duration(milliseconds: 250), () {
        if (!mounted) return;
        _controller.reverse().then((_) {
          _isFlashing = false;
        });
      });
    });
  }

  @override
  void dispose() {
    globalTapNotifier.removeListener(_onGlobalTap);
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: widget.height,
      child: Stack(
        alignment: Alignment.center,
        children: [
          Opacity(
            opacity: 1.0 - _opacity,
            child: Image.asset(
              'assets/images/logo/header-logo.png',
              height: widget.height,
            ),
          ),
          Opacity(
            opacity: _opacity,
            child: Image.asset(
              'assets/images/logo/sigook-logo.png',
              height: widget.height,
            ),
          ),
        ],
      ),
    );
  }
}
