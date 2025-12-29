import 'package:equatable/equatable.dart';

class CatalogItem extends Equatable {
  final String? id;
  final String value;

  const CatalogItem({this.id, required this.value});

  @override
  List<Object?> get props => [id, value];

  @override
  String toString() => value;
}
