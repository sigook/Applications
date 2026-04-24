// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'account_settings_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$AccountSettingsState {

 bool get isChangingEmail; bool get isSavingEmail; String? get emailError; bool get justChangedEmail;
/// Create a copy of AccountSettingsState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AccountSettingsStateCopyWith<AccountSettingsState> get copyWith => _$AccountSettingsStateCopyWithImpl<AccountSettingsState>(this as AccountSettingsState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AccountSettingsState&&(identical(other.isChangingEmail, isChangingEmail) || other.isChangingEmail == isChangingEmail)&&(identical(other.isSavingEmail, isSavingEmail) || other.isSavingEmail == isSavingEmail)&&(identical(other.emailError, emailError) || other.emailError == emailError)&&(identical(other.justChangedEmail, justChangedEmail) || other.justChangedEmail == justChangedEmail));
}


@override
int get hashCode => Object.hash(runtimeType,isChangingEmail,isSavingEmail,emailError,justChangedEmail);

@override
String toString() {
  return 'AccountSettingsState(isChangingEmail: $isChangingEmail, isSavingEmail: $isSavingEmail, emailError: $emailError, justChangedEmail: $justChangedEmail)';
}


}

/// @nodoc
abstract mixin class $AccountSettingsStateCopyWith<$Res>  {
  factory $AccountSettingsStateCopyWith(AccountSettingsState value, $Res Function(AccountSettingsState) _then) = _$AccountSettingsStateCopyWithImpl;
@useResult
$Res call({
 bool isChangingEmail, bool isSavingEmail, String? emailError, bool justChangedEmail
});




}
/// @nodoc
class _$AccountSettingsStateCopyWithImpl<$Res>
    implements $AccountSettingsStateCopyWith<$Res> {
  _$AccountSettingsStateCopyWithImpl(this._self, this._then);

  final AccountSettingsState _self;
  final $Res Function(AccountSettingsState) _then;

/// Create a copy of AccountSettingsState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isChangingEmail = null,Object? isSavingEmail = null,Object? emailError = freezed,Object? justChangedEmail = null,}) {
  return _then(_self.copyWith(
isChangingEmail: null == isChangingEmail ? _self.isChangingEmail : isChangingEmail // ignore: cast_nullable_to_non_nullable
as bool,isSavingEmail: null == isSavingEmail ? _self.isSavingEmail : isSavingEmail // ignore: cast_nullable_to_non_nullable
as bool,emailError: freezed == emailError ? _self.emailError : emailError // ignore: cast_nullable_to_non_nullable
as String?,justChangedEmail: null == justChangedEmail ? _self.justChangedEmail : justChangedEmail // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [AccountSettingsState].
extension AccountSettingsStatePatterns on AccountSettingsState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AccountSettingsState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AccountSettingsState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AccountSettingsState value)  $default,){
final _that = this;
switch (_that) {
case _AccountSettingsState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AccountSettingsState value)?  $default,){
final _that = this;
switch (_that) {
case _AccountSettingsState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isChangingEmail,  bool isSavingEmail,  String? emailError,  bool justChangedEmail)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AccountSettingsState() when $default != null:
return $default(_that.isChangingEmail,_that.isSavingEmail,_that.emailError,_that.justChangedEmail);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isChangingEmail,  bool isSavingEmail,  String? emailError,  bool justChangedEmail)  $default,) {final _that = this;
switch (_that) {
case _AccountSettingsState():
return $default(_that.isChangingEmail,_that.isSavingEmail,_that.emailError,_that.justChangedEmail);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isChangingEmail,  bool isSavingEmail,  String? emailError,  bool justChangedEmail)?  $default,) {final _that = this;
switch (_that) {
case _AccountSettingsState() when $default != null:
return $default(_that.isChangingEmail,_that.isSavingEmail,_that.emailError,_that.justChangedEmail);case _:
  return null;

}
}

}

/// @nodoc


class _AccountSettingsState implements AccountSettingsState {
  const _AccountSettingsState({this.isChangingEmail = false, this.isSavingEmail = false, this.emailError, this.justChangedEmail = false});
  

@override@JsonKey() final  bool isChangingEmail;
@override@JsonKey() final  bool isSavingEmail;
@override final  String? emailError;
@override@JsonKey() final  bool justChangedEmail;

/// Create a copy of AccountSettingsState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AccountSettingsStateCopyWith<_AccountSettingsState> get copyWith => __$AccountSettingsStateCopyWithImpl<_AccountSettingsState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AccountSettingsState&&(identical(other.isChangingEmail, isChangingEmail) || other.isChangingEmail == isChangingEmail)&&(identical(other.isSavingEmail, isSavingEmail) || other.isSavingEmail == isSavingEmail)&&(identical(other.emailError, emailError) || other.emailError == emailError)&&(identical(other.justChangedEmail, justChangedEmail) || other.justChangedEmail == justChangedEmail));
}


@override
int get hashCode => Object.hash(runtimeType,isChangingEmail,isSavingEmail,emailError,justChangedEmail);

@override
String toString() {
  return 'AccountSettingsState(isChangingEmail: $isChangingEmail, isSavingEmail: $isSavingEmail, emailError: $emailError, justChangedEmail: $justChangedEmail)';
}


}

/// @nodoc
abstract mixin class _$AccountSettingsStateCopyWith<$Res> implements $AccountSettingsStateCopyWith<$Res> {
  factory _$AccountSettingsStateCopyWith(_AccountSettingsState value, $Res Function(_AccountSettingsState) _then) = __$AccountSettingsStateCopyWithImpl;
@override @useResult
$Res call({
 bool isChangingEmail, bool isSavingEmail, String? emailError, bool justChangedEmail
});




}
/// @nodoc
class __$AccountSettingsStateCopyWithImpl<$Res>
    implements _$AccountSettingsStateCopyWith<$Res> {
  __$AccountSettingsStateCopyWithImpl(this._self, this._then);

  final _AccountSettingsState _self;
  final $Res Function(_AccountSettingsState) _then;

/// Create a copy of AccountSettingsState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isChangingEmail = null,Object? isSavingEmail = null,Object? emailError = freezed,Object? justChangedEmail = null,}) {
  return _then(_AccountSettingsState(
isChangingEmail: null == isChangingEmail ? _self.isChangingEmail : isChangingEmail // ignore: cast_nullable_to_non_nullable
as bool,isSavingEmail: null == isSavingEmail ? _self.isSavingEmail : isSavingEmail // ignore: cast_nullable_to_non_nullable
as bool,emailError: freezed == emailError ? _self.emailError : emailError // ignore: cast_nullable_to_non_nullable
as String?,justChangedEmail: null == justChangedEmail ? _self.justChangedEmail : justChangedEmail // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
