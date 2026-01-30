import 'package:equatable/equatable.dart';

class Skill extends Equatable {
  final String skill;

  const Skill({required this.skill});

  String get value => skill;

  Map<String, dynamic> toJson() => {'skill': skill};

  @override
  List<Object?> get props => [skill];
}
