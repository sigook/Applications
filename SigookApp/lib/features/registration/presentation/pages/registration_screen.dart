import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../../../core/widgets/error_state_widget.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../providers/registration_providers.dart';
import '../widgets/registration_step_indicator.dart';
import 'basic_info_page.dart';
import 'preferences_page.dart';
import 'documents_page.dart';
import 'account_page.dart';

class RegistrationScreen extends ConsumerWidget {
  const RegistrationScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final gendersAsync = ref.watch(gendersProvider);
    final identificationTypesAsync = ref.watch(identificationTypesListProvider);
    final languagesAsync = ref.watch(languagesProvider);
    final skillsAsync = ref.watch(skillsProvider);
    final availabilityAsync = ref.watch(availabilityListProvider);
    final availabilityTimeAsync = ref.watch(availabilityTimeListProvider);
    final countriesAsync = ref.watch(countriesListProvider);

    final isLoading =
        gendersAsync.isLoading ||
        identificationTypesAsync.isLoading ||
        languagesAsync.isLoading ||
        skillsAsync.isLoading ||
        availabilityAsync.isLoading ||
        availabilityTimeAsync.isLoading ||
        countriesAsync.isLoading;

    final hasError =
        gendersAsync.hasError ||
        identificationTypesAsync.hasError ||
        languagesAsync.hasError ||
        skillsAsync.hasError ||
        availabilityAsync.hasError ||
        availabilityTimeAsync.hasError ||
        countriesAsync.hasError;

    if (isLoading) {
      return const Scaffold(
        body: LoadingIndicator(message: 'Loading registration form...'),
      );
    }

    if (hasError) {
      return Scaffold(
        body: ErrorStateWidget(
          title: 'Failed to load registration data',
          message: 'Please check your connection and try again',
          onRetry: () {
            ref.invalidate(gendersProvider);
            ref.invalidate(identificationTypesListProvider);
            ref.invalidate(languagesProvider);
            ref.invalidate(skillsProvider);
            ref.invalidate(availabilityListProvider);
            ref.invalidate(availabilityTimeListProvider);
            ref.invalidate(countriesListProvider);
          },
        ),
      );
    }

    return const _RegistrationFormScreen();
  }
}

class _RegistrationFormScreen extends ConsumerStatefulWidget {
  const _RegistrationFormScreen();

  @override
  ConsumerState<_RegistrationFormScreen> createState() =>
      _RegistrationFormScreenState();
}

class _RegistrationFormScreenState
    extends ConsumerState<_RegistrationFormScreen> {
  final ScrollController _scrollController = ScrollController();

  static const int _lastStepIndex = 3;

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _scrollToTop() {
    if (_scrollController.hasClients) {
      _scrollController.animateTo(
        0,
        duration: const Duration(milliseconds: 250),
        curve: Curves.easeOut,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(registrationFormStateProvider);
    final form = ref.watch(registrationViewModelProvider);
    final authState = ref.watch(authViewModelProvider);

    // Navigate to jobs when sign-in succeeds
    ref.listen(authViewModelProvider, (previous, next) {
      if (next.isAuthenticated &&
          next.token != null &&
          next.token!.accessToken != null &&
          next.token!.accessToken!.isNotEmpty) {
        context.go(AppRoutes.jobs);
      }
      if (next.error != null && previous?.error != next.error) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(next.error!),
            backgroundColor: AppTheme.errorRed,
          ),
        );
      }
    });

    return Stack(
      children: [
        Scaffold(
          backgroundColor: AppTheme.surfaceGrey,
          body: SafeArea(
            child: Column(
              children: [
                // ── Scrollable content: back button + step indicator + form
                Expanded(
                  child: SingleChildScrollView(
                    controller: _scrollController,
                    padding: const EdgeInsets.fromLTRB(16, 8, 16, 16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        // Back button
                        Align(
                          alignment: Alignment.centerLeft,
                          child: IconButton(
                            onPressed: () => context.go(AppRoutes.welcome),
                            icon: const Icon(Icons.arrow_back),
                            style: IconButton.styleFrom(
                                backgroundColor: Colors.white),
                          ),
                        ),
                        const SizedBox(height: 8),
                        // Step indicator
                        RegistrationStepIndicator(
                          currentStep: formState.currentStep,
                          completedSteps: [
                            form.isBasicInfoComplete,
                            form.isPreferencesInfoComplete,
                            form.isDocumentsInfoComplete,
                            form.isAccountInfoComplete,
                          ],
                          onStepTapped: (step) {
                            ref
                                .read(registrationFormStateProvider.notifier)
                                .goToStep(step);
                            WidgetsBinding.instance
                                .addPostFrameCallback((_) => _scrollToTop());
                          },
                        ),
                        const SizedBox(height: 12),
                        // Form content
                        _buildStepContent(formState.currentStep),
                      ],
                    ),
                  ),
                ),

                // ── Fixed bottom: action buttons + sign-in link
                _buildActionButtons(formState, form, ref),
              ],
            ),
          ),
        ),
        if (authState.isLoading)
          const Positioned.fill(
            child: LoadingOverlay(message: 'Signing in...'),
          ),
      ],
    );
  }

  Widget _buildStepContent(int step) {
    switch (step) {
      case 0:
        return const BasicInfoPage();
      case 1:
        return const PreferencesPage();
      case 2:
        return const DocumentsPage();
      case 3:
        return const AccountPage();
      default:
        return const SizedBox.shrink();
    }
  }

  Widget _buildActionButtons(dynamic formState, dynamic form, WidgetRef ref) {
    final isLastStep = formState.currentStep == _lastStepIndex;

    return Container(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
      decoration: BoxDecoration(
        color: AppTheme.surfaceGrey,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 4,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              if (formState.currentStep > 0) ...[
                OutlinedButton(
                  onPressed: formState.isSubmitting
                      ? null
                      : () {
                          ref
                              .read(registrationFormStateProvider.notifier)
                              .previousStep();
                          WidgetsBinding.instance
                              .addPostFrameCallback((_) => _scrollToTop());
                        },
                  style: OutlinedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 24, vertical: 16),
                  ),
                  child: const Text('Back'),
                ),
                const SizedBox(width: 12),
              ],
              Expanded(
                child: ElevatedButton(
                  onPressed: formState.isSubmitting
                      ? null
                      : () {
                          if (_canContinue(formState.currentStep, form)) {
                            if (formState.currentStep < _lastStepIndex) {
                              ref
                                  .read(registrationFormStateProvider.notifier)
                                  .nextStep();
                              WidgetsBinding.instance
                                  .addPostFrameCallback((_) => _scrollToTop());
                            } else {
                              _submitForm(context, ref);
                            }
                          } else {
                            _showValidationError(context, formState.currentStep);
                          }
                        },
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 32, vertical: 16),
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
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                'Already have an account? ',
                style: TextStyle(color: Colors.grey.shade600, fontSize: 15),
              ),
              TextButton(
                onPressed: () {
                  ref.read(authViewModelProvider.notifier).signIn();
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
        ],
      ),
    );
  }

  bool _canContinue(int currentStep, dynamic form) {
    switch (currentStep) {
      case 0:
        return form.isBasicInfoComplete;
      case 1:
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
    final notifier = ref.read(registrationFormStateProvider.notifier);
    notifier.setSubmitting(true);

    final viewModel = ref.read(registrationViewModelProvider.notifier);
    final result = await viewModel.submitRegistration();

    notifier.setSubmitting(false);

    if (!context.mounted) return;

    if (result == 'Success') {
      notifier.setSuccess('Registration submitted successfully!');
      context.go(AppRoutes.registrationConfirmation);
    } else {
      notifier.setError(result);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result), backgroundColor: Colors.red),
      );
    }
  }
}
