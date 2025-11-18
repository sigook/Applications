import 'package:equatable/equatable.dart';
import 'basic_info.dart';
import 'preferences_info.dart';
import 'documents_info.dart';
import 'account_info.dart';

/// Complete registration form entity
/// Aggregates all form sections (new 4-section structure)
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

  /// Empty form constructor
  factory RegistrationForm.empty() {
    return const RegistrationForm();
  }

  /// Validates if all required sections are complete and valid
  /// Preferences are optional: the user may skip them entirely.
  bool get isComplete {
    final basicOk = basicInfo?.isValid ?? false;
    final docsOk = documentsInfo?.isValid ?? false;
    final accountOk = accountInfo?.isValid ?? false;
    // Preferences are optional: either not provided or valid if present
    final prefsOk = preferencesInfo == null || preferencesInfo!.isValid;
    return basicOk && docsOk && accountOk && prefsOk;
  }

  /// Checks which sections are completed
  bool get isBasicInfoComplete => basicInfo?.isValid ?? false;
  // Preferences section is optional: only mark complete when valid data exists
  bool get isPreferencesInfoComplete => preferencesInfo?.isValid ?? false;
  bool get isDocumentsInfoComplete => documentsInfo?.isValid ?? false;
  bool get isAccountInfoComplete => accountInfo?.isValid ?? false;

  /// Calculates completion percentage
  double get completionPercentage {
    int completed = 0;
    if (isBasicInfoComplete) completed++;
    if (isPreferencesInfoComplete) completed++;
    if (isDocumentsInfoComplete) completed++;
    if (isAccountInfoComplete) completed++;
    return completed / 4;
  }

  /// Creates a copy with updated fields
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

  @override
  List<Object?> get props => [
    basicInfo,
    preferencesInfo,
    documentsInfo,
    accountInfo,
  ];
}
