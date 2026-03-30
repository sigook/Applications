import 'package:phone_numbers_parser/phone_numbers_parser.dart';
import '../../domain/entities/value_objects/phone_number.dart' as domain;
import '../../domain/services/phone_validation_service.dart';

class PhoneNumberParserValidationService implements PhoneValidationService {
  static const List<String> _nanpCodes = ['CA', 'US'];

  /// Validates [phoneNumber] for [countryCode].
  ///
  /// For NANP countries (CA/US, which share the +1 prefix) both codes are
  /// tried — a number is accepted if it is a valid mobile number for either.
  /// When [countryCode] is null the service auto-detects between CA and US.
  /// If the number matches neither country it is rejected.
  @override
  domain.PhoneNumber validate(String phoneNumber, String? countryCode) {
    try {
      final upper = countryCode?.toUpperCase();

      if (upper != null && !_nanpCodes.contains(upper)) {
        return domain.PhoneNumber.invalid(
          phoneNumber,
          'Country $upper is not supported. Supported: ${_nanpCodes.join(", ")}',
        );
      }

      final cleanNumber = phoneNumber.trim();
      if (cleanNumber.isEmpty) {
        return domain.PhoneNumber.empty();
      }

      // When countryCode is null, try CA first then US (auto-detect).
      // When it is CA or US, still try both since they share +1 (NANP).
      final codesToTry = upper == 'US'
          ? ['US', 'CA']
          : ['CA', 'US']; // CA-first for null or explicit CA

      for (final code in codesToTry) {
        try {
          final parsed = PhoneNumber.parse(
            cleanNumber,
            callerCountry: _toIsoCode(code),
          );
          if (parsed.isValid(type: PhoneNumberType.mobile)) {
            return domain.PhoneNumber.valid(
              value: cleanNumber,
              countryCode: code, // resolved country, not the raw input
              nationalFormat: parsed.formatNsn(),
              internationalFormat: parsed.international,
            );
          }
        } on PhoneNumberException {
          continue;
        }
      }

      return domain.PhoneNumber.invalid(
        cleanNumber,
        'Invalid mobile number — must be a valid Canadian or US number',
      );
    } catch (e) {
      return domain.PhoneNumber.invalid(
        phoneNumber,
        'Phone validation error: ${e.toString()}',
      );
    }
  }

  @override
  domain.PhoneNumber parse(String phoneNumber, {String? defaultCountryCode}) {
    return validate(phoneNumber, defaultCountryCode);
  }

  @override
  bool isValid(String phoneNumber, String? countryCode) {
    return validate(phoneNumber, countryCode).isValid;
  }

  @override
  String? formatNational(String phoneNumber, String? countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.nationalFormat : null;
    } catch (e) {
      return null;
    }
  }

  @override
  String? formatInternational(String phoneNumber, String? countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.internationalFormat : null;
    } catch (e) {
      return null;
    }
  }

  @override
  String? getE164Format(String phoneNumber, String? countryCode) {
    try {
      final result = validate(phoneNumber, countryCode);
      return result.isValid ? result.e164Format : null;
    } catch (e) {
      return null;
    }
  }

  IsoCode _toIsoCode(String code) {
    switch (code) {
      case 'US':
        return IsoCode.US;
      case 'CA':
      default:
        return IsoCode.CA;
    }
  }
}
