// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'basic_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

BasicInfoModel _$BasicInfoModelFromJson(Map<String, dynamic> json) {
  return _BasicInfoModel.fromJson(json);
}

/// @nodoc
mixin _$BasicInfoModel {
  String get firstName => throw _privateConstructorUsedError;
  String get lastName => throw _privateConstructorUsedError;
  String get dateOfBirth =>
      throw _privateConstructorUsedError; // ISO 8601 string
  String get genderId => throw _privateConstructorUsedError;
  String get genderValue => throw _privateConstructorUsedError;
  Map<String, dynamic>? get country => throw _privateConstructorUsedError;
  Map<String, dynamic>? get provinceState => throw _privateConstructorUsedError;
  Map<String, dynamic>? get city => throw _privateConstructorUsedError;
  String get address => throw _privateConstructorUsedError;
  String get zipCode => throw _privateConstructorUsedError;
  String get mobileNumber => throw _privateConstructorUsedError;
  String? get identificationType => throw _privateConstructorUsedError;
  String? get identificationNumber => throw _privateConstructorUsedError;

  /// Serializes this BasicInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of BasicInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $BasicInfoModelCopyWith<BasicInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $BasicInfoModelCopyWith<$Res> {
  factory $BasicInfoModelCopyWith(
    BasicInfoModel value,
    $Res Function(BasicInfoModel) then,
  ) = _$BasicInfoModelCopyWithImpl<$Res, BasicInfoModel>;
  @useResult
  $Res call({
    String firstName,
    String lastName,
    String dateOfBirth,
    String genderId,
    String genderValue,
    Map<String, dynamic>? country,
    Map<String, dynamic>? provinceState,
    Map<String, dynamic>? city,
    String address,
    String zipCode,
    String mobileNumber,
    String? identificationType,
    String? identificationNumber,
  });
}

/// @nodoc
class _$BasicInfoModelCopyWithImpl<$Res, $Val extends BasicInfoModel>
    implements $BasicInfoModelCopyWith<$Res> {
  _$BasicInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of BasicInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? firstName = null,
    Object? lastName = null,
    Object? dateOfBirth = null,
    Object? genderId = null,
    Object? genderValue = null,
    Object? country = freezed,
    Object? provinceState = freezed,
    Object? city = freezed,
    Object? address = null,
    Object? zipCode = null,
    Object? mobileNumber = null,
    Object? identificationType = freezed,
    Object? identificationNumber = freezed,
  }) {
    return _then(
      _value.copyWith(
            firstName: null == firstName
                ? _value.firstName
                : firstName // ignore: cast_nullable_to_non_nullable
                      as String,
            lastName: null == lastName
                ? _value.lastName
                : lastName // ignore: cast_nullable_to_non_nullable
                      as String,
            dateOfBirth: null == dateOfBirth
                ? _value.dateOfBirth
                : dateOfBirth // ignore: cast_nullable_to_non_nullable
                      as String,
            genderId: null == genderId
                ? _value.genderId
                : genderId // ignore: cast_nullable_to_non_nullable
                      as String,
            genderValue: null == genderValue
                ? _value.genderValue
                : genderValue // ignore: cast_nullable_to_non_nullable
                      as String,
            country: freezed == country
                ? _value.country
                : country // ignore: cast_nullable_to_non_nullable
                      as Map<String, dynamic>?,
            provinceState: freezed == provinceState
                ? _value.provinceState
                : provinceState // ignore: cast_nullable_to_non_nullable
                      as Map<String, dynamic>?,
            city: freezed == city
                ? _value.city
                : city // ignore: cast_nullable_to_non_nullable
                      as Map<String, dynamic>?,
            address: null == address
                ? _value.address
                : address // ignore: cast_nullable_to_non_nullable
                      as String,
            zipCode: null == zipCode
                ? _value.zipCode
                : zipCode // ignore: cast_nullable_to_non_nullable
                      as String,
            mobileNumber: null == mobileNumber
                ? _value.mobileNumber
                : mobileNumber // ignore: cast_nullable_to_non_nullable
                      as String,
            identificationType: freezed == identificationType
                ? _value.identificationType
                : identificationType // ignore: cast_nullable_to_non_nullable
                      as String?,
            identificationNumber: freezed == identificationNumber
                ? _value.identificationNumber
                : identificationNumber // ignore: cast_nullable_to_non_nullable
                      as String?,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$BasicInfoModelImplCopyWith<$Res>
    implements $BasicInfoModelCopyWith<$Res> {
  factory _$$BasicInfoModelImplCopyWith(
    _$BasicInfoModelImpl value,
    $Res Function(_$BasicInfoModelImpl) then,
  ) = __$$BasicInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String firstName,
    String lastName,
    String dateOfBirth,
    String genderId,
    String genderValue,
    Map<String, dynamic>? country,
    Map<String, dynamic>? provinceState,
    Map<String, dynamic>? city,
    String address,
    String zipCode,
    String mobileNumber,
    String? identificationType,
    String? identificationNumber,
  });
}

/// @nodoc
class __$$BasicInfoModelImplCopyWithImpl<$Res>
    extends _$BasicInfoModelCopyWithImpl<$Res, _$BasicInfoModelImpl>
    implements _$$BasicInfoModelImplCopyWith<$Res> {
  __$$BasicInfoModelImplCopyWithImpl(
    _$BasicInfoModelImpl _value,
    $Res Function(_$BasicInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of BasicInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? firstName = null,
    Object? lastName = null,
    Object? dateOfBirth = null,
    Object? genderId = null,
    Object? genderValue = null,
    Object? country = freezed,
    Object? provinceState = freezed,
    Object? city = freezed,
    Object? address = null,
    Object? zipCode = null,
    Object? mobileNumber = null,
    Object? identificationType = freezed,
    Object? identificationNumber = freezed,
  }) {
    return _then(
      _$BasicInfoModelImpl(
        firstName: null == firstName
            ? _value.firstName
            : firstName // ignore: cast_nullable_to_non_nullable
                  as String,
        lastName: null == lastName
            ? _value.lastName
            : lastName // ignore: cast_nullable_to_non_nullable
                  as String,
        dateOfBirth: null == dateOfBirth
            ? _value.dateOfBirth
            : dateOfBirth // ignore: cast_nullable_to_non_nullable
                  as String,
        genderId: null == genderId
            ? _value.genderId
            : genderId // ignore: cast_nullable_to_non_nullable
                  as String,
        genderValue: null == genderValue
            ? _value.genderValue
            : genderValue // ignore: cast_nullable_to_non_nullable
                  as String,
        country: freezed == country
            ? _value._country
            : country // ignore: cast_nullable_to_non_nullable
                  as Map<String, dynamic>?,
        provinceState: freezed == provinceState
            ? _value._provinceState
            : provinceState // ignore: cast_nullable_to_non_nullable
                  as Map<String, dynamic>?,
        city: freezed == city
            ? _value._city
            : city // ignore: cast_nullable_to_non_nullable
                  as Map<String, dynamic>?,
        address: null == address
            ? _value.address
            : address // ignore: cast_nullable_to_non_nullable
                  as String,
        zipCode: null == zipCode
            ? _value.zipCode
            : zipCode // ignore: cast_nullable_to_non_nullable
                  as String,
        mobileNumber: null == mobileNumber
            ? _value.mobileNumber
            : mobileNumber // ignore: cast_nullable_to_non_nullable
                  as String,
        identificationType: freezed == identificationType
            ? _value.identificationType
            : identificationType // ignore: cast_nullable_to_non_nullable
                  as String?,
        identificationNumber: freezed == identificationNumber
            ? _value.identificationNumber
            : identificationNumber // ignore: cast_nullable_to_non_nullable
                  as String?,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$BasicInfoModelImpl extends _BasicInfoModel {
  const _$BasicInfoModelImpl({
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
    required this.genderId,
    required this.genderValue,
    final Map<String, dynamic>? country,
    final Map<String, dynamic>? provinceState,
    final Map<String, dynamic>? city,
    required this.address,
    required this.zipCode,
    required this.mobileNumber,
    this.identificationType,
    this.identificationNumber,
  }) : _country = country,
       _provinceState = provinceState,
       _city = city,
       super._();

  factory _$BasicInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$BasicInfoModelImplFromJson(json);

  @override
  final String firstName;
  @override
  final String lastName;
  @override
  final String dateOfBirth;
  // ISO 8601 string
  @override
  final String genderId;
  @override
  final String genderValue;
  final Map<String, dynamic>? _country;
  @override
  Map<String, dynamic>? get country {
    final value = _country;
    if (value == null) return null;
    if (_country is EqualUnmodifiableMapView) return _country;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(value);
  }

  final Map<String, dynamic>? _provinceState;
  @override
  Map<String, dynamic>? get provinceState {
    final value = _provinceState;
    if (value == null) return null;
    if (_provinceState is EqualUnmodifiableMapView) return _provinceState;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(value);
  }

  final Map<String, dynamic>? _city;
  @override
  Map<String, dynamic>? get city {
    final value = _city;
    if (value == null) return null;
    if (_city is EqualUnmodifiableMapView) return _city;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(value);
  }

  @override
  final String address;
  @override
  final String zipCode;
  @override
  final String mobileNumber;
  @override
  final String? identificationType;
  @override
  final String? identificationNumber;

  @override
  String toString() {
    return 'BasicInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, genderId: $genderId, genderValue: $genderValue, country: $country, provinceState: $provinceState, city: $city, address: $address, zipCode: $zipCode, mobileNumber: $mobileNumber, identificationType: $identificationType, identificationNumber: $identificationNumber)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$BasicInfoModelImpl &&
            (identical(other.firstName, firstName) ||
                other.firstName == firstName) &&
            (identical(other.lastName, lastName) ||
                other.lastName == lastName) &&
            (identical(other.dateOfBirth, dateOfBirth) ||
                other.dateOfBirth == dateOfBirth) &&
            (identical(other.genderId, genderId) ||
                other.genderId == genderId) &&
            (identical(other.genderValue, genderValue) ||
                other.genderValue == genderValue) &&
            const DeepCollectionEquality().equals(other._country, _country) &&
            const DeepCollectionEquality().equals(
              other._provinceState,
              _provinceState,
            ) &&
            const DeepCollectionEquality().equals(other._city, _city) &&
            (identical(other.address, address) || other.address == address) &&
            (identical(other.zipCode, zipCode) || other.zipCode == zipCode) &&
            (identical(other.mobileNumber, mobileNumber) ||
                other.mobileNumber == mobileNumber) &&
            (identical(other.identificationType, identificationType) ||
                other.identificationType == identificationType) &&
            (identical(other.identificationNumber, identificationNumber) ||
                other.identificationNumber == identificationNumber));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    firstName,
    lastName,
    dateOfBirth,
    genderId,
    genderValue,
    const DeepCollectionEquality().hash(_country),
    const DeepCollectionEquality().hash(_provinceState),
    const DeepCollectionEquality().hash(_city),
    address,
    zipCode,
    mobileNumber,
    identificationType,
    identificationNumber,
  );

  /// Create a copy of BasicInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$BasicInfoModelImplCopyWith<_$BasicInfoModelImpl> get copyWith =>
      __$$BasicInfoModelImplCopyWithImpl<_$BasicInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$BasicInfoModelImplToJson(this);
  }
}

abstract class _BasicInfoModel extends BasicInfoModel {
  const factory _BasicInfoModel({
    required final String firstName,
    required final String lastName,
    required final String dateOfBirth,
    required final String genderId,
    required final String genderValue,
    final Map<String, dynamic>? country,
    final Map<String, dynamic>? provinceState,
    final Map<String, dynamic>? city,
    required final String address,
    required final String zipCode,
    required final String mobileNumber,
    final String? identificationType,
    final String? identificationNumber,
  }) = _$BasicInfoModelImpl;
  const _BasicInfoModel._() : super._();

  factory _BasicInfoModel.fromJson(Map<String, dynamic> json) =
      _$BasicInfoModelImpl.fromJson;

  @override
  String get firstName;
  @override
  String get lastName;
  @override
  String get dateOfBirth; // ISO 8601 string
  @override
  String get genderId;
  @override
  String get genderValue;
  @override
  Map<String, dynamic>? get country;
  @override
  Map<String, dynamic>? get provinceState;
  @override
  Map<String, dynamic>? get city;
  @override
  String get address;
  @override
  String get zipCode;
  @override
  String get mobileNumber;
  @override
  String? get identificationType;
  @override
  String? get identificationNumber;

  /// Create a copy of BasicInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$BasicInfoModelImplCopyWith<_$BasicInfoModelImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
