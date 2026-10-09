import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { RequestShiftModel } from '@/shared/request-detail/types';
import type { CompanyRequestFilter, CompanyRequestListItem, CompanyRequestDetail, CompanyRequestWorkerFilter, CompanyRequestWorker, ClockInModel, ClockInResult, CommentsModel, CreateCompanyRequestModel } from '@/modules/company/requests/types';
import type { TimeSheetListItem, TimeSheetModel } from '@/shared/punch-card/types';

// Requests
export function getRequests(filter: CompanyRequestFilter): Promise<PaginatedList<CompanyRequestListItem>> {
  return api.get<PaginatedList<CompanyRequestListItem>>('/api/company/requests', { params: { ...filter } });
}

export function getRequest(id: string): Promise<CompanyRequestDetail> {
  return api.get<CompanyRequestDetail>(`/api/company/requests/${id}`);
}

export function createRequest(request: CreateCompanyRequestModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/company/requests', request);
}

export function editRequest(id: string, model: { requirements: string }): Promise<void> {
  return api.put(`/api/company/requests/${id}`, model);
}

export function getRequestShift(requestId: string): Promise<RequestShiftModel> {
  return api.get<RequestShiftModel>(`/api/company/requests/${requestId}/Shift`);
}

export function cancelRequest(id: string, cancellationReasonId: string, otherCancellationReason: string): Promise<void> {
  return api.put(`/api/company/requests/${id}/Cancel`, { cancellationReasonId, otherCancellationReason });
}

// Request Workers
export function getRequestWorkers(filter: CompanyRequestWorkerFilter): Promise<PaginatedList<CompanyRequestWorker>> {
  return api.get<PaginatedList<CompanyRequestWorker>>(`/api/company/requests/${filter.requestId}/Workers`, { params: { ...filter } });
}

export function rejectCompanyRequestWorker(requestId: string, workerProfileId: string, model: CommentsModel): Promise<void> {
  return api.put(`/api/company/requests/${requestId}/Workers/${workerProfileId}/Reject`, model);
}

export function requestAnotherWorker(requestId: string, comment: CommentsModel): Promise<void> {
  return api.post(`/api/company/requests/${requestId}/Workers/RequestNewWorker`, comment);
}

// TimeSheet
export function getCompanyWorkerTimeSheetByDate(requestId: string, workerProfileId: string, date: { startDate: string; endDate: string }): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, { params: { ...date } });
}

export function postCompanyWorkerTimeSheet(requestId: string, workerProfileId: string, model: TimeSheetModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`, model);
}

export function validateHoursTimeSheet(requestId: string, workerProfileId: string, id: string, model: TimeSheetModel): Promise<void> {
  return api.put(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`, model);
}

export function updateCompanyRequestWorkerTimeSheet(requestId: string, workerProfileId: string, id: string, model: TimeSheetModel): Promise<void> {
  return api.put(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`, model);
}

export function deleteCompanyWorkerTimeSheet(requestId: string, workerProfileId: string, id: string): Promise<void> {
  return api.del(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/${id}`);
}

export function companyTimeSheetClockIn(requestId: string, workerProfileId: string, model: ClockInModel): Promise<ClockInResult> {
  return api.post<ClockInResult>(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets/ClockIn`, model);
}

// Request timesheets
export function getCompanyRequestTimeSheetFile(requestId: string): Promise<Blob> {
  return api.get<Blob>(`/api/company/requests/${requestId}/TimeSheets/File`, { responseType: 'blob' });
}
