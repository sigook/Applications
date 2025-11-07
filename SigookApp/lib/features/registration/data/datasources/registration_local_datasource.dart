import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/registration_form_model.dart';

/// Local data source for registration form data
abstract class RegistrationLocalDataSource {
  /// Saves registration form draft
  Future<void> saveDraft(RegistrationFormModel form);

  /// Retrieves saved draft
  Future<RegistrationFormModel?> getDraft();

  /// Clears saved draft
  Future<void> clearDraft();
}

class RegistrationLocalDataSourceImpl implements RegistrationLocalDataSource {
  final SharedPreferences sharedPreferences;
  static const String _draftKey = 'REGISTRATION_DRAFT';

  RegistrationLocalDataSourceImpl({required this.sharedPreferences});

  @override
  Future<void> saveDraft(RegistrationFormModel form) async {
    final jsonString = jsonEncode(form.toJson());
    await sharedPreferences.setString(_draftKey, jsonString);
  }

  @override
  Future<RegistrationFormModel?> getDraft() async {
    try {
      final jsonString = sharedPreferences.getString(_draftKey);
      if (jsonString == null) return null;

      final json = jsonDecode(jsonString) as Map<String, dynamic>;
      return RegistrationFormModel.fromJson(json);
    } catch (e) {
      // If draft is corrupted or incompatible (e.g., from old enum format),
      // clear it and return null
      await clearDraft();
      return null;
    }
  }

  @override
  Future<void> clearDraft() async {
    await sharedPreferences.remove(_draftKey);
  }
}
