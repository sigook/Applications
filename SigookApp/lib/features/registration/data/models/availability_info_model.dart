import 'package:freezed_annotation/freezed_annotation.dart';
import '../../../../core/constants/enums.dart';
import '../../domain/entities/availability_info.dart';

part 'availability_info_model.freezed.dart';
part 'availability_info_model.g.dart';

@freezed
class AvailabilityInfoModel with _$AvailabilityInfoModel {
  const AvailabilityInfoModel._();

  const factory AvailabilityInfoModel({
    required AvailabilityType availabilityType,
    required List<TimeOfDay> availableTimes,
    required List<DayOfWeek> availableDays,
  }) = _AvailabilityInfoModel;

  /// Convert from domain entity
  factory AvailabilityInfoModel.fromEntity(AvailabilityInfo entity) {
    return AvailabilityInfoModel(
      availabilityType: entity.availabilityType,
      availableTimes: entity.availableTimes,
      availableDays: entity.availableDays,
    );
  }

  /// Convert to domain entity
  AvailabilityInfo toEntity() {
    return AvailabilityInfo(
      availabilityType: availabilityType,
      availableTimes: availableTimes,
      availableDays: availableDays,
    );
  }

  /// From JSON
  factory AvailabilityInfoModel.fromJson(Map<String, dynamic> json) =>
      _$AvailabilityInfoModelFromJson(json);
}
