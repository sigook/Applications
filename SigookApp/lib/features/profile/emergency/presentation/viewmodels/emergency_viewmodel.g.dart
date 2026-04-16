// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'emergency_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(EmergencyViewModel)
const emergencyViewModelProvider = EmergencyViewModelProvider._();

final class EmergencyViewModelProvider
    extends $NotifierProvider<EmergencyViewModel, EmergencyState> {
  const EmergencyViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'emergencyViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$emergencyViewModelHash();

  @$internal
  @override
  EmergencyViewModel create() => EmergencyViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(EmergencyState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<EmergencyState>(value),
    );
  }
}

String _$emergencyViewModelHash() =>
    r'7264935fb2503809d7884217f25b059d58c706f9';

abstract class _$EmergencyViewModel extends $Notifier<EmergencyState> {
  EmergencyState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<EmergencyState, EmergencyState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<EmergencyState, EmergencyState>,
              EmergencyState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
