import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import TimeSheetHistory from '@/modules/agency/recruiting/workers/components/TimeSheetHistory.vue';
import { getWorkerProfileTimeSheetHistory } from '@/modules/agency/recruiting/workers/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerTimeSheetHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getWorkerProfileTimeSheetHistory: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const row: WorkerTimeSheetHistoryItem = {
  id: 'ts-1',
  requestId: 'req-1',
  numberId: 501,
  businessName: 'Acme Foods',
  jobTitle: 'Packer',
  date: '2026-03-02T12:00:00',
  isHoliday: true,
  regularHours: 8,
  holidayHours: 0,
  overtimeHours: 1.5,
  missingHours: 0,
  missingHoursOvertime: 0,
  totalHours: 9.5,
  accumulatedRegularHours: 40,
  accumulatedHolidayHours: 0,
  accumulatedOvertimeHours: 2,
  accumulatedMissingHours: 0,
  accumulatedMissingHoursOvertime: 0,
  accumulatedTotalHours: 42,
};

function page(items: WorkerTimeSheetHistoryItem[]): PaginatedList<WorkerTimeSheetHistoryItem> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages: 1 };
}

async function setup(items: WorkerTimeSheetHistoryItem[] = [row]) {
  vi.mocked(getWorkerProfileTimeSheetHistory).mockResolvedValue(page(items));
  const wrapper = mountWithPlugins(TimeSheetHistory, { props: { workerId: 'wp-1' } });
  await flushPromises();
  return wrapper;
}

describe('TimeSheetHistory', () => {
  it('loads the worker timesheets newest first', async () => {
    await setup();

    expect(getWorkerProfileTimeSheetHistory).toHaveBeenCalledWith(expect.objectContaining({
      profileId: 'wp-1',
      sortBy: 2,
      isDescending: true,
      pageIndex: 1,
    }));
  });

  it('renders the company, request link, holiday mark and hours', async () => {
    const wrapper = await setup();
    const cells = wrapper.findAll('tbody tr td').map((c) => c.text());

    expect(cells[0]).toBe('Acme Foods');
    expect(cells[1]).toContain('501');
    expect(cells[1]).toContain('Packer');
    expect(cells[2]).toContain('Holiday');
    expect(cells[3]).toContain('Acc.');
    expect(wrapper.find('tbody a').attributes('href')).toBe('/recruiting/requests/req-1');
  });

  it('omits the holiday mark on regular days', async () => {
    const wrapper = await setup([{ ...row, isHoliday: false }]);

    expect(wrapper.find('.holiday-text').exists()).toBe(false);
  });

  it('searches by the selected date range and clears it', async () => {
    const wrapper = await setup();
    const from = new Date('2026-03-01T00:00:00');
    const to = new Date('2026-03-07T00:00:00');
    const picker = wrapper.findComponent({ name: 'BDatepicker' });

    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getWorkerProfileTimeSheetHistory).toHaveBeenLastCalledWith(expect.objectContaining({
      dateFrom: from.toISOString(),
      dateTo: to.toISOString(),
      pageIndex: 1,
    }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(getWorkerProfileTimeSheetHistory).toHaveBeenLastCalledWith(expect.objectContaining({ dateFrom: null, dateTo: null }));
  });
});
