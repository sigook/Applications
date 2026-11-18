import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/registration_form_model.dart';

abstract class RegistrationLocalDataSource {
  Future<void> saveDraft(RegistrationFormModel form);
  Future<RegistrationFormModel?> getDraft();
  Future<void> clearDraft();
}

class RegistrationLocalDataSourceImpl implements RegistrationLocalDataSource {
  final SharedPreferences sharedPreferences;
  static const String _draftKey = 'REGISTRATION_DRAFT';

  RegistrationLocalDataSourceImpl({required this.sharedPreferences});

  @override
  Future<void> saveDraft(RegistrationFormModel form) async {
    final jsonString = jsonEncode(_withoutPassword(form).toJson());
    await sharedPreferences.setString(_draftKey, jsonString);
  }

  RegistrationFormModel _withoutPassword(RegistrationFormModel form) {
    final accountInfo = form.accountInfo;
    if (accountInfo == null) return form;
    return form.copyWith(
      accountInfo: accountInfo.copyWith(password: '', confirmPassword: ''),
    );
  }

  @override
  Future<RegistrationFormModel?> getDraft() async {
    try {
      final jsonString = sharedPreferences.getString(_draftKey);
      if (jsonString == null) return null;

      final json = jsonDecode(jsonString) as Map<String, dynamic>;
      final form = RegistrationFormModel.fromJson(json);
      final accountInfo = form.accountInfo;
      if (accountInfo == null ||
          (accountInfo.password.isEmpty &&
              accountInfo.confirmPassword.isEmpty)) {
        return form;
      }
      await saveDraft(form);
      return _withoutPassword(form);
    } catch (e) {
      await clearDraft();
      return null;
    }
  }

  @override
  Future<void> clearDraft() async {
    await sharedPreferences.remove(_draftKey);
  }
}
