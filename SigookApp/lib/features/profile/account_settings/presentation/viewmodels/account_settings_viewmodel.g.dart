// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_settings_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(AccountSettingsViewModel)
const accountSettingsViewModelProvider = AccountSettingsViewModelProvider._();

final class AccountSettingsViewModelProvider
    extends $NotifierProvider<AccountSettingsViewModel, AccountSettingsState> {
  const AccountSettingsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'accountSettingsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$accountSettingsViewModelHash();

  @$internal
  @override
  AccountSettingsViewModel create() => AccountSettingsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AccountSettingsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AccountSettingsState>(value),
    );
  }
}

String _$accountSettingsViewModelHash() =>
    r'b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1';

abstract class _$AccountSettingsViewModel
    extends $Notifier<AccountSettingsState> {
  AccountSettingsState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<AccountSettingsState, AccountSettingsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<AccountSettingsState, AccountSettingsState>,
              AccountSettingsState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
