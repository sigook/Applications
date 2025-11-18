import '../entities/value_objects/phone_number.dart';

/// Interface for phone number validation service
/// Follows Interface Segregation Principle - focused contract for phone validation
/// Follows Dependency Inversion Principle - domain depends on abstraction, not implementation
abstract class PhoneValidationService {
  /// Validates and formats a phone number for a specific country
  /// 
  /// [phoneNumber] - The raw phone number string to validate
  /// [countryCode] - ISO 2-letter country code (e.g., 'US', 'CA')
  /// 
  /// Returns a [PhoneNumber] value object with validation results and formatting
  PhoneNumber validate(String phoneNumber, String countryCode);

  /// Parses a phone number with automatic country detection
  /// 
  /// [phoneNumber] - Phone number string (should include country code like +1)
  /// [defaultCountryCode] - Fallback country code if detection fails
  /// 
  /// Returns a [PhoneNumber] value object with validation results
  PhoneNumber parse(String phoneNumber, {String defaultCountryCode = 'US'});

  /// Checks if a phone number is valid for a specific country
  /// 
  /// [phoneNumber] - The phone number string to check
  /// [countryCode] - ISO 2-letter country code
  /// 
  /// Returns true if the phone number is valid
  bool isValid(String phoneNumber, String countryCode);

  /// Formats a phone number to national format (e.g., (555) 123-4567)
  /// 
  /// [phoneNumber] - The phone number to format
  /// [countryCode] - ISO 2-letter country code
  /// 
  /// Returns formatted phone number or null if invalid
  String? formatNational(String phoneNumber, String countryCode);

  /// Formats a phone number to international format (e.g., +1 555-123-4567)
  /// 
  /// [phoneNumber] - The phone number to format
  /// [countryCode] - ISO 2-letter country code
  /// 
  /// Returns formatted phone number or null if invalid
  String? formatInternational(String phoneNumber, String countryCode);

  /// Gets the E.164 format of a phone number (e.g., +15551234567)
  /// This is the standard format for API submission
  /// 
  /// [phoneNumber] - The phone number to format
  /// [countryCode] - ISO 2-letter country code
  /// 
  /// Returns E.164 formatted phone number or null if invalid
  String? getE164Format(String phoneNumber, String countryCode);
}
