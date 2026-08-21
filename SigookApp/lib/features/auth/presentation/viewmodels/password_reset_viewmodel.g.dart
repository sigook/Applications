// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'password_reset_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PasswordResetViewModel)
const passwordResetViewModelProvider = PasswordResetViewModelProvider._();

final class PasswordResetViewModelProvider
    extends $NotifierProvider<PasswordResetViewModel, PasswordResetState> {
  const PasswordResetViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'passwordResetViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$passwordResetViewModelHash();

  @$internal
  @override
  PasswordResetViewModel create() => PasswordResetViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(PasswordResetState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<PasswordResetState>(value),
    );
  }
}

String _$passwordResetViewModelHash() =>
    r'd54b59eb0e386ac120bf5b64417f84852102a9f7';

abstract class _$PasswordResetViewModel extends $Notifier<PasswordResetState> {
  PasswordResetState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<PasswordResetState, PasswordResetState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<PasswordResetState, PasswordResetState>,
              PasswordResetState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
