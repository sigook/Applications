// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preferences_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PreferencesViewModel)
const preferencesViewModelProvider = PreferencesViewModelProvider._();

final class PreferencesViewModelProvider
    extends $NotifierProvider<PreferencesViewModel, PreferencesState> {
  const PreferencesViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'preferencesViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$preferencesViewModelHash();

  @$internal
  @override
  PreferencesViewModel create() => PreferencesViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(PreferencesState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<PreferencesState>(value),
    );
  }
}

String _$preferencesViewModelHash() =>
    r'842b06e29c19fdc33cc7867e528a5f77aff2927f';

abstract class _$PreferencesViewModel extends $Notifier<PreferencesState> {
  PreferencesState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<PreferencesState, PreferencesState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<PreferencesState, PreferencesState>,
              PreferencesState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
