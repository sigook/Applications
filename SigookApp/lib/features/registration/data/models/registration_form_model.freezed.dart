// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'registration_form_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

RegistrationFormModel _$RegistrationFormModelFromJson(
  Map<String, dynamic> json,
) {
  return _RegistrationFormModel.fromJson(json);
}

/// @nodoc
mixin _$RegistrationFormModel {
  BasicInfoModel? get basicInfo => throw _privateConstructorUsedError;
  PreferencesInfoModel? get preferencesInfo =>
      throw _privateConstructorUsedError;
  DocumentsInfoModel? get documentsInfo => throw _privateConstructorUsedError;
  AccountInfoModel? get accountInfo => throw _privateConstructorUsedError;

  /// Serializes this RegistrationFormModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $RegistrationFormModelCopyWith<RegistrationFormModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $RegistrationFormModelCopyWith<$Res> {
  factory $RegistrationFormModelCopyWith(
    RegistrationFormModel value,
    $Res Function(RegistrationFormModel) then,
  ) = _$RegistrationFormModelCopyWithImpl<$Res, RegistrationFormModel>;
  @useResult
  $Res call({
    BasicInfoModel? basicInfo,
    PreferencesInfoModel? preferencesInfo,
    DocumentsInfoModel? documentsInfo,
    AccountInfoModel? accountInfo,
  });

  $BasicInfoModelCopyWith<$Res>? get basicInfo;
  $PreferencesInfoModelCopyWith<$Res>? get preferencesInfo;
  $AccountInfoModelCopyWith<$Res>? get accountInfo;
}

/// @nodoc
class _$RegistrationFormModelCopyWithImpl<
  $Res,
  $Val extends RegistrationFormModel
>
    implements $RegistrationFormModelCopyWith<$Res> {
  _$RegistrationFormModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? basicInfo = freezed,
    Object? preferencesInfo = freezed,
    Object? documentsInfo = freezed,
    Object? accountInfo = freezed,
  }) {
    return _then(
      _value.copyWith(
            basicInfo: freezed == basicInfo
                ? _value.basicInfo
                : basicInfo // ignore: cast_nullable_to_non_nullable
                      as BasicInfoModel?,
            preferencesInfo: freezed == preferencesInfo
                ? _value.preferencesInfo
                : preferencesInfo // ignore: cast_nullable_to_non_nullable
                      as PreferencesInfoModel?,
            documentsInfo: freezed == documentsInfo
                ? _value.documentsInfo
                : documentsInfo // ignore: cast_nullable_to_non_nullable
                      as DocumentsInfoModel?,
            accountInfo: freezed == accountInfo
                ? _value.accountInfo
                : accountInfo // ignore: cast_nullable_to_non_nullable
                      as AccountInfoModel?,
          )
          as $Val,
    );
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $BasicInfoModelCopyWith<$Res>? get basicInfo {
    if (_value.basicInfo == null) {
      return null;
    }

    return $BasicInfoModelCopyWith<$Res>(_value.basicInfo!, (value) {
      return _then(_value.copyWith(basicInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $PreferencesInfoModelCopyWith<$Res>? get preferencesInfo {
    if (_value.preferencesInfo == null) {
      return null;
    }

    return $PreferencesInfoModelCopyWith<$Res>(_value.preferencesInfo!, (
      value,
    ) {
      return _then(_value.copyWith(preferencesInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $AccountInfoModelCopyWith<$Res>? get accountInfo {
    if (_value.accountInfo == null) {
      return null;
    }

    return $AccountInfoModelCopyWith<$Res>(_value.accountInfo!, (value) {
      return _then(_value.copyWith(accountInfo: value) as $Val);
    });
  }
}

/// @nodoc
abstract class _$$RegistrationFormModelImplCopyWith<$Res>
    implements $RegistrationFormModelCopyWith<$Res> {
  factory _$$RegistrationFormModelImplCopyWith(
    _$RegistrationFormModelImpl value,
    $Res Function(_$RegistrationFormModelImpl) then,
  ) = __$$RegistrationFormModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    BasicInfoModel? basicInfo,
    PreferencesInfoModel? preferencesInfo,
    DocumentsInfoModel? documentsInfo,
    AccountInfoModel? accountInfo,
  });

  @override
  $BasicInfoModelCopyWith<$Res>? get basicInfo;
  @override
  $PreferencesInfoModelCopyWith<$Res>? get preferencesInfo;
  @override
  $AccountInfoModelCopyWith<$Res>? get accountInfo;
}

/// @nodoc
class __$$RegistrationFormModelImplCopyWithImpl<$Res>
    extends
        _$RegistrationFormModelCopyWithImpl<$Res, _$RegistrationFormModelImpl>
    implements _$$RegistrationFormModelImplCopyWith<$Res> {
  __$$RegistrationFormModelImplCopyWithImpl(
    _$RegistrationFormModelImpl _value,
    $Res Function(_$RegistrationFormModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? basicInfo = freezed,
    Object? preferencesInfo = freezed,
    Object? documentsInfo = freezed,
    Object? accountInfo = freezed,
  }) {
    return _then(
      _$RegistrationFormModelImpl(
        basicInfo: freezed == basicInfo
            ? _value.basicInfo
            : basicInfo // ignore: cast_nullable_to_non_nullable
                  as BasicInfoModel?,
        preferencesInfo: freezed == preferencesInfo
            ? _value.preferencesInfo
            : preferencesInfo // ignore: cast_nullable_to_non_nullable
                  as PreferencesInfoModel?,
        documentsInfo: freezed == documentsInfo
            ? _value.documentsInfo
            : documentsInfo // ignore: cast_nullable_to_non_nullable
                  as DocumentsInfoModel?,
        accountInfo: freezed == accountInfo
            ? _value.accountInfo
            : accountInfo // ignore: cast_nullable_to_non_nullable
                  as AccountInfoModel?,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$RegistrationFormModelImpl extends _RegistrationFormModel {
  const _$RegistrationFormModelImpl({
    this.basicInfo,
    this.preferencesInfo,
    this.documentsInfo,
    this.accountInfo,
  }) : super._();

  factory _$RegistrationFormModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$RegistrationFormModelImplFromJson(json);

  @override
  final BasicInfoModel? basicInfo;
  @override
  final PreferencesInfoModel? preferencesInfo;
  @override
  final DocumentsInfoModel? documentsInfo;
  @override
  final AccountInfoModel? accountInfo;

  @override
  String toString() {
    return 'RegistrationFormModel(basicInfo: $basicInfo, preferencesInfo: $preferencesInfo, documentsInfo: $documentsInfo, accountInfo: $accountInfo)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$RegistrationFormModelImpl &&
            (identical(other.basicInfo, basicInfo) ||
                other.basicInfo == basicInfo) &&
            (identical(other.preferencesInfo, preferencesInfo) ||
                other.preferencesInfo == preferencesInfo) &&
            (identical(other.documentsInfo, documentsInfo) ||
                other.documentsInfo == documentsInfo) &&
            (identical(other.accountInfo, accountInfo) ||
                other.accountInfo == accountInfo));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    basicInfo,
    preferencesInfo,
    documentsInfo,
    accountInfo,
  );

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$RegistrationFormModelImplCopyWith<_$RegistrationFormModelImpl>
  get copyWith =>
      __$$RegistrationFormModelImplCopyWithImpl<_$RegistrationFormModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$RegistrationFormModelImplToJson(this);
  }
}

abstract class _RegistrationFormModel extends RegistrationFormModel {
  const factory _RegistrationFormModel({
    final BasicInfoModel? basicInfo,
    final PreferencesInfoModel? preferencesInfo,
    final DocumentsInfoModel? documentsInfo,
    final AccountInfoModel? accountInfo,
  }) = _$RegistrationFormModelImpl;
  const _RegistrationFormModel._() : super._();

  factory _RegistrationFormModel.fromJson(Map<String, dynamic> json) =
      _$RegistrationFormModelImpl.fromJson;

  @override
  BasicInfoModel? get basicInfo;
  @override
  PreferencesInfoModel? get preferencesInfo;
  @override
  DocumentsInfoModel? get documentsInfo;
  @override
  AccountInfoModel? get accountInfo;

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$RegistrationFormModelImplCopyWith<_$RegistrationFormModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
