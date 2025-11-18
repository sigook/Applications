// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'availability_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AvailabilityInfoModel {

 String? get availabilityTypeId;// UUID from API (may be null)
 String get availabilityTypeName;// Name from API
 Map<String, String> get availableTimes;// {id: value} - only times with IDs
 List<String> get availableDays;
/// Create a copy of AvailabilityInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AvailabilityInfoModelCopyWith<AvailabilityInfoModel> get copyWith => _$AvailabilityInfoModelCopyWithImpl<AvailabilityInfoModel>(this as AvailabilityInfoModel, _$identity);

  /// Serializes this AvailabilityInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AvailabilityInfoModel&&(identical(other.availabilityTypeId, availabilityTypeId) || other.availabilityTypeId == availabilityTypeId)&&(identical(other.availabilityTypeName, availabilityTypeName) || other.availabilityTypeName == availabilityTypeName)&&const DeepCollectionEquality().equals(other.availableTimes, availableTimes)&&const DeepCollectionEquality().equals(other.availableDays, availableDays));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,availabilityTypeId,availabilityTypeName,const DeepCollectionEquality().hash(availableTimes),const DeepCollectionEquality().hash(availableDays));

@override
String toString() {
  return 'AvailabilityInfoModel(availabilityTypeId: $availabilityTypeId, availabilityTypeName: $availabilityTypeName, availableTimes: $availableTimes, availableDays: $availableDays)';
}


}

/// @nodoc
abstract mixin class $AvailabilityInfoModelCopyWith<$Res>  {
  factory $AvailabilityInfoModelCopyWith(AvailabilityInfoModel value, $Res Function(AvailabilityInfoModel) _then) = _$AvailabilityInfoModelCopyWithImpl;
@useResult
$Res call({
 String? availabilityTypeId, String availabilityTypeName, Map<String, String> availableTimes, List<String> availableDays
});




}
/// @nodoc
class _$AvailabilityInfoModelCopyWithImpl<$Res>
    implements $AvailabilityInfoModelCopyWith<$Res> {
  _$AvailabilityInfoModelCopyWithImpl(this._self, this._then);

  final AvailabilityInfoModel _self;
  final $Res Function(AvailabilityInfoModel) _then;

/// Create a copy of AvailabilityInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? availabilityTypeId = freezed,Object? availabilityTypeName = null,Object? availableTimes = null,Object? availableDays = null,}) {
  return _then(_self.copyWith(
availabilityTypeId: freezed == availabilityTypeId ? _self.availabilityTypeId : availabilityTypeId // ignore: cast_nullable_to_non_nullable
as String?,availabilityTypeName: null == availabilityTypeName ? _self.availabilityTypeName : availabilityTypeName // ignore: cast_nullable_to_non_nullable
as String,availableTimes: null == availableTimes ? _self.availableTimes : availableTimes // ignore: cast_nullable_to_non_nullable
as Map<String, String>,availableDays: null == availableDays ? _self.availableDays : availableDays // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}

}


/// Adds pattern-matching-related methods to [AvailabilityInfoModel].
extension AvailabilityInfoModelPatterns on AvailabilityInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AvailabilityInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AvailabilityInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AvailabilityInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _AvailabilityInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AvailabilityInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _AvailabilityInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? availabilityTypeId,  String availabilityTypeName,  Map<String, String> availableTimes,  List<String> availableDays)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AvailabilityInfoModel() when $default != null:
return $default(_that.availabilityTypeId,_that.availabilityTypeName,_that.availableTimes,_that.availableDays);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? availabilityTypeId,  String availabilityTypeName,  Map<String, String> availableTimes,  List<String> availableDays)  $default,) {final _that = this;
switch (_that) {
case _AvailabilityInfoModel():
return $default(_that.availabilityTypeId,_that.availabilityTypeName,_that.availableTimes,_that.availableDays);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? availabilityTypeId,  String availabilityTypeName,  Map<String, String> availableTimes,  List<String> availableDays)?  $default,) {final _that = this;
switch (_that) {
case _AvailabilityInfoModel() when $default != null:
return $default(_that.availabilityTypeId,_that.availabilityTypeName,_that.availableTimes,_that.availableDays);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AvailabilityInfoModel extends AvailabilityInfoModel {
  const _AvailabilityInfoModel({this.availabilityTypeId, required this.availabilityTypeName, required final  Map<String, String> availableTimes, required final  List<String> availableDays}): _availableTimes = availableTimes,_availableDays = availableDays,super._();
  factory _AvailabilityInfoModel.fromJson(Map<String, dynamic> json) => _$AvailabilityInfoModelFromJson(json);

@override final  String? availabilityTypeId;
// UUID from API (may be null)
@override final  String availabilityTypeName;
// Name from API
 final  Map<String, String> _availableTimes;
// Name from API
@override Map<String, String> get availableTimes {
  if (_availableTimes is EqualUnmodifiableMapView) return _availableTimes;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(_availableTimes);
}

// {id: value} - only times with IDs
 final  List<String> _availableDays;
// {id: value} - only times with IDs
@override List<String> get availableDays {
  if (_availableDays is EqualUnmodifiableListView) return _availableDays;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availableDays);
}


/// Create a copy of AvailabilityInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AvailabilityInfoModelCopyWith<_AvailabilityInfoModel> get copyWith => __$AvailabilityInfoModelCopyWithImpl<_AvailabilityInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AvailabilityInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AvailabilityInfoModel&&(identical(other.availabilityTypeId, availabilityTypeId) || other.availabilityTypeId == availabilityTypeId)&&(identical(other.availabilityTypeName, availabilityTypeName) || other.availabilityTypeName == availabilityTypeName)&&const DeepCollectionEquality().equals(other._availableTimes, _availableTimes)&&const DeepCollectionEquality().equals(other._availableDays, _availableDays));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,availabilityTypeId,availabilityTypeName,const DeepCollectionEquality().hash(_availableTimes),const DeepCollectionEquality().hash(_availableDays));

@override
String toString() {
  return 'AvailabilityInfoModel(availabilityTypeId: $availabilityTypeId, availabilityTypeName: $availabilityTypeName, availableTimes: $availableTimes, availableDays: $availableDays)';
}


}

/// @nodoc
abstract mixin class _$AvailabilityInfoModelCopyWith<$Res> implements $AvailabilityInfoModelCopyWith<$Res> {
  factory _$AvailabilityInfoModelCopyWith(_AvailabilityInfoModel value, $Res Function(_AvailabilityInfoModel) _then) = __$AvailabilityInfoModelCopyWithImpl;
@override @useResult
$Res call({
 String? availabilityTypeId, String availabilityTypeName, Map<String, String> availableTimes, List<String> availableDays
});




}
/// @nodoc
class __$AvailabilityInfoModelCopyWithImpl<$Res>
    implements _$AvailabilityInfoModelCopyWith<$Res> {
  __$AvailabilityInfoModelCopyWithImpl(this._self, this._then);

  final _AvailabilityInfoModel _self;
  final $Res Function(_AvailabilityInfoModel) _then;

/// Create a copy of AvailabilityInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? availabilityTypeId = freezed,Object? availabilityTypeName = null,Object? availableTimes = null,Object? availableDays = null,}) {
  return _then(_AvailabilityInfoModel(
availabilityTypeId: freezed == availabilityTypeId ? _self.availabilityTypeId : availabilityTypeId // ignore: cast_nullable_to_non_nullable
as String?,availabilityTypeName: null == availabilityTypeName ? _self.availabilityTypeName : availabilityTypeName // ignore: cast_nullable_to_non_nullable
as String,availableTimes: null == availableTimes ? _self._availableTimes : availableTimes // ignore: cast_nullable_to_non_nullable
as Map<String, String>,availableDays: null == availableDays ? _self._availableDays : availableDays // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}


}

// dart format on
