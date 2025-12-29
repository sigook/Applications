import 'package:equatable/equatable.dart';

class Password extends Equatable {
  final String value;

  const Password(this.value);

  static const int minLength = 8;

  bool get hasMinLength => value.length >= minLength;

  bool get hasUppercase => value.contains(RegExp(r'[A-Z]'));

  bool get hasLowercase => value.contains(RegExp(r'[a-z]'));

  bool get hasNumber => value.contains(RegExp(r'[0-9]'));

  bool get hasSpecialChar => value.contains(RegExp(r'[!@#$%^&*(),.?":{}|<>]'));

  bool get isValid => hasMinLength && hasUppercase && hasLowercase && hasNumber;

  String? get errorMessage {
    if (value.isEmpty) return 'Password is required';
    if (!hasMinLength) return 'Password must be at least $minLength characters';
    if (!hasUppercase) return 'Password must contain an uppercase letter';
    if (!hasLowercase) return 'Password must contain a lowercase letter';
    if (!hasNumber) return 'Password must contain a number';
    return null;
  }

  @override
  List<Object?> get props => [value];
}
