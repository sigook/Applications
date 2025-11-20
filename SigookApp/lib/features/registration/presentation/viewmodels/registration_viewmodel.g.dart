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
    r'10f30d9c1e91e60125fd0f8004f78faf9d5db528';

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
