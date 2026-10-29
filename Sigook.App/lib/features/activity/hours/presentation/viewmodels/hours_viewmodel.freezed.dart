// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'hours_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$HoursState {

 DateTime get weekStart; DateTime get currentWeekStart; WeekTimeSheets? get week; bool get isLoading; String? get error;
/// Create a copy of HoursState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$HoursStateCopyWith<HoursState> get copyWith => _$HoursStateCopyWithImpl<HoursState>(this as HoursState, _$identity);



@override
bool operator ==(Object other) {
  final _this = this as HoursState;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is HoursState&&(identical(other.weekStart, _this.weekStart) || other.weekStart == _this.weekStart)&&(identical(other.currentWeekStart, _this.currentWeekStart) || other.currentWeekStart == _this.currentWeekStart)&&(identical(other.week, _this.week) || other.week == _this.week)&&(identical(other.isLoading, _this.isLoading) || other.isLoading == _this.isLoading)&&(identical(other.error, _this.error) || other.error == _this.error));
}


@override
int get hashCode {
  final _this = this as HoursState;
  return Object.hash(runtimeType,_this.weekStart,_this.currentWeekStart,_this.week,_this.isLoading,_this.error);
}

@override
String toString() {
  final _this = this as HoursState;
  return 'HoursState(weekStart: ${_this.weekStart}, currentWeekStart: ${_this.currentWeekStart}, week: ${_this.week}, isLoading: ${_this.isLoading}, error: ${_this.error})';
}


}

/// @nodoc
abstract mixin class $HoursStateCopyWith<$Res>  {
  factory $HoursStateCopyWith(HoursState value, $Res Function(HoursState) _then) = _$HoursStateCopyWithImpl;
@useResult
$Res call({
 DateTime weekStart, DateTime currentWeekStart, WeekTimeSheets? week, bool isLoading, String? error
});




}
/// @nodoc
class _$HoursStateCopyWithImpl<$Res>
    implements $HoursStateCopyWith<$Res> {
  _$HoursStateCopyWithImpl(this._self, this._then);

  final HoursState _self;
  final $Res Function(HoursState) _then;

/// Create a copy of HoursState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? weekStart = null,Object? currentWeekStart = null,Object? week = freezed,Object? isLoading = null,Object? error = freezed,}) {
  return _then(HoursState(
weekStart: null == weekStart ? _self.weekStart : weekStart // ignore: cast_nullable_to_non_nullable
as DateTime,currentWeekStart: null == currentWeekStart ? _self.currentWeekStart : currentWeekStart // ignore: cast_nullable_to_non_nullable
as DateTime,week: freezed == week ? _self.week : week // ignore: cast_nullable_to_non_nullable
as WeekTimeSheets?,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [HoursState].
extension HoursStatePatterns on HoursState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _HoursState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _HoursState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _HoursState value)  $default,){
final _that = this;
switch (_that) {
case _HoursState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _HoursState value)?  $default,){
final _that = this;
switch (_that) {
case _HoursState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( DateTime weekStart,  DateTime currentWeekStart,  WeekTimeSheets? week,  bool isLoading,  String? error)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _HoursState() when $default != null:
return $default(_that.weekStart,_that.currentWeekStart,_that.week,_that.isLoading,_that.error);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( DateTime weekStart,  DateTime currentWeekStart,  WeekTimeSheets? week,  bool isLoading,  String? error)  $default,) {final _that = this;
switch (_that) {
case _HoursState():
return $default(_that.weekStart,_that.currentWeekStart,_that.week,_that.isLoading,_that.error);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( DateTime weekStart,  DateTime currentWeekStart,  WeekTimeSheets? week,  bool isLoading,  String? error)?  $default,) {final _that = this;
switch (_that) {
case _HoursState() when $default != null:
return $default(_that.weekStart,_that.currentWeekStart,_that.week,_that.isLoading,_that.error);case _:
  return null;

}
}

}

/// @nodoc


class _HoursState extends HoursState {
  const _HoursState({required this.weekStart, required this.currentWeekStart, this.week, this.isLoading = false, this.error}): super._();
  

@override final  DateTime weekStart;
@override final  DateTime currentWeekStart;
@override final  WeekTimeSheets? week;
@override@JsonKey() final  bool isLoading;
@override final  String? error;

/// Create a copy of HoursState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$HoursStateCopyWith<_HoursState> get copyWith => __$HoursStateCopyWithImpl<_HoursState>(this, _$identity);



@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _HoursState&&(identical(other.weekStart, weekStart) || other.weekStart == weekStart)&&(identical(other.currentWeekStart, currentWeekStart) || other.currentWeekStart == currentWeekStart)&&(identical(other.week, week) || other.week == week)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.error, error) || other.error == error));
}


@override
int get hashCode {
    return Object.hash(runtimeType,weekStart,currentWeekStart,week,isLoading,error);
}

@override
String toString() {
    return 'HoursState(weekStart: $weekStart, currentWeekStart: $currentWeekStart, week: $week, isLoading: $isLoading, error: $error)';
}


}

/// @nodoc
abstract mixin class _$HoursStateCopyWith<$Res> implements $HoursStateCopyWith<$Res> {
  factory _$HoursStateCopyWith(_HoursState value, $Res Function(_HoursState) _then) = __$HoursStateCopyWithImpl;
@override @useResult
$Res call({
 DateTime weekStart, DateTime currentWeekStart, WeekTimeSheets? week, bool isLoading, String? error
});




}
/// @nodoc
class __$HoursStateCopyWithImpl<$Res>
    implements _$HoursStateCopyWith<$Res> {
  __$HoursStateCopyWithImpl(this._self, this._then);

  final _HoursState _self;
  final $Res Function(_HoursState) _then;

/// Create a copy of HoursState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? weekStart = null,Object? currentWeekStart = null,Object? week = freezed,Object? isLoading = null,Object? error = freezed,}) {
  return _then(_HoursState(
weekStart: null == weekStart ? _self.weekStart : weekStart // ignore: cast_nullable_to_non_nullable
as DateTime,currentWeekStart: null == currentWeekStart ? _self.currentWeekStart : currentWeekStart // ignore: cast_nullable_to_non_nullable
as DateTime,week: freezed == week ? _self.week : week // ignore: cast_nullable_to_non_nullable
as WeekTimeSheets?,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on
