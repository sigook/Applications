// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'profile_image_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$ProfileImageState implements DiagnosticableTreeMixin {

 bool get isUploading; String? get uploadError; bool get justUploaded;
/// Create a copy of ProfileImageState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProfileImageStateCopyWith<ProfileImageState> get copyWith => _$ProfileImageStateCopyWithImpl<ProfileImageState>(this as ProfileImageState, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
  properties
    ..add(DiagnosticsProperty('type', 'ProfileImageState'))
    ..add(DiagnosticsProperty('isUploading', isUploading))..add(DiagnosticsProperty('uploadError', uploadError))..add(DiagnosticsProperty('justUploaded', justUploaded));
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProfileImageState&&(identical(other.isUploading, isUploading) || other.isUploading == isUploading)&&(identical(other.uploadError, uploadError) || other.uploadError == uploadError)&&(identical(other.justUploaded, justUploaded) || other.justUploaded == justUploaded));
}


@override
int get hashCode => Object.hash(runtimeType,isUploading,uploadError,justUploaded);

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
  return 'ProfileImageState(isUploading: $isUploading, uploadError: $uploadError, justUploaded: $justUploaded)';
}


}

/// @nodoc
abstract mixin class $ProfileImageStateCopyWith<$Res>  {
  factory $ProfileImageStateCopyWith(ProfileImageState value, $Res Function(ProfileImageState) _then) = _$ProfileImageStateCopyWithImpl;
@useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded
});




}
/// @nodoc
class _$ProfileImageStateCopyWithImpl<$Res>
    implements $ProfileImageStateCopyWith<$Res> {
  _$ProfileImageStateCopyWithImpl(this._self, this._then);

  final ProfileImageState _self;
  final $Res Function(ProfileImageState) _then;

/// Create a copy of ProfileImageState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,}) {
  return _then(_self.copyWith(
isUploading: null == isUploading ? _self.isUploading : isUploading // ignore: cast_nullable_to_non_nullable
as bool,uploadError: freezed == uploadError ? _self.uploadError : uploadError // ignore: cast_nullable_to_non_nullable
as String?,justUploaded: null == justUploaded ? _self.justUploaded : justUploaded // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [ProfileImageState].
extension ProfileImageStatePatterns on ProfileImageState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProfileImageState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProfileImageState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProfileImageState value)  $default,){
final _that = this;
switch (_that) {
case _ProfileImageState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProfileImageState value)?  $default,){
final _that = this;
switch (_that) {
case _ProfileImageState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProfileImageState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded)  $default,) {final _that = this;
switch (_that) {
case _ProfileImageState():
return $default(_that.isUploading,_that.uploadError,_that.justUploaded);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isUploading,  String? uploadError,  bool justUploaded)?  $default,) {final _that = this;
switch (_that) {
case _ProfileImageState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded);case _:
  return null;

}
}

}

/// @nodoc


class _ProfileImageState with DiagnosticableTreeMixin implements ProfileImageState {
  const _ProfileImageState({this.isUploading = false, this.uploadError, this.justUploaded = false});
  

@override@JsonKey() final  bool isUploading;
@override final  String? uploadError;
@override@JsonKey() final  bool justUploaded;

/// Create a copy of ProfileImageState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProfileImageStateCopyWith<_ProfileImageState> get copyWith => __$ProfileImageStateCopyWithImpl<_ProfileImageState>(this, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
  properties
    ..add(DiagnosticsProperty('type', 'ProfileImageState'))
    ..add(DiagnosticsProperty('isUploading', isUploading))..add(DiagnosticsProperty('uploadError', uploadError))..add(DiagnosticsProperty('justUploaded', justUploaded));
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProfileImageState&&(identical(other.isUploading, isUploading) || other.isUploading == isUploading)&&(identical(other.uploadError, uploadError) || other.uploadError == uploadError)&&(identical(other.justUploaded, justUploaded) || other.justUploaded == justUploaded));
}


@override
int get hashCode => Object.hash(runtimeType,isUploading,uploadError,justUploaded);

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
  return 'ProfileImageState(isUploading: $isUploading, uploadError: $uploadError, justUploaded: $justUploaded)';
}


}

/// @nodoc
abstract mixin class _$ProfileImageStateCopyWith<$Res> implements $ProfileImageStateCopyWith<$Res> {
  factory _$ProfileImageStateCopyWith(_ProfileImageState value, $Res Function(_ProfileImageState) _then) = __$ProfileImageStateCopyWithImpl;
@override @useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded
});




}
/// @nodoc
class __$ProfileImageStateCopyWithImpl<$Res>
    implements _$ProfileImageStateCopyWith<$Res> {
  __$ProfileImageStateCopyWithImpl(this._self, this._then);

  final _ProfileImageState _self;
  final $Res Function(_ProfileImageState) _then;

/// Create a copy of ProfileImageState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,}) {
  return _then(_ProfileImageState(
isUploading: null == isUploading ? _self.isUploading : isUploading // ignore: cast_nullable_to_non_nullable
as bool,uploadError: freezed == uploadError ? _self.uploadError : uploadError // ignore: cast_nullable_to_non_nullable
as String?,justUploaded: null == justUploaded ? _self.justUploaded : justUploaded // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
