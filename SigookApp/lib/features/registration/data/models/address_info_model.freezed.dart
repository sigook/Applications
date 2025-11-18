// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'address_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AddressInfoModel {

 String get country; String get provinceState; String get city; String get address;
/// Create a copy of AddressInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AddressInfoModelCopyWith<AddressInfoModel> get copyWith => _$AddressInfoModelCopyWithImpl<AddressInfoModel>(this as AddressInfoModel, _$identity);

  /// Serializes this AddressInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AddressInfoModel&&(identical(other.country, country) || other.country == country)&&(identical(other.provinceState, provinceState) || other.provinceState == provinceState)&&(identical(other.city, city) || other.city == city)&&(identical(other.address, address) || other.address == address));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,country,provinceState,city,address);

@override
String toString() {
  return 'AddressInfoModel(country: $country, provinceState: $provinceState, city: $city, address: $address)';
}


}

/// @nodoc
abstract mixin class $AddressInfoModelCopyWith<$Res>  {
  factory $AddressInfoModelCopyWith(AddressInfoModel value, $Res Function(AddressInfoModel) _then) = _$AddressInfoModelCopyWithImpl;
@useResult
$Res call({
 String country, String provinceState, String city, String address
});




}
/// @nodoc
class _$AddressInfoModelCopyWithImpl<$Res>
    implements $AddressInfoModelCopyWith<$Res> {
  _$AddressInfoModelCopyWithImpl(this._self, this._then);

  final AddressInfoModel _self;
  final $Res Function(AddressInfoModel) _then;

/// Create a copy of AddressInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? country = null,Object? provinceState = null,Object? city = null,Object? address = null,}) {
  return _then(_self.copyWith(
country: null == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String,provinceState: null == provinceState ? _self.provinceState : provinceState // ignore: cast_nullable_to_non_nullable
as String,city: null == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String,address: null == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [AddressInfoModel].
extension AddressInfoModelPatterns on AddressInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AddressInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AddressInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AddressInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _AddressInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AddressInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _AddressInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String country,  String provinceState,  String city,  String address)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AddressInfoModel() when $default != null:
return $default(_that.country,_that.provinceState,_that.city,_that.address);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String country,  String provinceState,  String city,  String address)  $default,) {final _that = this;
switch (_that) {
case _AddressInfoModel():
return $default(_that.country,_that.provinceState,_that.city,_that.address);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String country,  String provinceState,  String city,  String address)?  $default,) {final _that = this;
switch (_that) {
case _AddressInfoModel() when $default != null:
return $default(_that.country,_that.provinceState,_that.city,_that.address);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AddressInfoModel extends AddressInfoModel {
  const _AddressInfoModel({required this.country, required this.provinceState, required this.city, required this.address}): super._();
  factory _AddressInfoModel.fromJson(Map<String, dynamic> json) => _$AddressInfoModelFromJson(json);

@override final  String country;
@override final  String provinceState;
@override final  String city;
@override final  String address;

/// Create a copy of AddressInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AddressInfoModelCopyWith<_AddressInfoModel> get copyWith => __$AddressInfoModelCopyWithImpl<_AddressInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AddressInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AddressInfoModel&&(identical(other.country, country) || other.country == country)&&(identical(other.provinceState, provinceState) || other.provinceState == provinceState)&&(identical(other.city, city) || other.city == city)&&(identical(other.address, address) || other.address == address));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,country,provinceState,city,address);

@override
String toString() {
  return 'AddressInfoModel(country: $country, provinceState: $provinceState, city: $city, address: $address)';
}


}

/// @nodoc
abstract mixin class _$AddressInfoModelCopyWith<$Res> implements $AddressInfoModelCopyWith<$Res> {
  factory _$AddressInfoModelCopyWith(_AddressInfoModel value, $Res Function(_AddressInfoModel) _then) = __$AddressInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String country, String provinceState, String city, String address
});




}
/// @nodoc
class __$AddressInfoModelCopyWithImpl<$Res>
    implements _$AddressInfoModelCopyWith<$Res> {
  __$AddressInfoModelCopyWithImpl(this._self, this._then);

  final _AddressInfoModel _self;
  final $Res Function(_AddressInfoModel) _then;

/// Create a copy of AddressInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? country = null,Object? provinceState = null,Object? city = null,Object? address = null,}) {
  return _then(_AddressInfoModel(
country: null == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String,provinceState: null == provinceState ? _self.provinceState : provinceState // ignore: cast_nullable_to_non_nullable
as String,city: null == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String,address: null == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on
