import 'package:equatable/equatable.dart';

class Language extends Equatable {
  final String? id;
  final String value;

  const Language({this.id, required this.value});

  Map<String, dynamic> toJson() => {'id': id, 'value': value};

  @override
  List<Object?> get props => [id, value];
}
