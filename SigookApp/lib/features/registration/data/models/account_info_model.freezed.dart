// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'account_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AccountInfoModel {

 String get email; String get password; String get confirmPassword; bool get termsAccepted;
/// Create a copy of AccountInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AccountInfoModelCopyWith<AccountInfoModel> get copyWith => _$AccountInfoModelCopyWithImpl<AccountInfoModel>(this as AccountInfoModel, _$identity);

  /// Serializes this AccountInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AccountInfoModel&&(identical(other.email, email) || other.email == email)&&(identical(other.password, password) || other.password == password)&&(identical(other.confirmPassword, confirmPassword) || other.confirmPassword == confirmPassword)&&(identical(other.termsAccepted, termsAccepted) || other.termsAccepted == termsAccepted));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email,password,confirmPassword,termsAccepted);

@override
String toString() {
  return 'AccountInfoModel(email: $email, password: $password, confirmPassword: $confirmPassword, termsAccepted: $termsAccepted)';
}


}

/// @nodoc
abstract mixin class $AccountInfoModelCopyWith<$Res>  {
  factory $AccountInfoModelCopyWith(AccountInfoModel value, $Res Function(AccountInfoModel) _then) = _$AccountInfoModelCopyWithImpl;
@useResult
$Res call({
 String email, String password, String confirmPassword, bool termsAccepted
});




}
/// @nodoc
class _$AccountInfoModelCopyWithImpl<$Res>
    implements $AccountInfoModelCopyWith<$Res> {
  _$AccountInfoModelCopyWithImpl(this._self, this._then);

  final AccountInfoModel _self;
  final $Res Function(AccountInfoModel) _then;

/// Create a copy of AccountInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? email = null,Object? password = null,Object? confirmPassword = null,Object? termsAccepted = null,}) {
  return _then(_self.copyWith(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,password: null == password ? _self.password : password // ignore: cast_nullable_to_non_nullable
as String,confirmPassword: null == confirmPassword ? _self.confirmPassword : confirmPassword // ignore: cast_nullable_to_non_nullable
as String,termsAccepted: null == termsAccepted ? _self.termsAccepted : termsAccepted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [AccountInfoModel].
extension AccountInfoModelPatterns on AccountInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AccountInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AccountInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AccountInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _AccountInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AccountInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _AccountInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String email,  String password,  String confirmPassword,  bool termsAccepted)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AccountInfoModel() when $default != null:
return $default(_that.email,_that.password,_that.confirmPassword,_that.termsAccepted);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String email,  String password,  String confirmPassword,  bool termsAccepted)  $default,) {final _that = this;
switch (_that) {
case _AccountInfoModel():
return $default(_that.email,_that.password,_that.confirmPassword,_that.termsAccepted);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String email,  String password,  String confirmPassword,  bool termsAccepted)?  $default,) {final _that = this;
switch (_that) {
case _AccountInfoModel() when $default != null:
return $default(_that.email,_that.password,_that.confirmPassword,_that.termsAccepted);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AccountInfoModel extends AccountInfoModel {
  const _AccountInfoModel({required this.email, required this.password, required this.confirmPassword, required this.termsAccepted}): super._();
  factory _AccountInfoModel.fromJson(Map<String, dynamic> json) => _$AccountInfoModelFromJson(json);

@override final  String email;
@override final  String password;
@override final  String confirmPassword;
@override final  bool termsAccepted;

/// Create a copy of AccountInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AccountInfoModelCopyWith<_AccountInfoModel> get copyWith => __$AccountInfoModelCopyWithImpl<_AccountInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AccountInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AccountInfoModel&&(identical(other.email, email) || other.email == email)&&(identical(other.password, password) || other.password == password)&&(identical(other.confirmPassword, confirmPassword) || other.confirmPassword == confirmPassword)&&(identical(other.termsAccepted, termsAccepted) || other.termsAccepted == termsAccepted));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email,password,confirmPassword,termsAccepted);

@override
String toString() {
  return 'AccountInfoModel(email: $email, password: $password, confirmPassword: $confirmPassword, termsAccepted: $termsAccepted)';
}


}

/// @nodoc
abstract mixin class _$AccountInfoModelCopyWith<$Res> implements $AccountInfoModelCopyWith<$Res> {
  factory _$AccountInfoModelCopyWith(_AccountInfoModel value, $Res Function(_AccountInfoModel) _then) = __$AccountInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String email, String password, String confirmPassword, bool termsAccepted
});




}
/// @nodoc
class __$AccountInfoModelCopyWithImpl<$Res>
    implements _$AccountInfoModelCopyWith<$Res> {
  __$AccountInfoModelCopyWithImpl(this._self, this._then);

  final _AccountInfoModel _self;
  final $Res Function(_AccountInfoModel) _then;

/// Create a copy of AccountInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? email = null,Object? password = null,Object? confirmPassword = null,Object? termsAccepted = null,}) {
  return _then(_AccountInfoModel(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,password: null == password ? _self.password : password // ignore: cast_nullable_to_non_nullable
as String,confirmPassword: null == confirmPassword ? _self.confirmPassword : confirmPassword // ignore: cast_nullable_to_non_nullable
as String,termsAccepted: null == termsAccepted ? _self.termsAccepted : termsAccepted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
