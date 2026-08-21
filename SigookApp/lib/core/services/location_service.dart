import 'package:geolocator/geolocator.dart';

enum LocationPrecision { precise, reduced, undetermined }

abstract class LocationService {
  Future<bool> ensurePermissions();

  Future<Position?> getCurrentPosition();

  Future<LocationPrecision> resolveLocationPrecision({
    String purposeKey = 'TimesheetPunch',
  });

  Future<Position?> getFreshPosition({
    double targetAccuracyMeters = 20,
    Duration maxFixAge = const Duration(seconds: 10),
    Duration overallTimeout = const Duration(seconds: 15),
    Duration fixDroughtTimeout = const Duration(seconds: 5),
  });
}
