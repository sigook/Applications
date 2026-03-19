import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
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
}

class ProfileRemoteDataSourceImpl implements ProfileRemoteDataSource {
  final ApiClient apiClient;

  ProfileRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<String> getWorkerId() async {
    try {
      final response = await apiClient.dio.get('/WorkerProfile');

      if (response.statusCode == 200) {
        final list = response.data as List<dynamic>;
        if (list.isEmpty) {
          throw ServerException(message: 'No worker profile found');
        }
        final item = WorkerProfileListItemModel.fromJson(
          list[0] as Map<String, dynamic>,
        );
        return item.id;
      } else {
        throw ServerException(
          message: 'Failed to load worker profiles: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to load worker profiles: $e');
    }
  }

  @override
  Future<WorkerProfileModel> getWorkerFullProfile(String workerId) async {
    try {
      final response = await apiClient.dio.get(
        '/WorkerProfile/$workerId',
      );

      if (response.statusCode == 200) {
        return WorkerProfileModel.fromJson(
          response.data as Map<String, dynamic>,
        );
      } else {
        throw ServerException(
          message: 'Failed to load worker profile: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to load worker profile: $e');
    }
  }

  @override
  Future<void> updateWorkerBasicInfo(
    String workerId,
    WorkerProfileModel profile,
  ) async {
    try {
      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/BasicInformation',
        data: profile.toJson(),
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to update worker profile: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to update worker profile: $e');
    }
  }

  @override
  Future<void> updateWorkerContactInfo(
    String workerId,
    WorkerProfileModel profile,
  ) async {
    try {
      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/ContactInformation',
        data: profile.toJson(),
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to update contact info: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to update contact info: $e');
    }
  }

  @override
  Future<void> updateWorkerEmergencyInfo(
    String workerId,
    WorkerProfileModel profile,
  ) async {
    try {
      final emergencyData = <String, dynamic>{
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
      };

      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/EmergencyInformation',
        data: emergencyData,
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to update emergency info: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to update emergency info: $e');
    }
  }

  static String _basenameOf(String path) =>
      path.split(RegExp(r'[/\\]')).last;

  @override
  Future<void> updateWorkerSinInfo(
    String workerId,
    WorkerProfileModel profile, {
    String? sinFilePath,
  }) async {
    try {
      // Determine effective filename: newly picked file takes precedence over existing
      final String? fileName = sinFilePath != null
          ? _basenameOf(sinFilePath)
          : profile.socialInsuranceFile?.fileName;

      final sinData = <String, dynamic>{
        'socialInsurance': profile.socialInsurance,
        'socialInsuranceExpire': profile.socialInsuranceExpire,
      };
      if (profile.dueDate != null) {
        sinData['dueDate'] = profile.dueDate;
      }
      if (fileName != null) {
        sinData['socialInsuranceFile'] = {
          'fileName': fileName,
          'description': profile.socialInsuranceFile?.description ?? '',
        };
      }

      final formData = FormData();
      formData.fields.add(MapEntry('data', jsonEncode(sinData)));

      if (sinFilePath != null && fileName != null) {
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(sinFilePath, filename: fileName),
        ));
      }

      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/SinInformation',
        data: formData,
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to update SIN info: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to update SIN info: $e');
    }
  }

  @override
  Future<void> updateWorkerDocuments(
    String workerId,
    WorkerProfileModel profile, {
    Map<String, String>? newFilePaths,
  }) async {
    try {
      // For each document field, use the new file name (from picked local path)
      // if provided; otherwise fall back to the existing name from the profile.
      final id1FileName = newFilePaths?['id1File'] != null
          ? _basenameOf(newFilePaths!['id1File']!)
          : profile.identificationType1File?.fileName;
      final id2FileName = newFilePaths?['id2File'] != null
          ? _basenameOf(newFilePaths!['id2File']!)
          : profile.identificationType2File?.fileName;
      final policeCheckFileName = newFilePaths?['policeCheckFile'] != null
          ? _basenameOf(newFilePaths!['policeCheckFile']!)
          : profile.policeCheckBackGround?.fileName;
      final resumeFileName = newFilePaths?['resumeFile'] != null
          ? _basenameOf(newFilePaths!['resumeFile']!)
          : profile.resume?.fileName;

      final documentsData = <String, dynamic>{
        'havePoliceCheckBackground': profile.havePoliceCheckBackground,
      };

      if (profile.identificationNumber1 != null) {
        documentsData['identificationNumber1'] = profile.identificationNumber1;
      }
      if (profile.identificationType1?.id != null) {
        documentsData['identificationType1'] = {
          'id': profile.identificationType1!.id,
        };
      }
      if (id1FileName != null) {
        documentsData['identificationType1File'] = {
          'fileName': id1FileName,
          'description': profile.identificationType1File?.description ?? '',
        };
      }
      if (profile.identificationNumber2 != null) {
        documentsData['identificationNumber2'] = profile.identificationNumber2;
      }
      if (profile.identificationType2?.id != null) {
        documentsData['identificationType2'] = {
          'id': profile.identificationType2!.id,
        };
      }
      if (id2FileName != null) {
        documentsData['identificationType2File'] = {
          'fileName': id2FileName,
          'description': profile.identificationType2File?.description ?? '',
        };
      }
      if (policeCheckFileName != null) {
        documentsData['policeCheckBackGround'] = {
          'fileName': policeCheckFileName,
        };
      }
      if (resumeFileName != null) {
        documentsData['resume'] = {'fileName': resumeFileName};
      }

      final formData = FormData();
      formData.fields.add(MapEntry('data', jsonEncode(documentsData)));

      // Attach any newly picked files as multipart parts.
      // The part name must match the fileName value in the JSON data field.
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

      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/Documents',
        data: formData,
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to update documents: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to update documents: $e');
    }
  }

  @override
  Future<void> uploadWorkerResume(
    String workerId, {
    required String filePath,
  }) async {
    try {
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

      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/Resume',
        data: formData,
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(
          message: 'Failed to upload resume: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to upload resume: $e');
    }
  }
}
