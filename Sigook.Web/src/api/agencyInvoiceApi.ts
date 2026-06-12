import http from '@/security/apiService';
import type {
  AgencyInvoiceFilter,
  AgencyInvoiceListResponse,
  InvoiceSummaryModel,
  CreateAgencyInvoiceModel,
  DeleteInvoicePayload,
  PayStubDeleteWarningItem,
  SendInvoiceEmailPayload,
} from '@/types/accounting';

// ---------------------------------------------------------------------------
// Invoices CRUD
// ---------------------------------------------------------------------------

export function getAgencyInvoices(filter: AgencyInvoiceFilter): Promise<AgencyInvoiceListResponse> {
  return http.get('/api/agency/accounting/Invoices', { params: { ...filter } }).then(r => r.data);
}

export function previewAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<InvoiceSummaryModel> {
  return http.post('/api/agency/accounting/Invoices/Preview', payload).then(r => r.data);
}

export function createAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<void> {
  return http.post('/api/agency/accounting/Invoices', payload).then(() => {});
}

export function deleteAgencyInvoice(payload: DeleteInvoicePayload): Promise<void> {
  return http.delete(`/api/agency/accounting/Invoices/${payload.invoiceId}`, { data: payload }).then(() => {});
}

// ---------------------------------------------------------------------------
// Invoice document / verification / paystubs
// ---------------------------------------------------------------------------

export function downloadInvoicePdf(invoiceId: string): Promise<Blob> {
  return http
    .get(`/api/agency/accounting/Invoices/${invoiceId}/pdf`, { responseType: 'blob' })
    .then(r => r.data);
}

export function getPayStubsByInvoice(invoiceId: string): Promise<PayStubDeleteWarningItem[]> {
  return http.get(`/api/v4/Accounting/Invoice/${invoiceId}/PayStub`).then(r => r.data);
}

export function sendInvoiceEmail(payload: SendInvoiceEmailPayload): Promise<void> {
  const formData = new FormData();
  payload.recipients.forEach(recipient => formData.append('cc', recipient));
  formData.append('subject', payload.subject);
  formData.append('message', payload.body);
  payload.attachments.forEach(file => formData.append('files', file));
  return http
    .post(`/api/agency/accounting/Invoices/${payload.invoiceId}/email`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then(() => {});
}
