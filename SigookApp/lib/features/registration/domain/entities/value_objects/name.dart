import 'package:equatable/equatable.dart';

class Name extends Equatable {
  final String value;
  final int minLength;

  const Name(this.value, {this.minLength = 2});

  static const int maxLength = 20;

  static final RegExp _allowed = RegExp(r"^[\p{L}\s\-']+$", unicode: true);

  bool get isValid => errorMessage == null;

  String? get errorMessage {
    if (value.isEmpty) return 'Name is required';
    if (value.length < minLength) {
      return 'Name must be at least $minLength characters';
    }
    if (value.length > maxLength) {
      return 'Name must be at most $maxLength characters';
    }
    if (!_allowed.hasMatch(value)) {
      return 'Name can only contain letters, spaces, hyphens, or apostrophes';
    }
    return null;
  }

  @override
  List<Object?> get props => [value];
}
