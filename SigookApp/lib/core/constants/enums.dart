enum Gender {
  male,
  female,
  other,
  preferNotToSay;

  String get displayName {
    switch (this) {
      case Gender.male:
        return 'Male';
      case Gender.female:
        return 'Female';
      case Gender.other:
        return 'Other';
      case Gender.preferNotToSay:
        return 'Prefer not to say';
    }
  }
}

enum AvailabilityType {
  fullTime,
  partTime;

  String get displayName {
    switch (this) {
      case AvailabilityType.fullTime:
        return 'Full Time';
      case AvailabilityType.partTime:
        return 'Part Time';
    }
  }
}

enum TimeOfDay {
  morning,
  afternoon,
  evening;

  String get displayName {
    switch (this) {
      case TimeOfDay.morning:
        return 'Morning';
      case TimeOfDay.afternoon:
        return 'Afternoon';
      case TimeOfDay.evening:
        return 'Evening';
    }
  }
}

enum DayOfWeek {
  monday,
  tuesday,
  wednesday,
  thursday,
  friday,
  saturday,
  sunday;

  String get displayName {
    switch (this) {
      case DayOfWeek.monday:
        return 'Monday';
      case DayOfWeek.tuesday:
        return 'Tuesday';
      case DayOfWeek.wednesday:
        return 'Wednesday';
      case DayOfWeek.thursday:
        return 'Thursday';
      case DayOfWeek.friday:
        return 'Friday';
      case DayOfWeek.saturday:
        return 'Saturday';
      case DayOfWeek.sunday:
        return 'Sunday';
    }
  }
}

enum ClockType { none, clockIn, clockOut }
