import 'package:equatable/equatable.dart';

/// Password value object with validation
class Password extends Equatable {
  final String value;

  const Password(this.value);

  /// Minimum password length
  static const int minLength = 8;

  /// Checks if password meets minimum length
  bool get hasMinLength => value.length >= minLength;

  /// Checks if password contains uppercase letter
  bool get hasUppercase => value.contains(RegExp(r'[A-Z]'));

  /// Checks if password contains lowercase letter
  bool get hasLowercase => value.contains(RegExp(r'[a-z]'));

  /// Checks if password contains number
  bool get hasNumber => value.contains(RegExp(r'[0-9]'));

  /// Checks if password contains special character
  bool get hasSpecialChar => value.contains(RegExp(r'[!@#$%^&*(),.?":{}|<>]'));

  /// Overall password validity
  bool get isValid => hasMinLength && hasUppercase && hasLowercase && hasNumber;

  /// Returns validation error message if invalid
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
