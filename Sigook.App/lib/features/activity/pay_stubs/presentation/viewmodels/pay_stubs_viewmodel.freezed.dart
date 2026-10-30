// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'pay_stubs_viewmodel.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$PayStubsState {

 List<int> get years; int? get year; List<PayStubSummary> get items; int get totalCount; double get ytdNet; bool get isLoading; bool get isLoadingMore; int get currentPage; bool get hasMore; String? get error; String? get openingId; String? get pdfError;
/// Create a copy of PayStubsState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PayStubsStateCopyWith<PayStubsState> get copyWith => _$PayStubsStateCopyWithImpl<PayStubsState>(this as PayStubsState, _$identity);



@override
bool operator ==(Object other) {
  final _this = this as PayStubsState;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PayStubsState&&const DeepCollectionEquality().equals(other.years, _this.years)&&(identical(other.year, _this.year) || other.year == _this.year)&&const DeepCollectionEquality().equals(other.items, _this.items)&&(identical(other.totalCount, _this.totalCount) || other.totalCount == _this.totalCount)&&(identical(other.ytdNet, _this.ytdNet) || other.ytdNet == _this.ytdNet)&&(identical(other.isLoading, _this.isLoading) || other.isLoading == _this.isLoading)&&(identical(other.isLoadingMore, _this.isLoadingMore) || other.isLoadingMore == _this.isLoadingMore)&&(identical(other.currentPage, _this.currentPage) || other.currentPage == _this.currentPage)&&(identical(other.hasMore, _this.hasMore) || other.hasMore == _this.hasMore)&&(identical(other.error, _this.error) || other.error == _this.error)&&(identical(other.openingId, _this.openingId) || other.openingId == _this.openingId)&&(identical(other.pdfError, _this.pdfError) || other.pdfError == _this.pdfError));
}


@override
int get hashCode {
  final _this = this as PayStubsState;
  return Object.hash(runtimeType,const DeepCollectionEquality().hash(_this.years),_this.year,const DeepCollectionEquality().hash(_this.items),_this.totalCount,_this.ytdNet,_this.isLoading,_this.isLoadingMore,_this.currentPage,_this.hasMore,_this.error,_this.openingId,_this.pdfError);
}

@override
String toString() {
  final _this = this as PayStubsState;
  return 'PayStubsState(years: ${_this.years}, year: ${_this.year}, items: ${_this.items}, totalCount: ${_this.totalCount}, ytdNet: ${_this.ytdNet}, isLoading: ${_this.isLoading}, isLoadingMore: ${_this.isLoadingMore}, currentPage: ${_this.currentPage}, hasMore: ${_this.hasMore}, error: ${_this.error}, openingId: ${_this.openingId}, pdfError: ${_this.pdfError})';
}


}

/// @nodoc
abstract mixin class $PayStubsStateCopyWith<$Res>  {
  factory $PayStubsStateCopyWith(PayStubsState value, $Res Function(PayStubsState) _then) = _$PayStubsStateCopyWithImpl;
@useResult
$Res call({
 List<int> years, int? year, List<PayStubSummary> items, int totalCount, double ytdNet, bool isLoading, bool isLoadingMore, int currentPage, bool hasMore, String? error, String? openingId, String? pdfError
});




}
/// @nodoc
class _$PayStubsStateCopyWithImpl<$Res>
    implements $PayStubsStateCopyWith<$Res> {
  _$PayStubsStateCopyWithImpl(this._self, this._then);

  final PayStubsState _self;
  final $Res Function(PayStubsState) _then;

/// Create a copy of PayStubsState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? years = null,Object? year = freezed,Object? items = null,Object? totalCount = null,Object? ytdNet = null,Object? isLoading = null,Object? isLoadingMore = null,Object? currentPage = null,Object? hasMore = null,Object? error = freezed,Object? openingId = freezed,Object? pdfError = freezed,}) {
  return _then(PayStubsState(
years: null == years ? _self.years : years // ignore: cast_nullable_to_non_nullable
as List<int>,year: freezed == year ? _self.year : year // ignore: cast_nullable_to_non_nullable
as int?,items: null == items ? _self.items : items // ignore: cast_nullable_to_non_nullable
as List<PayStubSummary>,totalCount: null == totalCount ? _self.totalCount : totalCount // ignore: cast_nullable_to_non_nullable
as int,ytdNet: null == ytdNet ? _self.ytdNet : ytdNet // ignore: cast_nullable_to_non_nullable
as double,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,openingId: freezed == openingId ? _self.openingId : openingId // ignore: cast_nullable_to_non_nullable
as String?,pdfError: freezed == pdfError ? _self.pdfError : pdfError // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [PayStubsState].
extension PayStubsStatePatterns on PayStubsState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PayStubsState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PayStubsState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PayStubsState value)  $default,){
final _that = this;
switch (_that) {
case _PayStubsState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PayStubsState value)?  $default,){
final _that = this;
switch (_that) {
case _PayStubsState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( List<int> years,  int? year,  List<PayStubSummary> items,  int totalCount,  double ytdNet,  bool isLoading,  bool isLoadingMore,  int currentPage,  bool hasMore,  String? error,  String? openingId,  String? pdfError)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PayStubsState() when $default != null:
return $default(_that.years,_that.year,_that.items,_that.totalCount,_that.ytdNet,_that.isLoading,_that.isLoadingMore,_that.currentPage,_that.hasMore,_that.error,_that.openingId,_that.pdfError);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( List<int> years,  int? year,  List<PayStubSummary> items,  int totalCount,  double ytdNet,  bool isLoading,  bool isLoadingMore,  int currentPage,  bool hasMore,  String? error,  String? openingId,  String? pdfError)  $default,) {final _that = this;
switch (_that) {
case _PayStubsState():
return $default(_that.years,_that.year,_that.items,_that.totalCount,_that.ytdNet,_that.isLoading,_that.isLoadingMore,_that.currentPage,_that.hasMore,_that.error,_that.openingId,_that.pdfError);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( List<int> years,  int? year,  List<PayStubSummary> items,  int totalCount,  double ytdNet,  bool isLoading,  bool isLoadingMore,  int currentPage,  bool hasMore,  String? error,  String? openingId,  String? pdfError)?  $default,) {final _that = this;
switch (_that) {
case _PayStubsState() when $default != null:
return $default(_that.years,_that.year,_that.items,_that.totalCount,_that.ytdNet,_that.isLoading,_that.isLoadingMore,_that.currentPage,_that.hasMore,_that.error,_that.openingId,_that.pdfError);case _:
  return null;

}
}

}

/// @nodoc


class _PayStubsState implements PayStubsState {
  const _PayStubsState({ List<int> years = const [], this.year,  List<PayStubSummary> items = const [], this.totalCount = 0, this.ytdNet = 0, this.isLoading = false, this.isLoadingMore = false, this.currentPage = 1, this.hasMore = false, this.error, this.openingId, this.pdfError}): _years = years,_items = items;
  

 final  List<int> _years;
@override@JsonKey() List<int> get years {
  if (_years is EqualUnmodifiableListView) return _years;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_years);
}

@override final  int? year;
 final  List<PayStubSummary> _items;
@override@JsonKey() List<PayStubSummary> get items {
  if (_items is EqualUnmodifiableListView) return _items;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_items);
}

@override@JsonKey() final  int totalCount;
@override@JsonKey() final  double ytdNet;
@override@JsonKey() final  bool isLoading;
@override@JsonKey() final  bool isLoadingMore;
@override@JsonKey() final  int currentPage;
@override@JsonKey() final  bool hasMore;
@override final  String? error;
@override final  String? openingId;
@override final  String? pdfError;

/// Create a copy of PayStubsState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PayStubsStateCopyWith<_PayStubsState> get copyWith => __$PayStubsStateCopyWithImpl<_PayStubsState>(this, _$identity);



@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _PayStubsState&&const DeepCollectionEquality().equals(other.years, _years)&&(identical(other.year, year) || other.year == year)&&const DeepCollectionEquality().equals(other.items, _items)&&(identical(other.totalCount, totalCount) || other.totalCount == totalCount)&&(identical(other.ytdNet, ytdNet) || other.ytdNet == ytdNet)&&(identical(other.isLoading, isLoading) || other.isLoading == isLoading)&&(identical(other.isLoadingMore, isLoadingMore) || other.isLoadingMore == isLoadingMore)&&(identical(other.currentPage, currentPage) || other.currentPage == currentPage)&&(identical(other.hasMore, hasMore) || other.hasMore == hasMore)&&(identical(other.error, error) || other.error == error)&&(identical(other.openingId, openingId) || other.openingId == openingId)&&(identical(other.pdfError, pdfError) || other.pdfError == pdfError));
}


@override
int get hashCode {
    return Object.hash(runtimeType,const DeepCollectionEquality().hash(_years),year,const DeepCollectionEquality().hash(_items),totalCount,ytdNet,isLoading,isLoadingMore,currentPage,hasMore,error,openingId,pdfError);
}

@override
String toString() {
    return 'PayStubsState(years: $years, year: $year, items: $items, totalCount: $totalCount, ytdNet: $ytdNet, isLoading: $isLoading, isLoadingMore: $isLoadingMore, currentPage: $currentPage, hasMore: $hasMore, error: $error, openingId: $openingId, pdfError: $pdfError)';
}


}

/// @nodoc
abstract mixin class _$PayStubsStateCopyWith<$Res> implements $PayStubsStateCopyWith<$Res> {
  factory _$PayStubsStateCopyWith(_PayStubsState value, $Res Function(_PayStubsState) _then) = __$PayStubsStateCopyWithImpl;
@override @useResult
$Res call({
 List<int> years, int? year, List<PayStubSummary> items, int totalCount, double ytdNet, bool isLoading, bool isLoadingMore, int currentPage, bool hasMore, String? error, String? openingId, String? pdfError
});




}
/// @nodoc
class __$PayStubsStateCopyWithImpl<$Res>
    implements _$PayStubsStateCopyWith<$Res> {
  __$PayStubsStateCopyWithImpl(this._self, this._then);

  final _PayStubsState _self;
  final $Res Function(_PayStubsState) _then;

/// Create a copy of PayStubsState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? years = null,Object? year = freezed,Object? items = null,Object? totalCount = null,Object? ytdNet = null,Object? isLoading = null,Object? isLoadingMore = null,Object? currentPage = null,Object? hasMore = null,Object? error = freezed,Object? openingId = freezed,Object? pdfError = freezed,}) {
  return _then(_PayStubsState(
years: null == years ? _self._years : years // ignore: cast_nullable_to_non_nullable
as List<int>,year: freezed == year ? _self.year : year // ignore: cast_nullable_to_non_nullable
as int?,items: null == items ? _self._items : items // ignore: cast_nullable_to_non_nullable
as List<PayStubSummary>,totalCount: null == totalCount ? _self.totalCount : totalCount // ignore: cast_nullable_to_non_nullable
as int,ytdNet: null == ytdNet ? _self.ytdNet : ytdNet // ignore: cast_nullable_to_non_nullable
as double,isLoading: null == isLoading ? _self.isLoading : isLoading // ignore: cast_nullable_to_non_nullable
as bool,isLoadingMore: null == isLoadingMore ? _self.isLoadingMore : isLoadingMore // ignore: cast_nullable_to_non_nullable
as bool,currentPage: null == currentPage ? _self.currentPage : currentPage // ignore: cast_nullable_to_non_nullable
as int,hasMore: null == hasMore ? _self.hasMore : hasMore // ignore: cast_nullable_to_non_nullable
as bool,error: freezed == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String?,openingId: freezed == openingId ? _self.openingId : openingId // ignore: cast_nullable_to_non_nullable
as String?,pdfError: freezed == pdfError ? _self.pdfError : pdfError // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on
