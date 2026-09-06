import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyPayStubFilter,
  AgencyPayStubListItem,
  CreatePayStubPayload,
  CreateSkipPayrollNumberPayload,
  PayrollSubContractorListItem,
  SkipPayrollNumberItem,
  SubcontractorPayrollFilter,
  WorkerReadyForPayStubModel,
} from '@/types/accounting';

// ---------------------------------------------------------------------------
// PayStubs CRUD
// ---------------------------------------------------------------------------

export function getAgencyPayStubs(filter: AgencyPayStubFilter): Promise<PaginatedList<AgencyPayStubListItem>> {
  return api.get<PaginatedList<AgencyPayStubListItem>>('/api/agency/accounting/PayStubs', { params: { ...filter } });
}

export function downloadPayStubPdf(payStubId: string): Promise<Blob> {
  return api.get<Blob>(`/api/agency/accounting/PayStubs/${payStubId}/pdf`, { responseType: 'blob' });
}

export function deleteAgencyPayStub(payStubId: string): Promise<void> {
  return api.del(`/api/agency/accounting/PayStubs/${payStubId}`);
}

export function sendPayStubEmail(payStubId: string): Promise<void> {
  return api.post(`/api/agency/accounting/PayStubs/${payStubId}/email`);
}

export function sendPayStubEmailBulk(payStubIds: string[]): Promise<void> {
  return api.post('/api/agency/accounting/PayStubs/email/bulk', { payStubIds });
}

export function createAgencyPayStub(payload: CreatePayStubPayload): Promise<void> {
  return api.post('/api/agency/accounting/PayStubs', payload);
}

// ---------------------------------------------------------------------------
// Generation
// ---------------------------------------------------------------------------

export function getWorkersReadyForPayStub(): Promise<WorkerReadyForPayStubModel[]> {
  return api.get<WorkerReadyForPayStubModel[]>('/api/agency/accounting/PayStubs/WorkersReadyForPayStub');
}

export function generatePayStubs(workerIds: string[]): Promise<void> {
  return api.post('/api/agency/accounting/PayStubs/generate', workerIds);
}

// ---------------------------------------------------------------------------
// Subcontractors report
// ---------------------------------------------------------------------------

export function getPayrollSubcontractors(filter: SubcontractorPayrollFilter): Promise<PaginatedList<PayrollSubContractorListItem>> {
  return api.get<PaginatedList<PayrollSubContractorListItem>>('/api/agency/accounting/reports/subcontractors', {
    params: { ...filter },
  });
}

export function downloadSubcontractorReport(weekEnding: string): Promise<Blob> {
  return api.get<Blob>('/api/agency/accounting/reports/subcontractors/file', {
    params: { weekEnding },
    responseType: 'blob',
  });
}

export function deleteSubcontractorReport(weekEnding: string): Promise<void> {
  return api.del('/api/agency/accounting/reports/subcontractors', { params: { weekEnding } });
}

// ---------------------------------------------------------------------------
// Skip payroll numbers
// ---------------------------------------------------------------------------

export function getSkipPayrollNumbers(filter: { searchTerm?: string }): Promise<SkipPayrollNumberItem[]> {
  return api.get<SkipPayrollNumberItem[]>('/api/agency/accounting/PayStubs/skip-payroll-number', {
    params: { ...filter },
  });
}

export function addSkipPayrollNumber(payload: CreateSkipPayrollNumberPayload): Promise<void> {
  return api.post('/api/agency/accounting/PayStubs/skip-payroll-number', payload);
}
