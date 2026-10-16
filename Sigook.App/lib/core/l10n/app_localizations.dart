import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class AppLocalizations {
  final Locale locale;

  AppLocalizations(this.locale);

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  Map<String, dynamic>? _localizedStrings;

  Future<bool> load() async {
    String jsonString = await rootBundle.loadString(
      'assets/i18n/app_${locale.languageCode}.json',
    );
    Map<String, dynamic> jsonMap = json.decode(jsonString);

    _localizedStrings = jsonMap;
    return true;
  }

  String translate(String key) {
    if (_localizedStrings == null) return key;

    final keys = key.split('.');
    dynamic value = _localizedStrings;

    for (final k in keys) {
      if (value is Map) {
        value = value[k];
      } else {
        return key;
      }
    }

    return value?.toString() ?? key;
  }

  String get appName => translate('app.name');
  String get welcome => translate('common.welcome');
  String get logout => translate('common.logout');
  String get save => translate('common.save');
  String get cancel => translate('common.cancel');
  String get retry => translate('common.retry');
  String get loading => translate('common.loading');
  String get error => translate('common.error');
  String get success => translate('common.success');

  String get signIn => translate('auth.signIn');
  String get signOut => translate('auth.signOut');
  String get email => translate('auth.email');
  String get password => translate('auth.password');

  String get jobs => translate('navigation.jobs');
  String get profile => translate('navigation.profile');
  String get settings => translate('navigation.settings');

  String get firstName => translate('profile.firstName');
  String get lastName => translate('profile.lastName');
  String get phoneNumber => translate('profile.phoneNumber');
  String get address => translate('profile.address');
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  bool isSupported(Locale locale) {
    return ['en', 'es', 'fr'].contains(locale.languageCode);
  }

  @override
  Future<AppLocalizations> load(Locale locale) async {
    AppLocalizations localizations = AppLocalizations(locale);
    await localizations.load();
    return localizations;
  }

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}
