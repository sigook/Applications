import { api } from '@/app/security/apiService';
import type { ChangeEmailRequest, AccountResponse } from '@/shared/types/security';

export function changeEmail(model: ChangeEmailRequest): Promise<void> {
  return api.post('/api/account/ChangeEmail', model);
}

export function getAccount(): Promise<AccountResponse> {
  return api.get<AccountResponse>('/api/account');
}

export function deactivateAccount(): Promise<void> {
  return api.patch('/identity');
}
