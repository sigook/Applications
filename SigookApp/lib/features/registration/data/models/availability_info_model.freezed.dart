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
  AvailabilityType get availabilityType => throw _privateConstructorUsedError;
  List<TimeOfDay> get availableTimes => throw _privateConstructorUsedError;
  List<DayOfWeek> get availableDays => throw _privateConstructorUsedError;

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
    AvailabilityType availabilityType,
    List<TimeOfDay> availableTimes,
    List<DayOfWeek> availableDays,
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
    Object? availabilityType = null,
    Object? availableTimes = null,
    Object? availableDays = null,
  }) {
    return _then(
      _value.copyWith(
            availabilityType: null == availabilityType
                ? _value.availabilityType
                : availabilityType // ignore: cast_nullable_to_non_nullable
                      as AvailabilityType,
            availableTimes: null == availableTimes
                ? _value.availableTimes
                : availableTimes // ignore: cast_nullable_to_non_nullable
                      as List<TimeOfDay>,
            availableDays: null == availableDays
                ? _value.availableDays
                : availableDays // ignore: cast_nullable_to_non_nullable
                      as List<DayOfWeek>,
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
    AvailabilityType availabilityType,
    List<TimeOfDay> availableTimes,
    List<DayOfWeek> availableDays,
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
    Object? availabilityType = null,
    Object? availableTimes = null,
    Object? availableDays = null,
  }) {
    return _then(
      _$AvailabilityInfoModelImpl(
        availabilityType: null == availabilityType
            ? _value.availabilityType
            : availabilityType // ignore: cast_nullable_to_non_nullable
                  as AvailabilityType,
        availableTimes: null == availableTimes
            ? _value._availableTimes
            : availableTimes // ignore: cast_nullable_to_non_nullable
                  as List<TimeOfDay>,
        availableDays: null == availableDays
            ? _value._availableDays
            : availableDays // ignore: cast_nullable_to_non_nullable
                  as List<DayOfWeek>,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$AvailabilityInfoModelImpl extends _AvailabilityInfoModel {
  const _$AvailabilityInfoModelImpl({
    required this.availabilityType,
    required final List<TimeOfDay> availableTimes,
    required final List<DayOfWeek> availableDays,
  }) : _availableTimes = availableTimes,
       _availableDays = availableDays,
       super._();

  factory _$AvailabilityInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$AvailabilityInfoModelImplFromJson(json);

  @override
  final AvailabilityType availabilityType;
  final List<TimeOfDay> _availableTimes;
  @override
  List<TimeOfDay> get availableTimes {
    if (_availableTimes is EqualUnmodifiableListView) return _availableTimes;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_availableTimes);
  }

  final List<DayOfWeek> _availableDays;
  @override
  List<DayOfWeek> get availableDays {
    if (_availableDays is EqualUnmodifiableListView) return _availableDays;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_availableDays);
  }

  @override
  String toString() {
    return 'AvailabilityInfoModel(availabilityType: $availabilityType, availableTimes: $availableTimes, availableDays: $availableDays)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$AvailabilityInfoModelImpl &&
            (identical(other.availabilityType, availabilityType) ||
                other.availabilityType == availabilityType) &&
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
    availabilityType,
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
    required final AvailabilityType availabilityType,
    required final List<TimeOfDay> availableTimes,
    required final List<DayOfWeek> availableDays,
  }) = _$AvailabilityInfoModelImpl;
  const _AvailabilityInfoModel._() : super._();

  factory _AvailabilityInfoModel.fromJson(Map<String, dynamic> json) =
      _$AvailabilityInfoModelImpl.fromJson;

  @override
  AvailabilityType get availabilityType;
  @override
  List<TimeOfDay> get availableTimes;
  @override
  List<DayOfWeek> get availableDays;

  /// Create a copy of AvailabilityInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$AvailabilityInfoModelImplCopyWith<_$AvailabilityInfoModelImpl>
  get copyWith => throw _privateConstructorUsedError;
}
