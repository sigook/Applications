// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'analytics_providers.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(analyticsService)
const analyticsServiceProvider = AnalyticsServiceProvider._();

final class AnalyticsServiceProvider
    extends
        $FunctionalProvider<
          AnalyticsService,
          AnalyticsService,
          AnalyticsService
        >
    with $Provider<AnalyticsService> {
  const AnalyticsServiceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'analyticsServiceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$analyticsServiceHash();

  @$internal
  @override
  $ProviderElement<AnalyticsService> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  AnalyticsService create(Ref ref) {
    return analyticsService(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AnalyticsService value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AnalyticsService>(value),
    );
  }
}

String _$analyticsServiceHash() => r'abeffc869bc3a0b4309cef916d64847b863d9fd2';

@ProviderFor(crashReportingService)
const crashReportingServiceProvider = CrashReportingServiceProvider._();

final class CrashReportingServiceProvider
    extends
        $FunctionalProvider<
          CrashReportingService,
          CrashReportingService,
          CrashReportingService
        >
    with $Provider<CrashReportingService> {
  const CrashReportingServiceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'crashReportingServiceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$crashReportingServiceHash();

  @$internal
  @override
  $ProviderElement<CrashReportingService> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CrashReportingService create(Ref ref) {
    return crashReportingService(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CrashReportingService value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CrashReportingService>(value),
    );
  }
}

String _$crashReportingServiceHash() =>
    r'4b4ffd8e2afc468495ddeae43577a21e2da5d75c';
