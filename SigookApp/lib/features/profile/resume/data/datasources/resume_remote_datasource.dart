import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/services/file_naming_service.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class ResumeRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  ResumeRemoteDataSource({required this.apiClient});

  Future<void> uploadResume(
    String workerId, {
    required String filePath,
  }) =>
      execute(() async {
        final fileName = FileNamingService.generateResumeName(filePath);
        final formData = FormData();
        formData.fields.add(MapEntry(
          'data',
          jsonEncode({'fileName': fileName, 'description': ''}),
        ));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/Resume',
          data: formData,
        );
      });
}
