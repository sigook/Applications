import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/catalog_item.dart';

part 'catalog_item_model.g.dart';

/// Data model for catalog items
/// Handles JSON serialization/deserialization
@JsonSerializable()
class CatalogItemModel extends CatalogItem {
  const CatalogItemModel({
    required super.id,
    required super.name,
    super.description,
    super.isActive,
  });

  /// Create from JSON
  factory CatalogItemModel.fromJson(Map<String, dynamic> json) =>
      _$CatalogItemModelFromJson(json);

  /// Convert to JSON
  Map<String, dynamic> toJson() => _$CatalogItemModelToJson(this);

  /// Create from entity
  factory CatalogItemModel.fromEntity(CatalogItem entity) {
    return CatalogItemModel(
      id: entity.id,
      name: entity.name,
      description: entity.description,
      isActive: entity.isActive,
    );
  }

  /// Convert to entity
  CatalogItem toEntity() {
    return CatalogItem(
      id: id,
      name: name,
      description: description,
      isActive: isActive,
    );
  }
}
