import { api } from '@/app/security/apiService';

export function registerWorker(payload: FormData, requestId?: number): Promise<string> {
  return api.post<string>('/api/worker/profile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' },
    params: requestId ? { requestId } : undefined
  });
}
