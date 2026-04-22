import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/services/file_naming_service.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

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
    required List<LicenseItemModel> existingLicenses,
  }) =>
      execute(() async {
        final fileName = FileNamingService.generateLicenseName(filePath);
        final formData = FormData();
        final dataArray = [
          ...existingLicenses.map((l) => {
            'license': {
              'fileName': l.license?.fileName ?? '',
              'description': l.license?.description ?? 'license',
            },
            'number': l.number ?? '',
            'issued': l.issued ?? '',
            'expires': l.expires ?? '',
          }),
          {
            'license': {'fileName': fileName, 'description': 'license'},
            'number': number,
            'issued': issued,
            'expires': expires,
          },
        ];
        formData.fields.add(MapEntry('data', jsonEncode(dataArray)));
        formData.files.add(MapEntry(
          fileName,
          await MultipartFile.fromFile(filePath, filename: fileName),
        ));
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/Licenses',
          data: formData,
        );
      });

  Future<void> deleteLicense(String workerId, String licenseId) =>
      execute(() => apiClient.dio.delete(
            '/WorkerProfile/$workerId/Licenses/$licenseId',
          ));
}
