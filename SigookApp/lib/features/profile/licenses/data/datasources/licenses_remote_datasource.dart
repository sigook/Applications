import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/services/file_naming_service.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class LicensesRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  LicensesRemoteDataSource({required this.apiClient});

  Future<void> uploadLicense(
    String workerId, {
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  }) =>
      execute(() async {
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
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/Licenses',
          data: formData,
        );
      });
}
