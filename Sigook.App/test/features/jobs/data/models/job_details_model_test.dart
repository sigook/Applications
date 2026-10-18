import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/job_details_model.dart';

void main() {
  Map<String, dynamic> baseJson() => {
    'id': 'request-id',
    'jobTitle': 'Forklift operator',
    'numberId': 4242,
    'status': 'Booked',
    'punchCardOptionEnabled': true,
  };

  Map<String, dynamic> checkpointJson() => {
    'latitude': 37.6876,
    'longitude': -77.8964,
    'radiusMeters': 100,
  };

  test('parses the job location coordinates', () {
    final json = baseJson()
      ..['jobLocation'] = {'latitude': 45.4215, 'longitude': -75.6972};

    final model = JobDetailsModel.fromJson(json);

    expect(model.hasCoordinates, isTrue);
    expect(model.latitude, 45.4215);
    expect(model.longitude, -75.6972);
    expect(model.toJson()['jobLocation'], {
      'latitude': 45.4215,
      'longitude': -75.6972,
    });
  });

  test('has no coordinates when the job location is not geocoded', () {
    final json = baseJson()..['jobLocation'] = {'address': '1 Main St'};

    final model = JobDetailsModel.fromJson(json);

    expect(model.hasCoordinates, isFalse);
    expect(model.toJson()['jobLocation'], isNull);
  });

  test('parses the punch checkpoint when the API sends it', () {
    final json = baseJson()..['punchCheckpoint'] = checkpointJson();

    final model = JobDetailsModel.fromJson(json);

    expect(model.hasPunchCheckpoint, isTrue);
    expect(model.checkpointLatitude, 37.6876);
    expect(model.checkpointLongitude, -77.8964);
    expect(model.checkpointRadiusMeters, 100);
  });

  test('has no checkpoint when the API sends null', () {
    final json = baseJson()..['punchCheckpoint'] = null;

    final model = JobDetailsModel.fromJson(json);

    expect(model.hasPunchCheckpoint, isFalse);
    expect(model.toJson()['punchCheckpoint'], isNull);
  });

  test('has no checkpoint when the field is missing', () {
    final model = JobDetailsModel.fromJson(baseJson());

    expect(model.hasPunchCheckpoint, isFalse);
  });

  test('has no checkpoint when the radius is missing', () {
    final json = baseJson()
      ..['punchCheckpoint'] = {'latitude': 37.6876, 'longitude': -77.8964};

    final model = JobDetailsModel.fromJson(json);

    expect(model.hasPunchCheckpoint, isFalse);
  });

  test('round-trips the checkpoint through toJson', () {
    final json = baseJson()..['punchCheckpoint'] = checkpointJson();

    final roundTrip = JobDetailsModel.fromJson(
      JobDetailsModel.fromJson(json).toJson(),
    );

    expect(roundTrip.checkpointLatitude, 37.6876);
    expect(roundTrip.checkpointLongitude, -77.8964);
    expect(roundTrip.checkpointRadiusMeters, 100);
  });
}
