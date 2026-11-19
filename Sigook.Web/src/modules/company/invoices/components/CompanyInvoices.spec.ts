import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyInvoices from '@/modules/company/invoices/components/CompanyInvoices.vue';
import { downloadCompanyInvoicePdf, getCompanyInvoice } from '@/modules/company/invoices/api';
import { downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyInvoiceListItem } from '@/modules/company/invoices/types';

vi.mock('@/modules/company/invoices/api', () => ({ getCompanyInvoice: vi.fn(), downloadCompanyInvoicePdf: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadPDF: vi.fn(), openFileUrl: vi.fn(), downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const invoices: CompanyInvoiceListItem[] = [
  { id: 'inv-1', numberId: 1, invoiceNumber: 'INV-0001', createdAt: '2026-11-10T14:30:00', weekEnding: '2026-11-08T00:00:00', totalNet: 1234.5, pdfUrl: null },
  { id: 'inv-2', numberId: 2, invoiceNumber: 'INV-0002', createdAt: '2026-11-11T09:00:00', weekEnding: null, totalNet: 100, pdfUrl: 'https://files/inv-2.pdf' },
];

async function setup() {
  vi.mocked(getCompanyInvoice).mockResolvedValue({ items: invoices, totalItems: 2, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(CompanyInvoices);
  await flushPromises();
  return wrapper;
}

function rows(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 4).map((cell) => cell.text()));
}

function downloadButton(wrapper: Awaited<ReturnType<typeof setup>>, index: number) {
  return wrapper.findAll('button[aria-label="Download invoice"]')[index];
}

describe('CompanyInvoices', () => {
  it('loads the first page sorted by default and lists the invoices', async () => {
    const wrapper = await setup();

    expect(getCompanyInvoice).toHaveBeenCalledExactlyOnceWith({ sortBy: 0, isDescending: false, pageIndex: 1, pageSize: 30 });
    expect(wrapper.findAll('thead th').map((th) => th.text()).slice(0, 4)).toEqual(['Invoice Number', 'Created At', 'Week Ending', 'Total']);
    const [first, second] = rows(wrapper);
    expect(first[0]).toBe('INV-0001');
    expect(first[2]).not.toBe('N/A');
    expect(first[3]).toContain('1,234.50');
    expect(second[0]).toBe('INV-0002');
    expect(second[2]).toBe('N/A');
  });

  it('opens the stored pdf when the invoice has one', async () => {
    const wrapper = await setup();
    await downloadButton(wrapper, 1).trigger('click');

    expect(openFileUrl).toHaveBeenCalledExactlyOnceWith('https://files/inv-2.pdf');
    expect(downloadCompanyInvoicePdf).not.toHaveBeenCalled();
  });

  it('downloads the generated pdf named after the invoice', async () => {
    const blob = new Blob(['pdf']);
    vi.mocked(downloadCompanyInvoicePdf).mockResolvedValue(blob);
    const wrapper = await setup();
    await downloadButton(wrapper, 0).trigger('click');
    await flushPromises();

    expect(downloadCompanyInvoicePdf).toHaveBeenCalledExactlyOnceWith('inv-1');
    expect(downloadPDF).toHaveBeenCalledExactlyOnceWith(blob, 'INV-0001');
    expect(openFileUrl).not.toHaveBeenCalled();
  });

  it('shows the error when the download fails', async () => {
    const error = new Error('download failed');
    vi.mocked(downloadCompanyInvoicePdf).mockRejectedValue(error);
    const wrapper = await setup();
    await downloadButton(wrapper, 0).trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(downloadPDF).not.toHaveBeenCalled();
  });
});
