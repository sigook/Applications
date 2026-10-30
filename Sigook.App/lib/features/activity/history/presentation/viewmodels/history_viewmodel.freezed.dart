// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'history_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$AssignmentHistoryState {

 List<Assignment> get items; String? get status; bool get isLoading; bool get isLoadingMore; String? get error; int get currentPage; bool get hasMore;
/// Create a copy of AssignmentHistoryState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AssignmentHistoryStateCopyWith<AssignmentHistoryState> get copyWith => _$AssignmentHistoryStateCopyWithImpl<AssignmentHistoryState>(this as AssignmentHistoryState, _$identity);



@override
bool operator ==(Object other) {
  final _this = this as AssignmentHistoryState;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AssignmentHistoryState&&const DeepCollectionEquality().equals(other.items, _this.items)&&(identical(other.status, _this.status) || other.status == _this.status)&&(identical(other.isLoading, _this.isLoading) || other.isLoading == _this.isLoading)&&(identical(other.isLoadingMore, _this.isLoadingMore) || other.isLoadingMore == _this.isLoadingMore)&&(identical(other.error, _this.error) || other.error == _this.error)&&(identical(other.currentPage, _this.currentPage) || other.currentPage == _this.currentPage)&&(identical(other.hasMore, _this.hasMore) || other.hasMore == _this.hasMore));
}


@override
int get hashCode {
  final _this = this as AssignmentHistoryState;
  return Object.hash(runtimeType,const DeepCollectionEquality().hash(_this.items),_this.status,_this.isLoading,_this.isLoadingMore,_this.error,_this.currentPage,_this.hasMore);
}

@override
String toString() {
  final _this = this as AssignmentHistoryState;
  return 'AssignmentHistoryState(items: ${_this.items}, status: ${_this.status}, isLoading: ${_this.isLoading}, isLoadingMore: ${_this.isLoadingMore}, error: ${_this.error}, currentPage: ${_this.currentPage}, hasMore: ${_this.hasMore})';
}


}

/// @nodoc
abstract mixin class $AssignmentHistoryStateCopyWith<$Res>  {
  factory $AssignmentHistoryStateCopyWith(AssignmentHistoryState value, $Res Function(AssignmentHistoryState) _then) = _$AssignmentHistoryStateCopyWithImpl;
@useResult
$Res call({
 List<Assignment> items, String? status, bool isLoading, bool isLoadingMore, String? error, int currentPage, bool hasMore
});




}
/// @nodoc
class _$AssignmentHistoryStateCopyWithImpl<$Res>
    implements $AssignmentHistoryStateCopyWith<$Res> {
  _$AssignmentHistoryStateCopyWithImpl(this._self, this._then);

  final AssignmentHistoryState _self;
  final $Res Function(AssignmentHistoryState) _then;

/// Create a copy of AssignmentHistoryState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? items = null,Object? status = freezed,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? hasMore = null,}) {
  return _then(AssignmentHistoryState(
items: null == items ? _self.items : items // ignore: cast_nullable_to_non_nullable
as List<Assignment>,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [AssignmentHistoryState].
extension AssignmentHistoryStatePatterns on AssignmentHistoryState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AssignmentHistoryState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AssignmentHistoryState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AssignmentHistoryState value)  $default,){
final _that = this;
switch (_that) {
case _AssignmentHistoryState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AssignmentHistoryState value)?  $default,){
final _that = this;
switch (_that) {
case _AssignmentHistoryState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( List<Assignment> items,  String? status,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AssignmentHistoryState() when $default != null:
return $default(_that.items,_that.status,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( List<Assignment> items,  String? status,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)  $default,) {final _that = this;
switch (_that) {
case _AssignmentHistoryState():
return $default(_that.items,_that.status,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( List<Assignment> items,  String? status,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  bool hasMore)?  $default,) {final _that = this;
switch (_that) {
case _AssignmentHistoryState() when $default != null:
return $default(_that.items,_that.status,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.hasMore);case _:
  return null;

}
}

}

/// @nodoc


class _AssignmentHistoryState implements AssignmentHistoryState {
  const _AssignmentHistoryState({ List<Assignment> items = const [], this.status, this.isLoading = false, this.isLoadingMore = false, this.error, this.currentPage = 1, this.hasMore = false}): _items = items;
  

 final  List<Assignment> _items;
@override@JsonKey() List<Assignment> get items {
  if (_items is EqualUnmodifiableListView) return _items;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_items);
}

@override final  String? status;
@override@JsonKey() final  bool isLoading;
@override@JsonKey() final  bool isLoadingMore;
@override final  String? error;
@override@JsonKey() final  int currentPage;
@override@JsonKey() final  bool hasMore;

/// Create a copy of AssignmentHistoryState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AssignmentHistoryStateCopyWith<_AssignmentHistoryState> get copyWith => __$AssignmentHistoryStateCopyWithImpl<_AssignmentHistoryState>(this, _$identity);



@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _AssignmentHistoryState&&const DeepCollectionEquality().equals(other.items, _items)&&(identical(other.status, status) || other.status == status)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.hasMore, hasMore) || other.hasMore == hasMore));
}


@override
int get hashCode {
    return Object.hash(runtimeType,const DeepCollectionEquality().hash(_items),status,isLoading,isLoadingMore,error,currentPage,hasMore);
}

@override
String toString() {
    return 'AssignmentHistoryState(items: $items, status: $status, isLoading: $isLoading, isLoadingMore: $isLoadingMore, error: $error, currentPage: $currentPage, hasMore: $hasMore)';
}


}

/// @nodoc
abstract mixin class _$AssignmentHistoryStateCopyWith<$Res> implements $AssignmentHistoryStateCopyWith<$Res> {
  factory _$AssignmentHistoryStateCopyWith(_AssignmentHistoryState value, $Res Function(_AssignmentHistoryState) _then) = __$AssignmentHistoryStateCopyWithImpl;
@override @useResult
$Res call({
 List<Assignment> items, String? status, bool isLoading, bool isLoadingMore, String? error, int currentPage, bool hasMore
});




}
/// @nodoc
class __$AssignmentHistoryStateCopyWithImpl<$Res>
    implements _$AssignmentHistoryStateCopyWith<$Res> {
  __$AssignmentHistoryStateCopyWithImpl(this._self, this._then);

  final _AssignmentHistoryState _self;
  final $Res Function(_AssignmentHistoryState) _then;

/// Create a copy of AssignmentHistoryState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? items = null,Object? status = freezed,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? hasMore = null,}) {
  return _then(_AssignmentHistoryState(
items: null == items ? _self._items : items // ignore: cast_nullable_to_non_nullable
as List<Assignment>,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
