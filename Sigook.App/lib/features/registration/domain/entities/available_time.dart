import 'package:equatable/equatable.dart';

class AvailableTime extends Equatable {
  final String? id;
  final String value;

  const AvailableTime({this.id, required this.value});

  Map<String, dynamic> toJson() => {
    if (id != null) 'id': id,
    'value': value,
  };

  @override
  List<Object?> get props => [id, value];
}
