import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Invoices from '@/modules/agency/accounting/invoices/pages/Invoices.vue';
import {
  changeInvoiceStatus,
  downloadInvoicePdf,
  downloadInvoicesPaymentsReport,
  getAgencyInvoices,
  regenerateInvoicePdf,
} from '@/modules/agency/accounting/invoices/api';
import { InvoiceStatus } from '@/modules/agency/accounting/invoices/types';
import { useAgencyStore } from '@/modules/agency/store';
import { downloadFile, downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInvoiceListItem } from '@/testing/accountingFixtures';
import type { AgencyInvoiceFilter, AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';

const { childStub } = vi.hoisted(() => ({
  childStub: (name: string, emits: string[]) => ({ name, props: { invoice: Object }, emits, template: `<div class="${name}" />` }),
}));

vi.mock('@/modules/agency/accounting/invoices/api', () => ({
  changeInvoiceStatus: vi.fn(),
  downloadInvoicePdf: vi.fn(),
  downloadInvoicesPaymentsReport: vi.fn(),
  getAgencyInvoices: vi.fn(),
  regenerateInvoicePdf: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn(), downloadPDF: vi.fn(), openFileUrl: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/accounting/invoices/components/DeleteInvoice.vue', () => ({ default: childStub('DeleteInvoice', ['deleted']) }));
vi.mock('@/modules/agency/accounting/invoices/components/SendInvoiceEmail.vue', () => ({ default: childStub('SendInvoiceEmail', ['sent']) }));
vi.mock('@/modules/agency/accounting/invoices/components/MarkInvoicePaidModal.vue', () => ({ default: childStub('MarkInvoicePaidModal', ['updated', 'close']) }));

const pending = createInvoiceListItem({ salesRepresentative: 'Rita Sales', dueDays: 3, totalNet: 1130 });
const paid = createInvoiceListItem({
  id: 'inv-2',
  invoiceNumber: 'INV-0011',
  companyFullName: 'Globex',
  status: InvoiceStatus.Paid,
  paidAt: '2026-11-10T12:00:00',
  updatedByName: 'Pat Admin',
  updatedAt: '2026-11-10T12:00:00',
  pdfUrl: 'https://files/inv-2.pdf',
  totalNet: 2260,
});

let pinia: Pinia;

async function setup(items: AgencyInvoiceListItem[] = [pending, paid]) {
  vi.mocked(getAgencyInvoices).mockResolvedValue({ detail: { items, totalItems: 2, pageIndex: 1, totalPages: 1 }, total: 3390 });
  const wrapper = mountWithPlugins(Invoices, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr');
}

async function openActions(wrapper: Wrapper, index: number) {
  await rows(wrapper)[index].find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

function menuItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

async function choose(wrapper: Wrapper, index: number, text: string) {
  await openActions(wrapper, index);
  menuItem(text).click();
  await flushPromises();
}

function lastFilter(): AgencyInvoiceFilter {
  const call = vi.mocked(getAgencyInvoices).mock.calls.at(-1);
  if (!call) throw new Error('not called');
  return call[0];
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Invoices', () => {
  it('loads newest invoices first, shows the grand total and keeps the filter', async () => {
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 1, isDescending: true, pageIndex: 1 });
    expect(useAgencyStore().agencyInvoiceFilter).toMatchObject({ sortBy: 1, isDescending: true });
    expect(wrapper.find('.page-header').text()).toContain('$3,390.00');
    expect(wrapper.find('.page-header').text()).toContain('2');
  });

  it('restores the stored filter and date ranges', async () => {
    useAgencyStore().updateAgencyInvoiceFilter({ sortBy: 2, isDescending: false, createdAtFrom: '2026-11-01T04:00:00.000Z', createdAtTo: '2026-11-14T05:00:00.000Z' });
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 2, isDescending: false, createdAtFrom: '2026-11-01T04:00:00.000Z' });
    expect(wrapper.findAllComponents({ name: 'BDatepicker' })[0].props('modelValue')).toEqual([
      new Date('2026-11-01T04:00:00.000Z'),
      new Date('2026-11-14T05:00:00.000Z'),
    ]);
  });

  it('renders the invoice rows', async () => {
    const wrapper = await setup();
    const [first, second] = rows(wrapper);

    expect(first.text()).toContain('INV-0010');
    expect(first.text()).toContain('01-Nov-2026');
    expect(first.text()).toContain('Rita Sales');
    expect(first.find('.tag.is-warning').text()).toBe('Pending');
    expect(first.text()).toContain('01-Dec-2026');
    expect(first.text()).toContain('Net 30');
    expect(first.text()).toContain('· 3 days due');
    expect(first.text()).toContain('$1,130.00');

    expect(second.find('.tag.is-success').text()).toBe('Paid');
    expect(second.text()).not.toContain('Net 30');
    expect(second.text()).toContain('10-Nov-2026');
    expect(second.findComponent({ name: 'HoverTooltip' }).props('label')).toBe('Updated: Pat Admin · 10-Nov-2026');
  });

  it('says 1 day due in singular', async () => {
    const wrapper = await setup([{ ...pending, dueDays: 1 }]);
    expect(rows(wrapper)[0].text()).toContain('· 1 day due');
  });

  it('offers actions by status', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 0);
    expect(menuItems()).toEqual(expect.arrayContaining(['Download', 'Send Email', 'Mark as Paid', 'Delete']));
    expect(menuItems()).not.toContain('Regenerate PDF');
    expect(menuItems()).not.toContain('Mark as Pending');
  });

  it('offers payment date edit, pending and regenerate for paid invoices with a pdf', async () => {
    const wrapper = await setup([paid]);
    await openActions(wrapper, 0);
    expect(menuItems()).toEqual(expect.arrayContaining(['Download', 'Regenerate PDF', 'Edit Payment Date', 'Mark as Pending']));
    expect(menuItems()).not.toContain('Mark as Paid');
  });

  it('filters by statuses and resets to page one', async () => {
    const wrapper = await setup();
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    taginput.vm.$emit('update:modelValue', [{ id: InvoiceStatus.Due, value: 'Due' }]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ statuses: [InvoiceStatus.Due], pageIndex: 1 });
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([{ id: InvoiceStatus.Due, value: 'Due' }]);

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', []);
    await flushPromises();
    expect(lastFilter().statuses).toBeUndefined();
  });

  it('filters by a date range and clears it', async () => {
    const wrapper = await setup();
    const dueDate = wrapper.findAllComponents({ name: 'BDatepicker' })[1];
    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 30);
    dueDate.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ dueDateFrom: from.toISOString(), dueDateTo: to.toISOString() });

    wrapper.findAllComponents({ name: 'BDatepicker' })[1].vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ dueDateFrom: null, dueDateTo: null });
  });

  it('downloads the payments report with the current filter', async () => {
    const blob = new Blob(['x']);
    vi.mocked(downloadInvoicesPaymentsReport).mockResolvedValue(blob);
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
    await flushPromises();
    menuItem('Payments report').click();
    await flushPromises();

    expect(downloadInvoicesPaymentsReport).toHaveBeenCalledWith(expect.objectContaining({ sortBy: 1, isDescending: true }));
    expect(downloadFile).toHaveBeenCalledWith(blob, expect.stringMatching(/^Payments_/));
  });

  it('shows the error when the payments report fails', async () => {
    const error = new Error('report');
    vi.mocked(downloadInvoicesPaymentsReport).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-toolbar .dropdown-trigger').trigger('click');
    await flushPromises();
    menuItem('Payments report').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the stored pdf when there is one', async () => {
    const wrapper = await setup();
    await choose(wrapper, 1, 'Download');

    expect(openFileUrl).toHaveBeenCalledWith('https://files/inv-2.pdf');
    expect(downloadInvoicePdf).not.toHaveBeenCalled();
  });

  it('renders the pdf when none is stored and reloads', async () => {
    const blob = new Blob(['pdf']);
    vi.mocked(downloadInvoicePdf).mockResolvedValue(blob);
    const wrapper = await setup();
    await choose(wrapper, 0, 'Download');

    expect(downloadInvoicePdf).toHaveBeenCalledWith('inv-1');
    expect(downloadPDF).toHaveBeenCalledWith(blob, 'INV-0010 Acme');
    expect(getAgencyInvoices).toHaveBeenCalledTimes(2);
  });

  it('regenerates the pdf', async () => {
    const blob = new Blob(['pdf']);
    vi.mocked(regenerateInvoicePdf).mockResolvedValue(blob);
    const wrapper = await setup();
    await choose(wrapper, 1, 'Regenerate PDF');

    expect(regenerateInvoicePdf).toHaveBeenCalledWith('inv-2');
    expect(downloadPDF).toHaveBeenCalledWith(blob, 'INV-0011 Globex');
  });

  it('shows the response data when the pdf fails', async () => {
    vi.mocked(downloadInvoicePdf).mockRejectedValue({ data: 'Invoice not found' });
    const wrapper = await setup();
    await choose(wrapper, 0, 'Download');

    expect(showAlertError).toHaveBeenCalledWith('Invoice not found');
  });

  it('marks a paid invoice as pending once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(changeInvoiceStatus).mockResolvedValue();
    const wrapper = await setup();
    await choose(wrapper, 1, 'Mark as Pending');

    expect(showAlertConfirm).toHaveBeenCalledWith('Mark as Pending', 'Invoice <b>INV-0011</b> will be marked as Pending and its payment date cleared.', 'Confirm');
    expect(changeInvoiceStatus).toHaveBeenCalledExactlyOnceWith('inv-2', { status: InvoiceStatus.Pending });
    expect(showAlertSuccess).toHaveBeenCalledWith('Invoice marked as Pending');
    expect(getAgencyInvoices).toHaveBeenCalledTimes(2);
  });

  it('escapes the invoice number in the confirmation', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup([{ ...paid, invoiceNumber: '<i>X</i>' }]);
    await choose(wrapper, 0, 'Mark as Pending');

    expect(vi.mocked(showAlertConfirm).mock.calls[0][1]).toContain('&lt;i&gt;X&lt;/i&gt;');
    expect(changeInvoiceStatus).not.toHaveBeenCalled();
  });

  it('shows the error when marking as pending fails', async () => {
    const error = new Error('status');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(changeInvoiceStatus).mockRejectedValue(error);
    const wrapper = await setup();
    await choose(wrapper, 1, 'Mark as Pending');

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it.each([
    ['Mark as Paid', 0, 'MarkInvoicePaidModal', 'updated'],
    ['Send Email', 0, 'SendInvoiceEmail', 'sent'],
    ['Delete', 0, 'DeleteInvoice', 'deleted'],
    ['Edit Payment Date', 1, 'MarkInvoicePaidModal', 'updated'],
  ])('%s opens its modal and reloads when done', async (action, index, name, event) => {
    const wrapper = await setup();
    await choose(wrapper, index, action);

    const modal = wrapper.findComponent({ name });
    expect(modal.props('invoice')).toEqual([pending, paid][index]);
    modal.vm.$emit(event);
    await flushPromises();

    expect(getAgencyInvoices).toHaveBeenCalledTimes(2);
  });

  it('closes the mark paid modal on close', async () => {
    const wrapper = await setup();
    await choose(wrapper, 0, 'Mark as Paid');
    wrapper.findComponent({ name: 'MarkInvoicePaidModal' }).vm.$emit('close');
    await nextTick();

    const modal = wrapper.findAllComponents({ name: 'BModal' }).find((m) => m.findComponent({ name: 'MarkInvoicePaidModal' }).exists());
    expect(modal?.props('modelValue') ?? false).toBe(false);
  });
});
