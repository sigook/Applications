import 'package:equatable/equatable.dart';

/// Base failure class
abstract class Failure extends Equatable {
  final String message;

  const Failure({this.message = 'An error occurred'});

  @override
  List<Object?> get props => [message];
}

/// Server failure (4xx, 5xx errors)
class ServerFailure extends Failure {
  const ServerFailure({super.message = 'Server error occurred'});
}

/// Network failure (no connection, timeout)
class NetworkFailure extends Failure {
  const NetworkFailure({super.message = 'No internet connection'});
}

/// Cache failure (local storage errors)
class CacheFailure extends Failure {
  const CacheFailure({super.message = 'Cache error occurred'});
}

/// Parse failure (JSON parsing errors)
class ParseFailure extends Failure {
  const ParseFailure({super.message = 'Failed to parse data'});
}

/// Validation failure (input validation errors)
class ValidationFailure extends Failure {
  const ValidationFailure({super.message = 'Validation error'});
}

class PermissionFailure extends Failure {
  const PermissionFailure({super.message = 'Permission denied by user'});
}

class UserCancelledFailure extends Failure {
  const UserCancelledFailure({super.message = 'User cancelled the operation'});
}
