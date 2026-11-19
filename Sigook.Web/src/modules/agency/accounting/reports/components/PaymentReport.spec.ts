import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import PaymentReport from '@/modules/agency/accounting/reports/components/PaymentReport.vue';
import { downloadWeeklyPayrollReport, getPaymentReport } from '@/modules/agency/accounting/reports/api';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/accounting/reports/api', () => ({ downloadWeeklyPayrollReport: vi.fn(), getPaymentReport: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

let pinia: Pinia;

async function setup() {
  vi.mocked(getPaymentReport).mockResolvedValue({
    items: [{ weekEnding: '2026-11-13T00:00:00', numberOfPayStubs: 12, totalNet: 9876.5, displayWeekEnding: '11-13-2026' }],
    totalItems: 1,
    pageIndex: 1,
    totalPages: 1,
  });
  const wrapper = mountWithPlugins(PaymentReport, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('PaymentReport', () => {
  it('lists payrolls by payment date', async () => {
    const wrapper = await setup();

    expect(getPaymentReport).toHaveBeenCalledWith(expect.objectContaining({ pageIndex: 1 }));
    expect(wrapper.findAll('tbody tr td').map((c) => c.text()).slice(0, 3)).toEqual(['Fri 13 November, 2026', '12', '$9,876.50']);
  });

  it('downloads the weekly payroll', async () => {
    const blob = new Blob(['x']);
    vi.mocked(downloadWeeklyPayrollReport).mockResolvedValue(blob);
    const wrapper = await setup();
    await wrapper.find('tbody button').trigger('click');
    await flushPromises();

    expect(downloadWeeklyPayrollReport).toHaveBeenCalledExactlyOnceWith('11-13-2026');
    expect(downloadFile).toHaveBeenCalledWith(blob, 'Payroll November 13 2026');
  });

  it('shows the error when the download fails', async () => {
    const error = new Error('download');
    vi.mocked(downloadWeeklyPayrollReport).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('tbody button').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('tbody button').classes()).not.toContain('is-loading');
  });
});
