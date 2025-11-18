// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'preferences_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$PreferencesInfoModel {

 Map<String, String> get availabilityType;// {id, value}
 List<Map<String, String>> get availableTimes; List<Map<String, String>> get availableDays; Map<String, String>? get liftingCapacity;// {id, value}
 bool get hasVehicle; List<Map<String, String>> get languages; List<String> get skills;
/// Create a copy of PreferencesInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$PreferencesInfoModelCopyWith<PreferencesInfoModel> get copyWith => _$PreferencesInfoModelCopyWithImpl<PreferencesInfoModel>(this as PreferencesInfoModel, _$identity);

  /// Serializes this PreferencesInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is PreferencesInfoModel&&const DeepCollectionEquality().equals(other.availabilityType, availabilityType)&&const DeepCollectionEquality().equals(other.availableTimes, availableTimes)&&const DeepCollectionEquality().equals(other.availableDays, availableDays)&&const DeepCollectionEquality().equals(other.liftingCapacity, liftingCapacity)&&(identical(other.hasVehicle, hasVehicle) || other.hasVehicle == hasVehicle)&&const DeepCollectionEquality().equals(other.languages, languages)&&const DeepCollectionEquality().equals(other.skills, skills));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(availabilityType),const DeepCollectionEquality().hash(availableTimes),const DeepCollectionEquality().hash(availableDays),const DeepCollectionEquality().hash(liftingCapacity),hasVehicle,const DeepCollectionEquality().hash(languages),const DeepCollectionEquality().hash(skills));

@override
String toString() {
  return 'PreferencesInfoModel(availabilityType: $availabilityType, availableTimes: $availableTimes, availableDays: $availableDays, liftingCapacity: $liftingCapacity, hasVehicle: $hasVehicle, languages: $languages, skills: $skills)';
}


}

/// @nodoc
abstract mixin class $PreferencesInfoModelCopyWith<$Res>  {
  factory $PreferencesInfoModelCopyWith(PreferencesInfoModel value, $Res Function(PreferencesInfoModel) _then) = _$PreferencesInfoModelCopyWithImpl;
@useResult
$Res call({
 Map<String, String> availabilityType, List<Map<String, String>> availableTimes, List<Map<String, String>> availableDays, Map<String, String>? liftingCapacity, bool hasVehicle, List<Map<String, String>> languages, List<String> skills
});




}
/// @nodoc
class _$PreferencesInfoModelCopyWithImpl<$Res>
    implements $PreferencesInfoModelCopyWith<$Res> {
  _$PreferencesInfoModelCopyWithImpl(this._self, this._then);

  final PreferencesInfoModel _self;
  final $Res Function(PreferencesInfoModel) _then;

/// Create a copy of PreferencesInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? availabilityType = null,Object? availableTimes = null,Object? availableDays = null,Object? liftingCapacity = freezed,Object? hasVehicle = null,Object? languages = null,Object? skills = null,}) {
  return _then(_self.copyWith(
availabilityType: null == availabilityType ? _self.availabilityType : availabilityType // ignore: cast_nullable_to_non_nullable
as Map<String, String>,availableTimes: null == availableTimes ? _self.availableTimes : availableTimes // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,availableDays: null == availableDays ? _self.availableDays : availableDays // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,liftingCapacity: freezed == liftingCapacity ? _self.liftingCapacity : liftingCapacity // ignore: cast_nullable_to_non_nullable
as Map<String, String>?,hasVehicle: null == hasVehicle ? _self.hasVehicle : hasVehicle // ignore: cast_nullable_to_non_nullable
as bool,languages: null == languages ? _self.languages : languages // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,skills: null == skills ? _self.skills : skills // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}

}


/// Adds pattern-matching-related methods to [PreferencesInfoModel].
extension PreferencesInfoModelPatterns on PreferencesInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _PreferencesInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _PreferencesInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _PreferencesInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _PreferencesInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _PreferencesInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _PreferencesInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( Map<String, String> availabilityType,  List<Map<String, String>> availableTimes,  List<Map<String, String>> availableDays,  Map<String, String>? liftingCapacity,  bool hasVehicle,  List<Map<String, String>> languages,  List<String> skills)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _PreferencesInfoModel() when $default != null:
return $default(_that.availabilityType,_that.availableTimes,_that.availableDays,_that.liftingCapacity,_that.hasVehicle,_that.languages,_that.skills);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( Map<String, String> availabilityType,  List<Map<String, String>> availableTimes,  List<Map<String, String>> availableDays,  Map<String, String>? liftingCapacity,  bool hasVehicle,  List<Map<String, String>> languages,  List<String> skills)  $default,) {final _that = this;
switch (_that) {
case _PreferencesInfoModel():
return $default(_that.availabilityType,_that.availableTimes,_that.availableDays,_that.liftingCapacity,_that.hasVehicle,_that.languages,_that.skills);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( Map<String, String> availabilityType,  List<Map<String, String>> availableTimes,  List<Map<String, String>> availableDays,  Map<String, String>? liftingCapacity,  bool hasVehicle,  List<Map<String, String>> languages,  List<String> skills)?  $default,) {final _that = this;
switch (_that) {
case _PreferencesInfoModel() when $default != null:
return $default(_that.availabilityType,_that.availableTimes,_that.availableDays,_that.liftingCapacity,_that.hasVehicle,_that.languages,_that.skills);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _PreferencesInfoModel extends PreferencesInfoModel {
  const _PreferencesInfoModel({required final  Map<String, String> availabilityType, required final  List<Map<String, String>> availableTimes, required final  List<Map<String, String>> availableDays, required final  Map<String, String>? liftingCapacity, required this.hasVehicle, required final  List<Map<String, String>> languages, required final  List<String> skills}): _availabilityType = availabilityType,_availableTimes = availableTimes,_availableDays = availableDays,_liftingCapacity = liftingCapacity,_languages = languages,_skills = skills,super._();
  factory _PreferencesInfoModel.fromJson(Map<String, dynamic> json) => _$PreferencesInfoModelFromJson(json);

 final  Map<String, String> _availabilityType;
@override Map<String, String> get availabilityType {
  if (_availabilityType is EqualUnmodifiableMapView) return _availabilityType;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(_availabilityType);
}

// {id, value}
 final  List<Map<String, String>> _availableTimes;
// {id, value}
@override List<Map<String, String>> get availableTimes {
  if (_availableTimes is EqualUnmodifiableListView) return _availableTimes;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availableTimes);
}

 final  List<Map<String, String>> _availableDays;
@override List<Map<String, String>> get availableDays {
  if (_availableDays is EqualUnmodifiableListView) return _availableDays;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availableDays);
}

 final  Map<String, String>? _liftingCapacity;
@override Map<String, String>? get liftingCapacity {
  final value = _liftingCapacity;
  if (value == null) return null;
  if (_liftingCapacity is EqualUnmodifiableMapView) return _liftingCapacity;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(value);
}

// {id, value}
@override final  bool hasVehicle;
 final  List<Map<String, String>> _languages;
@override List<Map<String, String>> get languages {
  if (_languages is EqualUnmodifiableListView) return _languages;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_languages);
}

 final  List<String> _skills;
@override List<String> get skills {
  if (_skills is EqualUnmodifiableListView) return _skills;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_skills);
}


/// Create a copy of PreferencesInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$PreferencesInfoModelCopyWith<_PreferencesInfoModel> get copyWith => __$PreferencesInfoModelCopyWithImpl<_PreferencesInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$PreferencesInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _PreferencesInfoModel&&const DeepCollectionEquality().equals(other._availabilityType, _availabilityType)&&const DeepCollectionEquality().equals(other._availableTimes, _availableTimes)&&const DeepCollectionEquality().equals(other._availableDays, _availableDays)&&const DeepCollectionEquality().equals(other._liftingCapacity, _liftingCapacity)&&(identical(other.hasVehicle, hasVehicle) || other.hasVehicle == hasVehicle)&&const DeepCollectionEquality().equals(other._languages, _languages)&&const DeepCollectionEquality().equals(other._skills, _skills));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_availabilityType),const DeepCollectionEquality().hash(_availableTimes),const DeepCollectionEquality().hash(_availableDays),const DeepCollectionEquality().hash(_liftingCapacity),hasVehicle,const DeepCollectionEquality().hash(_languages),const DeepCollectionEquality().hash(_skills));

@override
String toString() {
  return 'PreferencesInfoModel(availabilityType: $availabilityType, availableTimes: $availableTimes, availableDays: $availableDays, liftingCapacity: $liftingCapacity, hasVehicle: $hasVehicle, languages: $languages, skills: $skills)';
}


}

/// @nodoc
abstract mixin class _$PreferencesInfoModelCopyWith<$Res> implements $PreferencesInfoModelCopyWith<$Res> {
  factory _$PreferencesInfoModelCopyWith(_PreferencesInfoModel value, $Res Function(_PreferencesInfoModel) _then) = __$PreferencesInfoModelCopyWithImpl;
@override @useResult
$Res call({
 Map<String, String> availabilityType, List<Map<String, String>> availableTimes, List<Map<String, String>> availableDays, Map<String, String>? liftingCapacity, bool hasVehicle, List<Map<String, String>> languages, List<String> skills
});




}
/// @nodoc
class __$PreferencesInfoModelCopyWithImpl<$Res>
    implements _$PreferencesInfoModelCopyWith<$Res> {
  __$PreferencesInfoModelCopyWithImpl(this._self, this._then);

  final _PreferencesInfoModel _self;
  final $Res Function(_PreferencesInfoModel) _then;

/// Create a copy of PreferencesInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? availabilityType = null,Object? availableTimes = null,Object? availableDays = null,Object? liftingCapacity = freezed,Object? hasVehicle = null,Object? languages = null,Object? skills = null,}) {
  return _then(_PreferencesInfoModel(
availabilityType: null == availabilityType ? _self._availabilityType : availabilityType // ignore: cast_nullable_to_non_nullable
as Map<String, String>,availableTimes: null == availableTimes ? _self._availableTimes : availableTimes // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,availableDays: null == availableDays ? _self._availableDays : availableDays // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,liftingCapacity: freezed == liftingCapacity ? _self._liftingCapacity : liftingCapacity // ignore: cast_nullable_to_non_nullable
as Map<String, String>?,hasVehicle: null == hasVehicle ? _self.hasVehicle : hasVehicle // ignore: cast_nullable_to_non_nullable
as bool,languages: null == languages ? _self._languages : languages // ignore: cast_nullable_to_non_nullable
as List<Map<String, String>>,skills: null == skills ? _self._skills : skills // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}


}

// dart format on
