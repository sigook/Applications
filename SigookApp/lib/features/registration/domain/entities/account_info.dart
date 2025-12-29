import 'package:equatable/equatable.dart';
import 'value_objects/email.dart';
import 'value_objects/password.dart';

class AccountInfo extends Equatable {
  final Email email;
  final Password password;
  final String confirmPassword;
  final bool termsAccepted;

  const AccountInfo({
    required this.email,
    required this.password,
    required this.confirmPassword,
    required this.termsAccepted,
  });

  bool get isValid {
    return email.isValid &&
        password.isValid &&
        confirmPasswordError == null &&
        termsAccepted;
  }

  String? get confirmPasswordError {
    if (confirmPassword.isEmpty) return 'Please confirm your password';
    if (confirmPassword != password.value) return 'Passwords do not match';
    return null;
  }

  String? get termsError =>
      !termsAccepted ? 'You must accept the terms and conditions' : null;

  AccountInfo copyWith({
    Email? email,
    Password? password,
    String? confirmPassword,
    bool? termsAccepted,
  }) {
    return AccountInfo(
      email: email ?? this.email,
      password: password ?? this.password,
      confirmPassword: confirmPassword ?? this.confirmPassword,
      termsAccepted: termsAccepted ?? this.termsAccepted,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'email': email.value,
      'password': password.value,
      'confirmPassword': confirmPassword,
      'termsAccepted': termsAccepted,
    };
  }

  @override
  List<Object?> get props => [email, password, confirmPassword, termsAccepted];
}
