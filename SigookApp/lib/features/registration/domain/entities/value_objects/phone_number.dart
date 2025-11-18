import 'package:equatable/equatable.dart';

/// Value object representing a validated phone number
/// Follows Single Responsibility Principle - only handles phone number representation
class PhoneNumber extends Equatable {
  final String value;
  final String? countryCode; // ISO country code (e.g., 'US', 'CA')
  final String? internationalFormat;
  final String? nationalFormat;
  final String? errorMessage;

  const PhoneNumber({
    required this.value,
    this.countryCode,
    this.internationalFormat,
    this.nationalFormat,
    this.errorMessage,
  });

  /// Factory for creating an empty/initial phone number
  factory PhoneNumber.empty() {
    return const PhoneNumber(value: '');
  }

  /// Factory for creating an invalid phone number with error
  factory PhoneNumber.invalid(String value, String error) {
    return PhoneNumber(
      value: value,
      errorMessage: error,
    );
  }

  /// Factory for creating a valid phone number
  factory PhoneNumber.valid({
    required String value,
    required String countryCode,
    required String internationalFormat,
    required String nationalFormat,
  }) {
    return PhoneNumber(
      value: value,
      countryCode: countryCode,
      internationalFormat: internationalFormat,
      nationalFormat: nationalFormat,
    );
  }

  bool get isValid => errorMessage == null && value.isNotEmpty;
  bool get isEmpty => value.isEmpty;

  /// Get the formatted phone number for display
  /// Prefers national format, falls back to international, then raw value
  String get displayFormat {
    if (nationalFormat != null && nationalFormat!.isNotEmpty) {
      return nationalFormat!;
    }
    if (internationalFormat != null && internationalFormat!.isNotEmpty) {
      return internationalFormat!;
    }
    return value;
  }

  /// Get E164 format for API submission (e.g., +15551234567)
  String get e164Format {
    return internationalFormat?.replaceAll(RegExp(r'[^\d+]'), '') ?? value;
  }

  @override
  List<Object?> get props => [
        value,
        countryCode,
        internationalFormat,
        nationalFormat,
        errorMessage,
      ];

  @override
  String toString() => displayFormat;
}
