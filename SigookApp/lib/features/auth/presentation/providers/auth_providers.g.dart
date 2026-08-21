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

@ProviderFor(requestPasswordResetCode)
const requestPasswordResetCodeProvider = RequestPasswordResetCodeProvider._();

final class RequestPasswordResetCodeProvider
    extends
        $FunctionalProvider<
          RequestPasswordResetCode,
          RequestPasswordResetCode,
          RequestPasswordResetCode
        >
    with $Provider<RequestPasswordResetCode> {
  const RequestPasswordResetCodeProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'requestPasswordResetCodeProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$requestPasswordResetCodeHash();

  @$internal
  @override
  $ProviderElement<RequestPasswordResetCode> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  RequestPasswordResetCode create(Ref ref) {
    return requestPasswordResetCode(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(RequestPasswordResetCode value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<RequestPasswordResetCode>(value),
    );
  }
}

String _$requestPasswordResetCodeHash() =>
    r'f31a43d35951eaf0a455661f166c85500ed71014';

@ProviderFor(resetPassword)
const resetPasswordProvider = ResetPasswordProvider._();

final class ResetPasswordProvider
    extends $FunctionalProvider<ResetPassword, ResetPassword, ResetPassword>
    with $Provider<ResetPassword> {
  const ResetPasswordProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'resetPasswordProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$resetPasswordHash();

  @$internal
  @override
  $ProviderElement<ResetPassword> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  ResetPassword create(Ref ref) {
    return resetPassword(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ResetPassword value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ResetPassword>(value),
    );
  }
}

String _$resetPasswordHash() => r'9d7a2211899fe2d3dd0ddee1ed97bebf623b03fd';

@ProviderFor(resendConfirmationLink)
const resendConfirmationLinkProvider = ResendConfirmationLinkProvider._();

final class ResendConfirmationLinkProvider
    extends
        $FunctionalProvider<
          ResendConfirmationLink,
          ResendConfirmationLink,
          ResendConfirmationLink
        >
    with $Provider<ResendConfirmationLink> {
  const ResendConfirmationLinkProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'resendConfirmationLinkProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$resendConfirmationLinkHash();

  @$internal
  @override
  $ProviderElement<ResendConfirmationLink> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  ResendConfirmationLink create(Ref ref) {
    return resendConfirmationLink(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ResendConfirmationLink value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ResendConfirmationLink>(value),
    );
  }
}

String _$resendConfirmationLinkHash() =>
    r'6cd8d4c4f3c6be5e4aae98aba3cdf8da5bb9a45e';
