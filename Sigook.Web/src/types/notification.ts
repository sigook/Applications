// One HTTP call returns every notification kind in a single payload, grouped by
// type. Mirrors backend Covenant.Common.Models.Notifications.NotificationsModel.
// New kinds: add a list here + a NotificationType + a mapper in useNotifications.
export type NotificationsResponse = Record<string, never>;

export enum NotificationType {}

export const NOTIFICATION_TYPE_LABELS: Record<NotificationType, string> = {};

// Detail page each notification type links to from the bell.
export const NOTIFICATION_TYPE_ROUTES: Record<NotificationType, string> = {};

// Generic, UI-agnostic notification rendered by the bell. Any kind maps to this.
export interface AppNotification {
  id: string;
  type: NotificationType;
  title: string;
  lines: string[];
  badge?: string;
  route?: string;
}

export interface NotificationGroup {
  type: NotificationType;
  label: string;
  route: string;
  items: AppNotification[];
}
