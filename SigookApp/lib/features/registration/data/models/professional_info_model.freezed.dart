// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'professional_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

ProfessionalInfoModel _$ProfessionalInfoModelFromJson(
  Map<String, dynamic> json,
) {
  return _ProfessionalInfoModel.fromJson(json);
}

/// @nodoc
mixin _$ProfessionalInfoModel {
  Map<String, String> get languages =>
      throw _privateConstructorUsedError; // {id: value} - only languages with IDs
  List<String> get skills => throw _privateConstructorUsedError;

  /// Serializes this ProfessionalInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of ProfessionalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $ProfessionalInfoModelCopyWith<ProfessionalInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $ProfessionalInfoModelCopyWith<$Res> {
  factory $ProfessionalInfoModelCopyWith(
    ProfessionalInfoModel value,
    $Res Function(ProfessionalInfoModel) then,
  ) = _$ProfessionalInfoModelCopyWithImpl<$Res, ProfessionalInfoModel>;
  @useResult
  $Res call({Map<String, String> languages, List<String> skills});
}

/// @nodoc
class _$ProfessionalInfoModelCopyWithImpl<
  $Res,
  $Val extends ProfessionalInfoModel
>
    implements $ProfessionalInfoModelCopyWith<$Res> {
  _$ProfessionalInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of ProfessionalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({Object? languages = null, Object? skills = null}) {
    return _then(
      _value.copyWith(
            languages: null == languages
                ? _value.languages
                : languages // ignore: cast_nullable_to_non_nullable
                      as Map<String, String>,
            skills: null == skills
                ? _value.skills
                : skills // ignore: cast_nullable_to_non_nullable
                      as List<String>,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$ProfessionalInfoModelImplCopyWith<$Res>
    implements $ProfessionalInfoModelCopyWith<$Res> {
  factory _$$ProfessionalInfoModelImplCopyWith(
    _$ProfessionalInfoModelImpl value,
    $Res Function(_$ProfessionalInfoModelImpl) then,
  ) = __$$ProfessionalInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({Map<String, String> languages, List<String> skills});
}

/// @nodoc
class __$$ProfessionalInfoModelImplCopyWithImpl<$Res>
    extends
        _$ProfessionalInfoModelCopyWithImpl<$Res, _$ProfessionalInfoModelImpl>
    implements _$$ProfessionalInfoModelImplCopyWith<$Res> {
  __$$ProfessionalInfoModelImplCopyWithImpl(
    _$ProfessionalInfoModelImpl _value,
    $Res Function(_$ProfessionalInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of ProfessionalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({Object? languages = null, Object? skills = null}) {
    return _then(
      _$ProfessionalInfoModelImpl(
        languages: null == languages
            ? _value._languages
            : languages // ignore: cast_nullable_to_non_nullable
                  as Map<String, String>,
        skills: null == skills
            ? _value._skills
            : skills // ignore: cast_nullable_to_non_nullable
                  as List<String>,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$ProfessionalInfoModelImpl extends _ProfessionalInfoModel {
  const _$ProfessionalInfoModelImpl({
    required final Map<String, String> languages,
    required final List<String> skills,
  }) : _languages = languages,
       _skills = skills,
       super._();

  factory _$ProfessionalInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$ProfessionalInfoModelImplFromJson(json);

  final Map<String, String> _languages;
  @override
  Map<String, String> get languages {
    if (_languages is EqualUnmodifiableMapView) return _languages;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(_languages);
  }

  // {id: value} - only languages with IDs
  final List<String> _skills;
  // {id: value} - only languages with IDs
  @override
  List<String> get skills {
    if (_skills is EqualUnmodifiableListView) return _skills;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_skills);
  }

  @override
  String toString() {
    return 'ProfessionalInfoModel(languages: $languages, skills: $skills)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$ProfessionalInfoModelImpl &&
            const DeepCollectionEquality().equals(
              other._languages,
              _languages,
            ) &&
            const DeepCollectionEquality().equals(other._skills, _skills));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    const DeepCollectionEquality().hash(_languages),
    const DeepCollectionEquality().hash(_skills),
  );

  /// Create a copy of ProfessionalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$ProfessionalInfoModelImplCopyWith<_$ProfessionalInfoModelImpl>
  get copyWith =>
      __$$ProfessionalInfoModelImplCopyWithImpl<_$ProfessionalInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$ProfessionalInfoModelImplToJson(this);
  }
}

abstract class _ProfessionalInfoModel extends ProfessionalInfoModel {
  const factory _ProfessionalInfoModel({
    required final Map<String, String> languages,
    required final List<String> skills,
  }) = _$ProfessionalInfoModelImpl;
  const _ProfessionalInfoModel._() : super._();

  factory _ProfessionalInfoModel.fromJson(Map<String, dynamic> json) =
      _$ProfessionalInfoModelImpl.fromJson;

  @override
  Map<String, String> get languages; // {id: value} - only languages with IDs
  @override
  List<String> get skills;

  /// Create a copy of ProfessionalInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$ProfessionalInfoModelImplCopyWith<_$ProfessionalInfoModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
