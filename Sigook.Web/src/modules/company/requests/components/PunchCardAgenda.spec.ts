import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import PunchCardAgenda from '@/modules/company/requests/components/PunchCardAgenda.vue';
import { WorkerRequestStatus } from '@/shared/constants/enums';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardDay, PunchCardWorker } from '@/shared/punch-card/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface Props {
  highlights?: PunchCardDay[];
  startDate?: string | null;
  worker?: PunchCardWorker;
  errors?: Record<string, string>;
}

const approved: PunchCardDay = {
  id: 'ts-1', day: '2026-11-16T00:00:00', clockIn: '2026-11-16T08:00:00', clockOut: '2026-11-16T16:00:00',
  totalHours: 8, totalHoursApproved: 8, canUpdate: true,
};
const pending: PunchCardDay = {
  id: 'ts-2', day: '2026-11-17T00:00:00', clockIn: '2026-11-17T08:00:00', clockOut: null,
  totalHours: 0, totalHoursApproved: 0, canUpdate: true,
};
const locked: PunchCardDay = {
  id: 'ts-3', day: '2026-11-18T00:00:00', clockIn: null, totalHoursApproved: 6, canUpdate: false,
};

function setup(props: Props = {}) {
  return mountWithPlugins(PunchCardAgenda, { props: { highlights: [], startDate: '2026-11-02T00:00:00', ...props } });
}

type Wrapper = ReturnType<typeof setup>;

function weekCards(wrapper: Wrapper) {
  return wrapper.findAll('.week-card');
}

async function openWeek(wrapper: Wrapper, index: number) {
  await weekCards(wrapper)[index].find('.week-header').trigger('click');
}

function dayRow(wrapper: Wrapper, dayNumber: string) {
  const row = wrapper.findAll('.day-row').find((r) => r.find('.day-num').text() === dayNumber);
  if (!row) throw new Error(`day ${dayNumber} not found`);
  return row;
}

beforeEach(() => {
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('PunchCardAgenda', () => {
  it('shows the current month as collapsible weeks and reports its range', () => {
    const wrapper = setup();

    expect(wrapper.find('h2').text()).toBe('November 2026');
    expect(weekCards(wrapper).map((w) => w.find('.week-range').text())).toEqual([
      'Nov 1 – Nov 7', 'Nov 8 – Nov 14', 'Nov 15 – Nov 21', 'Nov 22 – Nov 28', 'Nov 29 – Dec 5',
    ]);
    expect(wrapper.emitted('monthChange')).toEqual([[{ startDate: '2026-11-01', endDate: '2026-12-05' }]]);
  });

  it('expands the week containing today on first render', () => {
    const wrapper = setup();

    expect(weekCards(wrapper).map((c) => c.classes('is-collapsed'))).toEqual([true, true, false, true, true]);
    expect(weekCards(wrapper)[2].findAll('.day-row')).toHaveLength(7);
  });

  it('shows highlights passed at mount', () => {
    const wrapper = setup({ highlights: [approved] });

    expect(dayRow(wrapper, '16').find('.day-times').text()).toBe('08:00→16:00');
  });

  it('expands the week containing today after navigating back to today', async () => {
    const wrapper = setup();
    await wrapper.findAll('.cal-nav button')[1].trigger('click');

    const cards = weekCards(wrapper);
    expect(cards.map((c) => c.classes('is-collapsed'))).toEqual([true, true, false, true, true]);
    expect(cards[2].findAll('.day-row')).toHaveLength(7);
    expect(wrapper.find('.day-num.is-today').text()).toBe('19');
  });

  it('expands the first week of other months', async () => {
    const wrapper = setup();
    await wrapper.findAll('.cal-nav button')[2].trigger('click');

    expect(wrapper.find('h2').text()).toBe('December 2026');
    expect(weekCards(wrapper).map((c) => c.classes('is-collapsed'))).toEqual([false, true, true, true, true]);
    expect(wrapper.emitted('monthChange')?.at(-1)).toEqual([{ startDate: '2026-11-29', endDate: '2027-01-02' }]);

    await wrapper.findAll('.cal-nav button')[0].trigger('click');
    await wrapper.findAll('.cal-nav button')[0].trigger('click');
    expect(wrapper.find('h2').text()).toBe('October 2026');
  });

  it('toggles a week open and closed', async () => {
    const wrapper = setup();
    await openWeek(wrapper, 1);
    expect(weekCards(wrapper)[1].classes('is-collapsed')).toBe(false);

    await weekCards(wrapper)[1].find('.week-toggle').trigger('click');
    expect(weekCards(wrapper)[1].classes('is-collapsed')).toBe(true);
  });

  it('shows the recorded days with their actions', async () => {
    const wrapper = setup();
    await wrapper.setProps({ highlights: [approved, pending, locked] });

    const approvedRow = dayRow(wrapper, '16');
    expect(approvedRow.find('.day-times').text()).toBe('08:00→16:00');
    expect(approvedRow.find('.day-status').text()).toContain('Approved 8');
    expect(approvedRow.findAll('.day-actions button')).toHaveLength(1);
    await approvedRow.find('.day-actions button').trigger('click');
    expect(wrapper.emitted('edit')).toEqual([[approved]]);

    const pendingRow = dayRow(wrapper, '17');
    expect(pendingRow.find('.day-times').text()).toBe('08:00→—');
    expect(pendingRow.find('.day-status').text()).toContain('Pending');
    await pendingRow.findAll('.day-actions button')[0].trigger('click');
    expect(wrapper.emitted('approve')).toEqual([[pending]]);

    const lockedRow = dayRow(wrapper, '18');
    expect(lockedRow.find('.day-times').text()).toBe('Manual entry');
    await lockedRow.find('.day-actions button').trigger('click');
    expect(wrapper.emitted('view')).toEqual([[locked]]);

    expect(weekCards(wrapper)[2].find('.week-total input').exists()).toBe(true);
  });

  it('posts hours for an empty day and offers clock in today', async () => {
    const wrapper = setup({ errors: { '2026-11-20': 'Hours must be between 0 and 14' } });

    const today = dayRow(wrapper, '19');
    await today.find('.day-actions button').trigger('click');
    expect(wrapper.emitted('clockIn')).toHaveLength(1);

    await today.find('.inline-hours button').trigger('click');
    expect(wrapper.emitted('postHours')?.[0][0]).toMatchObject({ id: null, totalHoursApproved: 0 });

    expect(dayRow(wrapper, '20').find('.help.is-danger').text()).toBe('Hours must be between 0 and 14');
    expect(dayRow(wrapper, '20').find('.day-actions').exists()).toBe(false);
  });

  it('locks days before the start and after the worker ended', async () => {
    const wrapper = setup({
      startDate: '2026-11-10T00:00:00',
      worker: { workerRequestStatus: WorkerRequestStatus.Finished, finishedAt: '2026-10-10T00:00:00' },
    });
    await openWeek(wrapper, 1);

    expect(dayRow(wrapper, '09').text()).toContain('Outside request period');
    expect(dayRow(wrapper, '09').classes()).toContain('is-locked');
    expect(dayRow(wrapper, '10').text()).toContain('Outside request period');
  });

  it('distributes the week hours over its days', async () => {
    const wrapper = setup();
    const week = weekCards(wrapper)[2];
    week.findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 14);
    await nextTick();
    await week.find('.week-total button').trigger('click');

    const hours = week.findAll('.day-row .inline-hours input').map((i) => Number((i.element as HTMLInputElement).value));
    expect(hours.reduce((a, b) => a + b, 0)).toBeCloseTo(14);
    expect(showAlertError).not.toHaveBeenCalled();

    await openWeek(wrapper, 2);
    expect(weekCards(wrapper)[2].find('.week-sum').text()).toBe('14 h');
  });

  it('rejects invalid week hours', async () => {
    const wrapper = setup();
    const week = weekCards(wrapper)[2];
    week.findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 1000);
    await nextTick();
    await week.find('.week-total button').trigger('click');

    expect(showAlertError).toHaveBeenCalledWith('Total hours is invalid');
  });
});
