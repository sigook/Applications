import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/address_info.dart';

part 'address_info_model.freezed.dart';
part 'address_info_model.g.dart';

@freezed
sealed class AddressInfoModel with _$AddressInfoModel {
  const AddressInfoModel._();

  const factory AddressInfoModel({
    required String country,
    required String provinceState,
    required String city,
    required String address,
    @Default('') String zipCode,
  }) = _AddressInfoModel;

  /// Convert from domain entity
  factory AddressInfoModel.fromEntity(AddressInfo entity) {
    return AddressInfoModel(
      country: entity.country,
      provinceState: entity.provinceState,
      city: entity.city,
      address: entity.address,
      zipCode: entity.zipCode,
    );
  }

  /// Convert to domain entity
  AddressInfo toEntity() {
    return AddressInfo(
      country: country,
      provinceState: provinceState,
      city: city,
      address: address,
      zipCode: zipCode,
    );
  }

  // Example for comment

  /// From JSON
  factory AddressInfoModel.fromJson(Map<String, dynamic> json) =>
      _$AddressInfoModelFromJson(json);
}
