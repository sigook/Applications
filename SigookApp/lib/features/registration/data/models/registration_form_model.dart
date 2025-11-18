import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/registration_form.dart';
import 'basic_info_model.dart';
import 'preferences_info_model.dart';
import 'documents_info_model.dart';
import 'account_info_model.dart';

part 'registration_form_model.freezed.dart';
part 'registration_form_model.g.dart';

@freezed
class RegistrationFormModel with _$RegistrationFormModel {
  const RegistrationFormModel._();

  const factory RegistrationFormModel({
    BasicInfoModel? basicInfo,
    PreferencesInfoModel? preferencesInfo,
    DocumentsInfoModel? documentsInfo,
    AccountInfoModel? accountInfo,
  }) = _RegistrationFormModel;

  /// Convert from domain entity
  factory RegistrationFormModel.fromEntity(RegistrationForm entity) {
    return RegistrationFormModel(
      basicInfo: entity.basicInfo != null
          ? BasicInfoModel.fromEntity(entity.basicInfo!)
          : null,
      preferencesInfo: entity.preferencesInfo != null
          ? PreferencesInfoModel.fromEntity(entity.preferencesInfo!)
          : null,
      documentsInfo: entity.documentsInfo != null
          ? DocumentsInfoModel.fromEntity(entity.documentsInfo!)
          : null,
      accountInfo: entity.accountInfo != null
          ? AccountInfoModel.fromEntity(entity.accountInfo!)
          : null,
    );
  }

  /// Convert to domain entity
  RegistrationForm toEntity() {
    return RegistrationForm(
      basicInfo: basicInfo?.toEntity(),
      preferencesInfo: preferencesInfo?.toEntity(),
      documentsInfo: documentsInfo?.toEntity(),
      accountInfo: accountInfo?.toEntity(),
    );
  }

  /// Empty form
  factory RegistrationFormModel.empty() {
    return const RegistrationFormModel();
  }

  /// From JSON
  factory RegistrationFormModel.fromJson(Map<String, dynamic> json) =>
      _$RegistrationFormModelFromJson(json);
}
