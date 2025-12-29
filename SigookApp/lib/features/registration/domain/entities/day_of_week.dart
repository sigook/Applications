import 'package:equatable/equatable.dart';

class DayOfWeekEntity extends Equatable {
  final String id;
  final String value;

  const DayOfWeekEntity({required this.id, required this.value});

  Map<String, dynamic> toJson() => {'id': id, 'value': value};

  @override
  List<Object?> get props => [id, value];
}
