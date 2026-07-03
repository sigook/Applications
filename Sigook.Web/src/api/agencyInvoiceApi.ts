import { api } from '@/security/apiService';
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
  return api.get<AgencyInvoiceListResponse>('/api/agency/accounting/Invoices', { params: { ...filter } });
}

export function previewAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<InvoiceSummaryModel> {
  return api.post<InvoiceSummaryModel>('/api/agency/accounting/Invoices/Preview', payload);
}

export function createAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<void> {
  return api.post('/api/agency/accounting/Invoices', payload);
}

export function deleteAgencyInvoice(payload: DeleteInvoicePayload): Promise<void> {
  return api.del(`/api/agency/accounting/Invoices/${payload.invoiceId}`, { data: payload });
}

// ---------------------------------------------------------------------------
// Invoice document / verification / paystubs
// ---------------------------------------------------------------------------

export function downloadInvoicePdf(invoiceId: string): Promise<Blob> {
  return api.get<Blob>(`/api/agency/accounting/Invoices/${invoiceId}/pdf`, { responseType: 'blob' });
}

export function getPayStubsByInvoice(invoiceId: string): Promise<PayStubDeleteWarningItem[]> {
  return api.get<PayStubDeleteWarningItem[]>(`/api/agency/accounting/Invoices/${invoiceId}/paystubs`);
}

export function sendInvoiceEmail(payload: SendInvoiceEmailPayload): Promise<void> {
  const formData = new FormData();
  payload.recipients.forEach(recipient => formData.append('cc', recipient));
  formData.append('subject', payload.subject);
  formData.append('message', payload.body);
  payload.attachments.forEach(file => formData.append('files', file));
  return api.post(`/api/agency/accounting/Invoices/${payload.invoiceId}/email`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
