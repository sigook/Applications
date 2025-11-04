import 'package:equatable/equatable.dart';

/// Email value object with validation
class Email extends Equatable {
  final String value;

  const Email(this.value);

  /// Email validation regex
  static final RegExp _emailRegex = RegExp(
    r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$',
  );

  /// Validates email format
  bool get isValid => _emailRegex.hasMatch(value);

  /// Returns validation error message if invalid
  String? get errorMessage {
    if (value.isEmpty) return 'Email is required';
    if (!isValid) return 'Invalid email format';
    return null;
  }

  @override
  List<Object?> get props => [value];
}
