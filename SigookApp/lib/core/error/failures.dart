import 'package:equatable/equatable.dart';

abstract class Failure extends Equatable {
  final String message;

  const Failure({this.message = 'An error occurred'});

  @override
  List<Object?> get props => [message];
}

class ServerFailure extends Failure {
  const ServerFailure({super.message = 'Server error occurred'});
}

class NetworkFailure extends Failure {
  const NetworkFailure({super.message = 'No internet connection'});
}

class CacheFailure extends Failure {
  const CacheFailure({super.message = 'Cache error occurred'});
}

class ParseFailure extends Failure {
  const ParseFailure({super.message = 'Failed to parse data'});
}

class ValidationFailure extends Failure {
  const ValidationFailure({super.message = 'Validation error'});
}

class PermissionFailure extends Failure {
  const PermissionFailure({super.message = 'Permission denied by user'});
}

class UserCancelledFailure extends Failure {
  const UserCancelledFailure({super.message = 'User cancelled the operation'});
}
