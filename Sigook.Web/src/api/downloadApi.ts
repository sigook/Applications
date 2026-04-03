import http from '@/security/apiService';

export function fetchInvoicePdf(invoiceId: string): Promise<Blob> {
  return http.get(`/api/Invoice/${invoiceId}/Document/PDF`, { responseType: 'blob' }).then(r => r.data);
}

export function downloadPayrollSubcontractor(weekEnding: string): Promise<Blob> {
  return http.get(`/api/PayrollSubcontractor/${weekEnding}/Document/EXCEL`, { responseType: 'blob' }).then(r => r.data);
}

export function downloadWeeklyPayrollExcel(weekEnding: string): Promise<Blob> {
  return http.get(`/api/WeeklyPayroll/${weekEnding}/Document/EXCEL`, { responseType: 'blob' }).then(r => r.data);
}

export function downloadWeeklyPayrollExcelByWeekEnding(date: string): Promise<Blob> {
  return http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByWeekEnding`, { responseType: 'blob' }).then(r => r.data);
}

export function downloadWeeklyPayrollExcelByPaymentDate(date: string): Promise<Blob> {
  return http.get(`/api/WeeklyPayroll/${date}/Document/EXCEL/ByPaymentDate`, { responseType: 'blob' }).then(r => r.data);
}
