import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import TimeSheetsTab from '@/modules/worker/activity/components/TimeSheetsTab.vue';
import { getWorkerWeekTimeSheets } from '@/modules/worker/activity/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { WorkerTimeSheetStatus, type WorkerWeekTimeSheet, type WorkerWeekTimeSheets } from '@/modules/worker/activity/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/modules/worker/activity/api', () => ({ getWorkerWeekTimeSheets: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const { sheetStub } = vi.hoisted(() => ({
  sheetStub: { name: 'TimeSheetDetailSheet', props: { modelValue: Boolean, entry: Object }, emits: ['update:modelValue'], template: '<div class="detail-sheet" />' },
}));

vi.mock('@/modules/worker/activity/components/TimeSheetDetailSheet.vue', () => ({ default: sheetStub }));

const punched: WorkerWeekTimeSheet = {
  id: 't1',
  requestId: 'r1',
  date: '2026-06-01T00:00:00',
  clientName: 'Acme',
  jobTitle: 'Picker',
  clockIn: '2026-06-01T07:00:00',
  clockOut: '2026-06-01T15:30:00',
  durationBreak: '00:30',
  breakIsPaid: false,
  regularHours: 8,
  overtimeHours: 0,
  holidayHours: 0,
  totalHours: 8,
  status: WorkerTimeSheetStatus.Approved,
  missingHours: '00:00',
  missingHoursOvertime: '00:00',
  bonusOrOthers: 0,
  deductionsOthers: 0,
  reimbursements: 0,
};

const manual: WorkerWeekTimeSheet = { ...punched, id: 't2', date: '2026-06-02T00:00:00', clockIn: null, clockOut: null, durationBreak: '00:00', overtimeHours: 1.5, status: WorkerTimeSheetStatus.Open };

function week(items: WorkerWeekTimeSheet[] = [punched, manual]): WorkerWeekTimeSheets {
  return { weekStart: '2026-05-31', weekEnd: '2026-06-06', hours: { total: 17.5, regular: 16, overtime: 1.5, holiday: 0, pendingApproval: 0 }, items };
}

async function setup() {
  const wrapper = mountWithPlugins(TimeSheetsTab);
  await flushPromises();
  return wrapper;
}

function rowCells(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findAll('tbody tr').map((r) => r.findAll('td').map((c) => c.text()));
}

beforeEach(() => {
  touch.value = false;
  vi.useFakeTimers({ now: new Date('2026-06-03T10:00:00'), toFake: ['Date'] });
  vi.mocked(getWorkerWeekTimeSheets).mockResolvedValue(week());
});

afterEach(() => {
  vi.useRealTimers();
});

describe('TimeSheetsTab', () => {
  it('loads the current week', async () => {
    const wrapper = await setup();
    expect(getWorkerWeekTimeSheets).toHaveBeenCalledWith('2026-05-31');
    expect(wrapper.find('.week-label').text()).toBe('May 31 – Jun 6, 2026');
    expect(wrapper.find('.week-total').text()).toBe('Week total: 17.5 h');
    expect(rowCells(wrapper)).toEqual([
      ['Mon, Jun 1', 'Acme · Picker', '07:00', '15:30', '30 min', '8.0', '—', '8.0', 'Approved'],
      ['Tue, Jun 2', 'Acme · Picker', 'Manual entry', '—', '—', '8.0', '1.5', '8.0', 'No clock-out'],
    ]);
    expect(wrapper.find('button[aria-label="Next week"]').attributes('disabled')).toBeDefined();
  });

  it('moves between weeks', async () => {
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Previous week"]').trigger('click');
    await flushPromises();
    expect(getWorkerWeekTimeSheets).toHaveBeenLastCalledWith('2026-05-24');
    expect(wrapper.find('.week-label').text()).toBe('May 24 – May 30, 2026');
    expect(wrapper.find('button[aria-label="Next week"]').attributes('disabled')).toBeUndefined();

    await wrapper.find('button[aria-label="Next week"]').trigger('click');
    await flushPromises();
    expect(getWorkerWeekTimeSheets).toHaveBeenLastCalledWith('2026-05-31');
  });

  it('opens the detail of a clicked day', async () => {
    const wrapper = await setup();
    await wrapper.find('tbody tr').trigger('click');
    await nextTick();
    expect(wrapper.findComponent(sheetStub).props()).toEqual({ modelValue: true, entry: punched });
  });

  it('reports a failed load and shows an empty week', async () => {
    vi.mocked(getWorkerWeekTimeSheets).mockRejectedValue('down');
    const wrapper = await setup();
    expect(showAlertError).toHaveBeenCalledWith('down');
    expect(wrapper.text()).toContain('No hours recorded this week');
    expect(wrapper.find('.week-total').text()).toBe('Week total: 0.0 h');
  });

  it('shows cards on touch screens and opens their detail', async () => {
    touch.value = true;
    const wrapper = await setup();
    const cards = wrapper.findAll('.rcard');
    expect(cards).toHaveLength(2);
    expect(cards[0].text()).toContain('07:00 – 15:30');
    expect(cards[1].text()).toContain('Manual entry');
    expect(cards[1].find('.tag').text()).toBe('No clock-out');
    await cards[1].trigger('click');
    expect(wrapper.findComponent(sheetStub).props('entry')).toEqual(manual);
  });
});
