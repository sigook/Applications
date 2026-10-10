// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'password_reset_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PasswordResetViewModel)
final passwordResetViewModelProvider = PasswordResetViewModelProvider._();

final class PasswordResetViewModelProvider
    extends $NotifierProvider<PasswordResetViewModel, PasswordResetState> {
  PasswordResetViewModelProvider._()
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
    r'1e1b1f1a55a20e865f574df22a9f8367f44dddb8';

abstract class _$PasswordResetViewModel extends $Notifier<PasswordResetState> {
  PasswordResetState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<PasswordResetState, PasswordResetState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<PasswordResetState, PasswordResetState>,
              PasswordResetState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
