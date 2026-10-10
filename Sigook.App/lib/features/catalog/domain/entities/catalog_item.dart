import 'package:equatable/equatable.dart';

class CatalogItem extends Equatable {
  final String? id;
  final String value;
  final int? code;

  const CatalogItem({this.id, required this.value, this.code});

  @override
  List<Object?> get props => [id, value, code];

  @override
  String toString() => value;
}
