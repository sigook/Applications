import { api } from '@/app/security/apiService';
import type { AgencyInvoiceFilter, AgencyInvoiceListResponse, ChangeInvoiceStatusModel, CreateAgencyInvoiceModel, DeleteInvoicePayload, PayStubDeleteWarningItem, SendInvoiceEmailPayload } from '@/modules/agency/accounting/invoices/types';
import type { InvoiceSummaryModel } from '@/shared/types/invoice';

// ---------------------------------------------------------------------------
// Invoices CRUD
// ---------------------------------------------------------------------------

export function getAgencyInvoices(filter: AgencyInvoiceFilter): Promise<AgencyInvoiceListResponse> {
  return api.get<AgencyInvoiceListResponse>('/api/agency/accounting/invoices', { params: { ...filter } });
}

export function previewAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<InvoiceSummaryModel> {
  return api.post<InvoiceSummaryModel>('/api/agency/accounting/invoices/Preview', payload);
}

export function createAgencyInvoice(payload: CreateAgencyInvoiceModel): Promise<void> {
  return api.post('/api/agency/accounting/invoices', payload);
}

export function deleteAgencyInvoice(payload: DeleteInvoicePayload): Promise<void> {
  return api.del(`/api/agency/accounting/invoices/${payload.invoiceId}`, { data: payload });
}

export function changeInvoiceStatus(invoiceId: string, payload: ChangeInvoiceStatusModel): Promise<void> {
  return api.put(`/api/agency/accounting/invoices/${invoiceId}/status`, payload);
}

// ---------------------------------------------------------------------------
// Invoice document / verification / paystubs
// ---------------------------------------------------------------------------

export function downloadInvoicePdf(invoiceId: string): Promise<Blob> {
  return api.get<Blob>(`/api/agency/accounting/invoices/${invoiceId}/pdf`, { responseType: 'blob' });
}

export function getPayStubsByInvoice(invoiceId: string): Promise<PayStubDeleteWarningItem[]> {
  return api.get<PayStubDeleteWarningItem[]>(`/api/agency/accounting/invoices/${invoiceId}/paystubs`);
}

export function sendInvoiceEmail(payload: SendInvoiceEmailPayload): Promise<void> {
  const formData = new FormData();
  payload.recipients.forEach(recipient => formData.append('cc', recipient));
  formData.append('subject', payload.subject);
  formData.append('message', payload.body);
  payload.attachments.forEach(file => formData.append('files', file));
  return api.post(`/api/agency/accounting/invoices/${payload.invoiceId}/email`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
