import { describe, expect, it } from 'vitest';
import { authErrorMessage, DEFAULT_AUTH_ERROR } from '@/app/security/authErrors';

describe('authErrorMessage', () => {
  it.each([
    ['invalid_credentials', 'Invalid email or password.'],
    ['inactive_user', 'Your account is inactive. Please contact support.'],
    ['email_not_confirmed', 'Your email address has not been confirmed yet.'],
    ['locked_out', 'Too many failed attempts. Try again in a few minutes.'],
    ['invalid_code', 'Invalid code. Please check it and try again.'],
    ['code_expired', 'The code has expired. Request a new one.'],
    ['too_many_attempts', 'Too many attempts. Request a new code.'],
    ['password_policy', 'The new password was rejected.'],
    ['invalid_token', 'This link is invalid or has expired.'],
    ['invalid_user', 'Your Microsoft 365 account is not allowed to sign in.'],
  ])('%s', (code, message) => {
    expect(authErrorMessage(code)).toBe(message);
  });

  it.each([undefined, '', 'unknown_code', 'INVALID_CREDENTIALS', 'toString', 'constructor', 'hasOwnProperty', '__proto__'])('falls back for %s', (code) => {
    expect(authErrorMessage(code)).toBe(DEFAULT_AUTH_ERROR);
  });

  it('has a generic default message', () => {
    expect(DEFAULT_AUTH_ERROR).toBe('Something went wrong. Please try again.');
  });
});
