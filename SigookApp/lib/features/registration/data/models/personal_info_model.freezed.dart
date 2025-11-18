// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'personal_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$PersonalInfoModel {

 String get firstName; String get lastName; DateTime get dateOfBirth; String? get genderId;// UUID from API (may be null)
 String get genderName;
/// Create a copy of PersonalInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PersonalInfoModelCopyWith<PersonalInfoModel> get copyWith => _$PersonalInfoModelCopyWithImpl<PersonalInfoModel>(this as PersonalInfoModel, _$identity);

  /// Serializes this PersonalInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PersonalInfoModel&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.genderId, genderId) || other.genderId == genderId)&&(identical(other.genderName, genderName) || other.genderName == genderName));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,firstName,lastName,dateOfBirth,genderId,genderName);

@override
String toString() {
  return 'PersonalInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, genderId: $genderId, genderName: $genderName)';
}


}

/// @nodoc
abstract mixin class $PersonalInfoModelCopyWith<$Res>  {
  factory $PersonalInfoModelCopyWith(PersonalInfoModel value, $Res Function(PersonalInfoModel) _then) = _$PersonalInfoModelCopyWithImpl;
@useResult
$Res call({
 String firstName, String lastName, DateTime dateOfBirth, String? genderId, String genderName
});




}
/// @nodoc
class _$PersonalInfoModelCopyWithImpl<$Res>
    implements $PersonalInfoModelCopyWith<$Res> {
  _$PersonalInfoModelCopyWithImpl(this._self, this._then);

  final PersonalInfoModel _self;
  final $Res Function(PersonalInfoModel) _then;

/// Create a copy of PersonalInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? firstName = null,Object? lastName = null,Object? dateOfBirth = null,Object? genderId = freezed,Object? genderName = null,}) {
  return _then(_self.copyWith(
firstName: null == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String,lastName: null == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String,dateOfBirth: null == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as DateTime,genderId: freezed == genderId ? _self.genderId : genderId // ignore: cast_nullable_to_non_nullable
as String?,genderName: null == genderName ? _self.genderName : genderName // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [PersonalInfoModel].
extension PersonalInfoModelPatterns on PersonalInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PersonalInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PersonalInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PersonalInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _PersonalInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PersonalInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _PersonalInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String firstName,  String lastName,  DateTime dateOfBirth,  String? genderId,  String genderName)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PersonalInfoModel() when $default != null:
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderName);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String firstName,  String lastName,  DateTime dateOfBirth,  String? genderId,  String genderName)  $default,) {final _that = this;
switch (_that) {
case _PersonalInfoModel():
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderName);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String firstName,  String lastName,  DateTime dateOfBirth,  String? genderId,  String genderName)?  $default,) {final _that = this;
switch (_that) {
case _PersonalInfoModel() when $default != null:
return $default(_that.firstName,_that.lastName,_that.dateOfBirth,_that.genderId,_that.genderName);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _PersonalInfoModel extends PersonalInfoModel {
  const _PersonalInfoModel({required this.firstName, required this.lastName, required this.dateOfBirth, this.genderId, required this.genderName}): super._();
  factory _PersonalInfoModel.fromJson(Map<String, dynamic> json) => _$PersonalInfoModelFromJson(json);

@override final  String firstName;
@override final  String lastName;
@override final  DateTime dateOfBirth;
@override final  String? genderId;
// UUID from API (may be null)
@override final  String genderName;

/// Create a copy of PersonalInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PersonalInfoModelCopyWith<_PersonalInfoModel> get copyWith => __$PersonalInfoModelCopyWithImpl<_PersonalInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$PersonalInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _PersonalInfoModel&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.genderId, genderId) || other.genderId == genderId)&&(identical(other.genderName, genderName) || other.genderName == genderName));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,firstName,lastName,dateOfBirth,genderId,genderName);

@override
String toString() {
  return 'PersonalInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, genderId: $genderId, genderName: $genderName)';
}


}

/// @nodoc
abstract mixin class _$PersonalInfoModelCopyWith<$Res> implements $PersonalInfoModelCopyWith<$Res> {
  factory _$PersonalInfoModelCopyWith(_PersonalInfoModel value, $Res Function(_PersonalInfoModel) _then) = __$PersonalInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String firstName, String lastName, DateTime dateOfBirth, String? genderId, String genderName
});




}
/// @nodoc
class __$PersonalInfoModelCopyWithImpl<$Res>
    implements _$PersonalInfoModelCopyWith<$Res> {
  __$PersonalInfoModelCopyWithImpl(this._self, this._then);

  final _PersonalInfoModel _self;
  final $Res Function(_PersonalInfoModel) _then;

/// Create a copy of PersonalInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? firstName = null,Object? lastName = null,Object? dateOfBirth = null,Object? genderId = freezed,Object? genderName = null,}) {
  return _then(_PersonalInfoModel(
firstName: null == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String,lastName: null == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String,dateOfBirth: null == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as DateTime,genderId: freezed == genderId ? _self.genderId : genderId // ignore: cast_nullable_to_non_nullable
as String?,genderName: null == genderName ? _self.genderName : genderName // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on
