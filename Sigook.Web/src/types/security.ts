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

export interface ChangeEmailRequest {
  newEmail: string;
  confirmNewEmail: string;
}

export interface GetEmailResponse {
  email: string;
}
