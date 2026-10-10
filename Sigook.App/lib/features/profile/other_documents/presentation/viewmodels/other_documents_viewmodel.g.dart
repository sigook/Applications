// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'other_documents_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(OtherDocumentsViewModel)
final otherDocumentsViewModelProvider = OtherDocumentsViewModelProvider._();

final class OtherDocumentsViewModelProvider
    extends $NotifierProvider<OtherDocumentsViewModel, OtherDocumentsState> {
  OtherDocumentsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'otherDocumentsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$otherDocumentsViewModelHash();

  @$internal
  @override
  OtherDocumentsViewModel create() => OtherDocumentsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(OtherDocumentsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<OtherDocumentsState>(value),
    );
  }
}

String _$otherDocumentsViewModelHash() =>
    r'8fd727f70ab4974b1df70de24017b926a984486f';

abstract class _$OtherDocumentsViewModel
    extends $Notifier<OtherDocumentsState> {
  OtherDocumentsState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<OtherDocumentsState, OtherDocumentsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<OtherDocumentsState, OtherDocumentsState>,
              OtherDocumentsState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
