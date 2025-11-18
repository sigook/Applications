// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'registration_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning
/// ViewModel for managing registration form state

@ProviderFor(RegistrationViewModel)
const registrationViewModelProvider = RegistrationViewModelProvider._();

/// ViewModel for managing registration form state
final class RegistrationViewModelProvider
    extends $NotifierProvider<RegistrationViewModel, RegistrationForm> {
  /// ViewModel for managing registration form state
  const RegistrationViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'registrationViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$registrationViewModelHash();

  @$internal
  @override
  RegistrationViewModel create() => RegistrationViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(RegistrationForm value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<RegistrationForm>(value),
    );
  }
}

String _$registrationViewModelHash() =>
    r'18397918bdbe9f7c7f0ec8e6d568d9caa6b821af';

/// ViewModel for managing registration form state

abstract class _$RegistrationViewModel extends $Notifier<RegistrationForm> {
  RegistrationForm build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<RegistrationForm, RegistrationForm>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<RegistrationForm, RegistrationForm>,
              RegistrationForm,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
