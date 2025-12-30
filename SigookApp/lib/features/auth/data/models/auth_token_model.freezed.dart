// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'auth_token_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AuthTokenModel {

 String? get accessToken; String? get idToken; String? get refreshToken; DateTime? get expirationDateTime; String? get tokenType; List<String>? get scopes; UserInfoModel? get userInfo;
/// Create a copy of AuthTokenModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AuthTokenModelCopyWith<AuthTokenModel> get copyWith => _$AuthTokenModelCopyWithImpl<AuthTokenModel>(this as AuthTokenModel, _$identity);

  /// Serializes this AuthTokenModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AuthTokenModel&&super == other&&(identical(other.accessToken, accessToken) || other.accessToken == accessToken)&&(identical(other.idToken, idToken) || other.idToken == idToken)&&(identical(other.refreshToken, refreshToken) || other.refreshToken == refreshToken)&&(identical(other.expirationDateTime, expirationDateTime) || other.expirationDateTime == expirationDateTime)&&(identical(other.tokenType, tokenType) || other.tokenType == tokenType)&&const DeepCollectionEquality().equals(other.scopes, scopes)&&(identical(other.userInfo, userInfo) || other.userInfo == userInfo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,super.hashCode,accessToken,idToken,refreshToken,expirationDateTime,tokenType,const DeepCollectionEquality().hash(scopes),userInfo);



}

/// @nodoc
abstract mixin class $AuthTokenModelCopyWith<$Res>  {
  factory $AuthTokenModelCopyWith(AuthTokenModel value, $Res Function(AuthTokenModel) _then) = _$AuthTokenModelCopyWithImpl;
@useResult
$Res call({
 String? accessToken, String? idToken, String? refreshToken, DateTime? expirationDateTime, String? tokenType, List<String>? scopes, UserInfoModel? userInfo
});




}
/// @nodoc
class _$AuthTokenModelCopyWithImpl<$Res>
    implements $AuthTokenModelCopyWith<$Res> {
  _$AuthTokenModelCopyWithImpl(this._self, this._then);

  final AuthTokenModel _self;
  final $Res Function(AuthTokenModel) _then;

/// Create a copy of AuthTokenModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? accessToken = freezed,Object? idToken = freezed,Object? refreshToken = freezed,Object? expirationDateTime = freezed,Object? tokenType = freezed,Object? scopes = freezed,Object? userInfo = freezed,}) {
  return _then(_self.copyWith(
accessToken: freezed == accessToken ? _self.accessToken : accessToken // ignore: cast_nullable_to_non_nullable
as String?,idToken: freezed == idToken ? _self.idToken : idToken // ignore: cast_nullable_to_non_nullable
as String?,refreshToken: freezed == refreshToken ? _self.refreshToken : refreshToken // ignore: cast_nullable_to_non_nullable
as String?,expirationDateTime: freezed == expirationDateTime ? _self.expirationDateTime : expirationDateTime // ignore: cast_nullable_to_non_nullable
as DateTime?,tokenType: freezed == tokenType ? _self.tokenType : tokenType // ignore: cast_nullable_to_non_nullable
as String?,scopes: freezed == scopes ? _self.scopes : scopes // ignore: cast_nullable_to_non_nullable
as List<String>?,userInfo: freezed == userInfo ? _self.userInfo : userInfo // ignore: cast_nullable_to_non_nullable
as UserInfoModel?,
  ));
}

}


/// Adds pattern-matching-related methods to [AuthTokenModel].
extension AuthTokenModelPatterns on AuthTokenModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AuthTokenModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AuthTokenModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AuthTokenModel value)  $default,){
final _that = this;
switch (_that) {
case _AuthTokenModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AuthTokenModel value)?  $default,){
final _that = this;
switch (_that) {
case _AuthTokenModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? accessToken,  String? idToken,  String? refreshToken,  DateTime? expirationDateTime,  String? tokenType,  List<String>? scopes,  UserInfoModel? userInfo)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AuthTokenModel() when $default != null:
return $default(_that.accessToken,_that.idToken,_that.refreshToken,_that.expirationDateTime,_that.tokenType,_that.scopes,_that.userInfo);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? accessToken,  String? idToken,  String? refreshToken,  DateTime? expirationDateTime,  String? tokenType,  List<String>? scopes,  UserInfoModel? userInfo)  $default,) {final _that = this;
switch (_that) {
case _AuthTokenModel():
return $default(_that.accessToken,_that.idToken,_that.refreshToken,_that.expirationDateTime,_that.tokenType,_that.scopes,_that.userInfo);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? accessToken,  String? idToken,  String? refreshToken,  DateTime? expirationDateTime,  String? tokenType,  List<String>? scopes,  UserInfoModel? userInfo)?  $default,) {final _that = this;
switch (_that) {
case _AuthTokenModel() when $default != null:
return $default(_that.accessToken,_that.idToken,_that.refreshToken,_that.expirationDateTime,_that.tokenType,_that.scopes,_that.userInfo);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AuthTokenModel extends AuthTokenModel {
  const _AuthTokenModel({this.accessToken, this.idToken, this.refreshToken, this.expirationDateTime, this.tokenType, final  List<String>? scopes, this.userInfo}): _scopes = scopes,super._();
  factory _AuthTokenModel.fromJson(Map<String, dynamic> json) => _$AuthTokenModelFromJson(json);

@override final  String? accessToken;
@override final  String? idToken;
@override final  String? refreshToken;
@override final  DateTime? expirationDateTime;
@override final  String? tokenType;
 final  List<String>? _scopes;
@override List<String>? get scopes {
  final value = _scopes;
  if (value == null) return null;
  if (_scopes is EqualUnmodifiableListView) return _scopes;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(value);
}

@override final  UserInfoModel? userInfo;

/// Create a copy of AuthTokenModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AuthTokenModelCopyWith<_AuthTokenModel> get copyWith => __$AuthTokenModelCopyWithImpl<_AuthTokenModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AuthTokenModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AuthTokenModel&&super == other&&(identical(other.accessToken, accessToken) || other.accessToken == accessToken)&&(identical(other.idToken, idToken) || other.idToken == idToken)&&(identical(other.refreshToken, refreshToken) || other.refreshToken == refreshToken)&&(identical(other.expirationDateTime, expirationDateTime) || other.expirationDateTime == expirationDateTime)&&(identical(other.tokenType, tokenType) || other.tokenType == tokenType)&&const DeepCollectionEquality().equals(other._scopes, _scopes)&&(identical(other.userInfo, userInfo) || other.userInfo == userInfo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,super.hashCode,accessToken,idToken,refreshToken,expirationDateTime,tokenType,const DeepCollectionEquality().hash(_scopes),userInfo);



}

/// @nodoc
abstract mixin class _$AuthTokenModelCopyWith<$Res> implements $AuthTokenModelCopyWith<$Res> {
  factory _$AuthTokenModelCopyWith(_AuthTokenModel value, $Res Function(_AuthTokenModel) _then) = __$AuthTokenModelCopyWithImpl;
@override @useResult
$Res call({
 String? accessToken, String? idToken, String? refreshToken, DateTime? expirationDateTime, String? tokenType, List<String>? scopes, UserInfoModel? userInfo
});




}
/// @nodoc
class __$AuthTokenModelCopyWithImpl<$Res>
    implements _$AuthTokenModelCopyWith<$Res> {
  __$AuthTokenModelCopyWithImpl(this._self, this._then);

  final _AuthTokenModel _self;
  final $Res Function(_AuthTokenModel) _then;

/// Create a copy of AuthTokenModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? accessToken = freezed,Object? idToken = freezed,Object? refreshToken = freezed,Object? expirationDateTime = freezed,Object? tokenType = freezed,Object? scopes = freezed,Object? userInfo = freezed,}) {
  return _then(_AuthTokenModel(
accessToken: freezed == accessToken ? _self.accessToken : accessToken // ignore: cast_nullable_to_non_nullable
as String?,idToken: freezed == idToken ? _self.idToken : idToken // ignore: cast_nullable_to_non_nullable
as String?,refreshToken: freezed == refreshToken ? _self.refreshToken : refreshToken // ignore: cast_nullable_to_non_nullable
as String?,expirationDateTime: freezed == expirationDateTime ? _self.expirationDateTime : expirationDateTime // ignore: cast_nullable_to_non_nullable
as DateTime?,tokenType: freezed == tokenType ? _self.tokenType : tokenType // ignore: cast_nullable_to_non_nullable
as String?,scopes: freezed == scopes ? _self._scopes : scopes // ignore: cast_nullable_to_non_nullable
as List<String>?,userInfo: freezed == userInfo ? _self.userInfo : userInfo // ignore: cast_nullable_to_non_nullable
as UserInfoModel?,
  ));
}


}

// dart format on
