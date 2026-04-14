// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'emergency_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$EmergencyState {

 bool get isEditing; bool get isSaving; String? get saveError; bool get justSaved;
/// Create a copy of EmergencyState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$EmergencyStateCopyWith<EmergencyState> get copyWith => _$EmergencyStateCopyWithImpl<EmergencyState>(this as EmergencyState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is EmergencyState&&(identical(other.isEditing, isEditing) || other.isEditing == isEditing)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved));
}


@override
int get hashCode => Object.hash(runtimeType,isEditing,isSaving,saveError,justSaved);

@override
String toString() {
  return 'EmergencyState(isEditing: $isEditing, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved)';
}


}

/// @nodoc
abstract mixin class $EmergencyStateCopyWith<$Res>  {
  factory $EmergencyStateCopyWith(EmergencyState value, $Res Function(EmergencyState) _then) = _$EmergencyStateCopyWithImpl;
@useResult
$Res call({
 bool isEditing, bool isSaving, String? saveError, bool justSaved
});




}
/// @nodoc
class _$EmergencyStateCopyWithImpl<$Res>
    implements $EmergencyStateCopyWith<$Res> {
  _$EmergencyStateCopyWithImpl(this._self, this._then);

  final EmergencyState _self;
  final $Res Function(EmergencyState) _then;

/// Create a copy of EmergencyState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isEditing = null,Object? isSaving = null,Object? saveError = freezed,Object? justSaved = null,}) {
  return _then(_self.copyWith(
isEditing: null == isEditing ? _self.isEditing : isEditing // ignore: cast_nullable_to_non_nullable
as bool,isSaving: null == isSaving ? _self.isSaving : isSaving // ignore: cast_nullable_to_non_nullable
as bool,saveError: freezed == saveError ? _self.saveError : saveError // ignore: cast_nullable_to_non_nullable
as String?,justSaved: null == justSaved ? _self.justSaved : justSaved // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [EmergencyState].
extension EmergencyStatePatterns on EmergencyState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _EmergencyState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _EmergencyState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _EmergencyState value)  $default,){
final _that = this;
switch (_that) {
case _EmergencyState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _EmergencyState value)?  $default,){
final _that = this;
switch (_that) {
case _EmergencyState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isEditing,  bool isSaving,  String? saveError,  bool justSaved)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _EmergencyState() when $default != null:
return $default(_that.isEditing,_that.isSaving,_that.saveError,_that.justSaved);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isEditing,  bool isSaving,  String? saveError,  bool justSaved)  $default,) {final _that = this;
switch (_that) {
case _EmergencyState():
return $default(_that.isEditing,_that.isSaving,_that.saveError,_that.justSaved);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isEditing,  bool isSaving,  String? saveError,  bool justSaved)?  $default,) {final _that = this;
switch (_that) {
case _EmergencyState() when $default != null:
return $default(_that.isEditing,_that.isSaving,_that.saveError,_that.justSaved);case _:
  return null;

}
}

}

/// @nodoc


class _EmergencyState implements EmergencyState {
  const _EmergencyState({this.isEditing = false, this.isSaving = false, this.saveError, this.justSaved = false});
  

@override@JsonKey() final  bool isEditing;
@override@JsonKey() final  bool isSaving;
@override final  String? saveError;
@override@JsonKey() final  bool justSaved;

/// Create a copy of EmergencyState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$EmergencyStateCopyWith<_EmergencyState> get copyWith => __$EmergencyStateCopyWithImpl<_EmergencyState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _EmergencyState&&(identical(other.isEditing, isEditing) || other.isEditing == isEditing)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved));
}


@override
int get hashCode => Object.hash(runtimeType,isEditing,isSaving,saveError,justSaved);

@override
String toString() {
  return 'EmergencyState(isEditing: $isEditing, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved)';
}


}

/// @nodoc
abstract mixin class _$EmergencyStateCopyWith<$Res> implements $EmergencyStateCopyWith<$Res> {
  factory _$EmergencyStateCopyWith(_EmergencyState value, $Res Function(_EmergencyState) _then) = __$EmergencyStateCopyWithImpl;
@override @useResult
$Res call({
 bool isEditing, bool isSaving, String? saveError, bool justSaved
});




}
/// @nodoc
class __$EmergencyStateCopyWithImpl<$Res>
    implements _$EmergencyStateCopyWith<$Res> {
  __$EmergencyStateCopyWithImpl(this._self, this._then);

  final _EmergencyState _self;
  final $Res Function(_EmergencyState) _then;

/// Create a copy of EmergencyState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isEditing = null,Object? isSaving = null,Object? saveError = freezed,Object? justSaved = null,}) {
  return _then(_EmergencyState(
isEditing: null == isEditing ? _self.isEditing : isEditing // ignore: cast_nullable_to_non_nullable
as bool,isSaving: null == isSaving ? _self.isSaving : isSaving // ignore: cast_nullable_to_non_nullable
as bool,saveError: freezed == saveError ? _self.saveError : saveError // ignore: cast_nullable_to_non_nullable
as String?,justSaved: null == justSaved ? _self.justSaved : justSaved // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
