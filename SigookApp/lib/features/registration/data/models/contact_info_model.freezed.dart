// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'contact_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ContactInfoModel {

 String get email; String get password; String get identification; String? get identificationTypeId;// May be null from API
 String get identificationTypeName; String get mobileNumber;
/// Create a copy of ContactInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ContactInfoModelCopyWith<ContactInfoModel> get copyWith => _$ContactInfoModelCopyWithImpl<ContactInfoModel>(this as ContactInfoModel, _$identity);

  /// Serializes this ContactInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ContactInfoModel&&(identical(other.email, email) || other.email == email)&&(identical(other.password, password) || other.password == password)&&(identical(other.identification, identification) || other.identification == identification)&&(identical(other.identificationTypeId, identificationTypeId) || other.identificationTypeId == identificationTypeId)&&(identical(other.identificationTypeName, identificationTypeName) || other.identificationTypeName == identificationTypeName)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email,password,identification,identificationTypeId,identificationTypeName,mobileNumber);

@override
String toString() {
  return 'ContactInfoModel(email: $email, password: $password, identification: $identification, identificationTypeId: $identificationTypeId, identificationTypeName: $identificationTypeName, mobileNumber: $mobileNumber)';
}


}

/// @nodoc
abstract mixin class $ContactInfoModelCopyWith<$Res>  {
  factory $ContactInfoModelCopyWith(ContactInfoModel value, $Res Function(ContactInfoModel) _then) = _$ContactInfoModelCopyWithImpl;
@useResult
$Res call({
 String email, String password, String identification, String? identificationTypeId, String identificationTypeName, String mobileNumber
});




}
/// @nodoc
class _$ContactInfoModelCopyWithImpl<$Res>
    implements $ContactInfoModelCopyWith<$Res> {
  _$ContactInfoModelCopyWithImpl(this._self, this._then);

  final ContactInfoModel _self;
  final $Res Function(ContactInfoModel) _then;

/// Create a copy of ContactInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? email = null,Object? password = null,Object? identification = null,Object? identificationTypeId = freezed,Object? identificationTypeName = null,Object? mobileNumber = null,}) {
  return _then(_self.copyWith(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,password: null == password ? _self.password : password // ignore: cast_nullable_to_non_nullable
as String,identification: null == identification ? _self.identification : identification // ignore: cast_nullable_to_non_nullable
as String,identificationTypeId: freezed == identificationTypeId ? _self.identificationTypeId : identificationTypeId // ignore: cast_nullable_to_non_nullable
as String?,identificationTypeName: null == identificationTypeName ? _self.identificationTypeName : identificationTypeName // ignore: cast_nullable_to_non_nullable
as String,mobileNumber: null == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [ContactInfoModel].
extension ContactInfoModelPatterns on ContactInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ContactInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ContactInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ContactInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _ContactInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ContactInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _ContactInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String email,  String password,  String identification,  String? identificationTypeId,  String identificationTypeName,  String mobileNumber)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ContactInfoModel() when $default != null:
return $default(_that.email,_that.password,_that.identification,_that.identificationTypeId,_that.identificationTypeName,_that.mobileNumber);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String email,  String password,  String identification,  String? identificationTypeId,  String identificationTypeName,  String mobileNumber)  $default,) {final _that = this;
switch (_that) {
case _ContactInfoModel():
return $default(_that.email,_that.password,_that.identification,_that.identificationTypeId,_that.identificationTypeName,_that.mobileNumber);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String email,  String password,  String identification,  String? identificationTypeId,  String identificationTypeName,  String mobileNumber)?  $default,) {final _that = this;
switch (_that) {
case _ContactInfoModel() when $default != null:
return $default(_that.email,_that.password,_that.identification,_that.identificationTypeId,_that.identificationTypeName,_that.mobileNumber);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ContactInfoModel extends ContactInfoModel {
  const _ContactInfoModel({required this.email, required this.password, required this.identification, this.identificationTypeId, required this.identificationTypeName, required this.mobileNumber}): super._();
  factory _ContactInfoModel.fromJson(Map<String, dynamic> json) => _$ContactInfoModelFromJson(json);

@override final  String email;
@override final  String password;
@override final  String identification;
@override final  String? identificationTypeId;
// May be null from API
@override final  String identificationTypeName;
@override final  String mobileNumber;

/// Create a copy of ContactInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ContactInfoModelCopyWith<_ContactInfoModel> get copyWith => __$ContactInfoModelCopyWithImpl<_ContactInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ContactInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ContactInfoModel&&(identical(other.email, email) || other.email == email)&&(identical(other.password, password) || other.password == password)&&(identical(other.identification, identification) || other.identification == identification)&&(identical(other.identificationTypeId, identificationTypeId) || other.identificationTypeId == identificationTypeId)&&(identical(other.identificationTypeName, identificationTypeName) || other.identificationTypeName == identificationTypeName)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email,password,identification,identificationTypeId,identificationTypeName,mobileNumber);

@override
String toString() {
  return 'ContactInfoModel(email: $email, password: $password, identification: $identification, identificationTypeId: $identificationTypeId, identificationTypeName: $identificationTypeName, mobileNumber: $mobileNumber)';
}


}

/// @nodoc
abstract mixin class _$ContactInfoModelCopyWith<$Res> implements $ContactInfoModelCopyWith<$Res> {
  factory _$ContactInfoModelCopyWith(_ContactInfoModel value, $Res Function(_ContactInfoModel) _then) = __$ContactInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String email, String password, String identification, String? identificationTypeId, String identificationTypeName, String mobileNumber
});




}
/// @nodoc
class __$ContactInfoModelCopyWithImpl<$Res>
    implements _$ContactInfoModelCopyWith<$Res> {
  __$ContactInfoModelCopyWithImpl(this._self, this._then);

  final _ContactInfoModel _self;
  final $Res Function(_ContactInfoModel) _then;

/// Create a copy of ContactInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? email = null,Object? password = null,Object? identification = null,Object? identificationTypeId = freezed,Object? identificationTypeName = null,Object? mobileNumber = null,}) {
  return _then(_ContactInfoModel(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,password: null == password ? _self.password : password // ignore: cast_nullable_to_non_nullable
as String,identification: null == identification ? _self.identification : identification // ignore: cast_nullable_to_non_nullable
as String,identificationTypeId: freezed == identificationTypeId ? _self.identificationTypeId : identificationTypeId // ignore: cast_nullable_to_non_nullable
as String?,identificationTypeName: null == identificationTypeName ? _self.identificationTypeName : identificationTypeName // ignore: cast_nullable_to_non_nullable
as String,mobileNumber: null == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on
