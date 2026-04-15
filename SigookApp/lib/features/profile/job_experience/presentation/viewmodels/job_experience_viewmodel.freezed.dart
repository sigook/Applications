// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'job_experience_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$JobExperienceState {

// Add flow
 bool get isAdding; String? get addError; bool get justAdded; bool get showForm;// Edit flow
 String? get editingId; bool get isSaving; String? get saveError; bool get justSaved;// Delete flow
 String? get deletingId; String? get deleteError; bool get justDeleted;
/// Create a copy of JobExperienceState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$JobExperienceStateCopyWith<JobExperienceState> get copyWith => _$JobExperienceStateCopyWithImpl<JobExperienceState>(this as JobExperienceState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is JobExperienceState&&(identical(other.isAdding, isAdding) || other.isAdding == isAdding)&&(identical(other.addError, addError) || other.addError == addError)&&(identical(other.justAdded, justAdded) || other.justAdded == justAdded)&&(identical(other.showForm, showForm) || other.showForm == showForm)&&(identical(other.editingId, editingId) || other.editingId == editingId)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved)&&(identical(other.deletingId, deletingId) || other.deletingId == deletingId)&&(identical(other.deleteError, deleteError) || other.deleteError == deleteError)&&(identical(other.justDeleted, justDeleted) || other.justDeleted == justDeleted));
}


@override
int get hashCode => Object.hash(runtimeType,isAdding,addError,justAdded,showForm,editingId,isSaving,saveError,justSaved,deletingId,deleteError,justDeleted);

@override
String toString() {
  return 'JobExperienceState(isAdding: $isAdding, addError: $addError, justAdded: $justAdded, showForm: $showForm, editingId: $editingId, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved, deletingId: $deletingId, deleteError: $deleteError, justDeleted: $justDeleted)';
}


}

/// @nodoc
abstract mixin class $JobExperienceStateCopyWith<$Res>  {
  factory $JobExperienceStateCopyWith(JobExperienceState value, $Res Function(JobExperienceState) _then) = _$JobExperienceStateCopyWithImpl;
@useResult
$Res call({
 bool isAdding, String? addError, bool justAdded, bool showForm, String? editingId, bool isSaving, String? saveError, bool justSaved, String? deletingId, String? deleteError, bool justDeleted
});




}
/// @nodoc
class _$JobExperienceStateCopyWithImpl<$Res>
    implements $JobExperienceStateCopyWith<$Res> {
  _$JobExperienceStateCopyWithImpl(this._self, this._then);

  final JobExperienceState _self;
  final $Res Function(JobExperienceState) _then;

/// Create a copy of JobExperienceState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? isAdding = null,Object? addError = freezed,Object? justAdded = null,Object? showForm = null,Object? editingId = freezed,Object? isSaving = null,Object? saveError = freezed,Object? justSaved = null,Object? deletingId = freezed,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(_self.copyWith(
isAdding: null == isAdding ? _self.isAdding : isAdding // ignore: cast_nullable_to_non_nullable
as bool,addError: freezed == addError ? _self.addError : addError // ignore: cast_nullable_to_non_nullable
as String?,justAdded: null == justAdded ? _self.justAdded : justAdded // ignore: cast_nullable_to_non_nullable
as bool,showForm: null == showForm ? _self.showForm : showForm // ignore: cast_nullable_to_non_nullable
as bool,editingId: freezed == editingId ? _self.editingId : editingId // ignore: cast_nullable_to_non_nullable
as String?,isSaving: null == isSaving ? _self.isSaving : isSaving // ignore: cast_nullable_to_non_nullable
as bool,saveError: freezed == saveError ? _self.saveError : saveError // ignore: cast_nullable_to_non_nullable
as String?,justSaved: null == justSaved ? _self.justSaved : justSaved // ignore: cast_nullable_to_non_nullable
as bool,deletingId: freezed == deletingId ? _self.deletingId : deletingId // ignore: cast_nullable_to_non_nullable
as String?,deleteError: freezed == deleteError ? _self.deleteError : deleteError // ignore: cast_nullable_to_non_nullable
as String?,justDeleted: null == justDeleted ? _self.justDeleted : justDeleted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [JobExperienceState].
extension JobExperienceStatePatterns on JobExperienceState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _JobExperienceState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _JobExperienceState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _JobExperienceState value)  $default,){
final _that = this;
switch (_that) {
case _JobExperienceState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _JobExperienceState value)?  $default,){
final _that = this;
switch (_that) {
case _JobExperienceState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( bool isAdding,  String? addError,  bool justAdded,  bool showForm,  String? editingId,  bool isSaving,  String? saveError,  bool justSaved,  String? deletingId,  String? deleteError,  bool justDeleted)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _JobExperienceState() when $default != null:
return $default(_that.isAdding,_that.addError,_that.justAdded,_that.showForm,_that.editingId,_that.isSaving,_that.saveError,_that.justSaved,_that.deletingId,_that.deleteError,_that.justDeleted);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( bool isAdding,  String? addError,  bool justAdded,  bool showForm,  String? editingId,  bool isSaving,  String? saveError,  bool justSaved,  String? deletingId,  String? deleteError,  bool justDeleted)  $default,) {final _that = this;
switch (_that) {
case _JobExperienceState():
return $default(_that.isAdding,_that.addError,_that.justAdded,_that.showForm,_that.editingId,_that.isSaving,_that.saveError,_that.justSaved,_that.deletingId,_that.deleteError,_that.justDeleted);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( bool isAdding,  String? addError,  bool justAdded,  bool showForm,  String? editingId,  bool isSaving,  String? saveError,  bool justSaved,  String? deletingId,  String? deleteError,  bool justDeleted)?  $default,) {final _that = this;
switch (_that) {
case _JobExperienceState() when $default != null:
return $default(_that.isAdding,_that.addError,_that.justAdded,_that.showForm,_that.editingId,_that.isSaving,_that.saveError,_that.justSaved,_that.deletingId,_that.deleteError,_that.justDeleted);case _:
  return null;

}
}

}

/// @nodoc


class _JobExperienceState implements JobExperienceState {
  const _JobExperienceState({this.isAdding = false, this.addError, this.justAdded = false, this.showForm = false, this.editingId, this.isSaving = false, this.saveError, this.justSaved = false, this.deletingId, this.deleteError, this.justDeleted = false});
  

// Add flow
@override@JsonKey() final  bool isAdding;
@override final  String? addError;
@override@JsonKey() final  bool justAdded;
@override@JsonKey() final  bool showForm;
// Edit flow
@override final  String? editingId;
@override@JsonKey() final  bool isSaving;
@override final  String? saveError;
@override@JsonKey() final  bool justSaved;
// Delete flow
@override final  String? deletingId;
@override final  String? deleteError;
@override@JsonKey() final  bool justDeleted;

/// Create a copy of JobExperienceState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$JobExperienceStateCopyWith<_JobExperienceState> get copyWith => __$JobExperienceStateCopyWithImpl<_JobExperienceState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _JobExperienceState&&(identical(other.isAdding, isAdding) || other.isAdding == isAdding)&&(identical(other.addError, addError) || other.addError == addError)&&(identical(other.justAdded, justAdded) || other.justAdded == justAdded)&&(identical(other.showForm, showForm) || other.showForm == showForm)&&(identical(other.editingId, editingId) || other.editingId == editingId)&&(identical(other.isSaving, isSaving) || other.isSaving == isSaving)&&(identical(other.saveError, saveError) || other.saveError == saveError)&&(identical(other.justSaved, justSaved) || other.justSaved == justSaved)&&(identical(other.deletingId, deletingId) || other.deletingId == deletingId)&&(identical(other.deleteError, deleteError) || other.deleteError == deleteError)&&(identical(other.justDeleted, justDeleted) || other.justDeleted == justDeleted));
}


@override
int get hashCode => Object.hash(runtimeType,isAdding,addError,justAdded,showForm,editingId,isSaving,saveError,justSaved,deletingId,deleteError,justDeleted);

@override
String toString() {
  return 'JobExperienceState(isAdding: $isAdding, addError: $addError, justAdded: $justAdded, showForm: $showForm, editingId: $editingId, isSaving: $isSaving, saveError: $saveError, justSaved: $justSaved, deletingId: $deletingId, deleteError: $deleteError, justDeleted: $justDeleted)';
}


}

/// @nodoc
abstract mixin class _$JobExperienceStateCopyWith<$Res> implements $JobExperienceStateCopyWith<$Res> {
  factory _$JobExperienceStateCopyWith(_JobExperienceState value, $Res Function(_JobExperienceState) _then) = __$JobExperienceStateCopyWithImpl;
@override @useResult
$Res call({
 bool isAdding, String? addError, bool justAdded, bool showForm, String? editingId, bool isSaving, String? saveError, bool justSaved, String? deletingId, String? deleteError, bool justDeleted
});




}
/// @nodoc
class __$JobExperienceStateCopyWithImpl<$Res>
    implements _$JobExperienceStateCopyWith<$Res> {
  __$JobExperienceStateCopyWithImpl(this._self, this._then);

  final _JobExperienceState _self;
  final $Res Function(_JobExperienceState) _then;

/// Create a copy of JobExperienceState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? isAdding = null,Object? addError = freezed,Object? justAdded = null,Object? showForm = null,Object? editingId = freezed,Object? isSaving = null,Object? saveError = freezed,Object? justSaved = null,Object? deletingId = freezed,Object? deleteError = freezed,Object? justDeleted = null,}) {
  return _then(_JobExperienceState(
isAdding: null == isAdding ? _self.isAdding : isAdding // ignore: cast_nullable_to_non_nullable
as bool,addError: freezed == addError ? _self.addError : addError // ignore: cast_nullable_to_non_nullable
as String?,justAdded: null == justAdded ? _self.justAdded : justAdded // ignore: cast_nullable_to_non_nullable
as bool,showForm: null == showForm ? _self.showForm : showForm // ignore: cast_nullable_to_non_nullable
as bool,editingId: freezed == editingId ? _self.editingId : editingId // ignore: cast_nullable_to_non_nullable
as String?,isSaving: null == isSaving ? _self.isSaving : isSaving // ignore: cast_nullable_to_non_nullable
as bool,saveError: freezed == saveError ? _self.saveError : saveError // ignore: cast_nullable_to_non_nullable
as String?,justSaved: null == justSaved ? _self.justSaved : justSaved // ignore: cast_nullable_to_non_nullable
as bool,deletingId: freezed == deletingId ? _self.deletingId : deletingId // ignore: cast_nullable_to_non_nullable
as String?,deleteError: freezed == deleteError ? _self.deleteError : deleteError // ignore: cast_nullable_to_non_nullable
as String?,justDeleted: null == justDeleted ? _self.justDeleted : justDeleted // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
