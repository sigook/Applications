import 'package:equatable/equatable.dart';
import 'value_objects/email.dart';
import 'value_objects/password.dart';
import 'identification_type.dart';

/// Contact information entity
/// Represents the contact section of the registration form
class ContactInfo extends Equatable {
  final Email email;
  final Password password;
  final String identification;
  final IdentificationType identificationType;
  final String mobileNumber;

  const ContactInfo({
    required this.email,
    required this.password,
    required this.identification,
    required this.identificationType,
    required this.mobileNumber,
  });

  /// Validates all fields
  bool get isValid {
    return email.isValid &&
        password.isValid &&
        identification.isNotEmpty &&
        identificationType.value.isNotEmpty && // Check value instead of nullable id
        mobileNumber.isNotEmpty &&
        mobileNumber.length >= 10; // Basic phone validation
  }

  /// Creates a copy with updated fields
  ContactInfo copyWith({
    Email? email,
    Password? password,
    String? identification,
    IdentificationType? identificationType,
    String? mobileNumber,
  }) {
    return ContactInfo(
      email: email ?? this.email,
      password: password ?? this.password,
      identification: identification ?? this.identification,
      identificationType: identificationType ?? this.identificationType,
      mobileNumber: mobileNumber ?? this.mobileNumber,
    );
  }

  @override
  List<Object?> get props => [
    email,
    password,
    identification,
    identificationType,
    mobileNumber,
  ];
}
