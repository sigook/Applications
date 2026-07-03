import { api } from '@/security/apiService';
import type { UnsubscribeRequest } from '@/types/common';

export async function unsubscribe(model: UnsubscribeRequest): Promise<void> {
  await api.post('/api/EmailPreferences/Unsubscribe', model);
}
