import 'dart:convert';
import 'package:flutter/services.dart';

class ErrorMessages {
  ErrorMessages._();

  static Map<String, dynamic>? _messages;

  static Future<void> load() async {
    final jsonString = await rootBundle.loadString(
      'assets/i18n/error_messages_en.json',
    );
    _messages = json.decode(jsonString);
  }

  static String get networkError =>
      _messages?['network']?['error'] ?? 'Network error';
  static String get connectionTimeout =>
      _messages?['network']?['timeout'] ?? 'Connection timeout';
  static String get serverError =>
      _messages?['network']?['server_error'] ?? 'Server error';
  static String get unknownError =>
      _messages?['network']?['unknown'] ?? 'Unknown error';

  static String get authenticationFailed =>
      _messages?['authentication']?['failed'] ?? 'Authentication failed';
  static String get tokenExpired =>
      _messages?['authentication']?['token_expired'] ?? 'Token expired';
  static String get userCancelled =>
      _messages?['authentication']?['user_cancelled'] ?? 'User cancelled';
  static String get invalidCredentials =>
      _messages?['authentication']?['invalid_credentials'] ??
      'Invalid credentials';

  static String get requiredField =>
      _messages?['validation']?['required_field'] ?? 'Required field';
  static String get invalidEmail =>
      _messages?['validation']?['invalid_email'] ?? 'Invalid email';
  static String get invalidPhone =>
      _messages?['validation']?['invalid_phone'] ?? 'Invalid phone';
  static String get invalidDate =>
      _messages?['validation']?['invalid_date'] ?? 'Invalid date';
  static String get passwordTooShort =>
      _messages?['validation']?['password_too_short'] ?? 'Password too short';
  static String get passwordMismatch =>
      _messages?['validation']?['password_mismatch'] ??
      'Passwords do not match';

  static String get locationPermissionDenied =>
      _messages?['permission']?['location_denied'] ??
      'Location permission denied';
  static String get cameraPermissionDenied =>
      _messages?['permission']?['camera_denied'] ?? 'Camera permission denied';
  static String get storagePermissionDenied =>
      _messages?['permission']?['storage_denied'] ??
      'Storage permission denied';

  static String get dataNotFound =>
      _messages?['data']?['not_found'] ?? 'Data not found';
  static String get loadDataFailed =>
      _messages?['data']?['load_failed'] ?? 'Load failed';
  static String get saveDataFailed =>
      _messages?['data']?['save_failed'] ?? 'Save failed';
  static String get deleteDataFailed =>
      _messages?['data']?['delete_failed'] ?? 'Delete failed';

  static String get filePickFailed =>
      _messages?['file']?['pick_failed'] ?? 'File pick failed';
  static String get fileUploadFailed =>
      _messages?['file']?['upload_failed'] ?? 'File upload failed';
  static String get fileTooLarge =>
      _messages?['file']?['too_large'] ?? 'File too large';
  static String get invalidFileType =>
      _messages?['file']?['invalid_type'] ?? 'Invalid file type';

  static String get locationServiceDisabled =>
      _messages?['location']?['service_disabled'] ??
      'Location service disabled';
  static String get locationUnavailable =>
      _messages?['location']?['unavailable'] ?? 'Location unavailable';

  static String get jobNotFound =>
      _messages?['job']?['not_found'] ?? 'Job not found';
  static String get applicationFailed =>
      _messages?['job']?['application_failed'] ?? 'Application failed';
  static String get timesheetSubmitFailed =>
      _messages?['job']?['timesheet_submit_failed'] ??
      'Timesheet submit failed';

  static String get profileLoadFailed =>
      _messages?['profile']?['load_failed'] ?? 'Profile load failed';
  static String get profileUpdateFailed =>
      _messages?['profile']?['update_failed'] ?? 'Profile update failed';

  static String get registrationFailed =>
      _messages?['registration']?['failed'] ?? 'Registration failed';
  static String get registrationDataLoadFailed =>
      _messages?['registration']?['data_load_failed'] ??
      'Registration data load failed';

  static String fromException(Exception e) {
    final message = e.toString().toLowerCase();

    if (message.contains('network') || message.contains('connection')) {
      return networkError;
    } else if (message.contains('timeout')) {
      return connectionTimeout;
    } else if (message.contains('server')) {
      return serverError;
    } else if (message.contains('auth')) {
      return authenticationFailed;
    } else if (message.contains('permission')) {
      return locationPermissionDenied;
    }

    return unknownError;
  }
}
