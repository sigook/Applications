import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/services/file_naming_service.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class OtherDocumentsRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  OtherDocumentsRemoteDataSource({required this.apiClient});

  Future<void> uploadOtherDocument(
    String workerId, {
    required String filePath,
    required String description,
  }) =>
      execute(() async {
        final fileName = FileNamingService.generateOtherDocumentName(filePath);
        final formData = FormData();
        formData.fields.add(MapEntry(
          'data',
          jsonEncode({'fileName': fileName, 'description': description}),
        ));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/OtherDocument',
          data: formData,
        );
      });

  Future<void> deleteOtherDocument(String workerId, String documentId) =>
      execute(() => apiClient.dio.delete(
            '/WorkerProfile/$workerId/OtherDocument/$documentId',
          ));
}
