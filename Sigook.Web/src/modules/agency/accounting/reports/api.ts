import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { AgencyReportFilter, HoursWorkedResume, WeeklyPayrollItem } from '@/modules/agency/accounting/reports/types';
import type { AgencyCompanyJobPosition } from '@/modules/agency/recruiting/clients/types';

// Job positions hours worked report (data, not blob)
export function getJobPositionsHoursWorked(filter: AgencyReportFilter & { companyProfileId: string }): Promise<AgencyCompanyJobPosition[]> {
  return api.get<AgencyCompanyJobPosition[]>(`/api/agency/accounting/reports/${filter.companyProfileId}/job-positions`, { params: { ...filter } });
}

// Hours worked report (data, not blob)
export function getHoursWorkedReport(filter: AgencyReportFilter): Promise<HoursWorkedResume> {
  return api.get<HoursWorkedResume>('/api/agency/accounting/reports/hours-worked', { params: { ...filter } });
}

// T4 report (blob)
export function getT4Report(filter: AgencyReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/accounting/reports/t4', { params: { ...filter }, responseType: 'blob' });
}

// CRA payroll report (blob)
export function getCraPayrollReport(filter: AgencyReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/accounting/reports/cra-payroll', { params: { ...filter }, responseType: 'blob' });
}

// Timesheets report (blob, USA agencies)
export function getTimesheetsReport(filter: AgencyReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/accounting/reports/timesheets/file', { params: { ...filter }, responseType: 'blob' });
}

// Payment report (data, not blob)
export function getPaymentReport(filter: AgencyReportFilter): Promise<PaginatedList<WeeklyPayrollItem>> {
  return api.get<PaginatedList<WeeklyPayrollItem>>('/api/agency/accounting/reports/payments', { params: { ...filter } });
}

// Weekly payroll report (blob)
export function downloadWeeklyPayrollReport(weekEnding: string): Promise<Blob> {
  return api.get<Blob>('/api/agency/accounting/reports/payments/file', {
    params: { weekEnding },
    responseType: 'blob',
  });
}
