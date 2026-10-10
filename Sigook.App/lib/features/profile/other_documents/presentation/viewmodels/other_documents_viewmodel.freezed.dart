// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'other_documents_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$OtherDocumentsState {

 bool get isUploading; String? get uploadError; bool get justUploaded; bool get isDeleting; String? get deleteError; bool get justDeleted;
/// Create a copy of OtherDocumentsState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$OtherDocumentsStateCopyWith<OtherDocumentsState> get copyWith => _$OtherDocumentsStateCopyWithImpl<OtherDocumentsState>(this as OtherDocumentsState, _$identity);



@override
bool operator ==(Object other) {
  final _this = this as OtherDocumentsState;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is OtherDocumentsState&&(identical(other.isUploading, _this.isUploading) || other.isUploading == _this.isUploading)&&(identical(other.uploadError, _this.uploadError) || other.uploadError == _this.uploadError)&&(identical(other.justUploaded, _this.justUploaded) || other.justUploaded == _this.justUploaded)&&(identical(other.isDeleting, _this.isDeleting) || other.isDeleting == _this.isDeleting)&&(identical(other.deleteError, _this.deleteError) || other.deleteError == _this.deleteError)&&(identical(other.justDeleted, _this.justDeleted) || other.justDeleted == _this.justDeleted));
}


@override
int get hashCode {
  final _this = this as OtherDocumentsState;
  return Object.hash(runtimeType,_this.isUploading,_this.uploadError,_this.justUploaded,_this.isDeleting,_this.deleteError,_this.justDeleted);
}

@override
String toString() {
  final _this = this as OtherDocumentsState;
  return 'OtherDocumentsState(isUploading: ${_this.isUploading}, uploadError: ${_this.uploadError}, justUploaded: ${_this.justUploaded}, isDeleting: ${_this.isDeleting}, deleteError: ${_this.deleteError}, justDeleted: ${_this.justDeleted})';
}


}

/// @nodoc
abstract mixin class $OtherDocumentsStateCopyWith<$Res>  {
  factory $OtherDocumentsStateCopyWith(OtherDocumentsState value, $Res Function(OtherDocumentsState) _then) = _$OtherDocumentsStateCopyWithImpl;
@useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded, bool isDeleting, String? deleteError, bool justDeleted
});




}
/// @nodoc
class _$OtherDocumentsStateCopyWithImpl<$Res>
    implements $OtherDocumentsStateCopyWith<$Res> {
  _$OtherDocumentsStateCopyWithImpl(this._self, this._then);

  final OtherDocumentsState _self;
  final $Res Function(OtherDocumentsState) _then;

/// Create a copy of OtherDocumentsState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,Object? isDeleting = null,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(OtherDocumentsState(
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


/// Adds pattern-matching-related methods to [OtherDocumentsState].
extension OtherDocumentsStatePatterns on OtherDocumentsState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _OtherDocumentsState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _OtherDocumentsState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _OtherDocumentsState value)  $default,){
final _that = this;
switch (_that) {
case _OtherDocumentsState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _OtherDocumentsState value)?  $default,){
final _that = this;
switch (_that) {
case _OtherDocumentsState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _OtherDocumentsState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)  $default,) {final _that = this;
switch (_that) {
case _OtherDocumentsState():
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isUploading,  String? uploadError,  bool justUploaded,  bool isDeleting,  String? deleteError,  bool justDeleted)?  $default,) {final _that = this;
switch (_that) {
case _OtherDocumentsState() when $default != null:
return $default(_that.isUploading,_that.uploadError,_that.justUploaded,_that.isDeleting,_that.deleteError,_that.justDeleted);case _:
  return null;

}
}

}

/// @nodoc


class _OtherDocumentsState implements OtherDocumentsState {
  const _OtherDocumentsState({this.isUploading = false, this.uploadError, this.justUploaded = false, this.isDeleting = false, this.deleteError, this.justDeleted = false});
  

@override@JsonKey() final  bool isUploading;
@override final  String? uploadError;
@override@JsonKey() final  bool justUploaded;
@override@JsonKey() final  bool isDeleting;
@override final  String? deleteError;
@override@JsonKey() final  bool justDeleted;

/// Create a copy of OtherDocumentsState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$OtherDocumentsStateCopyWith<_OtherDocumentsState> get copyWith => __$OtherDocumentsStateCopyWithImpl<_OtherDocumentsState>(this, _$identity);



@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _OtherDocumentsState&&(identical(other.isUploading, isUploading) || other.isUploading == isUploading)&&(identical(other.uploadError, uploadError) || other.uploadError == uploadError)&&(identical(other.justUploaded, justUploaded) || other.justUploaded == justUploaded)&&(identical(other.isDeleting, isDeleting) || other.isDeleting == isDeleting)&&(identical(other.deleteError, deleteError) || other.deleteError == deleteError)&&(identical(other.justDeleted, justDeleted) || other.justDeleted == justDeleted));
}


@override
int get hashCode {
    return Object.hash(runtimeType,isUploading,uploadError,justUploaded,isDeleting,deleteError,justDeleted);
}

@override
String toString() {
    return 'OtherDocumentsState(isUploading: $isUploading, uploadError: $uploadError, justUploaded: $justUploaded, isDeleting: $isDeleting, deleteError: $deleteError, justDeleted: $justDeleted)';
}


}

/// @nodoc
abstract mixin class _$OtherDocumentsStateCopyWith<$Res> implements $OtherDocumentsStateCopyWith<$Res> {
  factory _$OtherDocumentsStateCopyWith(_OtherDocumentsState value, $Res Function(_OtherDocumentsState) _then) = __$OtherDocumentsStateCopyWithImpl;
@override @useResult
$Res call({
 bool isUploading, String? uploadError, bool justUploaded, bool isDeleting, String? deleteError, bool justDeleted
});




}
/// @nodoc
class __$OtherDocumentsStateCopyWithImpl<$Res>
    implements _$OtherDocumentsStateCopyWith<$Res> {
  __$OtherDocumentsStateCopyWithImpl(this._self, this._then);

  final _OtherDocumentsState _self;
  final $Res Function(_OtherDocumentsState) _then;

/// Create a copy of OtherDocumentsState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isUploading = null,Object? uploadError = freezed,Object? justUploaded = null,Object? isDeleting = null,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(_OtherDocumentsState(
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
