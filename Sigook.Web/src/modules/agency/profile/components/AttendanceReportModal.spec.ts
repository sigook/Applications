import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import AttendanceReportModal from '@/modules/agency/profile/components/AttendanceReportModal.vue';
import { downloadAttendanceReport, getAttendanceReport } from '@/modules/agency/profile/api';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyPersonnelListItem, UserAttendanceListItem, UserAttendanceReport } from '@/modules/agency/profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/composables/useBreakpoint', () => ({ useBreakpoint: () => ({ isTouch: computed(() => false) }) }));
vi.mock('@/modules/agency/profile/api', () => ({ getAttendanceReport: vi.fn(), downloadAttendanceReport: vi.fn() }));
vi.mock('@/modules/agency/profile/components/AttendanceEditModal.vue', () => ({
  default: { name: 'AttendanceEditModal', props: ['attendance'], emits: ['saved'], template: '<div class="edit-stub" />' },
}));

const users: AgencyPersonnelListItem[] = [
  { id: 'p-1', userId: 'u-1', name: 'Zoe Adams', email: null, role: 'admin', agencyIds: [], agencies: [] },
  { id: 'p-2', userId: 'u-2', name: 'Ana Smith', email: null, role: 'admin', agencyIds: [], agencies: [] },
];

function item(overrides: Partial<UserAttendanceListItem>): UserAttendanceListItem {
  return {
    id: 'att', userId: 'u-1', name: 'Zoe Adams', date: '2026-11-16T00:00:00', clockIn: '2026-11-16T08:00:00', clockOut: '2026-11-16T18:00:00',
    lunchMinutes: 60, workedHours: 9, lunchHours: 1, regularHours: 8, overtimeHours: 1,
    isWeekend: false, isMissingClockOut: false, isEdited: false, editedBy: null, editedAt: null, editReason: null,
    ...overrides,
  };
}

const zoeMonday = item({ id: 'att-1' });
const zoeSaturday = item({
  id: 'att-2', date: '2026-11-21T00:00:00', clockIn: '2026-11-21T09:00:00', clockOut: '2026-11-21T13:00:00',
  lunchMinutes: 0, workedHours: 4, lunchHours: 0, regularHours: 0, overtimeHours: 4, isWeekend: true, isEdited: true, editedBy: 'Boss', editReason: 'Late',
});
const anaMissing = item({
  id: 'att-3', userId: 'u-2', name: 'Ana Smith', clockOut: null, workedHours: 0, lunchHours: 0, regularHours: 0, overtimeHours: 0, isMissingClockOut: true,
});

const report: UserAttendanceReport = {
  items: [zoeMonday, zoeSaturday, anaMissing],
  totals: { workedHours: 13, lunchHours: 1, regularHours: 8, overtimeHours: 5 },
};

const week = { from: '2026-11-16', to: '2026-11-22' };

async function setup(agencyIds?: string) {
  const wrapper = mountWithPlugins(AttendanceReportModal, { props: { users, agencyIds } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function boxes(wrapper: Wrapper) {
  return wrapper.findAll('.box .title').map((t) => t.text());
}

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((r) => r.findAll('td').map((c) => c.text()));
}

function footer(wrapper: Wrapper) {
  return wrapper.findAll('tfoot th').map((c) => c.text());
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date'] });
  vi.mocked(getAttendanceReport).mockResolvedValue(report);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('AttendanceReportModal', () => {
  it('loads this week for every user and summarizes it by user', async () => {
    const wrapper = await setup('a1');

    expect(getAttendanceReport).toHaveBeenCalledExactlyOnceWith('a1', { userId: undefined, ...week });
    expect(wrapper.find('select').findAll('option').map((o) => o.text())).toEqual(['All users', 'Zoe Adams', 'Ana Smith']);
    expect(boxes(wrapper)).toEqual(['13.00 h', '8.00 h', '5.00 h']);
    expect(rows(wrapper).map((r) => r.slice(0, 7))).toEqual([
      ['Ana Smith', '1', '0.00', '0.00', '0.00', '0.00', '1 missing out'],
      ['Zoe Adams', '2', '13.00', '1.00', '8.00', '5.00', ''],
    ]);
    expect(footer(wrapper)).toEqual(['Total', '3', '13.00', '1.00', '8.00', '5.00', '', '']);
  });

  it('shows the punches of the selected user and goes back to all users', async () => {
    const wrapper = await setup();
    vi.mocked(getAttendanceReport).mockResolvedValue({ items: [zoeMonday, zoeSaturday], totals: { workedHours: 13, lunchHours: 1, regularHours: 8, overtimeHours: 5 } });
    await wrapper.findAll('tbody tr')[1].find('button[aria-label="View detail"]').trigger('click');
    await flushPromises();

    expect(getAttendanceReport).toHaveBeenLastCalledWith(undefined, { userId: 'u-1', ...week });
    expect((wrapper.find('select').element as HTMLSelectElement).value).toBe('u-1');
    expect(rows(wrapper).map((r) => r.slice(0, 9))).toEqual([
      ['Zoe Adams', 'Mon, Nov 16', '08:00', '18:00', '9.00', '1.00', '8.00', '1.00', ''],
      ['Zoe Adams', 'Sat, Nov 21', '09:00', '13:00', '4.00', '0.00', '0.00', '4.00', 'WeekendEdited'],
    ]);
    expect(footer(wrapper)).toEqual(['Total', '', '', '', '13.00', '1.00', '8.00', '5.00', '', '']);

    await button(wrapper, 'All users').trigger('click');
    await flushPromises();

    expect(getAttendanceReport).toHaveBeenLastCalledWith(undefined, { userId: undefined, ...week });
    expect(wrapper.find('button[aria-label="View detail"]').exists()).toBe(true);
  });

  it('marks a missing clock out in the user detail', async () => {
    vi.mocked(getAttendanceReport).mockResolvedValue({ items: [anaMissing], totals: report.totals });
    const wrapper = await setup();
    await wrapper.find('select').setValue('u-2');
    await flushPromises();

    expect(rows(wrapper)[0].slice(3, 9)).toEqual(['—', '0.00', '0.00', '0.00', '0.00', 'Missing out']);
  });

  it('opens a punch to edit and reloads after saving it', async () => {
    const wrapper = await setup();
    await wrapper.find('select').setValue('u-1');
    await flushPromises();
    await wrapper.findAll('button[aria-label="Edit punch"]')[0].trigger('click');
    await nextTick();

    const edit = wrapper.findComponent({ name: 'AttendanceEditModal' });
    expect(edit.props('attendance')).toEqual(zoeMonday);

    edit.vm.$emit('saved');
    await flushPromises();

    expect(getAttendanceReport).toHaveBeenCalledTimes(3);
    expect(wrapper.findComponent({ name: 'AttendanceEditModal' }).exists()).toBe(false);
  });

  it('reloads when the dates change and waits for a full range', async () => {
    const wrapper = await setup();
    const picker = wrapper.findComponent({ name: 'BDatepicker' });
    picker.vm.$emit('update:modelValue', [new Date('2026-11-01T00:00:00')]);
    await flushPromises();

    expect(getAttendanceReport).toHaveBeenCalledTimes(1);

    picker.vm.$emit('update:modelValue', [new Date('2026-11-01T00:00:00'), new Date('2026-11-15T00:00:00')]);
    await flushPromises();

    expect(getAttendanceReport).toHaveBeenLastCalledWith(undefined, { userId: undefined, from: '2026-11-01', to: '2026-11-15' });
  });

  it('exports the report for the current filter', async () => {
    const blob = new Blob(['xlsx']);
    vi.mocked(downloadAttendanceReport).mockResolvedValue(blob);
    const wrapper = await setup('a1');
    await button(wrapper, 'Export Excel').trigger('click');
    await flushPromises();

    expect(downloadAttendanceReport).toHaveBeenCalledExactlyOnceWith('a1', { userId: undefined, ...week });
    expect(downloadFile).toHaveBeenCalledExactlyOnceWith(blob, 'Attendance_2026-11-16_2026-11-22');
  });

  it('disables the export when there are no punches', async () => {
    vi.mocked(getAttendanceReport).mockResolvedValue({ items: [], totals: { workedHours: 0, lunchHours: 0, regularHours: 0, overtimeHours: 0 } });
    const wrapper = await setup();

    expect(button(wrapper, 'Export Excel').attributes('disabled')).toBeDefined();
    expect(footer(wrapper)).toEqual([]);
  });

  it('shows the error when the export fails', async () => {
    const error = new Error('export failed');
    vi.mocked(downloadAttendanceReport).mockRejectedValue(error);
    const wrapper = await setup();
    await button(wrapper, 'Export Excel').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(downloadFile).not.toHaveBeenCalled();
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getAttendanceReport).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(boxes(wrapper)).toEqual(['0.00 h', '0.00 h', '0.00 h']);
  });
});
