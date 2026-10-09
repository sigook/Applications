import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { WorkerRequestFilter, WorkerRequestListItem, WorkerRequestDetail } from '@/modules/worker/requests/types';

// Request History
export function getWorkerRequestHistory(filter: WorkerRequestFilter): Promise<PaginatedList<WorkerRequestListItem>> {
  return api.get<PaginatedList<WorkerRequestListItem>>('/api/worker/history', { params: { ...filter } });
}

export function getWorkerRequestHistoryDetail(id: string): Promise<WorkerRequestDetail> {
  return api.get<WorkerRequestDetail>(`/api/worker/history/${id}`);
}
