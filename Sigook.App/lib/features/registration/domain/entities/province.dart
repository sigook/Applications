import 'package:equatable/equatable.dart';
import 'country.dart';

class Province extends Equatable {
  final String? id;
  final String value;
  final String? code;
  final Country country;

  const Province({
    this.id,
    required this.value,
    this.code,
    required this.country,
  });

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'value': value,
      'code': code,
      'country': country.toJson(),
    };
  }

  @override
  List<Object?> get props => [id, value, code, country];
}
