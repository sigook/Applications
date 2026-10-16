class IdentificationNumber {
  static const int sinSsnTypeCode = 12;
  static const int sinMinLength = 9;
  static const int sinMaxLength = 15;

  static String? validate(String value, {int? typeCode}) {
    final text = value.trim();
    if (text.isEmpty) return 'Identification number is required';
    if (typeCode == sinSsnTypeCode &&
        (text.length < sinMinLength || text.length > sinMaxLength)) {
      return 'SIN / SSN must be between $sinMinLength and $sinMaxLength characters';
    }
    return null;
  }
}
