// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'paginated_jobs_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$PaginatedJobsModel {

 List<JobModel> get items; int get pageIndex; int get totalPages; int get totalItems;
/// Create a copy of PaginatedJobsModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PaginatedJobsModelCopyWith<PaginatedJobsModel> get copyWith => _$PaginatedJobsModelCopyWithImpl<PaginatedJobsModel>(this as PaginatedJobsModel, _$identity);

  /// Serializes this PaginatedJobsModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PaginatedJobsModel&&const DeepCollectionEquality().equals(other.items, items)&&(identical(other.pageIndex, pageIndex) || other.pageIndex == pageIndex)&&(identical(other.totalPages, totalPages) || other.totalPages == totalPages)&&(identical(other.totalItems, totalItems) || other.totalItems == totalItems));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(items),pageIndex,totalPages,totalItems);

@override
String toString() {
  return 'PaginatedJobsModel(items: $items, pageIndex: $pageIndex, totalPages: $totalPages, totalItems: $totalItems)';
}


}

/// @nodoc
abstract mixin class $PaginatedJobsModelCopyWith<$Res>  {
  factory $PaginatedJobsModelCopyWith(PaginatedJobsModel value, $Res Function(PaginatedJobsModel) _then) = _$PaginatedJobsModelCopyWithImpl;
@useResult
$Res call({
 List<JobModel> items, int pageIndex, int totalPages, int totalItems
});




}
/// @nodoc
class _$PaginatedJobsModelCopyWithImpl<$Res>
    implements $PaginatedJobsModelCopyWith<$Res> {
  _$PaginatedJobsModelCopyWithImpl(this._self, this._then);

  final PaginatedJobsModel _self;
  final $Res Function(PaginatedJobsModel) _then;

/// Create a copy of PaginatedJobsModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? items = null,Object? pageIndex = null,Object? totalPages = null,Object? totalItems = null,}) {
  return _then(_self.copyWith(
items: null == items ? _self.items : items // ignore: cast_nullable_to_non_nullable
as List<JobModel>,pageIndex: null == pageIndex ? _self.pageIndex : pageIndex // ignore: cast_nullable_to_non_nullable
as int,totalPages: null == totalPages ? _self.totalPages : totalPages // ignore: cast_nullable_to_non_nullable
as int,totalItems: null == totalItems ? _self.totalItems : totalItems // ignore: cast_nullable_to_non_nullable
as int,
  ));
}

}


/// Adds pattern-matching-related methods to [PaginatedJobsModel].
extension PaginatedJobsModelPatterns on PaginatedJobsModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PaginatedJobsModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PaginatedJobsModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PaginatedJobsModel value)  $default,){
final _that = this;
switch (_that) {
case _PaginatedJobsModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PaginatedJobsModel value)?  $default,){
final _that = this;
switch (_that) {
case _PaginatedJobsModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( List<JobModel> items,  int pageIndex,  int totalPages,  int totalItems)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PaginatedJobsModel() when $default != null:
return $default(_that.items,_that.pageIndex,_that.totalPages,_that.totalItems);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( List<JobModel> items,  int pageIndex,  int totalPages,  int totalItems)  $default,) {final _that = this;
switch (_that) {
case _PaginatedJobsModel():
return $default(_that.items,_that.pageIndex,_that.totalPages,_that.totalItems);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( List<JobModel> items,  int pageIndex,  int totalPages,  int totalItems)?  $default,) {final _that = this;
switch (_that) {
case _PaginatedJobsModel() when $default != null:
return $default(_that.items,_that.pageIndex,_that.totalPages,_that.totalItems);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _PaginatedJobsModel extends PaginatedJobsModel {
  const _PaginatedJobsModel({required final  List<JobModel> items, required this.pageIndex, required this.totalPages, required this.totalItems}): _items = items,super._();
  factory _PaginatedJobsModel.fromJson(Map<String, dynamic> json) => _$PaginatedJobsModelFromJson(json);

 final  List<JobModel> _items;
@override List<JobModel> get items {
  if (_items is EqualUnmodifiableListView) return _items;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_items);
}

@override final  int pageIndex;
@override final  int totalPages;
@override final  int totalItems;

/// Create a copy of PaginatedJobsModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PaginatedJobsModelCopyWith<_PaginatedJobsModel> get copyWith => __$PaginatedJobsModelCopyWithImpl<_PaginatedJobsModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$PaginatedJobsModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _PaginatedJobsModel&&const DeepCollectionEquality().equals(other._items, _items)&&(identical(other.pageIndex, pageIndex) || other.pageIndex == pageIndex)&&(identical(other.totalPages, totalPages) || other.totalPages == totalPages)&&(identical(other.totalItems, totalItems) || other.totalItems == totalItems));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_items),pageIndex,totalPages,totalItems);

@override
String toString() {
  return 'PaginatedJobsModel(items: $items, pageIndex: $pageIndex, totalPages: $totalPages, totalItems: $totalItems)';
}


}

/// @nodoc
abstract mixin class _$PaginatedJobsModelCopyWith<$Res> implements $PaginatedJobsModelCopyWith<$Res> {
  factory _$PaginatedJobsModelCopyWith(_PaginatedJobsModel value, $Res Function(_PaginatedJobsModel) _then) = __$PaginatedJobsModelCopyWithImpl;
@override @useResult
$Res call({
 List<JobModel> items, int pageIndex, int totalPages, int totalItems
});




}
/// @nodoc
class __$PaginatedJobsModelCopyWithImpl<$Res>
    implements _$PaginatedJobsModelCopyWith<$Res> {
  __$PaginatedJobsModelCopyWithImpl(this._self, this._then);

  final _PaginatedJobsModel _self;
  final $Res Function(_PaginatedJobsModel) _then;

/// Create a copy of PaginatedJobsModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? items = null,Object? pageIndex = null,Object? totalPages = null,Object? totalItems = null,}) {
  return _then(_PaginatedJobsModel(
items: null == items ? _self._items : items // ignore: cast_nullable_to_non_nullable
as List<JobModel>,pageIndex: null == pageIndex ? _self.pageIndex : pageIndex // ignore: cast_nullable_to_non_nullable
as int,totalPages: null == totalPages ? _self.totalPages : totalPages // ignore: cast_nullable_to_non_nullable
as int,totalItems: null == totalItems ? _self.totalItems : totalItems // ignore: cast_nullable_to_non_nullable
as int,
  ));
}


}

// dart format on
