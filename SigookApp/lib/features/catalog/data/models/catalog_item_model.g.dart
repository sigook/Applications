// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_item_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CatalogItemModel _$CatalogItemModelFromJson(Map<String, dynamic> json) =>
    CatalogItemModel(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
      description: json['description'] as String?,
      isActive: json['isActive'] as bool? ?? true,
    );

Map<String, dynamic> _$CatalogItemModelToJson(CatalogItemModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'description': instance.description,
      'isActive': instance.isActive,
    };
