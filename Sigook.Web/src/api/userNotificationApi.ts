import { api } from '@/security/apiService';
import type { UserNotificationItem } from '@/types/common';

export function getUserNotifications(): Promise<UserNotificationItem[]> {
  return api.get<UserNotificationItem[]>('/api/UserNotification');
}

export function updateUserNotification(model: UserNotificationItem): Promise<void> {
  return api.put('/api/UserNotification', model);
}
