import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/constants/error_messages.dart';

void main() {
  group('ErrorMessages', () {
    test('fromException returns appropriate message for network errors', () {
      final networkError = Exception('network error occurred');
      final message = ErrorMessages.fromException(networkError);

      expect(message.toLowerCase(), contains('network'));
    });

    test('fromException returns appropriate message for timeout errors', () {
      final timeoutError = Exception('connection timeout');
      final message = ErrorMessages.fromException(timeoutError);

      expect(message.toLowerCase(), contains('timeout'));
    });

    test('fromException returns appropriate message for auth errors', () {
      final authError = Exception('unauthorized access');
      final message = ErrorMessages.fromException(authError);

      expect(
        message.toLowerCase().contains('session') ||
            message.toLowerCase().contains('sign in'),
        true,
      );
    });

    test('fromException returns appropriate message for forbidden errors', () {
      final forbiddenError = Exception('403 forbidden');
      final message = ErrorMessages.fromException(forbiddenError);

      expect(message.toLowerCase(), contains('permission'));
    });

    test('fromException returns appropriate message for not found errors', () {
      final notFoundError = Exception('404 not found');
      final message = ErrorMessages.fromException(notFoundError);

      expect(
        message.toLowerCase().contains('not found') ||
            message.toLowerCase().contains('unavailable'),
        true,
      );
    });

    test('fromException returns appropriate message for server errors', () {
      final serverError = Exception('500 internal server error');
      final message = ErrorMessages.fromException(serverError);

      expect(message.toLowerCase(), contains('server'));
    });

    test('fromException returns generic message for unknown errors', () {
      final unknownError = Exception('some random error');
      final message = ErrorMessages.fromException(unknownError);

      expect(message, isNotEmpty);
      expect(
        message.toLowerCase().contains('try again') ||
            message.toLowerCase().contains('error'),
        true,
      );
    });

    test('fromException handles null exception gracefully', () {
      final message = ErrorMessages.fromException(Exception('null'));

      expect(message, isNotEmpty);
    });
  });
}
