// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'worker_profile_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$WorkerProfileModel {

 String get id; String? get firstName; String? get lastName; String? get email; String? get profilePhoto; String? get phoneNumber; String? get address; String? get city; String? get state; String? get zipCode; DateTime? get dateOfBirth;
/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WorkerProfileModelCopyWith<WorkerProfileModel> get copyWith => _$WorkerProfileModelCopyWithImpl<WorkerProfileModel>(this as WorkerProfileModel, _$identity);

  /// Serializes this WorkerProfileModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WorkerProfileModel&&(identical(other.id, id) || other.id == id)&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.email, email) || other.email == email)&&(identical(other.profilePhoto, profilePhoto) || other.profilePhoto == profilePhoto)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.address, address) || other.address == address)&&(identical(other.city, city) || other.city == city)&&(identical(other.state, state) || other.state == state)&&(identical(other.zipCode, zipCode) || other.zipCode == zipCode)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,firstName,lastName,email,profilePhoto,phoneNumber,address,city,state,zipCode,dateOfBirth);

@override
String toString() {
  return 'WorkerProfileModel(id: $id, firstName: $firstName, lastName: $lastName, email: $email, profilePhoto: $profilePhoto, phoneNumber: $phoneNumber, address: $address, city: $city, state: $state, zipCode: $zipCode, dateOfBirth: $dateOfBirth)';
}


}

/// @nodoc
abstract mixin class $WorkerProfileModelCopyWith<$Res>  {
  factory $WorkerProfileModelCopyWith(WorkerProfileModel value, $Res Function(WorkerProfileModel) _then) = _$WorkerProfileModelCopyWithImpl;
@useResult
$Res call({
 String id, String? firstName, String? lastName, String? email, String? profilePhoto, String? phoneNumber, String? address, String? city, String? state, String? zipCode, DateTime? dateOfBirth
});




}
/// @nodoc
class _$WorkerProfileModelCopyWithImpl<$Res>
    implements $WorkerProfileModelCopyWith<$Res> {
  _$WorkerProfileModelCopyWithImpl(this._self, this._then);

  final WorkerProfileModel _self;
  final $Res Function(WorkerProfileModel) _then;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? firstName = freezed,Object? lastName = freezed,Object? email = freezed,Object? profilePhoto = freezed,Object? phoneNumber = freezed,Object? address = freezed,Object? city = freezed,Object? state = freezed,Object? zipCode = freezed,Object? dateOfBirth = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,firstName: freezed == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String?,lastName: freezed == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,profilePhoto: freezed == profilePhoto ? _self.profilePhoto : profilePhoto // ignore: cast_nullable_to_non_nullable
as String?,phoneNumber: freezed == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,zipCode: freezed == zipCode ? _self.zipCode : zipCode // ignore: cast_nullable_to_non_nullable
as String?,dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as DateTime?,
  ));
}

}


/// Adds pattern-matching-related methods to [WorkerProfileModel].
extension WorkerProfileModelPatterns on WorkerProfileModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WorkerProfileModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WorkerProfileModel value)  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileModel():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WorkerProfileModel value)?  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String? firstName,  String? lastName,  String? email,  String? profilePhoto,  String? phoneNumber,  String? address,  String? city,  String? state,  String? zipCode,  DateTime? dateOfBirth)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
return $default(_that.id,_that.firstName,_that.lastName,_that.email,_that.profilePhoto,_that.phoneNumber,_that.address,_that.city,_that.state,_that.zipCode,_that.dateOfBirth);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String? firstName,  String? lastName,  String? email,  String? profilePhoto,  String? phoneNumber,  String? address,  String? city,  String? state,  String? zipCode,  DateTime? dateOfBirth)  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileModel():
return $default(_that.id,_that.firstName,_that.lastName,_that.email,_that.profilePhoto,_that.phoneNumber,_that.address,_that.city,_that.state,_that.zipCode,_that.dateOfBirth);case _:
  throw StateError('Unexpected subclass');

}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String? firstName,  String? lastName,  String? email,  String? profilePhoto,  String? phoneNumber,  String? address,  String? city,  String? state,  String? zipCode,  DateTime? dateOfBirth)?  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
return $default(_that.id,_that.firstName,_that.lastName,_that.email,_that.profilePhoto,_that.phoneNumber,_that.address,_that.city,_that.state,_that.zipCode,_that.dateOfBirth);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WorkerProfileModel extends WorkerProfileModel {
  const _WorkerProfileModel({required this.id, this.firstName, this.lastName, this.email, this.profilePhoto, this.phoneNumber, this.address, this.city, this.state, this.zipCode, this.dateOfBirth}): super._();
  factory _WorkerProfileModel.fromJson(Map<String, dynamic> json) => _$WorkerProfileModelFromJson(json);

@override final  String id;
@override final  String? firstName;
@override final  String? lastName;
@override final  String? email;
@override final  String? profilePhoto;
@override final  String? phoneNumber;
@override final  String? address;
@override final  String? city;
@override final  String? state;
@override final  String? zipCode;
@override final  DateTime? dateOfBirth;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WorkerProfileModelCopyWith<_WorkerProfileModel> get copyWith => __$WorkerProfileModelCopyWithImpl<_WorkerProfileModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WorkerProfileModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _WorkerProfileModel&&(identical(other.id, id) || other.id == id)&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.email, email) || other.email == email)&&(identical(other.profilePhoto, profilePhoto) || other.profilePhoto == profilePhoto)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.address, address) || other.address == address)&&(identical(other.city, city) || other.city == city)&&(identical(other.state, state) || other.state == state)&&(identical(other.zipCode, zipCode) || other.zipCode == zipCode)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,firstName,lastName,email,profilePhoto,phoneNumber,address,city,state,zipCode,dateOfBirth);

@override
String toString() {
  return 'WorkerProfileModel(id: $id, firstName: $firstName, lastName: $lastName, email: $email, profilePhoto: $profilePhoto, phoneNumber: $phoneNumber, address: $address, city: $city, state: $state, zipCode: $zipCode, dateOfBirth: $dateOfBirth)';
}


}

/// @nodoc
abstract mixin class _$WorkerProfileModelCopyWith<$Res> implements $WorkerProfileModelCopyWith<$Res> {
  factory _$WorkerProfileModelCopyWith(_WorkerProfileModel value, $Res Function(_WorkerProfileModel) _then) = __$WorkerProfileModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String? firstName, String? lastName, String? email, String? profilePhoto, String? phoneNumber, String? address, String? city, String? state, String? zipCode, DateTime? dateOfBirth
});




}
/// @nodoc
class __$WorkerProfileModelCopyWithImpl<$Res>
    implements _$WorkerProfileModelCopyWith<$Res> {
  __$WorkerProfileModelCopyWithImpl(this._self, this._then);

  final _WorkerProfileModel _self;
  final $Res Function(_WorkerProfileModel) _then;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? firstName = freezed,Object? lastName = freezed,Object? email = freezed,Object? profilePhoto = freezed,Object? phoneNumber = freezed,Object? address = freezed,Object? city = freezed,Object? state = freezed,Object? zipCode = freezed,Object? dateOfBirth = freezed,}) {
  return _then(_WorkerProfileModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,firstName: freezed == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String?,lastName: freezed == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,profilePhoto: freezed == profilePhoto ? _self.profilePhoto : profilePhoto // ignore: cast_nullable_to_non_nullable
as String?,phoneNumber: freezed == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,zipCode: freezed == zipCode ? _self.zipCode : zipCode // ignore: cast_nullable_to_non_nullable
as String?,dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as DateTime?,
  ));
}


}

// dart format on
