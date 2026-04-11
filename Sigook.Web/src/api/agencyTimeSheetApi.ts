import http from '@/security/apiService';
import type { TimeSheetListItem, TimeSheetModel, TimeSheetUsagesModel } from '@/types/company';

// Get all timesheets for a worker on a request
export function getAgencyWorkerTimeSheet(requestId: string, workerId: string): Promise<TimeSheetListItem[]> {
  return http.get(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`).then(r => r.data);
}

// Get timesheets filtered by date range
export function getAgencyWorkerTimeSheetByDate(
  requestId: string,
  workerId: string,
  date: { startDate: string; endDate: string },
): Promise<TimeSheetListItem[]> {
  return http
    .get(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`, { params: { ...date } })
    .then(r => r.data);
}

export function postAgencyWorkerTimeSheet(
  requestId: string,
  workerId: string,
  model: TimeSheetModel,
): Promise<{ id: string }> {
  return http.post(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet`, model).then(r => r.data);
}

export function updateAgencyWorkerTimeSheet(
  requestId: string,
  workerId: string,
  id: string,
  model: TimeSheetModel,
): Promise<void> {
  return http.put(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model).then(() => {});
}

export function deleteAgencyWorkerTimeSheet(requestId: string, workerId: string, id: string): Promise<void> {
  return http.delete(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`).then(() => {});
}

export function getAgencyTimeSheetUsages(requestId: string, workerId: string, id: string): Promise<TimeSheetUsagesModel> {
  return http
    .get(`/api/v2/AgencyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}/Usages`)
    .then(r => r.data);
}
