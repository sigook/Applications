/// Utility class for mapping catalog values to their IDs
/// This provides a centralized place for catalog ID mappings
class CatalogIdMapper {
  /// Map day names to their catalog IDs
  /// These IDs should match the backend catalog
  static const Map<String, String> dayNameToId = {
    'Monday': '00000000-0000-0000-0000-000000000001',
    'Tuesday': '00000000-0000-0000-0000-000000000002',
    'Wednesday': '00000000-0000-0000-0000-000000000003',
    'Thursday': '00000000-0000-0000-0000-000000000004',
    'Friday': '00000000-0000-0000-0000-000000000005',
    'Saturday': '00000000-0000-0000-0000-000000000006',
    'Sunday': '00000000-0000-0000-0000-000000000007',
  };

  /// Default/unknown ID for fallback cases
  static const String unknownId = '00000000-0000-0000-0000-000000000000';

  /// Get catalog ID for a day name
  static String getDayId(String dayName) {
    return dayNameToId[dayName] ?? unknownId;
  }

  /// Map time slot values to IDs
  /// TODO: These should ideally be retrieved from catalog API and cached
  /// For now, we use a placeholder approach
  static String getTimeSlotId(String timeValue) {
    // In a real implementation, this would look up from cached catalog data
    // For now, return null to let backend handle it
    return unknownId;
  }

  /// Map availability type to ID
  /// TODO: Should be retrieved from catalog API
  static String getAvailabilityTypeId(String availabilityType) {
    // In a real implementation, this would look up from cached catalog data
    return unknownId;
  }

  /// Validate if a day name is recognized
  static bool isValidDay(String dayName) {
    return dayNameToId.containsKey(dayName);
  }

  /// Get all valid day names
  static List<String> get validDayNames => dayNameToId.keys.toList();

  /// Get all day IDs
  static List<String> get allDayIds => dayNameToId.values.toList();
}
