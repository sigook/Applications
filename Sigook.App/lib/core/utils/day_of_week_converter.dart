import 'package:freezed_annotation/freezed_annotation.dart';
import '../constants/enums.dart';

class DayOfWeekConverter implements JsonConverter<DayOfWeek, String> {
  const DayOfWeekConverter();

  @override
  DayOfWeek fromJson(String json) {
    switch (json.toLowerCase()) {
      case 'monday':
        return DayOfWeek.monday;
      case 'tuesday':
        return DayOfWeek.tuesday;
      case 'wednesday':
        return DayOfWeek.wednesday;
      case 'thursday':
        return DayOfWeek.thursday;
      case 'friday':
        return DayOfWeek.friday;
      case 'saturday':
        return DayOfWeek.saturday;
      case 'sunday':
        return DayOfWeek.sunday;
      default:
        throw ArgumentError('Unknown day of week: $json');
    }
  }

  @override
  String toJson(DayOfWeek object) {
    return object.toString().split('.').last;
  }
}
