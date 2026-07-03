import { api } from '@/security/apiService';
import type { NotificationsResponse } from '@/types/notification';

export function getNotifications(): Promise<NotificationsResponse> {
  return api.get<NotificationsResponse>('/api/agency/Notifications');
}
