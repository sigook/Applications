import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/network/network_info.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/auth_token.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_local_datasource.dart';
import '../datasources/auth_remote_datasource.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remote;
  final AuthLocalDataSource local;
  final NetworkInfo networkInfo;

  AuthRepositoryImpl({
    required this.remote,
    required this.local,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, AuthToken>> signIn() async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final token = await remote.signIn();
      await local.cacheToken(token);
      return Right(token);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, AuthToken>> refreshToken(
    String currentRefreshToken,
  ) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final token = await remote.refreshToken(currentRefreshToken);
      await local.cacheToken(token);
      return Right(token);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      await remote.logout();
      await local.clearToken();
      return Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }
}
