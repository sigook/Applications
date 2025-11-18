import 'package:equatable/equatable.dart';

class Language extends Equatable {
  final String? id; // Nullable - may not come from catalog API
  final String value;

  const Language({this.id, required this.value});

  Map<String, dynamic> toJson() => {
    if (id != null) 'id': id,
    'value': value,
  };

  @override
  List<Object?> get props => [id, value];
}
