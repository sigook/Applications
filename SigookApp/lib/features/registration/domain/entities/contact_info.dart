import 'package:equatable/equatable.dart';
import 'value_objects/email.dart';
import 'value_objects/password.dart';

/// Contact information entity
/// Represents the contact section of the registration form
class ContactInfo extends Equatable {
  final Email email;
  final Password password;

  const ContactInfo({required this.email, required this.password});

  /// Validates all fields
  bool get isValid {
    return email.isValid && password.isValid;
  }

  /// Creates a copy with updated fields
  ContactInfo copyWith({Email? email, Password? password}) {
    return ContactInfo(
      email: email ?? this.email,
      password: password ?? this.password,
    );
  }

  @override
  List<Object?> get props => [email, password];
}
