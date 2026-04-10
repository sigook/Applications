import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyInvoiceFilter,
  AgencyInvoiceListItem,
  DeleteInvoicePayload,
  SendInvoiceEmailPayload,
} from '@/types/accounting';

// ---------------------------------------------------------------------------
// Invoices CRUD
// ---------------------------------------------------------------------------

export function getAgencyInvoices(filter: AgencyInvoiceFilter): Promise<PaginatedList<AgencyInvoiceListItem>> {
  return http.get('/api/agency/accounting/Invoices', { params: { ...filter } }).then(r => r.data);
}

export function previewAgencyInvoice(payload: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post('/api/agency/accounting/Invoices/Preview', payload).then(r => r.data);
}

export function createAgencyInvoice(payload: Record<string, unknown>): Promise<{ id: string }> {
  return http.post('/api/agency/accounting/Invoices', payload).then(r => r.data);
}

export function deleteAgencyInvoice(payload: DeleteInvoicePayload): Promise<void> {
  return http.delete(`/api/v4/Accounting/Invoice/${payload.invoiceId}`, { data: payload }).then(() => {});
}

// ---------------------------------------------------------------------------
// Invoice document / verification / paystubs
// ---------------------------------------------------------------------------

export function downloadInvoicePdf(invoiceId: string): Promise<Blob> {
  return http
    .get(`/api/v4/Accounting/Invoice/${invoiceId}/Document/PDF`, { responseType: 'blob' })
    .then(r => r.data);
}

export function sendInvoiceVerificationCode(invoiceId: string): Promise<void> {
  return http.post(`/api/v4/Accounting/Invoice/${invoiceId}/SendVerificationCode`).then(() => {});
}

export function getPayStubsByInvoice(invoiceId: string): Promise<Record<string, unknown>[]> {
  return http.get(`/api/v4/Accounting/Invoice/${invoiceId}/PayStub`).then(r => r.data);
}

export function sendInvoiceEmail(payload: SendInvoiceEmailPayload): Promise<void> {
  const formData = new FormData();
  payload.recipients.forEach(recipient => formData.append('cc', recipient));
  formData.append('subject', payload.subject);
  formData.append('message', payload.body);
  payload.attachments.forEach(file => formData.append('files', file));
  return http
    .post(`/api/v4/Accounting/Invoice/${payload.invoiceId}/Document/Email`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then(() => {});
}
