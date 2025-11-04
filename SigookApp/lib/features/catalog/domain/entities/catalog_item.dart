import 'package:equatable/equatable.dart';

/// Base catalog item entity
/// Represents a generic catalog entry (gender, country, language, etc.)
class CatalogItem extends Equatable {
  final int id;
  final String name;
  final String? description;
  final bool isActive;

  const CatalogItem({
    required this.id,
    required this.name,
    this.description,
    this.isActive = true,
  });

  @override
  List<Object?> get props => [id, name, description, isActive];

  @override
  String toString() => name;
}
