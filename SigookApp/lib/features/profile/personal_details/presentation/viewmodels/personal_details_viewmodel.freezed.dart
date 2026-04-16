// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'personal_details_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$PersonalDetailsState {

 bool get isEditing; bool get isSaving; String? get saveError; bool get justSaved;
/// Create a copy of PersonalDetailsState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PersonalDetailsStateCopyWith<PersonalDetailsState> get copyWith => _$PersonalDetailsStateCopyWithImpl<PersonalDetailsState>(this as PersonalDetailsState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PersonalDetailsState&&(identical(other.isEditing, isEditing) || other.isEditing == isEditing)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved));
}


@override
int get hashCode => Object.hash(runtimeType,isEditing,isSaving,saveError,justSaved);

@override
String toString() {
  return 'PersonalDetailsState(isEditing: $isEditing, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved)';
}


}

/// @nodoc
abstract mixin class $PersonalDetailsStateCopyWith<$Res>  {
  factory $PersonalDetailsStateCopyWith(PersonalDetailsState value, $Res Function(PersonalDetailsState) _then) = _$PersonalDetailsStateCopyWithImpl;
@useResult
$Res call({
 bool isEditing, bool isSaving, String? saveError, bool justSaved
});




}
/// @nodoc
class _$PersonalDetailsStateCopyWithImpl<$Res>
    implements $PersonalDetailsStateCopyWith<$Res> {
  _$PersonalDetailsStateCopyWithImpl(this._self, this._then);

  final PersonalDetailsState _self;
  final $Res Function(PersonalDetailsState) _then;

/// Create a copy of PersonalDetailsState
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


/// Adds pattern-matching-related methods to [PersonalDetailsState].
extension PersonalDetailsStatePatterns on PersonalDetailsState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PersonalDetailsState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PersonalDetailsState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PersonalDetailsState value)  $default,){
final _that = this;
switch (_that) {
case _PersonalDetailsState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PersonalDetailsState value)?  $default,){
final _that = this;
switch (_that) {
case _PersonalDetailsState() when $default != null:
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
case _PersonalDetailsState() when $default != null:
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
case _PersonalDetailsState():
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
case _PersonalDetailsState() when $default != null:
return $default(_that.isEditing,_that.isSaving,_that.saveError,_that.justSaved);case _:
  return null;

}
}

}

/// @nodoc


class _PersonalDetailsState implements PersonalDetailsState {
  const _PersonalDetailsState({this.isEditing = false, this.isSaving = false, this.saveError, this.justSaved = false});
  

@override@JsonKey() final  bool isEditing;
@override@JsonKey() final  bool isSaving;
@override final  String? saveError;
@override@JsonKey() final  bool justSaved;

/// Create a copy of PersonalDetailsState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PersonalDetailsStateCopyWith<_PersonalDetailsState> get copyWith => __$PersonalDetailsStateCopyWithImpl<_PersonalDetailsState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _PersonalDetailsState&&(identical(other.isEditing, isEditing) || other.isEditing == isEditing)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved));
}


@override
int get hashCode => Object.hash(runtimeType,isEditing,isSaving,saveError,justSaved);

@override
String toString() {
  return 'PersonalDetailsState(isEditing: $isEditing, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved)';
}


}

/// @nodoc
abstract mixin class _$PersonalDetailsStateCopyWith<$Res> implements $PersonalDetailsStateCopyWith<$Res> {
  factory _$PersonalDetailsStateCopyWith(_PersonalDetailsState value, $Res Function(_PersonalDetailsState) _then) = __$PersonalDetailsStateCopyWithImpl;
@override @useResult
$Res call({
 bool isEditing, bool isSaving, String? saveError, bool justSaved
});




}
/// @nodoc
class __$PersonalDetailsStateCopyWithImpl<$Res>
    implements _$PersonalDetailsStateCopyWith<$Res> {
  __$PersonalDetailsStateCopyWithImpl(this._self, this._then);

  final _PersonalDetailsState _self;
  final $Res Function(_PersonalDetailsState) _then;

/// Create a copy of PersonalDetailsState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isEditing = null,Object? isSaving = null,Object? saveError = freezed,Object? justSaved = null,}) {
  return _then(_PersonalDetailsState(
isEditing: null == isEditing ? _self.isEditing : isEditing // ignore: cast_nullable_to_non_nullable
as bool,isSaving: null == isSaving ? _self.isSaving : isSaving // ignore: cast_nullable_to_non_nullable
as bool,saveError: freezed == saveError ? _self.saveError : saveError // ignore: cast_nullable_to_non_nullable
as String?,justSaved: null == justSaved ? _self.justSaved : justSaved // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
