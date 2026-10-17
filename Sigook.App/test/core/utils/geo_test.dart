import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/geo.dart';

void main() {
  const pinLatitude = 37.6876;
  const pinLongitude = -77.8964;
  const oneDegreeLatitudeMeters = 111290.0;

  test('distance is zero at the same point', () {
    expect(
      distanceInMeters(pinLatitude, pinLongitude, pinLatitude, pinLongitude),
      0,
    );
  });

  test('distance of one thousandth of a degree north is about 111 m', () {
    final meters = distanceInMeters(
      pinLatitude,
      pinLongitude,
      pinLatitude + 0.001,
      pinLongitude,
    );
    expect(meters, closeTo(oneDegreeLatitudeMeters / 1000, 1));
  });

  test('distance is symmetric', () {
    final forward = distanceInMeters(pinLatitude, pinLongitude, 37.6900, -77.9000);
    final backward = distanceInMeters(37.6900, -77.9000, pinLatitude, pinLongitude);
    expect(forward, closeTo(backward, 0.000001));
  });

  group('formatDistance', () {
    test('meters under one kilometre', () {
      expect(formatDistance(164.6), '165 m');
    });

    test('one decimal under ten kilometres', () {
      expect(formatDistance(2460), '2.5 km');
    });

    test('whole kilometres from ten kilometres', () {
      expect(formatDistance(875111), '875 km');
    });
  });

  group('cardinalDirection from the pin to the worker', () {
    test('north', () {
      expect(
        cardinalDirection(pinLatitude, pinLongitude, pinLatitude + 0.001, pinLongitude),
        'north',
      );
    });

    test('south', () {
      expect(
        cardinalDirection(pinLatitude, pinLongitude, pinLatitude - 0.001, pinLongitude),
        'south',
      );
    });

    test('east', () {
      expect(
        cardinalDirection(pinLatitude, pinLongitude, pinLatitude, pinLongitude + 0.001),
        'east',
      );
    });

    test('west', () {
      expect(
        cardinalDirection(pinLatitude, pinLongitude, pinLatitude, pinLongitude - 0.001),
        'west',
      );
    });

    test('south-west', () {
      expect(
        cardinalDirection(
          pinLatitude,
          pinLongitude,
          pinLatitude - 0.001,
          pinLongitude - 0.001 / 0.7918,
        ),
        'south-west',
      );
    });
  });
}
