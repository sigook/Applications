import 'package:equatable/equatable.dart';

/// Country entity for location
class Country extends Equatable {
  final String? id;
  final String value;
  final String? code;

  const Country({
    this.id,
    required this.value,
    this.code,
  });

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'value': value,
      'code': code,
    };
  }

  @override
  List<Object?> get props => [id, value, code];
}
