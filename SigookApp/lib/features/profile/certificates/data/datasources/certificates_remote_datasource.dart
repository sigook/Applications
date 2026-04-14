import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/services/file_naming_service.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class CertificatesRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  CertificatesRemoteDataSource({required this.apiClient});

  Future<void> uploadCertificate(
    String workerId, {
    required String filePath,
  }) =>
      execute(() async {
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
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/Certificates',
          data: formData,
        );
      });
}
