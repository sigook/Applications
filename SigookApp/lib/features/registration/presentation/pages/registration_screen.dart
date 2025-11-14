import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/utils/responsive.dart';
import '../../../../core/routing/app_router.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../providers/registration_providers.dart';
import 'basic_info_page.dart';
import 'preferences_page.dart';
import 'documents_page.dart';
import 'account_page.dart';

/// Wrapper that pre-loads all catalog data before showing the registration form
class RegistrationScreen extends ConsumerWidget {
  const RegistrationScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    // Pre-load all static catalog data
    final gendersAsync = ref.watch(gendersProvider);
    final identificationTypesAsync = ref.watch(identificationTypesListProvider);
    final languagesAsync = ref.watch(languagesProvider);
    final skillsAsync = ref.watch(skillsProvider);
    final availabilityAsync = ref.watch(availabilityListProvider);
    final availabilityTimeAsync = ref.watch(availabilityTimeListProvider);
    final countriesAsync = ref.watch(countriesListProvider);

    // Check if any are still loading
    final isLoading =
        gendersAsync.isLoading ||
        identificationTypesAsync.isLoading ||
        languagesAsync.isLoading ||
        skillsAsync.isLoading ||
        availabilityAsync.isLoading ||
        availabilityTimeAsync.isLoading ||
        countriesAsync.isLoading;

    // Check if any have errors
    final hasError =
        gendersAsync.hasError ||
        identificationTypesAsync.hasError ||
        languagesAsync.hasError ||
        skillsAsync.hasError ||
        availabilityAsync.hasError ||
        availabilityTimeAsync.hasError ||
        countriesAsync.hasError;

    if (isLoading) {
      return Scaffold(
        body: const Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              CircularProgressIndicator(),
              SizedBox(height: 16),
              Text(
                'Loading registration form...',
                style: TextStyle(fontSize: 16, color: Colors.grey),
              ),
            ],
          ),
        ),
      );
    }

    if (hasError) {
      return Scaffold(
        appBar: AppBar(title: const Text('Registration'), elevation: 0),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 64, color: Colors.red),
              const SizedBox(height: 16),
              const Text(
                'Failed to load registration data',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              const Text(
                'Please check your connection and try again',
                style: TextStyle(color: Colors.grey),
              ),
              const SizedBox(height: 24),
              ElevatedButton.icon(
                onPressed: () {
                  // Invalidate all providers to retry
                  ref.invalidate(gendersProvider);
                  ref.invalidate(identificationTypesListProvider);
                  ref.invalidate(languagesProvider);
                  ref.invalidate(skillsProvider);
                  ref.invalidate(availabilityListProvider);
                  ref.invalidate(availabilityTimeListProvider);
                  ref.invalidate(countriesListProvider);
                },
                icon: const Icon(Icons.refresh),
                label: const Text('Retry'),
              ),
            ],
          ),
        ),
      );
    }

    // All data loaded successfully, show the form
    return const _RegistrationFormScreen();
  }
}

/// The actual registration form screen (shown only after data is loaded)
class _RegistrationFormScreen extends ConsumerStatefulWidget {
  const _RegistrationFormScreen();

  @override
  ConsumerState<_RegistrationFormScreen> createState() =>
      _RegistrationFormScreenState();
}

class _RegistrationFormScreenState
    extends ConsumerState<_RegistrationFormScreen> {
  final ScrollController _scrollController = ScrollController();

  // Total number of steps (0-indexed, so last step is 3)
  static const int _lastStepIndex = 3;

  @override
  void initState() {
    super.initState();
    // Scroll to top when widget is first built
    WidgetsBinding.instance.addPostFrameCallback((_) => _scrollToTop());
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _scrollToTop() {
    if (_scrollController.hasClients) {
      _scrollController.jumpTo(0);
    }
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(registrationFormStateNotifierProvider);
    final form = ref.watch(registrationViewModelProvider);
    final responsive = context.responsive;

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      body: SafeArea(
        child: Column(
          children: [
            // Back button (same style as sign in page)
            Align(
              alignment: Alignment.centerLeft,
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: IconButton(
                  onPressed: () => context.go(AppRoutes.welcome),
                  icon: const Icon(Icons.arrow_back),
                  style: IconButton.styleFrom(backgroundColor: Colors.white),
                ),
              ),
            ),

            Expanded(
              child: PrimaryScrollController(
                controller: _scrollController,
                child: Stepper(
                  type: responsive.isMobile
                      ? StepperType.vertical
                      : StepperType.vertical,
                  currentStep: formState.currentStep,
                  onStepContinue: () {
                    if (_canContinue(formState.currentStep, form)) {
                      if (formState.currentStep < _lastStepIndex) {
                        ref
                            .read(
                              registrationFormStateNotifierProvider.notifier,
                            )
                            .nextStep();
                        WidgetsBinding.instance.addPostFrameCallback(
                          (_) => _scrollToTop(),
                        );
                      } else {
                        _submitForm(context, ref);
                      }
                    } else {
                      _showValidationError(context, formState.currentStep);
                    }
                  },
                  onStepCancel: () {
                    if (formState.currentStep > 0) {
                      ref
                          .read(registrationFormStateNotifierProvider.notifier)
                          .previousStep();
                      WidgetsBinding.instance.addPostFrameCallback(
                        (_) => _scrollToTop(),
                      );
                    }
                  },
                  onStepTapped: (step) {
                    ref
                        .read(registrationFormStateNotifierProvider.notifier)
                        .goToStep(step);
                    WidgetsBinding.instance.addPostFrameCallback(
                      (_) => _scrollToTop(),
                    );
                  },
                  controlsBuilder: (context, details) {
                    final isLastStep = details.currentStep == _lastStepIndex;
                    return Padding(
                      padding: const EdgeInsets.only(top: 24),
                      child: Row(
                        children: [
                          ElevatedButton(
                            onPressed: formState.isSubmitting
                                ? null
                                : details.onStepContinue,
                            style: ElevatedButton.styleFrom(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 32,
                                vertical: 16,
                              ),
                            ),
                            child: formState.isSubmitting
                                ? const SizedBox(
                                    height: 20,
                                    width: 20,
                                    child: CircularProgressIndicator(
                                      strokeWidth: 2,
                                      valueColor: AlwaysStoppedAnimation<Color>(
                                        Colors.white,
                                      ),
                                    ),
                                  )
                                : Text(isLastStep ? 'Submit' : 'Continue'),
                          ),
                          if (details.currentStep > 0) ...[
                            const SizedBox(width: 12),
                            OutlinedButton(
                              onPressed: formState.isSubmitting
                                  ? null
                                  : details.onStepCancel,
                              style: OutlinedButton.styleFrom(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 24,
                                  vertical: 16,
                                ),
                              ),
                              child: const Text('Back'),
                            ),
                          ],
                        ],
                      ),
                    );
                  },
                  steps: [
                    Step(
                      title: const Text('Basic Information'),
                      subtitle: form.isBasicInfoComplete
                          ? const Text(
                              'Completed',
                              style: TextStyle(color: Colors.green),
                            )
                          : null,
                      content: const BasicInfoPage(),
                      isActive: formState.currentStep >= 0,
                      state: _getStepState(
                        0,
                        formState.currentStep,
                        form.isBasicInfoComplete,
                      ),
                    ),
                    Step(
                      title: const Text('Preferences'),
                      subtitle: form.isPreferencesInfoComplete
                          ? const Text(
                              'Completed',
                              style: TextStyle(color: Colors.green),
                            )
                          : null,
                      content: const PreferencesPage(),
                      isActive: formState.currentStep >= 1,
                      state: _getStepState(
                        1,
                        formState.currentStep,
                        form.isPreferencesInfoComplete,
                      ),
                    ),
                    Step(
                      title: const Text('Documents'),
                      subtitle: form.isDocumentsInfoComplete
                          ? const Text(
                              'Completed',
                              style: TextStyle(color: Colors.green),
                            )
                          : null,
                      content: const DocumentsPage(),
                      isActive: formState.currentStep >= 2,
                      state: _getStepState(
                        2,
                        formState.currentStep,
                        form.isDocumentsInfoComplete,
                      ),
                    ),
                    Step(
                      title: const Text('Account Setup'),
                      subtitle: form.isAccountInfoComplete
                          ? const Text(
                              'Completed',
                              style: TextStyle(color: Colors.green),
                            )
                          : null,
                      content: const AccountPage(),
                      isActive: formState.currentStep >= 3,
                      state: _getStepState(
                        3,
                        formState.currentStep,
                        form.isAccountInfoComplete,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            // Bottom sign in link
            Container(
              padding: const EdgeInsets.fromLTRB(24, 16, 24, 24),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    'Already have an account? ',
                    style: TextStyle(color: Colors.grey.shade600, fontSize: 15),
                  ),
                  TextButton(
                    onPressed: () {
                      context.go(AppRoutes.signIn);
                    },
                    style: TextButton.styleFrom(
                      padding: EdgeInsets.zero,
                      minimumSize: const Size(0, 0),
                      tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    ),
                    child: const Text(
                      'Sign In',
                      style: TextStyle(
                        color: AppTheme.primaryBlue,
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  StepState _getStepState(int stepIndex, int currentStep, bool isComplete) {
    if (stepIndex == currentStep) {
      return StepState.editing;
    } else if (isComplete) {
      return StepState.complete;
    } else if (stepIndex < currentStep) {
      return StepState.indexed;
    }
    return StepState.indexed;
  }

  bool _canContinue(int currentStep, form) {
    switch (currentStep) {
      case 0:
        return form.isBasicInfoComplete;
      case 1:
        // Preferences are optional: never block navigation on this step
        return true;
      case 2:
        return form.isDocumentsInfoComplete;
      case 3:
        return form.isAccountInfoComplete;
      default:
        return false;
    }
  }

  void _showValidationError(BuildContext context, int step) {
    String sectionName;
    switch (step) {
      case 0:
        sectionName = 'Basic Information';
        break;
      case 1:
        sectionName = 'Preferences';
        break;
      case 2:
        sectionName = 'Documents';
        break;
      case 3:
        sectionName = 'Account Setup';
        break;
      default:
        sectionName = 'Current section';
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Please complete all required fields in $sectionName'),
        backgroundColor: Colors.orange,
      ),
    );
  }

  Future<void> _submitForm(BuildContext context, WidgetRef ref) async {
    final notifier = ref.read(registrationFormStateNotifierProvider.notifier);
    notifier.setSubmitting(true);

    final viewModel = ref.read(registrationViewModelProvider.notifier);
    final success = await viewModel.submitRegistration();

    notifier.setSubmitting(false);

    if (!context.mounted) return;

    if (success) {
      notifier.setSuccess('Registration submitted successfully!');
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Registration submitted successfully!'),
          backgroundColor: Colors.green,
        ),
      );

      // Navigate to welcome page after successful registration
      context.go(AppRoutes.welcome);
    } else {
      notifier.setError('Failed to submit registration. Please try again.');
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to submit registration. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }
}
