/// Base exception for server errors
class ServerException implements Exception {
  final String message;
  final int? statusCode;

  ServerException({
    required this.message,
    this.statusCode,
  });

  @override
  String toString() => 'ServerException: $message (Status: $statusCode)';
}

/// Exception for network/connection errors
class NetworkException implements Exception {
  final String message;

  NetworkException([this.message = 'No internet connection']);

  @override
  String toString() => 'NetworkException: $message';
}

/// Exception for caching errors
class CacheException implements Exception {
  final String message;

  CacheException([this.message = 'Cache error occurred']);

  @override
  String toString() => 'CacheException: $message';
}

/// Exception for parsing/serialization errors
class ParseException implements Exception {
  final String message;

  ParseException([this.message = 'Failed to parse data']);

  @override
  String toString() => 'ParseException: $message';
}
