// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'availability_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

AvailabilityInfoModel _$AvailabilityInfoModelFromJson(
  Map<String, dynamic> json,
) {
  return _AvailabilityInfoModel.fromJson(json);
}

/// @nodoc
mixin _$AvailabilityInfoModel {
  String? get availabilityTypeId =>
      throw _privateConstructorUsedError; // UUID from API (may be null)
  String get availabilityTypeName =>
      throw _privateConstructorUsedError; // Name from API
  Map<String, String> get availableTimes =>
      throw _privateConstructorUsedError; // {id: value} - only times with IDs
  List<String> get availableDays => throw _privateConstructorUsedError;

  /// Serializes this AvailabilityInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $AvailabilityInfoModelCopyWith<AvailabilityInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $AvailabilityInfoModelCopyWith<$Res> {
  factory $AvailabilityInfoModelCopyWith(
    AvailabilityInfoModel value,
    $Res Function(AvailabilityInfoModel) then,
  ) = _$AvailabilityInfoModelCopyWithImpl<$Res, AvailabilityInfoModel>;
  @useResult
  $Res call({
    String? availabilityTypeId,
    String availabilityTypeName,
    Map<String, String> availableTimes,
    List<String> availableDays,
  });
}

/// @nodoc
class _$AvailabilityInfoModelCopyWithImpl<
  $Res,
  $Val extends AvailabilityInfoModel
>
    implements $AvailabilityInfoModelCopyWith<$Res> {
  _$AvailabilityInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? availabilityTypeId = freezed,
    Object? availabilityTypeName = null,
    Object? availableTimes = null,
    Object? availableDays = null,
  }) {
    return _then(
      _value.copyWith(
            availabilityTypeId: freezed == availabilityTypeId
                ? _value.availabilityTypeId
                : availabilityTypeId // ignore: cast_nullable_to_non_nullable
                      as String?,
            availabilityTypeName: null == availabilityTypeName
                ? _value.availabilityTypeName
                : availabilityTypeName // ignore: cast_nullable_to_non_nullable
                      as String,
            availableTimes: null == availableTimes
                ? _value.availableTimes
                : availableTimes // ignore: cast_nullable_to_non_nullable
                      as Map<String, String>,
            availableDays: null == availableDays
                ? _value.availableDays
                : availableDays // ignore: cast_nullable_to_non_nullable
                      as List<String>,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$AvailabilityInfoModelImplCopyWith<$Res>
    implements $AvailabilityInfoModelCopyWith<$Res> {
  factory _$$AvailabilityInfoModelImplCopyWith(
    _$AvailabilityInfoModelImpl value,
    $Res Function(_$AvailabilityInfoModelImpl) then,
  ) = __$$AvailabilityInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String? availabilityTypeId,
    String availabilityTypeName,
    Map<String, String> availableTimes,
    List<String> availableDays,
  });
}

/// @nodoc
class __$$AvailabilityInfoModelImplCopyWithImpl<$Res>
    extends
        _$AvailabilityInfoModelCopyWithImpl<$Res, _$AvailabilityInfoModelImpl>
    implements _$$AvailabilityInfoModelImplCopyWith<$Res> {
  __$$AvailabilityInfoModelImplCopyWithImpl(
    _$AvailabilityInfoModelImpl _value,
    $Res Function(_$AvailabilityInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? availabilityTypeId = freezed,
    Object? availabilityTypeName = null,
    Object? availableTimes = null,
    Object? availableDays = null,
  }) {
    return _then(
      _$AvailabilityInfoModelImpl(
        availabilityTypeId: freezed == availabilityTypeId
            ? _value.availabilityTypeId
            : availabilityTypeId // ignore: cast_nullable_to_non_nullable
                  as String?,
        availabilityTypeName: null == availabilityTypeName
            ? _value.availabilityTypeName
            : availabilityTypeName // ignore: cast_nullable_to_non_nullable
                  as String,
        availableTimes: null == availableTimes
            ? _value._availableTimes
            : availableTimes // ignore: cast_nullable_to_non_nullable
                  as Map<String, String>,
        availableDays: null == availableDays
            ? _value._availableDays
            : availableDays // ignore: cast_nullable_to_non_nullable
                  as List<String>,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$AvailabilityInfoModelImpl extends _AvailabilityInfoModel {
  const _$AvailabilityInfoModelImpl({
    this.availabilityTypeId,
    required this.availabilityTypeName,
    required final Map<String, String> availableTimes,
    required final List<String> availableDays,
  }) : _availableTimes = availableTimes,
       _availableDays = availableDays,
       super._();

  factory _$AvailabilityInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$AvailabilityInfoModelImplFromJson(json);

  @override
  final String? availabilityTypeId;
  // UUID from API (may be null)
  @override
  final String availabilityTypeName;
  // Name from API
  final Map<String, String> _availableTimes;
  // Name from API
  @override
  Map<String, String> get availableTimes {
    if (_availableTimes is EqualUnmodifiableMapView) return _availableTimes;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(_availableTimes);
  }

  // {id: value} - only times with IDs
  final List<String> _availableDays;
  // {id: value} - only times with IDs
  @override
  List<String> get availableDays {
    if (_availableDays is EqualUnmodifiableListView) return _availableDays;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_availableDays);
  }

  @override
  String toString() {
    return 'AvailabilityInfoModel(availabilityTypeId: $availabilityTypeId, availabilityTypeName: $availabilityTypeName, availableTimes: $availableTimes, availableDays: $availableDays)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$AvailabilityInfoModelImpl &&
            (identical(other.availabilityTypeId, availabilityTypeId) ||
                other.availabilityTypeId == availabilityTypeId) &&
            (identical(other.availabilityTypeName, availabilityTypeName) ||
                other.availabilityTypeName == availabilityTypeName) &&
            const DeepCollectionEquality().equals(
              other._availableTimes,
              _availableTimes,
            ) &&
            const DeepCollectionEquality().equals(
              other._availableDays,
              _availableDays,
            ));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    availabilityTypeId,
    availabilityTypeName,
    const DeepCollectionEquality().hash(_availableTimes),
    const DeepCollectionEquality().hash(_availableDays),
  );

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$AvailabilityInfoModelImplCopyWith<_$AvailabilityInfoModelImpl>
  get copyWith =>
      __$$AvailabilityInfoModelImplCopyWithImpl<_$AvailabilityInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$AvailabilityInfoModelImplToJson(this);
  }
}

abstract class _AvailabilityInfoModel extends AvailabilityInfoModel {
  const factory _AvailabilityInfoModel({
    final String? availabilityTypeId,
    required final String availabilityTypeName,
    required final Map<String, String> availableTimes,
    required final List<String> availableDays,
  }) = _$AvailabilityInfoModelImpl;
  const _AvailabilityInfoModel._() : super._();

  factory _AvailabilityInfoModel.fromJson(Map<String, dynamic> json) =
      _$AvailabilityInfoModelImpl.fromJson;

  @override
  String? get availabilityTypeId; // UUID from API (may be null)
  @override
  String get availabilityTypeName; // Name from API
  @override
  Map<String, String> get availableTimes; // {id: value} - only times with IDs
  @override
  List<String> get availableDays;

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$AvailabilityInfoModelImplCopyWith<_$AvailabilityInfoModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
