import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

class SinRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  SinRemoteDataSource({required this.apiClient});

  Future<void> updateSinInfo(
    String workerId,
    WorkerProfileModel profile, {
    String? sinFilePath,
  }) =>
      execute(() async {
        final String? fileName = sinFilePath != null
            ? ProfileBaseDatasource.basenameOf(sinFilePath)
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
}
