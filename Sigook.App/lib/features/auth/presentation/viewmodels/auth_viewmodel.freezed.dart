// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'auth_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$AuthState implements DiagnosticableTreeMixin {

 bool get isLoading; String? get error; String? get errorCode; AuthToken? get token; bool get isAuthenticated; bool get isRestoringSession; bool get sessionExpired; bool get justConfirmationSent;
/// Create a copy of AuthState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AuthStateCopyWith<AuthState> get copyWith => _$AuthStateCopyWithImpl<AuthState>(this as AuthState, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
  final _this = this as AuthState;
  properties
    ..add(DiagnosticsProperty('type', 'AuthState'))
    ..add(DiagnosticsProperty('isLoading', _this.isLoading))..add(DiagnosticsProperty('error', _this.error))..add(DiagnosticsProperty('errorCode', _this.errorCode))..add(DiagnosticsProperty('token', _this.token))..add(DiagnosticsProperty('isAuthenticated', _this.isAuthenticated))..add(DiagnosticsProperty('isRestoringSession', _this.isRestoringSession))..add(DiagnosticsProperty('sessionExpired', _this.sessionExpired))..add(DiagnosticsProperty('justConfirmationSent', _this.justConfirmationSent));
}

@override
bool operator ==(Object other) {
  final _this = this as AuthState;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AuthState&&(identical(other.isLoading, _this.isLoading) || other.isLoading == _this.isLoading)&&(identical(other.error, _this.error) || other.error == _this.error)&&(identical(other.errorCode, _this.errorCode) || other.errorCode == _this.errorCode)&&(identical(other.token, _this.token) || other.token == _this.token)&&(identical(other.isAuthenticated, _this.isAuthenticated) || other.isAuthenticated == _this.isAuthenticated)&&(identical(other.isRestoringSession, _this.isRestoringSession) || other.isRestoringSession == _this.isRestoringSession)&&(identical(other.sessionExpired, _this.sessionExpired) || other.sessionExpired == _this.sessionExpired)&&(identical(other.justConfirmationSent, _this.justConfirmationSent) || other.justConfirmationSent == _this.justConfirmationSent));
}


@override
int get hashCode {
  final _this = this as AuthState;
  return Object.hash(runtimeType,_this.isLoading,_this.error,_this.errorCode,_this.token,_this.isAuthenticated,_this.isRestoringSession,_this.sessionExpired,_this.justConfirmationSent);
}

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
  final _this = this as AuthState;
  return 'AuthState(isLoading: ${_this.isLoading}, error: ${_this.error}, errorCode: ${_this.errorCode}, token: ${_this.token}, isAuthenticated: ${_this.isAuthenticated}, isRestoringSession: ${_this.isRestoringSession}, sessionExpired: ${_this.sessionExpired}, justConfirmationSent: ${_this.justConfirmationSent})';
}


}

/// @nodoc
abstract mixin class $AuthStateCopyWith<$Res>  {
  factory $AuthStateCopyWith(AuthState value, $Res Function(AuthState) _then) = _$AuthStateCopyWithImpl;
@useResult
$Res call({
 bool isLoading, String? error, String? errorCode, AuthToken? token, bool isAuthenticated, bool isRestoringSession, bool sessionExpired, bool justConfirmationSent
});




}
/// @nodoc
class _$AuthStateCopyWithImpl<$Res>
    implements $AuthStateCopyWith<$Res> {
  _$AuthStateCopyWithImpl(this._self, this._then);

  final AuthState _self;
  final $Res Function(AuthState) _then;

/// Create a copy of AuthState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isLoading = null,Object? error = freezed,Object? errorCode = freezed,Object? token = freezed,Object? isAuthenticated = null,Object? isRestoringSession = null,Object? sessionExpired = null,Object? justConfirmationSent = null,}) {
  return _then(AuthState(
isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,errorCode: freezed == errorCode ? _self.errorCode : errorCode // ignore: cast_nullable_to_non_nullable
as String?,token: freezed == token ? _self.token : token // ignore: cast_nullable_to_non_nullable
as AuthToken?,isAuthenticated: null == isAuthenticated ? _self.isAuthenticated : isAuthenticated // ignore: cast_nullable_to_non_nullable
as bool,isRestoringSession: null == isRestoringSession ? _self.isRestoringSession : isRestoringSession // ignore: cast_nullable_to_non_nullable
as bool,sessionExpired: null == sessionExpired ? _self.sessionExpired : sessionExpired // ignore: cast_nullable_to_non_nullable
as bool,justConfirmationSent: null == justConfirmationSent ? _self.justConfirmationSent : justConfirmationSent // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [AuthState].
extension AuthStatePatterns on AuthState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AuthState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AuthState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AuthState value)  $default,){
final _that = this;
switch (_that) {
case _AuthState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AuthState value)?  $default,){
final _that = this;
switch (_that) {
case _AuthState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isLoading,  String? error,  String? errorCode,  AuthToken? token,  bool isAuthenticated,  bool isRestoringSession,  bool sessionExpired,  bool justConfirmationSent)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AuthState() when $default != null:
return $default(_that.isLoading,_that.error,_that.errorCode,_that.token,_that.isAuthenticated,_that.isRestoringSession,_that.sessionExpired,_that.justConfirmationSent);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isLoading,  String? error,  String? errorCode,  AuthToken? token,  bool isAuthenticated,  bool isRestoringSession,  bool sessionExpired,  bool justConfirmationSent)  $default,) {final _that = this;
switch (_that) {
case _AuthState():
return $default(_that.isLoading,_that.error,_that.errorCode,_that.token,_that.isAuthenticated,_that.isRestoringSession,_that.sessionExpired,_that.justConfirmationSent);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isLoading,  String? error,  String? errorCode,  AuthToken? token,  bool isAuthenticated,  bool isRestoringSession,  bool sessionExpired,  bool justConfirmationSent)?  $default,) {final _that = this;
switch (_that) {
case _AuthState() when $default != null:
return $default(_that.isLoading,_that.error,_that.errorCode,_that.token,_that.isAuthenticated,_that.isRestoringSession,_that.sessionExpired,_that.justConfirmationSent);case _:
  return null;

}
}

}

/// @nodoc


class _AuthState with DiagnosticableTreeMixin implements AuthState {
  const _AuthState({this.isLoading = false, this.error, this.errorCode, this.token, this.isAuthenticated = false, this.isRestoringSession = false, this.sessionExpired = false, this.justConfirmationSent = false});
  

@override@JsonKey() final  bool isLoading;
@override final  String? error;
@override final  String? errorCode;
@override final  AuthToken? token;
@override@JsonKey() final  bool isAuthenticated;
@override@JsonKey() final  bool isRestoringSession;
@override@JsonKey() final  bool sessionExpired;
@override@JsonKey() final  bool justConfirmationSent;

/// Create a copy of AuthState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AuthStateCopyWith<_AuthState> get copyWith => __$AuthStateCopyWithImpl<_AuthState>(this, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
    properties
    ..add(DiagnosticsProperty('type', 'AuthState'))
    ..add(DiagnosticsProperty('isLoading', isLoading))..add(DiagnosticsProperty('error', error))..add(DiagnosticsProperty('errorCode', errorCode))..add(DiagnosticsProperty('token', token))..add(DiagnosticsProperty('isAuthenticated', isAuthenticated))..add(DiagnosticsProperty('isRestoringSession', isRestoringSession))..add(DiagnosticsProperty('sessionExpired', sessionExpired))..add(DiagnosticsProperty('justConfirmationSent', justConfirmationSent));
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _AuthState&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.error, error) || other.error == error)&&(identical(other.errorCode, errorCode) || other.errorCode == errorCode)&&(identical(other.token, token) || other.token == token)&&(identical(other.isAuthenticated, isAuthenticated) || other.isAuthenticated == isAuthenticated)&&(identical(other.isRestoringSession, isRestoringSession) || other.isRestoringSession == isRestoringSession)&&(identical(other.sessionExpired, sessionExpired) || other.sessionExpired == sessionExpired)&&(identical(other.justConfirmationSent, justConfirmationSent) || other.justConfirmationSent == justConfirmationSent));
}


@override
int get hashCode {
    return Object.hash(runtimeType,isLoading,error,errorCode,token,isAuthenticated,isRestoringSession,sessionExpired,justConfirmationSent);
}

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
    return 'AuthState(isLoading: $isLoading, error: $error, errorCode: $errorCode, token: $token, isAuthenticated: $isAuthenticated, isRestoringSession: $isRestoringSession, sessionExpired: $sessionExpired, justConfirmationSent: $justConfirmationSent)';
}


}

/// @nodoc
abstract mixin class _$AuthStateCopyWith<$Res> implements $AuthStateCopyWith<$Res> {
  factory _$AuthStateCopyWith(_AuthState value, $Res Function(_AuthState) _then) = __$AuthStateCopyWithImpl;
@override @useResult
$Res call({
 bool isLoading, String? error, String? errorCode, AuthToken? token, bool isAuthenticated, bool isRestoringSession, bool sessionExpired, bool justConfirmationSent
});




}
/// @nodoc
class __$AuthStateCopyWithImpl<$Res>
    implements _$AuthStateCopyWith<$Res> {
  __$AuthStateCopyWithImpl(this._self, this._then);

  final _AuthState _self;
  final $Res Function(_AuthState) _then;

/// Create a copy of AuthState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isLoading = null,Object? error = freezed,Object? errorCode = freezed,Object? token = freezed,Object? isAuthenticated = null,Object? isRestoringSession = null,Object? sessionExpired = null,Object? justConfirmationSent = null,}) {
  return _then(_AuthState(
isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,errorCode: freezed == errorCode ? _self.errorCode : errorCode // ignore: cast_nullable_to_non_nullable
as String?,token: freezed == token ? _self.token : token // ignore: cast_nullable_to_non_nullable
as AuthToken?,isAuthenticated: null == isAuthenticated ? _self.isAuthenticated : isAuthenticated // ignore: cast_nullable_to_non_nullable
as bool,isRestoringSession: null == isRestoringSession ? _self.isRestoringSession : isRestoringSession // ignore: cast_nullable_to_non_nullable
as bool,sessionExpired: null == sessionExpired ? _self.sessionExpired : sessionExpired // ignore: cast_nullable_to_non_nullable
as bool,justConfirmationSent: null == justConfirmationSent ? _self.justConfirmationSent : justConfirmationSent // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
