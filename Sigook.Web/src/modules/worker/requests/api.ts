import { api } from '@/app/security/apiService';
import { ClockType } from '@/shared/constants/enums';
import type { PaginatedList } from '@/shared/types/common';
import type { WorkerRequestFilter, WorkerRequestApplyModel, WorkerRequestListItem, WorkerRequestDetail, WorkerTimeSheetItem } from '@/modules/worker/requests/types';

// Requests
export function getJobs(filter: WorkerRequestFilter): Promise<PaginatedList<WorkerRequestListItem>> {
  return api.get<PaginatedList<WorkerRequestListItem>>('/api/worker/requests', { params: { ...filter } });
}

export function getWorkerRequest(id: string): Promise<WorkerRequestDetail> {
  return api.get<WorkerRequestDetail>(`/api/worker/requests/${id}`);
}

export function workerRequestApplySelf(requestId: string, model: WorkerRequestApplyModel): Promise<void> {
  return api.post(`/api/worker/requests/${requestId}/Apply/`, model);
}

export function requestApplyByEmail(numberId: number, email: string): Promise<void> {
  return api.post<void>('/api/worker/requests/Apply', { numberId, email });
}

// TimeSheet
export function workerRegisterTime(requestId: string, latitude: number, longitude: number): Promise<void> {
  return api.post(`/api/worker/requests/${requestId}/TimeSheet`, { latitude, longitude });
}

export function workerGetTimeSheet(requestId: string): Promise<PaginatedList<WorkerTimeSheetItem>> {
  return api.get<PaginatedList<WorkerTimeSheetItem>>(`/api/worker/requests/${requestId}/TimeSheet`);
}

export function getClockType(
  requestId: string,
  latitude: number,
  longitude: number,
  date: string
): Promise<ClockType> {
  return api.get<ClockType>(`/api/worker/requests/${requestId}/TimeSheet/clock-type/${latitude}/${longitude}`, {
    params: { date }
  });
}
