// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'documents_info_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
  'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models',
);

DocumentsInfoModel _$DocumentsInfoModelFromJson(Map<String, dynamic> json) {
  return _DocumentsInfoModel.fromJson(json);
}

/// @nodoc
mixin _$DocumentsInfoModel {
  List<String> get documents => throw _privateConstructorUsedError;
  List<String> get licenses => throw _privateConstructorUsedError;
  List<String> get certificates => throw _privateConstructorUsedError;
  String? get resume => throw _privateConstructorUsedError;

  /// Serializes this DocumentsInfoModel to a JSON map.
  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;

  /// Create a copy of DocumentsInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $DocumentsInfoModelCopyWith<DocumentsInfoModel> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $DocumentsInfoModelCopyWith<$Res> {
  factory $DocumentsInfoModelCopyWith(
    DocumentsInfoModel value,
    $Res Function(DocumentsInfoModel) then,
  ) = _$DocumentsInfoModelCopyWithImpl<$Res, DocumentsInfoModel>;
  @useResult
  $Res call({
    List<String> documents,
    List<String> licenses,
    List<String> certificates,
    String? resume,
  });
}

/// @nodoc
class _$DocumentsInfoModelCopyWithImpl<$Res, $Val extends DocumentsInfoModel>
    implements $DocumentsInfoModelCopyWith<$Res> {
  _$DocumentsInfoModelCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of DocumentsInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? documents = null,
    Object? licenses = null,
    Object? certificates = null,
    Object? resume = freezed,
  }) {
    return _then(
      _value.copyWith(
            documents: null == documents
                ? _value.documents
                : documents // ignore: cast_nullable_to_non_nullable
                      as List<String>,
            licenses: null == licenses
                ? _value.licenses
                : licenses // ignore: cast_nullable_to_non_nullable
                      as List<String>,
            certificates: null == certificates
                ? _value.certificates
                : certificates // ignore: cast_nullable_to_non_nullable
                      as List<String>,
            resume: freezed == resume
                ? _value.resume
                : resume // ignore: cast_nullable_to_non_nullable
                      as String?,
          )
          as $Val,
    );
  }
}

/// @nodoc
abstract class _$$DocumentsInfoModelImplCopyWith<$Res>
    implements $DocumentsInfoModelCopyWith<$Res> {
  factory _$$DocumentsInfoModelImplCopyWith(
    _$DocumentsInfoModelImpl value,
    $Res Function(_$DocumentsInfoModelImpl) then,
  ) = __$$DocumentsInfoModelImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({
    List<String> documents,
    List<String> licenses,
    List<String> certificates,
    String? resume,
  });
}

/// @nodoc
class __$$DocumentsInfoModelImplCopyWithImpl<$Res>
    extends _$DocumentsInfoModelCopyWithImpl<$Res, _$DocumentsInfoModelImpl>
    implements _$$DocumentsInfoModelImplCopyWith<$Res> {
  __$$DocumentsInfoModelImplCopyWithImpl(
    _$DocumentsInfoModelImpl _value,
    $Res Function(_$DocumentsInfoModelImpl) _then,
  ) : super(_value, _then);

  /// Create a copy of DocumentsInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? documents = null,
    Object? licenses = null,
    Object? certificates = null,
    Object? resume = freezed,
  }) {
    return _then(
      _$DocumentsInfoModelImpl(
        documents: null == documents
            ? _value._documents
            : documents // ignore: cast_nullable_to_non_nullable
                  as List<String>,
        licenses: null == licenses
            ? _value._licenses
            : licenses // ignore: cast_nullable_to_non_nullable
                  as List<String>,
        certificates: null == certificates
            ? _value._certificates
            : certificates // ignore: cast_nullable_to_non_nullable
                  as List<String>,
        resume: freezed == resume
            ? _value.resume
            : resume // ignore: cast_nullable_to_non_nullable
                  as String?,
      ),
    );
  }
}

/// @nodoc
@JsonSerializable()
class _$DocumentsInfoModelImpl extends _DocumentsInfoModel {
  const _$DocumentsInfoModelImpl({
    required final List<String> documents,
    required final List<String> licenses,
    required final List<String> certificates,
    this.resume,
  }) : _documents = documents,
       _licenses = licenses,
       _certificates = certificates,
       super._();

  factory _$DocumentsInfoModelImpl.fromJson(Map<String, dynamic> json) =>
      _$$DocumentsInfoModelImplFromJson(json);

  final List<String> _documents;
  @override
  List<String> get documents {
    if (_documents is EqualUnmodifiableListView) return _documents;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_documents);
  }

  final List<String> _licenses;
  @override
  List<String> get licenses {
    if (_licenses is EqualUnmodifiableListView) return _licenses;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_licenses);
  }

  final List<String> _certificates;
  @override
  List<String> get certificates {
    if (_certificates is EqualUnmodifiableListView) return _certificates;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_certificates);
  }

  @override
  final String? resume;

  @override
  String toString() {
    return 'DocumentsInfoModel(documents: $documents, licenses: $licenses, certificates: $certificates, resume: $resume)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$DocumentsInfoModelImpl &&
            const DeepCollectionEquality().equals(
              other._documents,
              _documents,
            ) &&
            const DeepCollectionEquality().equals(other._licenses, _licenses) &&
            const DeepCollectionEquality().equals(
              other._certificates,
              _certificates,
            ) &&
            (identical(other.resume, resume) || other.resume == resume));
  }

  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  int get hashCode => Object.hash(
    runtimeType,
    const DeepCollectionEquality().hash(_documents),
    const DeepCollectionEquality().hash(_licenses),
    const DeepCollectionEquality().hash(_certificates),
    resume,
  );

  /// Create a copy of DocumentsInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$DocumentsInfoModelImplCopyWith<_$DocumentsInfoModelImpl> get copyWith =>
      __$$DocumentsInfoModelImplCopyWithImpl<_$DocumentsInfoModelImpl>(
        this,
        _$identity,
      );

  @override
  Map<String, dynamic> toJson() {
    return _$$DocumentsInfoModelImplToJson(this);
  }
}

abstract class _DocumentsInfoModel extends DocumentsInfoModel {
  const factory _DocumentsInfoModel({
    required final List<String> documents,
    required final List<String> licenses,
    required final List<String> certificates,
    final String? resume,
  }) = _$DocumentsInfoModelImpl;
  const _DocumentsInfoModel._() : super._();

  factory _DocumentsInfoModel.fromJson(Map<String, dynamic> json) =
      _$DocumentsInfoModelImpl.fromJson;

  @override
  List<String> get documents;
  @override
  List<String> get licenses;
  @override
  List<String> get certificates;
  @override
  String? get resume;

  /// Create a copy of DocumentsInfoModel
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$DocumentsInfoModelImplCopyWith<_$DocumentsInfoModelImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
