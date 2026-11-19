import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import PayStubsTab from '@/modules/worker/activity/components/PayStubsTab.vue';
import { getWorkerPayStubs, getWorkerPayStubYears, getWorkerPayStubYtd } from '@/modules/worker/activity/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerPayStubListItem } from '@/modules/worker/activity/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/modules/worker/activity/api', () => ({
  getWorkerPayStubs: vi.fn(),
  getWorkerPayStubYears: vi.fn(),
  getWorkerPayStubYtd: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const payStub: WorkerPayStubListItem = {
  id: 'ps1',
  payStubNumber: 'PS-100',
  weekEnding: '2026-05-30T12:00:00',
  paymentDate: '2026-06-05T12:00:00',
  hours: 40,
  totalEarnings: 800,
  totalDeductions: 120,
  totalPaid: 680,
  companies: ['Acme', 'Globex'],
  pdfUrl: null,
};

async function setup(years: number[] = [2026, 2025]) {
  vi.mocked(getWorkerPayStubYears).mockResolvedValue(years);
  vi.mocked(getWorkerPayStubYtd).mockResolvedValue({ earnings: 5000, totalDeductions: 700, totalPaid: 4300 });
  vi.mocked(getWorkerPayStubs).mockResolvedValue({ items: [payStub], totalItems: 1, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(PayStubsTab);
  await flushPromises();
  return wrapper;
}

beforeEach(() => {
  touch.value = false;
});

describe('PayStubsTab', () => {
  it('loads the latest year with its YTD', async () => {
    const wrapper = await setup();
    expect(getWorkerPayStubYtd).toHaveBeenCalledWith(2026);
    expect(getWorkerPayStubs).toHaveBeenCalledWith(expect.objectContaining({ year: 2026, pageIndex: 1, pageSize: 30 }));
    expect(wrapper.find('.ytd').text()).toBe('YTD net: $4,300.00');
    const cells = wrapper.findAll('tbody tr td').map((c) => c.text());
    expect(cells).toEqual(['PS-100', '30-May-2026', 'Acme, Globex', '40.0', '$800.00', '−$120.00', '$680.00', 'PDF']);
  });

  it('downloads a pay stub', async () => {
    const wrapper = await setup();
    await wrapper.find('tbody button').trigger('click');
    expect(wrapper.emitted('download')).toEqual([[payStub]]);
  });

  it('reloads when another year is chosen', async () => {
    const wrapper = await setup();
    await wrapper.find('select[aria-label="Year"]').setValue('2025');
    await flushPromises();
    expect(getWorkerPayStubs).toHaveBeenLastCalledWith(expect.objectContaining({ year: 2025, pageIndex: 1 }));
    expect(getWorkerPayStubYtd).toHaveBeenLastCalledWith(2025);
  });

  it('says there are no pay stubs without years', async () => {
    const wrapper = await setup([]);
    expect(wrapper.text()).toContain('You have no pay stubs yet');
    expect(getWorkerPayStubYtd).not.toHaveBeenCalled();
    expect(getWorkerPayStubs).not.toHaveBeenCalled();
  });

  it('reports failures loading years and YTD', async () => {
    vi.mocked(getWorkerPayStubYears).mockRejectedValue('years down');
    mountWithPlugins(PayStubsTab);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('years down');

    vi.mocked(getWorkerPayStubYears).mockResolvedValue([2026]);
    vi.mocked(getWorkerPayStubYtd).mockRejectedValue('ytd down');
    vi.mocked(getWorkerPayStubs).mockResolvedValue({ items: [], totalItems: 0, pageIndex: 1, totalPages: 0 });
    mountWithPlugins(PayStubsTab);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('ytd down');
  });

  it('shows cards on touch screens', async () => {
    touch.value = true;
    const wrapper = await setup();
    const card = wrapper.find('.rcard');
    expect(card.find('.rcard__sub').text()).toBe('PS-100 · Acme, Globex');
    expect(card.text()).toContain('−$120.00');
    await card.find('button').trigger('click');
    expect(wrapper.emitted('download')).toEqual([[payStub]]);
  });
});
