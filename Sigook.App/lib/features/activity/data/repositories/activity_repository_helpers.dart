import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';

Future<Either<Failure, T>> guardedActivityCall<T>(
  NetworkInfo networkInfo,
  Future<T> Function() call,
) async {
  if (!await networkInfo.isConnected) return const Left(NetworkFailure());
  try {
    return Right(await call());
  } on ServerException catch (e) {
    return Left(ServerFailure(message: e.message));
  } on NetworkException catch (e) {
    return Left(NetworkFailure(message: e.message));
  } catch (e) {
    return Left(ServerFailure(message: 'Unexpected error: $e'));
  }
}
