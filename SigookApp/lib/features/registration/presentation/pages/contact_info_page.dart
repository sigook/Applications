import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/contact_info.dart';
import '../../domain/entities/value_objects/email.dart';
import '../../domain/entities/value_objects/password.dart';
import '../providers/registration_providers.dart';
import '../widgets/custom_text_field.dart';

class ContactInfoPage extends ConsumerStatefulWidget {
  const ContactInfoPage({super.key});

  @override
  ConsumerState<ContactInfoPage> createState() => _ContactInfoPageState();
}

class _ContactInfoPageState extends ConsumerState<ContactInfoPage> {
  late TextEditingController _emailController;
  late TextEditingController _passwordController;
  bool _obscurePassword = true;

  String? _emailError;
  String? _passwordError;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController();
    _passwordController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.contactInfo != null) {
        _emailController.text = form.contactInfo!.email.value;
        _passwordController.text = form.contactInfo!.password.value;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      final email = Email(_emailController.text);
      final password = Password(_passwordController.text);

      _emailError = email.errorMessage;
      _passwordError = password.errorMessage;

      if (_emailError == null && _passwordError == null) {
        final contactInfo = ContactInfo(
          email: email,
          password: password,
        );
        ref.read(registrationViewModelProvider.notifier).updateContactInfo(contactInfo);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;
    
    return SingleChildScrollView(
      padding: EdgeInsets.all(isMobile ? 12 : 16),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Card(
        elevation: 0,
        color: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Contact Information',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
          ),
          const SizedBox(height: 8),
          Text(
            'Please provide your contact details',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.grey.shade600,
                ),
          ),
          const SizedBox(height: 32),
          CustomTextField(
            label: 'Email',
            hint: 'example@email.com',
            initialValue: _emailController.text,
            errorText: _emailError,
            keyboardType: TextInputType.emailAddress,
            onChanged: (value) {
              _emailController.text = value;
              _validateAndSave();
            },
          ),
          const SizedBox(height: 24),
          CustomTextField(
            label: 'Password',
            hint: 'Enter a strong password',
            initialValue: _passwordController.text,
            errorText: _passwordError,
            obscureText: _obscurePassword,
            suffixIcon: IconButton(
              icon: Icon(
                _obscurePassword ? Icons.visibility : Icons.visibility_off,
              ),
              onPressed: () {
                setState(() {
                  _obscurePassword = !_obscurePassword;
                });
              },
            ),
            onChanged: (value) {
              _passwordController.text = value;
              _validateAndSave();
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
                _buildRequirement('At least 8 characters', 
                    _passwordController.text.length >= 8),
                _buildRequirement('Contains uppercase letter', 
                    _passwordController.text.contains(RegExp(r'[A-Z]'))),
                _buildRequirement('Contains lowercase letter', 
                    _passwordController.text.contains(RegExp(r'[a-z]'))),
                _buildRequirement('Contains number', 
                    _passwordController.text.contains(RegExp(r'[0-9]'))),
              ],
            ),
          ),
        ],
      ),
        ),
      ),
    );
  }

  Widget _buildRequirement(String text, bool met) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Icon(
            met ? Icons.check_circle : Icons.circle_outlined,
            size: 16,
            color: met ? Colors.green : Colors.grey,
          ),
          const SizedBox(width: 8),
          Text(
            text,
            style: TextStyle(
              fontSize: 13,
              color: met ? Colors.green : Colors.grey.shade700,
            ),
          ),
        ],
      ),
    );
  }
}
