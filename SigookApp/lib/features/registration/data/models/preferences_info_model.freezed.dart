// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'preferences_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

PreferencesInfoModel _$PreferencesInfoModelFromJson(Map<String, dynamic> json) {
  return _PreferencesInfoModel.fromJson(json);
}

/// @nodoc
mixin _$PreferencesInfoModel {
  String get availabilityType => throw _privateConstructorUsedError;
  List<String> get availableTimes => throw _privateConstructorUsedError;
  List<String> get availableDays => throw _privateConstructorUsedError;
  String get liftingCapacity =>
      throw _privateConstructorUsedError; // Stored as string (enum label)
  bool get hasVehicle => throw _privateConstructorUsedError;
  Map<String, String> get languages =>
      throw _privateConstructorUsedError; // id -> value
  List<String> get skills => throw _privateConstructorUsedError;

  /// Serializes this PreferencesInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of PreferencesInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $PreferencesInfoModelCopyWith<PreferencesInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $PreferencesInfoModelCopyWith<$Res> {
  factory $PreferencesInfoModelCopyWith(
    PreferencesInfoModel value,
    $Res Function(PreferencesInfoModel) then,
  ) = _$PreferencesInfoModelCopyWithImpl<$Res, PreferencesInfoModel>;
  @useResult
  $Res call({
    String availabilityType,
    List<String> availableTimes,
    List<String> availableDays,
    String liftingCapacity,
    bool hasVehicle,
    Map<String, String> languages,
    List<String> skills,
  });
}

/// @nodoc
class _$PreferencesInfoModelCopyWithImpl<
  $Res,
  $Val extends PreferencesInfoModel
>
    implements $PreferencesInfoModelCopyWith<$Res> {
  _$PreferencesInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of PreferencesInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? availabilityType = null,
    Object? availableTimes = null,
    Object? availableDays = null,
    Object? liftingCapacity = null,
    Object? hasVehicle = null,
    Object? languages = null,
    Object? skills = null,
  }) {
    return _then(
      _value.copyWith(
            availabilityType: null == availabilityType
                ? _value.availabilityType
                : availabilityType // ignore: cast_nullable_to_non_nullable
                      as String,
            availableTimes: null == availableTimes
                ? _value.availableTimes
                : availableTimes // ignore: cast_nullable_to_non_nullable
                      as List<String>,
            availableDays: null == availableDays
                ? _value.availableDays
                : availableDays // ignore: cast_nullable_to_non_nullable
                      as List<String>,
            liftingCapacity: null == liftingCapacity
                ? _value.liftingCapacity
                : liftingCapacity // ignore: cast_nullable_to_non_nullable
                      as String,
            hasVehicle: null == hasVehicle
                ? _value.hasVehicle
                : hasVehicle // ignore: cast_nullable_to_non_nullable
                      as bool,
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
abstract class _$$PreferencesInfoModelImplCopyWith<$Res>
    implements $PreferencesInfoModelCopyWith<$Res> {
  factory _$$PreferencesInfoModelImplCopyWith(
    _$PreferencesInfoModelImpl value,
    $Res Function(_$PreferencesInfoModelImpl) then,
  ) = __$$PreferencesInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String availabilityType,
    List<String> availableTimes,
    List<String> availableDays,
    String liftingCapacity,
    bool hasVehicle,
    Map<String, String> languages,
    List<String> skills,
  });
}

/// @nodoc
class __$$PreferencesInfoModelImplCopyWithImpl<$Res>
    extends _$PreferencesInfoModelCopyWithImpl<$Res, _$PreferencesInfoModelImpl>
    implements _$$PreferencesInfoModelImplCopyWith<$Res> {
  __$$PreferencesInfoModelImplCopyWithImpl(
    _$PreferencesInfoModelImpl _value,
    $Res Function(_$PreferencesInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of PreferencesInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? availabilityType = null,
    Object? availableTimes = null,
    Object? availableDays = null,
    Object? liftingCapacity = null,
    Object? hasVehicle = null,
    Object? languages = null,
    Object? skills = null,
  }) {
    return _then(
      _$PreferencesInfoModelImpl(
        availabilityType: null == availabilityType
            ? _value.availabilityType
            : availabilityType // ignore: cast_nullable_to_non_nullable
                  as String,
        availableTimes: null == availableTimes
            ? _value._availableTimes
            : availableTimes // ignore: cast_nullable_to_non_nullable
                  as List<String>,
        availableDays: null == availableDays
            ? _value._availableDays
            : availableDays // ignore: cast_nullable_to_non_nullable
                  as List<String>,
        liftingCapacity: null == liftingCapacity
            ? _value.liftingCapacity
            : liftingCapacity // ignore: cast_nullable_to_non_nullable
                  as String,
        hasVehicle: null == hasVehicle
            ? _value.hasVehicle
            : hasVehicle // ignore: cast_nullable_to_non_nullable
                  as bool,
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
class _$PreferencesInfoModelImpl extends _PreferencesInfoModel {
  const _$PreferencesInfoModelImpl({
    required this.availabilityType,
    required final List<String> availableTimes,
    required final List<String> availableDays,
    required this.liftingCapacity,
    required this.hasVehicle,
    required final Map<String, String> languages,
    required final List<String> skills,
  }) : _availableTimes = availableTimes,
       _availableDays = availableDays,
       _languages = languages,
       _skills = skills,
       super._();

  factory _$PreferencesInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$PreferencesInfoModelImplFromJson(json);

  @override
  final String availabilityType;
  final List<String> _availableTimes;
  @override
  List<String> get availableTimes {
    if (_availableTimes is EqualUnmodifiableListView) return _availableTimes;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_availableTimes);
  }

  final List<String> _availableDays;
  @override
  List<String> get availableDays {
    if (_availableDays is EqualUnmodifiableListView) return _availableDays;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_availableDays);
  }

  @override
  final String liftingCapacity;
  // Stored as string (enum label)
  @override
  final bool hasVehicle;
  final Map<String, String> _languages;
  @override
  Map<String, String> get languages {
    if (_languages is EqualUnmodifiableMapView) return _languages;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(_languages);
  }

  // id -> value
  final List<String> _skills;
  // id -> value
  @override
  List<String> get skills {
    if (_skills is EqualUnmodifiableListView) return _skills;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_skills);
  }

  @override
  String toString() {
    return 'PreferencesInfoModel(availabilityType: $availabilityType, availableTimes: $availableTimes, availableDays: $availableDays, liftingCapacity: $liftingCapacity, hasVehicle: $hasVehicle, languages: $languages, skills: $skills)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$PreferencesInfoModelImpl &&
            (identical(other.availabilityType, availabilityType) ||
                other.availabilityType == availabilityType) &&
            const DeepCollectionEquality().equals(
              other._availableTimes,
              _availableTimes,
            ) &&
            const DeepCollectionEquality().equals(
              other._availableDays,
              _availableDays,
            ) &&
            (identical(other.liftingCapacity, liftingCapacity) ||
                other.liftingCapacity == liftingCapacity) &&
            (identical(other.hasVehicle, hasVehicle) ||
                other.hasVehicle == hasVehicle) &&
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
    availabilityType,
    const DeepCollectionEquality().hash(_availableTimes),
    const DeepCollectionEquality().hash(_availableDays),
    liftingCapacity,
    hasVehicle,
    const DeepCollectionEquality().hash(_languages),
    const DeepCollectionEquality().hash(_skills),
  );

  /// Create a copy of PreferencesInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$PreferencesInfoModelImplCopyWith<_$PreferencesInfoModelImpl>
  get copyWith =>
      __$$PreferencesInfoModelImplCopyWithImpl<_$PreferencesInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$PreferencesInfoModelImplToJson(this);
  }
}

abstract class _PreferencesInfoModel extends PreferencesInfoModel {
  const factory _PreferencesInfoModel({
    required final String availabilityType,
    required final List<String> availableTimes,
    required final List<String> availableDays,
    required final String liftingCapacity,
    required final bool hasVehicle,
    required final Map<String, String> languages,
    required final List<String> skills,
  }) = _$PreferencesInfoModelImpl;
  const _PreferencesInfoModel._() : super._();

  factory _PreferencesInfoModel.fromJson(Map<String, dynamic> json) =
      _$PreferencesInfoModelImpl.fromJson;

  @override
  String get availabilityType;
  @override
  List<String> get availableTimes;
  @override
  List<String> get availableDays;
  @override
  String get liftingCapacity; // Stored as string (enum label)
  @override
  bool get hasVehicle;
  @override
  Map<String, String> get languages; // id -> value
  @override
  List<String> get skills;

  /// Create a copy of PreferencesInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$PreferencesInfoModelImplCopyWith<_$PreferencesInfoModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
