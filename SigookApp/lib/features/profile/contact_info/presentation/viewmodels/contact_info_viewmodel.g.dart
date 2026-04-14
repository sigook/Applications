// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'contact_info_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ContactInfoViewModel)
const contactInfoViewModelProvider = ContactInfoViewModelProvider._();

final class ContactInfoViewModelProvider
    extends $NotifierProvider<ContactInfoViewModel, ContactInfoState> {
  const ContactInfoViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'contactInfoViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$contactInfoViewModelHash();

  @$internal
  @override
  ContactInfoViewModel create() => ContactInfoViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ContactInfoState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ContactInfoState>(value),
    );
  }
}

String _$contactInfoViewModelHash() =>
    r'135767f5bcd5628f7cea65a81aac826882e0acc9';

abstract class _$ContactInfoViewModel extends $Notifier<ContactInfoState> {
  ContactInfoState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<ContactInfoState, ContactInfoState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<ContactInfoState, ContactInfoState>,
              ContactInfoState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
