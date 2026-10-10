// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'licenses_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(LicensesViewModel)
final licensesViewModelProvider = LicensesViewModelProvider._();

final class LicensesViewModelProvider
    extends $NotifierProvider<LicensesViewModel, LicensesState> {
  LicensesViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'licensesViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$licensesViewModelHash();

  @$internal
  @override
  LicensesViewModel create() => LicensesViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(LicensesState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<LicensesState>(value),
    );
  }
}

String _$licensesViewModelHash() => r'2cc76ea80fa2ead2f93732022aa82605a9fd56ce';

abstract class _$LicensesViewModel extends $Notifier<LicensesState> {
  LicensesState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<LicensesState, LicensesState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<LicensesState, LicensesState>,
              LicensesState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
