// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'licenses_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$LicensesState {

 bool get isUploading; String? get uploadError; bool get justUploaded; bool get isDeleting; String? get deleteError; bool get justDeleted;
/// Create a copy of LicensesState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$LicensesStateCopyWith<LicensesState> get copyWith => _$LicensesStateCopyWithImpl<LicensesState>(this as LicensesState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is LicensesState&&(identical(other.isUploading, isUploading) || other.isUploading == isUploading)&&(identical(other.uploadError, uploadError) || other.uploadError == uploadError)&&(identical(other.justUploaded, justUploaded) || other.justUploaded == justUploaded)&&(identical(other.isDeleting, isDeleting) || other.isDeleting == isDeleting)&&(identical(other.deleteError, deleteError) || other.deleteError == deleteError)&&(identical(other.justDeleted, justDeleted) || other.justDeleted == justDeleted));
}


@override
int get hashCode => Object.hash(runtimeType,isUploading,uploadError,justUploaded,isDeleting,deleteError,justDeleted);

@override
String toString() {
  return 'LicensesState(isUploading: $isUploading, uploadError: $uploadError, justUploaded: $justUploaded, isDeleting: $isDeleting, deleteError: $deleteError, justDeleted: $justDeleted)';
}


}

/// @nodoc
abstract mixin class $LicensesStateCopyWith<$Res>  {
  factory $LicensesStateCopyWith(LicensesState value, $Res Function(LicensesState) _then) = _$LicensesStateCopyWithImpl;
@useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded, bool isDeleting, String? deleteError, bool justDeleted
});




}
/// @nodoc
class _$LicensesStateCopyWithImpl<$Res>
    implements $LicensesStateCopyWith<$Res> {
  _$LicensesStateCopyWithImpl(this._self, this._then);

  final LicensesState _self;
  final $Res Function(LicensesState) _then;

/// Create a copy of LicensesState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,Object? isDeleting = null,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(_self.copyWith(
isUploading: null == isUploading ? _self.isUploading : isUploading // ignore: cast_nullable_to_non_nullable
as bool,uploadError: freezed == uploadError ? _self.uploadError : uploadError // ignore: cast_nullable_to_non_nullable
as String?,justUploaded: null == justUploaded ? _self.justUploaded : justUploaded // ignore: cast_nullable_to_non_nullable
as bool,isDeleting: null == isDeleting ? _self.isDeleting : isDeleting // ignore: cast_nullable_to_non_nullable
as bool,deleteError: freezed == deleteError ? _self.deleteError : deleteError // ignore: cast_nullable_to_non_nullable
as String?,justDeleted: null == justDeleted ? _self.justDeleted : justDeleted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [LicensesState].
extension LicensesStatePatterns on LicensesState {
@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _LicensesState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _LicensesState() when $default != null:
return $default(_that);case _:
  return orElse();

}
}

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _LicensesState value)  $default,){
final _that = this;
switch (_that) {
case _LicensesState():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
}

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _LicensesState value)?  $default,){
final _that = this;
switch (_that) {
case _LicensesState() when $default != null:
return $default(_that);case _:
  return null;

}
}

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _LicensesState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
  return orElse();

}
}

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)  $default,) {final _that = this;
switch (_that) {
case _LicensesState():
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
  throw StateError('Unexpected subclass');

}
}

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)?  $default,) {final _that = this;
switch (_that) {
case _LicensesState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
  return null;

}
}

}

/// @nodoc


class _LicensesState implements LicensesState {
  const _LicensesState({this.isUploading = false, this.uploadError, this.justUploaded = false, this.isDeleting = false, this.deleteError, this.justDeleted = false});


@override@JsonKey() final  bool isUploading;
@override final  String? uploadError;
@override@JsonKey() final  bool justUploaded;
@override@JsonKey() final  bool isDeleting;
@override final  String? deleteError;
@override@JsonKey() final  bool justDeleted;

/// Create a copy of LicensesState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$LicensesStateCopyWith<_LicensesState> get copyWith => __$LicensesStateCopyWithImpl<_LicensesState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _LicensesState&&(identical(other.isUploading, isUploading) || other.isUploading == isUploading)&&(identical(other.uploadError, uploadError) || other.uploadError == uploadError)&&(identical(other.justUploaded, justUploaded) || other.justUploaded == justUploaded)&&(identical(other.isDeleting, isDeleting) || other.isDeleting == isDeleting)&&(identical(other.deleteError, deleteError) || other.deleteError == deleteError)&&(identical(other.justDeleted, justDeleted) || other.justDeleted == justDeleted));
}


@override
int get hashCode => Object.hash(runtimeType,isUploading,uploadError,justUploaded,isDeleting,deleteError,justDeleted);

@override
String toString() {
  return 'LicensesState(isUploading: $isUploading, uploadError: $uploadError, justUploaded: $justUploaded, isDeleting: $isDeleting, deleteError: $deleteError, justDeleted: $justDeleted)';
}


}

/// @nodoc
abstract mixin class _$LicensesStateCopyWith<$Res> implements $LicensesStateCopyWith<$Res> {
  factory _$LicensesStateCopyWith(_LicensesState value, $Res Function(_LicensesState) _then) = __$LicensesStateCopyWithImpl;
@override @useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded, bool isDeleting, String? deleteError, bool justDeleted
});




}
/// @nodoc
class __$LicensesStateCopyWithImpl<$Res>
    implements _$LicensesStateCopyWith<$Res> {
  __$LicensesStateCopyWithImpl(this._self, this._then);

  final _LicensesState _self;
  final $Res Function(_LicensesState) _then;

/// Create a copy of LicensesState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,Object? isDeleting = null,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(_LicensesState(
isUploading: null == isUploading ? _self.isUploading : isUploading // ignore: cast_nullable_to_non_nullable
as bool,uploadError: freezed == uploadError ? _self.uploadError : uploadError // ignore: cast_nullable_to_non_nullable
as String?,justUploaded: null == justUploaded ? _self.justUploaded : justUploaded // ignore: cast_nullable_to_non_nullable
as bool,isDeleting: null == isDeleting ? _self.isDeleting : isDeleting // ignore: cast_nullable_to_non_nullable
as bool,deleteError: freezed == deleteError ? _self.deleteError : deleteError // ignore: cast_nullable_to_non_nullable
as String?,justDeleted: null == justDeleted ? _self.justDeleted : justDeleted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
