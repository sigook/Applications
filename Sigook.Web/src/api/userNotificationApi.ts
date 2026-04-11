import http from '@/security/apiService';
import type { UserNotificationItem } from '@/types/common';

export function getUserNotifications(): Promise<UserNotificationItem[]> {
  return http.get('/api/UserNotification').then(r => r.data);
}

export function updateUserNotification(model: UserNotificationItem): Promise<void> {
  return http.put('/api/UserNotification', model).then(() => {});
}
