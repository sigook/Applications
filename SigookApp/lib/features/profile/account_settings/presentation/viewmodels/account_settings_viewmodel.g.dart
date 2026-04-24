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
    r'79341efec0dc938990b02f2a060244ebe8d7f08a';

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
