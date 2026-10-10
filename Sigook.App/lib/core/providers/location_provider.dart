import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../services/location_service.dart';
import '../services/location_service_impl.dart';

final locationServiceProvider = Provider<LocationService>((ref) {
  return LocationServiceImpl();
});
