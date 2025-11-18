import 'package:equatable/equatable.dart';

/// Name value object with validation
class Name extends Equatable {
  final String value;

  const Name(this.value);

  /// Minimum name length
  static const int minLength = 2;

  /// Maximum name length
  static const int maxLength = 50;

  /// Validates name format (only letters and spaces)
  bool get isValid {
    if (value.isEmpty) return false;
    if (value.length < minLength || value.length > maxLength) return false;
    return RegExp(r'^[a-zA-Z\s]+$').hasMatch(value);
  }

  /// Returns validation error message if invalid
  String? get errorMessage {
    if (value.isEmpty) return 'Name is required';
    if (value.length < minLength) {
      return 'Name must be at least $minLength characters';
    }
    if (value.length > maxLength) {
      return 'Name must be less than $maxLength characters';
    }
    if (!RegExp(r'^[a-zA-Z\s]+$').hasMatch(value)) {
      return 'Name can only contain letters and spaces';
    }
    return null;
  }

  @override
  List<Object?> get props => [value];
}
