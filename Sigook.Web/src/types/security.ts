export interface UserProfile {
  token_type: string;
  access_token: string;
  profile: UserClaims;
}

export interface UserClaims {
  email: string;
  role: string | string[];
  sub: string;
  name: string;
}

export enum UserRole {
  SuperAdmin = 'superadmin',
  Admin = 'admin',
  Recruiting = 'recruiting',
  Sales = 'sales',
  Company = 'company',
  CompanyUser = 'company.user',
  Worker = 'worker',
}

export interface TokenResponse {
  access_token: string;
  expires_in: number;
  token_type: string;
  refresh_token?: string;
  scope?: string;
}

export interface TokenErrorResponse {
  error: string;
  error_description?: string;
}

export interface UserInfoResponse {
  sub: string;
  role?: string | string[];
  [claim: string]: unknown;
}

export interface ResetPasswordWithCodePayload {
  email: string;
  code: string;
  newPassword: string;
}

export interface PasswordResetErrorResponse {
  error: string;
  messages?: string[];
}

export interface ChangeEmailRequest {
  newEmail: string;
  confirmNewEmail: string;
}

export interface GetEmailResponse {
  email: string;
}
