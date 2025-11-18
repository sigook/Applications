import 'package:phone_numbers_parser/phone_numbers_parser.dart';
import '../../domain/entities/value_objects/phone_number.dart' as domain;
import '../../domain/services/phone_validation_service.dart';

/// Implementation of PhoneValidationService using phone_numbers_parser package
/// Follows Single Responsibility Principle - only handles phone validation logic
/// Follows Dependency Inversion Principle - implements domain interface
class PhoneNumberParserValidationService implements PhoneValidationService {
  /// Supported countries for this application
  static const List<String> supportedCountries = ['US', 'CA'];

  @override
  domain.PhoneNumber validate(String phoneNumber, String countryCode) {
    try {
      // Validate country code
      if (!supportedCountries.contains(countryCode.toUpperCase())) {
        return domain.PhoneNumber.invalid(
          phoneNumber,
          'Country code $countryCode is not supported. Supported: ${supportedCountries.join(", ")}',
        );
      }

      // Clean the input
      final cleanNumber = phoneNumber.trim();
      if (cleanNumber.isEmpty) {
        return domain.PhoneNumber.empty();
      }

      // Parse and validate using phone_numbers_parser
      try {
        // Try to parse with country code
        final isoCode = _getIsoCode(countryCode);
        final parsed = PhoneNumber.parse(cleanNumber, callerCountry: isoCode);
        
        // Validate the parsed number
        if (!parsed.isValid(type: PhoneNumberType.mobile)) {
          return domain.PhoneNumber.invalid(
            cleanNumber,
            'Invalid mobile number for $countryCode',
          );
        }

        // Get formatted versions
        final nationalFormat = parsed.formatNsn(); // National format
        final internationalFormat = parsed.international; // International format

        return domain.PhoneNumber.valid(
          value: cleanNumber,
          countryCode: countryCode,
          nationalFormat: nationalFormat,
          internationalFormat: internationalFormat,
        );
      } on PhoneNumberException catch (e) {
        return domain.PhoneNumber.invalid(
          cleanNumber,
          'Invalid phone number: ${e.toString()}',
        );
      }
    } catch (e) {
      return domain.PhoneNumber.invalid(
        phoneNumber,
        'Phone validation error: ${e.toString()}',
      );
    }
  }

  @override
  domain.PhoneNumber parse(String phoneNumber, {String defaultCountryCode = 'US'}) {
    return validate(phoneNumber, defaultCountryCode);
  }

  @override
  bool isValid(String phoneNumber, String countryCode) {
    final result = validate(phoneNumber, countryCode);
    return result.isValid;
  }

  @override
  String? formatNational(String phoneNumber, String countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.nationalFormat : null;
    } catch (e) {
      return null;
    }
  }

  @override
  String? formatInternational(String phoneNumber, String countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.internationalFormat : null;
    } catch (e) {
      return null;
    }
  }

  @override
  String? getE164Format(String phoneNumber, String countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.e164Format : null;
    } catch (e) {
      return null;
    }
  }

  /// Convert country code to ISO code for phone_numbers_parser
  IsoCode _getIsoCode(String countryCode) {
    switch (countryCode.toUpperCase()) {
      case 'US':
        return IsoCode.US;
      case 'CA':
        return IsoCode.CA;
      default:
        return IsoCode.US; // Default to US
    }
  }

  /// Validates a phone number asynchronously with full formatting
  /// This is kept for compatibility but is now synchronous
  Future<domain.PhoneNumber> validateAsync(
    String phoneNumber,
    String countryCode,
  ) async {
    return validate(phoneNumber, countryCode);
  }
}
