import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CalendarPunchCard from '@/shared/punch-card/CalendarPunchCard.vue';
import { maximumHoursPerDay } from '@/shared/constants/catalog';
import { WorkerRequestStatus } from '@/shared/constants/enums';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardDay, PunchCardWorker } from '@/shared/punch-card/types';

const mobile = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isMobile: computed(() => mobile.value) }),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface Props {
  startDate?: string | null;
  worker?: PunchCardWorker | null;
  highlights?: PunchCardDay[];
}

const slot = '<template #punch-input="{ item }"><span class="punch" :data-day="item.day.getDate ? item.day.getDate() : item.day">{{ item.totalHoursApproved }}</span></template>';

function setup(props: Props = { startDate: '2026-10-01' }) {
  return mountWithPlugins(CalendarPunchCard, { props, slots: { 'punch-input': slot } });
}

function heading(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('h2').text();
}

function lastRange(wrapper: ReturnType<typeof setup>) {
  return wrapper.emitted('onMonthChange')?.at(-1)?.[0];
}

function firstWeekHours(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('tbody tr')[0].findAll('.punch').map((p) => p.text());
}

function grayDays(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('td.bg-gray').map((td) => td.find('.wrapper-day > span').text());
}

beforeEach(() => {
  mobile.value = false;
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('CalendarPunchCard', () => {
  it('opens on the current month and reports its visible range', () => {
    const wrapper = setup();

    expect(heading(wrapper)).toBe('November 2026');
    expect(wrapper.findAll('thead th').map((th) => th.text())).toEqual(['Total', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']);
    expect(wrapper.findAll('tbody tr')).toHaveLength(5);
    expect(wrapper.emitted('onMonthChange')).toEqual([[{ startDate: '2026-11-01', endDate: '2026-12-05' }]]);
    expect(wrapper.find('.isToday').text()).toBe('19');
    expect(wrapper.findAll('.notCurrentMonth').map((d) => d.text())).toEqual(['01', '02', '03', '04', '05']);
  });

  it('navigates between months and back to today', async () => {
    const wrapper = setup();
    const buttons = wrapper.findAll('.column.is-4 button');

    await buttons[2].trigger('click');
    expect(heading(wrapper)).toBe('December 2026');
    expect(lastRange(wrapper)).toEqual({ startDate: '2026-11-29', endDate: '2027-01-02' });

    await buttons[0].trigger('click');
    await buttons[0].trigger('click');
    expect(heading(wrapper)).toBe('October 2026');
    expect(lastRange(wrapper)).toEqual({ startDate: '2026-09-27', endDate: '2026-10-31' });

    await buttons[1].trigger('click');
    expect(heading(wrapper)).toBe('November 2026');
    expect(lastRange(wrapper)).toEqual({ startDate: '2026-11-01', endDate: '2026-12-05' });
  });

  it('locks days before the start date', () => {
    const wrapper = setup({ startDate: '2026-11-04' });

    expect(grayDays(wrapper)).toEqual(['01', '02', '03']);
    expect(wrapper.findAll('.punch')).toHaveLength(35 - 3);
  });

  it('locks days a month after a finished worker left', () => {
    const wrapper = setup({
      startDate: '2026-10-01',
      worker: { workerRequestStatus: WorkerRequestStatus.Finished, finishedAt: '2026-10-28' },
    });

    expect(grayDays(wrapper)).toEqual(['28', '29', '30', '01', '02', '03', '04', '05']);
  });

  it('uses the rejection date when there is no finish date', () => {
    const wrapper = setup({
      startDate: '2026-10-01',
      worker: { workerRequestStatus: WorkerRequestStatus.Rejected, rejectedAt: '2026-11-02', finishedAt: null },
    });

    expect(grayDays(wrapper)).toEqual(['02', '03', '04', '05']);
  });

  it('does not limit a booked worker', () => {
    const wrapper = setup({
      startDate: '2026-10-01',
      worker: { workerRequestStatus: WorkerRequestStatus.Booked, finishedAt: '2026-10-02' },
    });

    expect(grayDays(wrapper)).toEqual([]);
  });

  it('places loaded punches on their days and totals each week', async () => {
    const wrapper = setup();
    await wrapper.setProps({
      highlights: [
        { id: 'p1', day: '2026-11-02T00:00:00', totalHoursApproved: 8 },
        { id: 'p2', day: '2026-11-03T00:00:00', totalHoursApproved: 7.5 },
        { id: 'p3', day: '2026-11-10T00:00:00', totalHoursApproved: null },
      ],
    });

    expect(firstWeekHours(wrapper)).toEqual(['0', '8', '7.5', '0', '0', '0', '0']);
    expect(wrapper.findAll('.highlight-day')).toHaveLength(3);
    expect(wrapper.find('table').classes()).toContain('hasEvents');
    const totals = wrapper.findAll('tbody tr').map((tr) => (tr.find('input').element as HTMLInputElement).value);
    expect(totals.slice(0, 2)).toEqual(['15.5', '0']);
  });

  it('distributes the weekly total across the days', async () => {
    const wrapper = setup();
    const row = wrapper.findAll('tbody tr')[0];
    await row.find('input').setValue('40');
    await row.find('button').trigger('click');
    await flushPromises();

    expect(firstWeekHours(wrapper).map(Number).reduce((a, b) => a + b, 0)).toBe(40);
    expect(firstWeekHours(wrapper).every((h) => Number(h) <= maximumHoursPerDay)).toBe(true);
  });

  it('shows punches passed when mounting', () => {
    const wrapper = setup({
      startDate: '2026-10-01',
      highlights: [{ id: 'p1', day: '2026-11-02T00:00:00', totalHoursApproved: 8 }],
    });

    expect(firstWeekHours(wrapper)).toEqual(['0', '8', '0', '0', '0', '0', '0']);
    expect((wrapper.find('tbody tr input').element as HTMLInputElement).value).toBe('8');
  });

  it('distributes the weekly total only across editable days', async () => {
    const wrapper = setup({ startDate: '2026-11-04' });
    const row = wrapper.findAll('tbody tr')[0];
    await row.find('input').setValue('30');
    await row.find('button').trigger('click');
    await flushPromises();

    const hours = firstWeekHours(wrapper).map(Number);
    expect(hours).toHaveLength(4);
    expect(hours.reduce((a, b) => a + b, 0)).toBe(30);
  });

  it('rejects a weekly total over the daily maximum of the editable days', async () => {
    const wrapper = setup({ startDate: '2026-11-04' });
    const row = wrapper.findAll('tbody tr')[0];
    await row.find('input').setValue(String(maximumHoursPerDay * 4 + 1));
    await row.find('button').trigger('click');

    expect(showAlertError).toHaveBeenCalledWith('Total hours is invalid');
  });

  it('rejects a weekly total over the daily maximum', async () => {
    const wrapper = setup();
    const row = wrapper.findAll('tbody tr')[0];
    await row.find('input').setValue(String((maximumHoursPerDay + 1) * 7));
    await row.find('button').trigger('click');

    expect(showAlertError).toHaveBeenCalledWith('Total hours is invalid');
    expect(firstWeekHours(wrapper)).toEqual(['0', '0', '0', '0', '0', '0', '0']);
  });

  it('renders week cards on mobile', () => {
    mobile.value = true;
    const wrapper = setup({ startDate: '2026-11-04' });

    expect(wrapper.find('table').exists()).toBe(false);
    expect(wrapper.findAll('.week-title').map((w) => w.text())).toEqual(['Week 1', 'Week 2', 'Week 3', 'Week 4', 'Week 5']);
    const firstWeek = wrapper.findAll('.week-card')[0];
    expect(firstWeek.findAll('.day-header').map((d) => d.text())).toEqual(['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']);
    expect(firstWeek.findAll('.day-card.bg-gray')).toHaveLength(3);
    expect(wrapper.find('.day-card.is-today .day-number').text()).toBe('19');

    expect(firstWeek.find('.day-card.bg-gray').find('.punch').exists()).toBe(false);
  });

  it('distributes the weekly total on mobile', async () => {
    mobile.value = true;
    const wrapper = setup();
    const firstWeek = wrapper.findAll('.week-card')[0];
    await firstWeek.find('input').setValue('30');
    await firstWeek.find('button').trigger('click');

    expect(firstWeek.findAll('.punch').map((p) => Number(p.text())).reduce((a, b) => a + b, 0)).toBe(30);
  });
});
