import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/worker_profile.dart';
import '../../domain/repositories/profile_repository.dart';
import '../../domain/usecases/update_worker_profile.dart';
import '../datasources/profile_remote_datasource.dart';
import '../models/worker_profile_model.dart';

class ProfileRepositoryImpl implements ProfileRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  ProfileRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, WorkerProfile>> getWorkerBasicInfo() async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final workerId = await remoteDataSource.getWorkerId();
      final profileModel = await remoteDataSource.getWorkerFullProfile(workerId);
      return Right(profileModel.toEntity());
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }

  @override
  Future<Either<Failure, void>> updateWorkerBasicInfo(
    Map<String, String> editedFields, {
    required ProfileSection section,
    Map<String, String>? newFilePaths,
  }) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final workerId = await remoteDataSource.getWorkerId();
      final currentModel =
          await remoteDataSource.getWorkerFullProfile(workerId);

      // Extract document deletion flags before building the model
      final deleteSinFile = editedFields.remove('_deleteSinFile') == 'true';
      final deleteId1File = editedFields.remove('_deleteId1File') == 'true';
      final deleteId2File = editedFields.remove('_deleteId2File') == 'true';
      final deletePoliceCheck =
          editedFields.remove('_deletePoliceCheck') == 'true';

      final updatedModel = currentModel.copyWith(
        firstName: editedFields['firstName'] ?? currentModel.firstName,
        middleName: editedFields['middleName'] ?? currentModel.middleName,
        lastName: editedFields['lastName'] ?? currentModel.lastName,
        secondLastName:
            editedFields['secondLastName'] ?? currentModel.secondLastName,
        hasVehicle: editedFields['hasVehicle'] != null
            ? editedFields['hasVehicle'] == 'true'
            : currentModel.hasVehicle,
        mobileNumber:
            editedFields['mobileNumber'] ?? currentModel.mobileNumber,
        phone: editedFields['phone'] ?? currentModel.phone,
        email: editedFields['email'] ?? currentModel.email,
        contactEmergencyName: editedFields['contactEmergencyName'] ??
            currentModel.contactEmergencyName,
        contactEmergencyLastName: editedFields['contactEmergencyLastName'] ??
            currentModel.contactEmergencyLastName,
        contactEmergencyPhone: editedFields['contactEmergencyPhone'] ??
            currentModel.contactEmergencyPhone,
        socialInsurance:
            editedFields['socialInsurance'] ?? currentModel.socialInsurance,
        identificationNumber1: editedFields['identificationNumber1'] ??
            currentModel.identificationNumber1,
        identificationNumber2: editedFields['identificationNumber2'] ??
            currentModel.identificationNumber2,
        socialInsuranceFile:
            deleteSinFile ? null : currentModel.socialInsuranceFile,
        identificationType1File:
            deleteId1File ? null : currentModel.identificationType1File,
        identificationType2File:
            deleteId2File ? null : currentModel.identificationType2File,
        policeCheckBackGround:
            deletePoliceCheck ? null : currentModel.policeCheckBackGround,
        location: currentModel.location?.copyWith(
          address: editedFields['address'] ?? currentModel.location?.address,
          postalCode:
              editedFields['postalCode'] ?? currentModel.location?.postalCode,
          city: editedFields['cityId'] != null
              ? CityModel(
                  id: editedFields['cityId'],
                  value: '',
                  province: currentModel.location?.city?.province,
                )
              : currentModel.location?.city,
        ),
      );

      switch (section) {
        case ProfileSection.personal:
          await remoteDataSource.updateWorkerBasicInfo(workerId, updatedModel);
        case ProfileSection.contact:
          await remoteDataSource.updateWorkerContactInfo(
              workerId, updatedModel);
        case ProfileSection.emergency:
          await remoteDataSource.updateWorkerEmergencyInfo(
              workerId, updatedModel);
        case ProfileSection.sin:
          await remoteDataSource.updateWorkerSinInfo(
            workerId,
            updatedModel,
            sinFilePath: newFilePaths?['sinFile'],
          );
        case ProfileSection.documents:
          await remoteDataSource.updateWorkerDocuments(
            workerId,
            updatedModel,
            newFilePaths: {
              if (newFilePaths?['id1File'] != null)
                'id1File': newFilePaths!['id1File']!,
              if (newFilePaths?['id2File'] != null)
                'id2File': newFilePaths!['id2File']!,
              if (newFilePaths?['policeCheckFile'] != null)
                'policeCheckFile': newFilePaths!['policeCheckFile']!,
            },
          );
        case ProfileSection.resume:
          if (newFilePaths?['resumeFile'] != null) {
            await remoteDataSource.uploadWorkerResume(
              workerId,
              filePath: newFilePaths!['resumeFile']!,
            );
          }
        case ProfileSection.licenses:
          if (newFilePaths?['licenseFile'] != null) {
            await remoteDataSource.uploadWorkerLicense(
              workerId,
              filePath: newFilePaths!['licenseFile']!,
              number: editedFields['licenseNumber'] ?? '',
              issued: editedFields['licenseIssued'] ?? '',
              expires: editedFields['licenseExpires'] ?? '',
            );
          }
        case ProfileSection.certificates:
          if (newFilePaths?['certificateFile'] != null) {
            await remoteDataSource.uploadWorkerCertificate(
              workerId,
              filePath: newFilePaths!['certificateFile']!,
            );
          }
        case ProfileSection.preferences:
          List<String> parseIds(String key) => (editedFields[key] ?? '')
              .split(',')
              .where((s) => s.isNotEmpty)
              .toList();
          final prefModel = currentModel.copyWith(
            availabilities: parseIds('availabilityIds')
                .map((id) => CatalogItemModel(id: id, value: ''))
                .toList(),
            availabilityTimes: parseIds('availabilityTimeIds')
                .map((id) => CatalogItemModel(id: id, value: ''))
                .toList(),
            availabilityDays: parseIds('availabilityDayIds')
                .map((id) => CatalogItemModel(id: id, value: ''))
                .toList(),
            lift: editedFields['liftId'] != null
                ? CatalogItemModel(id: editedFields['liftId'], value: '')
                : currentModel.lift,
            hasVehicle: editedFields['hasVehicle'] == 'true',
            languages: parseIds('languageIds')
                .map((id) => CatalogItemModel(id: id, value: ''))
                .toList(),
            skills: parseIds('skillIds')
                .map((name) => SkillItemModel(skill: name))
                .toList(),
          );
          await remoteDataSource.updateWorkerBasicInfo(workerId, prefModel);
      }

      return Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
