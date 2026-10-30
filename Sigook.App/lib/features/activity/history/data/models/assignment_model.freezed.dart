// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'assignment_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AssignmentModel {

 String get id; int get numberId; String? get jobTitle; String? get clientName; String? get location; String? get status; DateTime? get startAt; DateTime? get startWorking; DateTime? get endedAt; double get hoursWorked;
/// Create a copy of AssignmentModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AssignmentModelCopyWith<AssignmentModel> get copyWith => _$AssignmentModelCopyWithImpl<AssignmentModel>(this as AssignmentModel, _$identity);

  /// Serializes this AssignmentModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  final _this = this as AssignmentModel;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AssignmentModel&&(identical(other.id, _this.id) || other.id == _this.id)&&(identical(other.numberId, _this.numberId) || other.numberId == _this.numberId)&&(identical(other.jobTitle, _this.jobTitle) || other.jobTitle == _this.jobTitle)&&(identical(other.clientName, _this.clientName) || other.clientName == _this.clientName)&&(identical(other.location, _this.location) || other.location == _this.location)&&(identical(other.status, _this.status) || other.status == _this.status)&&(identical(other.startAt, _this.startAt) || other.startAt == _this.startAt)&&(identical(other.startWorking, _this.startWorking) || other.startWorking == _this.startWorking)&&(identical(other.endedAt, _this.endedAt) || other.endedAt == _this.endedAt)&&(identical(other.hoursWorked, _this.hoursWorked) || other.hoursWorked == _this.hoursWorked));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
  final _this = this as AssignmentModel;
  return Object.hash(runtimeType,_this.id,_this.numberId,_this.jobTitle,_this.clientName,_this.location,_this.status,_this.startAt,_this.startWorking,_this.endedAt,_this.hoursWorked);
}

@override
String toString() {
  final _this = this as AssignmentModel;
  return 'AssignmentModel(id: ${_this.id}, numberId: ${_this.numberId}, jobTitle: ${_this.jobTitle}, clientName: ${_this.clientName}, location: ${_this.location}, status: ${_this.status}, startAt: ${_this.startAt}, startWorking: ${_this.startWorking}, endedAt: ${_this.endedAt}, hoursWorked: ${_this.hoursWorked})';
}


}

/// @nodoc
abstract mixin class $AssignmentModelCopyWith<$Res>  {
  factory $AssignmentModelCopyWith(AssignmentModel value, $Res Function(AssignmentModel) _then) = _$AssignmentModelCopyWithImpl;
@useResult
$Res call({
 String id, int numberId, String? jobTitle, String? clientName, String? location, String? status, DateTime? startAt, DateTime? startWorking, DateTime? endedAt, double hoursWorked
});




}
/// @nodoc
class _$AssignmentModelCopyWithImpl<$Res>
    implements $AssignmentModelCopyWith<$Res> {
  _$AssignmentModelCopyWithImpl(this._self, this._then);

  final AssignmentModel _self;
  final $Res Function(AssignmentModel) _then;

/// Create a copy of AssignmentModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? numberId = null,Object? jobTitle = freezed,Object? clientName = freezed,Object? location = freezed,Object? status = freezed,Object? startAt = freezed,Object? startWorking = freezed,Object? endedAt = freezed,Object? hoursWorked = null,}) {
  return _then(AssignmentModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,jobTitle: freezed == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String?,clientName: freezed == clientName ? _self.clientName : clientName // ignore: cast_nullable_to_non_nullable
as String?,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as String?,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,startAt: freezed == startAt ? _self.startAt : startAt // ignore: cast_nullable_to_non_nullable
as DateTime?,startWorking: freezed == startWorking ? _self.startWorking : startWorking // ignore: cast_nullable_to_non_nullable
as DateTime?,endedAt: freezed == endedAt ? _self.endedAt : endedAt // ignore: cast_nullable_to_non_nullable
as DateTime?,hoursWorked: null == hoursWorked ? _self.hoursWorked : hoursWorked // ignore: cast_nullable_to_non_nullable
as double,
  ));
}

}


/// Adds pattern-matching-related methods to [AssignmentModel].
extension AssignmentModelPatterns on AssignmentModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AssignmentModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AssignmentModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AssignmentModel value)  $default,){
final _that = this;
switch (_that) {
case _AssignmentModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AssignmentModel value)?  $default,){
final _that = this;
switch (_that) {
case _AssignmentModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  int numberId,  String? jobTitle,  String? clientName,  String? location,  String? status,  DateTime? startAt,  DateTime? startWorking,  DateTime? endedAt,  double hoursWorked)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AssignmentModel() when $default != null:
return $default(_that.id,_that.numberId,_that.jobTitle,_that.clientName,_that.location,_that.status,_that.startAt,_that.startWorking,_that.endedAt,_that.hoursWorked);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  int numberId,  String? jobTitle,  String? clientName,  String? location,  String? status,  DateTime? startAt,  DateTime? startWorking,  DateTime? endedAt,  double hoursWorked)  $default,) {final _that = this;
switch (_that) {
case _AssignmentModel():
return $default(_that.id,_that.numberId,_that.jobTitle,_that.clientName,_that.location,_that.status,_that.startAt,_that.startWorking,_that.endedAt,_that.hoursWorked);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  int numberId,  String? jobTitle,  String? clientName,  String? location,  String? status,  DateTime? startAt,  DateTime? startWorking,  DateTime? endedAt,  double hoursWorked)?  $default,) {final _that = this;
switch (_that) {
case _AssignmentModel() when $default != null:
return $default(_that.id,_that.numberId,_that.jobTitle,_that.clientName,_that.location,_that.status,_that.startAt,_that.startWorking,_that.endedAt,_that.hoursWorked);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AssignmentModel extends AssignmentModel {
  const _AssignmentModel({required this.id, required this.numberId, this.jobTitle, this.clientName, this.location, this.status, this.startAt, this.startWorking, this.endedAt, this.hoursWorked = 0}): super._();
  factory _AssignmentModel.fromJson(Map<String, dynamic> json) => _$AssignmentModelFromJson(json);

@override final  String id;
@override final  int numberId;
@override final  String? jobTitle;
@override final  String? clientName;
@override final  String? location;
@override final  String? status;
@override final  DateTime? startAt;
@override final  DateTime? startWorking;
@override final  DateTime? endedAt;
@override@JsonKey() final  double hoursWorked;

/// Create a copy of AssignmentModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AssignmentModelCopyWith<_AssignmentModel> get copyWith => __$AssignmentModelCopyWithImpl<_AssignmentModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AssignmentModelToJson(this, );
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _AssignmentModel&&(identical(other.id, id) || other.id == id)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.jobTitle, jobTitle) || other.jobTitle == jobTitle)&&(identical(other.clientName, clientName) || other.clientName == clientName)&&(identical(other.location, location) || other.location == location)&&(identical(other.status, status) || other.status == status)&&(identical(other.startAt, startAt) || other.startAt == startAt)&&(identical(other.startWorking, startWorking) || other.startWorking == startWorking)&&(identical(other.endedAt, endedAt) || other.endedAt == endedAt)&&(identical(other.hoursWorked, hoursWorked) || other.hoursWorked == hoursWorked));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
    return Object.hash(runtimeType,id,numberId,jobTitle,clientName,location,status,startAt,startWorking,endedAt,hoursWorked);
}

@override
String toString() {
    return 'AssignmentModel(id: $id, numberId: $numberId, jobTitle: $jobTitle, clientName: $clientName, location: $location, status: $status, startAt: $startAt, startWorking: $startWorking, endedAt: $endedAt, hoursWorked: $hoursWorked)';
}


}

/// @nodoc
abstract mixin class _$AssignmentModelCopyWith<$Res> implements $AssignmentModelCopyWith<$Res> {
  factory _$AssignmentModelCopyWith(_AssignmentModel value, $Res Function(_AssignmentModel) _then) = __$AssignmentModelCopyWithImpl;
@override @useResult
$Res call({
 String id, int numberId, String? jobTitle, String? clientName, String? location, String? status, DateTime? startAt, DateTime? startWorking, DateTime? endedAt, double hoursWorked
});




}
/// @nodoc
class __$AssignmentModelCopyWithImpl<$Res>
    implements _$AssignmentModelCopyWith<$Res> {
  __$AssignmentModelCopyWithImpl(this._self, this._then);

  final _AssignmentModel _self;
  final $Res Function(_AssignmentModel) _then;

/// Create a copy of AssignmentModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? numberId = null,Object? jobTitle = freezed,Object? clientName = freezed,Object? location = freezed,Object? status = freezed,Object? startAt = freezed,Object? startWorking = freezed,Object? endedAt = freezed,Object? hoursWorked = null,}) {
  return _then(_AssignmentModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,jobTitle: freezed == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String?,clientName: freezed == clientName ? _self.clientName : clientName // ignore: cast_nullable_to_non_nullable
as String?,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as String?,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,startAt: freezed == startAt ? _self.startAt : startAt // ignore: cast_nullable_to_non_nullable
as DateTime?,startWorking: freezed == startWorking ? _self.startWorking : startWorking // ignore: cast_nullable_to_non_nullable
as DateTime?,endedAt: freezed == endedAt ? _self.endedAt : endedAt // ignore: cast_nullable_to_non_nullable
as DateTime?,hoursWorked: null == hoursWorked ? _self.hoursWorked : hoursWorked // ignore: cast_nullable_to_non_nullable
as double,
  ));
}


}

// dart format on
