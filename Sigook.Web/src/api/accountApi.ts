import { api } from '@/security/apiService';
import type { ChangeEmailRequest, GetEmailResponse } from '@/types/security';

export function changeEmail(model: ChangeEmailRequest): Promise<void> {
  return api.post('/api/Account/ChangeEmail', model);
}

export function getEmail(): Promise<GetEmailResponse> {
  return api.get<GetEmailResponse>('/api/Account/GetEmail');
}

export function deactivateAccount(): Promise<void> {
  return api.patch('/identity');
}
