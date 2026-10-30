// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pay_stubs_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PayStubsViewModel)
final payStubsViewModelProvider = PayStubsViewModelProvider._();

final class PayStubsViewModelProvider
    extends $NotifierProvider<PayStubsViewModel, PayStubsState> {
  PayStubsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'payStubsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$payStubsViewModelHash();

  @$internal
  @override
  PayStubsViewModel create() => PayStubsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(PayStubsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<PayStubsState>(value),
    );
  }
}

String _$payStubsViewModelHash() => r'd5ee8e0c6229e0dcee6a561ba3a0c666a8b6d0d9';

abstract class _$PayStubsViewModel extends $Notifier<PayStubsState> {
  PayStubsState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<PayStubsState, PayStubsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<PayStubsState, PayStubsState>,
              PayStubsState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
