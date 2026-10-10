import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';

/// Wraps any profile repository call with the standard network check
/// and error mapping to [Failure], avoiding 8 identical try/catch blocks.
Future<Either<Failure, void>> guardedProfileCall(
  NetworkInfo networkInfo,
  Future<void> Function() call,
) async {
  if (!await networkInfo.isConnected) return Left(NetworkFailure());
  try {
    await call();
    return Right(null);
  } on ServerException catch (e) {
    return Left(ServerFailure(message: e.message));
  } on NetworkException catch (e) {
    return Left(NetworkFailure(message: e.message));
  } catch (e) {
    return Left(ServerFailure(message: 'Unexpected error: $e'));
  }
}
