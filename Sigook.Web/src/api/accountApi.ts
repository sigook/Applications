import http from '@/security/apiService';
import type { ChangeEmailRequest, GetEmailResponse } from '@/types/security';

export function changeEmail(model: ChangeEmailRequest): Promise<void> {
  return http.post('/api/Account/ChangeEmail', model).then(() => {});
}

export function getEmail(): Promise<GetEmailResponse> {
  return http.get('/api/Account/GetEmail').then(r => r.data);
}

export function deactivateAccount(): Promise<void> {
  return http.patch('/identity').then(() => {});
}
