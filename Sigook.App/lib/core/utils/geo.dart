import 'dart:math' as math;

const double _earthRadiusMeters = 6376500.0;
const double _degreesToRadians = math.pi / 180.0;

const List<String> _cardinalDirections = [
  'north',
  'north-east',
  'east',
  'south-east',
  'south',
  'south-west',
  'west',
  'north-west',
];

double distanceInMeters(
  double latitude1,
  double longitude1,
  double latitude2,
  double longitude2,
) {
  final lat1 = latitude1 * _degreesToRadians;
  final lon1 = longitude1 * _degreesToRadians;
  final lat2 = latitude2 * _degreesToRadians;
  final deltaLon = longitude2 * _degreesToRadians - lon1;
  final a =
      math.pow(math.sin((lat2 - lat1) / 2.0), 2.0) +
      math.cos(lat1) * math.cos(lat2) * math.pow(math.sin(deltaLon / 2.0), 2.0);
  return _earthRadiusMeters *
      (2.0 * math.atan2(math.sqrt(a), math.sqrt(1.0 - a)));
}

String cardinalDirection(
  double fromLatitude,
  double fromLongitude,
  double toLatitude,
  double toLongitude,
) {
  final lat1 = fromLatitude * _degreesToRadians;
  final lat2 = toLatitude * _degreesToRadians;
  final deltaLon = (toLongitude - fromLongitude) * _degreesToRadians;
  final y = math.sin(deltaLon) * math.cos(lat2);
  final x =
      math.cos(lat1) * math.sin(lat2) -
      math.sin(lat1) * math.cos(lat2) * math.cos(deltaLon);
  final bearing = (math.atan2(y, x) / _degreesToRadians + 360.0) % 360.0;
  return _cardinalDirections[(bearing / 45.0).round() % 8];
}
