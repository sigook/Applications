import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/worker_profile.dart';

part 'worker_profile_model.freezed.dart';
part 'worker_profile_model.g.dart';

@freezed
abstract class WorkerProfileModel with _$WorkerProfileModel {
  const WorkerProfileModel._();

  const factory WorkerProfileModel({
    required String id,
    String? firstName,
    String? lastName,
    String? email,
    String? profilePhoto,
    String? phoneNumber,
    String? address,
    String? city,
    String? state,
    String? zipCode,
    DateTime? dateOfBirth,
  }) = _WorkerProfileModel;

  factory WorkerProfileModel.fromJson(Map<String, dynamic> json) =>
      _$WorkerProfileModelFromJson(json);

  WorkerProfile toEntity() {
    return WorkerProfile(
      id: id,
      firstName: firstName,
      lastName: lastName,
      email: email,
      profilePhoto: profilePhoto,
      phoneNumber: phoneNumber,
      address: address,
      city: city,
      state: state,
      zipCode: zipCode,
      dateOfBirth: dateOfBirth,
    );
  }
}
