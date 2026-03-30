import '../entities/value_objects/phone_number.dart';

abstract class PhoneValidationService {
  /// Validates [phoneNumber] against [countryCode] (ISO-2, e.g. 'CA', 'US').
  /// When [countryCode] is null the service auto-detects the country.
  PhoneNumber validate(String phoneNumber, String? countryCode);
  PhoneNumber parse(String phoneNumber, {String? defaultCountryCode});
  bool isValid(String phoneNumber, String? countryCode);
  String? formatNational(String phoneNumber, String? countryCode);
  String? formatInternational(String phoneNumber, String? countryCode);
  String? getE164Format(String phoneNumber, String? countryCode);
}
