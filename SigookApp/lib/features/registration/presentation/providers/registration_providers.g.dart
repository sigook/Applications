// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'registration_providers.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(RegistrationFormStateNotifier)
const registrationFormStateProvider = RegistrationFormStateNotifierProvider._();

final class RegistrationFormStateNotifierProvider
    extends
        $NotifierProvider<
          RegistrationFormStateNotifier,
          RegistrationFormState
        > {
  const RegistrationFormStateNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'registrationFormStateProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$registrationFormStateNotifierHash();

  @$internal
  @override
  RegistrationFormStateNotifier create() => RegistrationFormStateNotifier();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(RegistrationFormState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<RegistrationFormState>(value),
    );
  }
}

String _$registrationFormStateNotifierHash() =>
    r'392152c872ff9e9d659c94bd345ad25b30dda160';

abstract class _$RegistrationFormStateNotifier
    extends $Notifier<RegistrationFormState> {
  RegistrationFormState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<RegistrationFormState, RegistrationFormState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<RegistrationFormState, RegistrationFormState>,
              RegistrationFormState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
