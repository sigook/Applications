import 'package:flutter/foundation.dart';
import '../../domain/entities/catalog_item.dart';

class CatalogItemModel extends CatalogItem {
  const CatalogItemModel({required super.id, required super.value});

  static CatalogItemModel? fromJsonSafe(Map<String, dynamic> json) {
    try {
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

      final rawId =
          json['id'] ??
          json['skillId'] ??
          json['languageId'] ??
          json['genderId'] ??
          json['countryId'];

      final id = rawId?.toString();

      return CatalogItemModel(id: id, value: valueString);
    } catch (e) {
      debugPrint('⚠️ Catalog: Parse error - $e');
      return null;
    }
  }

  factory CatalogItemModel.fromJson(Map<String, dynamic> json) {
    final item = fromJsonSafe(json);
    if (item == null) {
      throw FormatException('Invalid catalog item: $json');
    }
    return item;
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'value': value};
  }

  factory CatalogItemModel.fromEntity(CatalogItem entity) {
    return CatalogItemModel(id: entity.id, value: entity.value);
  }

  CatalogItem toEntity() {
    return CatalogItem(id: id, value: value);
  }
}
