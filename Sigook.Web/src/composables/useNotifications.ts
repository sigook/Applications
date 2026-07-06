import { ref, computed, type Ref, type ComputedRef } from 'vue';
import { getNotifications } from '@/api/notificationApi';
import {
  NotificationType,
  NOTIFICATION_TYPE_LABELS,
  NOTIFICATION_TYPE_ROUTES,
  type AppNotification,
  type NotificationGroup,
  type NotificationsResponse,
} from '@/types/notification';

const FOLLOW_UP_DAYS = 3;

// Maps each typed list of the backend payload into generic AppNotification[].
// Adding a new kind = add a mapper entry; the bell renders the result unchanged.
function mapResponse(response: NotificationsResponse): AppNotification[] {
  const workersToReview: AppNotification[] = response.workersToReview.map((r): AppNotification => ({
    id: r.runnerId,
    type: NotificationType.WorkerAttendanceReview,
    title: r.workerName,
    lines: [`#${r.requestNumberId} · ${r.companyName}`, r.jobTitle],
    badge: `Day ${r.dayNumber} of ${FOLLOW_UP_DAYS}`,
    route: `/recruiting/requests/${r.requestId}?tab=PunchCard`,
  }));

  return [...workersToReview];
}

export function useNotifications(): {
  notifications: Ref<AppNotification[]>;
  grouped: ComputedRef<NotificationGroup[]>;
  hasNotifications: ComputedRef<boolean>;
  load: () => Promise<void>;
} {
  const notifications = ref<AppNotification[]>([]);

  const grouped = computed<NotificationGroup[]>(() => {
    const groups = new Map<NotificationType, AppNotification[]>();
    for (const notification of notifications.value) {
      const bucket = groups.get(notification.type) ?? [];
      bucket.push(notification);
      groups.set(notification.type, bucket);
    }
    return [...groups.entries()].map(([type, items]) => ({
      type,
      label: NOTIFICATION_TYPE_LABELS[type],
      route: NOTIFICATION_TYPE_ROUTES[type],
      items,
    }));
  });

  const hasNotifications = computed(() => notifications.value.length > 0);

  async function load(): Promise<void> {
    try {
      const response = await getNotifications();
      notifications.value = mapResponse(response);
    } catch {
      notifications.value = [];
    }
  }

  return { notifications, grouped, hasNotifications, load };
}
