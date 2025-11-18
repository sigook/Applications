// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'professional_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ProfessionalInfoModel {

 Map<String, String> get languages;// {id: value} - only languages with IDs
 List<String> get skills;
/// Create a copy of ProfessionalInfoModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProfessionalInfoModelCopyWith<ProfessionalInfoModel> get copyWith => _$ProfessionalInfoModelCopyWithImpl<ProfessionalInfoModel>(this as ProfessionalInfoModel, _$identity);

  /// Serializes this ProfessionalInfoModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProfessionalInfoModel&&const DeepCollectionEquality().equals(other.languages, languages)&&const DeepCollectionEquality().equals(other.skills, skills));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(languages),const DeepCollectionEquality().hash(skills));

@override
String toString() {
  return 'ProfessionalInfoModel(languages: $languages, skills: $skills)';
}


}

/// @nodoc
abstract mixin class $ProfessionalInfoModelCopyWith<$Res>  {
  factory $ProfessionalInfoModelCopyWith(ProfessionalInfoModel value, $Res Function(ProfessionalInfoModel) _then) = _$ProfessionalInfoModelCopyWithImpl;
@useResult
$Res call({
 Map<String, String> languages, List<String> skills
});




}
/// @nodoc
class _$ProfessionalInfoModelCopyWithImpl<$Res>
    implements $ProfessionalInfoModelCopyWith<$Res> {
  _$ProfessionalInfoModelCopyWithImpl(this._self, this._then);

  final ProfessionalInfoModel _self;
  final $Res Function(ProfessionalInfoModel) _then;

/// Create a copy of ProfessionalInfoModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? languages = null,Object? skills = null,}) {
  return _then(_self.copyWith(
languages: null == languages ? _self.languages : languages // ignore: cast_nullable_to_non_nullable
as Map<String, String>,skills: null == skills ? _self.skills : skills // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}

}


/// Adds pattern-matching-related methods to [ProfessionalInfoModel].
extension ProfessionalInfoModelPatterns on ProfessionalInfoModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProfessionalInfoModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProfessionalInfoModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProfessionalInfoModel value)  $default,){
final _that = this;
switch (_that) {
case _ProfessionalInfoModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProfessionalInfoModel value)?  $default,){
final _that = this;
switch (_that) {
case _ProfessionalInfoModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( Map<String, String> languages,  List<String> skills)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProfessionalInfoModel() when $default != null:
return $default(_that.languages,_that.skills);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( Map<String, String> languages,  List<String> skills)  $default,) {final _that = this;
switch (_that) {
case _ProfessionalInfoModel():
return $default(_that.languages,_that.skills);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( Map<String, String> languages,  List<String> skills)?  $default,) {final _that = this;
switch (_that) {
case _ProfessionalInfoModel() when $default != null:
return $default(_that.languages,_that.skills);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ProfessionalInfoModel extends ProfessionalInfoModel {
  const _ProfessionalInfoModel({required final  Map<String, String> languages, required final  List<String> skills}): _languages = languages,_skills = skills,super._();
  factory _ProfessionalInfoModel.fromJson(Map<String, dynamic> json) => _$ProfessionalInfoModelFromJson(json);

 final  Map<String, String> _languages;
@override Map<String, String> get languages {
  if (_languages is EqualUnmodifiableMapView) return _languages;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(_languages);
}

// {id: value} - only languages with IDs
 final  List<String> _skills;
// {id: value} - only languages with IDs
@override List<String> get skills {
  if (_skills is EqualUnmodifiableListView) return _skills;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_skills);
}


/// Create a copy of ProfessionalInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProfessionalInfoModelCopyWith<_ProfessionalInfoModel> get copyWith => __$ProfessionalInfoModelCopyWithImpl<_ProfessionalInfoModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ProfessionalInfoModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProfessionalInfoModel&&const DeepCollectionEquality().equals(other._languages, _languages)&&const DeepCollectionEquality().equals(other._skills, _skills));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_languages),const DeepCollectionEquality().hash(_skills));

@override
String toString() {
  return 'ProfessionalInfoModel(languages: $languages, skills: $skills)';
}


}

/// @nodoc
abstract mixin class _$ProfessionalInfoModelCopyWith<$Res> implements $ProfessionalInfoModelCopyWith<$Res> {
  factory _$ProfessionalInfoModelCopyWith(_ProfessionalInfoModel value, $Res Function(_ProfessionalInfoModel) _then) = __$ProfessionalInfoModelCopyWithImpl;
@override @useResult
$Res call({
 Map<String, String> languages, List<String> skills
});




}
/// @nodoc
class __$ProfessionalInfoModelCopyWithImpl<$Res>
    implements _$ProfessionalInfoModelCopyWith<$Res> {
  __$ProfessionalInfoModelCopyWithImpl(this._self, this._then);

  final _ProfessionalInfoModel _self;
  final $Res Function(_ProfessionalInfoModel) _then;

/// Create a copy of ProfessionalInfoModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? languages = null,Object? skills = null,}) {
  return _then(_ProfessionalInfoModel(
languages: null == languages ? _self._languages : languages // ignore: cast_nullable_to_non_nullable
as Map<String, String>,skills: null == skills ? _self._skills : skills // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}


}

// dart format on
