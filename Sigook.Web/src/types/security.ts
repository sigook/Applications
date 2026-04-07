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
  Agency = 'agency',
  Company = 'company',
  Worker = 'worker',
  CompanyUser = 'company.user',
}

export interface ChangeEmailRequest {
  newEmail: string;
  confirmNewEmail: string;
}

export interface GetEmailResponse {
  email: string;
}
