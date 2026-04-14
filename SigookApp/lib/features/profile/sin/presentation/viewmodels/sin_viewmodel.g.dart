// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sin_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(SinViewModel)
const sinViewModelProvider = SinViewModelProvider._();

final class SinViewModelProvider
    extends $NotifierProvider<SinViewModel, SinState> {
  const SinViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'sinViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$sinViewModelHash();

  @$internal
  @override
  SinViewModel create() => SinViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(SinState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<SinState>(value),
    );
  }
}

String _$sinViewModelHash() => r'8024b5550d90cdbeda1cd785e7b941639ca08da2';

abstract class _$SinViewModel extends $Notifier<SinState> {
  SinState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<SinState, SinState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<SinState, SinState>,
              SinState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
