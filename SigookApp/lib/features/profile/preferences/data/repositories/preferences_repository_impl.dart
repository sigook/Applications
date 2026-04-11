import 'dart:convert';

import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/preferences_repository.dart';

class PreferencesRepositoryImpl implements PreferencesRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  PreferencesRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(Map<String, String> fields) =>
      guardedProfileCall(networkInfo, () async {
        final workerId = await remoteDataSource.getWorkerId();

        List<Map<String, String>> parseIdValueList(String key) {
          final raw = fields[key];
          if (raw == null || raw.isEmpty) return [];
          final decoded = jsonDecode(raw) as List<dynamic>;
          return decoded
              .map((e) => Map<String, String>.from(e as Map))
              .toList();
        }

        final availabilities = parseIdValueList('availabilities');
        final availabilityTimes = parseIdValueList('availabilityTimes');
        final availabilityDays = parseIdValueList('availabilityDays');
        final languages = parseIdValueList('languages');
        final locationPreferences = parseIdValueList('locationPreferences');
        final skills =
            fields['skills'] != null && fields['skills']!.isNotEmpty
                ? (jsonDecode(fields['skills']!) as List<dynamic>).cast<String>()
                : <String>[];

        Map<String, String>? lift;
        if (fields['lift'] != null && fields['lift']!.isNotEmpty) {
          lift = Map<String, String>.from(jsonDecode(fields['lift']!) as Map);
        }

        await Future.wait([
          remoteDataSource.updateAvailabilities(
            workerId,
            availabilities: availabilities,
          ),
          remoteDataSource.updateAvailabilityTimes(
            workerId,
            availabilityTimes: availabilityTimes,
          ),
          remoteDataSource.updateAvailabilityDays(
            workerId,
            availabilityDays: availabilityDays,
          ),
          remoteDataSource.updateSkills(workerId, skills: skills),
          remoteDataSource.updateLanguages(workerId, languages: languages),
          if (locationPreferences.isNotEmpty)
            remoteDataSource.updateLocationPreferences(
              workerId,
              locationPreferences: locationPreferences,
            ),
          if (lift != null)
            remoteDataSource.updateOtherInformation(workerId, lift: lift),
        ]);
      });
}
