import 'package:equatable/equatable.dart';

/// Enum representing the weight lifting capacity options
enum LiftingCapacityType {
  light('10 - 20 lbs'),
  medium('20 - 40 lbs'),
  heavy('40 - 60 lbs');

  const LiftingCapacityType(this.label);
  final String label;
}

/// Domain entity for lifting capacity
class LiftingCapacity extends Equatable {
  final LiftingCapacityType capacity;

  const LiftingCapacity({required this.capacity});

  @override
  List<Object?> get props => [capacity];
}
