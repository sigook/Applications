import '../../../registration/domain/entities/basic_info.dart';
import '../../../registration/domain/entities/country.dart';
import '../../../registration/domain/entities/value_objects/email.dart';
import '../../../registration/domain/entities/value_objects/zip_code.dart';

class ProfileValidators {
  static const int nameMaxLength = 20;
  static const int socialInsuranceMinLength = 9;
  static const int socialInsuranceMaxLength = 15;
  static const int healthProblemMaxLength = 50;
  static const int emergencyNameMinLength = 2;
  static const int emergencyNameMaxLength = 20;
  static const int companyMinLength = 2;
  static const int companyMaxLength = 50;
  static const int supervisorMinLength = 2;
  static const int supervisorMaxLength = 50;
  static const int dutiesMinLength = 2;
  static const int dutiesMaxLength = 5000;

  static final RegExp _phoneDigits = RegExp(r'^\d{10}$');

  static String? requiredLength(String value, String label, int min, int max) {
    final text = value.trim();
    if (text.isEmpty) return '$label is required';
    return _length(text, label, min, max);
  }

  static String? optionalLength(String value, String label, int min, int max) {
    final text = value.trim();
    if (text.isEmpty) return null;
    return _length(text, label, min, max);
  }

  static String? _length(String text, String label, int min, int max) {
    if (text.length < min || text.length > max) {
      return '$label must be between $min and $max characters';
    }
    return null;
  }

  static String? firstName(String value) =>
      requiredLength(value, 'First name', 1, nameMaxLength);

  static String? lastName(String value) =>
      requiredLength(value, 'Last name', 2, nameMaxLength);

  static const int minimumAge = 18;

  static String? birthDay(DateTime? value) {
    if (value == null) return 'Date of birth is required';
    final now = DateTime.now();
    final limit = DateTime(now.year - minimumAge, now.month, now.day);
    if (_dateOnly(value).isAfter(limit)) {
      return 'You must be at least $minimumAge years old';
    }
    return null;
  }

  static String? gender(String? genderId) =>
      genderId == null || genderId.isEmpty ? 'Gender is required' : null;

  static String? optionalName(String value, String label) =>
      optionalLength(value, label, 1, nameMaxLength);

  static String? address(String value) =>
      BasicInfo.validateAddress(value.trim());

  static String? postalCode(String value, String? countryCodeOrName) {
    final countryCode = Country.toIsoCode(countryCodeOrName);
    if (countryCode == null) {
      return value.trim().isEmpty ? 'Postal/ZIP code is required' : null;
    }
    return ZipCode.parse(input: value, countryCode: countryCode)
        .fold((error) => error, (_) => null);
  }

  static String? phone(String value, String label, {bool required = false}) {
    final digits = value.replaceAll(RegExp(r'\D'), '');
    if (digits.isEmpty) return required ? '$label is required' : null;
    if (!_phoneDigits.hasMatch(digits)) return '$label must have 10 digits';
    return null;
  }

  static String? email(String value) => Email(value.trim()).errorMessage;

  static String? socialInsurance(String value) => optionalLength(
        value,
        'SIN / SSN',
        socialInsuranceMinLength,
        socialInsuranceMaxLength,
      );

  static String? sinDueDate({
    required String socialInsurance,
    required bool expires,
    required DateTime? dueDate,
  }) {
    if (socialInsurance.trim().isEmpty || !expires) return null;
    return dueDate == null ? 'Due date is required when the SIN expires' : null;
  }

  static String? healthProblem(String value) =>
      requiredLength(value, 'Health problem', 1, healthProblemMaxLength);

  static String? otherHealthProblem(String value) =>
      optionalLength(value, 'Other allergies', 1, healthProblemMaxLength);

  static String? emergencyName(String value, String label) => optionalLength(
        value,
        label,
        emergencyNameMinLength,
        emergencyNameMaxLength,
      );

  static String? company(String value) =>
      requiredLength(value, 'Company', companyMinLength, companyMaxLength);

  static String? supervisor(String value) => optionalLength(
        value,
        'Supervisor',
        supervisorMinLength,
        supervisorMaxLength,
      );

  static String? duties(String value) =>
      requiredLength(value, 'Duties', dutiesMinLength, dutiesMaxLength);

  static String? startDate(DateTime? value) {
    if (value == null) return 'Start date is required';
    if (_dateOnly(value).isAfter(_dateOnly(DateTime.now()))) {
      return 'Start date cannot be in the future';
    }
    return null;
  }

  static String? endDate({
    required DateTime? start,
    required DateTime? end,
    required bool isCurrent,
  }) {
    if (isCurrent) return null;
    if (end == null) return 'End date is required';
    if (start != null && _dateOnly(end).isBefore(_dateOnly(start))) {
      return 'End date cannot be before the start date';
    }
    return null;
  }

  static DateTime _dateOnly(DateTime d) => DateTime(d.year, d.month, d.day);
}
