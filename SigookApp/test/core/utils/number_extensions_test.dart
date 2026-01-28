import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/number_extensions.dart';

void main() {
  group('NumberExtensions', () {
    group('Currency Formatting', () {
      test('toCurrency formats as USD by default', () {
        expect(1234.56.toCurrency(), '\$1,234.56');
        expect(0.0.toCurrency(), '\$0.00');
        expect(1000000.0.toCurrency(), '\$1,000,000.00');
      });

      test('toCurrency respects custom symbol', () {
        expect(1234.56.toCurrency(symbol: '€'), '€1,234.56');
        expect(1234.56.toCurrency(symbol: '£'), '£1,234.56');
      });

      test('toCurrency respects decimal places', () {
        expect(1234.5678.toCurrency(decimalDigits: 0), '\$1,235');
        expect(1234.5678.toCurrency(decimalDigits: 3), '\$1,234.568');
      });
    });

    group('Number Formatting', () {
      test('toFormattedString adds thousand separators', () {
        expect(1234.toFormattedString(), '1,234');
        expect(1234567.toFormattedString(), '1,234,567');
        expect(0.toFormattedString(), '0');
      });

      test('toPercentage formats as percentage', () {
        expect(0.75.toPercentage(), '75%');
        expect(0.5.toPercentage(), '50%');
        expect(1.0.toPercentage(), '100%');
        expect(0.123.toPercentage(decimalDigits: 1), '12.3%');
      });
    });

    group('Range Checks', () {
      test('isBetween checks if number is in range', () {
        expect(5.isBetween(1, 10), true);
        expect(1.isBetween(1, 10), true);
        expect(10.isBetween(1, 10), true);
        expect(0.isBetween(1, 10), false);
        expect(11.isBetween(1, 10), false);
      });

      test('clamp restricts value to range', () {
        expect(5.clamp(1, 10), 5);
        expect(0.clamp(1, 10), 1);
        expect(15.clamp(1, 10), 10);
        expect((-5).clamp(1, 10), 1);
      });
    });

    group('Rounding', () {
      test('roundToDecimal rounds to specified decimals', () {
        expect(1.2345.roundToDecimal(2), 1.23);
        expect(1.2356.roundToDecimal(2), 1.24);
        expect(1.2345.roundToDecimal(0), 1.0);
        expect(1.2345.roundToDecimal(3), 1.235);
      });
    });

    group('Abbreviations', () {
      test('toAbbreviated formats large numbers', () {
        expect(500.toAbbreviated(), '500');
        expect(1500.toAbbreviated(), '1.5K');
        expect(1000000.toAbbreviated(), '1.0M');
        expect(1500000.toAbbreviated(), '1.5M');
        expect(1000000000.toAbbreviated(), '1.0B');
      });
    });
  });
}
