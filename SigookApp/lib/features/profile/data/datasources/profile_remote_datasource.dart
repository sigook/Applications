import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/dio_error_interceptor.dart';
import '../../../../core/services/file_naming_service.dart';
import '../models/worker_profile_model.dart';

abstract class ProfileRemoteDataSource {
  Future<String> getWorkerId();
  Future<WorkerProfileModel> getWorkerFullProfile(String workerId);
  Future<void> updateWorkerBasicInfo(
    String workerId,
    WorkerProfileModel profile,
  );
  Future<void> updateWorkerContactInfo(
    String workerId,
    WorkerProfileModel profile,
  );
  Future<void> updateWorkerSinInfo(
    String workerId,
    WorkerProfileModel profile, {
    String? sinFilePath,
  });
  Future<void> updateWorkerDocuments(
    String workerId,
    WorkerProfileModel profile, {
    Map<String, String>? newFilePaths,
  });
  Future<void> updateWorkerEmergencyInfo(
    String workerId,
    WorkerProfileModel profile,
  );
  Future<void> uploadWorkerResume(
    String workerId, {
    required String filePath,
  });
  Future<void> uploadWorkerLicense(
    String workerId, {
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  });
  Future<void> uploadWorkerCertificate(
    String workerId, {
    required String filePath,
  });

  // Preference update endpoints
  Future<void> updateOtherInformation(
    String workerId, {
    required Map<String, String> lift,
  });
  Future<void> updateAvailabilities(
    String workerId, {
    required List<Map<String, String>> availabilities,
  });
  Future<void> updateAvailabilityTimes(
    String workerId, {
    required List<Map<String, String>> availabilityTimes,
  });
  Future<void> updateAvailabilityDays(
    String workerId, {
    required List<Map<String, String>> availabilityDays,
  });
  Future<void> updateLocationPreferences(
    String workerId, {
    required List<Map<String, String>> locationPreferences,
  });
  Future<void> updateSkills(
    String workerId, {
    required List<String> skills,
  });
  Future<void> updateLanguages(
    String workerId, {
    required List<Map<String, String>> languages,
  });
}

class ProfileRemoteDataSourceImpl implements ProfileRemoteDataSource {
  final ApiClient apiClient;

  ProfileRemoteDataSourceImpl({required this.apiClient});

  Future<T> _execute<T>(Future<T> Function() call) async {
    try {
      return await call();
    } on DioException catch (e) {
      handleDioException(e);
    } on ServerException {
      rethrow;
    } on NetworkException {
      rethrow;
    } catch (e) {
      throw ServerException(message: 'Unexpected error: $e');
    }
  }

  static String _basenameOf(String path) =>
      path.split(RegExp(r'[/\\]')).last;

  @override
  Future<String> getWorkerId() => _execute(() async {
        final response = await apiClient.dio.get('/WorkerProfile');
        final list = response.data as List<dynamic>;
        if (list.isEmpty) {
          throw ServerException(message: 'No worker profile found');
        }
        return WorkerProfileListItemModel.fromJson(
          list[0] as Map<String, dynamic>,
        ).id;
      });

  @override
  Future<WorkerProfileModel> getWorkerFullProfile(String workerId) =>
      _execute(() async {
        final response = await apiClient.dio.get('/WorkerProfile/$workerId');
        return WorkerProfileModel.fromJson(
          response.data as Map<String, dynamic>,
        );
      });

  @override
  Future<void> updateWorkerBasicInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      _execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/BasicInformation',
            data: profile.toJson(),
          ));

  @override
  Future<void> updateWorkerContactInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      _execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/ContactInformation',
            data: profile.toJson(),
          ));

  @override
  Future<void> updateWorkerEmergencyInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      _execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/EmergencyInformation',
            data: {
              'haveAnyHealthProblem': profile.haveAnyHealthProblem,
              if (profile.healthProblem != null)
                'healthProblem': profile.healthProblem,
              if (profile.otherHealthProblem != null)
                'otherHealthProblem': profile.otherHealthProblem,
              if (profile.contactEmergencyName != null)
                'contactEmergencyName': profile.contactEmergencyName,
              if (profile.contactEmergencyLastName != null)
                'contactEmergencyLastName': profile.contactEmergencyLastName,
              if (profile.contactEmergencyPhone != null)
                'contactEmergencyPhone': profile.contactEmergencyPhone,
            },
          ));

  @override
  Future<void> updateWorkerSinInfo(
    String workerId,
    WorkerProfileModel profile, {
    String? sinFilePath,
  }) =>
      _execute(() async {
        final String? fileName = sinFilePath != null
            ? _basenameOf(sinFilePath)
            : profile.socialInsuranceFile?.fileName;

        final sinData = <String, dynamic>{
          'socialInsurance': profile.socialInsurance,
          'socialInsuranceExpire': profile.socialInsuranceExpire,
          if (profile.dueDate != null) 'dueDate': profile.dueDate,
          if (fileName != null)
            'socialInsuranceFile': {
              'fileName': fileName,
              'description': profile.socialInsuranceFile?.description ?? '',
            },
        };

        final formData = FormData();
        formData.fields.add(MapEntry('data', jsonEncode(sinData)));
        if (sinFilePath != null && fileName != null) {
          formData.files.add(MapEntry(
            fileName,
            await MultipartFile.fromFile(sinFilePath, filename: fileName),
          ));
        }

        await apiClient.dio.post(
          '/WorkerProfile/$workerId/SinInformation',
          data: formData,
        );
      });

  @override
  Future<void> updateWorkerDocuments(
    String workerId,
    WorkerProfileModel profile, {
    Map<String, String>? newFilePaths,
  }) =>
      _execute(() async {
        String? resolveFileName(String key, String? Function() fallback) {
          final path = newFilePaths?[key];
          return path != null ? _basenameOf(path) : fallback();
        }

        final id1FileName = resolveFileName('id1File', () => profile.identificationType1File?.fileName);
        final id2FileName = resolveFileName('id2File', () => profile.identificationType2File?.fileName);
        final policeCheckFileName = resolveFileName('policeCheckFile', () => profile.policeCheckBackGround?.fileName);
        final resumeFileName = resolveFileName('resumeFile', () => profile.resume?.fileName);

        final documentsData = <String, dynamic>{
          'havePoliceCheckBackground': profile.havePoliceCheckBackground,
          if (profile.identificationNumber1 != null)
            'identificationNumber1': profile.identificationNumber1,
          if (profile.identificationType1?.id != null)
            'identificationType1': {'id': profile.identificationType1!.id},
          if (id1FileName != null)
            'identificationType1File': {
              'fileName': id1FileName,
              'description': profile.identificationType1File?.description ?? '',
            },
          if (profile.identificationNumber2 != null)
            'identificationNumber2': profile.identificationNumber2,
          if (profile.identificationType2?.id != null)
            'identificationType2': {'id': profile.identificationType2!.id},
          if (id2FileName != null)
            'identificationType2File': {
              'fileName': id2FileName,
              'description': profile.identificationType2File?.description ?? '',
            },
          if (policeCheckFileName != null)
            'policeCheckBackGround': {'fileName': policeCheckFileName},
          if (resumeFileName != null)
            'resume': {'fileName': resumeFileName},
        };

        final formData = FormData();
        formData.fields.add(MapEntry('data', jsonEncode(documentsData)));

        Future<void> attachFile(String? path, String? name) async {
          if (path != null && name != null) {
            formData.files.add(MapEntry(
              name,
              await MultipartFile.fromFile(path, filename: name),
            ));
          }
        }

        await attachFile(newFilePaths?['id1File'], id1FileName);
        await attachFile(newFilePaths?['id2File'], id2FileName);
        await attachFile(newFilePaths?['policeCheckFile'], policeCheckFileName);
        await attachFile(newFilePaths?['resumeFile'], resumeFileName);

        await apiClient.dio.post(
          '/WorkerProfile/$workerId/Documents',
          data: formData,
        );
      });

  @override
  Future<void> uploadWorkerResume(String workerId, {required String filePath}) =>
      _execute(() async {
        final fileName = _basenameOf(filePath);
        final formData = FormData();
        formData.fields.add(MapEntry(
          'data',
          jsonEncode({'fileName': fileName, 'description': ''}),
        ));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post('/WorkerProfile/$workerId/Resume', data: formData);
      });

  @override
  Future<void> uploadWorkerLicense(
    String workerId, {
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  }) =>
      _execute(() async {
        final fileName = FileNamingService.generateLicenseName(filePath);
        final formData = FormData();
        formData.fields.add(MapEntry(
          'data',
          jsonEncode([
            {
              'license': {'fileName': fileName, 'description': 'license'},
              'number': number,
              'issued': issued,
              'expires': expires,
            }
          ]),
        ));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post('/WorkerProfile/$workerId/Licenses', data: formData);
      });

  @override
  Future<void> uploadWorkerCertificate(String workerId, {required String filePath}) =>
      _execute(() async {
        final fileName = FileNamingService.generateCertificateName(filePath);
        final formData = FormData();
        formData.fields.add(MapEntry(
          'data',
          jsonEncode([
            {'fileName': fileName, 'description': 'CERTIFICATE'}
          ]),
        ));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post('/WorkerProfile/$workerId/Certificates', data: formData);
      });

  // ── Preference update endpoints ──────────────────────────────────────────

  Future<void> _postPreference(String workerId, String path, Object data) =>
      _execute(() => apiClient.dio.post('/WorkerProfile/$workerId/$path', data: data));

  @override
  Future<void> updateOtherInformation(String workerId, {required Map<String, String> lift}) =>
      _postPreference(workerId, 'OtherInformation', {'lift': lift});

  @override
  Future<void> updateAvailabilities(String workerId, {required List<Map<String, String>> availabilities}) =>
      _postPreference(workerId, 'Availabilities', availabilities);

  @override
  Future<void> updateAvailabilityTimes(String workerId, {required List<Map<String, String>> availabilityTimes}) =>
      _postPreference(workerId, 'AvailabilityTimes', availabilityTimes);

  @override
  Future<void> updateAvailabilityDays(String workerId, {required List<Map<String, String>> availabilityDays}) =>
      _postPreference(workerId, 'AvailabilityDays', availabilityDays);

  @override
  Future<void> updateLocationPreferences(String workerId, {required List<Map<String, String>> locationPreferences}) =>
      _postPreference(workerId, 'LocationPreferences', locationPreferences);

  @override
  Future<void> updateSkills(String workerId, {required List<String> skills}) =>
      _postPreference(workerId, 'Skills', skills);

  @override
  Future<void> updateLanguages(String workerId, {required List<Map<String, String>> languages}) =>
      _postPreference(workerId, 'Languages', languages);
}
