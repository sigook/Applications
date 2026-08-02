import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { WorkerCommentCreateModel } from '@/types/worker';
import type {
  AgencyWorkerFilter,
  AgencyWorkerListItem,
  AgencyWorkerDropdownItem,
  UpdateWorkerEmailModel,
  UpdateWorkerProfileFieldsPayload,
  AgencyWorkerHoliday,
  AddNewHolidayPayload,
  AgencyWorkerRequestHistoryItem,
} from '@/types/agency';
import type { WorkerProfile } from '@/types/worker';

// Workers list (paginated)
export function getAgencyWorkers(filter: AgencyWorkerFilter): Promise<PaginatedList<AgencyWorkerListItem>> {
  return api.get<PaginatedList<AgencyWorkerListItem>>('api/agency/workers', { params: { ...filter } });
}

// Workers autocomplete (Dropdown)
export function getAgencyWorkersDropdown(filter: { searchTerm: string }): Promise<AgencyWorkerDropdownItem[]> {
  return api.get<AgencyWorkerDropdownItem[]>('api/agency/workers/Dropdown', { params: { ...filter } });
}

// Single worker
export function getAgencyWorker(id: string): Promise<WorkerProfile> {
  return api.get<WorkerProfile>(`/api/agency/workers/${id}`);
}

// Toggle approved-to-work flag
export function updateApprovedToWork(id: string): Promise<void> {
  return api.put(`/api/agency/workers/${id}/ApprovedToWork`);
}

// Toggle DNU (Do Not Use) flag
export function updateAgencyWorkerProfileDNU(id: string): Promise<void> {
  return api.put(`/api/agency/workers/${id}/Dnu`);
}

// Toggle contractor / subcontractor flags
export function updateAgencyWorkerContractor(id: string): Promise<void> {
  return api.put(`/api/agency/workers/${id}/IsContractor`);
}

export function updateAgencyWorkerSubContractor(id: string): Promise<void> {
  return api.put(`/api/agency/workers/${id}/IsSubcontractor`);
}

// Tax / external id updates
export function updateWorkerProfileTaxCategory(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/workers/${payload.id}/tax-category`, payload);
}

export function updateWorkerProfileTaxRate(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/workers/${payload.id}/tax-rate`, payload);
}

export function updateWorkerProfileExternalId(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/workers/${payload.id}/ExternalId`, payload);
}

export function updateWorkerProfileWcCode(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/workers/${payload.id}/WcCode`, payload);
}

// Worker email
export function updateAgencyWorkerEmail(workerProfileId: string, model: UpdateWorkerEmailModel): Promise<void> {
  return api.put(`/api/agency/workers/${workerProfileId}/Email`, model);
}

// Worker comment from agency side (used by shared Comments component)
export function agencyCommentWorker(workerProfileId: string, comment: WorkerCommentCreateModel): Promise<void> {
  return api.post(`/api/agency/workers/${workerProfileId}/Comments`, comment);
}

// Request history for a worker
export function getAgencyWorkerProfileRequestHistory(
  workerId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<AgencyWorkerRequestHistoryItem>> {
  return api.get<PaginatedList<AgencyWorkerRequestHistoryItem>>(
    `/api/agency/workers/${workerId}/RequestHistory?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

// Worker holidays
export function getAgencyWorkerProfileHolidays(workerProfileId: string): Promise<AgencyWorkerHoliday[]> {
  return api.get<AgencyWorkerHoliday[]>(`/api/agency/workers/${workerProfileId}/Holidays`);
}

export function addUpdateAgencyWorkerProfileHolidays(workerProfileId: string, data: AgencyWorkerHoliday): Promise<void> {
  return api.post(`/api/agency/workers/${workerProfileId}/Holidays`, data);
}

export function addNewHoliday(workerProfileId: string, payload: AddNewHolidayPayload): Promise<void> {
  return api.post(`/api/agency/workers/${workerProfileId}/Holidays/new-holiday`, payload);
}
