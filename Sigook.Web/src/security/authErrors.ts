const AUTH_ERROR_MESSAGES: Record<string, string> = {
  invalid_credentials: "Invalid email or password.",
  inactive_user: "Your account is inactive. Please contact support.",
  email_not_confirmed: "Your email address has not been confirmed yet.",
  locked_out: "Too many failed attempts. Try again in a few minutes.",
  invalid_code: "Invalid code. Please check it and try again.",
  code_expired: "The code has expired. Request a new one.",
  too_many_attempts: "Too many attempts. Request a new code.",
  password_policy: "The new password was rejected.",
};

export const DEFAULT_AUTH_ERROR = "Something went wrong. Please try again.";

export function authErrorMessage(code: string | undefined): string {
  if (!code) return DEFAULT_AUTH_ERROR;
  return AUTH_ERROR_MESSAGES[code] ?? DEFAULT_AUTH_ERROR;
}
