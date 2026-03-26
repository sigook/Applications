// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'worker_profile_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ProfileImageModel {

 String? get id; String? get pathFile; String? get fileName; String? get description; bool get canDownload;
/// Create a copy of ProfileImageModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<ProfileImageModel> get copyWith => _$ProfileImageModelCopyWithImpl<ProfileImageModel>(this as ProfileImageModel, _$identity);

  /// Serializes this ProfileImageModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProfileImageModel&&(identical(other.id, id) || other.id == id)&&(identical(other.pathFile, pathFile) || other.pathFile == pathFile)&&(identical(other.fileName, fileName) || other.fileName == fileName)&&(identical(other.description, description) || other.description == description)&&(identical(other.canDownload, canDownload) || other.canDownload == canDownload));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,pathFile,fileName,description,canDownload);

@override
String toString() {
  return 'ProfileImageModel(id: $id, pathFile: $pathFile, fileName: $fileName, description: $description, canDownload: $canDownload)';
}


}

/// @nodoc
abstract mixin class $ProfileImageModelCopyWith<$Res>  {
  factory $ProfileImageModelCopyWith(ProfileImageModel value, $Res Function(ProfileImageModel) _then) = _$ProfileImageModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? pathFile, String? fileName, String? description, bool canDownload
});




}
/// @nodoc
class _$ProfileImageModelCopyWithImpl<$Res>
    implements $ProfileImageModelCopyWith<$Res> {
  _$ProfileImageModelCopyWithImpl(this._self, this._then);

  final ProfileImageModel _self;
  final $Res Function(ProfileImageModel) _then;

/// Create a copy of ProfileImageModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? pathFile = freezed,Object? fileName = freezed,Object? description = freezed,Object? canDownload = null,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,pathFile: freezed == pathFile ? _self.pathFile : pathFile // ignore: cast_nullable_to_non_nullable
as String?,fileName: freezed == fileName ? _self.fileName : fileName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,canDownload: null == canDownload ? _self.canDownload : canDownload // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [ProfileImageModel].
extension ProfileImageModelPatterns on ProfileImageModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProfileImageModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProfileImageModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProfileImageModel value)  $default,){
final _that = this;
switch (_that) {
case _ProfileImageModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProfileImageModel value)?  $default,){
final _that = this;
switch (_that) {
case _ProfileImageModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? pathFile,  String? fileName,  String? description,  bool canDownload)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProfileImageModel() when $default != null:
return $default(_that.id,_that.pathFile,_that.fileName,_that.description,_that.canDownload);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? pathFile,  String? fileName,  String? description,  bool canDownload)  $default,) {final _that = this;
switch (_that) {
case _ProfileImageModel():
return $default(_that.id,_that.pathFile,_that.fileName,_that.description,_that.canDownload);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? pathFile,  String? fileName,  String? description,  bool canDownload)?  $default,) {final _that = this;
switch (_that) {
case _ProfileImageModel() when $default != null:
return $default(_that.id,_that.pathFile,_that.fileName,_that.description,_that.canDownload);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ProfileImageModel implements ProfileImageModel {
  const _ProfileImageModel({this.id, this.pathFile, this.fileName, this.description, this.canDownload = false});
  factory _ProfileImageModel.fromJson(Map<String, dynamic> json) => _$ProfileImageModelFromJson(json);

@override final  String? id;
@override final  String? pathFile;
@override final  String? fileName;
@override final  String? description;
@override@JsonKey() final  bool canDownload;

/// Create a copy of ProfileImageModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProfileImageModelCopyWith<_ProfileImageModel> get copyWith => __$ProfileImageModelCopyWithImpl<_ProfileImageModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ProfileImageModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProfileImageModel&&(identical(other.id, id) || other.id == id)&&(identical(other.pathFile, pathFile) || other.pathFile == pathFile)&&(identical(other.fileName, fileName) || other.fileName == fileName)&&(identical(other.description, description) || other.description == description)&&(identical(other.canDownload, canDownload) || other.canDownload == canDownload));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,pathFile,fileName,description,canDownload);

@override
String toString() {
  return 'ProfileImageModel(id: $id, pathFile: $pathFile, fileName: $fileName, description: $description, canDownload: $canDownload)';
}


}

/// @nodoc
abstract mixin class _$ProfileImageModelCopyWith<$Res> implements $ProfileImageModelCopyWith<$Res> {
  factory _$ProfileImageModelCopyWith(_ProfileImageModel value, $Res Function(_ProfileImageModel) _then) = __$ProfileImageModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? pathFile, String? fileName, String? description, bool canDownload
});




}
/// @nodoc
class __$ProfileImageModelCopyWithImpl<$Res>
    implements _$ProfileImageModelCopyWith<$Res> {
  __$ProfileImageModelCopyWithImpl(this._self, this._then);

  final _ProfileImageModel _self;
  final $Res Function(_ProfileImageModel) _then;

/// Create a copy of ProfileImageModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? pathFile = freezed,Object? fileName = freezed,Object? description = freezed,Object? canDownload = null,}) {
  return _then(_ProfileImageModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,pathFile: freezed == pathFile ? _self.pathFile : pathFile // ignore: cast_nullable_to_non_nullable
as String?,fileName: freezed == fileName ? _self.fileName : fileName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,canDownload: null == canDownload ? _self.canDownload : canDownload // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}


/// @nodoc
mixin _$CatalogItemModel {

 String? get id; String? get value;
/// Create a copy of CatalogItemModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<CatalogItemModel> get copyWith => _$CatalogItemModelCopyWithImpl<CatalogItemModel>(this as CatalogItemModel, _$identity);

  /// Serializes this CatalogItemModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CatalogItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value);

@override
String toString() {
  return 'CatalogItemModel(id: $id, value: $value)';
}


}

/// @nodoc
abstract mixin class $CatalogItemModelCopyWith<$Res>  {
  factory $CatalogItemModelCopyWith(CatalogItemModel value, $Res Function(CatalogItemModel) _then) = _$CatalogItemModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? value
});




}
/// @nodoc
class _$CatalogItemModelCopyWithImpl<$Res>
    implements $CatalogItemModelCopyWith<$Res> {
  _$CatalogItemModelCopyWithImpl(this._self, this._then);

  final CatalogItemModel _self;
  final $Res Function(CatalogItemModel) _then;

/// Create a copy of CatalogItemModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? value = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [CatalogItemModel].
extension CatalogItemModelPatterns on CatalogItemModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CatalogItemModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CatalogItemModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CatalogItemModel value)  $default,){
final _that = this;
switch (_that) {
case _CatalogItemModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CatalogItemModel value)?  $default,){
final _that = this;
switch (_that) {
case _CatalogItemModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? value)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CatalogItemModel() when $default != null:
return $default(_that.id,_that.value);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? value)  $default,) {final _that = this;
switch (_that) {
case _CatalogItemModel():
return $default(_that.id,_that.value);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? value)?  $default,) {final _that = this;
switch (_that) {
case _CatalogItemModel() when $default != null:
return $default(_that.id,_that.value);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CatalogItemModel implements CatalogItemModel {
  const _CatalogItemModel({this.id, this.value});
  factory _CatalogItemModel.fromJson(Map<String, dynamic> json) => _$CatalogItemModelFromJson(json);

@override final  String? id;
@override final  String? value;

/// Create a copy of CatalogItemModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CatalogItemModelCopyWith<_CatalogItemModel> get copyWith => __$CatalogItemModelCopyWithImpl<_CatalogItemModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CatalogItemModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CatalogItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value);

@override
String toString() {
  return 'CatalogItemModel(id: $id, value: $value)';
}


}

/// @nodoc
abstract mixin class _$CatalogItemModelCopyWith<$Res> implements $CatalogItemModelCopyWith<$Res> {
  factory _$CatalogItemModelCopyWith(_CatalogItemModel value, $Res Function(_CatalogItemModel) _then) = __$CatalogItemModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? value
});




}
/// @nodoc
class __$CatalogItemModelCopyWithImpl<$Res>
    implements _$CatalogItemModelCopyWith<$Res> {
  __$CatalogItemModelCopyWithImpl(this._self, this._then);

  final _CatalogItemModel _self;
  final $Res Function(_CatalogItemModel) _then;

/// Create a copy of CatalogItemModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? value = freezed,}) {
  return _then(_CatalogItemModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}


/// @nodoc
mixin _$SkillItemModel {

 String? get id; String? get skill;
/// Create a copy of SkillItemModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$SkillItemModelCopyWith<SkillItemModel> get copyWith => _$SkillItemModelCopyWithImpl<SkillItemModel>(this as SkillItemModel, _$identity);

  /// Serializes this SkillItemModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is SkillItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.skill, skill) || other.skill == skill));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,skill);

@override
String toString() {
  return 'SkillItemModel(id: $id, skill: $skill)';
}


}

/// @nodoc
abstract mixin class $SkillItemModelCopyWith<$Res>  {
  factory $SkillItemModelCopyWith(SkillItemModel value, $Res Function(SkillItemModel) _then) = _$SkillItemModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? skill
});




}
/// @nodoc
class _$SkillItemModelCopyWithImpl<$Res>
    implements $SkillItemModelCopyWith<$Res> {
  _$SkillItemModelCopyWithImpl(this._self, this._then);

  final SkillItemModel _self;
  final $Res Function(SkillItemModel) _then;

/// Create a copy of SkillItemModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? skill = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,skill: freezed == skill ? _self.skill : skill // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [SkillItemModel].
extension SkillItemModelPatterns on SkillItemModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _SkillItemModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _SkillItemModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _SkillItemModel value)  $default,){
final _that = this;
switch (_that) {
case _SkillItemModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _SkillItemModel value)?  $default,){
final _that = this;
switch (_that) {
case _SkillItemModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? skill)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _SkillItemModel() when $default != null:
return $default(_that.id,_that.skill);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? skill)  $default,) {final _that = this;
switch (_that) {
case _SkillItemModel():
return $default(_that.id,_that.skill);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? skill)?  $default,) {final _that = this;
switch (_that) {
case _SkillItemModel() when $default != null:
return $default(_that.id,_that.skill);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _SkillItemModel implements SkillItemModel {
  const _SkillItemModel({this.id, this.skill});
  factory _SkillItemModel.fromJson(Map<String, dynamic> json) => _$SkillItemModelFromJson(json);

@override final  String? id;
@override final  String? skill;

/// Create a copy of SkillItemModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$SkillItemModelCopyWith<_SkillItemModel> get copyWith => __$SkillItemModelCopyWithImpl<_SkillItemModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$SkillItemModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _SkillItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.skill, skill) || other.skill == skill));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,skill);

@override
String toString() {
  return 'SkillItemModel(id: $id, skill: $skill)';
}


}

/// @nodoc
abstract mixin class _$SkillItemModelCopyWith<$Res> implements $SkillItemModelCopyWith<$Res> {
  factory _$SkillItemModelCopyWith(_SkillItemModel value, $Res Function(_SkillItemModel) _then) = __$SkillItemModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? skill
});




}
/// @nodoc
class __$SkillItemModelCopyWithImpl<$Res>
    implements _$SkillItemModelCopyWith<$Res> {
  __$SkillItemModelCopyWithImpl(this._self, this._then);

  final _SkillItemModel _self;
  final $Res Function(_SkillItemModel) _then;

/// Create a copy of SkillItemModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? skill = freezed,}) {
  return _then(_SkillItemModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,skill: freezed == skill ? _self.skill : skill // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}


/// @nodoc
mixin _$CountryModel {

 String? get id; String? get value; String? get code;
/// Create a copy of CountryModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CountryModelCopyWith<CountryModel> get copyWith => _$CountryModelCopyWithImpl<CountryModel>(this as CountryModel, _$identity);

  /// Serializes this CountryModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CountryModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code);

@override
String toString() {
  return 'CountryModel(id: $id, value: $value, code: $code)';
}


}

/// @nodoc
abstract mixin class $CountryModelCopyWith<$Res>  {
  factory $CountryModelCopyWith(CountryModel value, $Res Function(CountryModel) _then) = _$CountryModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? value, String? code
});




}
/// @nodoc
class _$CountryModelCopyWithImpl<$Res>
    implements $CountryModelCopyWith<$Res> {
  _$CountryModelCopyWithImpl(this._self, this._then);

  final CountryModel _self;
  final $Res Function(CountryModel) _then;

/// Create a copy of CountryModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [CountryModel].
extension CountryModelPatterns on CountryModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CountryModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CountryModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CountryModel value)  $default,){
final _that = this;
switch (_that) {
case _CountryModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CountryModel value)?  $default,){
final _that = this;
switch (_that) {
case _CountryModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CountryModel() when $default != null:
return $default(_that.id,_that.value,_that.code);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code)  $default,) {final _that = this;
switch (_that) {
case _CountryModel():
return $default(_that.id,_that.value,_that.code);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? value,  String? code)?  $default,) {final _that = this;
switch (_that) {
case _CountryModel() when $default != null:
return $default(_that.id,_that.value,_that.code);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CountryModel implements CountryModel {
  const _CountryModel({this.id, this.value, this.code});
  factory _CountryModel.fromJson(Map<String, dynamic> json) => _$CountryModelFromJson(json);

@override final  String? id;
@override final  String? value;
@override final  String? code;

/// Create a copy of CountryModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CountryModelCopyWith<_CountryModel> get copyWith => __$CountryModelCopyWithImpl<_CountryModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CountryModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CountryModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code);

@override
String toString() {
  return 'CountryModel(id: $id, value: $value, code: $code)';
}


}

/// @nodoc
abstract mixin class _$CountryModelCopyWith<$Res> implements $CountryModelCopyWith<$Res> {
  factory _$CountryModelCopyWith(_CountryModel value, $Res Function(_CountryModel) _then) = __$CountryModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? value, String? code
});




}
/// @nodoc
class __$CountryModelCopyWithImpl<$Res>
    implements _$CountryModelCopyWith<$Res> {
  __$CountryModelCopyWithImpl(this._self, this._then);

  final _CountryModel _self;
  final $Res Function(_CountryModel) _then;

/// Create a copy of CountryModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,}) {
  return _then(_CountryModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}


/// @nodoc
mixin _$ProvinceModel {

 String? get id; String? get value; String? get code; CountryModel? get country;
/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProvinceModelCopyWith<ProvinceModel> get copyWith => _$ProvinceModelCopyWithImpl<ProvinceModel>(this as ProvinceModel, _$identity);

  /// Serializes this ProvinceModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProvinceModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code)&&(identical(other.country, country) || other.country == country));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code,country);

@override
String toString() {
  return 'ProvinceModel(id: $id, value: $value, code: $code, country: $country)';
}


}

/// @nodoc
abstract mixin class $ProvinceModelCopyWith<$Res>  {
  factory $ProvinceModelCopyWith(ProvinceModel value, $Res Function(ProvinceModel) _then) = _$ProvinceModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? value, String? code, CountryModel? country
});


$CountryModelCopyWith<$Res>? get country;

}
/// @nodoc
class _$ProvinceModelCopyWithImpl<$Res>
    implements $ProvinceModelCopyWith<$Res> {
  _$ProvinceModelCopyWithImpl(this._self, this._then);

  final ProvinceModel _self;
  final $Res Function(ProvinceModel) _then;

/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,Object? country = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as CountryModel?,
  ));
}
/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CountryModelCopyWith<$Res>? get country {
    if (_self.country == null) {
    return null;
  }

  return $CountryModelCopyWith<$Res>(_self.country!, (value) {
    return _then(_self.copyWith(country: value));
  });
}
}


/// Adds pattern-matching-related methods to [ProvinceModel].
extension ProvinceModelPatterns on ProvinceModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProvinceModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProvinceModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProvinceModel value)  $default,){
final _that = this;
switch (_that) {
case _ProvinceModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProvinceModel value)?  $default,){
final _that = this;
switch (_that) {
case _ProvinceModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code,  CountryModel? country)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProvinceModel() when $default != null:
return $default(_that.id,_that.value,_that.code,_that.country);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code,  CountryModel? country)  $default,) {final _that = this;
switch (_that) {
case _ProvinceModel():
return $default(_that.id,_that.value,_that.code,_that.country);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? value,  String? code,  CountryModel? country)?  $default,) {final _that = this;
switch (_that) {
case _ProvinceModel() when $default != null:
return $default(_that.id,_that.value,_that.code,_that.country);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ProvinceModel implements ProvinceModel {
  const _ProvinceModel({this.id, this.value, this.code, this.country});
  factory _ProvinceModel.fromJson(Map<String, dynamic> json) => _$ProvinceModelFromJson(json);

@override final  String? id;
@override final  String? value;
@override final  String? code;
@override final  CountryModel? country;

/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProvinceModelCopyWith<_ProvinceModel> get copyWith => __$ProvinceModelCopyWithImpl<_ProvinceModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ProvinceModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProvinceModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code)&&(identical(other.country, country) || other.country == country));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code,country);

@override
String toString() {
  return 'ProvinceModel(id: $id, value: $value, code: $code, country: $country)';
}


}

/// @nodoc
abstract mixin class _$ProvinceModelCopyWith<$Res> implements $ProvinceModelCopyWith<$Res> {
  factory _$ProvinceModelCopyWith(_ProvinceModel value, $Res Function(_ProvinceModel) _then) = __$ProvinceModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? value, String? code, CountryModel? country
});


@override $CountryModelCopyWith<$Res>? get country;

}
/// @nodoc
class __$ProvinceModelCopyWithImpl<$Res>
    implements _$ProvinceModelCopyWith<$Res> {
  __$ProvinceModelCopyWithImpl(this._self, this._then);

  final _ProvinceModel _self;
  final $Res Function(_ProvinceModel) _then;

/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,Object? country = freezed,}) {
  return _then(_ProvinceModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as CountryModel?,
  ));
}

/// Create a copy of ProvinceModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CountryModelCopyWith<$Res>? get country {
    if (_self.country == null) {
    return null;
  }

  return $CountryModelCopyWith<$Res>(_self.country!, (value) {
    return _then(_self.copyWith(country: value));
  });
}
}


/// @nodoc
mixin _$CityModel {

 String? get id; String? get value; String? get code; ProvinceModel? get province;
/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CityModelCopyWith<CityModel> get copyWith => _$CityModelCopyWithImpl<CityModel>(this as CityModel, _$identity);

  /// Serializes this CityModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CityModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code)&&(identical(other.province, province) || other.province == province));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code,province);

@override
String toString() {
  return 'CityModel(id: $id, value: $value, code: $code, province: $province)';
}


}

/// @nodoc
abstract mixin class $CityModelCopyWith<$Res>  {
  factory $CityModelCopyWith(CityModel value, $Res Function(CityModel) _then) = _$CityModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? value, String? code, ProvinceModel? province
});


$ProvinceModelCopyWith<$Res>? get province;

}
/// @nodoc
class _$CityModelCopyWithImpl<$Res>
    implements $CityModelCopyWith<$Res> {
  _$CityModelCopyWithImpl(this._self, this._then);

  final CityModel _self;
  final $Res Function(CityModel) _then;

/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,Object? province = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,province: freezed == province ? _self.province : province // ignore: cast_nullable_to_non_nullable
as ProvinceModel?,
  ));
}
/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProvinceModelCopyWith<$Res>? get province {
    if (_self.province == null) {
    return null;
  }

  return $ProvinceModelCopyWith<$Res>(_self.province!, (value) {
    return _then(_self.copyWith(province: value));
  });
}
}


/// Adds pattern-matching-related methods to [CityModel].
extension CityModelPatterns on CityModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CityModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CityModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CityModel value)  $default,){
final _that = this;
switch (_that) {
case _CityModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CityModel value)?  $default,){
final _that = this;
switch (_that) {
case _CityModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code,  ProvinceModel? province)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CityModel() when $default != null:
return $default(_that.id,_that.value,_that.code,_that.province);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? value,  String? code,  ProvinceModel? province)  $default,) {final _that = this;
switch (_that) {
case _CityModel():
return $default(_that.id,_that.value,_that.code,_that.province);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? value,  String? code,  ProvinceModel? province)?  $default,) {final _that = this;
switch (_that) {
case _CityModel() when $default != null:
return $default(_that.id,_that.value,_that.code,_that.province);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CityModel implements CityModel {
  const _CityModel({this.id, this.value, this.code, this.province});
  factory _CityModel.fromJson(Map<String, dynamic> json) => _$CityModelFromJson(json);

@override final  String? id;
@override final  String? value;
@override final  String? code;
@override final  ProvinceModel? province;

/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CityModelCopyWith<_CityModel> get copyWith => __$CityModelCopyWithImpl<_CityModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CityModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CityModel&&(identical(other.id, id) || other.id == id)&&(identical(other.value, value) || other.value == value)&&(identical(other.code, code) || other.code == code)&&(identical(other.province, province) || other.province == province));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,value,code,province);

@override
String toString() {
  return 'CityModel(id: $id, value: $value, code: $code, province: $province)';
}


}

/// @nodoc
abstract mixin class _$CityModelCopyWith<$Res> implements $CityModelCopyWith<$Res> {
  factory _$CityModelCopyWith(_CityModel value, $Res Function(_CityModel) _then) = __$CityModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? value, String? code, ProvinceModel? province
});


@override $ProvinceModelCopyWith<$Res>? get province;

}
/// @nodoc
class __$CityModelCopyWithImpl<$Res>
    implements _$CityModelCopyWith<$Res> {
  __$CityModelCopyWithImpl(this._self, this._then);

  final _CityModel _self;
  final $Res Function(_CityModel) _then;

/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? value = freezed,Object? code = freezed,Object? province = freezed,}) {
  return _then(_CityModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,value: freezed == value ? _self.value : value // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,province: freezed == province ? _self.province : province // ignore: cast_nullable_to_non_nullable
as ProvinceModel?,
  ));
}

/// Create a copy of CityModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProvinceModelCopyWith<$Res>? get province {
    if (_self.province == null) {
    return null;
  }

  return $ProvinceModelCopyWith<$Res>(_self.province!, (value) {
    return _then(_self.copyWith(province: value));
  });
}
}


/// @nodoc
mixin _$LocationModel {

 String? get id; String? get address; CityModel? get city; String? get postalCode; String? get entrance; String? get mainIntersection; bool get isBilling; double? get latitude; double? get longitude; String? get formattedAddress; bool get isUSA;
/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$LocationModelCopyWith<LocationModel> get copyWith => _$LocationModelCopyWithImpl<LocationModel>(this as LocationModel, _$identity);

  /// Serializes this LocationModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is LocationModel&&(identical(other.id, id) || other.id == id)&&(identical(other.address, address) || other.address == address)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.entrance, entrance) || other.entrance == entrance)&&(identical(other.mainIntersection, mainIntersection) || other.mainIntersection == mainIntersection)&&(identical(other.isBilling, isBilling) || other.isBilling == isBilling)&&(identical(other.latitude, latitude) || other.latitude == latitude)&&(identical(other.longitude, longitude) || other.longitude == longitude)&&(identical(other.formattedAddress, formattedAddress) || other.formattedAddress == formattedAddress)&&(identical(other.isUSA, isUSA) || other.isUSA == isUSA));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,address,city,postalCode,entrance,mainIntersection,isBilling,latitude,longitude,formattedAddress,isUSA);

@override
String toString() {
  return 'LocationModel(id: $id, address: $address, city: $city, postalCode: $postalCode, entrance: $entrance, mainIntersection: $mainIntersection, isBilling: $isBilling, latitude: $latitude, longitude: $longitude, formattedAddress: $formattedAddress, isUSA: $isUSA)';
}


}

/// @nodoc
abstract mixin class $LocationModelCopyWith<$Res>  {
  factory $LocationModelCopyWith(LocationModel value, $Res Function(LocationModel) _then) = _$LocationModelCopyWithImpl;
@useResult
$Res call({
 String? id, String? address, CityModel? city, String? postalCode, String? entrance, String? mainIntersection, bool isBilling, double? latitude, double? longitude, String? formattedAddress, bool isUSA
});


$CityModelCopyWith<$Res>? get city;

}
/// @nodoc
class _$LocationModelCopyWithImpl<$Res>
    implements $LocationModelCopyWith<$Res> {
  _$LocationModelCopyWithImpl(this._self, this._then);

  final LocationModel _self;
  final $Res Function(LocationModel) _then;

/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? address = freezed,Object? city = freezed,Object? postalCode = freezed,Object? entrance = freezed,Object? mainIntersection = freezed,Object? isBilling = null,Object? latitude = freezed,Object? longitude = freezed,Object? formattedAddress = freezed,Object? isUSA = null,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as CityModel?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,entrance: freezed == entrance ? _self.entrance : entrance // ignore: cast_nullable_to_non_nullable
as String?,mainIntersection: freezed == mainIntersection ? _self.mainIntersection : mainIntersection // ignore: cast_nullable_to_non_nullable
as String?,isBilling: null == isBilling ? _self.isBilling : isBilling // ignore: cast_nullable_to_non_nullable
as bool,latitude: freezed == latitude ? _self.latitude : latitude // ignore: cast_nullable_to_non_nullable
as double?,longitude: freezed == longitude ? _self.longitude : longitude // ignore: cast_nullable_to_non_nullable
as double?,formattedAddress: freezed == formattedAddress ? _self.formattedAddress : formattedAddress // ignore: cast_nullable_to_non_nullable
as String?,isUSA: null == isUSA ? _self.isUSA : isUSA // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}
/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CityModelCopyWith<$Res>? get city {
    if (_self.city == null) {
    return null;
  }

  return $CityModelCopyWith<$Res>(_self.city!, (value) {
    return _then(_self.copyWith(city: value));
  });
}
}


/// Adds pattern-matching-related methods to [LocationModel].
extension LocationModelPatterns on LocationModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _LocationModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _LocationModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _LocationModel value)  $default,){
final _that = this;
switch (_that) {
case _LocationModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _LocationModel value)?  $default,){
final _that = this;
switch (_that) {
case _LocationModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? address,  CityModel? city,  String? postalCode,  String? entrance,  String? mainIntersection,  bool isBilling,  double? latitude,  double? longitude,  String? formattedAddress,  bool isUSA)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _LocationModel() when $default != null:
return $default(_that.id,_that.address,_that.city,_that.postalCode,_that.entrance,_that.mainIntersection,_that.isBilling,_that.latitude,_that.longitude,_that.formattedAddress,_that.isUSA);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? address,  CityModel? city,  String? postalCode,  String? entrance,  String? mainIntersection,  bool isBilling,  double? latitude,  double? longitude,  String? formattedAddress,  bool isUSA)  $default,) {final _that = this;
switch (_that) {
case _LocationModel():
return $default(_that.id,_that.address,_that.city,_that.postalCode,_that.entrance,_that.mainIntersection,_that.isBilling,_that.latitude,_that.longitude,_that.formattedAddress,_that.isUSA);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? address,  CityModel? city,  String? postalCode,  String? entrance,  String? mainIntersection,  bool isBilling,  double? latitude,  double? longitude,  String? formattedAddress,  bool isUSA)?  $default,) {final _that = this;
switch (_that) {
case _LocationModel() when $default != null:
return $default(_that.id,_that.address,_that.city,_that.postalCode,_that.entrance,_that.mainIntersection,_that.isBilling,_that.latitude,_that.longitude,_that.formattedAddress,_that.isUSA);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _LocationModel implements LocationModel {
  const _LocationModel({this.id, this.address, this.city, this.postalCode, this.entrance, this.mainIntersection, this.isBilling = false, this.latitude, this.longitude, this.formattedAddress, this.isUSA = false});
  factory _LocationModel.fromJson(Map<String, dynamic> json) => _$LocationModelFromJson(json);

@override final  String? id;
@override final  String? address;
@override final  CityModel? city;
@override final  String? postalCode;
@override final  String? entrance;
@override final  String? mainIntersection;
@override@JsonKey() final  bool isBilling;
@override final  double? latitude;
@override final  double? longitude;
@override final  String? formattedAddress;
@override@JsonKey() final  bool isUSA;

/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$LocationModelCopyWith<_LocationModel> get copyWith => __$LocationModelCopyWithImpl<_LocationModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$LocationModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _LocationModel&&(identical(other.id, id) || other.id == id)&&(identical(other.address, address) || other.address == address)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.entrance, entrance) || other.entrance == entrance)&&(identical(other.mainIntersection, mainIntersection) || other.mainIntersection == mainIntersection)&&(identical(other.isBilling, isBilling) || other.isBilling == isBilling)&&(identical(other.latitude, latitude) || other.latitude == latitude)&&(identical(other.longitude, longitude) || other.longitude == longitude)&&(identical(other.formattedAddress, formattedAddress) || other.formattedAddress == formattedAddress)&&(identical(other.isUSA, isUSA) || other.isUSA == isUSA));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,address,city,postalCode,entrance,mainIntersection,isBilling,latitude,longitude,formattedAddress,isUSA);

@override
String toString() {
  return 'LocationModel(id: $id, address: $address, city: $city, postalCode: $postalCode, entrance: $entrance, mainIntersection: $mainIntersection, isBilling: $isBilling, latitude: $latitude, longitude: $longitude, formattedAddress: $formattedAddress, isUSA: $isUSA)';
}


}

/// @nodoc
abstract mixin class _$LocationModelCopyWith<$Res> implements $LocationModelCopyWith<$Res> {
  factory _$LocationModelCopyWith(_LocationModel value, $Res Function(_LocationModel) _then) = __$LocationModelCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? address, CityModel? city, String? postalCode, String? entrance, String? mainIntersection, bool isBilling, double? latitude, double? longitude, String? formattedAddress, bool isUSA
});


@override $CityModelCopyWith<$Res>? get city;

}
/// @nodoc
class __$LocationModelCopyWithImpl<$Res>
    implements _$LocationModelCopyWith<$Res> {
  __$LocationModelCopyWithImpl(this._self, this._then);

  final _LocationModel _self;
  final $Res Function(_LocationModel) _then;

/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? address = freezed,Object? city = freezed,Object? postalCode = freezed,Object? entrance = freezed,Object? mainIntersection = freezed,Object? isBilling = null,Object? latitude = freezed,Object? longitude = freezed,Object? formattedAddress = freezed,Object? isUSA = null,}) {
  return _then(_LocationModel(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as CityModel?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,entrance: freezed == entrance ? _self.entrance : entrance // ignore: cast_nullable_to_non_nullable
as String?,mainIntersection: freezed == mainIntersection ? _self.mainIntersection : mainIntersection // ignore: cast_nullable_to_non_nullable
as String?,isBilling: null == isBilling ? _self.isBilling : isBilling // ignore: cast_nullable_to_non_nullable
as bool,latitude: freezed == latitude ? _self.latitude : latitude // ignore: cast_nullable_to_non_nullable
as double?,longitude: freezed == longitude ? _self.longitude : longitude // ignore: cast_nullable_to_non_nullable
as double?,formattedAddress: freezed == formattedAddress ? _self.formattedAddress : formattedAddress // ignore: cast_nullable_to_non_nullable
as String?,isUSA: null == isUSA ? _self.isUSA : isUSA // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

/// Create a copy of LocationModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CityModelCopyWith<$Res>? get city {
    if (_self.city == null) {
    return null;
  }

  return $CityModelCopyWith<$Res>(_self.city!, (value) {
    return _then(_self.copyWith(city: value));
  });
}
}


/// @nodoc
mixin _$LicenseItemModel {

 ProfileImageModel? get license; String? get number; String? get issued; String? get expires;
/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$LicenseItemModelCopyWith<LicenseItemModel> get copyWith => _$LicenseItemModelCopyWithImpl<LicenseItemModel>(this as LicenseItemModel, _$identity);

  /// Serializes this LicenseItemModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is LicenseItemModel&&(identical(other.license, license) || other.license == license)&&(identical(other.number, number) || other.number == number)&&(identical(other.issued, issued) || other.issued == issued)&&(identical(other.expires, expires) || other.expires == expires));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,license,number,issued,expires);

@override
String toString() {
  return 'LicenseItemModel(license: $license, number: $number, issued: $issued, expires: $expires)';
}


}

/// @nodoc
abstract mixin class $LicenseItemModelCopyWith<$Res>  {
  factory $LicenseItemModelCopyWith(LicenseItemModel value, $Res Function(LicenseItemModel) _then) = _$LicenseItemModelCopyWithImpl;
@useResult
$Res call({
 ProfileImageModel? license, String? number, String? issued, String? expires
});


$ProfileImageModelCopyWith<$Res>? get license;

}
/// @nodoc
class _$LicenseItemModelCopyWithImpl<$Res>
    implements $LicenseItemModelCopyWith<$Res> {
  _$LicenseItemModelCopyWithImpl(this._self, this._then);

  final LicenseItemModel _self;
  final $Res Function(LicenseItemModel) _then;

/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? license = freezed,Object? number = freezed,Object? issued = freezed,Object? expires = freezed,}) {
  return _then(_self.copyWith(
license: freezed == license ? _self.license : license // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,number: freezed == number ? _self.number : number // ignore: cast_nullable_to_non_nullable
as String?,issued: freezed == issued ? _self.issued : issued // ignore: cast_nullable_to_non_nullable
as String?,expires: freezed == expires ? _self.expires : expires // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}
/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get license {
    if (_self.license == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.license!, (value) {
    return _then(_self.copyWith(license: value));
  });
}
}


/// Adds pattern-matching-related methods to [LicenseItemModel].
extension LicenseItemModelPatterns on LicenseItemModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _LicenseItemModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _LicenseItemModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _LicenseItemModel value)  $default,){
final _that = this;
switch (_that) {
case _LicenseItemModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _LicenseItemModel value)?  $default,){
final _that = this;
switch (_that) {
case _LicenseItemModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( ProfileImageModel? license,  String? number,  String? issued,  String? expires)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _LicenseItemModel() when $default != null:
return $default(_that.license,_that.number,_that.issued,_that.expires);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( ProfileImageModel? license,  String? number,  String? issued,  String? expires)  $default,) {final _that = this;
switch (_that) {
case _LicenseItemModel():
return $default(_that.license,_that.number,_that.issued,_that.expires);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( ProfileImageModel? license,  String? number,  String? issued,  String? expires)?  $default,) {final _that = this;
switch (_that) {
case _LicenseItemModel() when $default != null:
return $default(_that.license,_that.number,_that.issued,_that.expires);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _LicenseItemModel implements LicenseItemModel {
  const _LicenseItemModel({this.license, this.number, this.issued, this.expires});
  factory _LicenseItemModel.fromJson(Map<String, dynamic> json) => _$LicenseItemModelFromJson(json);

@override final  ProfileImageModel? license;
@override final  String? number;
@override final  String? issued;
@override final  String? expires;

/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$LicenseItemModelCopyWith<_LicenseItemModel> get copyWith => __$LicenseItemModelCopyWithImpl<_LicenseItemModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$LicenseItemModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _LicenseItemModel&&(identical(other.license, license) || other.license == license)&&(identical(other.number, number) || other.number == number)&&(identical(other.issued, issued) || other.issued == issued)&&(identical(other.expires, expires) || other.expires == expires));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,license,number,issued,expires);

@override
String toString() {
  return 'LicenseItemModel(license: $license, number: $number, issued: $issued, expires: $expires)';
}


}

/// @nodoc
abstract mixin class _$LicenseItemModelCopyWith<$Res> implements $LicenseItemModelCopyWith<$Res> {
  factory _$LicenseItemModelCopyWith(_LicenseItemModel value, $Res Function(_LicenseItemModel) _then) = __$LicenseItemModelCopyWithImpl;
@override @useResult
$Res call({
 ProfileImageModel? license, String? number, String? issued, String? expires
});


@override $ProfileImageModelCopyWith<$Res>? get license;

}
/// @nodoc
class __$LicenseItemModelCopyWithImpl<$Res>
    implements _$LicenseItemModelCopyWith<$Res> {
  __$LicenseItemModelCopyWithImpl(this._self, this._then);

  final _LicenseItemModel _self;
  final $Res Function(_LicenseItemModel) _then;

/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? license = freezed,Object? number = freezed,Object? issued = freezed,Object? expires = freezed,}) {
  return _then(_LicenseItemModel(
license: freezed == license ? _self.license : license // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,number: freezed == number ? _self.number : number // ignore: cast_nullable_to_non_nullable
as String?,issued: freezed == issued ? _self.issued : issued // ignore: cast_nullable_to_non_nullable
as String?,expires: freezed == expires ? _self.expires : expires // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

/// Create a copy of LicenseItemModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get license {
    if (_self.license == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.license!, (value) {
    return _then(_self.copyWith(license: value));
  });
}
}


/// @nodoc
mixin _$WorkerProfileListItemModel {

 String get id; String? get agencyFullName; String? get agencyLogo;
/// Create a copy of WorkerProfileListItemModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WorkerProfileListItemModelCopyWith<WorkerProfileListItemModel> get copyWith => _$WorkerProfileListItemModelCopyWithImpl<WorkerProfileListItemModel>(this as WorkerProfileListItemModel, _$identity);

  /// Serializes this WorkerProfileListItemModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WorkerProfileListItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.agencyFullName, agencyFullName) || other.agencyFullName == agencyFullName)&&(identical(other.agencyLogo, agencyLogo) || other.agencyLogo == agencyLogo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,agencyFullName,agencyLogo);

@override
String toString() {
  return 'WorkerProfileListItemModel(id: $id, agencyFullName: $agencyFullName, agencyLogo: $agencyLogo)';
}


}

/// @nodoc
abstract mixin class $WorkerProfileListItemModelCopyWith<$Res>  {
  factory $WorkerProfileListItemModelCopyWith(WorkerProfileListItemModel value, $Res Function(WorkerProfileListItemModel) _then) = _$WorkerProfileListItemModelCopyWithImpl;
@useResult
$Res call({
 String id, String? agencyFullName, String? agencyLogo
});




}
/// @nodoc
class _$WorkerProfileListItemModelCopyWithImpl<$Res>
    implements $WorkerProfileListItemModelCopyWith<$Res> {
  _$WorkerProfileListItemModelCopyWithImpl(this._self, this._then);

  final WorkerProfileListItemModel _self;
  final $Res Function(WorkerProfileListItemModel) _then;

/// Create a copy of WorkerProfileListItemModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? agencyFullName = freezed,Object? agencyLogo = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,agencyFullName: freezed == agencyFullName ? _self.agencyFullName : agencyFullName // ignore: cast_nullable_to_non_nullable
as String?,agencyLogo: freezed == agencyLogo ? _self.agencyLogo : agencyLogo // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [WorkerProfileListItemModel].
extension WorkerProfileListItemModelPatterns on WorkerProfileListItemModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WorkerProfileListItemModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WorkerProfileListItemModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WorkerProfileListItemModel value)  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileListItemModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WorkerProfileListItemModel value)?  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileListItemModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String? agencyFullName,  String? agencyLogo)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WorkerProfileListItemModel() when $default != null:
return $default(_that.id,_that.agencyFullName,_that.agencyLogo);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String? agencyFullName,  String? agencyLogo)  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileListItemModel():
return $default(_that.id,_that.agencyFullName,_that.agencyLogo);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String? agencyFullName,  String? agencyLogo)?  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileListItemModel() when $default != null:
return $default(_that.id,_that.agencyFullName,_that.agencyLogo);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WorkerProfileListItemModel implements WorkerProfileListItemModel {
  const _WorkerProfileListItemModel({required this.id, this.agencyFullName, this.agencyLogo});
  factory _WorkerProfileListItemModel.fromJson(Map<String, dynamic> json) => _$WorkerProfileListItemModelFromJson(json);

@override final  String id;
@override final  String? agencyFullName;
@override final  String? agencyLogo;

/// Create a copy of WorkerProfileListItemModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WorkerProfileListItemModelCopyWith<_WorkerProfileListItemModel> get copyWith => __$WorkerProfileListItemModelCopyWithImpl<_WorkerProfileListItemModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WorkerProfileListItemModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _WorkerProfileListItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.agencyFullName, agencyFullName) || other.agencyFullName == agencyFullName)&&(identical(other.agencyLogo, agencyLogo) || other.agencyLogo == agencyLogo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,agencyFullName,agencyLogo);

@override
String toString() {
  return 'WorkerProfileListItemModel(id: $id, agencyFullName: $agencyFullName, agencyLogo: $agencyLogo)';
}


}

/// @nodoc
abstract mixin class _$WorkerProfileListItemModelCopyWith<$Res> implements $WorkerProfileListItemModelCopyWith<$Res> {
  factory _$WorkerProfileListItemModelCopyWith(_WorkerProfileListItemModel value, $Res Function(_WorkerProfileListItemModel) _then) = __$WorkerProfileListItemModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String? agencyFullName, String? agencyLogo
});




}
/// @nodoc
class __$WorkerProfileListItemModelCopyWithImpl<$Res>
    implements _$WorkerProfileListItemModelCopyWith<$Res> {
  __$WorkerProfileListItemModelCopyWithImpl(this._self, this._then);

  final _WorkerProfileListItemModel _self;
  final $Res Function(_WorkerProfileListItemModel) _then;

/// Create a copy of WorkerProfileListItemModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? agencyFullName = freezed,Object? agencyLogo = freezed,}) {
  return _then(_WorkerProfileListItemModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,agencyFullName: freezed == agencyFullName ? _self.agencyFullName : agencyFullName // ignore: cast_nullable_to_non_nullable
as String?,agencyLogo: freezed == agencyLogo ? _self.agencyLogo : agencyLogo // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}


/// @nodoc
mixin _$WorkerProfileModel {

 String get id; int? get numberId; ProfileImageModel? get profileImage; String? get firstName; String? get middleName; String? get lastName; String? get secondLastName; String? get birthDay; CatalogItemModel? get gender; String? get socialInsurance; bool get socialInsuranceExpire; String? get dueDate; ProfileImageModel? get socialInsuranceFile; String? get identificationNumber1; String? get identificationNumber2; bool get havePoliceCheckBackground; ProfileImageModel? get identificationType1File; ProfileImageModel? get identificationType2File; CatalogItemModel? get identificationType1; CatalogItemModel? get identificationType2; ProfileImageModel? get policeCheckBackGround; String? get mobileNumber; String? get phone; String? get phoneExt; LocationModel? get location; bool get hasVehicle; List<LicenseItemModel> get licenses; List<ProfileImageModel> get certificates; List<CatalogItemModel> get otherDocuments; List<CatalogItemModel> get availabilities; List<CatalogItemModel> get availabilityTimes; List<CatalogItemModel> get availabilityDays; List<CatalogItemModel> get locationPreferences; CatalogItemModel? get lift; List<CatalogItemModel> get languages; List<SkillItemModel> get skills; ProfileImageModel? get resume; bool get haveAnyHealthProblem; String? get healthProblem; String? get otherHealthProblem; String? get contactEmergencyName; String? get contactEmergencyLastName; String? get contactEmergencyPhone; String? get email; bool get approvedToWork; String? get workerId; bool get isSubcontractor; bool get isContractor; bool get dnu; String? get punchCardId;
/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$WorkerProfileModelCopyWith<WorkerProfileModel> get copyWith => _$WorkerProfileModelCopyWithImpl<WorkerProfileModel>(this as WorkerProfileModel, _$identity);

  /// Serializes this WorkerProfileModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is WorkerProfileModel&&(identical(other.id, id) || other.id == id)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.profileImage, profileImage) || other.profileImage == profileImage)&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.middleName, middleName) || other.middleName == middleName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.secondLastName, secondLastName) || other.secondLastName == secondLastName)&&(identical(other.birthDay, birthDay) || other.birthDay == birthDay)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.socialInsurance, socialInsurance) || other.socialInsurance == socialInsurance)&&(identical(other.socialInsuranceExpire, socialInsuranceExpire) || other.socialInsuranceExpire == socialInsuranceExpire)&&(identical(other.dueDate, dueDate) || other.dueDate == dueDate)&&(identical(other.socialInsuranceFile, socialInsuranceFile) || other.socialInsuranceFile == socialInsuranceFile)&&(identical(other.identificationNumber1, identificationNumber1) || other.identificationNumber1 == identificationNumber1)&&(identical(other.identificationNumber2, identificationNumber2) || other.identificationNumber2 == identificationNumber2)&&(identical(other.havePoliceCheckBackground, havePoliceCheckBackground) || other.havePoliceCheckBackground == havePoliceCheckBackground)&&(identical(other.identificationType1File, identificationType1File) || other.identificationType1File == identificationType1File)&&(identical(other.identificationType2File, identificationType2File) || other.identificationType2File == identificationType2File)&&(identical(other.identificationType1, identificationType1) || other.identificationType1 == identificationType1)&&(identical(other.identificationType2, identificationType2) || other.identificationType2 == identificationType2)&&(identical(other.policeCheckBackGround, policeCheckBackGround) || other.policeCheckBackGround == policeCheckBackGround)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.phoneExt, phoneExt) || other.phoneExt == phoneExt)&&(identical(other.location, location) || other.location == location)&&(identical(other.hasVehicle, hasVehicle) || other.hasVehicle == hasVehicle)&&const DeepCollectionEquality().equals(other.licenses, licenses)&&const DeepCollectionEquality().equals(other.certificates, certificates)&&const DeepCollectionEquality().equals(other.otherDocuments, otherDocuments)&&const DeepCollectionEquality().equals(other.availabilities, availabilities)&&const DeepCollectionEquality().equals(other.availabilityTimes, availabilityTimes)&&const DeepCollectionEquality().equals(other.availabilityDays, availabilityDays)&&const DeepCollectionEquality().equals(other.locationPreferences, locationPreferences)&&(identical(other.lift, lift) || other.lift == lift)&&const DeepCollectionEquality().equals(other.languages, languages)&&const DeepCollectionEquality().equals(other.skills, skills)&&(identical(other.resume, resume) || other.resume == resume)&&(identical(other.haveAnyHealthProblem, haveAnyHealthProblem) || other.haveAnyHealthProblem == haveAnyHealthProblem)&&(identical(other.healthProblem, healthProblem) || other.healthProblem == healthProblem)&&(identical(other.otherHealthProblem, otherHealthProblem) || other.otherHealthProblem == otherHealthProblem)&&(identical(other.contactEmergencyName, contactEmergencyName) || other.contactEmergencyName == contactEmergencyName)&&(identical(other.contactEmergencyLastName, contactEmergencyLastName) || other.contactEmergencyLastName == contactEmergencyLastName)&&(identical(other.contactEmergencyPhone, contactEmergencyPhone) || other.contactEmergencyPhone == contactEmergencyPhone)&&(identical(other.email, email) || other.email == email)&&(identical(other.approvedToWork, approvedToWork) || other.approvedToWork == approvedToWork)&&(identical(other.workerId, workerId) || other.workerId == workerId)&&(identical(other.isSubcontractor, isSubcontractor) || other.isSubcontractor == isSubcontractor)&&(identical(other.isContractor, isContractor) || other.isContractor == isContractor)&&(identical(other.dnu, dnu) || other.dnu == dnu)&&(identical(other.punchCardId, punchCardId) || other.punchCardId == punchCardId));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,numberId,profileImage,firstName,middleName,lastName,secondLastName,birthDay,gender,socialInsurance,socialInsuranceExpire,dueDate,socialInsuranceFile,identificationNumber1,identificationNumber2,havePoliceCheckBackground,identificationType1File,identificationType2File,identificationType1,identificationType2,policeCheckBackGround,mobileNumber,phone,phoneExt,location,hasVehicle,const DeepCollectionEquality().hash(licenses),const DeepCollectionEquality().hash(certificates),const DeepCollectionEquality().hash(otherDocuments),const DeepCollectionEquality().hash(availabilities),const DeepCollectionEquality().hash(availabilityTimes),const DeepCollectionEquality().hash(availabilityDays),const DeepCollectionEquality().hash(locationPreferences),lift,const DeepCollectionEquality().hash(languages),const DeepCollectionEquality().hash(skills),resume,haveAnyHealthProblem,healthProblem,otherHealthProblem,contactEmergencyName,contactEmergencyLastName,contactEmergencyPhone,email,approvedToWork,workerId,isSubcontractor,isContractor,dnu,punchCardId]);

@override
String toString() {
  return 'WorkerProfileModel(id: $id, numberId: $numberId, profileImage: $profileImage, firstName: $firstName, middleName: $middleName, lastName: $lastName, secondLastName: $secondLastName, birthDay: $birthDay, gender: $gender, socialInsurance: $socialInsurance, socialInsuranceExpire: $socialInsuranceExpire, dueDate: $dueDate, socialInsuranceFile: $socialInsuranceFile, identificationNumber1: $identificationNumber1, identificationNumber2: $identificationNumber2, havePoliceCheckBackground: $havePoliceCheckBackground, identificationType1File: $identificationType1File, identificationType2File: $identificationType2File, identificationType1: $identificationType1, identificationType2: $identificationType2, policeCheckBackGround: $policeCheckBackGround, mobileNumber: $mobileNumber, phone: $phone, phoneExt: $phoneExt, location: $location, hasVehicle: $hasVehicle, licenses: $licenses, certificates: $certificates, otherDocuments: $otherDocuments, availabilities: $availabilities, availabilityTimes: $availabilityTimes, availabilityDays: $availabilityDays, locationPreferences: $locationPreferences, lift: $lift, languages: $languages, skills: $skills, resume: $resume, haveAnyHealthProblem: $haveAnyHealthProblem, healthProblem: $healthProblem, otherHealthProblem: $otherHealthProblem, contactEmergencyName: $contactEmergencyName, contactEmergencyLastName: $contactEmergencyLastName, contactEmergencyPhone: $contactEmergencyPhone, email: $email, approvedToWork: $approvedToWork, workerId: $workerId, isSubcontractor: $isSubcontractor, isContractor: $isContractor, dnu: $dnu, punchCardId: $punchCardId)';
}


}

/// @nodoc
abstract mixin class $WorkerProfileModelCopyWith<$Res>  {
  factory $WorkerProfileModelCopyWith(WorkerProfileModel value, $Res Function(WorkerProfileModel) _then) = _$WorkerProfileModelCopyWithImpl;
@useResult
$Res call({
 String id, int? numberId, ProfileImageModel? profileImage, String? firstName, String? middleName, String? lastName, String? secondLastName, String? birthDay, CatalogItemModel? gender, String? socialInsurance, bool socialInsuranceExpire, String? dueDate, ProfileImageModel? socialInsuranceFile, String? identificationNumber1, String? identificationNumber2, bool havePoliceCheckBackground, ProfileImageModel? identificationType1File, ProfileImageModel? identificationType2File, CatalogItemModel? identificationType1, CatalogItemModel? identificationType2, ProfileImageModel? policeCheckBackGround, String? mobileNumber, String? phone, String? phoneExt, LocationModel? location, bool hasVehicle, List<LicenseItemModel> licenses, List<ProfileImageModel> certificates, List<CatalogItemModel> otherDocuments, List<CatalogItemModel> availabilities, List<CatalogItemModel> availabilityTimes, List<CatalogItemModel> availabilityDays, List<CatalogItemModel> locationPreferences, CatalogItemModel? lift, List<CatalogItemModel> languages, List<SkillItemModel> skills, ProfileImageModel? resume, bool haveAnyHealthProblem, String? healthProblem, String? otherHealthProblem, String? contactEmergencyName, String? contactEmergencyLastName, String? contactEmergencyPhone, String? email, bool approvedToWork, String? workerId, bool isSubcontractor, bool isContractor, bool dnu, String? punchCardId
});


$ProfileImageModelCopyWith<$Res>? get profileImage;$CatalogItemModelCopyWith<$Res>? get gender;$ProfileImageModelCopyWith<$Res>? get socialInsuranceFile;$ProfileImageModelCopyWith<$Res>? get identificationType1File;$ProfileImageModelCopyWith<$Res>? get identificationType2File;$CatalogItemModelCopyWith<$Res>? get identificationType1;$CatalogItemModelCopyWith<$Res>? get identificationType2;$ProfileImageModelCopyWith<$Res>? get policeCheckBackGround;$LocationModelCopyWith<$Res>? get location;$CatalogItemModelCopyWith<$Res>? get lift;$ProfileImageModelCopyWith<$Res>? get resume;

}
/// @nodoc
class _$WorkerProfileModelCopyWithImpl<$Res>
    implements $WorkerProfileModelCopyWith<$Res> {
  _$WorkerProfileModelCopyWithImpl(this._self, this._then);

  final WorkerProfileModel _self;
  final $Res Function(WorkerProfileModel) _then;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? numberId = freezed,Object? profileImage = freezed,Object? firstName = freezed,Object? middleName = freezed,Object? lastName = freezed,Object? secondLastName = freezed,Object? birthDay = freezed,Object? gender = freezed,Object? socialInsurance = freezed,Object? socialInsuranceExpire = null,Object? dueDate = freezed,Object? socialInsuranceFile = freezed,Object? identificationNumber1 = freezed,Object? identificationNumber2 = freezed,Object? havePoliceCheckBackground = null,Object? identificationType1File = freezed,Object? identificationType2File = freezed,Object? identificationType1 = freezed,Object? identificationType2 = freezed,Object? policeCheckBackGround = freezed,Object? mobileNumber = freezed,Object? phone = freezed,Object? phoneExt = freezed,Object? location = freezed,Object? hasVehicle = null,Object? licenses = null,Object? certificates = null,Object? otherDocuments = null,Object? availabilities = null,Object? availabilityTimes = null,Object? availabilityDays = null,Object? locationPreferences = null,Object? lift = freezed,Object? languages = null,Object? skills = null,Object? resume = freezed,Object? haveAnyHealthProblem = null,Object? healthProblem = freezed,Object? otherHealthProblem = freezed,Object? contactEmergencyName = freezed,Object? contactEmergencyLastName = freezed,Object? contactEmergencyPhone = freezed,Object? email = freezed,Object? approvedToWork = null,Object? workerId = freezed,Object? isSubcontractor = null,Object? isContractor = null,Object? dnu = null,Object? punchCardId = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,numberId: freezed == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int?,profileImage: freezed == profileImage ? _self.profileImage : profileImage // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,firstName: freezed == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String?,middleName: freezed == middleName ? _self.middleName : middleName // ignore: cast_nullable_to_non_nullable
as String?,lastName: freezed == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String?,secondLastName: freezed == secondLastName ? _self.secondLastName : secondLastName // ignore: cast_nullable_to_non_nullable
as String?,birthDay: freezed == birthDay ? _self.birthDay : birthDay // ignore: cast_nullable_to_non_nullable
as String?,gender: freezed == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,socialInsurance: freezed == socialInsurance ? _self.socialInsurance : socialInsurance // ignore: cast_nullable_to_non_nullable
as String?,socialInsuranceExpire: null == socialInsuranceExpire ? _self.socialInsuranceExpire : socialInsuranceExpire // ignore: cast_nullable_to_non_nullable
as bool,dueDate: freezed == dueDate ? _self.dueDate : dueDate // ignore: cast_nullable_to_non_nullable
as String?,socialInsuranceFile: freezed == socialInsuranceFile ? _self.socialInsuranceFile : socialInsuranceFile // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationNumber1: freezed == identificationNumber1 ? _self.identificationNumber1 : identificationNumber1 // ignore: cast_nullable_to_non_nullable
as String?,identificationNumber2: freezed == identificationNumber2 ? _self.identificationNumber2 : identificationNumber2 // ignore: cast_nullable_to_non_nullable
as String?,havePoliceCheckBackground: null == havePoliceCheckBackground ? _self.havePoliceCheckBackground : havePoliceCheckBackground // ignore: cast_nullable_to_non_nullable
as bool,identificationType1File: freezed == identificationType1File ? _self.identificationType1File : identificationType1File // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationType2File: freezed == identificationType2File ? _self.identificationType2File : identificationType2File // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationType1: freezed == identificationType1 ? _self.identificationType1 : identificationType1 // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,identificationType2: freezed == identificationType2 ? _self.identificationType2 : identificationType2 // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,policeCheckBackGround: freezed == policeCheckBackGround ? _self.policeCheckBackGround : policeCheckBackGround // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,mobileNumber: freezed == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String?,phone: freezed == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String?,phoneExt: freezed == phoneExt ? _self.phoneExt : phoneExt // ignore: cast_nullable_to_non_nullable
as String?,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as LocationModel?,hasVehicle: null == hasVehicle ? _self.hasVehicle : hasVehicle // ignore: cast_nullable_to_non_nullable
as bool,licenses: null == licenses ? _self.licenses : licenses // ignore: cast_nullable_to_non_nullable
as List<LicenseItemModel>,certificates: null == certificates ? _self.certificates : certificates // ignore: cast_nullable_to_non_nullable
as List<ProfileImageModel>,otherDocuments: null == otherDocuments ? _self.otherDocuments : otherDocuments // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilities: null == availabilities ? _self.availabilities : availabilities // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilityTimes: null == availabilityTimes ? _self.availabilityTimes : availabilityTimes // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilityDays: null == availabilityDays ? _self.availabilityDays : availabilityDays // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,locationPreferences: null == locationPreferences ? _self.locationPreferences : locationPreferences // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,lift: freezed == lift ? _self.lift : lift // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,languages: null == languages ? _self.languages : languages // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,skills: null == skills ? _self.skills : skills // ignore: cast_nullable_to_non_nullable
as List<SkillItemModel>,resume: freezed == resume ? _self.resume : resume // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,haveAnyHealthProblem: null == haveAnyHealthProblem ? _self.haveAnyHealthProblem : haveAnyHealthProblem // ignore: cast_nullable_to_non_nullable
as bool,healthProblem: freezed == healthProblem ? _self.healthProblem : healthProblem // ignore: cast_nullable_to_non_nullable
as String?,otherHealthProblem: freezed == otherHealthProblem ? _self.otherHealthProblem : otherHealthProblem // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyName: freezed == contactEmergencyName ? _self.contactEmergencyName : contactEmergencyName // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyLastName: freezed == contactEmergencyLastName ? _self.contactEmergencyLastName : contactEmergencyLastName // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyPhone: freezed == contactEmergencyPhone ? _self.contactEmergencyPhone : contactEmergencyPhone // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,approvedToWork: null == approvedToWork ? _self.approvedToWork : approvedToWork // ignore: cast_nullable_to_non_nullable
as bool,workerId: freezed == workerId ? _self.workerId : workerId // ignore: cast_nullable_to_non_nullable
as String?,isSubcontractor: null == isSubcontractor ? _self.isSubcontractor : isSubcontractor // ignore: cast_nullable_to_non_nullable
as bool,isContractor: null == isContractor ? _self.isContractor : isContractor // ignore: cast_nullable_to_non_nullable
as bool,dnu: null == dnu ? _self.dnu : dnu // ignore: cast_nullable_to_non_nullable
as bool,punchCardId: freezed == punchCardId ? _self.punchCardId : punchCardId // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}
/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get profileImage {
    if (_self.profileImage == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.profileImage!, (value) {
    return _then(_self.copyWith(profileImage: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get gender {
    if (_self.gender == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.gender!, (value) {
    return _then(_self.copyWith(gender: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get socialInsuranceFile {
    if (_self.socialInsuranceFile == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.socialInsuranceFile!, (value) {
    return _then(_self.copyWith(socialInsuranceFile: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get identificationType1File {
    if (_self.identificationType1File == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.identificationType1File!, (value) {
    return _then(_self.copyWith(identificationType1File: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get identificationType2File {
    if (_self.identificationType2File == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.identificationType2File!, (value) {
    return _then(_self.copyWith(identificationType2File: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get identificationType1 {
    if (_self.identificationType1 == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.identificationType1!, (value) {
    return _then(_self.copyWith(identificationType1: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get identificationType2 {
    if (_self.identificationType2 == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.identificationType2!, (value) {
    return _then(_self.copyWith(identificationType2: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get policeCheckBackGround {
    if (_self.policeCheckBackGround == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.policeCheckBackGround!, (value) {
    return _then(_self.copyWith(policeCheckBackGround: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$LocationModelCopyWith<$Res>? get location {
    if (_self.location == null) {
    return null;
  }

  return $LocationModelCopyWith<$Res>(_self.location!, (value) {
    return _then(_self.copyWith(location: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get lift {
    if (_self.lift == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.lift!, (value) {
    return _then(_self.copyWith(lift: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get resume {
    if (_self.resume == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.resume!, (value) {
    return _then(_self.copyWith(resume: value));
  });
}
}


/// Adds pattern-matching-related methods to [WorkerProfileModel].
extension WorkerProfileModelPatterns on WorkerProfileModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _WorkerProfileModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _WorkerProfileModel value)  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _WorkerProfileModel value)?  $default,){
final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  int? numberId,  ProfileImageModel? profileImage,  String? firstName,  String? middleName,  String? lastName,  String? secondLastName,  String? birthDay,  CatalogItemModel? gender,  String? socialInsurance,  bool socialInsuranceExpire,  String? dueDate,  ProfileImageModel? socialInsuranceFile,  String? identificationNumber1,  String? identificationNumber2,  bool havePoliceCheckBackground,  ProfileImageModel? identificationType1File,  ProfileImageModel? identificationType2File,  CatalogItemModel? identificationType1,  CatalogItemModel? identificationType2,  ProfileImageModel? policeCheckBackGround,  String? mobileNumber,  String? phone,  String? phoneExt,  LocationModel? location,  bool hasVehicle,  List<LicenseItemModel> licenses,  List<ProfileImageModel> certificates,  List<CatalogItemModel> otherDocuments,  List<CatalogItemModel> availabilities,  List<CatalogItemModel> availabilityTimes,  List<CatalogItemModel> availabilityDays,  List<CatalogItemModel> locationPreferences,  CatalogItemModel? lift,  List<CatalogItemModel> languages,  List<SkillItemModel> skills,  ProfileImageModel? resume,  bool haveAnyHealthProblem,  String? healthProblem,  String? otherHealthProblem,  String? contactEmergencyName,  String? contactEmergencyLastName,  String? contactEmergencyPhone,  String? email,  bool approvedToWork,  String? workerId,  bool isSubcontractor,  bool isContractor,  bool dnu,  String? punchCardId)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
return $default(_that.id,_that.numberId,_that.profileImage,_that.firstName,_that.middleName,_that.lastName,_that.secondLastName,_that.birthDay,_that.gender,_that.socialInsurance,_that.socialInsuranceExpire,_that.dueDate,_that.socialInsuranceFile,_that.identificationNumber1,_that.identificationNumber2,_that.havePoliceCheckBackground,_that.identificationType1File,_that.identificationType2File,_that.identificationType1,_that.identificationType2,_that.policeCheckBackGround,_that.mobileNumber,_that.phone,_that.phoneExt,_that.location,_that.hasVehicle,_that.licenses,_that.certificates,_that.otherDocuments,_that.availabilities,_that.availabilityTimes,_that.availabilityDays,_that.locationPreferences,_that.lift,_that.languages,_that.skills,_that.resume,_that.haveAnyHealthProblem,_that.healthProblem,_that.otherHealthProblem,_that.contactEmergencyName,_that.contactEmergencyLastName,_that.contactEmergencyPhone,_that.email,_that.approvedToWork,_that.workerId,_that.isSubcontractor,_that.isContractor,_that.dnu,_that.punchCardId);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  int? numberId,  ProfileImageModel? profileImage,  String? firstName,  String? middleName,  String? lastName,  String? secondLastName,  String? birthDay,  CatalogItemModel? gender,  String? socialInsurance,  bool socialInsuranceExpire,  String? dueDate,  ProfileImageModel? socialInsuranceFile,  String? identificationNumber1,  String? identificationNumber2,  bool havePoliceCheckBackground,  ProfileImageModel? identificationType1File,  ProfileImageModel? identificationType2File,  CatalogItemModel? identificationType1,  CatalogItemModel? identificationType2,  ProfileImageModel? policeCheckBackGround,  String? mobileNumber,  String? phone,  String? phoneExt,  LocationModel? location,  bool hasVehicle,  List<LicenseItemModel> licenses,  List<ProfileImageModel> certificates,  List<CatalogItemModel> otherDocuments,  List<CatalogItemModel> availabilities,  List<CatalogItemModel> availabilityTimes,  List<CatalogItemModel> availabilityDays,  List<CatalogItemModel> locationPreferences,  CatalogItemModel? lift,  List<CatalogItemModel> languages,  List<SkillItemModel> skills,  ProfileImageModel? resume,  bool haveAnyHealthProblem,  String? healthProblem,  String? otherHealthProblem,  String? contactEmergencyName,  String? contactEmergencyLastName,  String? contactEmergencyPhone,  String? email,  bool approvedToWork,  String? workerId,  bool isSubcontractor,  bool isContractor,  bool dnu,  String? punchCardId)  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileModel():
return $default(_that.id,_that.numberId,_that.profileImage,_that.firstName,_that.middleName,_that.lastName,_that.secondLastName,_that.birthDay,_that.gender,_that.socialInsurance,_that.socialInsuranceExpire,_that.dueDate,_that.socialInsuranceFile,_that.identificationNumber1,_that.identificationNumber2,_that.havePoliceCheckBackground,_that.identificationType1File,_that.identificationType2File,_that.identificationType1,_that.identificationType2,_that.policeCheckBackGround,_that.mobileNumber,_that.phone,_that.phoneExt,_that.location,_that.hasVehicle,_that.licenses,_that.certificates,_that.otherDocuments,_that.availabilities,_that.availabilityTimes,_that.availabilityDays,_that.locationPreferences,_that.lift,_that.languages,_that.skills,_that.resume,_that.haveAnyHealthProblem,_that.healthProblem,_that.otherHealthProblem,_that.contactEmergencyName,_that.contactEmergencyLastName,_that.contactEmergencyPhone,_that.email,_that.approvedToWork,_that.workerId,_that.isSubcontractor,_that.isContractor,_that.dnu,_that.punchCardId);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  int? numberId,  ProfileImageModel? profileImage,  String? firstName,  String? middleName,  String? lastName,  String? secondLastName,  String? birthDay,  CatalogItemModel? gender,  String? socialInsurance,  bool socialInsuranceExpire,  String? dueDate,  ProfileImageModel? socialInsuranceFile,  String? identificationNumber1,  String? identificationNumber2,  bool havePoliceCheckBackground,  ProfileImageModel? identificationType1File,  ProfileImageModel? identificationType2File,  CatalogItemModel? identificationType1,  CatalogItemModel? identificationType2,  ProfileImageModel? policeCheckBackGround,  String? mobileNumber,  String? phone,  String? phoneExt,  LocationModel? location,  bool hasVehicle,  List<LicenseItemModel> licenses,  List<ProfileImageModel> certificates,  List<CatalogItemModel> otherDocuments,  List<CatalogItemModel> availabilities,  List<CatalogItemModel> availabilityTimes,  List<CatalogItemModel> availabilityDays,  List<CatalogItemModel> locationPreferences,  CatalogItemModel? lift,  List<CatalogItemModel> languages,  List<SkillItemModel> skills,  ProfileImageModel? resume,  bool haveAnyHealthProblem,  String? healthProblem,  String? otherHealthProblem,  String? contactEmergencyName,  String? contactEmergencyLastName,  String? contactEmergencyPhone,  String? email,  bool approvedToWork,  String? workerId,  bool isSubcontractor,  bool isContractor,  bool dnu,  String? punchCardId)?  $default,) {final _that = this;
switch (_that) {
case _WorkerProfileModel() when $default != null:
return $default(_that.id,_that.numberId,_that.profileImage,_that.firstName,_that.middleName,_that.lastName,_that.secondLastName,_that.birthDay,_that.gender,_that.socialInsurance,_that.socialInsuranceExpire,_that.dueDate,_that.socialInsuranceFile,_that.identificationNumber1,_that.identificationNumber2,_that.havePoliceCheckBackground,_that.identificationType1File,_that.identificationType2File,_that.identificationType1,_that.identificationType2,_that.policeCheckBackGround,_that.mobileNumber,_that.phone,_that.phoneExt,_that.location,_that.hasVehicle,_that.licenses,_that.certificates,_that.otherDocuments,_that.availabilities,_that.availabilityTimes,_that.availabilityDays,_that.locationPreferences,_that.lift,_that.languages,_that.skills,_that.resume,_that.haveAnyHealthProblem,_that.healthProblem,_that.otherHealthProblem,_that.contactEmergencyName,_that.contactEmergencyLastName,_that.contactEmergencyPhone,_that.email,_that.approvedToWork,_that.workerId,_that.isSubcontractor,_that.isContractor,_that.dnu,_that.punchCardId);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _WorkerProfileModel extends WorkerProfileModel {
  const _WorkerProfileModel({required this.id, this.numberId, this.profileImage, this.firstName, this.middleName, this.lastName, this.secondLastName, this.birthDay, this.gender, this.socialInsurance, this.socialInsuranceExpire = false, this.dueDate, this.socialInsuranceFile, this.identificationNumber1, this.identificationNumber2, this.havePoliceCheckBackground = false, this.identificationType1File, this.identificationType2File, this.identificationType1, this.identificationType2, this.policeCheckBackGround, this.mobileNumber, this.phone, this.phoneExt, this.location, this.hasVehicle = false, final  List<LicenseItemModel> licenses = const [], final  List<ProfileImageModel> certificates = const [], final  List<CatalogItemModel> otherDocuments = const [], final  List<CatalogItemModel> availabilities = const [], final  List<CatalogItemModel> availabilityTimes = const [], final  List<CatalogItemModel> availabilityDays = const [], final  List<CatalogItemModel> locationPreferences = const [], this.lift, final  List<CatalogItemModel> languages = const [], final  List<SkillItemModel> skills = const [], this.resume, this.haveAnyHealthProblem = false, this.healthProblem, this.otherHealthProblem, this.contactEmergencyName, this.contactEmergencyLastName, this.contactEmergencyPhone, this.email, this.approvedToWork = false, this.workerId, this.isSubcontractor = false, this.isContractor = false, this.dnu = false, this.punchCardId}): _licenses = licenses,_certificates = certificates,_otherDocuments = otherDocuments,_availabilities = availabilities,_availabilityTimes = availabilityTimes,_availabilityDays = availabilityDays,_locationPreferences = locationPreferences,_languages = languages,_skills = skills,super._();
  factory _WorkerProfileModel.fromJson(Map<String, dynamic> json) => _$WorkerProfileModelFromJson(json);

@override final  String id;
@override final  int? numberId;
@override final  ProfileImageModel? profileImage;
@override final  String? firstName;
@override final  String? middleName;
@override final  String? lastName;
@override final  String? secondLastName;
@override final  String? birthDay;
@override final  CatalogItemModel? gender;
@override final  String? socialInsurance;
@override@JsonKey() final  bool socialInsuranceExpire;
@override final  String? dueDate;
@override final  ProfileImageModel? socialInsuranceFile;
@override final  String? identificationNumber1;
@override final  String? identificationNumber2;
@override@JsonKey() final  bool havePoliceCheckBackground;
@override final  ProfileImageModel? identificationType1File;
@override final  ProfileImageModel? identificationType2File;
@override final  CatalogItemModel? identificationType1;
@override final  CatalogItemModel? identificationType2;
@override final  ProfileImageModel? policeCheckBackGround;
@override final  String? mobileNumber;
@override final  String? phone;
@override final  String? phoneExt;
@override final  LocationModel? location;
@override@JsonKey() final  bool hasVehicle;
 final  List<LicenseItemModel> _licenses;
@override@JsonKey() List<LicenseItemModel> get licenses {
  if (_licenses is EqualUnmodifiableListView) return _licenses;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_licenses);
}

 final  List<ProfileImageModel> _certificates;
@override@JsonKey() List<ProfileImageModel> get certificates {
  if (_certificates is EqualUnmodifiableListView) return _certificates;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_certificates);
}

 final  List<CatalogItemModel> _otherDocuments;
@override@JsonKey() List<CatalogItemModel> get otherDocuments {
  if (_otherDocuments is EqualUnmodifiableListView) return _otherDocuments;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_otherDocuments);
}

 final  List<CatalogItemModel> _availabilities;
@override@JsonKey() List<CatalogItemModel> get availabilities {
  if (_availabilities is EqualUnmodifiableListView) return _availabilities;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availabilities);
}

 final  List<CatalogItemModel> _availabilityTimes;
@override@JsonKey() List<CatalogItemModel> get availabilityTimes {
  if (_availabilityTimes is EqualUnmodifiableListView) return _availabilityTimes;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availabilityTimes);
}

 final  List<CatalogItemModel> _availabilityDays;
@override@JsonKey() List<CatalogItemModel> get availabilityDays {
  if (_availabilityDays is EqualUnmodifiableListView) return _availabilityDays;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_availabilityDays);
}

 final  List<CatalogItemModel> _locationPreferences;
@override@JsonKey() List<CatalogItemModel> get locationPreferences {
  if (_locationPreferences is EqualUnmodifiableListView) return _locationPreferences;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_locationPreferences);
}

@override final  CatalogItemModel? lift;
 final  List<CatalogItemModel> _languages;
@override@JsonKey() List<CatalogItemModel> get languages {
  if (_languages is EqualUnmodifiableListView) return _languages;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_languages);
}

 final  List<SkillItemModel> _skills;
@override@JsonKey() List<SkillItemModel> get skills {
  if (_skills is EqualUnmodifiableListView) return _skills;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_skills);
}

@override final  ProfileImageModel? resume;
@override@JsonKey() final  bool haveAnyHealthProblem;
@override final  String? healthProblem;
@override final  String? otherHealthProblem;
@override final  String? contactEmergencyName;
@override final  String? contactEmergencyLastName;
@override final  String? contactEmergencyPhone;
@override final  String? email;
@override@JsonKey() final  bool approvedToWork;
@override final  String? workerId;
@override@JsonKey() final  bool isSubcontractor;
@override@JsonKey() final  bool isContractor;
@override@JsonKey() final  bool dnu;
@override final  String? punchCardId;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$WorkerProfileModelCopyWith<_WorkerProfileModel> get copyWith => __$WorkerProfileModelCopyWithImpl<_WorkerProfileModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$WorkerProfileModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _WorkerProfileModel&&(identical(other.id, id) || other.id == id)&&(identical(other.numberId, numberId) || other.numberId == numberId)&&(identical(other.profileImage, profileImage) || other.profileImage == profileImage)&&(identical(other.firstName, firstName) || other.firstName == firstName)&&(identical(other.middleName, middleName) || other.middleName == middleName)&&(identical(other.lastName, lastName) || other.lastName == lastName)&&(identical(other.secondLastName, secondLastName) || other.secondLastName == secondLastName)&&(identical(other.birthDay, birthDay) || other.birthDay == birthDay)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.socialInsurance, socialInsurance) || other.socialInsurance == socialInsurance)&&(identical(other.socialInsuranceExpire, socialInsuranceExpire) || other.socialInsuranceExpire == socialInsuranceExpire)&&(identical(other.dueDate, dueDate) || other.dueDate == dueDate)&&(identical(other.socialInsuranceFile, socialInsuranceFile) || other.socialInsuranceFile == socialInsuranceFile)&&(identical(other.identificationNumber1, identificationNumber1) || other.identificationNumber1 == identificationNumber1)&&(identical(other.identificationNumber2, identificationNumber2) || other.identificationNumber2 == identificationNumber2)&&(identical(other.havePoliceCheckBackground, havePoliceCheckBackground) || other.havePoliceCheckBackground == havePoliceCheckBackground)&&(identical(other.identificationType1File, identificationType1File) || other.identificationType1File == identificationType1File)&&(identical(other.identificationType2File, identificationType2File) || other.identificationType2File == identificationType2File)&&(identical(other.identificationType1, identificationType1) || other.identificationType1 == identificationType1)&&(identical(other.identificationType2, identificationType2) || other.identificationType2 == identificationType2)&&(identical(other.policeCheckBackGround, policeCheckBackGround) || other.policeCheckBackGround == policeCheckBackGround)&&(identical(other.mobileNumber, mobileNumber) || other.mobileNumber == mobileNumber)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.phoneExt, phoneExt) || other.phoneExt == phoneExt)&&(identical(other.location, location) || other.location == location)&&(identical(other.hasVehicle, hasVehicle) || other.hasVehicle == hasVehicle)&&const DeepCollectionEquality().equals(other._licenses, _licenses)&&const DeepCollectionEquality().equals(other._certificates, _certificates)&&const DeepCollectionEquality().equals(other._otherDocuments, _otherDocuments)&&const DeepCollectionEquality().equals(other._availabilities, _availabilities)&&const DeepCollectionEquality().equals(other._availabilityTimes, _availabilityTimes)&&const DeepCollectionEquality().equals(other._availabilityDays, _availabilityDays)&&const DeepCollectionEquality().equals(other._locationPreferences, _locationPreferences)&&(identical(other.lift, lift) || other.lift == lift)&&const DeepCollectionEquality().equals(other._languages, _languages)&&const DeepCollectionEquality().equals(other._skills, _skills)&&(identical(other.resume, resume) || other.resume == resume)&&(identical(other.haveAnyHealthProblem, haveAnyHealthProblem) || other.haveAnyHealthProblem == haveAnyHealthProblem)&&(identical(other.healthProblem, healthProblem) || other.healthProblem == healthProblem)&&(identical(other.otherHealthProblem, otherHealthProblem) || other.otherHealthProblem == otherHealthProblem)&&(identical(other.contactEmergencyName, contactEmergencyName) || other.contactEmergencyName == contactEmergencyName)&&(identical(other.contactEmergencyLastName, contactEmergencyLastName) || other.contactEmergencyLastName == contactEmergencyLastName)&&(identical(other.contactEmergencyPhone, contactEmergencyPhone) || other.contactEmergencyPhone == contactEmergencyPhone)&&(identical(other.email, email) || other.email == email)&&(identical(other.approvedToWork, approvedToWork) || other.approvedToWork == approvedToWork)&&(identical(other.workerId, workerId) || other.workerId == workerId)&&(identical(other.isSubcontractor, isSubcontractor) || other.isSubcontractor == isSubcontractor)&&(identical(other.isContractor, isContractor) || other.isContractor == isContractor)&&(identical(other.dnu, dnu) || other.dnu == dnu)&&(identical(other.punchCardId, punchCardId) || other.punchCardId == punchCardId));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,numberId,profileImage,firstName,middleName,lastName,secondLastName,birthDay,gender,socialInsurance,socialInsuranceExpire,dueDate,socialInsuranceFile,identificationNumber1,identificationNumber2,havePoliceCheckBackground,identificationType1File,identificationType2File,identificationType1,identificationType2,policeCheckBackGround,mobileNumber,phone,phoneExt,location,hasVehicle,const DeepCollectionEquality().hash(_licenses),const DeepCollectionEquality().hash(_certificates),const DeepCollectionEquality().hash(_otherDocuments),const DeepCollectionEquality().hash(_availabilities),const DeepCollectionEquality().hash(_availabilityTimes),const DeepCollectionEquality().hash(_availabilityDays),const DeepCollectionEquality().hash(_locationPreferences),lift,const DeepCollectionEquality().hash(_languages),const DeepCollectionEquality().hash(_skills),resume,haveAnyHealthProblem,healthProblem,otherHealthProblem,contactEmergencyName,contactEmergencyLastName,contactEmergencyPhone,email,approvedToWork,workerId,isSubcontractor,isContractor,dnu,punchCardId]);

@override
String toString() {
  return 'WorkerProfileModel(id: $id, numberId: $numberId, profileImage: $profileImage, firstName: $firstName, middleName: $middleName, lastName: $lastName, secondLastName: $secondLastName, birthDay: $birthDay, gender: $gender, socialInsurance: $socialInsurance, socialInsuranceExpire: $socialInsuranceExpire, dueDate: $dueDate, socialInsuranceFile: $socialInsuranceFile, identificationNumber1: $identificationNumber1, identificationNumber2: $identificationNumber2, havePoliceCheckBackground: $havePoliceCheckBackground, identificationType1File: $identificationType1File, identificationType2File: $identificationType2File, identificationType1: $identificationType1, identificationType2: $identificationType2, policeCheckBackGround: $policeCheckBackGround, mobileNumber: $mobileNumber, phone: $phone, phoneExt: $phoneExt, location: $location, hasVehicle: $hasVehicle, licenses: $licenses, certificates: $certificates, otherDocuments: $otherDocuments, availabilities: $availabilities, availabilityTimes: $availabilityTimes, availabilityDays: $availabilityDays, locationPreferences: $locationPreferences, lift: $lift, languages: $languages, skills: $skills, resume: $resume, haveAnyHealthProblem: $haveAnyHealthProblem, healthProblem: $healthProblem, otherHealthProblem: $otherHealthProblem, contactEmergencyName: $contactEmergencyName, contactEmergencyLastName: $contactEmergencyLastName, contactEmergencyPhone: $contactEmergencyPhone, email: $email, approvedToWork: $approvedToWork, workerId: $workerId, isSubcontractor: $isSubcontractor, isContractor: $isContractor, dnu: $dnu, punchCardId: $punchCardId)';
}


}

/// @nodoc
abstract mixin class _$WorkerProfileModelCopyWith<$Res> implements $WorkerProfileModelCopyWith<$Res> {
  factory _$WorkerProfileModelCopyWith(_WorkerProfileModel value, $Res Function(_WorkerProfileModel) _then) = __$WorkerProfileModelCopyWithImpl;
@override @useResult
$Res call({
 String id, int? numberId, ProfileImageModel? profileImage, String? firstName, String? middleName, String? lastName, String? secondLastName, String? birthDay, CatalogItemModel? gender, String? socialInsurance, bool socialInsuranceExpire, String? dueDate, ProfileImageModel? socialInsuranceFile, String? identificationNumber1, String? identificationNumber2, bool havePoliceCheckBackground, ProfileImageModel? identificationType1File, ProfileImageModel? identificationType2File, CatalogItemModel? identificationType1, CatalogItemModel? identificationType2, ProfileImageModel? policeCheckBackGround, String? mobileNumber, String? phone, String? phoneExt, LocationModel? location, bool hasVehicle, List<LicenseItemModel> licenses, List<ProfileImageModel> certificates, List<CatalogItemModel> otherDocuments, List<CatalogItemModel> availabilities, List<CatalogItemModel> availabilityTimes, List<CatalogItemModel> availabilityDays, List<CatalogItemModel> locationPreferences, CatalogItemModel? lift, List<CatalogItemModel> languages, List<SkillItemModel> skills, ProfileImageModel? resume, bool haveAnyHealthProblem, String? healthProblem, String? otherHealthProblem, String? contactEmergencyName, String? contactEmergencyLastName, String? contactEmergencyPhone, String? email, bool approvedToWork, String? workerId, bool isSubcontractor, bool isContractor, bool dnu, String? punchCardId
});


@override $ProfileImageModelCopyWith<$Res>? get profileImage;@override $CatalogItemModelCopyWith<$Res>? get gender;@override $ProfileImageModelCopyWith<$Res>? get socialInsuranceFile;@override $ProfileImageModelCopyWith<$Res>? get identificationType1File;@override $ProfileImageModelCopyWith<$Res>? get identificationType2File;@override $CatalogItemModelCopyWith<$Res>? get identificationType1;@override $CatalogItemModelCopyWith<$Res>? get identificationType2;@override $ProfileImageModelCopyWith<$Res>? get policeCheckBackGround;@override $LocationModelCopyWith<$Res>? get location;@override $CatalogItemModelCopyWith<$Res>? get lift;@override $ProfileImageModelCopyWith<$Res>? get resume;

}
/// @nodoc
class __$WorkerProfileModelCopyWithImpl<$Res>
    implements _$WorkerProfileModelCopyWith<$Res> {
  __$WorkerProfileModelCopyWithImpl(this._self, this._then);

  final _WorkerProfileModel _self;
  final $Res Function(_WorkerProfileModel) _then;

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? numberId = freezed,Object? profileImage = freezed,Object? firstName = freezed,Object? middleName = freezed,Object? lastName = freezed,Object? secondLastName = freezed,Object? birthDay = freezed,Object? gender = freezed,Object? socialInsurance = freezed,Object? socialInsuranceExpire = null,Object? dueDate = freezed,Object? socialInsuranceFile = freezed,Object? identificationNumber1 = freezed,Object? identificationNumber2 = freezed,Object? havePoliceCheckBackground = null,Object? identificationType1File = freezed,Object? identificationType2File = freezed,Object? identificationType1 = freezed,Object? identificationType2 = freezed,Object? policeCheckBackGround = freezed,Object? mobileNumber = freezed,Object? phone = freezed,Object? phoneExt = freezed,Object? location = freezed,Object? hasVehicle = null,Object? licenses = null,Object? certificates = null,Object? otherDocuments = null,Object? availabilities = null,Object? availabilityTimes = null,Object? availabilityDays = null,Object? locationPreferences = null,Object? lift = freezed,Object? languages = null,Object? skills = null,Object? resume = freezed,Object? haveAnyHealthProblem = null,Object? healthProblem = freezed,Object? otherHealthProblem = freezed,Object? contactEmergencyName = freezed,Object? contactEmergencyLastName = freezed,Object? contactEmergencyPhone = freezed,Object? email = freezed,Object? approvedToWork = null,Object? workerId = freezed,Object? isSubcontractor = null,Object? isContractor = null,Object? dnu = null,Object? punchCardId = freezed,}) {
  return _then(_WorkerProfileModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,numberId: freezed == numberId ? _self.numberId : numberId // ignore: cast_nullable_to_non_nullable
as int?,profileImage: freezed == profileImage ? _self.profileImage : profileImage // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,firstName: freezed == firstName ? _self.firstName : firstName // ignore: cast_nullable_to_non_nullable
as String?,middleName: freezed == middleName ? _self.middleName : middleName // ignore: cast_nullable_to_non_nullable
as String?,lastName: freezed == lastName ? _self.lastName : lastName // ignore: cast_nullable_to_non_nullable
as String?,secondLastName: freezed == secondLastName ? _self.secondLastName : secondLastName // ignore: cast_nullable_to_non_nullable
as String?,birthDay: freezed == birthDay ? _self.birthDay : birthDay // ignore: cast_nullable_to_non_nullable
as String?,gender: freezed == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,socialInsurance: freezed == socialInsurance ? _self.socialInsurance : socialInsurance // ignore: cast_nullable_to_non_nullable
as String?,socialInsuranceExpire: null == socialInsuranceExpire ? _self.socialInsuranceExpire : socialInsuranceExpire // ignore: cast_nullable_to_non_nullable
as bool,dueDate: freezed == dueDate ? _self.dueDate : dueDate // ignore: cast_nullable_to_non_nullable
as String?,socialInsuranceFile: freezed == socialInsuranceFile ? _self.socialInsuranceFile : socialInsuranceFile // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationNumber1: freezed == identificationNumber1 ? _self.identificationNumber1 : identificationNumber1 // ignore: cast_nullable_to_non_nullable
as String?,identificationNumber2: freezed == identificationNumber2 ? _self.identificationNumber2 : identificationNumber2 // ignore: cast_nullable_to_non_nullable
as String?,havePoliceCheckBackground: null == havePoliceCheckBackground ? _self.havePoliceCheckBackground : havePoliceCheckBackground // ignore: cast_nullable_to_non_nullable
as bool,identificationType1File: freezed == identificationType1File ? _self.identificationType1File : identificationType1File // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationType2File: freezed == identificationType2File ? _self.identificationType2File : identificationType2File // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,identificationType1: freezed == identificationType1 ? _self.identificationType1 : identificationType1 // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,identificationType2: freezed == identificationType2 ? _self.identificationType2 : identificationType2 // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,policeCheckBackGround: freezed == policeCheckBackGround ? _self.policeCheckBackGround : policeCheckBackGround // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,mobileNumber: freezed == mobileNumber ? _self.mobileNumber : mobileNumber // ignore: cast_nullable_to_non_nullable
as String?,phone: freezed == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String?,phoneExt: freezed == phoneExt ? _self.phoneExt : phoneExt // ignore: cast_nullable_to_non_nullable
as String?,location: freezed == location ? _self.location : location // ignore: cast_nullable_to_non_nullable
as LocationModel?,hasVehicle: null == hasVehicle ? _self.hasVehicle : hasVehicle // ignore: cast_nullable_to_non_nullable
as bool,licenses: null == licenses ? _self._licenses : licenses // ignore: cast_nullable_to_non_nullable
as List<LicenseItemModel>,certificates: null == certificates ? _self._certificates : certificates // ignore: cast_nullable_to_non_nullable
as List<ProfileImageModel>,otherDocuments: null == otherDocuments ? _self._otherDocuments : otherDocuments // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilities: null == availabilities ? _self._availabilities : availabilities // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilityTimes: null == availabilityTimes ? _self._availabilityTimes : availabilityTimes // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,availabilityDays: null == availabilityDays ? _self._availabilityDays : availabilityDays // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,locationPreferences: null == locationPreferences ? _self._locationPreferences : locationPreferences // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,lift: freezed == lift ? _self.lift : lift // ignore: cast_nullable_to_non_nullable
as CatalogItemModel?,languages: null == languages ? _self._languages : languages // ignore: cast_nullable_to_non_nullable
as List<CatalogItemModel>,skills: null == skills ? _self._skills : skills // ignore: cast_nullable_to_non_nullable
as List<SkillItemModel>,resume: freezed == resume ? _self.resume : resume // ignore: cast_nullable_to_non_nullable
as ProfileImageModel?,haveAnyHealthProblem: null == haveAnyHealthProblem ? _self.haveAnyHealthProblem : haveAnyHealthProblem // ignore: cast_nullable_to_non_nullable
as bool,healthProblem: freezed == healthProblem ? _self.healthProblem : healthProblem // ignore: cast_nullable_to_non_nullable
as String?,otherHealthProblem: freezed == otherHealthProblem ? _self.otherHealthProblem : otherHealthProblem // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyName: freezed == contactEmergencyName ? _self.contactEmergencyName : contactEmergencyName // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyLastName: freezed == contactEmergencyLastName ? _self.contactEmergencyLastName : contactEmergencyLastName // ignore: cast_nullable_to_non_nullable
as String?,contactEmergencyPhone: freezed == contactEmergencyPhone ? _self.contactEmergencyPhone : contactEmergencyPhone // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,approvedToWork: null == approvedToWork ? _self.approvedToWork : approvedToWork // ignore: cast_nullable_to_non_nullable
as bool,workerId: freezed == workerId ? _self.workerId : workerId // ignore: cast_nullable_to_non_nullable
as String?,isSubcontractor: null == isSubcontractor ? _self.isSubcontractor : isSubcontractor // ignore: cast_nullable_to_non_nullable
as bool,isContractor: null == isContractor ? _self.isContractor : isContractor // ignore: cast_nullable_to_non_nullable
as bool,dnu: null == dnu ? _self.dnu : dnu // ignore: cast_nullable_to_non_nullable
as bool,punchCardId: freezed == punchCardId ? _self.punchCardId : punchCardId // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get profileImage {
    if (_self.profileImage == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.profileImage!, (value) {
    return _then(_self.copyWith(profileImage: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get gender {
    if (_self.gender == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.gender!, (value) {
    return _then(_self.copyWith(gender: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get socialInsuranceFile {
    if (_self.socialInsuranceFile == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.socialInsuranceFile!, (value) {
    return _then(_self.copyWith(socialInsuranceFile: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get identificationType1File {
    if (_self.identificationType1File == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.identificationType1File!, (value) {
    return _then(_self.copyWith(identificationType1File: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get identificationType2File {
    if (_self.identificationType2File == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.identificationType2File!, (value) {
    return _then(_self.copyWith(identificationType2File: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get identificationType1 {
    if (_self.identificationType1 == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.identificationType1!, (value) {
    return _then(_self.copyWith(identificationType1: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get identificationType2 {
    if (_self.identificationType2 == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.identificationType2!, (value) {
    return _then(_self.copyWith(identificationType2: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get policeCheckBackGround {
    if (_self.policeCheckBackGround == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.policeCheckBackGround!, (value) {
    return _then(_self.copyWith(policeCheckBackGround: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$LocationModelCopyWith<$Res>? get location {
    if (_self.location == null) {
    return null;
  }

  return $LocationModelCopyWith<$Res>(_self.location!, (value) {
    return _then(_self.copyWith(location: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$CatalogItemModelCopyWith<$Res>? get lift {
    if (_self.lift == null) {
    return null;
  }

  return $CatalogItemModelCopyWith<$Res>(_self.lift!, (value) {
    return _then(_self.copyWith(lift: value));
  });
}/// Create a copy of WorkerProfileModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ProfileImageModelCopyWith<$Res>? get resume {
    if (_self.resume == null) {
    return null;
  }

  return $ProfileImageModelCopyWith<$Res>(_self.resume!, (value) {
    return _then(_self.copyWith(resume: value));
  });
}
}

// dart format on
