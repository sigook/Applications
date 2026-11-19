import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import WorkWageHistory from '@/modules/agency/recruiting/workers/components/WorkWageHistory.vue';
import { getWorkerProfileWageHistory } from '@/modules/agency/recruiting/workers/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerWageHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getWorkerProfileWageHistory: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const row: WorkerWageHistoryItem = {
  id: 'ps-1',
  numberId: 77,
  payStubNumber: 'PS-0077',
  weekEnding: '2026-03-08T12:00:00',
  totalEarnings: 900,
  vacations: 36,
  totalPaid: 750,
  start: '2026-03-02T12:00:00',
  end: '2026-03-08T12:00:00',
  accumulatedQuantity: 120,
  accumulatedTotal: 2400,
  accumulatedVacations: 96,
  accumulatedTotalEarnings: 2496,
  accumulatedTotalPaid: 2100,
  items: [
    { description: 'Regular', quantity: 40, total: 800 },
    { description: 'Overtime', quantity: 4, total: 100 },
  ],
  companies: ['Acme Foods', 'Beta Logistics'],
};

function page(items: WorkerWageHistoryItem[]): PaginatedList<WorkerWageHistoryItem> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages: 1 };
}

async function setup() {
  vi.mocked(getWorkerProfileWageHistory).mockResolvedValue(page([row]));
  const wrapper = mountWithPlugins(WorkWageHistory, { props: { workerId: 'wp-1' } });
  await flushPromises();
  return wrapper;
}

describe('WorkWageHistory', () => {
  it('loads the worker pay stubs sorted by number, newest first', async () => {
    await setup();

    expect(getWorkerProfileWageHistory).toHaveBeenCalledWith(expect.objectContaining({
      profileId: 'wp-1',
      sortBy: 0,
      isDescending: true,
    }));
  });

  it('renders the pay stub with its companies, lines and totals', async () => {
    const wrapper = await setup();
    const cell = (index: number) => wrapper.findAll('tbody > tr')[0].findAll(':scope > td')[index].text();

    expect(cell(0)).toContain('PS-0077');
    expect(cell(0)).toContain('Acme Foods');
    expect(cell(0)).toContain('Beta Logistics');
    expect(cell(1)).toContain('From:');
    expect(cell(1)).toContain('To:');
    const lines = wrapper.findAll('table.no-border-bottom tbody tr').map((tr) => tr.findAll('td').map((td) => td.text()));
    expect(lines[0][0]).toBe('Regular');
    expect(lines[0][1]).toBe('40');
    expect(lines[1][0]).toBe('Overtime');
    expect(lines[2][0]).toBe('Total:');
    expect(lines[2][1]).toBe('44');
    expect(lines[2][2]).toContain('900');
    expect(lines[3][1]).toBe('120');
  });

  it('searches by week ending range and clears it', async () => {
    const wrapper = await setup();
    const from = new Date('2026-03-01T00:00:00');
    const to = new Date('2026-03-31T00:00:00');
    const picker = wrapper.findComponent({ name: 'BDatepicker' });

    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getWorkerProfileWageHistory).toHaveBeenLastCalledWith(expect.objectContaining({
      weekEndingFrom: from.toISOString(),
      weekEndingTo: to.toISOString(),
    }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(getWorkerProfileWageHistory).toHaveBeenLastCalledWith(expect.objectContaining({ weekEndingFrom: null, weekEndingTo: null }));
  });
});
