import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import HistoryTab from '@/modules/worker/activity/components/HistoryTab.vue';
import { getWorkerHistory } from '@/modules/worker/activity/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerAssignmentListItem } from '@/modules/worker/activity/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/modules/worker/activity/api', () => ({ getWorkerHistory: vi.fn() }));

const completed: WorkerAssignmentListItem = {
  id: 'a1',
  numberId: 77,
  jobTitle: 'Picker',
  clientName: 'Acme',
  location: 'Toronto',
  startAt: '2026-04-01T12:00:00',
  startWorking: '2026-04-02T12:00:00',
  endedAt: '2026-04-30T12:00:00',
  hoursWorked: 120,
  status: 'Completed',
};

const ended: WorkerAssignmentListItem = { ...completed, id: 'a2', numberId: 78, startWorking: null, endedAt: null, hoursWorked: 0, status: 'Ended' };

async function setup() {
  vi.mocked(getWorkerHistory).mockResolvedValue({ items: [completed, ended], totalItems: 2, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(HistoryTab);
  await flushPromises();
  return wrapper;
}

beforeEach(() => {
  touch.value = false;
});

describe('HistoryTab', () => {
  it('loads past assignments', async () => {
    const wrapper = await setup();
    expect(getWorkerHistory).toHaveBeenCalledWith(expect.objectContaining({ pageIndex: 1, pageSize: 30, status: 'Past' }));
    const rows = wrapper.findAll('tbody tr').map((r) => r.findAll('td').map((c) => c.text()));
    expect(rows[0]).toEqual(['#77', 'Acme', 'Picker', 'Toronto', '02-Apr-2026', '30-Apr-2026', '120.0', 'Completed']);
    expect(rows[1]).toEqual(['#78', 'Acme', 'Picker', 'Toronto', '01-Apr-2026', '—', '0.0', 'Ended']);
    expect(wrapper.find('tbody .tag').classes()).toContain('is-success');
  });

  it('filters by status', async () => {
    const wrapper = await setup();
    await wrapper.find('select[aria-label="Status"]').setValue('Ended');
    await flushPromises();
    expect(getWorkerHistory).toHaveBeenLastCalledWith(expect.objectContaining({ status: 'Ended', pageIndex: 1 }));
  });

  it('shows cards on touch screens', async () => {
    touch.value = true;
    const wrapper = await setup();
    const cards = wrapper.findAll('.rcard');
    expect(cards).toHaveLength(2);
    expect(cards[0].find('.rcard__sub').text()).toBe('Acme · #77');
    expect(cards[1].text()).toContain('01-Apr-2026 – —');
  });
});
