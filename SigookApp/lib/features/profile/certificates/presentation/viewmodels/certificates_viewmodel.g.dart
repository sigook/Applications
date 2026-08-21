// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'certificates_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(CertificatesViewModel)
const certificatesViewModelProvider = CertificatesViewModelProvider._();

final class CertificatesViewModelProvider
    extends $NotifierProvider<CertificatesViewModel, CertificatesState> {
  const CertificatesViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'certificatesViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$certificatesViewModelHash();

  @$internal
  @override
  CertificatesViewModel create() => CertificatesViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CertificatesState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CertificatesState>(value),
    );
  }
}

String _$certificatesViewModelHash() =>
    r'414186f6a8bc7599a021b33d2fac2a9313e73735';

abstract class _$CertificatesViewModel extends $Notifier<CertificatesState> {
  CertificatesState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<CertificatesState, CertificatesState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<CertificatesState, CertificatesState>,
              CertificatesState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
