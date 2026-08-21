// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'password_reset_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$PasswordResetState {

 int get step; String get email; bool get isLoading; String? get error; String? get errorCode; int get resendCooldownSeconds; bool get justCodeSent; bool get justReset;
/// Create a copy of PasswordResetState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PasswordResetStateCopyWith<PasswordResetState> get copyWith => _$PasswordResetStateCopyWithImpl<PasswordResetState>(this as PasswordResetState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PasswordResetState&&(identical(other.step, step) || other.step == step)&&(identical(other.email, email) || other.email == email)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.error, error) || other.error == error)&&(identical(other.errorCode, errorCode) || other.errorCode == errorCode)&&(identical(other.resendCooldownSeconds, resendCooldownSeconds) || other.resendCooldownSeconds == resendCooldownSeconds)&&(identical(other.justCodeSent, justCodeSent) || other.justCodeSent == justCodeSent)&&(identical(other.justReset, justReset) || other.justReset == justReset));
}


@override
int get hashCode => Object.hash(runtimeType,step,email,isLoading,error,errorCode,resendCooldownSeconds,justCodeSent,justReset);

@override
String toString() {
  return 'PasswordResetState(step: $step, email: $email, isLoading: $isLoading, error: $error, errorCode: $errorCode, resendCooldownSeconds: $resendCooldownSeconds, justCodeSent: $justCodeSent, justReset: $justReset)';
}


}

/// @nodoc
abstract mixin class $PasswordResetStateCopyWith<$Res>  {
  factory $PasswordResetStateCopyWith(PasswordResetState value, $Res Function(PasswordResetState) _then) = _$PasswordResetStateCopyWithImpl;
@useResult
$Res call({
 int step, String email, bool isLoading, String? error, String? errorCode, int resendCooldownSeconds, bool justCodeSent, bool justReset
});




}
/// @nodoc
class _$PasswordResetStateCopyWithImpl<$Res>
    implements $PasswordResetStateCopyWith<$Res> {
  _$PasswordResetStateCopyWithImpl(this._self, this._then);

  final PasswordResetState _self;
  final $Res Function(PasswordResetState) _then;

/// Create a copy of PasswordResetState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? step = null,Object? email = null,Object? isLoading = null,Object? error = freezed,Object? errorCode = freezed,Object? resendCooldownSeconds = null,Object? justCodeSent = null,Object? justReset = null,}) {
  return _then(_self.copyWith(
step: null == step ? _self.step : step // ignore: cast_nullable_to_non_nullable
as int,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,errorCode: freezed == errorCode ? _self.errorCode : errorCode // ignore: cast_nullable_to_non_nullable
as String?,resendCooldownSeconds: null == resendCooldownSeconds ? _self.resendCooldownSeconds : resendCooldownSeconds // ignore: cast_nullable_to_non_nullable
as int,justCodeSent: null == justCodeSent ? _self.justCodeSent : justCodeSent // ignore: cast_nullable_to_non_nullable
as bool,justReset: null == justReset ? _self.justReset : justReset // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [PasswordResetState].
extension PasswordResetStatePatterns on PasswordResetState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PasswordResetState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PasswordResetState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PasswordResetState value)  $default,){
final _that = this;
switch (_that) {
case _PasswordResetState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PasswordResetState value)?  $default,){
final _that = this;
switch (_that) {
case _PasswordResetState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( int step,  String email,  bool isLoading,  String? error,  String? errorCode,  int resendCooldownSeconds,  bool justCodeSent,  bool justReset)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PasswordResetState() when $default != null:
return $default(_that.step,_that.email,_that.isLoading,_that.error,_that.errorCode,_that.resendCooldownSeconds,_that.justCodeSent,_that.justReset);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( int step,  String email,  bool isLoading,  String? error,  String? errorCode,  int resendCooldownSeconds,  bool justCodeSent,  bool justReset)  $default,) {final _that = this;
switch (_that) {
case _PasswordResetState():
return $default(_that.step,_that.email,_that.isLoading,_that.error,_that.errorCode,_that.resendCooldownSeconds,_that.justCodeSent,_that.justReset);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( int step,  String email,  bool isLoading,  String? error,  String? errorCode,  int resendCooldownSeconds,  bool justCodeSent,  bool justReset)?  $default,) {final _that = this;
switch (_that) {
case _PasswordResetState() when $default != null:
return $default(_that.step,_that.email,_that.isLoading,_that.error,_that.errorCode,_that.resendCooldownSeconds,_that.justCodeSent,_that.justReset);case _:
  return null;

}
}

}

/// @nodoc


class _PasswordResetState implements PasswordResetState {
  const _PasswordResetState({this.step = 1, this.email = '', this.isLoading = false, this.error, this.errorCode, this.resendCooldownSeconds = 0, this.justCodeSent = false, this.justReset = false});
  

@override@JsonKey() final  int step;
@override@JsonKey() final  String email;
@override@JsonKey() final  bool isLoading;
@override final  String? error;
@override final  String? errorCode;
@override@JsonKey() final  int resendCooldownSeconds;
@override@JsonKey() final  bool justCodeSent;
@override@JsonKey() final  bool justReset;

/// Create a copy of PasswordResetState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PasswordResetStateCopyWith<_PasswordResetState> get copyWith => __$PasswordResetStateCopyWithImpl<_PasswordResetState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _PasswordResetState&&(identical(other.step, step) || other.step == step)&&(identical(other.email, email) || other.email == email)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.error, error) || other.error == error)&&(identical(other.errorCode, errorCode) || other.errorCode == errorCode)&&(identical(other.resendCooldownSeconds, resendCooldownSeconds) || other.resendCooldownSeconds == resendCooldownSeconds)&&(identical(other.justCodeSent, justCodeSent) || other.justCodeSent == justCodeSent)&&(identical(other.justReset, justReset) || other.justReset == justReset));
}


@override
int get hashCode => Object.hash(runtimeType,step,email,isLoading,error,errorCode,resendCooldownSeconds,justCodeSent,justReset);

@override
String toString() {
  return 'PasswordResetState(step: $step, email: $email, isLoading: $isLoading, error: $error, errorCode: $errorCode, resendCooldownSeconds: $resendCooldownSeconds, justCodeSent: $justCodeSent, justReset: $justReset)';
}


}

/// @nodoc
abstract mixin class _$PasswordResetStateCopyWith<$Res> implements $PasswordResetStateCopyWith<$Res> {
  factory _$PasswordResetStateCopyWith(_PasswordResetState value, $Res Function(_PasswordResetState) _then) = __$PasswordResetStateCopyWithImpl;
@override @useResult
$Res call({
 int step, String email, bool isLoading, String? error, String? errorCode, int resendCooldownSeconds, bool justCodeSent, bool justReset
});




}
/// @nodoc
class __$PasswordResetStateCopyWithImpl<$Res>
    implements _$PasswordResetStateCopyWith<$Res> {
  __$PasswordResetStateCopyWithImpl(this._self, this._then);

  final _PasswordResetState _self;
  final $Res Function(_PasswordResetState) _then;

/// Create a copy of PasswordResetState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? step = null,Object? email = null,Object? isLoading = null,Object? error = freezed,Object? errorCode = freezed,Object? resendCooldownSeconds = null,Object? justCodeSent = null,Object? justReset = null,}) {
  return _then(_PasswordResetState(
step: null == step ? _self.step : step // ignore: cast_nullable_to_non_nullable
as int,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,errorCode: freezed == errorCode ? _self.errorCode : errorCode // ignore: cast_nullable_to_non_nullable
as String?,resendCooldownSeconds: null == resendCooldownSeconds ? _self.resendCooldownSeconds : resendCooldownSeconds // ignore: cast_nullable_to_non_nullable
as int,justCodeSent: null == justCodeSent ? _self.justCodeSent : justCodeSent // ignore: cast_nullable_to_non_nullable
as bool,justReset: null == justReset ? _self.justReset : justReset // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
