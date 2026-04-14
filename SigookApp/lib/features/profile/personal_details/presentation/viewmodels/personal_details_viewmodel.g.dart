// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'personal_details_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PersonalDetailsViewModel)
const personalDetailsViewModelProvider = PersonalDetailsViewModelProvider._();

final class PersonalDetailsViewModelProvider
    extends $NotifierProvider<PersonalDetailsViewModel, PersonalDetailsState> {
  const PersonalDetailsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'personalDetailsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$personalDetailsViewModelHash();

  @$internal
  @override
  PersonalDetailsViewModel create() => PersonalDetailsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(PersonalDetailsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<PersonalDetailsState>(value),
    );
  }
}

String _$personalDetailsViewModelHash() =>
    r'5961d9adcaf3819eaa638acc53fbe268264787ee';

abstract class _$PersonalDetailsViewModel
    extends $Notifier<PersonalDetailsState> {
  PersonalDetailsState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<PersonalDetailsState, PersonalDetailsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<PersonalDetailsState, PersonalDetailsState>,
              PersonalDetailsState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
