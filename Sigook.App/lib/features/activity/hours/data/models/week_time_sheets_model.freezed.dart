// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'week_time_sheets_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$DayTimeSheetModel {

 String get id; DateTime get date; String? get clientName; String? get jobTitle; DateTime? get clockIn; DateTime? get clockOut; String? get durationBreak; bool get breakIsPaid; double get regularHours; double get overtimeHours; double get totalHours; int get status; String? get missingHours; String? get missingHoursOvertime; double get bonusOrOthers; String? get bonusOrOthersDescription; double get deductionsOthers; String? get deductionsOthersDescription; double get reimbursements; String? get reimbursementsDescription;
/// Create a copy of DayTimeSheetModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$DayTimeSheetModelCopyWith<DayTimeSheetModel> get copyWith => _$DayTimeSheetModelCopyWithImpl<DayTimeSheetModel>(this as DayTimeSheetModel, _$identity);

  /// Serializes this DayTimeSheetModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  final _this = this as DayTimeSheetModel;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is DayTimeSheetModel&&(identical(other.id, _this.id) || other.id == _this.id)&&(identical(other.date, _this.date) || other.date == _this.date)&&(identical(other.clientName, _this.clientName) || other.clientName == _this.clientName)&&(identical(other.jobTitle, _this.jobTitle) || other.jobTitle == _this.jobTitle)&&(identical(other.clockIn, _this.clockIn) || other.clockIn == _this.clockIn)&&(identical(other.clockOut, _this.clockOut) || other.clockOut == _this.clockOut)&&(identical(other.durationBreak, _this.durationBreak) || other.durationBreak == _this.durationBreak)&&(identical(other.breakIsPaid, _this.breakIsPaid) || other.breakIsPaid == _this.breakIsPaid)&&(identical(other.regularHours, _this.regularHours) || other.regularHours == _this.regularHours)&&(identical(other.overtimeHours, _this.overtimeHours) || other.overtimeHours == _this.overtimeHours)&&(identical(other.totalHours, _this.totalHours) || other.totalHours == _this.totalHours)&&(identical(other.status, _this.status) || other.status == _this.status)&&(identical(other.missingHours, _this.missingHours) || other.missingHours == _this.missingHours)&&(identical(other.missingHoursOvertime, _this.missingHoursOvertime) || other.missingHoursOvertime == _this.missingHoursOvertime)&&(identical(other.bonusOrOthers, _this.bonusOrOthers) || other.bonusOrOthers == _this.bonusOrOthers)&&(identical(other.bonusOrOthersDescription, _this.bonusOrOthersDescription) || other.bonusOrOthersDescription == _this.bonusOrOthersDescription)&&(identical(other.deductionsOthers, _this.deductionsOthers) || other.deductionsOthers == _this.deductionsOthers)&&(identical(other.deductionsOthersDescription, _this.deductionsOthersDescription) || other.deductionsOthersDescription == _this.deductionsOthersDescription)&&(identical(other.reimbursements, _this.reimbursements) || other.reimbursements == _this.reimbursements)&&(identical(other.reimbursementsDescription, _this.reimbursementsDescription) || other.reimbursementsDescription == _this.reimbursementsDescription));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
  final _this = this as DayTimeSheetModel;
  return Object.hashAll([runtimeType,_this.id,_this.date,_this.clientName,_this.jobTitle,_this.clockIn,_this.clockOut,_this.durationBreak,_this.breakIsPaid,_this.regularHours,_this.overtimeHours,_this.totalHours,_this.status,_this.missingHours,_this.missingHoursOvertime,_this.bonusOrOthers,_this.bonusOrOthersDescription,_this.deductionsOthers,_this.deductionsOthersDescription,_this.reimbursements,_this.reimbursementsDescription]);
}

@override
String toString() {
  final _this = this as DayTimeSheetModel;
  return 'DayTimeSheetModel(id: ${_this.id}, date: ${_this.date}, clientName: ${_this.clientName}, jobTitle: ${_this.jobTitle}, clockIn: ${_this.clockIn}, clockOut: ${_this.clockOut}, durationBreak: ${_this.durationBreak}, breakIsPaid: ${_this.breakIsPaid}, regularHours: ${_this.regularHours}, overtimeHours: ${_this.overtimeHours}, totalHours: ${_this.totalHours}, status: ${_this.status}, missingHours: ${_this.missingHours}, missingHoursOvertime: ${_this.missingHoursOvertime}, bonusOrOthers: ${_this.bonusOrOthers}, bonusOrOthersDescription: ${_this.bonusOrOthersDescription}, deductionsOthers: ${_this.deductionsOthers}, deductionsOthersDescription: ${_this.deductionsOthersDescription}, reimbursements: ${_this.reimbursements}, reimbursementsDescription: ${_this.reimbursementsDescription})';
}


}

/// @nodoc
abstract mixin class $DayTimeSheetModelCopyWith<$Res>  {
  factory $DayTimeSheetModelCopyWith(DayTimeSheetModel value, $Res Function(DayTimeSheetModel) _then) = _$DayTimeSheetModelCopyWithImpl;
@useResult
$Res call({
 String id, DateTime date, String? clientName, String? jobTitle, DateTime? clockIn, DateTime? clockOut, String? durationBreak, bool breakIsPaid, double regularHours, double overtimeHours, double totalHours, int status, String? missingHours, String? missingHoursOvertime, double bonusOrOthers, String? bonusOrOthersDescription, double deductionsOthers, String? deductionsOthersDescription, double reimbursements, String? reimbursementsDescription
});




}
/// @nodoc
class _$DayTimeSheetModelCopyWithImpl<$Res>
    implements $DayTimeSheetModelCopyWith<$Res> {
  _$DayTimeSheetModelCopyWithImpl(this._self, this._then);

  final DayTimeSheetModel _self;
  final $Res Function(DayTimeSheetModel) _then;

/// Create a copy of DayTimeSheetModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? date = null,Object? clientName = freezed,Object? jobTitle = freezed,Object? clockIn = freezed,Object? clockOut = freezed,Object? durationBreak = freezed,Object? breakIsPaid = null,Object? regularHours = null,Object? overtimeHours = null,Object? totalHours = null,Object? status = null,Object? missingHours = freezed,Object? missingHoursOvertime = freezed,Object? bonusOrOthers = null,Object? bonusOrOthersDescription = freezed,Object? deductionsOthers = null,Object? deductionsOthersDescription = freezed,Object? reimbursements = null,Object? reimbursementsDescription = freezed,}) {
  return _then(DayTimeSheetModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,date: null == date ? _self.date : date // ignore: cast_nullable_to_non_nullable
as DateTime,clientName: freezed == clientName ? _self.clientName : clientName // ignore: cast_nullable_to_non_nullable
as String?,jobTitle: freezed == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String?,clockIn: freezed == clockIn ? _self.clockIn : clockIn // ignore: cast_nullable_to_non_nullable
as DateTime?,clockOut: freezed == clockOut ? _self.clockOut : clockOut // ignore: cast_nullable_to_non_nullable
as DateTime?,durationBreak: freezed == durationBreak ? _self.durationBreak : durationBreak // ignore: cast_nullable_to_non_nullable
as String?,breakIsPaid: null == breakIsPaid ? _self.breakIsPaid : breakIsPaid // ignore: cast_nullable_to_non_nullable
as bool,regularHours: null == regularHours ? _self.regularHours : regularHours // ignore: cast_nullable_to_non_nullable
as double,overtimeHours: null == overtimeHours ? _self.overtimeHours : overtimeHours // ignore: cast_nullable_to_non_nullable
as double,totalHours: null == totalHours ? _self.totalHours : totalHours // ignore: cast_nullable_to_non_nullable
as double,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as int,missingHours: freezed == missingHours ? _self.missingHours : missingHours // ignore: cast_nullable_to_non_nullable
as String?,missingHoursOvertime: freezed == missingHoursOvertime ? _self.missingHoursOvertime : missingHoursOvertime // ignore: cast_nullable_to_non_nullable
as String?,bonusOrOthers: null == bonusOrOthers ? _self.bonusOrOthers : bonusOrOthers // ignore: cast_nullable_to_non_nullable
as double,bonusOrOthersDescription: freezed == bonusOrOthersDescription ? _self.bonusOrOthersDescription : bonusOrOthersDescription // ignore: cast_nullable_to_non_nullable
as String?,deductionsOthers: null == deductionsOthers ? _self.deductionsOthers : deductionsOthers // ignore: cast_nullable_to_non_nullable
as double,deductionsOthersDescription: freezed == deductionsOthersDescription ? _self.deductionsOthersDescription : deductionsOthersDescription // ignore: cast_nullable_to_non_nullable
as String?,reimbursements: null == reimbursements ? _self.reimbursements : reimbursements // ignore: cast_nullable_to_non_nullable
as double,reimbursementsDescription: freezed == reimbursementsDescription ? _self.reimbursementsDescription : reimbursementsDescription // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [DayTimeSheetModel].
extension DayTimeSheetModelPatterns on DayTimeSheetModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _DayTimeSheetModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _DayTimeSheetModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _DayTimeSheetModel value)  $default,){
final _that = this;
switch (_that) {
case _DayTimeSheetModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _DayTimeSheetModel value)?  $default,){
final _that = this;
switch (_that) {
case _DayTimeSheetModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  DateTime date,  String? clientName,  String? jobTitle,  DateTime? clockIn,  DateTime? clockOut,  String? durationBreak,  bool breakIsPaid,  double regularHours,  double overtimeHours,  double totalHours,  int status,  String? missingHours,  String? missingHoursOvertime,  double bonusOrOthers,  String? bonusOrOthersDescription,  double deductionsOthers,  String? deductionsOthersDescription,  double reimbursements,  String? reimbursementsDescription)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _DayTimeSheetModel() when $default != null:
return $default(_that.id,_that.date,_that.clientName,_that.jobTitle,_that.clockIn,_that.clockOut,_that.durationBreak,_that.breakIsPaid,_that.regularHours,_that.overtimeHours,_that.totalHours,_that.status,_that.missingHours,_that.missingHoursOvertime,_that.bonusOrOthers,_that.bonusOrOthersDescription,_that.deductionsOthers,_that.deductionsOthersDescription,_that.reimbursements,_that.reimbursementsDescription);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  DateTime date,  String? clientName,  String? jobTitle,  DateTime? clockIn,  DateTime? clockOut,  String? durationBreak,  bool breakIsPaid,  double regularHours,  double overtimeHours,  double totalHours,  int status,  String? missingHours,  String? missingHoursOvertime,  double bonusOrOthers,  String? bonusOrOthersDescription,  double deductionsOthers,  String? deductionsOthersDescription,  double reimbursements,  String? reimbursementsDescription)  $default,) {final _that = this;
switch (_that) {
case _DayTimeSheetModel():
return $default(_that.id,_that.date,_that.clientName,_that.jobTitle,_that.clockIn,_that.clockOut,_that.durationBreak,_that.breakIsPaid,_that.regularHours,_that.overtimeHours,_that.totalHours,_that.status,_that.missingHours,_that.missingHoursOvertime,_that.bonusOrOthers,_that.bonusOrOthersDescription,_that.deductionsOthers,_that.deductionsOthersDescription,_that.reimbursements,_that.reimbursementsDescription);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  DateTime date,  String? clientName,  String? jobTitle,  DateTime? clockIn,  DateTime? clockOut,  String? durationBreak,  bool breakIsPaid,  double regularHours,  double overtimeHours,  double totalHours,  int status,  String? missingHours,  String? missingHoursOvertime,  double bonusOrOthers,  String? bonusOrOthersDescription,  double deductionsOthers,  String? deductionsOthersDescription,  double reimbursements,  String? reimbursementsDescription)?  $default,) {final _that = this;
switch (_that) {
case _DayTimeSheetModel() when $default != null:
return $default(_that.id,_that.date,_that.clientName,_that.jobTitle,_that.clockIn,_that.clockOut,_that.durationBreak,_that.breakIsPaid,_that.regularHours,_that.overtimeHours,_that.totalHours,_that.status,_that.missingHours,_that.missingHoursOvertime,_that.bonusOrOthers,_that.bonusOrOthersDescription,_that.deductionsOthers,_that.deductionsOthersDescription,_that.reimbursements,_that.reimbursementsDescription);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _DayTimeSheetModel extends DayTimeSheetModel {
  const _DayTimeSheetModel({required this.id, required this.date, this.clientName, this.jobTitle, this.clockIn, this.clockOut, this.durationBreak, this.breakIsPaid = false, this.regularHours = 0, this.overtimeHours = 0, this.totalHours = 0, this.status = 1, this.missingHours, this.missingHoursOvertime, this.bonusOrOthers = 0, this.bonusOrOthersDescription, this.deductionsOthers = 0, this.deductionsOthersDescription, this.reimbursements = 0, this.reimbursementsDescription}): super._();
  factory _DayTimeSheetModel.fromJson(Map<String, dynamic> json) => _$DayTimeSheetModelFromJson(json);

@override final  String id;
@override final  DateTime date;
@override final  String? clientName;
@override final  String? jobTitle;
@override final  DateTime? clockIn;
@override final  DateTime? clockOut;
@override final  String? durationBreak;
@override@JsonKey() final  bool breakIsPaid;
@override@JsonKey() final  double regularHours;
@override@JsonKey() final  double overtimeHours;
@override@JsonKey() final  double totalHours;
@override@JsonKey() final  int status;
@override final  String? missingHours;
@override final  String? missingHoursOvertime;
@override@JsonKey() final  double bonusOrOthers;
@override final  String? bonusOrOthersDescription;
@override@JsonKey() final  double deductionsOthers;
@override final  String? deductionsOthersDescription;
@override@JsonKey() final  double reimbursements;
@override final  String? reimbursementsDescription;

/// Create a copy of DayTimeSheetModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$DayTimeSheetModelCopyWith<_DayTimeSheetModel> get copyWith => __$DayTimeSheetModelCopyWithImpl<_DayTimeSheetModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$DayTimeSheetModelToJson(this, );
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _DayTimeSheetModel&&(identical(other.id, id) || other.id == id)&&(identical(other.date, date) || other.date == date)&&(identical(other.clientName, clientName) || other.clientName == clientName)&&(identical(other.jobTitle, jobTitle) || other.jobTitle == jobTitle)&&(identical(other.clockIn, clockIn) || other.clockIn == clockIn)&&(identical(other.clockOut, clockOut) || other.clockOut == clockOut)&&(identical(other.durationBreak, durationBreak) || other.durationBreak == durationBreak)&&(identical(other.breakIsPaid, breakIsPaid) || other.breakIsPaid == breakIsPaid)&&(identical(other.regularHours, regularHours) || other.regularHours == regularHours)&&(identical(other.overtimeHours, overtimeHours) || other.overtimeHours == overtimeHours)&&(identical(other.totalHours, totalHours) || other.totalHours == totalHours)&&(identical(other.status, status) || other.status == status)&&(identical(other.missingHours, missingHours) || other.missingHours == missingHours)&&(identical(other.missingHoursOvertime, missingHoursOvertime) || other.missingHoursOvertime == missingHoursOvertime)&&(identical(other.bonusOrOthers, bonusOrOthers) || other.bonusOrOthers == bonusOrOthers)&&(identical(other.bonusOrOthersDescription, bonusOrOthersDescription) || other.bonusOrOthersDescription == bonusOrOthersDescription)&&(identical(other.deductionsOthers, deductionsOthers) || other.deductionsOthers == deductionsOthers)&&(identical(other.deductionsOthersDescription, deductionsOthersDescription) || other.deductionsOthersDescription == deductionsOthersDescription)&&(identical(other.reimbursements, reimbursements) || other.reimbursements == reimbursements)&&(identical(other.reimbursementsDescription, reimbursementsDescription) || other.reimbursementsDescription == reimbursementsDescription));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
    return Object.hashAll([runtimeType,id,date,clientName,jobTitle,clockIn,clockOut,durationBreak,breakIsPaid,regularHours,overtimeHours,totalHours,status,missingHours,missingHoursOvertime,bonusOrOthers,bonusOrOthersDescription,deductionsOthers,deductionsOthersDescription,reimbursements,reimbursementsDescription]);
}

@override
String toString() {
    return 'DayTimeSheetModel(id: $id, date: $date, clientName: $clientName, jobTitle: $jobTitle, clockIn: $clockIn, clockOut: $clockOut, durationBreak: $durationBreak, breakIsPaid: $breakIsPaid, regularHours: $regularHours, overtimeHours: $overtimeHours, totalHours: $totalHours, status: $status, missingHours: $missingHours, missingHoursOvertime: $missingHoursOvertime, bonusOrOthers: $bonusOrOthers, bonusOrOthersDescription: $bonusOrOthersDescription, deductionsOthers: $deductionsOthers, deductionsOthersDescription: $deductionsOthersDescription, reimbursements: $reimbursements, reimbursementsDescription: $reimbursementsDescription)';
}


}

/// @nodoc
abstract mixin class _$DayTimeSheetModelCopyWith<$Res> implements $DayTimeSheetModelCopyWith<$Res> {
  factory _$DayTimeSheetModelCopyWith(_DayTimeSheetModel value, $Res Function(_DayTimeSheetModel) _then) = __$DayTimeSheetModelCopyWithImpl;
@override @useResult
$Res call({
 String id, DateTime date, String? clientName, String? jobTitle, DateTime? clockIn, DateTime? clockOut, String? durationBreak, bool breakIsPaid, double regularHours, double overtimeHours, double totalHours, int status, String? missingHours, String? missingHoursOvertime, double bonusOrOthers, String? bonusOrOthersDescription, double deductionsOthers, String? deductionsOthersDescription, double reimbursements, String? reimbursementsDescription
});




}
/// @nodoc
class __$DayTimeSheetModelCopyWithImpl<$Res>
    implements _$DayTimeSheetModelCopyWith<$Res> {
  __$DayTimeSheetModelCopyWithImpl(this._self, this._then);

  final _DayTimeSheetModel _self;
  final $Res Function(_DayTimeSheetModel) _then;

/// Create a copy of DayTimeSheetModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? date = null,Object? clientName = freezed,Object? jobTitle = freezed,Object? clockIn = freezed,Object? clockOut = freezed,Object? durationBreak = freezed,Object? breakIsPaid = null,Object? regularHours = null,Object? overtimeHours = null,Object? totalHours = null,Object? status = null,Object? missingHours = freezed,Object? missingHoursOvertime = freezed,Object? bonusOrOthers = null,Object? bonusOrOthersDescription = freezed,Object? deductionsOthers = null,Object? deductionsOthersDescription = freezed,Object? reimbursements = null,Object? reimbursementsDescription = freezed,}) {
  return _then(_DayTimeSheetModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,date: null == date ? _self.date : date // ignore: cast_nullable_to_non_nullable
as DateTime,clientName: freezed == clientName ? _self.clientName : clientName // ignore: cast_nullable_to_non_nullable
as String?,jobTitle: freezed == jobTitle ? _self.jobTitle : jobTitle // ignore: cast_nullable_to_non_nullable
as String?,clockIn: freezed == clockIn ? _self.clockIn : clockIn // ignore: cast_nullable_to_non_nullable
as DateTime?,clockOut: freezed == clockOut ? _self.clockOut : clockOut // ignore: cast_nullable_to_non_nullable
as DateTime?,durationBreak: freezed == durationBreak ? _self.durationBreak : durationBreak // ignore: cast_nullable_to_non_nullable
as String?,breakIsPaid: null == breakIsPaid ? _self.breakIsPaid : breakIsPaid // ignore: cast_nullable_to_non_nullable
as bool,regularHours: null == regularHours ? _self.regularHours : regularHours // ignore: cast_nullable_to_non_nullable
as double,overtimeHours: null == overtimeHours ? _self.overtimeHours : overtimeHours // ignore: cast_nullable_to_non_nullable
as double,totalHours: null == totalHours ? _self.totalHours : totalHours // ignore: cast_nullable_to_non_nullable
as double,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as int,missingHours: freezed == missingHours ? _self.missingHours : missingHours // ignore: cast_nullable_to_non_nullable
as String?,missingHoursOvertime: freezed == missingHoursOvertime ? _self.missingHoursOvertime : missingHoursOvertime // ignore: cast_nullable_to_non_nullable
as String?,bonusOrOthers: null == bonusOrOthers ? _self.bonusOrOthers : bonusOrOthers // ignore: cast_nullable_to_non_nullable
as double,bonusOrOthersDescription: freezed == bonusOrOthersDescription ? _self.bonusOrOthersDescription : bonusOrOthersDescription // ignore: cast_nullable_to_non_nullable
as String?,deductionsOthers: null == deductionsOthers ? _self.deductionsOthers : deductionsOthers // ignore: cast_nullable_to_non_nullable
as double,deductionsOthersDescription: freezed == deductionsOthersDescription ? _self.deductionsOthersDescription : deductionsOthersDescription // ignore: cast_nullable_to_non_nullable
as String?,reimbursements: null == reimbursements ? _self.reimbursements : reimbursements // ignore: cast_nullable_to_non_nullable
as double,reimbursementsDescription: freezed == reimbursementsDescription ? _self.reimbursementsDescription : reimbursementsDescription // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}


/// @nodoc
mixin _$WeekHoursModel {

 double get total; double get regular; double get overtime; double get holiday; double get pendingApproval;
/// Create a copy of WeekHoursModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WeekHoursModelCopyWith<WeekHoursModel> get copyWith => _$WeekHoursModelCopyWithImpl<WeekHoursModel>(this as WeekHoursModel, _$identity);

  /// Serializes this WeekHoursModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  final _this = this as WeekHoursModel;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WeekHoursModel&&(identical(other.total, _this.total) || other.total == _this.total)&&(identical(other.regular, _this.regular) || other.regular == _this.regular)&&(identical(other.overtime, _this.overtime) || other.overtime == _this.overtime)&&(identical(other.holiday, _this.holiday) || other.holiday == _this.holiday)&&(identical(other.pendingApproval, _this.pendingApproval) || other.pendingApproval == _this.pendingApproval));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
  final _this = this as WeekHoursModel;
  return Object.hash(runtimeType,_this.total,_this.regular,_this.overtime,_this.holiday,_this.pendingApproval);
}

@override
String toString() {
  final _this = this as WeekHoursModel;
  return 'WeekHoursModel(total: ${_this.total}, regular: ${_this.regular}, overtime: ${_this.overtime}, holiday: ${_this.holiday}, pendingApproval: ${_this.pendingApproval})';
}


}

/// @nodoc
abstract mixin class $WeekHoursModelCopyWith<$Res>  {
  factory $WeekHoursModelCopyWith(WeekHoursModel value, $Res Function(WeekHoursModel) _then) = _$WeekHoursModelCopyWithImpl;
@useResult
$Res call({
 double total, double regular, double overtime, double holiday, double pendingApproval
});




}
/// @nodoc
class _$WeekHoursModelCopyWithImpl<$Res>
    implements $WeekHoursModelCopyWith<$Res> {
  _$WeekHoursModelCopyWithImpl(this._self, this._then);

  final WeekHoursModel _self;
  final $Res Function(WeekHoursModel) _then;

/// Create a copy of WeekHoursModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? total = null,Object? regular = null,Object? overtime = null,Object? holiday = null,Object? pendingApproval = null,}) {
  return _then(WeekHoursModel(
total: null == total ? _self.total : total // ignore: cast_nullable_to_non_nullable
as double,regular: null == regular ? _self.regular : regular // ignore: cast_nullable_to_non_nullable
as double,overtime: null == overtime ? _self.overtime : overtime // ignore: cast_nullable_to_non_nullable
as double,holiday: null == holiday ? _self.holiday : holiday // ignore: cast_nullable_to_non_nullable
as double,pendingApproval: null == pendingApproval ? _self.pendingApproval : pendingApproval // ignore: cast_nullable_to_non_nullable
as double,
  ));
}

}


/// Adds pattern-matching-related methods to [WeekHoursModel].
extension WeekHoursModelPatterns on WeekHoursModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WeekHoursModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WeekHoursModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WeekHoursModel value)  $default,){
final _that = this;
switch (_that) {
case _WeekHoursModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WeekHoursModel value)?  $default,){
final _that = this;
switch (_that) {
case _WeekHoursModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( double total,  double regular,  double overtime,  double holiday,  double pendingApproval)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WeekHoursModel() when $default != null:
return $default(_that.total,_that.regular,_that.overtime,_that.holiday,_that.pendingApproval);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( double total,  double regular,  double overtime,  double holiday,  double pendingApproval)  $default,) {final _that = this;
switch (_that) {
case _WeekHoursModel():
return $default(_that.total,_that.regular,_that.overtime,_that.holiday,_that.pendingApproval);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( double total,  double regular,  double overtime,  double holiday,  double pendingApproval)?  $default,) {final _that = this;
switch (_that) {
case _WeekHoursModel() when $default != null:
return $default(_that.total,_that.regular,_that.overtime,_that.holiday,_that.pendingApproval);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WeekHoursModel implements WeekHoursModel {
  const _WeekHoursModel({this.total = 0, this.regular = 0, this.overtime = 0, this.holiday = 0, this.pendingApproval = 0});
  factory _WeekHoursModel.fromJson(Map<String, dynamic> json) => _$WeekHoursModelFromJson(json);

@override@JsonKey() final  double total;
@override@JsonKey() final  double regular;
@override@JsonKey() final  double overtime;
@override@JsonKey() final  double holiday;
@override@JsonKey() final  double pendingApproval;

/// Create a copy of WeekHoursModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WeekHoursModelCopyWith<_WeekHoursModel> get copyWith => __$WeekHoursModelCopyWithImpl<_WeekHoursModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WeekHoursModelToJson(this, );
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _WeekHoursModel&&(identical(other.total, total) || other.total == total)&&(identical(other.regular, regular) || other.regular == regular)&&(identical(other.overtime, overtime) || other.overtime == overtime)&&(identical(other.holiday, holiday) || other.holiday == holiday)&&(identical(other.pendingApproval, pendingApproval) || other.pendingApproval == pendingApproval));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
    return Object.hash(runtimeType,total,regular,overtime,holiday,pendingApproval);
}

@override
String toString() {
    return 'WeekHoursModel(total: $total, regular: $regular, overtime: $overtime, holiday: $holiday, pendingApproval: $pendingApproval)';
}


}

/// @nodoc
abstract mixin class _$WeekHoursModelCopyWith<$Res> implements $WeekHoursModelCopyWith<$Res> {
  factory _$WeekHoursModelCopyWith(_WeekHoursModel value, $Res Function(_WeekHoursModel) _then) = __$WeekHoursModelCopyWithImpl;
@override @useResult
$Res call({
 double total, double regular, double overtime, double holiday, double pendingApproval
});




}
/// @nodoc
class __$WeekHoursModelCopyWithImpl<$Res>
    implements _$WeekHoursModelCopyWith<$Res> {
  __$WeekHoursModelCopyWithImpl(this._self, this._then);

  final _WeekHoursModel _self;
  final $Res Function(_WeekHoursModel) _then;

/// Create a copy of WeekHoursModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? total = null,Object? regular = null,Object? overtime = null,Object? holiday = null,Object? pendingApproval = null,}) {
  return _then(_WeekHoursModel(
total: null == total ? _self.total : total // ignore: cast_nullable_to_non_nullable
as double,regular: null == regular ? _self.regular : regular // ignore: cast_nullable_to_non_nullable
as double,overtime: null == overtime ? _self.overtime : overtime // ignore: cast_nullable_to_non_nullable
as double,holiday: null == holiday ? _self.holiday : holiday // ignore: cast_nullable_to_non_nullable
as double,pendingApproval: null == pendingApproval ? _self.pendingApproval : pendingApproval // ignore: cast_nullable_to_non_nullable
as double,
  ));
}


}


/// @nodoc
mixin _$WeekTimeSheetsModel {

 DateTime get weekStart; WeekHoursModel get hours; List<DayTimeSheetModel> get items;
/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WeekTimeSheetsModelCopyWith<WeekTimeSheetsModel> get copyWith => _$WeekTimeSheetsModelCopyWithImpl<WeekTimeSheetsModel>(this as WeekTimeSheetsModel, _$identity);

  /// Serializes this WeekTimeSheetsModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  final _this = this as WeekTimeSheetsModel;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WeekTimeSheetsModel&&(identical(other.weekStart, _this.weekStart) || other.weekStart == _this.weekStart)&&(identical(other.hours, _this.hours) || other.hours == _this.hours)&&const DeepCollectionEquality().equals(other.items, _this.items));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
  final _this = this as WeekTimeSheetsModel;
  return Object.hash(runtimeType,_this.weekStart,_this.hours,const DeepCollectionEquality().hash(_this.items));
}

@override
String toString() {
  final _this = this as WeekTimeSheetsModel;
  return 'WeekTimeSheetsModel(weekStart: ${_this.weekStart}, hours: ${_this.hours}, items: ${_this.items})';
}


}

/// @nodoc
abstract mixin class $WeekTimeSheetsModelCopyWith<$Res>  {
  factory $WeekTimeSheetsModelCopyWith(WeekTimeSheetsModel value, $Res Function(WeekTimeSheetsModel) _then) = _$WeekTimeSheetsModelCopyWithImpl;
@useResult
$Res call({
 DateTime weekStart, WeekHoursModel hours, List<DayTimeSheetModel> items
});


$WeekHoursModelCopyWith<$Res> get hours;

}
/// @nodoc
class _$WeekTimeSheetsModelCopyWithImpl<$Res>
    implements $WeekTimeSheetsModelCopyWith<$Res> {
  _$WeekTimeSheetsModelCopyWithImpl(this._self, this._then);

  final WeekTimeSheetsModel _self;
  final $Res Function(WeekTimeSheetsModel) _then;

/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? weekStart = null,Object? hours = null,Object? items = null,}) {
  return _then(WeekTimeSheetsModel(
weekStart: null == weekStart ? _self.weekStart : weekStart // ignore: cast_nullable_to_non_nullable
as DateTime,hours: null == hours ? _self.hours : hours // ignore: cast_nullable_to_non_nullable
as WeekHoursModel,items: null == items ? _self.items : items // ignore: cast_nullable_to_non_nullable
as List<DayTimeSheetModel>,
  ));
}
/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$WeekHoursModelCopyWith<$Res> get hours {
  
  return $WeekHoursModelCopyWith<$Res>(_self.hours, (value) {
    return _then(_self.copyWith(hours: value));
  });
}
}


/// Adds pattern-matching-related methods to [WeekTimeSheetsModel].
extension WeekTimeSheetsModelPatterns on WeekTimeSheetsModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WeekTimeSheetsModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WeekTimeSheetsModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WeekTimeSheetsModel value)  $default,){
final _that = this;
switch (_that) {
case _WeekTimeSheetsModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WeekTimeSheetsModel value)?  $default,){
final _that = this;
switch (_that) {
case _WeekTimeSheetsModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( DateTime weekStart,  WeekHoursModel hours,  List<DayTimeSheetModel> items)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WeekTimeSheetsModel() when $default != null:
return $default(_that.weekStart,_that.hours,_that.items);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( DateTime weekStart,  WeekHoursModel hours,  List<DayTimeSheetModel> items)  $default,) {final _that = this;
switch (_that) {
case _WeekTimeSheetsModel():
return $default(_that.weekStart,_that.hours,_that.items);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( DateTime weekStart,  WeekHoursModel hours,  List<DayTimeSheetModel> items)?  $default,) {final _that = this;
switch (_that) {
case _WeekTimeSheetsModel() when $default != null:
return $default(_that.weekStart,_that.hours,_that.items);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WeekTimeSheetsModel extends WeekTimeSheetsModel {
  const _WeekTimeSheetsModel({required this.weekStart, this.hours = const WeekHoursModel(),  List<DayTimeSheetModel> items = const []}): _items = items,super._();
  factory _WeekTimeSheetsModel.fromJson(Map<String, dynamic> json) => _$WeekTimeSheetsModelFromJson(json);

@override final  DateTime weekStart;
@override@JsonKey() final  WeekHoursModel hours;
 final  List<DayTimeSheetModel> _items;
@override@JsonKey() List<DayTimeSheetModel> get items {
  if (_items is EqualUnmodifiableListView) return _items;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_items);
}


/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WeekTimeSheetsModelCopyWith<_WeekTimeSheetsModel> get copyWith => __$WeekTimeSheetsModelCopyWithImpl<_WeekTimeSheetsModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WeekTimeSheetsModelToJson(this, );
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _WeekTimeSheetsModel&&(identical(other.weekStart, weekStart) || other.weekStart == weekStart)&&(identical(other.hours, hours) || other.hours == hours)&&const DeepCollectionEquality().equals(other.items, _items));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
    return Object.hash(runtimeType,weekStart,hours,const DeepCollectionEquality().hash(_items));
}

@override
String toString() {
    return 'WeekTimeSheetsModel(weekStart: $weekStart, hours: $hours, items: $items)';
}


}

/// @nodoc
abstract mixin class _$WeekTimeSheetsModelCopyWith<$Res> implements $WeekTimeSheetsModelCopyWith<$Res> {
  factory _$WeekTimeSheetsModelCopyWith(_WeekTimeSheetsModel value, $Res Function(_WeekTimeSheetsModel) _then) = __$WeekTimeSheetsModelCopyWithImpl;
@override @useResult
$Res call({
 DateTime weekStart, WeekHoursModel hours, List<DayTimeSheetModel> items
});


@override $WeekHoursModelCopyWith<$Res> get hours;

}
/// @nodoc
class __$WeekTimeSheetsModelCopyWithImpl<$Res>
    implements _$WeekTimeSheetsModelCopyWith<$Res> {
  __$WeekTimeSheetsModelCopyWithImpl(this._self, this._then);

  final _WeekTimeSheetsModel _self;
  final $Res Function(_WeekTimeSheetsModel) _then;

/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? weekStart = null,Object? hours = null,Object? items = null,}) {
  return _then(_WeekTimeSheetsModel(
weekStart: null == weekStart ? _self.weekStart : weekStart // ignore: cast_nullable_to_non_nullable
as DateTime,hours: null == hours ? _self.hours : hours // ignore: cast_nullable_to_non_nullable
as WeekHoursModel,items: null == items ? _self._items : items // ignore: cast_nullable_to_non_nullable
as List<DayTimeSheetModel>,
  ));
}

/// Create a copy of WeekTimeSheetsModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$WeekHoursModelCopyWith<$Res> get hours {
  
  return $WeekHoursModelCopyWith<$Res>(_self.hours, (value) {
    return _then(_self.copyWith(hours: value));
  });
}
}

// dart format on
