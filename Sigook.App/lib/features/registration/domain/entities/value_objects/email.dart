import 'package:equatable/equatable.dart';

class Email extends Equatable {
  final String value;

  const Email(this.value);

  static final RegExp _emailRegex = RegExp(
    r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$',
  );

  static const int minLength = 6;
  static const int maxLength = 100;

  bool get isValid => errorMessage == null;

  String? get errorMessage {
    if (value.isEmpty) return 'Email is required';
    if (value.length < minLength || value.length > maxLength) {
      return 'Email must be between $minLength and $maxLength characters';
    }
    if (!_emailRegex.hasMatch(value)) return 'Invalid email format';
    return null;
  }

  @override
  List<Object?> get props => [value];
}
