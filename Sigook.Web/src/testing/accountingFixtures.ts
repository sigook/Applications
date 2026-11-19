import { InvoiceStatus } from '@/modules/agency/accounting/invoices/types';
import type { AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';

export function createInvoiceListItem(overrides: Partial<AgencyInvoiceListItem> = {}): AgencyInvoiceListItem {
  return {
    id: 'inv-1',
    agencyId: 'agency-1',
    numberId: 10,
    createdAt: '2026-11-01T12:00:00',
    agencyFullName: 'Covenant',
    companyFullName: 'Acme',
    email: 'billing@acme.test',
    subTotal: 1000,
    tax: 130,
    totalNet: 1130,
    invoiceNumberId: 10,
    companyProfileId: 'company-1',
    invoiceNumber: 'INV-0010',
    status: InvoiceStatus.Pending,
    paymentTermDays: 30,
    dueDate: '2026-12-01T12:00:00',
    dueDays: 12,
    pdfUrl: null,
    ...overrides,
  };
}
