import '../entities/value_objects/phone_number.dart';

abstract class PhoneValidationService {
  PhoneNumber validate(String phoneNumber, String countryCode);
  PhoneNumber parse(String phoneNumber, {String defaultCountryCode = 'US'});
  bool isValid(String phoneNumber, String countryCode);
  String? formatNational(String phoneNumber, String countryCode);
  String? formatInternational(String phoneNumber, String countryCode);
  String? getE164Format(String phoneNumber, String countryCode);
}
