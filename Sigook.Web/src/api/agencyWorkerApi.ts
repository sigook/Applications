import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyWorkerFilter,
  AgencyWorkerListItem,
  AgencyWorkerDropdownItem,
  AgencyWorkerCommentModel,
  UpdateWorkerEmailModel,
  UpdateWorkerProfileFieldsPayload,
  AgencyWorkerHoliday,
  AddNewHolidayPayload,
  AgencyWorkerRequestHistoryItem,
} from '@/types/agency';
import type { WorkerProfile } from '@/types/worker';

// Workers list (paginated)
export function getAgencyWorkers(filter: AgencyWorkerFilter): Promise<PaginatedList<AgencyWorkerListItem>> {
  return http.get('api/AgencyWorkerProfile', { params: { ...filter } }).then(r => r.data);
}

// Workers autocomplete (Dropdown)
export function getAgencyWorkersDropdown(filter: { searchTerm: string }): Promise<AgencyWorkerDropdownItem[]> {
  return http.get('api/AgencyWorkerProfile/Dropdown', { params: { ...filter } }).then(r => r.data);
}

// Single worker
export function getAgencyWorker(id: string): Promise<WorkerProfile> {
  return http.get(`/api/AgencyWorkerProfile/${id}`).then(r => r.data);
}

// Toggle approved-to-work flag
export function updateApprovedToWork(id: string): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${id}/ApprovedToWork`).then(() => {});
}

// Toggle DNU (Do Not Use) flag
export function updateAgencyWorkerProfileDNU(id: string): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${id}/Dnu`).then(() => {});
}

// Toggle contractor / subcontractor flags
export function updateAgencyWorkerContractor(id: string): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${id}/IsContractor`).then(() => {});
}

export function updateAgencyWorkerSubContractor(id: string): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${id}/IsSubcontractor`).then(() => {});
}

// Tax / external id updates
export function updateWorkerProfileTaxCategory(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${payload.id}/tax-category`, payload).then(() => {});
}

export function updateWorkerProfileTaxRate(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${payload.id}/tax-rate`, payload).then(() => {});
}

export function updateWorkerProfileExternalId(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${payload.id}/ExternalId`, payload).then(() => {});
}

// Worker email
export function updateAgencyWorkerEmail(workerProfileId: string, model: UpdateWorkerEmailModel): Promise<void> {
  return http.put(`/api/AgencyWorkerProfile/${workerProfileId}/Email`, model).then(() => {});
}

// Worker comment from agency side (used by shared Comments component)
export function agencyCommentWorker(id: string, comment: AgencyWorkerCommentModel): Promise<void> {
  return http.post(`/api/AgencyWorker/${id}/Comment`, comment).then(() => {});
}

// Request history for a worker
export function getAgencyWorkerProfileRequestHistory(
  workerId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<AgencyWorkerRequestHistoryItem>> {
  return http
    .get(`/api/AgencyWorkerProfile/${workerId}/RequestHistory?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

// Worker holidays
export function getAgencyWorkerProfileHolidays(workerProfileId: string): Promise<AgencyWorkerHoliday[]> {
  return http.get(`/api/agency-worker-profile-holiday/${workerProfileId}`).then(r => r.data);
}

export function addUpdateAgencyWorkerProfileHolidays(workerProfileId: string, data: AgencyWorkerHoliday): Promise<void> {
  return http.post(`/api/agency-worker-profile-holiday/${workerProfileId}`, data).then(() => {});
}

export function addNewHoliday(payload: AddNewHolidayPayload): Promise<void> {
  return http.post('/api/agency-worker-profile-holiday/new-holiday', payload).then(() => {});
}
