// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'personal_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

PersonalInfoModel _$PersonalInfoModelFromJson(Map<String, dynamic> json) {
  return _PersonalInfoModel.fromJson(json);
}

/// @nodoc
mixin _$PersonalInfoModel {
  String get firstName => throw _privateConstructorUsedError;
  String get lastName => throw _privateConstructorUsedError;
  DateTime get dateOfBirth => throw _privateConstructorUsedError;
  Gender get gender => throw _privateConstructorUsedError;

  /// Serializes this PersonalInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of PersonalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $PersonalInfoModelCopyWith<PersonalInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $PersonalInfoModelCopyWith<$Res> {
  factory $PersonalInfoModelCopyWith(
    PersonalInfoModel value,
    $Res Function(PersonalInfoModel) then,
  ) = _$PersonalInfoModelCopyWithImpl<$Res, PersonalInfoModel>;
  @useResult
  $Res call({
    String firstName,
    String lastName,
    DateTime dateOfBirth,
    Gender gender,
  });
}

/// @nodoc
class _$PersonalInfoModelCopyWithImpl<$Res, $Val extends PersonalInfoModel>
    implements $PersonalInfoModelCopyWith<$Res> {
  _$PersonalInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of PersonalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? firstName = null,
    Object? lastName = null,
    Object? dateOfBirth = null,
    Object? gender = null,
  }) {
    return _then(
      _value.copyWith(
            firstName: null == firstName
                ? _value.firstName
                : firstName // ignore: cast_nullable_to_non_nullable
                      as String,
            lastName: null == lastName
                ? _value.lastName
                : lastName // ignore: cast_nullable_to_non_nullable
                      as String,
            dateOfBirth: null == dateOfBirth
                ? _value.dateOfBirth
                : dateOfBirth // ignore: cast_nullable_to_non_nullable
                      as DateTime,
            gender: null == gender
                ? _value.gender
                : gender // ignore: cast_nullable_to_non_nullable
                      as Gender,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$PersonalInfoModelImplCopyWith<$Res>
    implements $PersonalInfoModelCopyWith<$Res> {
  factory _$$PersonalInfoModelImplCopyWith(
    _$PersonalInfoModelImpl value,
    $Res Function(_$PersonalInfoModelImpl) then,
  ) = __$$PersonalInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String firstName,
    String lastName,
    DateTime dateOfBirth,
    Gender gender,
  });
}

/// @nodoc
class __$$PersonalInfoModelImplCopyWithImpl<$Res>
    extends _$PersonalInfoModelCopyWithImpl<$Res, _$PersonalInfoModelImpl>
    implements _$$PersonalInfoModelImplCopyWith<$Res> {
  __$$PersonalInfoModelImplCopyWithImpl(
    _$PersonalInfoModelImpl _value,
    $Res Function(_$PersonalInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of PersonalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? firstName = null,
    Object? lastName = null,
    Object? dateOfBirth = null,
    Object? gender = null,
  }) {
    return _then(
      _$PersonalInfoModelImpl(
        firstName: null == firstName
            ? _value.firstName
            : firstName // ignore: cast_nullable_to_non_nullable
                  as String,
        lastName: null == lastName
            ? _value.lastName
            : lastName // ignore: cast_nullable_to_non_nullable
                  as String,
        dateOfBirth: null == dateOfBirth
            ? _value.dateOfBirth
            : dateOfBirth // ignore: cast_nullable_to_non_nullable
                  as DateTime,
        gender: null == gender
            ? _value.gender
            : gender // ignore: cast_nullable_to_non_nullable
                  as Gender,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$PersonalInfoModelImpl extends _PersonalInfoModel {
  const _$PersonalInfoModelImpl({
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
    required this.gender,
  }) : super._();

  factory _$PersonalInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$PersonalInfoModelImplFromJson(json);

  @override
  final String firstName;
  @override
  final String lastName;
  @override
  final DateTime dateOfBirth;
  @override
  final Gender gender;

  @override
  String toString() {
    return 'PersonalInfoModel(firstName: $firstName, lastName: $lastName, dateOfBirth: $dateOfBirth, gender: $gender)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$PersonalInfoModelImpl &&
            (identical(other.firstName, firstName) ||
                other.firstName == firstName) &&
            (identical(other.lastName, lastName) ||
                other.lastName == lastName) &&
            (identical(other.dateOfBirth, dateOfBirth) ||
                other.dateOfBirth == dateOfBirth) &&
            (identical(other.gender, gender) || other.gender == gender));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode =>
      Object.hash(runtimeType, firstName, lastName, dateOfBirth, gender);

  /// Create a copy of PersonalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$PersonalInfoModelImplCopyWith<_$PersonalInfoModelImpl> get copyWith =>
      __$$PersonalInfoModelImplCopyWithImpl<_$PersonalInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$PersonalInfoModelImplToJson(this);
  }
}

abstract class _PersonalInfoModel extends PersonalInfoModel {
  const factory _PersonalInfoModel({
    required final String firstName,
    required final String lastName,
    required final DateTime dateOfBirth,
    required final Gender gender,
  }) = _$PersonalInfoModelImpl;
  const _PersonalInfoModel._() : super._();

  factory _PersonalInfoModel.fromJson(Map<String, dynamic> json) =
      _$PersonalInfoModelImpl.fromJson;

  @override
  String get firstName;
  @override
  String get lastName;
  @override
  DateTime get dateOfBirth;
  @override
  Gender get gender;

  /// Create a copy of PersonalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$PersonalInfoModelImplCopyWith<_$PersonalInfoModelImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
