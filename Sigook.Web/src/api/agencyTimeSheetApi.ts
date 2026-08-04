import { api } from '@/security/apiService';
import type { TimeSheetListItem, TimeSheetModel, TimeSheetUsagesModel } from '@/types/company';

// Get all timesheets for a worker on a request
export function getAgencyWorkerTimeSheet(requestId: string, workerProfileId: string): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`);
}

// Get timesheets filtered by date range
export function getAgencyWorkerTimeSheetByDate(
  requestId: string,
  workerProfileId: string,
  date: { startDate: string; endDate: string },
): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, {
    params: { ...date },
  });
}

export function postAgencyWorkerTimeSheet(
  requestId: string,
  workerProfileId: string,
  model: TimeSheetModel,
): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, model);
}

export function updateAgencyWorkerTimeSheet(
  requestId: string,
  workerProfileId: string,
  id: string,
  model: TimeSheetModel,
): Promise<void> {
  return api.put(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`, model);
}

export function deleteAgencyWorkerTimeSheet(requestId: string, workerProfileId: string, id: string): Promise<void> {
  return api.del(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`);
}

export function getAgencyTimeSheetUsages(requestId: string, workerProfileId: string, id: string): Promise<TimeSheetUsagesModel> {
  return api.get<TimeSheetUsagesModel>(`/api/agency/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}/Usages`);
}
