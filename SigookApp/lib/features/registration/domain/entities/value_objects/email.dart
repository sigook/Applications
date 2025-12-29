import 'package:equatable/equatable.dart';

class Email extends Equatable {
  final String value;

  const Email(this.value);

  static final RegExp _emailRegex = RegExp(
    r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$',
  );

  bool get isValid => _emailRegex.hasMatch(value);

  String? get errorMessage {
    if (value.isEmpty) return 'Email is required';
    if (!isValid) return 'Invalid email format';
    return null;
  }

  @override
  List<Object?> get props => [value];
}
