import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { WageHistoryFilter, TimeSheetHistoryFilter, WorkerWageHistoryItem, WorkerWageHistoryAccumulated, WorkerTimeSheetHistoryItem, WorkerTimeSheetHistoryAccumulated } from '@/modules/agency/recruiting/workers/types';
import type { WorkerCommentCreateModel, WorkerCommentFilter, WorkerCommentList } from '@/shared/worker-profile/types';
import type { AgencyWorkerFilter, AgencyWorkerListItem, AgencyWorkerDropdownItem, UpdateWorkerEmailModel, UpdateWorkerProfileFieldsPayload, AgencyWorkerHoliday, AddNewHolidayPayload, AgencyWorkerRequestHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

// Wage History
export function getWorkerProfileWageHistory(filter: WageHistoryFilter): Promise<PaginatedList<WorkerWageHistoryItem>> {
  return api.get<PaginatedList<WorkerWageHistoryItem>>(`/api/agency/recruiting/workers/${filter.profileId}/WageHistory`, { params: { ...filter } });
}

export function getWorkerProfileWageHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerWageHistoryAccumulated> {
  return api.get<WorkerWageHistoryAccumulated>(`/api/agency/recruiting/workers/${profileId}/WageHistory/${rowNumber}`);
}

// TimeSheet History
export function getWorkerProfileTimeSheetHistory(filter: TimeSheetHistoryFilter): Promise<PaginatedList<WorkerTimeSheetHistoryItem>> {
  return api.get<PaginatedList<WorkerTimeSheetHistoryItem>>(`/api/agency/recruiting/workers/${filter.profileId}/TimeSheetHistory`, { params: { ...filter } });
}

export function getWorkerProfileTimeSheetHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerTimeSheetHistoryAccumulated> {
  return api.get<WorkerTimeSheetHistoryAccumulated>(`/api/agency/recruiting/workers/${profileId}/TimeSheetHistory/${rowNumber}`);
}

// Workers list (paginated)
export function getAgencyWorkers(filter: AgencyWorkerFilter): Promise<PaginatedList<AgencyWorkerListItem>> {
  return api.get<PaginatedList<AgencyWorkerListItem>>('/api/agency/recruiting/workers', { params: { ...filter } });
}

// Workers autocomplete (Dropdown)
export function getAgencyWorkersDropdown(filter: { searchTerm: string }): Promise<AgencyWorkerDropdownItem[]> {
  return api.get<AgencyWorkerDropdownItem[]>('/api/agency/recruiting/workers/Dropdown', { params: { ...filter } });
}

// Single worker
export function getAgencyWorker(id: string): Promise<WorkerProfileDetail> {
  return api.get<WorkerProfileDetail>(`/api/agency/recruiting/workers/${id}`);
}

// Toggle approved-to-work flag
export function updateApprovedToWork(id: string): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${id}/ApprovedToWork`);
}

// Toggle DNU (Do Not Use) flag
export function updateAgencyWorkerProfileDNU(id: string): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${id}/Dnu`);
}

// Toggle contractor / subcontractor flags
export function updateAgencyWorkerContractor(id: string): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${id}/IsContractor`);
}

export function updateAgencyWorkerSubContractor(id: string): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${id}/IsSubcontractor`);
}

// Tax / external id updates
export function updateWorkerProfileTaxCategory(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${payload.id}/tax-category`, payload);
}

export function updateWorkerProfileTaxRate(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${payload.id}/tax-rate`, payload);
}

export function updateWorkerProfileExternalId(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${payload.id}/ExternalId`, payload);
}

export function updateWorkerProfileWcCode(payload: UpdateWorkerProfileFieldsPayload): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${payload.id}/WcCode`, payload);
}

// Worker email
export function updateAgencyWorkerEmail(workerProfileId: string, model: UpdateWorkerEmailModel): Promise<void> {
  return api.put(`/api/agency/recruiting/workers/${workerProfileId}/Email`, model);
}

// Worker comments seen from the agency side (used by shared Comments component)
export function getAgencyWorkerComments(workerProfileId: string, filter: WorkerCommentFilter): Promise<WorkerCommentList> {
  return api.get<WorkerCommentList>(`/api/agency/recruiting/workers/${workerProfileId}/Comments`, {
    params: { PageSize: filter.size, PageIndex: filter.pageIndex },
  });
}

export function agencyCommentWorker(workerProfileId: string, comment: WorkerCommentCreateModel): Promise<void> {
  return api.post(`/api/agency/recruiting/workers/${workerProfileId}/Comments`, comment);
}

// Request history for a worker
export function getAgencyWorkerProfileRequestHistory(
  workerId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<AgencyWorkerRequestHistoryItem>> {
  return api.get<PaginatedList<AgencyWorkerRequestHistoryItem>>(
    `/api/agency/recruiting/workers/${workerId}/RequestHistory?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

// Worker holidays
export function getAgencyWorkerProfileHolidays(workerProfileId: string): Promise<AgencyWorkerHoliday[]> {
  return api.get<AgencyWorkerHoliday[]>(`/api/agency/recruiting/workers/${workerProfileId}/Holidays`);
}

export function addUpdateAgencyWorkerProfileHolidays(workerProfileId: string, data: AgencyWorkerHoliday): Promise<void> {
  return api.post(`/api/agency/recruiting/workers/${workerProfileId}/Holidays`, data);
}

export function addNewHoliday(workerProfileId: string, payload: AddNewHolidayPayload): Promise<void> {
  return api.post(`/api/agency/recruiting/workers/${workerProfileId}/Holidays/new-holiday`, payload);
}
