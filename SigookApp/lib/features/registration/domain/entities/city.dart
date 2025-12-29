import 'package:equatable/equatable.dart';
import 'province.dart';

class City extends Equatable {
  final String? id;
  final String value;
  final String? code;
  final Province province;

  const City({this.id, required this.value, this.code, required this.province});

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'value': value,
      'code': code,
      'province': province.toJson(),
    };
  }

  @override
  List<Object?> get props => [id, value, code, province];
}
