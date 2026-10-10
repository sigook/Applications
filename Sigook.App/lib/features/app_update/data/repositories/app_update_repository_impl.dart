import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/app_version_requirement.dart';
import '../../domain/repositories/app_update_repository.dart';
import '../datasources/app_update_remote_datasource.dart';

class AppUpdateRepositoryImpl implements AppUpdateRepository {
  final AppUpdateRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  AppUpdateRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, AppVersionRequirement>> getVersionRequirement() async {
    if (!await networkInfo.isConnected) {
      return const Left(NetworkFailure());
    }
    try {
      final model = await remoteDataSource.getVersionRequirement();
      return Right(model.toEntity());
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message, statusCode: e.statusCode));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } on ParseException catch (e) {
      return Left(ParseFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
