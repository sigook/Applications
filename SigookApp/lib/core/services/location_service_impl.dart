import 'package:flutter/foundation.dart';
import 'package:geolocator/geolocator.dart';
import 'location_service.dart';

class LocationServiceImpl implements LocationService {
  final GeolocatorPlatform _geolocator;

  LocationServiceImpl({GeolocatorPlatform? geolocator})
    : _geolocator = geolocator ?? GeolocatorPlatform.instance;

  LocationSettings _settings({
    required LocationAccuracy accuracy,
    Duration? interval,
  }) {
    switch (defaultTargetPlatform) {
      case TargetPlatform.android:
        return AndroidSettings(accuracy: accuracy, intervalDuration: interval);
      case TargetPlatform.iOS:
        return AppleSettings(
          accuracy: accuracy,
          activityType: ActivityType.otherNavigation,
        );
      default:
        return LocationSettings(accuracy: accuracy);
    }
  }

  @override
  Future<bool> ensurePermissions() async {
    if (!await _geolocator.isLocationServiceEnabled()) return false;
    var permission = await _geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await _geolocator.requestPermission();
      if (permission == LocationPermission.denied) return false;
    }
    return permission != LocationPermission.deniedForever;
  }

  @override
  Future<Position?> getCurrentPosition() async {
    if (!await ensurePermissions()) return null;
    try {
      return await _geolocator.getCurrentPosition(
        locationSettings: _settings(accuracy: LocationAccuracy.high),
      );
    } catch (_) {
      return null;
    }
  }

  @override
  Future<LocationPrecision> resolveLocationPrecision({
    String purposeKey = 'TimesheetPunch',
  }) async {
    final LocationAccuracyStatus status;
    try {
      status = await _geolocator.getLocationAccuracy();
    } catch (_) {
      return LocationPrecision.undetermined;
    }

    switch (status) {
      case LocationAccuracyStatus.precise:
        return LocationPrecision.precise;
      case LocationAccuracyStatus.unknown:
        return LocationPrecision.undetermined;
      case LocationAccuracyStatus.reduced:
        break;
    }

    if (defaultTargetPlatform != TargetPlatform.iOS) {
      return LocationPrecision.reduced;
    }

    try {
      final upgraded = await _geolocator.requestTemporaryFullAccuracy(
        purposeKey: purposeKey,
      );
      return upgraded == LocationAccuracyStatus.precise
          ? LocationPrecision.precise
          : LocationPrecision.reduced;
    } catch (_) {
      return LocationPrecision.undetermined;
    }
  }

  @override
  Future<Position?> getFreshPosition({
    double targetAccuracyMeters = 20,
    Duration maxFixAge = const Duration(seconds: 10),
    Duration overallTimeout = const Duration(seconds: 15),
    Duration fixDroughtTimeout = const Duration(seconds: 5),
  }) async {
    final deadline = DateTime.now().add(overallTimeout);
    Position? best;
    Position? latest;
    final stream = _geolocator
        .getPositionStream(
          locationSettings: _settings(
            accuracy: LocationAccuracy.bestForNavigation,
            interval: const Duration(seconds: 1),
          ),
        )
        .timeout(fixDroughtTimeout, onTimeout: (sink) => sink.close());
    try {
      await for (final position in stream) {
        final age = DateTime.now().difference(position.timestamp);
        if (age <= maxFixAge) {
          latest = position;
          if (best == null || position.accuracy < best.accuracy) {
            best = position;
          }
        }
        if (best != null && best.accuracy <= targetAccuracyMeters) break;
        if (DateTime.now().isAfter(deadline)) break;
      }
    } catch (_) {
      return _freshest(best, latest, maxFixAge);
    }
    return _freshest(best, latest, maxFixAge);
  }

  Position? _freshest(Position? best, Position? latest, Duration maxFixAge) {
    if (best == null) return null;
    if (DateTime.now().difference(best.timestamp) <= maxFixAge) return best;
    if (latest != null &&
        DateTime.now().difference(latest.timestamp) <= maxFixAge) {
      return latest;
    }
    return null;
  }
}
