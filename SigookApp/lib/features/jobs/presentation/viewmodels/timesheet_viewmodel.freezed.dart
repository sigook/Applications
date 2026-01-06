// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'timesheet_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$TimesheetState implements DiagnosticableTreeMixin {

 List<TimesheetEntry> get entries; bool get isLoading; bool get isLoadingMore; String? get error; int get currentPage; int get totalPages; int get totalItems;
/// Create a copy of TimesheetState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$TimesheetStateCopyWith<TimesheetState> get copyWith => _$TimesheetStateCopyWithImpl<TimesheetState>(this as TimesheetState, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
  properties
    ..add(DiagnosticsProperty('type', 'TimesheetState'))
    ..add(DiagnosticsProperty('entries', entries))..add(DiagnosticsProperty('isLoading', isLoading))..add(DiagnosticsProperty('isLoadingMore', isLoadingMore))..add(DiagnosticsProperty('error', error))..add(DiagnosticsProperty('currentPage', currentPage))..add(DiagnosticsProperty('totalPages', totalPages))..add(DiagnosticsProperty('totalItems', totalItems));
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is TimesheetState&&const DeepCollectionEquality().equals(other.entries, entries)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.totalPages, totalPages) || other.totalPages == totalPages)&&(identical(other.totalItems, totalItems) || other.totalItems == totalItems));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(entries),isLoading,isLoadingMore,error,currentPage,totalPages,totalItems);

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
  return 'TimesheetState(entries: $entries, isLoading: $isLoading, isLoadingMore: $isLoadingMore, error: $error, currentPage: $currentPage, totalPages: $totalPages, totalItems: $totalItems)';
}


}

/// @nodoc
abstract mixin class $TimesheetStateCopyWith<$Res>  {
  factory $TimesheetStateCopyWith(TimesheetState value, $Res Function(TimesheetState) _then) = _$TimesheetStateCopyWithImpl;
@useResult
$Res call({
 List<TimesheetEntry> entries, bool isLoading, bool isLoadingMore, String? error, int currentPage, int totalPages, int totalItems
});




}
/// @nodoc
class _$TimesheetStateCopyWithImpl<$Res>
    implements $TimesheetStateCopyWith<$Res> {
  _$TimesheetStateCopyWithImpl(this._self, this._then);

  final TimesheetState _self;
  final $Res Function(TimesheetState) _then;

/// Create a copy of TimesheetState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? entries = null,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? totalPages = null,Object? totalItems = null,}) {
  return _then(_self.copyWith(
entries: null == entries ? _self.entries : entries // ignore: cast_nullable_to_non_nullable
as List<TimesheetEntry>,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,totalPages: null == totalPages ? _self.totalPages : totalPages // ignore: cast_nullable_to_non_nullable
as int,totalItems: null == totalItems ? _self.totalItems : totalItems // ignore: cast_nullable_to_non_nullable
as int,
  ));
}

}


/// Adds pattern-matching-related methods to [TimesheetState].
extension TimesheetStatePatterns on TimesheetState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _TimesheetState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _TimesheetState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _TimesheetState value)  $default,){
final _that = this;
switch (_that) {
case _TimesheetState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _TimesheetState value)?  $default,){
final _that = this;
switch (_that) {
case _TimesheetState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( List<TimesheetEntry> entries,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  int totalPages,  int totalItems)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _TimesheetState() when $default != null:
return $default(_that.entries,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.totalPages,_that.totalItems);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( List<TimesheetEntry> entries,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  int totalPages,  int totalItems)  $default,) {final _that = this;
switch (_that) {
case _TimesheetState():
return $default(_that.entries,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.totalPages,_that.totalItems);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( List<TimesheetEntry> entries,  bool isLoading,  bool isLoadingMore,  String? error,  int currentPage,  int totalPages,  int totalItems)?  $default,) {final _that = this;
switch (_that) {
case _TimesheetState() when $default != null:
return $default(_that.entries,_that.isLoading,_that.isLoadingMore,_that.error,_that.currentPage,_that.totalPages,_that.totalItems);case _:
  return null;

}
}

}

/// @nodoc


class _TimesheetState with DiagnosticableTreeMixin implements TimesheetState {
  const _TimesheetState({final  List<TimesheetEntry> entries = const [], this.isLoading = false, this.isLoadingMore = false, this.error, this.currentPage = 1, this.totalPages = 1, this.totalItems = 0}): _entries = entries;
  

 final  List<TimesheetEntry> _entries;
@override@JsonKey() List<TimesheetEntry> get entries {
  if (_entries is EqualUnmodifiableListView) return _entries;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_entries);
}

@override@JsonKey() final  bool isLoading;
@override@JsonKey() final  bool isLoadingMore;
@override final  String? error;
@override@JsonKey() final  int currentPage;
@override@JsonKey() final  int totalPages;
@override@JsonKey() final  int totalItems;

/// Create a copy of TimesheetState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$TimesheetStateCopyWith<_TimesheetState> get copyWith => __$TimesheetStateCopyWithImpl<_TimesheetState>(this, _$identity);


@override
void debugFillProperties(DiagnosticPropertiesBuilder properties) {
  properties
    ..add(DiagnosticsProperty('type', 'TimesheetState'))
    ..add(DiagnosticsProperty('entries', entries))..add(DiagnosticsProperty('isLoading', isLoading))..add(DiagnosticsProperty('isLoadingMore', isLoadingMore))..add(DiagnosticsProperty('error', error))..add(DiagnosticsProperty('currentPage', currentPage))..add(DiagnosticsProperty('totalPages', totalPages))..add(DiagnosticsProperty('totalItems', totalItems));
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _TimesheetState&&const DeepCollectionEquality().equals(other._entries, _entries)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.totalPages, totalPages) || other.totalPages == totalPages)&&(identical(other.totalItems, totalItems) || other.totalItems == totalItems));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_entries),isLoading,isLoadingMore,error,currentPage,totalPages,totalItems);

@override
String toString({ DiagnosticLevel minLevel = DiagnosticLevel.info }) {
  return 'TimesheetState(entries: $entries, isLoading: $isLoading, isLoadingMore: $isLoadingMore, error: $error, currentPage: $currentPage, totalPages: $totalPages, totalItems: $totalItems)';
}


}

/// @nodoc
abstract mixin class _$TimesheetStateCopyWith<$Res> implements $TimesheetStateCopyWith<$Res> {
  factory _$TimesheetStateCopyWith(_TimesheetState value, $Res Function(_TimesheetState) _then) = __$TimesheetStateCopyWithImpl;
@override @useResult
$Res call({
 List<TimesheetEntry> entries, bool isLoading, bool isLoadingMore, String? error, int currentPage, int totalPages, int totalItems
});




}
/// @nodoc
class __$TimesheetStateCopyWithImpl<$Res>
    implements _$TimesheetStateCopyWith<$Res> {
  __$TimesheetStateCopyWithImpl(this._self, this._then);

  final _TimesheetState _self;
  final $Res Function(_TimesheetState) _then;

/// Create a copy of TimesheetState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? entries = null,Object? isLoading = null,Object? isLoadingMore = null,Object? error = freezed,Object? currentPage = null,Object? totalPages = null,Object? totalItems = null,}) {
  return _then(_TimesheetState(
entries: null == entries ? _self._entries : entries // ignore: cast_nullable_to_non_nullable
as List<TimesheetEntry>,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,totalPages: null == totalPages ? _self.totalPages : totalPages // ignore: cast_nullable_to_non_nullable
as int,totalItems: null == totalItems ? _self.totalItems : totalItems // ignore: cast_nullable_to_non_nullable
as int,
  ));
}


}

// dart format on
