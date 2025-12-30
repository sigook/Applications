// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'auth_providers.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(signIn)
const signInProvider = SignInProvider._();

final class SignInProvider extends $FunctionalProvider<SignIn, SignIn, SignIn>
    with $Provider<SignIn> {
  const SignInProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'signInProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$signInHash();

  @$internal
  @override
  $ProviderElement<SignIn> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  SignIn create(Ref ref) {
    return signIn(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(SignIn value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<SignIn>(value),
    );
  }
}

String _$signInHash() => r'22614f11f9564296d0cb7bdf5e3a0064602a9f75';

@ProviderFor(refreshToken)
const refreshTokenProvider = RefreshTokenProvider._();

final class RefreshTokenProvider
    extends $FunctionalProvider<RefreshToken, RefreshToken, RefreshToken>
    with $Provider<RefreshToken> {
  const RefreshTokenProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'refreshTokenProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$refreshTokenHash();

  @$internal
  @override
  $ProviderElement<RefreshToken> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  RefreshToken create(Ref ref) {
    return refreshToken(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(RefreshToken value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<RefreshToken>(value),
    );
  }
}

String _$refreshTokenHash() => r'61f13c03571f269236663bf6681ffa5df3c78619';

@ProviderFor(logout)
const logoutProvider = LogoutProvider._();

final class LogoutProvider extends $FunctionalProvider<Logout, Logout, Logout>
    with $Provider<Logout> {
  const LogoutProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'logoutProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$logoutHash();

  @$internal
  @override
  $ProviderElement<Logout> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  Logout create(Ref ref) {
    return logout(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(Logout value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<Logout>(value),
    );
  }
}

String _$logoutHash() => r'3f851dcbb9e8569c953ef50df79359f905c186c8';

@ProviderFor(currentAuthToken)
const currentAuthTokenProvider = CurrentAuthTokenProvider._();

final class CurrentAuthTokenProvider
    extends $FunctionalProvider<AuthToken?, AuthToken?, AuthToken?>
    with $Provider<AuthToken?> {
  const CurrentAuthTokenProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'currentAuthTokenProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$currentAuthTokenHash();

  @$internal
  @override
  $ProviderElement<AuthToken?> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  AuthToken? create(Ref ref) {
    return currentAuthToken(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AuthToken? value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AuthToken?>(value),
    );
  }
}

String _$currentAuthTokenHash() => r'3df1e536fe7e48ff406f7d9c1ec1d83c1076ec05';
