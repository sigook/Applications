import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyPayStubFilter,
  AgencyPayStubListItem,
  SkipPayrollNumberItem,
  SubcontractorPayrollFilter,
} from '@/types/accounting';

// ---------------------------------------------------------------------------
// PayStubs CRUD
// ---------------------------------------------------------------------------

export function getAgencyPayStubs(filter: AgencyPayStubFilter): Promise<PaginatedList<AgencyPayStubListItem>> {
  return http.get('/api/agency/accounting/PayStubs', { params: { ...filter } }).then(r => r.data);
}

export function downloadPayStubPdf(payStubId: string): Promise<Blob> {
  return http
    .get(`/api/v4/Accounting/PayStub/${payStubId}/Document/PDF`, { responseType: 'blob' })
    .then(r => r.data);
}

export function deleteAgencyPayStub(payStubId: string): Promise<void> {
  return http.delete(`/api/v4/Accounting/PayStub/${payStubId}`).then(() => {});
}

export function sendPayStubEmail(payStubId: string): Promise<void> {
  return http.post(`/api/v4/Accounting/PayStub/${payStubId}/Document/Email`).then(() => {});
}

export function createAgencyPayStub(payload: Record<string, unknown>): Promise<{ id: string }> {
  return http.post('/api/v4/accounting/PayStub', payload).then(r => r.data);
}

// ---------------------------------------------------------------------------
// Generation
// ---------------------------------------------------------------------------

export function getWorkersReadyForPayStub(): Promise<Record<string, unknown>[]> {
  return http.get('/api/agency/accounting/PayStubs/WorkersReadyForPayStub').then(r => r.data);
}

export function generatePayStubs(workerIds: string[]): Promise<Record<string, unknown>> {
  return http.post('/api/agency/accounting/PayStubs/generate', workerIds).then(r => r.data);
}

// ---------------------------------------------------------------------------
// Subcontractors report
// ---------------------------------------------------------------------------

export function getPayrollSubcontractors(filter: SubcontractorPayrollFilter): Promise<Record<string, unknown>[]> {
  return http.get('/api/agency/accounting/reports/subcontractors', { params: { ...filter } }).then(r => r.data);
}

export function downloadSubcontractorReport(weekEnding: string): Promise<Blob> {
  return http
    .get('/api/agency/accounting/reports/subcontractors/file', {
      params: { weekEnding },
      responseType: 'blob',
    })
    .then(r => r.data);
}

// ---------------------------------------------------------------------------
// Skip payroll numbers
// ---------------------------------------------------------------------------

export function getSkipPayrollNumbers(filter: { searchTerm?: string }): Promise<SkipPayrollNumberItem[]> {
  return http.get('/api/agency/accounting/PayStubs/skip-payroll-number', { params: { ...filter } }).then(r => r.data);
}

export function addSkipPayrollNumber(payload: { value: number; reason?: string }): Promise<{ id: string }> {
  return http.post('/api/agency/accounting/PayStubs/skip-payroll-number', payload).then(r => r.data);
}
