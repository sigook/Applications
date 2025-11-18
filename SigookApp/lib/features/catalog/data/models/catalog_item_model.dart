import 'package:flutter/foundation.dart';
import '../../domain/entities/catalog_item.dart';

/// Data model for catalog items
/// Handles JSON serialization/deserialization
class CatalogItemModel extends CatalogItem {
  const CatalogItemModel({required super.id, required super.value});

  /// Create from JSON
  /// Returns null if the item is invalid (missing required fields)
  static CatalogItemModel? fromJsonSafe(Map<String, dynamic> json) {
    try {
      // Safely parse value (required field)
      // API returns different field names: 'value', 'skill', 'language', 'country', etc.
      final rawValue =
          json['value'] ??
          json['skill'] ??
          json['language'] ??
          json['country'] ??
          json['gender'] ??
          json['availability'] ??
          json['availabilityTime'] ??
          json['identificationType'];

      if (rawValue == null || rawValue.toString().trim().isEmpty) {
        debugPrint('⚠️ Catalog: Skipping item with null/empty value');
        return null;
      }

      final valueString = rawValue.toString().trim();

      // Get ID - try multiple field names
      final rawId = json['id'] ?? 
                    json['skillId'] ?? 
                    json['languageId'] ??
                    json['genderId'] ??
                    json['countryId'];
      
      final id = rawId?.toString(); // Nullable - don't generate fake IDs

      return CatalogItemModel(
        id: id,
        value: valueString,
      );
    } catch (e) {
      debugPrint('⚠️ Catalog: Parse error - $e');
      return null;
    }
  }

  /// Create from JSON (throws on error - use fromJsonSafe for resilient parsing)
  factory CatalogItemModel.fromJson(Map<String, dynamic> json) {
    final item = fromJsonSafe(json);
    if (item == null) {
      throw FormatException('Invalid catalog item: $json');
    }
    return item;
  }

  /// Convert to JSON
  Map<String, dynamic> toJson() {
    return {'id': id, 'value': value};
  }

  /// Create from entity
  factory CatalogItemModel.fromEntity(CatalogItem entity) {
    return CatalogItemModel(id: entity.id, value: entity.value);
  }

  /// Convert to entity
  CatalogItem toEntity() {
    return CatalogItem(id: id, value: value);
  }
}
