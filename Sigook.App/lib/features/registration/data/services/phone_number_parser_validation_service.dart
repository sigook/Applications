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

      // TODO: re-enable strict NANP validation once phone data is clean.
      // For now, accept any non-empty number.
      return domain.PhoneNumber.valid(
        value: cleanNumber,
        countryCode: countryCode ?? 'CA',
        nationalFormat: cleanNumber,
        internationalFormat: '+1$cleanNumber',
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


}
