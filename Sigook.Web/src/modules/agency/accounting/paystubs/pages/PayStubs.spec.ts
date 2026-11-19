import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import PayStubs from '@/modules/agency/accounting/paystubs/pages/PayStubs.vue';
import {
  deleteAgencyPayStub,
  downloadPayStubPdf,
  getAgencyPayStubs,
  regeneratePayStubPdf,
  sendPayStubEmail,
  sendPayStubEmailBulk,
} from '@/modules/agency/accounting/paystubs/api';
import { useAgencyStore } from '@/modules/agency/store';
import { downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyPayStubFilter, AgencyPayStubListItem } from '@/modules/agency/accounting/paystubs/types';

vi.mock('@/modules/agency/accounting/paystubs/api', () => ({
  deleteAgencyPayStub: vi.fn(),
  downloadPayStubPdf: vi.fn(),
  getAgencyPayStubs: vi.fn(),
  regeneratePayStubPdf: vi.fn(),
  sendPayStubEmail: vi.fn(),
  sendPayStubEmailBulk: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn(), downloadPDF: vi.fn(), openFileUrl: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/accounting/paystubs/components/GeneratePayStubs.vue', () => ({
  default: { name: 'GeneratePayStubs', emits: ['pay-stubs-generated'], template: '<div class="generate-stub" />' },
}));
vi.mock('@/modules/agency/accounting/paystubs/components/SkipPayrollNumber.vue', () => ({
  default: { name: 'SkipPayrollNumber', template: '<div class="skip-stub" />' },
}));

interface ConfirmConfig {
  title: string;
  message: string;
  onConfirm: () => Promise<void>;
}

const first: AgencyPayStubListItem = {
  id: 'ps-1',
  agencyId: 'agency-1',
  numberId: 501,
  createdAt: '2026-11-14T12:00:00',
  totalPaid: 812.35,
  workerFullName: 'John Smith',
  payStubNumber: 'PS-100',
  payStubNumberId: 100,
  canDelete: true,
  pdfUrl: null,
};
const second: AgencyPayStubListItem = { ...first, id: 'ps-2', workerFullName: 'Ana Lopez', payStubNumber: 'PS-101', pdfUrl: 'https://files/ps-2.pdf' };

let pinia: Pinia;
let confirm: ReturnType<typeof vi.fn<(config: ConfirmConfig) => void>>;

async function setup(items: AgencyPayStubListItem[] = [first, second]) {
  vi.mocked(getAgencyPayStubs).mockResolvedValue({ items, totalItems: items.length, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(PayStubs, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr');
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function rowItems(wrapper: Wrapper, index: number) {
  const dropdown = wrapper.findAllComponents({ name: 'RowActionsDropdown' })[index];
  return dropdown.findAllComponents({ name: 'BDropdownItem' });
}

function rowMenu(wrapper: Wrapper, index: number) {
  return rowItems(wrapper, index).map((i) => i.text());
}

async function openActions(wrapper: Wrapper, index: number) {
  await rows(wrapper)[index].find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

async function choose(wrapper: Wrapper, index: number, text: string) {
  await openActions(wrapper, index);
  const item = rowItems(wrapper, index).find((i) => i.text() === text);
  if (!item) throw new Error(`${text} not found`);
  await item.trigger('click');
  await flushPromises();
}

async function toolbarAction(wrapper: Wrapper, text: string) {
  await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
  await flushPromises();
  menuItem(text).click();
  await flushPromises();
}

function lastFilter(): AgencyPayStubFilter {
  const call = vi.mocked(getAgencyPayStubs).mock.calls.at(-1);
  if (!call) throw new Error('not called');
  return call[0];
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  confirm = vi.fn<(config: ConfirmConfig) => void>();
  vi.mocked(getDialog).mockReturnValue({ confirm, prompt: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
});

describe('PayStubs', () => {
  it('loads pay stubs by number descending and stores the filter', async () => {
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 0, isDescending: true, pageIndex: 1 });
    expect(useAgencyStore().agencyPayStubFilter).toMatchObject({ sortBy: 0, isDescending: true });
    const cells = rows(wrapper)[0].findAll('td').map((c) => c.text());
    expect(cells).toEqual(expect.arrayContaining(['PS-100', '14-Nov-2026', '501', '$812.35']));
    expect(rows(wrapper)[0].text()).toContain('John Smith');
  });

  it('restores the stored created at range', async () => {
    useAgencyStore().updateAgencyPayStubFilter({ sortBy: 1, createdAtFrom: '2026-11-01T04:00:00.000Z', createdAtTo: '2026-11-08T05:00:00.000Z' });
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([
      new Date('2026-11-01T04:00:00.000Z'),
      new Date('2026-11-08T05:00:00.000Z'),
    ]);
  });

  it('filters by created at and clears it', async () => {
    const wrapper = await setup();
    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 7);
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [from, to]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() });

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ createdAtFrom: null, createdAtTo: null });
  });

  it('offers regenerate only when a pdf exists', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 0);
    await openActions(wrapper, 1);
    expect(rowMenu(wrapper, 0)).toEqual(['Download', 'Send Email', 'Delete']);
    expect(rowMenu(wrapper, 1)).toEqual(['Download', 'Regenerate PDF', 'Send Email', 'Delete']);
  });

  it('offers delete only when the pay stub can be deleted', async () => {
    const wrapper = await setup([first, { ...second, canDelete: false }]);
    await openActions(wrapper, 0);
    await openActions(wrapper, 1);

    expect(rowMenu(wrapper, 0)).toContain('Delete');
    expect(rowMenu(wrapper, 1)).not.toContain('Delete');
  });

  it('opens the stored pdf', async () => {
    const wrapper = await setup();
    await choose(wrapper, 1, 'Download');

    expect(openFileUrl).toHaveBeenCalledWith('https://files/ps-2.pdf');
  });

  it('renders the pdf and then offers to regenerate it', async () => {
    const blob = new Blob(['pdf']);
    vi.mocked(downloadPayStubPdf).mockResolvedValue(blob);
    vi.mocked(regeneratePayStubPdf).mockResolvedValue(blob);
    const wrapper = await setup();
    await choose(wrapper, 0, 'Download');

    expect(downloadPayStubPdf).toHaveBeenCalledWith('ps-1');
    expect(downloadPDF).toHaveBeenCalledWith(blob, 'PS-100 John Smith');

    await choose(wrapper, 0, 'Regenerate PDF');
    expect(regeneratePayStubPdf).toHaveBeenCalledWith('ps-1');
  });

  it('shows the response data when the pdf fails', async () => {
    vi.mocked(downloadPayStubPdf).mockRejectedValue({ data: 'Missing timesheets' });
    const wrapper = await setup();
    await choose(wrapper, 0, 'Download');

    expect(showAlertError).toHaveBeenCalledWith('Missing timesheets');
  });

  it('emails one pay stub and marks it sent', async () => {
    vi.mocked(sendPayStubEmail).mockResolvedValue();
    const wrapper = await setup();
    await choose(wrapper, 0, 'Send Email');

    expect(sendPayStubEmail).toHaveBeenCalledWith('ps-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Email to John Smith sent successfully');
    expect(rowMenu(wrapper, 0)).toContain('Email Sent');
  });

  it('lets the email be retried after a failure', async () => {
    const error = new Error('smtp');
    vi.mocked(sendPayStubEmail).mockRejectedValue(error);
    const wrapper = await setup();
    await choose(wrapper, 0, 'Send Email');

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(rowMenu(wrapper, 0)).toContain('Send Email');
  });

  it('emails the checked pay stubs in bulk once confirmed', async () => {
    vi.mocked(sendPayStubEmailBulk).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('tbody .checkbox-cell label')[0].trigger('click');
    await wrapper.findAll('tbody .checkbox-cell label')[1].trigger('click');
    await toolbarAction(wrapper, 'Send Email');

    const config = confirm.mock.calls[0][0];
    expect(config.message).toContain('<b>2</b> pay stub(s)');
    await config.onConfirm();
    await flushPromises();

    expect(sendPayStubEmailBulk).toHaveBeenCalledExactlyOnceWith(['ps-1', 'ps-2']);
    expect(showAlertSuccess).toHaveBeenCalledWith('Sending 2 pay stub(s). A summary will arrive on Teams.');
    expect(wrapper.findAll('tbody .checkbox-cell input:checked')).toHaveLength(0);
  });

  it('shows the error when the bulk email fails', async () => {
    const error = new Error('bulk');
    vi.mocked(sendPayStubEmailBulk).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('tbody .checkbox-cell label')[0].trigger('click');
    await toolbarAction(wrapper, 'Send Email');
    await confirm.mock.calls[0][0].onConfirm();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('deletes a pay stub once confirmed and reloads', async () => {
    vi.mocked(deleteAgencyPayStub).mockResolvedValue();
    const wrapper = await setup([{ ...first, payStubNumber: '<b>PS</b>' }]);
    await choose(wrapper, 0, 'Delete');

    const config = confirm.mock.calls[0][0];
    expect(config.message).toContain('&lt;b&gt;PS&lt;/b&gt;');
    await config.onConfirm();
    await flushPromises();

    expect(deleteAgencyPayStub).toHaveBeenCalledExactlyOnceWith('ps-1');
    expect(getAgencyPayStubs).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the deletion fails', async () => {
    const error = new Error('paid');
    vi.mocked(deleteAgencyPayStub).mockRejectedValue(error);
    const wrapper = await setup();
    await choose(wrapper, 0, 'Delete');
    await confirm.mock.calls[0][0].onConfirm();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyPayStubs).toHaveBeenCalledOnce();
  });

  it('opens the generate modal and reloads after generating', async () => {
    const wrapper = await setup();
    await toolbarAction(wrapper, 'Generate');

    wrapper.findComponent({ name: 'GeneratePayStubs' }).vm.$emit('pay-stubs-generated');
    await flushPromises();
    expect(getAgencyPayStubs).toHaveBeenCalledTimes(2);
  });

  it('opens the skip payroll number modal', async () => {
    const wrapper = await setup();
    await toolbarAction(wrapper, 'Skip Payroll Number');

    expect(wrapper.findComponent({ name: 'SkipPayrollNumber' }).exists()).toBe(true);
  });
});
