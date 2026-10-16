import 'package:dartz/dartz.dart';
import 'package:flutter/foundation.dart';
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

  Future<Either<Failure, AuthToken>>? _inFlightRefresh;

  AuthRepositoryImpl({
    required this.remote,
    required this.local,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, AuthToken>> signIn({
    required String email,
    required String password,
  }) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final tokenModel = await remote.signIn(email: email, password: password);
      await local.cacheToken(tokenModel);
      return Right(tokenModel.toEntity());
    } on ServerException catch (e) {
      return Left(
        ServerFailure(
          message: e.message,
          statusCode: e.statusCode,
          code: e.code,
        ),
      );
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, AuthToken>> refreshToken(String currentRefreshToken) {
    final inFlight = _inFlightRefresh;
    if (inFlight != null) return inFlight;

    final refresh = _refresh(currentRefreshToken).whenComplete(() {
      _inFlightRefresh = null;
    });
    _inFlightRefresh = refresh;
    return refresh;
  }

  Future<Either<Failure, AuthToken>> _refresh(
    String requestedRefreshToken,
  ) async {
    try {
      var refreshTokenToUse = requestedRefreshToken;
      final cached = await local.getCachedToken();
      final cachedRefreshToken = cached?.refreshToken;
      if (cached != null &&
          cachedRefreshToken != null &&
          cachedRefreshToken.isNotEmpty &&
          cachedRefreshToken != requestedRefreshToken) {
        if (!cached.isExpired()) return Right(cached.toEntity());
        refreshTokenToUse = cachedRefreshToken;
      }

      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final tokenModel = await remote.refreshToken(refreshTokenToUse);
      await local.cacheToken(tokenModel);
      return Right(tokenModel.toEntity());
    } on ServerException catch (e) {
      return Left(
        ServerFailure(
          message: e.message,
          statusCode: e.statusCode,
          code: e.code,
        ),
      );
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(CacheFailure(message: 'Token refresh error: $e'));
    }
  }

  @override
  Future<Either<Failure, void>> requestPasswordResetCode(String email) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      await remote.requestPasswordResetCode(email);
      return Right(null);
    } on ServerException catch (e) {
      return Left(
        ServerFailure(
          message: e.message,
          statusCode: e.statusCode,
          code: e.code,
        ),
      );
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, void>> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  }) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      await remote.resetPassword(
        email: email,
        code: code,
        newPassword: newPassword,
      );
      return Right(null);
    } on ServerException catch (e) {
      return Left(
        ServerFailure(
          message: e.message,
          statusCode: e.statusCode,
          code: e.code,
        ),
      );
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, void>> resendConfirmationLink(String email) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      await remote.resendConfirmationLink(email);
      return Right(null);
    } on ServerException catch (e) {
      return Left(
        ServerFailure(
          message: e.message,
          statusCode: e.statusCode,
          code: e.code,
        ),
      );
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, String>> getUserRole(String accessToken) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final role = await remote.getUserRole(accessToken);
      return Right(role);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message, statusCode: e.statusCode));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, void>> deactivateAccount(String accessToken) async {
    try {
      if (!await networkInfo.isConnected) return Left(NetworkFailure());
      final refreshToken = (await local.getCachedToken())?.refreshToken;
      await remote.deactivateAccount(accessToken);
      if (refreshToken != null) {
        try {
          await remote.revokeRefreshToken(refreshToken);
        } catch (e) {
          debugPrint('Token revocation failed: $e');
        }
      }
      await local.clearToken();
      return Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    try {
      final refreshToken = (await local.getCachedToken())?.refreshToken;

      if (refreshToken != null && await networkInfo.isConnected) {
        try {
          await remote.revokeRefreshToken(refreshToken);
        } catch (e) {
          debugPrint('Token revocation failed: $e');
        }
      }

      await local.clearToken();
      return Right(null);
    } catch (e) {
      await local.clearToken();
      return Left(ServerFailure(message: 'Logout error: ${e.toString()}'));
    }
  }

  @override
  Future<Either<Failure, void>> clearSession() async {
    try {
      await local.clearToken();
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(message: 'Clear session error: $e'));
    }
  }
}
