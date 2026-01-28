import 'package:phone_numbers_parser/phone_numbers_parser.dart';
import '../../domain/entities/value_objects/phone_number.dart' as domain;
import '../../domain/services/phone_validation_service.dart';

class PhoneNumberParserValidationService implements PhoneValidationService {
  static const List<String> supportedCountries = ['US', 'CA'];

  @override
  domain.PhoneNumber validate(String phoneNumber, String countryCode) {
    try {
      if (!supportedCountries.contains(countryCode.toUpperCase())) {
        return domain.PhoneNumber.invalid(
          phoneNumber,
          'Country code $countryCode is not supported. Supported: ${supportedCountries.join(", ")}',
        );
      }

      final cleanNumber = phoneNumber.trim();
      if (cleanNumber.isEmpty) {
        return domain.PhoneNumber.empty();
      }

      try {
        final isoCode = _getIsoCode(countryCode);
        final parsed = PhoneNumber.parse(cleanNumber, callerCountry: isoCode);

        if (!parsed.isValid(type: PhoneNumberType.mobile)) {
          return domain.PhoneNumber.invalid(
            cleanNumber,
            'Invalid mobile number for $countryCode',
          );
        }

        final nationalFormat = parsed.formatNsn();
        final internationalFormat = parsed.international;

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
  domain.PhoneNumber parse(
    String phoneNumber, {
    String defaultCountryCode = 'US',
  }) {
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

  IsoCode _getIsoCode(String countryCode) {
    switch (countryCode.toUpperCase()) {
      case 'US':
        return IsoCode.US;
      case 'CA':
        return IsoCode.CA;
      default:
        return IsoCode.US;
    }
  }

  Future<domain.PhoneNumber> validateAsync(
    String phoneNumber,
    String countryCode,
  ) async {
    return validate(phoneNumber, countryCode);
  }
}
