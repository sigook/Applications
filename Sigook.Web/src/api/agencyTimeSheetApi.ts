import { api } from '@/security/apiService';
import type { TimeSheetListItem, TimeSheetModel, TimeSheetUsagesModel } from '@/types/company';

// Get all timesheets for a worker on a request
export function getAgencyWorkerTimeSheet(requestId: string, workerId: string): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`);
}

// Get timesheets filtered by date range
export function getAgencyWorkerTimeSheetByDate(
  requestId: string,
  workerId: string,
  date: { startDate: string; endDate: string },
): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`, {
    params: { ...date },
  });
}

export function postAgencyWorkerTimeSheet(
  requestId: string,
  workerId: string,
  model: TimeSheetModel,
): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`, model);
}

export function updateAgencyWorkerTimeSheet(
  requestId: string,
  workerId: string,
  id: string,
  model: TimeSheetModel,
): Promise<void> {
  return api.put(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model);
}

export function deleteAgencyWorkerTimeSheet(requestId: string, workerId: string, id: string): Promise<void> {
  return api.del(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`);
}

export function getAgencyTimeSheetUsages(requestId: string, workerId: string, id: string): Promise<TimeSheetUsagesModel> {
  return api.get<TimeSheetUsagesModel>(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}/Usages`);
}
