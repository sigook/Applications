import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/contact_info.dart';
import '../../domain/entities/identification_type.dart';
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
  late TextEditingController _identificationController;
  late TextEditingController _mobileNumberController;
  bool _obscurePassword = true;

  String? _selectedIdentificationTypeId;
  String? _selectedIdentificationTypeName;

  String? _emailError;
  String? _passwordError;
  String? _identificationError;
  String? _identificationTypeError;
  String? _mobileNumberError;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController();
    _passwordController = TextEditingController();
    _identificationController = TextEditingController();
    _mobileNumberController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.contactInfo != null) {
        _emailController.text = form.contactInfo!.email.value;
        _passwordController.text = form.contactInfo!.password.value;
        _identificationController.text = form.contactInfo!.identification;
        _mobileNumberController.text = form.contactInfo!.mobileNumber;
        _selectedIdentificationTypeId = form.contactInfo!.identificationType.id;
        _selectedIdentificationTypeName = form.contactInfo!.identificationType.value;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _identificationController.dispose();
    _mobileNumberController.dispose();
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      final email = Email(_emailController.text);
      final password = Password(_passwordController.text);
      final identification = _identificationController.text;

      _emailError = email.errorMessage;
      _passwordError = password.errorMessage;
      _identificationError = identification.isEmpty
          ? 'Identification is required'
          : null;
      _identificationTypeError = _selectedIdentificationTypeId == null
          ? 'Identification type is required'
          : null;
      
      final mobileNumber = _mobileNumberController.text;
      _mobileNumberError = mobileNumber.isEmpty
          ? 'Mobile number is required'
          : mobileNumber.length < 10
          ? 'Mobile number must be at least 10 digits'
          : null;

      if (_emailError == null &&
          _passwordError == null &&
          _identificationError == null &&
          _mobileNumberError == null &&
          _selectedIdentificationTypeId != null &&
          _selectedIdentificationTypeName != null) {
        final contactInfo = ContactInfo(
          email: email,
          password: password,
          identification: identification,
          identificationType: IdentificationType(
            id: _selectedIdentificationTypeId!,
            value: _selectedIdentificationTypeName!,
          ),
          mobileNumber: mobileNumber,
        );
        ref
            .read(registrationViewModelProvider.notifier)
            .updateContactInfo(contactInfo);
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
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
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
                label: 'Mobile Number',
                hint: '+1 234 567 8900',
                initialValue: _mobileNumberController.text,
                errorText: _mobileNumberError,
                keyboardType: TextInputType.phone,
                onChanged: (value) {
                  _mobileNumberController.text = value;
                  _validateAndSave();
                },
              ),
              const SizedBox(height: 24),
              _buildIdentificationTypeSelector(),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'Identification Number',
                hint: 'Enter your ID number',
                initialValue: _identificationController.text,
                errorText: _identificationError,
                onChanged: (value) {
                  _identificationController.text = value;
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
                    _buildRequirement(
                      'At least 8 characters',
                      _passwordController.text.length >= 8,
                    ),
                    _buildRequirement(
                      'Contains uppercase letter',
                      _passwordController.text.contains(RegExp(r'[A-Z]')),
                    ),
                    _buildRequirement(
                      'Contains lowercase letter',
                      _passwordController.text.contains(RegExp(r'[a-z]')),
                    ),
                    _buildRequirement(
                      'Contains number',
                      _passwordController.text.contains(RegExp(r'[0-9]')),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildIdentificationTypeSelector() {
    final identificationTypesAsync = ref.watch(identificationTypesListProvider);

    return identificationTypesAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Identification Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          const Center(child: CircularProgressIndicator()),
        ],
      ),
      error: (error, _) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Identification Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.red.shade50,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.red.shade200),
            ),
            child: Row(
              children: [
                Icon(Icons.error_outline, color: Colors.red.shade700, size: 20),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'Failed to load identification types',
                    style: TextStyle(color: Colors.red.shade700),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.invalidate(identificationTypesListProvider);
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (identificationTypes) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Identification Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            initialValue: _selectedIdentificationTypeId,
            isExpanded: true,
            decoration: InputDecoration(
              hintText: 'Select ID type',
              errorText: _identificationTypeError,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: Colors.grey.shade300),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(
                  color: Theme.of(context).colorScheme.primary,
                  width: 2,
                ),
              ),
              filled: true,
              fillColor: Colors.white,
            ),
            items: identificationTypes.map((type) {
              return DropdownMenuItem<String>(
                value: type.id,
                child: Text(
                  type.value,
                  overflow: TextOverflow.ellipsis,
                ),
              );
            }).toList(),
            onChanged: (value) {
              if (value != null) {
                final selectedType = identificationTypes.firstWhere(
                  (type) => type.id == value,
                );
                setState(() {
                  _selectedIdentificationTypeId = value;
                  _selectedIdentificationTypeName = selectedType.value;
                });
              }
              _validateAndSave();
            },
          ),
        ],
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
