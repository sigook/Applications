import http from '@/security/apiService';
import type { NotificationsResponse } from '@/types/notification';

export function getNotifications(): Promise<NotificationsResponse> {
  return http.get('/api/agency/Notifications').then(r => r.data);
}
