import { api } from '@/app/security/apiService';
import type { UserNotificationItem } from '@/shared/types/common';

export function getUserNotifications(): Promise<UserNotificationItem[]> {
  return api.get<UserNotificationItem[]>('/api/usernotification');
}

export function updateUserNotification(model: UserNotificationItem): Promise<void> {
  return api.put('/api/usernotification', model);
}
