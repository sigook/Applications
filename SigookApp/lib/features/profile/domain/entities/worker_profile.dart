import 'package:equatable/equatable.dart';

class WorkerProfile extends Equatable {
  final String id;
  final String? firstName;
  final String? lastName;
  final String? email;
  final String? profilePhoto;
  final String? phoneNumber;
  final String? address;
  final String? city;
  final String? state;
  final String? zipCode;
  final DateTime? dateOfBirth;

  const WorkerProfile({
    required this.id,
    this.firstName,
    this.lastName,
    this.email,
    this.profilePhoto,
    this.phoneNumber,
    this.address,
    this.city,
    this.state,
    this.zipCode,
    this.dateOfBirth,
  });

  String get fullName {
    final first = firstName ?? '';
    final last = lastName ?? '';
    return '$first $last'.trim();
  }

  @override
  List<Object?> get props => [
    id,
    firstName,
    lastName,
    email,
    profilePhoto,
    phoneNumber,
    address,
    city,
    state,
    zipCode,
    dateOfBirth,
  ];
}
