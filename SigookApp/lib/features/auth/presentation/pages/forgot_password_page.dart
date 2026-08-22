import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../registration/domain/entities/value_objects/email.dart';
import '../../../registration/domain/entities/value_objects/password.dart';
import '../../../registration/presentation/widgets/custom_text_field.dart';
import '../viewmodels/password_reset_viewmodel.dart';

class ForgotPasswordPage extends ConsumerStatefulWidget {
  const ForgotPasswordPage({super.key});

  @override
  ConsumerState<ForgotPasswordPage> createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends ConsumerState<ForgotPasswordPage> {
  final _emailController = TextEditingController();
  final _codeController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  String? _emailError;
  String? _codeError;
  String? _passwordError;
  String? _confirmError;
  bool _obscurePassword = true;
  bool _obscureConfirm = true;

  static final _codePattern = RegExp(r'^[0-9]{6}$');

  @override
  void dispose() {
    _emailController.dispose();
    _codeController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  bool _validateEmail() {
    final email = Email(_emailController.text.trim());
    setState(() => _emailError = email.errorMessage);
    return _emailError == null;
  }

  bool _validateCode() {
    setState(() {
      _codeError = _codePattern.hasMatch(_codeController.text.trim())
          ? null
          : 'Enter the 6-digit code';
    });
    return _codeError == null;
  }

  bool _validatePasswords() {
    final password = Password(_passwordController.text);
    setState(() {
      _passwordError = password.errorMessage;
      _confirmError = _confirmController.text != _passwordController.text
          ? 'Passwords do not match'
          : null;
    });
    return _passwordError == null && _confirmError == null;
  }

  Future<void> _sendCode() async {
    FocusManager.instance.primaryFocus?.unfocus();
    if (!_validateEmail()) return;
    await ref
        .read(passwordResetViewModelProvider.notifier)
        .requestCode(_emailController.text.trim());
  }

  void _continueToPassword() {
    FocusManager.instance.primaryFocus?.unfocus();
    if (!_validateCode()) return;
    ref.read(passwordResetViewModelProvider.notifier).continueToPassword();
  }

  Future<void> _resetPassword() async {
    FocusManager.instance.primaryFocus?.unfocus();
    if (!_validatePasswords()) return;
    await ref.read(passwordResetViewModelProvider.notifier).resetPassword(
          code: _codeController.text.trim(),
          newPassword: _passwordController.text,
        );
  }

  void _goBack() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go(AppRoutes.signIn);
    }
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isMobile = size.width < 600;
    final resetState = ref.watch(passwordResetViewModelProvider);

    ref.listen(passwordResetViewModelProvider, (previous, next) {
      if (next.error != null && previous?.error != next.error) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(next.error!),
            backgroundColor: AppTheme.errorRed,
          ),
        );
      }

      if (previous?.justCodeSent != true && next.justCodeSent) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'If an account exists for that email, a 6-digit code was sent.',
            ),
            backgroundColor: AppTheme.successGreen,
          ),
        );
      }

      if (previous?.justReset != true && next.justReset) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Password updated. Sign in with your new password.'),
            backgroundColor: AppTheme.successGreen,
          ),
        );
        _goBack();
      }
    });

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      body: SafeArea(
        child: Stack(
          children: [
            Center(
              child: SingleChildScrollView(
                padding: EdgeInsets.symmetric(
                  horizontal: isMobile ? 16 : 48,
                  vertical: 32,
                ),
                child: Container(
                  constraints: const BoxConstraints(maxWidth: 440),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      const SizedBox(height: 24),
                      Center(
                        child: Image.asset(
                          'assets/images/logo/sigook-logo.png',
                          width: 200,
                        ),
                      ),
                      const SizedBox(height: 24),
                      Card(
                        elevation: 0,
                        color: Colors.white,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(16),
                        ),
                        child: Padding(
                          padding: const EdgeInsets.all(24),
                          child: resetState.step == 1
                              ? _buildEmailStep(resetState)
                              : resetState.step == 2
                              ? _buildCodeStep(resetState)
                              : _buildPasswordStep(resetState),
                        ),
                      ),
                      const SizedBox(height: 8),
                      Center(
                        child: TextButton(
                          onPressed: _goBack,
                          child: const Text('Back to sign in'),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            Positioned(
              top: 16,
              left: 16,
              child: IconButton(
                onPressed: _goBack,
                icon: const Icon(Icons.arrow_back),
                style: IconButton.styleFrom(backgroundColor: Colors.white),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader(String title, String subtitle) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: Theme.of(
            context,
          ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 8),
        Text(
          subtitle,
          style: Theme.of(
            context,
          ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
        ),
      ],
    );
  }

  Widget _buildSubmitButton({
    required String label,
    required bool isLoading,
    required VoidCallback onPressed,
  }) {
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: isLoading ? null : onPressed,
        child: isLoading
            ? const SizedBox(
                height: 20,
                width: 20,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                ),
              )
            : Text(label),
      ),
    );
  }

  Widget _buildEmailStep(PasswordResetState resetState) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildHeader(
          'Reset your password',
          'Enter your email and we will send you a 6-digit code.',
        ),
        const SizedBox(height: 32),
        CustomTextField(
          label: 'Email',
          hint: 'example@email.com',
          controller: _emailController,
          errorText: _emailError,
          keyboardType: TextInputType.emailAddress,
          textInputAction: TextInputAction.done,
          onChanged: (_) {
            if (_emailError != null) {
              setState(() => _emailError = null);
            }
          },
        ),
        const SizedBox(height: 32),
        _buildSubmitButton(
          label: 'Send Code',
          isLoading: resetState.isLoading,
          onPressed: _sendCode,
        ),
      ],
    );
  }

  Widget _buildCodeStep(PasswordResetState resetState) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildHeader(
          'Check your inbox',
          'If an account exists for ${resetState.email}, we sent a 6-digit code. Enter it below to continue.',
        ),
        const SizedBox(height: 32),
        CustomTextField(
          label: 'Code',
          hint: '6-digit code',
          controller: _codeController,
          errorText: _codeError,
          keyboardType: TextInputType.number,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(6),
          ],
          textInputAction: TextInputAction.done,
          onChanged: (_) {
            if (_codeError != null) {
              setState(() => _codeError = null);
            }
          },
        ),
        const SizedBox(height: 32),
        _buildSubmitButton(
          label: 'Verify Code',
          isLoading: resetState.isLoading,
          onPressed: _continueToPassword,
        ),
        const SizedBox(height: 8),
        Center(
          child: TextButton(
            onPressed:
                resetState.resendCooldownSeconds > 0 || resetState.isLoading
                ? null
                : () => ref
                      .read(passwordResetViewModelProvider.notifier)
                      .resendCode(),
            child: Text(
              resetState.resendCooldownSeconds > 0
                  ? 'Resend code (${resetState.resendCooldownSeconds}s)'
                  : 'Resend code',
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildPasswordStep(PasswordResetState resetState) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildHeader(
          'Create a new password',
          'Enter a new password for ${resetState.email}.',
        ),
        const SizedBox(height: 32),
        CustomTextField(
          label: 'New Password',
          hint: 'Enter a strong password',
          controller: _passwordController,
          errorText: _passwordError,
          obscureText: _obscurePassword,
          textInputAction: TextInputAction.next,
          suffixIcon: IconButton(
            icon: Icon(
              _obscurePassword ? Icons.visibility : Icons.visibility_off,
            ),
            onPressed: () =>
                setState(() => _obscurePassword = !_obscurePassword),
          ),
          onChanged: (_) {
            setState(() => _passwordError = null);
          },
        ),
        const SizedBox(height: 16),
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.blue.shade50,
            borderRadius: BorderRadius.circular(12),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Password Requirements:',
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  color: Colors.blue.shade900,
                ),
              ),
              const SizedBox(height: 8),
              _buildRequirement(
                'At least 8 characters',
                Password(_passwordController.text).hasMinLength,
              ),
              _buildRequirement(
                'Contains uppercase letter',
                Password(_passwordController.text).hasUppercase,
              ),
              _buildRequirement(
                'Contains lowercase letter',
                Password(_passwordController.text).hasLowercase,
              ),
              _buildRequirement(
                'Contains number',
                Password(_passwordController.text).hasNumber,
              ),
            ],
          ),
        ),
        const SizedBox(height: 24),
        CustomTextField(
          label: 'Confirm Password',
          hint: 'Re-enter your password',
          controller: _confirmController,
          errorText: _confirmError,
          obscureText: _obscureConfirm,
          textInputAction: TextInputAction.done,
          suffixIcon: IconButton(
            icon: Icon(
              _obscureConfirm ? Icons.visibility : Icons.visibility_off,
            ),
            onPressed: () => setState(() => _obscureConfirm = !_obscureConfirm),
          ),
          onChanged: (_) {
            if (_confirmError != null) {
              setState(() => _confirmError = null);
            }
          },
        ),
        const SizedBox(height: 32),
        _buildSubmitButton(
          label: 'Reset Password',
          isLoading: resetState.isLoading,
          onPressed: _resetPassword,
        ),
        const SizedBox(height: 8),
        Center(
          child: TextButton(
            onPressed: resetState.isLoading
                ? null
                : () => ref
                      .read(passwordResetViewModelProvider.notifier)
                      .backToCode(),
            child: const Text('Change code'),
          ),
        ),
      ],
    );
  }

  Widget _buildRequirement(String text, bool met) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Row(
        children: [
          Icon(
            met ? Icons.check_circle : Icons.circle_outlined,
            size: 16,
            color: met ? Colors.green.shade700 : Colors.grey.shade500,
          ),
          const SizedBox(width: 8),
          Text(
            text,
            style: TextStyle(
              fontSize: 13,
              color: met ? Colors.green.shade700 : Colors.grey.shade600,
            ),
          ),
        ],
      ),
    );
  }
}
