import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/worker_profile.dart';

part 'worker_profile_model.g.dart';

@JsonSerializable()
class WorkerProfileModel extends WorkerProfile {
  const WorkerProfileModel({
    required super.id,
    super.firstName,
    super.lastName,
    super.email,
    super.profilePhoto,
    super.phoneNumber,
    super.address,
    super.city,
    super.state,
    super.zipCode,
    super.dateOfBirth,
  });

  factory WorkerProfileModel.fromJson(Map<String, dynamic> json) =>
      _$WorkerProfileModelFromJson(json);

  Map<String, dynamic> toJson() => _$WorkerProfileModelToJson(this);
}
