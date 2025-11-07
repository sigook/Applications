import 'package:freezed_annotation/freezed_annotation.dart';
import '../../../../core/constants/enums.dart' as enums;
import '../../domain/entities/availability_info.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/availability_type.dart';

part 'availability_info_model.freezed.dart';
part 'availability_info_model.g.dart';

@freezed
class AvailabilityInfoModel with _$AvailabilityInfoModel {
  const AvailabilityInfoModel._();

  const factory AvailabilityInfoModel({
    String? availabilityTypeId, // UUID from API (may be null)
    required String availabilityTypeName, // Name from API
    required Map<String, String> availableTimes, // {id: value} - only times with IDs
    required List<String> availableDays, // Stored as strings
  }) = _AvailabilityInfoModel;

  /// Convert from domain entity
  factory AvailabilityInfoModel.fromEntity(AvailabilityInfo entity) {
    return AvailabilityInfoModel(
      availabilityTypeId: entity.availabilityType.id, // May be null
      availabilityTypeName: entity.availabilityType.value,
      // Only include times that have IDs
      availableTimes: {for (var time in entity.availableTimes.where((t) => t.id != null)) time.id!: time.value},
      availableDays: entity.availableDays.map((d) => d.toString().split('.').last).toList(),
    );
  }

  /// Convert to domain entity
  AvailabilityInfo toEntity() {
    // Convert string day names to enum
    final dayEnums = availableDays.map((dayStr) {
      switch (dayStr.toLowerCase()) {
        case 'monday':
          return enums.DayOfWeek.monday;
        case 'tuesday':
          return enums.DayOfWeek.tuesday;
        case 'wednesday':
          return enums.DayOfWeek.wednesday;
        case 'thursday':
          return enums.DayOfWeek.thursday;
        case 'friday':
          return enums.DayOfWeek.friday;
        case 'saturday':
          return enums.DayOfWeek.saturday;
        case 'sunday':
          return enums.DayOfWeek.sunday;
        default:
          return enums.DayOfWeek.monday; // fallback
      }
    }).toList();

    return AvailabilityInfo(
      availabilityType: AvailabilityType(
        id: availabilityTypeId,
        value: availabilityTypeName,
      ),
      availableTimes: availableTimes.entries
          .map((e) => AvailableTime(id: e.key, value: e.value))
          .toList(),
      availableDays: dayEnums,
    );
  }

  /// From JSON
  factory AvailabilityInfoModel.fromJson(Map<String, dynamic> json) =>
      _$AvailabilityInfoModelFromJson(json);
}
