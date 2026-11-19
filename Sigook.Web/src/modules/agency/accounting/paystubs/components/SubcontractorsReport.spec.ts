import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import SubcontractorsReport from '@/modules/agency/accounting/paystubs/components/SubcontractorsReport.vue';
import { deleteSubcontractorReport, downloadSubcontractorReport, getPayrollSubcontractors } from '@/modules/agency/accounting/paystubs/api';
import { downloadFile } from '@/shared/utils/downloadFile';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/accounting/paystubs/api', () => ({
  deleteSubcontractorReport: vi.fn(),
  downloadSubcontractorReport: vi.fn(),
  getPayrollSubcontractors: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

interface ConfirmConfig {
  message: string;
  confirmText: string;
  onConfirm: () => void;
}

let pinia: Pinia;

async function setup() {
  vi.mocked(getPayrollSubcontractors).mockResolvedValue({
    items: [{ weekEnding: '2026-11-15T00:00:00', numberOfWorkers: 4, totalNet: 2450.5, weekEndingDisplay: '15-Nov' }],
    totalItems: 1,
    pageIndex: 1,
    totalPages: 1,
  });
  const wrapper = mountWithPlugins(SubcontractorsReport, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function actionButtons(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr td:last-child button');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('SubcontractorsReport', () => {
  it('lists the weekly reports', async () => {
    const wrapper = await setup();

    expect(getPayrollSubcontractors).toHaveBeenCalledWith(expect.objectContaining({ pageIndex: 1 }));
    const cells = wrapper.findAll('tbody tr td').map((c) => c.text());
    expect(cells.slice(0, 3)).toEqual(['Sun 15 November, 2026', '4', '$2,450.50']);
  });

  it('downloads the report of the week', async () => {
    const blob = new Blob(['x']);
    vi.mocked(downloadSubcontractorReport).mockResolvedValue(blob);
    const wrapper = await setup();
    await actionButtons(wrapper)[0].trigger('click');
    await flushPromises();

    expect(downloadSubcontractorReport).toHaveBeenCalledExactlyOnceWith('11-15-2026');
    expect(downloadFile).toHaveBeenCalledWith(blob, 'Subcontractor_11-15-2026');
  });

  it('shows the response data when the download fails', async () => {
    vi.mocked(downloadSubcontractorReport).mockRejectedValue({ data: 'No report' });
    const wrapper = await setup();
    await actionButtons(wrapper)[0].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('No report');
  });

  it('deletes the report once confirmed and reloads', async () => {
    const confirm = vi.fn<(config: ConfirmConfig) => void>();
    vi.mocked(getDialog).mockReturnValue({ confirm, prompt: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
    vi.mocked(deleteSubcontractorReport).mockResolvedValue();
    const wrapper = await setup();
    await actionButtons(wrapper)[1].trigger('click');

    const config = confirm.mock.calls[0][0];
    expect(config.message).toContain('<b>11-15-2026</b>');
    expect(config.message).toContain('<b>4</b> worker report(s)');
    config.onConfirm();
    await flushPromises();

    expect(deleteSubcontractorReport).toHaveBeenCalledExactlyOnceWith('11-15-2026');
    expect(showAlertSuccess).toHaveBeenCalledWith('Subcontractor report 11-15-2026 deleted successfully');
    expect(getPayrollSubcontractors).toHaveBeenCalledTimes(2);
  });

  it('shows the response data when the deletion fails', async () => {
    const confirm = vi.fn<(config: ConfirmConfig) => void>();
    vi.mocked(getDialog).mockReturnValue({ confirm, prompt: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
    vi.mocked(deleteSubcontractorReport).mockRejectedValue({ data: 'Already paid' });
    const wrapper = await setup();
    await actionButtons(wrapper)[1].trigger('click');
    confirm.mock.calls[0][0].onConfirm();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('Already paid');
    expect(getPayrollSubcontractors).toHaveBeenCalledOnce();
  });
});
