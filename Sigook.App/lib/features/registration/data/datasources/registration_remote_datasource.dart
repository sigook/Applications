import 'dart:convert';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:retry/retry.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/services/file_naming_service.dart';
import '../models/worker_registration_request.dart';

abstract class RegistrationRemoteDataSource {
  Future<void> registerWorker(WorkerRegistrationRequest request);
}

class RegistrationRemoteDataSourceImpl implements RegistrationRemoteDataSource {
  final ApiClient apiClient;

  RegistrationRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<void> registerWorker(WorkerRegistrationRequest request) async {
    String? profileImageFileName;
    String? identificationType1FileName;
    String? identificationType2FileName;
    String? resumeFileName;

    if (request.profileImage != null) {
      profileImageFileName = FileNamingService.generateProfileImageName(
        request.profileImage!.pathFile,
      );
    }

    if (request.identificationType1File?.filePath != null) {
      identificationType1FileName = FileNamingService.generateDocumentName(
        request.identificationType1File!.filePath!,
      );
    }

    if (request.identificationType2File?.filePath != null) {
      identificationType2FileName = FileNamingService.generateDocumentName(
        request.identificationType2File!.filePath!,
      );
    }

    if (request.resume?.filePath != null) {
      resumeFileName = FileNamingService.generateResumeName(
        request.resume!.filePath!,
      );
    }

    final workerData = request.toWorkerProfileData(
      profileImageFileName: profileImageFileName,
      identificationType1FileName: identificationType1FileName,
      identificationType2FileName: identificationType2FileName,
      resumeFileName: resumeFileName,
    );
    final jsonData = workerData.toJson();

    final data = FormData();
    data.fields.add(MapEntry('data', jsonEncode(jsonData)));

    if (request.profileImage != null && profileImageFileName != null) {
      data.files.add(
        MapEntry(
          profileImageFileName,
          await MultipartFile.fromFile(
            request.profileImage!.pathFile,
            filename: profileImageFileName,
          ),
        ),
      );
    }

    if (request.identificationType1File != null &&
        request.identificationType1File!.filePath != null &&
        identificationType1FileName != null) {
      data.files.add(
        MapEntry(
          identificationType1FileName,
          await MultipartFile.fromFile(
            request.identificationType1File!.filePath!,
            filename: identificationType1FileName,
          ),
        ),
      );
    }

    if (request.identificationType2File != null &&
        request.identificationType2File!.filePath != null &&
        identificationType2FileName != null) {
      data.files.add(
        MapEntry(
          identificationType2FileName,
          await MultipartFile.fromFile(
            request.identificationType2File!.filePath!,
            filename: identificationType2FileName,
          ),
        ),
      );
    }

    if (request.resume != null &&
        request.resume!.filePath != null &&
        resumeFileName != null) {
      data.files.add(
        MapEntry(
          resumeFileName,
          await MultipartFile.fromFile(
            request.resume!.filePath!,
            filename: resumeFileName,
          ),
        ),
      );
    }

    const retryOptions = RetryOptions(
      maxAttempts: 3,
      delayFactor: Duration(seconds: 2),
      randomizationFactor: 0.25,
      maxDelay: Duration(seconds: 10),
    );

    try {
      final response = await retryOptions.retry(
        () async {
          return await apiClient.post('/worker/profile', data: data);
        },
        retryIf: (e) {
          if (e is DioException) {
            return e.type == DioExceptionType.connectionTimeout ||
                e.type == DioExceptionType.receiveTimeout ||
                e.type == DioExceptionType.sendTimeout ||
                e.type == DioExceptionType.connectionError;
          }
          return e.toString().contains('SocketException');
        },
        onRetry: (e) {
          debugPrint(
            '⚠️ Retrying registration request due to: ${e.toString()}',
          );
        },
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        return;
      } else {
        throw ServerException(
          message: 'Failed to register worker',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException(
          'Request timeout. Please check your internet connection.',
        );
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException(
          'Cannot connect to server. Please check your internet connection.',
        );
      } else if (e.error?.toString().contains('SocketException') == true) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      } else if (e.response != null) {
        String errorMessage = 'Registration failed';
        if (e.response!.data is Map<String, dynamic>) {
          final data = e.response!.data as Map<String, dynamic>;
          final errors = <String>[];
          data.forEach((key, value) {
            if (value is List) {
              errors.addAll(value.map((e) => e.toString()));
            }
          });
          if (errors.isNotEmpty) {
            errorMessage = errors.join('; ');
          }
        }
        throw ServerException(
          message: errorMessage,
          statusCode: e.response!.statusCode,
        );
      } else {
        throw ServerException(
          message: e.message ?? 'Server error occurred',
          statusCode: null,
        );
      }
    } on NetworkException {
      rethrow;
    } on ServerException {
      rethrow;
    } catch (e) {
      if (e.toString().contains('SocketException')) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      }
      throw ServerException(
        message: 'An unexpected error occurred: ${e.toString()}',
        statusCode: null,
      );
    }
  }
}
