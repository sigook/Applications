import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type {
  WorkerAssignmentListItem,
  WorkerHistoryFilter,
  WorkerActivitySummary,
  WorkerPayStubFilter,
  WorkerPayStubListItem,
  WorkerPayStubYtd,
  WorkerWeekTimeSheets,
} from '@/modules/worker/activity/types';

// Summary
export function getActivitySummary(weekStart: string): Promise<WorkerActivitySummary> {
  return api.get<WorkerActivitySummary>('/api/worker/activity/summary', { params: { weekStart } });
}

// History
export function getWorkerHistory(filter: WorkerHistoryFilter): Promise<PaginatedList<WorkerAssignmentListItem>> {
  return api.get<PaginatedList<WorkerAssignmentListItem>>('/api/worker/history', { params: { ...filter } });
}

// Pay stubs
export function getWorkerPayStubs(filter: WorkerPayStubFilter): Promise<PaginatedList<WorkerPayStubListItem>> {
  return api.get<PaginatedList<WorkerPayStubListItem>>('/api/worker/paystubs', { params: { ...filter } });
}

export function getWorkerPayStubYears(): Promise<number[]> {
  return api.get<number[]>('/api/worker/paystubs/years');
}

export function getWorkerPayStubYtd(year: number): Promise<WorkerPayStubYtd> {
  return api.get<WorkerPayStubYtd>(`/api/worker/paystubs/ytd/${year}`);
}

export function downloadWorkerPayStubPdf(payStubId: string): Promise<Blob> {
  return api.get<Blob>(`/api/worker/paystubs/${payStubId}/pdf`, { responseType: 'blob' });
}

// Timesheets
export function getWorkerWeekTimeSheets(weekStart: string): Promise<WorkerWeekTimeSheets> {
  return api.get<WorkerWeekTimeSheets>('/api/worker/activity/timesheets', { params: { weekStart } });
}
