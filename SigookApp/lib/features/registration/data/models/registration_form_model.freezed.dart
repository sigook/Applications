// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'registration_form_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$RegistrationFormModel {

 BasicInfoModel? get basicInfo; PreferencesInfoModel? get preferencesInfo; DocumentsInfoModel? get documentsInfo; AccountInfoModel? get accountInfo;
/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$RegistrationFormModelCopyWith<RegistrationFormModel> get copyWith => _$RegistrationFormModelCopyWithImpl<RegistrationFormModel>(this as RegistrationFormModel, _$identity);

  /// Serializes this RegistrationFormModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is RegistrationFormModel&&(identical(other.basicInfo, basicInfo) || other.basicInfo == basicInfo)&&(identical(other.preferencesInfo, preferencesInfo) || other.preferencesInfo == preferencesInfo)&&(identical(other.documentsInfo, documentsInfo) || other.documentsInfo == documentsInfo)&&(identical(other.accountInfo, accountInfo) || other.accountInfo == accountInfo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,basicInfo,preferencesInfo,documentsInfo,accountInfo);

@override
String toString() {
  return 'RegistrationFormModel(basicInfo: $basicInfo, preferencesInfo: $preferencesInfo, documentsInfo: $documentsInfo, accountInfo: $accountInfo)';
}


}

/// @nodoc
abstract mixin class $RegistrationFormModelCopyWith<$Res>  {
  factory $RegistrationFormModelCopyWith(RegistrationFormModel value, $Res Function(RegistrationFormModel) _then) = _$RegistrationFormModelCopyWithImpl;
@useResult
$Res call({
 BasicInfoModel? basicInfo, PreferencesInfoModel? preferencesInfo, DocumentsInfoModel? documentsInfo, AccountInfoModel? accountInfo
});


$BasicInfoModelCopyWith<$Res>? get basicInfo;$PreferencesInfoModelCopyWith<$Res>? get preferencesInfo;$AccountInfoModelCopyWith<$Res>? get accountInfo;

}
/// @nodoc
class _$RegistrationFormModelCopyWithImpl<$Res>
    implements $RegistrationFormModelCopyWith<$Res> {
  _$RegistrationFormModelCopyWithImpl(this._self, this._then);

  final RegistrationFormModel _self;
  final $Res Function(RegistrationFormModel) _then;

/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? basicInfo = freezed,Object? preferencesInfo = freezed,Object? documentsInfo = freezed,Object? accountInfo = freezed,}) {
  return _then(_self.copyWith(
basicInfo: freezed == basicInfo ? _self.basicInfo : basicInfo // ignore: cast_nullable_to_non_nullable
as BasicInfoModel?,preferencesInfo: freezed == preferencesInfo ? _self.preferencesInfo : preferencesInfo // ignore: cast_nullable_to_non_nullable
as PreferencesInfoModel?,documentsInfo: freezed == documentsInfo ? _self.documentsInfo : documentsInfo // ignore: cast_nullable_to_non_nullable
as DocumentsInfoModel?,accountInfo: freezed == accountInfo ? _self.accountInfo : accountInfo // ignore: cast_nullable_to_non_nullable
as AccountInfoModel?,
  ));
}
/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$BasicInfoModelCopyWith<$Res>? get basicInfo {
    if (_self.basicInfo == null) {
    return null;
  }

  return $BasicInfoModelCopyWith<$Res>(_self.basicInfo!, (value) {
    return _then(_self.copyWith(basicInfo: value));
  });
}/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$PreferencesInfoModelCopyWith<$Res>? get preferencesInfo {
    if (_self.preferencesInfo == null) {
    return null;
  }

  return $PreferencesInfoModelCopyWith<$Res>(_self.preferencesInfo!, (value) {
    return _then(_self.copyWith(preferencesInfo: value));
  });
}/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$AccountInfoModelCopyWith<$Res>? get accountInfo {
    if (_self.accountInfo == null) {
    return null;
  }

  return $AccountInfoModelCopyWith<$Res>(_self.accountInfo!, (value) {
    return _then(_self.copyWith(accountInfo: value));
  });
}
}


/// Adds pattern-matching-related methods to [RegistrationFormModel].
extension RegistrationFormModelPatterns on RegistrationFormModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _RegistrationFormModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _RegistrationFormModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _RegistrationFormModel value)  $default,){
final _that = this;
switch (_that) {
case _RegistrationFormModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _RegistrationFormModel value)?  $default,){
final _that = this;
switch (_that) {
case _RegistrationFormModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( BasicInfoModel? basicInfo,  PreferencesInfoModel? preferencesInfo,  DocumentsInfoModel? documentsInfo,  AccountInfoModel? accountInfo)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _RegistrationFormModel() when $default != null:
return $default(_that.basicInfo,_that.preferencesInfo,_that.documentsInfo,_that.accountInfo);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( BasicInfoModel? basicInfo,  PreferencesInfoModel? preferencesInfo,  DocumentsInfoModel? documentsInfo,  AccountInfoModel? accountInfo)  $default,) {final _that = this;
switch (_that) {
case _RegistrationFormModel():
return $default(_that.basicInfo,_that.preferencesInfo,_that.documentsInfo,_that.accountInfo);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( BasicInfoModel? basicInfo,  PreferencesInfoModel? preferencesInfo,  DocumentsInfoModel? documentsInfo,  AccountInfoModel? accountInfo)?  $default,) {final _that = this;
switch (_that) {
case _RegistrationFormModel() when $default != null:
return $default(_that.basicInfo,_that.preferencesInfo,_that.documentsInfo,_that.accountInfo);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _RegistrationFormModel extends RegistrationFormModel {
  const _RegistrationFormModel({this.basicInfo, this.preferencesInfo, this.documentsInfo, this.accountInfo}): super._();
  factory _RegistrationFormModel.fromJson(Map<String, dynamic> json) => _$RegistrationFormModelFromJson(json);

@override final  BasicInfoModel? basicInfo;
@override final  PreferencesInfoModel? preferencesInfo;
@override final  DocumentsInfoModel? documentsInfo;
@override final  AccountInfoModel? accountInfo;

/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$RegistrationFormModelCopyWith<_RegistrationFormModel> get copyWith => __$RegistrationFormModelCopyWithImpl<_RegistrationFormModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$RegistrationFormModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _RegistrationFormModel&&(identical(other.basicInfo, basicInfo) || other.basicInfo == basicInfo)&&(identical(other.preferencesInfo, preferencesInfo) || other.preferencesInfo == preferencesInfo)&&(identical(other.documentsInfo, documentsInfo) || other.documentsInfo == documentsInfo)&&(identical(other.accountInfo, accountInfo) || other.accountInfo == accountInfo));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,basicInfo,preferencesInfo,documentsInfo,accountInfo);

@override
String toString() {
  return 'RegistrationFormModel(basicInfo: $basicInfo, preferencesInfo: $preferencesInfo, documentsInfo: $documentsInfo, accountInfo: $accountInfo)';
}


}

/// @nodoc
abstract mixin class _$RegistrationFormModelCopyWith<$Res> implements $RegistrationFormModelCopyWith<$Res> {
  factory _$RegistrationFormModelCopyWith(_RegistrationFormModel value, $Res Function(_RegistrationFormModel) _then) = __$RegistrationFormModelCopyWithImpl;
@override @useResult
$Res call({
 BasicInfoModel? basicInfo, PreferencesInfoModel? preferencesInfo, DocumentsInfoModel? documentsInfo, AccountInfoModel? accountInfo
});


@override $BasicInfoModelCopyWith<$Res>? get basicInfo;@override $PreferencesInfoModelCopyWith<$Res>? get preferencesInfo;@override $AccountInfoModelCopyWith<$Res>? get accountInfo;

}
/// @nodoc
class __$RegistrationFormModelCopyWithImpl<$Res>
    implements _$RegistrationFormModelCopyWith<$Res> {
  __$RegistrationFormModelCopyWithImpl(this._self, this._then);

  final _RegistrationFormModel _self;
  final $Res Function(_RegistrationFormModel) _then;

/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? basicInfo = freezed,Object? preferencesInfo = freezed,Object? documentsInfo = freezed,Object? accountInfo = freezed,}) {
  return _then(_RegistrationFormModel(
basicInfo: freezed == basicInfo ? _self.basicInfo : basicInfo // ignore: cast_nullable_to_non_nullable
as BasicInfoModel?,preferencesInfo: freezed == preferencesInfo ? _self.preferencesInfo : preferencesInfo // ignore: cast_nullable_to_non_nullable
as PreferencesInfoModel?,documentsInfo: freezed == documentsInfo ? _self.documentsInfo : documentsInfo // ignore: cast_nullable_to_non_nullable
as DocumentsInfoModel?,accountInfo: freezed == accountInfo ? _self.accountInfo : accountInfo // ignore: cast_nullable_to_non_nullable
as AccountInfoModel?,
  ));
}

/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$BasicInfoModelCopyWith<$Res>? get basicInfo {
    if (_self.basicInfo == null) {
    return null;
  }

  return $BasicInfoModelCopyWith<$Res>(_self.basicInfo!, (value) {
    return _then(_self.copyWith(basicInfo: value));
  });
}/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$PreferencesInfoModelCopyWith<$Res>? get preferencesInfo {
    if (_self.preferencesInfo == null) {
    return null;
  }

  return $PreferencesInfoModelCopyWith<$Res>(_self.preferencesInfo!, (value) {
    return _then(_self.copyWith(preferencesInfo: value));
  });
}/// Create a copy of RegistrationFormModel
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$AccountInfoModelCopyWith<$Res>? get accountInfo {
    if (_self.accountInfo == null) {
    return null;
  }

  return $AccountInfoModelCopyWith<$Res>(_self.accountInfo!, (value) {
    return _then(_self.copyWith(accountInfo: value));
  });
}
}

// dart format on
