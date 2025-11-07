import 'package:equatable/equatable.dart';

/// Base catalog item entity
/// Represents a generic catalog entry (gender, country, language, etc.)
class CatalogItem extends Equatable {
  final String? id; // Nullable - some catalog items don't have IDs from API
  final String value;

  const CatalogItem({this.id, required this.value});

  @override
  List<Object?> get props => [id, value];

  @override
  String toString() => value;
}
