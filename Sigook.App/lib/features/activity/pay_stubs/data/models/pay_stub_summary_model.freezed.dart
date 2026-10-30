// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint, type=warning, deprecated_member_use, deprecated_member_use_from_same_package
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'pay_stub_summary_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$PayStubSummaryModel {

 String get id; String? get payStubNumber; DateTime get weekEnding; DateTime get paymentDate; double get hours; double get totalEarnings; double get totalDeductions; double get totalPaid; List<String> get companies;
/// Create a copy of PayStubSummaryModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PayStubSummaryModelCopyWith<PayStubSummaryModel> get copyWith => _$PayStubSummaryModelCopyWithImpl<PayStubSummaryModel>(this as PayStubSummaryModel, _$identity);

  /// Serializes this PayStubSummaryModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  final _this = this as PayStubSummaryModel;
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PayStubSummaryModel&&(identical(other.id, _this.id) || other.id == _this.id)&&(identical(other.payStubNumber, _this.payStubNumber) || other.payStubNumber == _this.payStubNumber)&&(identical(other.weekEnding, _this.weekEnding) || other.weekEnding == _this.weekEnding)&&(identical(other.paymentDate, _this.paymentDate) || other.paymentDate == _this.paymentDate)&&(identical(other.hours, _this.hours) || other.hours == _this.hours)&&(identical(other.totalEarnings, _this.totalEarnings) || other.totalEarnings == _this.totalEarnings)&&(identical(other.totalDeductions, _this.totalDeductions) || other.totalDeductions == _this.totalDeductions)&&(identical(other.totalPaid, _this.totalPaid) || other.totalPaid == _this.totalPaid)&&const DeepCollectionEquality().equals(other.companies, _this.companies));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
  final _this = this as PayStubSummaryModel;
  return Object.hash(runtimeType,_this.id,_this.payStubNumber,_this.weekEnding,_this.paymentDate,_this.hours,_this.totalEarnings,_this.totalDeductions,_this.totalPaid,const DeepCollectionEquality().hash(_this.companies));
}

@override
String toString() {
  final _this = this as PayStubSummaryModel;
  return 'PayStubSummaryModel(id: ${_this.id}, payStubNumber: ${_this.payStubNumber}, weekEnding: ${_this.weekEnding}, paymentDate: ${_this.paymentDate}, hours: ${_this.hours}, totalEarnings: ${_this.totalEarnings}, totalDeductions: ${_this.totalDeductions}, totalPaid: ${_this.totalPaid}, companies: ${_this.companies})';
}


}

/// @nodoc
abstract mixin class $PayStubSummaryModelCopyWith<$Res>  {
  factory $PayStubSummaryModelCopyWith(PayStubSummaryModel value, $Res Function(PayStubSummaryModel) _then) = _$PayStubSummaryModelCopyWithImpl;
@useResult
$Res call({
 String id, String? payStubNumber, DateTime weekEnding, DateTime paymentDate, double hours, double totalEarnings, double totalDeductions, double totalPaid, List<String> companies
});




}
/// @nodoc
class _$PayStubSummaryModelCopyWithImpl<$Res>
    implements $PayStubSummaryModelCopyWith<$Res> {
  _$PayStubSummaryModelCopyWithImpl(this._self, this._then);

  final PayStubSummaryModel _self;
  final $Res Function(PayStubSummaryModel) _then;

/// Create a copy of PayStubSummaryModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? payStubNumber = freezed,Object? weekEnding = null,Object? paymentDate = null,Object? hours = null,Object? totalEarnings = null,Object? totalDeductions = null,Object? totalPaid = null,Object? companies = null,}) {
  return _then(PayStubSummaryModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,payStubNumber: freezed == payStubNumber ? _self.payStubNumber : payStubNumber // ignore: cast_nullable_to_non_nullable
as String?,weekEnding: null == weekEnding ? _self.weekEnding : weekEnding // ignore: cast_nullable_to_non_nullable
as DateTime,paymentDate: null == paymentDate ? _self.paymentDate : paymentDate // ignore: cast_nullable_to_non_nullable
as DateTime,hours: null == hours ? _self.hours : hours // ignore: cast_nullable_to_non_nullable
as double,totalEarnings: null == totalEarnings ? _self.totalEarnings : totalEarnings // ignore: cast_nullable_to_non_nullable
as double,totalDeductions: null == totalDeductions ? _self.totalDeductions : totalDeductions // ignore: cast_nullable_to_non_nullable
as double,totalPaid: null == totalPaid ? _self.totalPaid : totalPaid // ignore: cast_nullable_to_non_nullable
as double,companies: null == companies ? _self.companies : companies // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}

}


/// Adds pattern-matching-related methods to [PayStubSummaryModel].
extension PayStubSummaryModelPatterns on PayStubSummaryModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PayStubSummaryModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PayStubSummaryModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PayStubSummaryModel value)  $default,){
final _that = this;
switch (_that) {
case _PayStubSummaryModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PayStubSummaryModel value)?  $default,){
final _that = this;
switch (_that) {
case _PayStubSummaryModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String? payStubNumber,  DateTime weekEnding,  DateTime paymentDate,  double hours,  double totalEarnings,  double totalDeductions,  double totalPaid,  List<String> companies)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PayStubSummaryModel() when $default != null:
return $default(_that.id,_that.payStubNumber,_that.weekEnding,_that.paymentDate,_that.hours,_that.totalEarnings,_that.totalDeductions,_that.totalPaid,_that.companies);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String? payStubNumber,  DateTime weekEnding,  DateTime paymentDate,  double hours,  double totalEarnings,  double totalDeductions,  double totalPaid,  List<String> companies)  $default,) {final _that = this;
switch (_that) {
case _PayStubSummaryModel():
return $default(_that.id,_that.payStubNumber,_that.weekEnding,_that.paymentDate,_that.hours,_that.totalEarnings,_that.totalDeductions,_that.totalPaid,_that.companies);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String? payStubNumber,  DateTime weekEnding,  DateTime paymentDate,  double hours,  double totalEarnings,  double totalDeductions,  double totalPaid,  List<String> companies)?  $default,) {final _that = this;
switch (_that) {
case _PayStubSummaryModel() when $default != null:
return $default(_that.id,_that.payStubNumber,_that.weekEnding,_that.paymentDate,_that.hours,_that.totalEarnings,_that.totalDeductions,_that.totalPaid,_that.companies);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _PayStubSummaryModel extends PayStubSummaryModel {
  const _PayStubSummaryModel({required this.id, this.payStubNumber, required this.weekEnding, required this.paymentDate, this.hours = 0, this.totalEarnings = 0, this.totalDeductions = 0, this.totalPaid = 0,  List<String> companies = const []}): _companies = companies,super._();
  factory _PayStubSummaryModel.fromJson(Map<String, dynamic> json) => _$PayStubSummaryModelFromJson(json);

@override final  String id;
@override final  String? payStubNumber;
@override final  DateTime weekEnding;
@override final  DateTime paymentDate;
@override@JsonKey() final  double hours;
@override@JsonKey() final  double totalEarnings;
@override@JsonKey() final  double totalDeductions;
@override@JsonKey() final  double totalPaid;
 final  List<String> _companies;
@override@JsonKey() List<String> get companies {
  if (_companies is EqualUnmodifiableListView) return _companies;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_companies);
}


/// Create a copy of PayStubSummaryModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PayStubSummaryModelCopyWith<_PayStubSummaryModel> get copyWith => __$PayStubSummaryModelCopyWithImpl<_PayStubSummaryModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$PayStubSummaryModelToJson(this, );
}

@override
bool operator ==(Object other) {
    return identical(this, other) || (other.runtimeType == runtimeType&&other is _PayStubSummaryModel&&(identical(other.id, id) || other.id == id)&&(identical(other.payStubNumber, payStubNumber) || other.payStubNumber == payStubNumber)&&(identical(other.weekEnding, weekEnding) || other.weekEnding == weekEnding)&&(identical(other.paymentDate, paymentDate) || other.paymentDate == paymentDate)&&(identical(other.hours, hours) || other.hours == hours)&&(identical(other.totalEarnings, totalEarnings) || other.totalEarnings == totalEarnings)&&(identical(other.totalDeductions, totalDeductions) || other.totalDeductions == totalDeductions)&&(identical(other.totalPaid, totalPaid) || other.totalPaid == totalPaid)&&const DeepCollectionEquality().equals(other.companies, _companies));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode {
    return Object.hash(runtimeType,id,payStubNumber,weekEnding,paymentDate,hours,totalEarnings,totalDeductions,totalPaid,const DeepCollectionEquality().hash(_companies));
}

@override
String toString() {
    return 'PayStubSummaryModel(id: $id, payStubNumber: $payStubNumber, weekEnding: $weekEnding, paymentDate: $paymentDate, hours: $hours, totalEarnings: $totalEarnings, totalDeductions: $totalDeductions, totalPaid: $totalPaid, companies: $companies)';
}


}

/// @nodoc
abstract mixin class _$PayStubSummaryModelCopyWith<$Res> implements $PayStubSummaryModelCopyWith<$Res> {
  factory _$PayStubSummaryModelCopyWith(_PayStubSummaryModel value, $Res Function(_PayStubSummaryModel) _then) = __$PayStubSummaryModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String? payStubNumber, DateTime weekEnding, DateTime paymentDate, double hours, double totalEarnings, double totalDeductions, double totalPaid, List<String> companies
});




}
/// @nodoc
class __$PayStubSummaryModelCopyWithImpl<$Res>
    implements _$PayStubSummaryModelCopyWith<$Res> {
  __$PayStubSummaryModelCopyWithImpl(this._self, this._then);

  final _PayStubSummaryModel _self;
  final $Res Function(_PayStubSummaryModel) _then;

/// Create a copy of PayStubSummaryModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? payStubNumber = freezed,Object? weekEnding = null,Object? paymentDate = null,Object? hours = null,Object? totalEarnings = null,Object? totalDeductions = null,Object? totalPaid = null,Object? companies = null,}) {
  return _then(_PayStubSummaryModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,payStubNumber: freezed == payStubNumber ? _self.payStubNumber : payStubNumber // ignore: cast_nullable_to_non_nullable
as String?,weekEnding: null == weekEnding ? _self.weekEnding : weekEnding // ignore: cast_nullable_to_non_nullable
as DateTime,paymentDate: null == paymentDate ? _self.paymentDate : paymentDate // ignore: cast_nullable_to_non_nullable
as DateTime,hours: null == hours ? _self.hours : hours // ignore: cast_nullable_to_non_nullable
as double,totalEarnings: null == totalEarnings ? _self.totalEarnings : totalEarnings // ignore: cast_nullable_to_non_nullable
as double,totalDeductions: null == totalDeductions ? _self.totalDeductions : totalDeductions // ignore: cast_nullable_to_non_nullable
as double,totalPaid: null == totalPaid ? _self.totalPaid : totalPaid // ignore: cast_nullable_to_non_nullable
as double,companies: null == companies ? _self._companies : companies // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}


}

// dart format on
