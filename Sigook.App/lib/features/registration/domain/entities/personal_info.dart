import 'package:equatable/equatable.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/gender.dart';
import 'value_objects/name.dart';

class PersonalInfo extends Equatable {
  final Name firstName;
  final Name lastName;
  final DateTime dateOfBirth;
  final Gender gender;

  const PersonalInfo({
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
    required this.gender,
  });

  bool get isAdult {
    final now = DateTime.now();
    final age = now.year - dateOfBirth.year;
    if (now.month < dateOfBirth.month ||
        (now.month == dateOfBirth.month && now.day < dateOfBirth.day)) {
      return age - 1 >= 18;
    }
    return age >= 18;
  }

  int get age {
    final now = DateTime.now();
    int age = now.year - dateOfBirth.year;
    if (now.month < dateOfBirth.month ||
        (now.month == dateOfBirth.month && now.day < dateOfBirth.day)) {
      age--;
    }
    return age;
  }

  bool get isValid {
    return firstName.isValid &&
        lastName.isValid &&
        isAdult &&
        gender.value.isNotEmpty;
  }

  PersonalInfo copyWith({
    Name? firstName,
    Name? lastName,
    DateTime? dateOfBirth,
    Gender? gender,
  }) {
    return PersonalInfo(
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      gender: gender ?? this.gender,
    );
  }

  @override
  List<Object?> get props => [firstName, lastName, dateOfBirth, gender];
}
