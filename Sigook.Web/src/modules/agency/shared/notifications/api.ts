import { api } from '@/app/security/apiService';
import type { NotificationsResponse } from '@/modules/agency/shared/notifications/types';

export function getNotifications(): Promise<NotificationsResponse> {
  return api.get<NotificationsResponse>('/api/agency/notifications');
}
