import { beforeEach, describe, expect, it, vi } from 'vitest';
import { effectScope } from 'vue';
import { useNotifications } from '@/modules/agency/shared/notifications/useNotifications';
import { getNotifications } from '@/modules/agency/shared/notifications/api';
import { NotificationType, type NotificationsResponse } from '@/modules/agency/shared/notifications/types';

vi.mock('@/modules/agency/shared/notifications/api', () => ({ getNotifications: vi.fn() }));

const response: NotificationsResponse = {
  pendingReviewRequests: [
    { requestId: 'r1', numberId: 101, jobTitle: 'Forklift Operator', companyName: 'Acme', createdAt: '2026-01-01' },
    { requestId: 'r2', numberId: 102, jobTitle: 'Picker', companyName: 'Globex', createdAt: '2026-01-02' },
  ],
};

function setup() {
  const scope = effectScope();
  const state = scope.run(() => useNotifications());
  if (!state) throw new Error('scope did not run');
  return state;
}

beforeEach(() => {
  vi.clearAllMocks();
});

describe('useNotifications', () => {
  it('starts empty', () => {
    const { notifications, grouped, hasNotifications } = setup();

    expect(notifications.value).toEqual([]);
    expect(grouped.value).toEqual([]);
    expect(hasNotifications.value).toBe(false);
  });

  it('maps pending review requests into notifications', async () => {
    vi.mocked(getNotifications).mockResolvedValue(response);
    const { notifications, hasNotifications, load } = setup();

    await load();

    expect(notifications.value).toEqual([
      {
        id: 'r1',
        type: NotificationType.PendingReviewRequest,
        title: '#101 · Forklift Operator',
        lines: ['Acme'],
        route: '/recruiting/requests/r1',
      },
      {
        id: 'r2',
        type: NotificationType.PendingReviewRequest,
        title: '#102 · Picker',
        lines: ['Globex'],
        route: '/recruiting/requests/r2',
      },
    ]);
    expect(hasNotifications.value).toBe(true);
  });

  it('groups notifications by type with label and route', async () => {
    vi.mocked(getNotifications).mockResolvedValue(response);
    const { grouped, load } = setup();

    await load();

    expect(grouped.value).toHaveLength(1);
    expect(grouped.value[0]).toMatchObject({
      type: NotificationType.PendingReviewRequest,
      label: 'Requests to review',
      route: '/recruiting/requests?status=pending-review',
    });
    expect(grouped.value[0]?.items.map((n) => n.id)).toEqual(['r1', 'r2']);
  });

  it('has no notifications when the backend returns empty lists', async () => {
    vi.mocked(getNotifications).mockResolvedValue({ pendingReviewRequests: [] });
    const { grouped, hasNotifications, load } = setup();

    await load();

    expect(grouped.value).toEqual([]);
    expect(hasNotifications.value).toBe(false);
  });

  it('clears previous notifications when loading fails', async () => {
    vi.mocked(getNotifications).mockResolvedValueOnce(response).mockRejectedValueOnce(new Error('offline'));
    const { notifications, hasNotifications, load } = setup();

    await load();
    expect(notifications.value).toHaveLength(2);

    await expect(load()).resolves.toBeUndefined();
    expect(notifications.value).toEqual([]);
    expect(hasNotifications.value).toBe(false);
  });
});
