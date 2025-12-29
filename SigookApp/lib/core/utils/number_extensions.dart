import 'dart:math';
import 'package:intl/intl.dart';

extension DoubleExtensions on double {
  String toCurrency({String symbol = '\$', int decimalDigits = 2}) {
    if (decimalDigits == 0) {
      final formatter = NumberFormat('#,##0');
      return '$symbol${formatter.format(this)}';
    }
    final formatter = NumberFormat('#,##0.${'0' * decimalDigits}');
    return '$symbol${formatter.format(this)}';
  }

  String toFormattedNumber() {
    final formatter = NumberFormat('#,##0.00');
    return formatter.format(this);
  }

  String toPercentage({int decimalDigits = 0}) {
    final percentage = this * 100;
    return '${percentage.toStringAsFixed(decimalDigits)}%';
  }

  double roundToDecimal(int places) {
    final mod = pow(10.0, places);
    return (this * mod).round() / mod;
  }

  bool isBetween(num min, num max) {
    return this >= min && this <= max;
  }
}

extension IntExtensions on int {
  String toCurrency({String symbol = '\$', int decimalDigits = 2}) {
    if (decimalDigits == 0) {
      final formatter = NumberFormat('#,##0');
      return '$symbol${formatter.format(this)}';
    }
    final formatter = NumberFormat('#,##0.${'0' * decimalDigits}');
    return '$symbol${formatter.format(this)}';
  }

  String toFormattedString() {
    final formatter = NumberFormat('#,##0');
    return formatter.format(this);
  }

  String toFormattedNumber() {
    final formatter = NumberFormat('#,##0');
    return formatter.format(this);
  }

  String toAbbreviated() {
    if (this < 1000) return toString();
    if (this < 1000000) return '${(this / 1000).toStringAsFixed(1)}K';
    if (this < 1000000000) return '${(this / 1000000).toStringAsFixed(1)}M';
    return '${(this / 1000000000).toStringAsFixed(1)}B';
  }

  bool isBetween(int min, int max) {
    return this >= min && this <= max;
  }

  bool get isEven => this % 2 == 0;

  bool get isOdd => this % 2 != 0;

  bool get isPositive => this > 0;

  bool get isNegative => this < 0;
}
