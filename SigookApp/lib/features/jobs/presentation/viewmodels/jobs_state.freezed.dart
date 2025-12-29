// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'jobs_state.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$JobsState {

 List<Job> get jobs; bool get isLoading; bool get isLoadingMore; String? get error; int get currentPage; bool get hasMore;
/// Create a copy of JobsState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$JobsStateCopyWith<JobsState> get copyWith => _$JobsStateCopyWithImpl<JobsState>(this as JobsState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is JobsState&&const DeepCollectionEquality().equals(other.jobs, jobs)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.hasMore, hasMore) || other.hasMore == hasMore));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(jobs),isLoading,isLoadingMore,error,currentPage,hasMore);

@override
String toString() {
  return 'JobsState(jobs: $jobs, isLoading: $isLoading, isLoadingMore: $isLoadingMore, error: $error, currentPage: $currentPage, hasMore: $hasMore)';
}


}

/// @nodoc
abstract mixin class $JobsStateCopyWith<$Res>  {
  factory $JobsStateCopyWith(JobsState value, $Res Function(JobsState) _then) = _$JobsStateCopyWithImpl;
@useResult
$Res call({
 List<Job> jobs, bool isLoading, bool isLoadingMore, String? error, int currentPage, bool hasMore
});




}
/// @nodoc
class _$JobsStateCopyWithImpl<$Res>
    implements $JobsStateCopyWith<$Res> {
  _$JobsStateCopyWithImpl(this._self, this._then);

  final JobsState _self;
  final $Res Function(JobsState) _then;

/// Create a copy of JobsState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? jobs = null,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? hasMore = null,}) {
  return _then(_self.copyWith(
jobs: null == jobs ? _self.jobs : jobs // ignore: cast_nullable_to_non_nullable
as List<Job>,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [JobsState].
extension JobsStatePatterns on JobsState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _JobsState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _JobsState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _JobsState value)  $default,){
final _that = this;
switch (_that) {
case _JobsState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _JobsState value)?  $default,){
final _that = this;
switch (_that) {
case _JobsState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( List<Job> jobs,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _JobsState() when $default != null:
return $default(_that.jobs,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( List<Job> jobs,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)  $default,) {final _that = this;
switch (_that) {
case _JobsState():
return $default(_that.jobs,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( List<Job> jobs,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)?  $default,) {final _that = this;
switch (_that) {
case _JobsState() when $default != null:
return $default(_that.jobs,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
  return null;

}
}

}

/// @nodoc


class _JobsState implements JobsState {
  const _JobsState({final  List<Job> jobs = const [], this.isLoading = false, this.isLoadingMore = false, this.error, this.currentPage = 1, this.hasMore = true}): _jobs = jobs;
  

 final  List<Job> _jobs;
@override@JsonKey() List<Job> get jobs {
  if (_jobs is EqualUnmodifiableListView) return _jobs;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_jobs);
}

@override@JsonKey() final  bool isLoading;
@override@JsonKey() final  bool isLoadingMore;
@override final  String? error;
@override@JsonKey() final  int currentPage;
@override@JsonKey() final  bool hasMore;

/// Create a copy of JobsState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$JobsStateCopyWith<_JobsState> get copyWith => __$JobsStateCopyWithImpl<_JobsState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _JobsState&&const DeepCollectionEquality().equals(other._jobs, _jobs)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.hasMore, hasMore) || other.hasMore == hasMore));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_jobs),isLoading,isLoadingMore,error,currentPage,hasMore);

@override
String toString() {
  return 'JobsState(jobs: $jobs, isLoading: $isLoading, isLoadingMore: $isLoadingMore, error: $error, currentPage: $currentPage, hasMore: $hasMore)';
}


}

/// @nodoc
abstract mixin class _$JobsStateCopyWith<$Res> implements $JobsStateCopyWith<$Res> {
  factory _$JobsStateCopyWith(_JobsState value, $Res Function(_JobsState) _then) = __$JobsStateCopyWithImpl;
@override @useResult
$Res call({
 List<Job> jobs, bool isLoading, bool isLoadingMore, String? error, int currentPage, bool hasMore
});




}
/// @nodoc
class __$JobsStateCopyWithImpl<$Res>
    implements _$JobsStateCopyWith<$Res> {
  __$JobsStateCopyWithImpl(this._self, this._then);

  final _JobsState _self;
  final $Res Function(_JobsState) _then;

/// Create a copy of JobsState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? jobs = null,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? hasMore = null,}) {
  return _then(_JobsState(
jobs: null == jobs ? _self._jobs : jobs // ignore: cast_nullable_to_non_nullable
as List<Job>,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
