class ServerException implements Exception {
  final String message;
  final int? statusCode;
  final String? code;

  ServerException({required this.message, this.statusCode, this.code});

  @override
  String toString() => 'ServerException: $message (Status: $statusCode)';
}

class NetworkException implements Exception {
  final String message;

  NetworkException([this.message = 'No internet connection']);

  @override
  String toString() => 'NetworkException: $message';
}

class ParseException implements Exception {
  final String message;

  ParseException([this.message = 'Failed to parse data']);

  @override
  String toString() => 'ParseException: $message';
}
