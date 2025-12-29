// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'job_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$JobModel {

 String get id; String get jobTitle; int get numberId; int get workersQuantity; String? get location; String? get entrance; String get agencyFullName; String? get agencyLogo; String? get status; bool get isAsap; bool? get workerApprovedToWork; double get workerRate; double? get workerSalary; DateTime get createdAt; DateTime? get finishAt; DateTime get startAt; String? get durationTerm;
/// Create a copy of JobModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$JobModelCopyWith<JobModel> get copyWith => _$JobModelCopyWithImpl<JobModel>(this as JobModel, _$identity);

  /// Serializes this JobModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is JobModel&&(identical(other.id, id) || other.id == id)&&(identical(other.jobTitle, jobTitle) || other.jobTitle == jobTitle)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.workersQuantity, workersQuantity) || other.workersQuantity == workersQuantity)&&(identical(other.location, location) || other.location == location)&&(identical(other.entrance, entrance) || other.entrance == entrance)&&(identical(other.agencyFullName, agencyFullName) || other.agencyFullName == agencyFullName)&&(identical(other.agencyLogo, agencyLogo) || other.agencyLogo == agencyLogo)&&(identical(other.status, status) || other.status == status)&&(identical(other.isAsap, isAsap) || other.isAsap == isAsap)&&(identical(other.workerApprovedToWork, workerApprovedToWork) || other.workerApprovedToWork == workerApprovedToWork)&&(identical(other.workerRate, workerRate) || other.workerRate == workerRate)&&(identical(other.workerSalary, workerSalary) || other.workerSalary == workerSalary)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.finishAt, finishAt) || other.finishAt == finishAt)&&(identical(other.startAt, startAt) || other.startAt == startAt)&&(identical(other.durationTerm, durationTerm) || other.durationTerm == durationTerm));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,jobTitle,numberId,workersQuantity,location,entrance,agencyFullName,agencyLogo,status,isAsap,workerApprovedToWork,workerRate,workerSalary,createdAt,finishAt,startAt,durationTerm);

@override
String toString() {
  return 'JobModel(id: $id, jobTitle: $jobTitle, numberId: $numberId, workersQuantity: $workersQuantity, location: $location, entrance: $entrance, agencyFullName: $agencyFullName, agencyLogo: $agencyLogo, status: $status, isAsap: $isAsap, workerApprovedToWork: $workerApprovedToWork, workerRate: $workerRate, workerSalary: $workerSalary, createdAt: $createdAt, finishAt: $finishAt, startAt: $startAt, durationTerm: $durationTerm)';
}


}

/// @nodoc
abstract mixin class $JobModelCopyWith<$Res>  {
  factory $JobModelCopyWith(JobModel value, $Res Function(JobModel) _then) = _$JobModelCopyWithImpl;
@useResult
$Res call({
 String id, String jobTitle, int numberId, int workersQuantity, String? location, String? entrance, String agencyFullName, String? agencyLogo, String? status, bool isAsap, bool? workerApprovedToWork, double workerRate, double? workerSalary, DateTime createdAt, DateTime? finishAt, DateTime startAt, String? durationTerm
});




}
/// @nodoc
class _$JobModelCopyWithImpl<$Res>
    implements $JobModelCopyWith<$Res> {
  _$JobModelCopyWithImpl(this._self, this._then);

  final JobModel _self;
  final $Res Function(JobModel) _then;

/// Create a copy of JobModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? jobTitle = null,Object? numberId = null,Object? workersQuantity = null,Object? location = freezed,Object? entrance = freezed,Object? agencyFullName = null,Object? agencyLogo = freezed,Object? status = freezed,Object? isAsap = null,Object? workerApprovedToWork = freezed,Object? workerRate = null,Object? workerSalary = freezed,Object? createdAt = null,Object? finishAt = freezed,Object? startAt = null,Object? durationTerm = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,jobTitle: null == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,workersQuantity: null == workersQuantity ? _self.workersQuantity : workersQuantity // ignore: cast_nullable_to_non_nullable
as int,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as String?,entrance: freezed == entrance ? _self.entrance : entrance // ignore: cast_nullable_to_non_nullable
as String?,agencyFullName: null == agencyFullName ? _self.agencyFullName : agencyFullName // ignore: cast_nullable_to_non_nullable
as String,agencyLogo: freezed == agencyLogo ? _self.agencyLogo : agencyLogo // ignore: cast_nullable_to_non_nullable
as String?,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,isAsap: null == isAsap ? _self.isAsap : isAsap // ignore: cast_nullable_to_non_nullable
as bool,workerApprovedToWork: freezed == workerApprovedToWork ? _self.workerApprovedToWork : workerApprovedToWork // ignore: cast_nullable_to_non_nullable
as bool?,workerRate: null == workerRate ? _self.workerRate : workerRate // ignore: cast_nullable_to_non_nullable
as double,workerSalary: freezed == workerSalary ? _self.workerSalary : workerSalary // ignore: cast_nullable_to_non_nullable
as double?,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,finishAt: freezed == finishAt ? _self.finishAt : finishAt // ignore: cast_nullable_to_non_nullable
as DateTime?,startAt: null == startAt ? _self.startAt : startAt // ignore: cast_nullable_to_non_nullable
as DateTime,durationTerm: freezed == durationTerm ? _self.durationTerm : durationTerm // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [JobModel].
extension JobModelPatterns on JobModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _JobModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _JobModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _JobModel value)  $default,){
final _that = this;
switch (_that) {
case _JobModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _JobModel value)?  $default,){
final _that = this;
switch (_that) {
case _JobModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String jobTitle,  int numberId,  int workersQuantity,  String? location,  String? entrance,  String agencyFullName,  String? agencyLogo,  String? status,  bool isAsap,  bool? workerApprovedToWork,  double workerRate,  double? workerSalary,  DateTime createdAt,  DateTime? finishAt,  DateTime startAt,  String? durationTerm)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _JobModel() when $default != null:
return $default(_that.id,_that.jobTitle,_that.numberId,_that.workersQuantity,_that.location,_that.entrance,_that.agencyFullName,_that.agencyLogo,_that.status,_that.isAsap,_that.workerApprovedToWork,_that.workerRate,_that.workerSalary,_that.createdAt,_that.finishAt,_that.startAt,_that.durationTerm);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String jobTitle,  int numberId,  int workersQuantity,  String? location,  String? entrance,  String agencyFullName,  String? agencyLogo,  String? status,  bool isAsap,  bool? workerApprovedToWork,  double workerRate,  double? workerSalary,  DateTime createdAt,  DateTime? finishAt,  DateTime startAt,  String? durationTerm)  $default,) {final _that = this;
switch (_that) {
case _JobModel():
return $default(_that.id,_that.jobTitle,_that.numberId,_that.workersQuantity,_that.location,_that.entrance,_that.agencyFullName,_that.agencyLogo,_that.status,_that.isAsap,_that.workerApprovedToWork,_that.workerRate,_that.workerSalary,_that.createdAt,_that.finishAt,_that.startAt,_that.durationTerm);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String jobTitle,  int numberId,  int workersQuantity,  String? location,  String? entrance,  String agencyFullName,  String? agencyLogo,  String? status,  bool isAsap,  bool? workerApprovedToWork,  double workerRate,  double? workerSalary,  DateTime createdAt,  DateTime? finishAt,  DateTime startAt,  String? durationTerm)?  $default,) {final _that = this;
switch (_that) {
case _JobModel() when $default != null:
return $default(_that.id,_that.jobTitle,_that.numberId,_that.workersQuantity,_that.location,_that.entrance,_that.agencyFullName,_that.agencyLogo,_that.status,_that.isAsap,_that.workerApprovedToWork,_that.workerRate,_that.workerSalary,_that.createdAt,_that.finishAt,_that.startAt,_that.durationTerm);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _JobModel extends JobModel {
  const _JobModel({required this.id, required this.jobTitle, required this.numberId, required this.workersQuantity, this.location, this.entrance, required this.agencyFullName, this.agencyLogo, this.status, required this.isAsap, this.workerApprovedToWork, required this.workerRate, this.workerSalary, required this.createdAt, this.finishAt, required this.startAt, this.durationTerm}): super._();
  factory _JobModel.fromJson(Map<String, dynamic> json) => _$JobModelFromJson(json);

@override final  String id;
@override final  String jobTitle;
@override final  int numberId;
@override final  int workersQuantity;
@override final  String? location;
@override final  String? entrance;
@override final  String agencyFullName;
@override final  String? agencyLogo;
@override final  String? status;
@override final  bool isAsap;
@override final  bool? workerApprovedToWork;
@override final  double workerRate;
@override final  double? workerSalary;
@override final  DateTime createdAt;
@override final  DateTime? finishAt;
@override final  DateTime startAt;
@override final  String? durationTerm;

/// Create a copy of JobModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$JobModelCopyWith<_JobModel> get copyWith => __$JobModelCopyWithImpl<_JobModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$JobModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _JobModel&&(identical(other.id, id) || other.id == id)&&(identical(other.jobTitle, jobTitle) || other.jobTitle == jobTitle)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.workersQuantity, workersQuantity) || other.workersQuantity == workersQuantity)&&(identical(other.location, location) || other.location == location)&&(identical(other.entrance, entrance) || other.entrance == entrance)&&(identical(other.agencyFullName, agencyFullName) || other.agencyFullName == agencyFullName)&&(identical(other.agencyLogo, agencyLogo) || other.agencyLogo == agencyLogo)&&(identical(other.status, status) || other.status == status)&&(identical(other.isAsap, isAsap) || other.isAsap == isAsap)&&(identical(other.workerApprovedToWork, workerApprovedToWork) || other.workerApprovedToWork == workerApprovedToWork)&&(identical(other.workerRate, workerRate) || other.workerRate == workerRate)&&(identical(other.workerSalary, workerSalary) || other.workerSalary == workerSalary)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.finishAt, finishAt) || other.finishAt == finishAt)&&(identical(other.startAt, startAt) || other.startAt == startAt)&&(identical(other.durationTerm, durationTerm) || other.durationTerm == durationTerm));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,jobTitle,numberId,workersQuantity,location,entrance,agencyFullName,agencyLogo,status,isAsap,workerApprovedToWork,workerRate,workerSalary,createdAt,finishAt,startAt,durationTerm);

@override
String toString() {
  return 'JobModel(id: $id, jobTitle: $jobTitle, numberId: $numberId, workersQuantity: $workersQuantity, location: $location, entrance: $entrance, agencyFullName: $agencyFullName, agencyLogo: $agencyLogo, status: $status, isAsap: $isAsap, workerApprovedToWork: $workerApprovedToWork, workerRate: $workerRate, workerSalary: $workerSalary, createdAt: $createdAt, finishAt: $finishAt, startAt: $startAt, durationTerm: $durationTerm)';
}


}

/// @nodoc
abstract mixin class _$JobModelCopyWith<$Res> implements $JobModelCopyWith<$Res> {
  factory _$JobModelCopyWith(_JobModel value, $Res Function(_JobModel) _then) = __$JobModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String jobTitle, int numberId, int workersQuantity, String? location, String? entrance, String agencyFullName, String? agencyLogo, String? status, bool isAsap, bool? workerApprovedToWork, double workerRate, double? workerSalary, DateTime createdAt, DateTime? finishAt, DateTime startAt, String? durationTerm
});




}
/// @nodoc
class __$JobModelCopyWithImpl<$Res>
    implements _$JobModelCopyWith<$Res> {
  __$JobModelCopyWithImpl(this._self, this._then);

  final _JobModel _self;
  final $Res Function(_JobModel) _then;

/// Create a copy of JobModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? jobTitle = null,Object? numberId = null,Object? workersQuantity = null,Object? location = freezed,Object? entrance = freezed,Object? agencyFullName = null,Object? agencyLogo = freezed,Object? status = freezed,Object? isAsap = null,Object? workerApprovedToWork = freezed,Object? workerRate = null,Object? workerSalary = freezed,Object? createdAt = null,Object? finishAt = freezed,Object? startAt = null,Object? durationTerm = freezed,}) {
  return _then(_JobModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,jobTitle: null == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String,numberId: null == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int,workersQuantity: null == workersQuantity ? _self.workersQuantity : workersQuantity // ignore: cast_nullable_to_non_nullable
as int,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as String?,entrance: freezed == entrance ? _self.entrance : entrance // ignore: cast_nullable_to_non_nullable
as String?,agencyFullName: null == agencyFullName ? _self.agencyFullName : agencyFullName // ignore: cast_nullable_to_non_nullable
as String,agencyLogo: freezed == agencyLogo ? _self.agencyLogo : agencyLogo // ignore: cast_nullable_to_non_nullable
as String?,status: freezed == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String?,isAsap: null == isAsap ? _self.isAsap : isAsap // ignore: cast_nullable_to_non_nullable
as bool,workerApprovedToWork: freezed == workerApprovedToWork ? _self.workerApprovedToWork : workerApprovedToWork // ignore: cast_nullable_to_non_nullable
as bool?,workerRate: null == workerRate ? _self.workerRate : workerRate // ignore: cast_nullable_to_non_nullable
as double,workerSalary: freezed == workerSalary ? _self.workerSalary : workerSalary // ignore: cast_nullable_to_non_nullable
as double?,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,finishAt: freezed == finishAt ? _self.finishAt : finishAt // ignore: cast_nullable_to_non_nullable
as DateTime?,startAt: null == startAt ? _self.startAt : startAt // ignore: cast_nullable_to_non_nullable
as DateTime,durationTerm: freezed == durationTerm ? _self.durationTerm : durationTerm // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on
