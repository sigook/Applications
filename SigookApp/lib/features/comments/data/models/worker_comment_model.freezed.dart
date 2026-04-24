// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'worker_comment_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$WorkerCommentModel {

 String get id; String get comment; double get rate; String? get logo; int get numberId; DateTime get createdAt;
/// Create a copy of WorkerCommentModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WorkerCommentModelCopyWith<WorkerCommentModel> get copyWith => _$WorkerCommentModelCopyWithImpl<WorkerCommentModel>(this as WorkerCommentModel, _$identity);

  /// Serializes this WorkerCommentModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WorkerCommentModel&&(identical(other.id, id) || other.id == id)&&(identical(other.comment, comment) || other.comment == comment)&&(identical(other.rate, rate) || other.rate == rate)&&(identical(other.logo, logo) || other.logo == logo)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,comment,rate,logo,numberId,createdAt);

@override
String toString() {
  return 'WorkerCommentModel(id: $id, comment: $comment, rate: $rate, logo: $logo, numberId: $numberId, createdAt: $createdAt)';
}


}

/// @nodoc
abstract mixin class $WorkerCommentModelCopyWith<$Res>  {
  factory $WorkerCommentModelCopyWith(WorkerCommentModel value, $Res Function(WorkerCommentModel) _then) = _$WorkerCommentModelCopyWithImpl;
@useResult
$Res call({
 String id, String comment, double rate, String? logo, int numberId, DateTime createdAt
});




}
/// @nodoc
class _$WorkerCommentModelCopyWithImpl<$Res>
    implements $WorkerCommentModelCopyWith<$Res> {
  _$WorkerCommentModelCopyWithImpl(this._self, this._then);

  final WorkerCommentModel _self;
  final $Res Function(WorkerCommentModel) _then;

/// Create a copy of WorkerCommentModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? comment = null,Object? rate = null,Object? logo = freezed,Object? numberId = null,Object? createdAt = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,comment: null == comment ? _self.comment : comment // ignore: cast_nullable_to_non_nullable
as String,rate: null == rate ? _self.rate : rate // ignore: cast_nullable_to_non_nullable
as double,logo: freezed == logo ? _self.logo : logo // ignore: cast_nullable_to_non_nullable
as String?,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,
  ));
}

}


/// Adds pattern-matching-related methods to [WorkerCommentModel].
extension WorkerCommentModelPatterns on WorkerCommentModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WorkerCommentModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WorkerCommentModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WorkerCommentModel value)  $default,){
final _that = this;
switch (_that) {
case _WorkerCommentModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WorkerCommentModel value)?  $default,){
final _that = this;
switch (_that) {
case _WorkerCommentModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String comment,  double rate,  String? logo,  int numberId,  DateTime createdAt)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WorkerCommentModel() when $default != null:
return $default(_that.id,_that.comment,_that.rate,_that.logo,_that.numberId,_that.createdAt);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String comment,  double rate,  String? logo,  int numberId,  DateTime createdAt)  $default,) {final _that = this;
switch (_that) {
case _WorkerCommentModel():
return $default(_that.id,_that.comment,_that.rate,_that.logo,_that.numberId,_that.createdAt);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String comment,  double rate,  String? logo,  int numberId,  DateTime createdAt)?  $default,) {final _that = this;
switch (_that) {
case _WorkerCommentModel() when $default != null:
return $default(_that.id,_that.comment,_that.rate,_that.logo,_that.numberId,_that.createdAt);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WorkerCommentModel extends WorkerCommentModel {
  const _WorkerCommentModel({required this.id, required this.comment, required this.rate, this.logo, required this.numberId, required this.createdAt}): super._();
  factory _WorkerCommentModel.fromJson(Map<String, dynamic> json) => _$WorkerCommentModelFromJson(json);

@override final  String id;
@override final  String comment;
@override final  double rate;
@override final  String? logo;
@override final  int numberId;
@override final  DateTime createdAt;

/// Create a copy of WorkerCommentModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WorkerCommentModelCopyWith<_WorkerCommentModel> get copyWith => __$WorkerCommentModelCopyWithImpl<_WorkerCommentModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WorkerCommentModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _WorkerCommentModel&&(identical(other.id, id) || other.id == id)&&(identical(other.comment, comment) || other.comment == comment)&&(identical(other.rate, rate) || other.rate == rate)&&(identical(other.logo, logo) || other.logo == logo)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,comment,rate,logo,numberId,createdAt);

@override
String toString() {
  return 'WorkerCommentModel(id: $id, comment: $comment, rate: $rate, logo: $logo, numberId: $numberId, createdAt: $createdAt)';
}


}

/// @nodoc
abstract mixin class _$WorkerCommentModelCopyWith<$Res> implements $WorkerCommentModelCopyWith<$Res> {
  factory _$WorkerCommentModelCopyWith(_WorkerCommentModel value, $Res Function(_WorkerCommentModel) _then) = __$WorkerCommentModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String comment, double rate, String? logo, int numberId, DateTime createdAt
});




}
/// @nodoc
class __$WorkerCommentModelCopyWithImpl<$Res>
    implements _$WorkerCommentModelCopyWith<$Res> {
  __$WorkerCommentModelCopyWithImpl(this._self, this._then);

  final _WorkerCommentModel _self;
  final $Res Function(_WorkerCommentModel) _then;

/// Create a copy of WorkerCommentModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? comment = null,Object? rate = null,Object? logo = freezed,Object? numberId = null,Object? createdAt = null,}) {
  return _then(_WorkerCommentModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,comment: null == comment ? _self.comment : comment // ignore: cast_nullable_to_non_nullable
as String,rate: null == rate ? _self.rate : rate // ignore: cast_nullable_to_non_nullable
as double,logo: freezed == logo ? _self.logo : logo // ignore: cast_nullable_to_non_nullable
as String?,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,
  ));
}


}

// dart format on
