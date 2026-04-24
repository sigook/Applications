import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

class DocumentsRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  DocumentsRemoteDataSource({required this.apiClient});

  Future<void> updateDocuments(
    String workerId,
    WorkerProfileModel profile, {
    Map<String, String>? newFilePaths,
  }) =>
      execute(() async {
        String? resolveFileName(String key, String? Function() fallback) {
          final path = newFilePaths?[key];
          return path != null
              ? ProfileBaseDatasource.basenameOf(path)
              : fallback();
        }

        final id1FileName = resolveFileName(
          'id1File',
          () => profile.identificationType1File?.fileName,
        );
        final id2FileName = resolveFileName(
          'id2File',
          () => profile.identificationType2File?.fileName,
        );
        final policeCheckFileName = resolveFileName(
          'policeCheckFile',
          () => profile.policeCheckBackGround?.fileName,
        );
        final resumeFileName = resolveFileName(
          'resumeFile',
          () => profile.resume?.fileName,
        );

        final documentsData = <String, dynamic>{
          'havePoliceCheckBackground': profile.havePoliceCheckBackground,
          if (profile.identificationNumber1 != null)
            'identificationNumber1': profile.identificationNumber1,
          if (profile.identificationType1?.id != null)
            'identificationType1': {'id': profile.identificationType1!.id},
          if (id1FileName != null)
            'identificationType1File': {
              'fileName': id1FileName,
              'description':
                  profile.identificationType1File?.description ?? '',
            },
          if (profile.identificationNumber2 != null)
            'identificationNumber2': profile.identificationNumber2,
          if (profile.identificationType2?.id != null)
            'identificationType2': {'id': profile.identificationType2!.id},
          if (id2FileName != null)
            'identificationType2File': {
              'fileName': id2FileName,
              'description':
                  profile.identificationType2File?.description ?? '',
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
}
