import http from '@/security/apiService';
import type { AgencyReportFilter } from '@/types/agency';

// Generic blob report downloader (used by Export.vue and Companies.vue)
export function downloadAgencyReport(url: string, filter: Record<string, unknown>): Promise<Blob> {
  return http.get(url, { params: { ...filter }, responseType: 'blob' }).then(r => r.data);
}

// Request timesheet document (PDF/Excel)
export function getRequestTimeSheetDocument(requestId: string): Promise<Blob> {
  return http.get(`/api/Request/${requestId}/TimeSheet/Document`, { responseType: 'blob' }).then(r => r.data);
}

// Workers report document (PDF/Excel)
export function getWorkersReportDocument(requestId: string): Promise<Blob> {
  return http.get(`/api/WorkersReportDocument/${requestId}/Document`, { responseType: 'blob' }).then(r => r.data);
}

// Job positions hours worked report (data, not blob)
export function getJobPositionsHoursWorked(filter: AgencyReportFilter & { companyId: string }): Promise<Record<string, unknown>[]> {
  return http
    .get(`/api/agency/accounting/reports/${filter.companyId}/job-positions`, { params: { ...filter } })
    .then(r => r.data);
}

// Hours worked report (data, not blob)
export function getHoursWorkedReport(filter: AgencyReportFilter): Promise<Record<string, unknown>> {
  return http.get('/api/agency/accounting/reports/hours-worked', { params: { ...filter } }).then(r => r.data);
}

// T4 report (blob)
export function getT4Report(filter: AgencyReportFilter): Promise<Blob> {
  return http
    .get('/api/agency/accounting/reports/t4', { params: { ...filter }, responseType: 'blob' })
    .then(r => r.data);
}

// CRA payroll report (blob)
export function getCraPayrollReport(filter: AgencyReportFilter): Promise<Blob> {
  return http
    .get('/api/agency/accounting/reports/cra-payroll', { params: { ...filter }, responseType: 'blob' })
    .then(r => r.data);
}

// Payment report (data, not blob)
export function getPaymentReport(filter: AgencyReportFilter): Promise<Record<string, unknown>> {
  return http.get('/api/agency/accounting/reports/payments', { params: { ...filter } }).then(r => r.data);
}

// Weekly payroll report (blob)
export function downloadWeeklyPayrollReport(weekEnding: string): Promise<Blob> {
  return http
    .get('/api/agency/accounting/reports/payments/file', {
      params: { weekEnding },
      responseType: 'blob',
    })
    .then(r => r.data);
}
