// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'address_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

AddressInfoModel _$AddressInfoModelFromJson(Map<String, dynamic> json) {
  return _AddressInfoModel.fromJson(json);
}

/// @nodoc
mixin _$AddressInfoModel {
  String get country => throw _privateConstructorUsedError;
  String get provinceState => throw _privateConstructorUsedError;
  String get city => throw _privateConstructorUsedError;
  String get address => throw _privateConstructorUsedError;

  /// Serializes this AddressInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of AddressInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $AddressInfoModelCopyWith<AddressInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $AddressInfoModelCopyWith<$Res> {
  factory $AddressInfoModelCopyWith(
    AddressInfoModel value,
    $Res Function(AddressInfoModel) then,
  ) = _$AddressInfoModelCopyWithImpl<$Res, AddressInfoModel>;
  @useResult
  $Res call({
    String country,
    String provinceState,
    String city,
    String address,
  });
}

/// @nodoc
class _$AddressInfoModelCopyWithImpl<$Res, $Val extends AddressInfoModel>
    implements $AddressInfoModelCopyWith<$Res> {
  _$AddressInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of AddressInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? country = null,
    Object? provinceState = null,
    Object? city = null,
    Object? address = null,
  }) {
    return _then(
      _value.copyWith(
            country: null == country
                ? _value.country
                : country // ignore: cast_nullable_to_non_nullable
                      as String,
            provinceState: null == provinceState
                ? _value.provinceState
                : provinceState // ignore: cast_nullable_to_non_nullable
                      as String,
            city: null == city
                ? _value.city
                : city // ignore: cast_nullable_to_non_nullable
                      as String,
            address: null == address
                ? _value.address
                : address // ignore: cast_nullable_to_non_nullable
                      as String,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$AddressInfoModelImplCopyWith<$Res>
    implements $AddressInfoModelCopyWith<$Res> {
  factory _$$AddressInfoModelImplCopyWith(
    _$AddressInfoModelImpl value,
    $Res Function(_$AddressInfoModelImpl) then,
  ) = __$$AddressInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String country,
    String provinceState,
    String city,
    String address,
  });
}

/// @nodoc
class __$$AddressInfoModelImplCopyWithImpl<$Res>
    extends _$AddressInfoModelCopyWithImpl<$Res, _$AddressInfoModelImpl>
    implements _$$AddressInfoModelImplCopyWith<$Res> {
  __$$AddressInfoModelImplCopyWithImpl(
    _$AddressInfoModelImpl _value,
    $Res Function(_$AddressInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of AddressInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? country = null,
    Object? provinceState = null,
    Object? city = null,
    Object? address = null,
  }) {
    return _then(
      _$AddressInfoModelImpl(
        country: null == country
            ? _value.country
            : country // ignore: cast_nullable_to_non_nullable
                  as String,
        provinceState: null == provinceState
            ? _value.provinceState
            : provinceState // ignore: cast_nullable_to_non_nullable
                  as String,
        city: null == city
            ? _value.city
            : city // ignore: cast_nullable_to_non_nullable
                  as String,
        address: null == address
            ? _value.address
            : address // ignore: cast_nullable_to_non_nullable
                  as String,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$AddressInfoModelImpl extends _AddressInfoModel {
  const _$AddressInfoModelImpl({
    required this.country,
    required this.provinceState,
    required this.city,
    required this.address,
  }) : super._();

  factory _$AddressInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$AddressInfoModelImplFromJson(json);

  @override
  final String country;
  @override
  final String provinceState;
  @override
  final String city;
  @override
  final String address;

  @override
  String toString() {
    return 'AddressInfoModel(country: $country, provinceState: $provinceState, city: $city, address: $address)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$AddressInfoModelImpl &&
            (identical(other.country, country) || other.country == country) &&
            (identical(other.provinceState, provinceState) ||
                other.provinceState == provinceState) &&
            (identical(other.city, city) || other.city == city) &&
            (identical(other.address, address) || other.address == address));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode =>
      Object.hash(runtimeType, country, provinceState, city, address);

  /// Create a copy of AddressInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$AddressInfoModelImplCopyWith<_$AddressInfoModelImpl> get copyWith =>
      __$$AddressInfoModelImplCopyWithImpl<_$AddressInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$AddressInfoModelImplToJson(this);
  }
}

abstract class _AddressInfoModel extends AddressInfoModel {
  const factory _AddressInfoModel({
    required final String country,
    required final String provinceState,
    required final String city,
    required final String address,
  }) = _$AddressInfoModelImpl;
  const _AddressInfoModel._() : super._();

  factory _AddressInfoModel.fromJson(Map<String, dynamic> json) =
      _$AddressInfoModelImpl.fromJson;

  @override
  String get country;
  @override
  String get provinceState;
  @override
  String get city;
  @override
  String get address;

  /// Create a copy of AddressInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$AddressInfoModelImplCopyWith<_$AddressInfoModelImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
