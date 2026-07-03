import { api } from '@/security/apiService';

export function fetchInvoicePdf(invoiceId: string): Promise<Blob> {
  return api.get<Blob>(`/api/Invoice/${invoiceId}/Document/PDF`, { responseType: 'blob' });
}

export function downloadPayrollSubcontractor(weekEnding: string): Promise<Blob> {
  return api.get<Blob>(`/api/PayrollSubcontractor/${weekEnding}/Document/EXCEL`, { responseType: 'blob' });
}

export function downloadWeeklyPayrollExcel(weekEnding: string): Promise<Blob> {
  return api.get<Blob>(`/api/WeeklyPayroll/${weekEnding}/Document/EXCEL`, { responseType: 'blob' });
}

export function downloadWeeklyPayrollExcelByWeekEnding(date: string): Promise<Blob> {
  return api.get<Blob>(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByWeekEnding`, { responseType: 'blob' });
}

export function downloadWeeklyPayrollExcelByPaymentDate(date: string): Promise<Blob> {
  return api.get<Blob>(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByPaymentDate`, { responseType: 'blob' });
}
