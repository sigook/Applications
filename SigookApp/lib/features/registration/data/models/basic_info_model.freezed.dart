// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'basic_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$BasicInfoModel {

 String get firstName; String get lastName; String get dateOfBirth;// ISO 8601 string
 String get genderId; String get genderValue; Map<String, dynamic>? get country; Map<String, dynamic>? get provinceState; Map<String, dynamic>? get city; String get address; String get zipCode; String get mobileNumber; String? get identificationType; String? get identificationNumber;
/// Create a copy of BasicInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$BasicInfoModelCopyWith<BasicInfoModel> get copyWith => _$BasicInfoModelCopyWithImpl<BasicInfoModel>(this as BasicInfoModel, _$identity);

  /// Serializes this BasicInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is BasicInfoModel&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.genderId, genderId) || other.genderId == genderId)&&(identical(other.genderValue, genderValue) || other.genderValue == genderValue)&&const DeepCollectionEquality().equals(other.country, country)&&const DeepCollectionEquality().equals(other.provinceState, provinceState)&&const DeepCollectionEquality().equals(other.city, city)&&(identical(other.address, address) || other.address == address)&&(identical(other.zipCode, zipCode) || other.zipCode == zipCode)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber)&&(identical(other.identificationType, identificationType) || other.identificationType == identificationType)&&(identical(other.identificationNumber, identificationNumber) || other.identificationNumber == identificationNumber));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,firstName,lastName,dateOfBirth,genderId,genderValue,const DeepCollectionEquality().hash(country),const DeepCollectionEquality().hash(provinceState),const DeepCollectionEquality().hash(city),address,zipCode,mobileNumber,identificationType,identificationNumber);

@override
String toString() {
  return 'BasicInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, genderId: $genderId, genderValue: $genderValue, country: $country, provinceState: $provinceState, city: $city, address: $address, zipCode: $zipCode, mobileNumber: $mobileNumber, identificationType: $identificationType, identificationNumber: $identificationNumber)';
}


}

/// @nodoc
abstract mixin class $BasicInfoModelCopyWith<$Res>  {
  factory $BasicInfoModelCopyWith(BasicInfoModel value, $Res Function(BasicInfoModel) _then) = _$BasicInfoModelCopyWithImpl;
@useResult
$Res call({
 String firstName, String lastName, String dateOfBirth, String genderId, String genderValue, Map<String, dynamic>? country, Map<String, dynamic>? provinceState, Map<String, dynamic>? city, String address, String zipCode, String mobileNumber, String? identificationType, String? identificationNumber
});




}
/// @nodoc
class _$BasicInfoModelCopyWithImpl<$Res>
    implements $BasicInfoModelCopyWith<$Res> {
  _$BasicInfoModelCopyWithImpl(this._self, this._then);

  final BasicInfoModel _self;
  final $Res Function(BasicInfoModel) _then;

/// Create a copy of BasicInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? firstName = null,Object? lastName = null,Object? dateOfBirth = null,Object? genderId = null,Object? genderValue = null,Object? country = freezed,Object? provinceState = freezed,Object? city = freezed,Object? address = null,Object? zipCode = null,Object? mobileNumber = null,Object? identificationType = freezed,Object? identificationNumber = freezed,}) {
  return _then(_self.copyWith(
firstName: null == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String,lastName: null == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String,dateOfBirth: null == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String,genderId: null == genderId ? _self.genderId : genderId // ignore: cast_nullable_to_non_nullable
as String,genderValue: null == genderValue ? _self.genderValue : genderValue // ignore: cast_nullable_to_non_nullable
as String,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,provinceState: freezed == provinceState ? _self.provinceState : provinceState // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,address: null == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String,zipCode: null == zipCode ? _self.zipCode : zipCode // ignore: cast_nullable_to_non_nullable
as String,mobileNumber: null == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String,identificationType: freezed == identificationType ? _self.identificationType : identificationType // ignore: cast_nullable_to_non_nullable
as String?,identificationNumber: freezed == identificationNumber ? _self.identificationNumber : identificationNumber // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [BasicInfoModel].
extension BasicInfoModelPatterns on BasicInfoModel {
/// A variant of `map` that fallback to returning `orElse`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _BasicInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _BasicInfoModel() when $default != null:
return $default(_that);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// Callbacks receives the raw object, upcasted.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case final Subclass2 value:
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _BasicInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _BasicInfoModel():
return $default(_that);}
}
/// A variant of `map` that fallback to returning `null`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _BasicInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _BasicInfoModel() when $default != null:
return $default(_that);case _:
  return null;

}
}
/// A variant of `when` that fallback to an `orElse` callback.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String firstName,  String lastName,  String dateOfBirth,  String genderId,  String genderValue,  Map<String, dynamic>? country,  Map<String, dynamic>? provinceState,  Map<String, dynamic>? city,  String address,  String zipCode,  String mobileNumber,  String? identificationType,  String? identificationNumber)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _BasicInfoModel() when $default != null:
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderValue,_that.country,_that.provinceState,_that.city,_that.address,_that.zipCode,_that.mobileNumber,_that.identificationType,_that.identificationNumber);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// As opposed to `map`, this offers destructuring.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case Subclass2(:final field2):
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String firstName,  String lastName,  String dateOfBirth,  String genderId,  String genderValue,  Map<String, dynamic>? country,  Map<String, dynamic>? provinceState,  Map<String, dynamic>? city,  String address,  String zipCode,  String mobileNumber,  String? identificationType,  String? identificationNumber)  $default,) {final _that = this;
switch (_that) {
case _BasicInfoModel():
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderValue,_that.country,_that.provinceState,_that.city,_that.address,_that.zipCode,_that.mobileNumber,_that.identificationType,_that.identificationNumber);}
}
/// A variant of `when` that fallback to returning `null`
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String firstName,  String lastName,  String dateOfBirth,  String genderId,  String genderValue,  Map<String, dynamic>? country,  Map<String, dynamic>? provinceState,  Map<String, dynamic>? city,  String address,  String zipCode,  String mobileNumber,  String? identificationType,  String? identificationNumber)?  $default,) {final _that = this;
switch (_that) {
case _BasicInfoModel() when $default != null:
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderValue,_that.country,_that.provinceState,_that.city,_that.address,_that.zipCode,_that.mobileNumber,_that.identificationType,_that.identificationNumber);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _BasicInfoModel extends BasicInfoModel {
  const _BasicInfoModel({required this.firstName, required this.lastName, required this.dateOfBirth, required this.genderId, required this.genderValue, final  Map<String, dynamic>? country, final  Map<String, dynamic>? provinceState, final  Map<String, dynamic>? city, required this.address, required this.zipCode, required this.mobileNumber, this.identificationType, this.identificationNumber}): _country = country,_provinceState = provinceState,_city = city,super._();
  factory _BasicInfoModel.fromJson(Map<String, dynamic> json) => _$BasicInfoModelFromJson(json);

@override final  String firstName;
@override final  String lastName;
@override final  String dateOfBirth;
// ISO 8601 string
@override final  String genderId;
@override final  String genderValue;
 final  Map<String, dynamic>? _country;
@override Map<String, dynamic>? get country {
  final value = _country;
  if (value == null) return null;
  if (_country is EqualUnmodifiableMapView) return _country;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(value);
}

 final  Map<String, dynamic>? _provinceState;
@override Map<String, dynamic>? get provinceState {
  final value = _provinceState;
  if (value == null) return null;
  if (_provinceState is EqualUnmodifiableMapView) return _provinceState;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(value);
}

 final  Map<String, dynamic>? _city;
@override Map<String, dynamic>? get city {
  final value = _city;
  if (value == null) return null;
  if (_city is EqualUnmodifiableMapView) return _city;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(value);
}

@override final  String address;
@override final  String zipCode;
@override final  String mobileNumber;
@override final  String? identificationType;
@override final  String? identificationNumber;

/// Create a copy of BasicInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$BasicInfoModelCopyWith<_BasicInfoModel> get copyWith => __$BasicInfoModelCopyWithImpl<_BasicInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$BasicInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _BasicInfoModel&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.genderId, genderId) || other.genderId == genderId)&&(identical(other.genderValue, genderValue) || other.genderValue == genderValue)&&const DeepCollectionEquality().equals(other._country, _country)&&const DeepCollectionEquality().equals(other._provinceState, _provinceState)&&const DeepCollectionEquality().equals(other._city, _city)&&(identical(other.address, address) || other.address == address)&&(identical(other.zipCode, zipCode) || other.zipCode == zipCode)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber)&&(identical(other.identificationType, identificationType) || other.identificationType == identificationType)&&(identical(other.identificationNumber, identificationNumber) || other.identificationNumber == identificationNumber));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,firstName,lastName,dateOfBirth,genderId,genderValue,const DeepCollectionEquality().hash(_country),const DeepCollectionEquality().hash(_provinceState),const DeepCollectionEquality().hash(_city),address,zipCode,mobileNumber,identificationType,identificationNumber);

@override
String toString() {
  return 'BasicInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, genderId: $genderId, genderValue: $genderValue, country: $country, provinceState: $provinceState, city: $city, address: $address, zipCode: $zipCode, mobileNumber: $mobileNumber, identificationType: $identificationType, identificationNumber: $identificationNumber)';
}


}

/// @nodoc
abstract mixin class _$BasicInfoModelCopyWith<$Res> implements $BasicInfoModelCopyWith<$Res> {
  factory _$BasicInfoModelCopyWith(_BasicInfoModel value, $Res Function(_BasicInfoModel) _then) = __$BasicInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String firstName, String lastName, String dateOfBirth, String genderId, String genderValue, Map<String, dynamic>? country, Map<String, dynamic>? provinceState, Map<String, dynamic>? city, String address, String zipCode, String mobileNumber, String? identificationType, String? identificationNumber
});




}
/// @nodoc
class __$BasicInfoModelCopyWithImpl<$Res>
    implements _$BasicInfoModelCopyWith<$Res> {
  __$BasicInfoModelCopyWithImpl(this._self, this._then);

  final _BasicInfoModel _self;
  final $Res Function(_BasicInfoModel) _then;

/// Create a copy of BasicInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? firstName = null,Object? lastName = null,Object? dateOfBirth = null,Object? genderId = null,Object? genderValue = null,Object? country = freezed,Object? provinceState = freezed,Object? city = freezed,Object? address = null,Object? zipCode = null,Object? mobileNumber = null,Object? identificationType = freezed,Object? identificationNumber = freezed,}) {
  return _then(_BasicInfoModel(
firstName: null == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String,lastName: null == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String,dateOfBirth: null == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String,genderId: null == genderId ? _self.genderId : genderId // ignore: cast_nullable_to_non_nullable
as String,genderValue: null == genderValue ? _self.genderValue : genderValue // ignore: cast_nullable_to_non_nullable
as String,country: freezed == country ? _self._country : country // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,provinceState: freezed == provinceState ? _self._provinceState : provinceState // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,city: freezed == city ? _self._city : city // ignore: cast_nullable_to_non_nullable
as Map<String, dynamic>?,address: null == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String,zipCode: null == zipCode ? _self.zipCode : zipCode // ignore: cast_nullable_to_non_nullable
as String,mobileNumber: null == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String,identificationType: freezed == identificationType ? _self.identificationType : identificationType // ignore: cast_nullable_to_non_nullable
as String?,identificationNumber: freezed == identificationNumber ? _self.identificationNumber : identificationNumber // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on
