// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'profile_image_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ProfileImageViewModel)
const profileImageViewModelProvider = ProfileImageViewModelProvider._();

final class ProfileImageViewModelProvider
    extends $NotifierProvider<ProfileImageViewModel, ProfileImageState> {
  const ProfileImageViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'profileImageViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$profileImageViewModelHash();

  @$internal
  @override
  ProfileImageViewModel create() => ProfileImageViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ProfileImageState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ProfileImageState>(value),
    );
  }
}

String _$profileImageViewModelHash() =>
    r'6f99d6f4d5a19f6898a823ae79b19855cb1805cd';

abstract class _$ProfileImageViewModel extends $Notifier<ProfileImageState> {
  ProfileImageState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<ProfileImageState, ProfileImageState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<ProfileImageState, ProfileImageState>,
              ProfileImageState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
