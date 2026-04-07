import http from '@/security/apiService';
import type { UnsubscribeRequest } from '@/types/common';

export async function unsubscribe(model: UnsubscribeRequest): Promise<void> {
  await http.post('/api/EmailPreferences/Unsubscribe', model);
}
