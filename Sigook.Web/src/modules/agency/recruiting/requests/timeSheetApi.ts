import { api } from '@/app/security/apiService';
import type { TimeSheetListItem, TimeSheetModel, TimeSheetUsagesModel } from '@/shared/punch-card/types';

export function getAgencyWorkerTimeSheetByDate(
  requestId: string,
  workerProfileId: string,
  date: { startDate: string; endDate: string },
): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/agency/recruiting/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, {
    params: { ...date },
  });
}

export function postAgencyWorkerTimeSheet(
  requestId: string,
  workerProfileId: string,
  model: TimeSheetModel,
): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/recruiting/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, model);
}

export function updateAgencyWorkerTimeSheet(
  requestId: string,
  workerProfileId: string,
  id: string,
  model: TimeSheetModel,
): Promise<void> {
  return api.put(`/api/agency/recruiting/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`, model);
}

export function deleteAgencyWorkerTimeSheet(requestId: string, workerProfileId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`);
}

export function getAgencyTimeSheetUsages(requestId: string, workerProfileId: string, id: string): Promise<TimeSheetUsagesModel> {
  return api.get<TimeSheetUsagesModel>(`/api/agency/recruiting/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}/Usages`);
}
