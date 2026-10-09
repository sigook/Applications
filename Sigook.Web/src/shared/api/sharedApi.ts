import { api } from '@/app/security/apiService';
import type { UnsubscribeRequest } from '@/shared/types/common';

export async function unsubscribe(model: UnsubscribeRequest): Promise<void> {
  await api.post('/api/emailpreferences/Unsubscribe', model);
}
