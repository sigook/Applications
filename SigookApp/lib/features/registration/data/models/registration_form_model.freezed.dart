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
  PersonalInfoModel? get personalInfo => throw _privateConstructorUsedError;
  ContactInfoModel? get contactInfo => throw _privateConstructorUsedError;
  AddressInfoModel? get addressInfo => throw _privateConstructorUsedError;
  AvailabilityInfoModel? get availabilityInfo =>
      throw _privateConstructorUsedError;
  ProfessionalInfoModel? get professionalInfo =>
      throw _privateConstructorUsedError;

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
    PersonalInfoModel? personalInfo,
    ContactInfoModel? contactInfo,
    AddressInfoModel? addressInfo,
    AvailabilityInfoModel? availabilityInfo,
    ProfessionalInfoModel? professionalInfo,
  });

  $PersonalInfoModelCopyWith<$Res>? get personalInfo;
  $ContactInfoModelCopyWith<$Res>? get contactInfo;
  $AddressInfoModelCopyWith<$Res>? get addressInfo;
  $AvailabilityInfoModelCopyWith<$Res>? get availabilityInfo;
  $ProfessionalInfoModelCopyWith<$Res>? get professionalInfo;
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
    Object? personalInfo = freezed,
    Object? contactInfo = freezed,
    Object? addressInfo = freezed,
    Object? availabilityInfo = freezed,
    Object? professionalInfo = freezed,
  }) {
    return _then(
      _value.copyWith(
            personalInfo: freezed == personalInfo
                ? _value.personalInfo
                : personalInfo // ignore: cast_nullable_to_non_nullable
                      as PersonalInfoModel?,
            contactInfo: freezed == contactInfo
                ? _value.contactInfo
                : contactInfo // ignore: cast_nullable_to_non_nullable
                      as ContactInfoModel?,
            addressInfo: freezed == addressInfo
                ? _value.addressInfo
                : addressInfo // ignore: cast_nullable_to_non_nullable
                      as AddressInfoModel?,
            availabilityInfo: freezed == availabilityInfo
                ? _value.availabilityInfo
                : availabilityInfo // ignore: cast_nullable_to_non_nullable
                      as AvailabilityInfoModel?,
            professionalInfo: freezed == professionalInfo
                ? _value.professionalInfo
                : professionalInfo // ignore: cast_nullable_to_non_nullable
                      as ProfessionalInfoModel?,
          )
          as $Val,
    );
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $PersonalInfoModelCopyWith<$Res>? get personalInfo {
    if (_value.personalInfo == null) {
      return null;
    }

    return $PersonalInfoModelCopyWith<$Res>(_value.personalInfo!, (value) {
      return _then(_value.copyWith(personalInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $ContactInfoModelCopyWith<$Res>? get contactInfo {
    if (_value.contactInfo == null) {
      return null;
    }

    return $ContactInfoModelCopyWith<$Res>(_value.contactInfo!, (value) {
      return _then(_value.copyWith(contactInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $AddressInfoModelCopyWith<$Res>? get addressInfo {
    if (_value.addressInfo == null) {
      return null;
    }

    return $AddressInfoModelCopyWith<$Res>(_value.addressInfo!, (value) {
      return _then(_value.copyWith(addressInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $AvailabilityInfoModelCopyWith<$Res>? get availabilityInfo {
    if (_value.availabilityInfo == null) {
      return null;
    }

    return $AvailabilityInfoModelCopyWith<$Res>(_value.availabilityInfo!, (
      value,
    ) {
      return _then(_value.copyWith(availabilityInfo: value) as $Val);
    });
  }

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @pragma('vm:prefer-inline')
  $ProfessionalInfoModelCopyWith<$Res>? get professionalInfo {
    if (_value.professionalInfo == null) {
      return null;
    }

    return $ProfessionalInfoModelCopyWith<$Res>(_value.professionalInfo!, (
      value,
    ) {
      return _then(_value.copyWith(professionalInfo: value) as $Val);
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
    PersonalInfoModel? personalInfo,
    ContactInfoModel? contactInfo,
    AddressInfoModel? addressInfo,
    AvailabilityInfoModel? availabilityInfo,
    ProfessionalInfoModel? professionalInfo,
  });

  @override
  $PersonalInfoModelCopyWith<$Res>? get personalInfo;
  @override
  $ContactInfoModelCopyWith<$Res>? get contactInfo;
  @override
  $AddressInfoModelCopyWith<$Res>? get addressInfo;
  @override
  $AvailabilityInfoModelCopyWith<$Res>? get availabilityInfo;
  @override
  $ProfessionalInfoModelCopyWith<$Res>? get professionalInfo;
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
    Object? personalInfo = freezed,
    Object? contactInfo = freezed,
    Object? addressInfo = freezed,
    Object? availabilityInfo = freezed,
    Object? professionalInfo = freezed,
  }) {
    return _then(
      _$RegistrationFormModelImpl(
        personalInfo: freezed == personalInfo
            ? _value.personalInfo
            : personalInfo // ignore: cast_nullable_to_non_nullable
                  as PersonalInfoModel?,
        contactInfo: freezed == contactInfo
            ? _value.contactInfo
            : contactInfo // ignore: cast_nullable_to_non_nullable
                  as ContactInfoModel?,
        addressInfo: freezed == addressInfo
            ? _value.addressInfo
            : addressInfo // ignore: cast_nullable_to_non_nullable
                  as AddressInfoModel?,
        availabilityInfo: freezed == availabilityInfo
            ? _value.availabilityInfo
            : availabilityInfo // ignore: cast_nullable_to_non_nullable
                  as AvailabilityInfoModel?,
        professionalInfo: freezed == professionalInfo
            ? _value.professionalInfo
            : professionalInfo // ignore: cast_nullable_to_non_nullable
                  as ProfessionalInfoModel?,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$RegistrationFormModelImpl extends _RegistrationFormModel {
  const _$RegistrationFormModelImpl({
    this.personalInfo,
    this.contactInfo,
    this.addressInfo,
    this.availabilityInfo,
    this.professionalInfo,
  }) : super._();

  factory _$RegistrationFormModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$RegistrationFormModelImplFromJson(json);

  @override
  final PersonalInfoModel? personalInfo;
  @override
  final ContactInfoModel? contactInfo;
  @override
  final AddressInfoModel? addressInfo;
  @override
  final AvailabilityInfoModel? availabilityInfo;
  @override
  final ProfessionalInfoModel? professionalInfo;

  @override
  String toString() {
    return 'RegistrationFormModel(personalInfo: $personalInfo, contactInfo: $contactInfo, addressInfo: $addressInfo, availabilityInfo: $availabilityInfo, professionalInfo: $professionalInfo)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$RegistrationFormModelImpl &&
            (identical(other.personalInfo, personalInfo) ||
                other.personalInfo == personalInfo) &&
            (identical(other.contactInfo, contactInfo) ||
                other.contactInfo == contactInfo) &&
            (identical(other.addressInfo, addressInfo) ||
                other.addressInfo == addressInfo) &&
            (identical(other.availabilityInfo, availabilityInfo) ||
                other.availabilityInfo == availabilityInfo) &&
            (identical(other.professionalInfo, professionalInfo) ||
                other.professionalInfo == professionalInfo));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    personalInfo,
    contactInfo,
    addressInfo,
    availabilityInfo,
    professionalInfo,
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
    final PersonalInfoModel? personalInfo,
    final ContactInfoModel? contactInfo,
    final AddressInfoModel? addressInfo,
    final AvailabilityInfoModel? availabilityInfo,
    final ProfessionalInfoModel? professionalInfo,
  }) = _$RegistrationFormModelImpl;
  const _RegistrationFormModel._() : super._();

  factory _RegistrationFormModel.fromJson(Map<String, dynamic> json) =
      _$RegistrationFormModelImpl.fromJson;

  @override
  PersonalInfoModel? get personalInfo;
  @override
  ContactInfoModel? get contactInfo;
  @override
  AddressInfoModel? get addressInfo;
  @override
  AvailabilityInfoModel? get availabilityInfo;
  @override
  ProfessionalInfoModel? get professionalInfo;

  /// Create a copy of RegistrationFormModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$RegistrationFormModelImplCopyWith<_$RegistrationFormModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
