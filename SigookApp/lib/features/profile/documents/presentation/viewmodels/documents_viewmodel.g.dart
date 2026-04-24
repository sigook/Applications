// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'documents_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(DocumentsViewModel)
const documentsViewModelProvider = DocumentsViewModelProvider._();

final class DocumentsViewModelProvider
    extends $NotifierProvider<DocumentsViewModel, DocumentsState> {
  const DocumentsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'documentsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$documentsViewModelHash();

  @$internal
  @override
  DocumentsViewModel create() => DocumentsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(DocumentsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<DocumentsState>(value),
    );
  }
}

String _$documentsViewModelHash() =>
    r'2f61535386658289da38c58d30f0c5b2d0ff66b4';

abstract class _$DocumentsViewModel extends $Notifier<DocumentsState> {
  DocumentsState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<DocumentsState, DocumentsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<DocumentsState, DocumentsState>,
              DocumentsState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
