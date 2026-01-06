// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'timesheet_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(TimesheetViewModel)
const timesheetViewModelProvider = TimesheetViewModelFamily._();

final class TimesheetViewModelProvider
    extends $NotifierProvider<TimesheetViewModel, TimesheetState> {
  const TimesheetViewModelProvider._({
    required TimesheetViewModelFamily super.from,
    required String super.argument,
  }) : super(
         retry: null,
         name: r'timesheetViewModelProvider',
         isAutoDispose: true,
         dependencies: null,
         $allTransitiveDependencies: null,
       );

  @override
  String debugGetCreateSourceHash() => _$timesheetViewModelHash();

  @override
  String toString() {
    return r'timesheetViewModelProvider'
        ''
        '($argument)';
  }

  @$internal
  @override
  TimesheetViewModel create() => TimesheetViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(TimesheetState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<TimesheetState>(value),
    );
  }

  @override
  bool operator ==(Object other) {
    return other is TimesheetViewModelProvider && other.argument == argument;
  }

  @override
  int get hashCode {
    return argument.hashCode;
  }
}

String _$timesheetViewModelHash() =>
    r'd472dee078aef654d3187698781207642f50d0bc';

final class TimesheetViewModelFamily extends $Family
    with
        $ClassFamilyOverride<
          TimesheetViewModel,
          TimesheetState,
          TimesheetState,
          TimesheetState,
          String
        > {
  const TimesheetViewModelFamily._()
    : super(
        retry: null,
        name: r'timesheetViewModelProvider',
        dependencies: null,
        $allTransitiveDependencies: null,
        isAutoDispose: true,
      );

  TimesheetViewModelProvider call(String jobId) =>
      TimesheetViewModelProvider._(argument: jobId, from: this);

  @override
  String toString() => r'timesheetViewModelProvider';
}

abstract class _$TimesheetViewModel extends $Notifier<TimesheetState> {
  late final _$args = ref.$arg as String;
  String get jobId => _$args;

  TimesheetState build(String jobId);
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build(_$args);
    final ref = this.ref as $Ref<TimesheetState, TimesheetState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<TimesheetState, TimesheetState>,
              TimesheetState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
