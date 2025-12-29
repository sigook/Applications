import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/string_extensions.dart';

void main() {
  group('StringExtensions', () {
    group('Validation', () {
      test('isValidEmail validates email addresses correctly', () {
        expect('test@example.com'.isValidEmail, true);
        expect('user.name+tag@example.co.uk'.isValidEmail, true);
        expect('invalid.email'.isValidEmail, false);
        expect('missing@domain'.isValidEmail, false);
        expect('@example.com'.isValidEmail, false);
        expect(''.isValidEmail, false);
      });

      test('isValidPhone validates phone numbers correctly', () {
        expect('1234567890'.isValidPhone, true);
        expect('123-456-7890'.isValidPhone, true);
        expect('(123) 456-7890'.isValidPhone, true);
        expect('+1 (123) 456-7890'.isValidPhone, true);
        expect('12345'.isValidPhone, false);
        expect('abc'.isValidPhone, false);
        expect(''.isValidPhone, false);
      });
    });

    group('Formatting', () {
      test('capitalize capitalizes first letter', () {
        expect('hello'.capitalize(), 'Hello');
        expect('HELLO'.capitalize(), 'HELLO');
        expect('h'.capitalize(), 'H');
        expect(''.capitalize(), '');
      });

      test('toTitleCase converts to title case', () {
        expect('hello world'.toTitleCase(), 'Hello World');
        expect('HELLO WORLD'.toTitleCase(), 'Hello World');
        expect('hELLO wORLD'.toTitleCase(), 'Hello World');
        expect(''.toTitleCase(), '');
      });

      test('truncate truncates long strings', () {
        const longString = 'This is a very long string that needs truncation';

        expect(longString.truncate(20), 'This is a very lo...');
        expect(longString.truncate(20, ellipsis: '…'), 'This is a very long…');
        expect('Short'.truncate(20), 'Short');
        expect(''.truncate(20), '');
      });

      test('removeWhitespace removes all whitespace', () {
        expect('hello world'.removeWhitespace(), 'helloworld');
        expect('  hello  world  '.removeWhitespace(), 'helloworld');
        expect('hello\nworld\t'.removeWhitespace(), 'helloworld');
        expect(''.removeWhitespace(), '');
      });
    });

    group('Checks', () {
      test('isNullOrEmpty checks for null or empty strings', () {
        expect(''.isNullOrEmpty, true);
        expect('  '.isNullOrEmpty, false);
        expect('hello'.isNullOrEmpty, false);
      });

      test('isNullOrBlank checks for null, empty, or whitespace', () {
        expect(''.isNullOrBlank, true);
        expect('  '.isNullOrBlank, true);
        expect('\t\n'.isNullOrBlank, true);
        expect('hello'.isNullOrBlank, false);
        expect(' hello '.isNullOrBlank, false);
      });

      test('hasMatch checks for pattern matches', () {
        expect('hello123'.hasMatch(r'\d+'), true);
        expect('hello'.hasMatch(r'\d+'), false);
        expect('test@example.com'.hasMatch(r'@'), true);
      });
    });

    group('Parsing', () {
      test('toInt parses integers correctly', () {
        expect('123'.toInt(), 123);
        expect('-456'.toInt(), -456);
        expect('0'.toInt(), 0);
        expect('abc'.toInt(), null);
        expect(''.toInt(), null);
        expect('12.5'.toInt(), null);
      });

      test('toDouble parses doubles correctly', () {
        expect('123.45'.toDouble(), 123.45);
        expect('-456.78'.toDouble(), -456.78);
        expect('0'.toDouble(), 0.0);
        expect('abc'.toDouble(), null);
        expect(''.toDouble(), null);
      });
    });
  });
}
