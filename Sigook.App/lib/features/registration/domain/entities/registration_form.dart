import 'package:equatable/equatable.dart';
import 'basic_info.dart';
import 'preferences_info.dart';
import 'documents_info.dart';
import 'account_info.dart';

class RegistrationForm extends Equatable {
  final BasicInfo? basicInfo;
  final PreferencesInfo? preferencesInfo;
  final DocumentsInfo? documentsInfo;
  final AccountInfo? accountInfo;

  const RegistrationForm({
    this.basicInfo,
    this.preferencesInfo,
    this.documentsInfo,
    this.accountInfo,
  });

  factory RegistrationForm.empty() {
    return const RegistrationForm();
  }

  bool get isComplete {
    final basicOk = basicInfo?.isValid ?? false;
    final docsOk = documentsInfo?.isValid ?? false;
    final accountOk = accountInfo?.isValid ?? false;
    final prefsOk = preferencesInfo == null || preferencesInfo!.isValid;
    return basicOk && docsOk && accountOk && prefsOk;
  }

  bool get isBasicInfoComplete => basicInfo?.isValid ?? false;
  bool get isPreferencesInfoComplete => preferencesInfo?.isValid ?? false;
  bool get isDocumentsInfoComplete => documentsInfo?.isValid ?? false;
  bool get isAccountInfoComplete => accountInfo?.isValid ?? false;

  double get completionPercentage {
    int completed = 0;
    if (isBasicInfoComplete) completed++;
    if (isPreferencesInfoComplete) completed++;
    if (isDocumentsInfoComplete) completed++;
    if (isAccountInfoComplete) completed++;
    return completed / 4;
  }

  RegistrationForm copyWith({
    BasicInfo? basicInfo,
    PreferencesInfo? preferencesInfo,
    DocumentsInfo? documentsInfo,
    AccountInfo? accountInfo,
  }) {
    return RegistrationForm(
      basicInfo: basicInfo ?? this.basicInfo,
      preferencesInfo: preferencesInfo ?? this.preferencesInfo,
      documentsInfo: documentsInfo ?? this.documentsInfo,
      accountInfo: accountInfo ?? this.accountInfo,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      if (basicInfo != null) 'basicInfo': basicInfo!.toJson(),
      if (preferencesInfo != null) 'preferencesInfo': preferencesInfo!.toJson(),
      if (documentsInfo != null) 'documentsInfo': documentsInfo!.toJson(),
      if (accountInfo != null) 'accountInfo': accountInfo!.toJson(),
    };
  }

  @override
  List<Object?> get props => [
    basicInfo,
    preferencesInfo,
    documentsInfo,
    accountInfo,
  ];
}
