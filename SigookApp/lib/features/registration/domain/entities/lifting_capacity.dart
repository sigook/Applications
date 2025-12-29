import 'package:equatable/equatable.dart';

enum LiftingCapacityType {
  light('10 - 20 lbs'),
  medium('20 - 40 lbs'),
  heavy('40 - 60 lbs');

  const LiftingCapacityType(this.label);
  final String label;
}

class LiftingCapacity extends Equatable {
  final String? id;
  final String value;

  const LiftingCapacity({this.id, required this.value});

  Map<String, dynamic> toJson() => {'id': id, 'value': value};

  @override
  List<Object?> get props => [id, value];
}
