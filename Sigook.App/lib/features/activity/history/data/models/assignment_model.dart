import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/assignment.dart';

part 'assignment_model.freezed.dart';
part 'assignment_model.g.dart';

@freezed
abstract class AssignmentModel with _$AssignmentModel {
  const AssignmentModel._();

  const factory AssignmentModel({
    required String id,
    required int numberId,
    String? jobTitle,
    String? clientName,
    String? location,
    String? status,
    DateTime? startAt,
    DateTime? startWorking,
    DateTime? endedAt,
    @Default(0) double hoursWorked,
  }) = _AssignmentModel;

  factory AssignmentModel.fromJson(Map<String, dynamic> json) =>
      _$AssignmentModelFromJson(json);

  Assignment toEntity() => Assignment(
        id: id,
        numberId: numberId,
        jobTitle: jobTitle ?? '',
        clientName: clientName ?? '',
        location: location,
        status: status ?? 'Ended',
        startedAt: startWorking ?? startAt,
        endedAt: endedAt,
        hoursWorked: hoursWorked,
      );
}
