import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sigook_app_flutter/features/registration/data/datasources/registration_local_datasource.dart';
import 'package:sigook_app_flutter/features/registration/data/models/account_info_model.dart';
import 'package:sigook_app_flutter/features/registration/data/models/registration_form_model.dart';

void main() {
  const draftKey = 'REGISTRATION_DRAFT';
  const form = RegistrationFormModel(
    accountInfo: AccountInfoModel(
      email: 'worker@test.com',
      password: 'Secret123',
      confirmPassword: 'Secret123',
      termsAccepted: true,
    ),
  );

  Future<(RegistrationLocalDataSourceImpl, SharedPreferences)> build([
    Map<String, Object> values = const {},
  ]) async {
    SharedPreferences.setMockInitialValues(values);
    final preferences = await SharedPreferences.getInstance();
    return (
      RegistrationLocalDataSourceImpl(sharedPreferences: preferences),
      preferences,
    );
  }

  test('saveDraft never persists the password', () async {
    final (dataSource, preferences) = await build();

    await dataSource.saveDraft(form);

    final stored = preferences.getString(draftKey)!;
    expect(stored.contains('Secret123'), isFalse);
    expect(stored.contains('worker@test.com'), isTrue);
  });

  test('getDraft returns the draft with an empty password', () async {
    final (dataSource, _) = await build();
    await dataSource.saveDraft(form);

    final draft = await dataSource.getDraft();

    expect(draft!.accountInfo!.email, 'worker@test.com');
    expect(draft.accountInfo!.password, isEmpty);
    expect(draft.accountInfo!.confirmPassword, isEmpty);
    expect(draft.accountInfo!.termsAccepted, isTrue);
  });

  test('getDraft removes a password stored by an older version', () async {
    final legacy = jsonEncode({
      'accountInfo': {
        'email': 'worker@test.com',
        'password': 'Secret123',
        'confirmPassword': 'Secret123',
        'termsAccepted': true,
      },
    });
    final (dataSource, preferences) = await build({draftKey: legacy});

    final draft = await dataSource.getDraft();

    expect(draft!.accountInfo!.password, isEmpty);
    expect(preferences.getString(draftKey)!.contains('Secret123'), isFalse);
  });

  test('getDraft returns null when there is no draft', () async {
    final (dataSource, _) = await build();

    expect(await dataSource.getDraft(), isNull);
  });
}
