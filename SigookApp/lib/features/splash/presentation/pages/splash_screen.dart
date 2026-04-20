import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';

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

  static const int _maxRetries = 5;
  static const int _retryDelaySeconds = 10;

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

    Future.delayed(const Duration(seconds: 3), () {
      if (mounted) _onLogoTapped();
    });
  }

  Future<void> _onLogoTapped() async {
    if (_isProcessing || _hasNavigated) return;

    setState(() => _isProcessing = true);

    debugPrint('🔐 [SPLASH] Starting authentication check...');

    // Wait for AuthViewModel to finish loading token from secure storage
    int attempts = 0;
    const maxWaitAttempts = 50;
    bool isInitialized = false;
    while (!isInitialized && attempts < maxWaitAttempts && mounted) {
      await Future.delayed(const Duration(milliseconds: 100));
      attempts++;
      isInitialized = ref.read(authViewModelProvider.notifier).isInitialized;
    }

    debugPrint('🔐 [SPLASH] Token loading completed after ${attempts * 100}ms');

    if (!mounted || _hasNavigated) return;

    final token = ref.read(authViewModelProvider).token;

    if (token == null || token.accessToken == null || token.accessToken!.isEmpty) {
      debugPrint('🔐 [SPLASH] No token found, redirecting to welcome');
      _navigateToWelcome();
      return;
    }

    debugPrint('🔐 [SPLASH] Token found, validating with server...');
    await _validateAndNavigate(token.accessToken!);
  }

  Future<void> _validateAndNavigate(String accessToken) async {
    for (int attempt = 0; attempt < _maxRetries; attempt++) {
      if (!mounted || _hasNavigated) return;

      final roleResult = await ref.read(authRepositoryProvider).getUserRole(accessToken);

      if (!mounted || _hasNavigated) return;

      bool handled = false;

      roleResult.fold(
        (failure) {
          if (failure is ServerFailure &&
              (failure.statusCode == 401 || failure.statusCode == 403)) {
            debugPrint('🔐 [SPLASH] Token rejected by server (${failure.statusCode}), clearing session');
            ref.read(authViewModelProvider.notifier).logout();
            _navigateToWelcome();
            handled = true;
          } else {
            debugPrint(
              '🔐 [SPLASH] Connection failed (attempt ${attempt + 1}/$_maxRetries): ${failure.message}',
            );
          }
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
          handled = true;
        },
      );

      if (handled || _hasNavigated) return;

      // Network/server error — show retry modal unless this was the last attempt
      if (attempt < _maxRetries - 1) {
        final shouldRetry = await _showRetryModal(attempt + 1);
        if (!mounted || _hasNavigated) return;
        if (!shouldRetry) {
          _navigateToWelcome();
          return;
        }
      }
    }

    // All retries exhausted without a valid response
    if (!mounted || _hasNavigated) return;
    debugPrint('🔐 [SPLASH] All retries exhausted, redirecting to welcome');
    _navigateToWelcome();
  }

  Future<bool> _showRetryModal(int attempt) async {
    if (!mounted) return false;

    final result = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => _RetryConnectionDialog(
        attempt: attempt,
        maxAttempts: _maxRetries,
        retryDelaySeconds: _retryDelaySeconds,
        onCancel: () => Navigator.of(ctx).pop(false),
        onCountdownComplete: () => Navigator.of(ctx).pop(true),
      ),
    );

    return result ?? false;
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
                Transform.translate(
                  offset: const Offset(0, -32),
                  child: FadeTransition(
                    opacity: _logoFadeAnimation,
                    child: ScaleTransition(
                      scale: _logoScaleAnimation,
                      child: Image.asset(
                        'assets/images/logo/sigook-logo.png',
                        width: 270,
                        color: Colors.white,
                        colorBlendMode: BlendMode.srcIn,
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

class _RetryConnectionDialog extends StatefulWidget {
  final int attempt;
  final int maxAttempts;
  final int retryDelaySeconds;
  final VoidCallback onCancel;
  final VoidCallback onCountdownComplete;

  const _RetryConnectionDialog({
    required this.attempt,
    required this.maxAttempts,
    required this.retryDelaySeconds,
    required this.onCancel,
    required this.onCountdownComplete,
  });

  @override
  State<_RetryConnectionDialog> createState() => _RetryConnectionDialogState();
}

class _RetryConnectionDialogState extends State<_RetryConnectionDialog> {
  late int _secondsLeft;
  Timer? _timer;
  bool _actionTaken = false;

  @override
  void initState() {
    super.initState();
    _secondsLeft = widget.retryDelaySeconds;
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (!mounted) {
        timer.cancel();
        return;
      }
      setState(() => _secondsLeft--);
      if (_secondsLeft <= 0) {
        timer.cancel();
        _triggerCountdownComplete();
      }
    });
  }

  void _triggerCountdownComplete() {
    if (_actionTaken) return;
    _actionTaken = true;
    widget.onCountdownComplete();
  }

  void _handleCancel() {
    if (_actionTaken) return;
    _actionTaken = true;
    _timer?.cancel();
    widget.onCancel();
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final progress = 1.0 - (_secondsLeft / widget.retryDelaySeconds);

    return AlertDialog(
      title: const Text('Connection Error'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text('Unable to reach the server. Please check your connection.'),
          const SizedBox(height: 16),
          Row(
            children: [
              Text('Retrying in '),
              Text(
                '$_secondsLeft s',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              const Spacer(),
              Text(
                'Attempt ${widget.attempt}/${widget.maxAttempts}',
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ],
          ),
          const SizedBox(height: 8),
          LinearProgressIndicator(value: progress),
        ],
      ),
      actions: [
        TextButton(
          onPressed: _handleCancel,
          child: const Text('Cancel'),
        ),
      ],
    );
  }
}
