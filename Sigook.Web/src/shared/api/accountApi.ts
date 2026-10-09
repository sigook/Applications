import { api } from '@/app/security/apiService';
import type { ChangeEmailRequest, GetEmailResponse } from '@/shared/types/security';

export function changeEmail(model: ChangeEmailRequest): Promise<void> {
  return api.post('/api/account/ChangeEmail', model);
}

export function getEmail(): Promise<GetEmailResponse> {
  return api.get<GetEmailResponse>('/api/account/GetEmail');
}

export function deactivateAccount(): Promise<void> {
  return api.patch('/identity');
}
