// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'section_state.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$SectionState<T> {





@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is SectionState<T>);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'SectionState<$T>()';
}


}

/// @nodoc
class $SectionStateCopyWith<T,$Res>  {
$SectionStateCopyWith(SectionState<T> _, $Res Function(SectionState<T>) __);
}


/// Adds pattern-matching-related methods to [SectionState].
extension SectionStatePatterns<T> on SectionState<T> {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>({TResult Function( _Initial<T> value)?  initial,TResult Function( _Editing<T> value)?  editing,TResult Function( _Validating<T> value)?  validating,TResult Function( _Valid<T> value)?  valid,TResult Function( _Invalid<T> value)?  invalid,required TResult orElse(),}){
final _that = this;
switch (_that) {
case _Initial() when initial != null:
return initial(_that);case _Editing() when editing != null:
return editing(_that);case _Validating() when validating != null:
return validating(_that);case _Valid() when valid != null:
return valid(_that);case _Invalid() when invalid != null:
return invalid(_that);case _:
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

@optionalTypeArgs TResult map<TResult extends Object?>({required TResult Function( _Initial<T> value)  initial,required TResult Function( _Editing<T> value)  editing,required TResult Function( _Validating<T> value)  validating,required TResult Function( _Valid<T> value)  valid,required TResult Function( _Invalid<T> value)  invalid,}){
final _that = this;
switch (_that) {
case _Initial():
return initial(_that);case _Editing():
return editing(_that);case _Validating():
return validating(_that);case _Valid():
return valid(_that);case _Invalid():
return invalid(_that);}
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>({TResult? Function( _Initial<T> value)?  initial,TResult? Function( _Editing<T> value)?  editing,TResult? Function( _Validating<T> value)?  validating,TResult? Function( _Valid<T> value)?  valid,TResult? Function( _Invalid<T> value)?  invalid,}){
final _that = this;
switch (_that) {
case _Initial() when initial != null:
return initial(_that);case _Editing() when editing != null:
return editing(_that);case _Validating() when validating != null:
return validating(_that);case _Valid() when valid != null:
return valid(_that);case _Invalid() when invalid != null:
return invalid(_that);case _:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>({TResult Function()?  initial,TResult Function( T data)?  editing,TResult Function()?  validating,TResult Function( T data)?  valid,TResult Function( T data,  String error)?  invalid,required TResult orElse(),}) {final _that = this;
switch (_that) {
case _Initial() when initial != null:
return initial();case _Editing() when editing != null:
return editing(_that.data);case _Validating() when validating != null:
return validating();case _Valid() when valid != null:
return valid(_that.data);case _Invalid() when invalid != null:
return invalid(_that.data,_that.error);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>({required TResult Function()  initial,required TResult Function( T data)  editing,required TResult Function()  validating,required TResult Function( T data)  valid,required TResult Function( T data,  String error)  invalid,}) {final _that = this;
switch (_that) {
case _Initial():
return initial();case _Editing():
return editing(_that.data);case _Validating():
return validating();case _Valid():
return valid(_that.data);case _Invalid():
return invalid(_that.data,_that.error);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>({TResult? Function()?  initial,TResult? Function( T data)?  editing,TResult? Function()?  validating,TResult? Function( T data)?  valid,TResult? Function( T data,  String error)?  invalid,}) {final _that = this;
switch (_that) {
case _Initial() when initial != null:
return initial();case _Editing() when editing != null:
return editing(_that.data);case _Validating() when validating != null:
return validating();case _Valid() when valid != null:
return valid(_that.data);case _Invalid() when invalid != null:
return invalid(_that.data,_that.error);case _:
  return null;

}
}

}

/// @nodoc


class _Initial<T> implements SectionState<T> {
  const _Initial();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _Initial<T>);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'SectionState<$T>.initial()';
}


}




/// @nodoc


class _Editing<T> implements SectionState<T> {
  const _Editing(this.data);
  

 final  T data;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$EditingCopyWith<T, _Editing<T>> get copyWith => __$EditingCopyWithImpl<T, _Editing<T>>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _Editing<T>&&const DeepCollectionEquality().equals(other.data, data));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(data));

@override
String toString() {
  return 'SectionState<$T>.editing(data: $data)';
}


}

/// @nodoc
abstract mixin class _$EditingCopyWith<T,$Res> implements $SectionStateCopyWith<T, $Res> {
  factory _$EditingCopyWith(_Editing<T> value, $Res Function(_Editing<T>) _then) = __$EditingCopyWithImpl;
@useResult
$Res call({
 T data
});




}
/// @nodoc
class __$EditingCopyWithImpl<T,$Res>
    implements _$EditingCopyWith<T, $Res> {
  __$EditingCopyWithImpl(this._self, this._then);

  final _Editing<T> _self;
  final $Res Function(_Editing<T>) _then;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') $Res call({Object? data = freezed,}) {
  return _then(_Editing<T>(
freezed == data ? _self.data : data // ignore: cast_nullable_to_non_nullable
as T,
  ));
}


}

/// @nodoc


class _Validating<T> implements SectionState<T> {
  const _Validating();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _Validating<T>);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'SectionState<$T>.validating()';
}


}




/// @nodoc


class _Valid<T> implements SectionState<T> {
  const _Valid(this.data);
  

 final  T data;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ValidCopyWith<T, _Valid<T>> get copyWith => __$ValidCopyWithImpl<T, _Valid<T>>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _Valid<T>&&const DeepCollectionEquality().equals(other.data, data));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(data));

@override
String toString() {
  return 'SectionState<$T>.valid(data: $data)';
}


}

/// @nodoc
abstract mixin class _$ValidCopyWith<T,$Res> implements $SectionStateCopyWith<T, $Res> {
  factory _$ValidCopyWith(_Valid<T> value, $Res Function(_Valid<T>) _then) = __$ValidCopyWithImpl;
@useResult
$Res call({
 T data
});




}
/// @nodoc
class __$ValidCopyWithImpl<T,$Res>
    implements _$ValidCopyWith<T, $Res> {
  __$ValidCopyWithImpl(this._self, this._then);

  final _Valid<T> _self;
  final $Res Function(_Valid<T>) _then;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') $Res call({Object? data = freezed,}) {
  return _then(_Valid<T>(
freezed == data ? _self.data : data // ignore: cast_nullable_to_non_nullable
as T,
  ));
}


}

/// @nodoc


class _Invalid<T> implements SectionState<T> {
  const _Invalid(this.data, this.error);
  

 final  T data;
 final  String error;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$InvalidCopyWith<T, _Invalid<T>> get copyWith => __$InvalidCopyWithImpl<T, _Invalid<T>>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _Invalid<T>&&const DeepCollectionEquality().equals(other.data, data)&&(identical(other.error, error) || other.error == error));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(data),error);

@override
String toString() {
  return 'SectionState<$T>.invalid(data: $data, error: $error)';
}


}

/// @nodoc
abstract mixin class _$InvalidCopyWith<T,$Res> implements $SectionStateCopyWith<T, $Res> {
  factory _$InvalidCopyWith(_Invalid<T> value, $Res Function(_Invalid<T>) _then) = __$InvalidCopyWithImpl;
@useResult
$Res call({
 T data, String error
});




}
/// @nodoc
class __$InvalidCopyWithImpl<T,$Res>
    implements _$InvalidCopyWith<T, $Res> {
  __$InvalidCopyWithImpl(this._self, this._then);

  final _Invalid<T> _self;
  final $Res Function(_Invalid<T>) _then;

/// Create a copy of SectionState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') $Res call({Object? data = freezed,Object? error = null,}) {
  return _then(_Invalid<T>(
freezed == data ? _self.data : data // ignore: cast_nullable_to_non_nullable
as T,null == error ? _self.error : error // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

/// @nodoc
mixin _$RegistrationFormState {

 int get currentStep; bool get isSubmitting; String? get errorMessage; String? get successMessage;
/// Create a copy of RegistrationFormState
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$RegistrationFormStateCopyWith<RegistrationFormState> get copyWith => _$RegistrationFormStateCopyWithImpl<RegistrationFormState>(this as RegistrationFormState, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is RegistrationFormState&&(identical(other.currentStep, currentStep) || other.currentStep == currentStep)&&(identical(other.isSubmitting, isSubmitting) || other.isSubmitting == isSubmitting)&&(identical(other.errorMessage, errorMessage) || other.errorMessage == errorMessage)&&(identical(other.successMessage, successMessage) || other.successMessage == successMessage));
}


@override
int get hashCode => Object.hash(runtimeType,currentStep,isSubmitting,errorMessage,successMessage);

@override
String toString() {
  return 'RegistrationFormState(currentStep: $currentStep, isSubmitting: $isSubmitting, errorMessage: $errorMessage, successMessage: $successMessage)';
}


}

/// @nodoc
abstract mixin class $RegistrationFormStateCopyWith<$Res>  {
  factory $RegistrationFormStateCopyWith(RegistrationFormState value, $Res Function(RegistrationFormState) _then) = _$RegistrationFormStateCopyWithImpl;
@useResult
$Res call({
 int currentStep, bool isSubmitting, String? errorMessage, String? successMessage
});




}
/// @nodoc
class _$RegistrationFormStateCopyWithImpl<$Res>
    implements $RegistrationFormStateCopyWith<$Res> {
  _$RegistrationFormStateCopyWithImpl(this._self, this._then);

  final RegistrationFormState _self;
  final $Res Function(RegistrationFormState) _then;

/// Create a copy of RegistrationFormState
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? currentStep = null,Object? isSubmitting = null,Object? errorMessage = freezed,Object? successMessage = freezed,}) {
  return _then(_self.copyWith(
currentStep: null == currentStep ? _self.currentStep : currentStep // ignore: cast_nullable_to_non_nullable
as int,isSubmitting: null == isSubmitting ? _self.isSubmitting : isSubmitting // ignore: cast_nullable_to_non_nullable
as bool,errorMessage: freezed == errorMessage ? _self.errorMessage : errorMessage // ignore: cast_nullable_to_non_nullable
as String?,successMessage: freezed == successMessage ? _self.successMessage : successMessage // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [RegistrationFormState].
extension RegistrationFormStatePatterns on RegistrationFormState {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _RegistrationFormState value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _RegistrationFormState() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _RegistrationFormState value)  $default,){
final _that = this;
switch (_that) {
case _RegistrationFormState():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _RegistrationFormState value)?  $default,){
final _that = this;
switch (_that) {
case _RegistrationFormState() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( int currentStep,  bool isSubmitting,  String? errorMessage,  String? successMessage)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _RegistrationFormState() when $default != null:
return $default(_that.currentStep,_that.isSubmitting,_that.errorMessage,_that.successMessage);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( int currentStep,  bool isSubmitting,  String? errorMessage,  String? successMessage)  $default,) {final _that = this;
switch (_that) {
case _RegistrationFormState():
return $default(_that.currentStep,_that.isSubmitting,_that.errorMessage,_that.successMessage);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( int currentStep,  bool isSubmitting,  String? errorMessage,  String? successMessage)?  $default,) {final _that = this;
switch (_that) {
case _RegistrationFormState() when $default != null:
return $default(_that.currentStep,_that.isSubmitting,_that.errorMessage,_that.successMessage);case _:
  return null;

}
}

}

/// @nodoc


class _RegistrationFormState implements RegistrationFormState {
  const _RegistrationFormState({required this.currentStep, required this.isSubmitting, this.errorMessage, this.successMessage});
  

@override final  int currentStep;
@override final  bool isSubmitting;
@override final  String? errorMessage;
@override final  String? successMessage;

/// Create a copy of RegistrationFormState
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$RegistrationFormStateCopyWith<_RegistrationFormState> get copyWith => __$RegistrationFormStateCopyWithImpl<_RegistrationFormState>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _RegistrationFormState&&(identical(other.currentStep, currentStep) || other.currentStep == currentStep)&&(identical(other.isSubmitting, isSubmitting) || other.isSubmitting == isSubmitting)&&(identical(other.errorMessage, errorMessage) || other.errorMessage == errorMessage)&&(identical(other.successMessage, successMessage) || other.successMessage == successMessage));
}


@override
int get hashCode => Object.hash(runtimeType,currentStep,isSubmitting,errorMessage,successMessage);

@override
String toString() {
  return 'RegistrationFormState(currentStep: $currentStep, isSubmitting: $isSubmitting, errorMessage: $errorMessage, successMessage: $successMessage)';
}


}

/// @nodoc
abstract mixin class _$RegistrationFormStateCopyWith<$Res> implements $RegistrationFormStateCopyWith<$Res> {
  factory _$RegistrationFormStateCopyWith(_RegistrationFormState value, $Res Function(_RegistrationFormState) _then) = __$RegistrationFormStateCopyWithImpl;
@override @useResult
$Res call({
 int currentStep, bool isSubmitting, String? errorMessage, String? successMessage
});




}
/// @nodoc
class __$RegistrationFormStateCopyWithImpl<$Res>
    implements _$RegistrationFormStateCopyWith<$Res> {
  __$RegistrationFormStateCopyWithImpl(this._self, this._then);

  final _RegistrationFormState _self;
  final $Res Function(_RegistrationFormState) _then;

/// Create a copy of RegistrationFormState
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? currentStep = null,Object? isSubmitting = null,Object? errorMessage = freezed,Object? successMessage = freezed,}) {
  return _then(_RegistrationFormState(
currentStep: null == currentStep ? _self.currentStep : currentStep // ignore: cast_nullable_to_non_nullable
as int,isSubmitting: null == isSubmitting ? _self.isSubmitting : isSubmitting // ignore: cast_nullable_to_non_nullable
as bool,errorMessage: freezed == errorMessage ? _self.errorMessage : errorMessage // ignore: cast_nullable_to_non_nullable
as String?,successMessage: freezed == successMessage ? _self.successMessage : successMessage // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on
