import 'package:equatable/equatable.dart';

class IdentificationType extends Equatable {
  final String? id;
  final String value;

  const IdentificationType({this.id, required this.value});

  Map<String, dynamic> toJson() => {
    if (id != null) 'id': id,
    'value': value,
  };

  @override
  List<Object?> get props => [id, value];
}
