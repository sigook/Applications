class AuthErrorCodes {
  AuthErrorCodes._();

  static const invalidCredentials = 'invalid_credentials';
  static const inactiveUser = 'inactive_user';
  static const emailNotConfirmed = 'email_not_confirmed';
  static const lockedOut = 'locked_out';
  static const invalidCode = 'invalid_code';
  static const codeExpired = 'code_expired';
  static const tooManyAttempts = 'too_many_attempts';
  static const passwordPolicy = 'password_policy';
}
