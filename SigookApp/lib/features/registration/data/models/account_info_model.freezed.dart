// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'account_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

AccountInfoModel _$AccountInfoModelFromJson(Map<String, dynamic> json) {
  return _AccountInfoModel.fromJson(json);
}

/// @nodoc
mixin _$AccountInfoModel {
  String get email => throw _privateConstructorUsedError;
  String get password => throw _privateConstructorUsedError;
  String get confirmPassword => throw _privateConstructorUsedError;
  bool get termsAccepted => throw _privateConstructorUsedError;

  /// Serializes this AccountInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of AccountInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $AccountInfoModelCopyWith<AccountInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $AccountInfoModelCopyWith<$Res> {
  factory $AccountInfoModelCopyWith(
    AccountInfoModel value,
    $Res Function(AccountInfoModel) then,
  ) = _$AccountInfoModelCopyWithImpl<$Res, AccountInfoModel>;
  @useResult
  $Res call({
    String email,
    String password,
    String confirmPassword,
    bool termsAccepted,
  });
}

/// @nodoc
class _$AccountInfoModelCopyWithImpl<$Res, $Val extends AccountInfoModel>
    implements $AccountInfoModelCopyWith<$Res> {
  _$AccountInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of AccountInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? email = null,
    Object? password = null,
    Object? confirmPassword = null,
    Object? termsAccepted = null,
  }) {
    return _then(
      _value.copyWith(
            email: null == email
                ? _value.email
                : email // ignore: cast_nullable_to_non_nullable
                      as String,
            password: null == password
                ? _value.password
                : password // ignore: cast_nullable_to_non_nullable
                      as String,
            confirmPassword: null == confirmPassword
                ? _value.confirmPassword
                : confirmPassword // ignore: cast_nullable_to_non_nullable
                      as String,
            termsAccepted: null == termsAccepted
                ? _value.termsAccepted
                : termsAccepted // ignore: cast_nullable_to_non_nullable
                      as bool,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$AccountInfoModelImplCopyWith<$Res>
    implements $AccountInfoModelCopyWith<$Res> {
  factory _$$AccountInfoModelImplCopyWith(
    _$AccountInfoModelImpl value,
    $Res Function(_$AccountInfoModelImpl) then,
  ) = __$$AccountInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    String email,
    String password,
    String confirmPassword,
    bool termsAccepted,
  });
}

/// @nodoc
class __$$AccountInfoModelImplCopyWithImpl<$Res>
    extends _$AccountInfoModelCopyWithImpl<$Res, _$AccountInfoModelImpl>
    implements _$$AccountInfoModelImplCopyWith<$Res> {
  __$$AccountInfoModelImplCopyWithImpl(
    _$AccountInfoModelImpl _value,
    $Res Function(_$AccountInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of AccountInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? email = null,
    Object? password = null,
    Object? confirmPassword = null,
    Object? termsAccepted = null,
  }) {
    return _then(
      _$AccountInfoModelImpl(
        email: null == email
            ? _value.email
            : email // ignore: cast_nullable_to_non_nullable
                  as String,
        password: null == password
            ? _value.password
            : password // ignore: cast_nullable_to_non_nullable
                  as String,
        confirmPassword: null == confirmPassword
            ? _value.confirmPassword
            : confirmPassword // ignore: cast_nullable_to_non_nullable
                  as String,
        termsAccepted: null == termsAccepted
            ? _value.termsAccepted
            : termsAccepted // ignore: cast_nullable_to_non_nullable
                  as bool,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$AccountInfoModelImpl extends _AccountInfoModel {
  const _$AccountInfoModelImpl({
    required this.email,
    required this.password,
    required this.confirmPassword,
    required this.termsAccepted,
  }) : super._();

  factory _$AccountInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$AccountInfoModelImplFromJson(json);

  @override
  final String email;
  @override
  final String password;
  @override
  final String confirmPassword;
  @override
  final bool termsAccepted;

  @override
  String toString() {
    return 'AccountInfoModel(email: $email, password: $password, confirmPassword: $confirmPassword, termsAccepted: $termsAccepted)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$AccountInfoModelImpl &&
            (identical(other.email, email) || other.email == email) &&
            (identical(other.password, password) ||
                other.password == password) &&
            (identical(other.confirmPassword, confirmPassword) ||
                other.confirmPassword == confirmPassword) &&
            (identical(other.termsAccepted, termsAccepted) ||
                other.termsAccepted == termsAccepted));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode =>
      Object.hash(runtimeType, email, password, confirmPassword, termsAccepted);

  /// Create a copy of AccountInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$AccountInfoModelImplCopyWith<_$AccountInfoModelImpl> get copyWith =>
      __$$AccountInfoModelImplCopyWithImpl<_$AccountInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$AccountInfoModelImplToJson(this);
  }
}

abstract class _AccountInfoModel extends AccountInfoModel {
  const factory _AccountInfoModel({
    required final String email,
    required final String password,
    required final String confirmPassword,
    required final bool termsAccepted,
  }) = _$AccountInfoModelImpl;
  const _AccountInfoModel._() : super._();

  factory _AccountInfoModel.fromJson(Map<String, dynamic> json) =
      _$AccountInfoModelImpl.fromJson;

  @override
  String get email;
  @override
  String get password;
  @override
  String get confirmPassword;
  @override
  bool get termsAccepted;

  /// Create a copy of AccountInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$AccountInfoModelImplCopyWith<_$AccountInfoModelImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
