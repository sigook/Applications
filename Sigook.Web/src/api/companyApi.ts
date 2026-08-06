import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { CreateAgencyRequestModel } from '@/types/agency';
import type { InvoiceSummaryModel } from '@/types/accounting';
import type {
  CompanyProfileDetail,
  CompanyProfileLocationDetail,
  CompanyProfileJobPositionRate,
  CompanyRequestFilter,
  CompanyRequestListItem,
  CompanyRequestWorkerFilter,
  CompanyRequestWorker,
  TimeSheetListItem,
  TimeSheetModel,
  ClockInModel,
  ClockInResult,
  CompanyUserModel,
  CreateCompanyUserModel,
  CompanyContactPersonModel,
  CompanyInvoiceFilter,
  CompanyInvoiceListItem,
  CommentsModel,
} from '@/types/company';
import type { WorkerCommentCreateModel } from '@/types/worker';

// Profile
export function getCompanyProfile(): Promise<CompanyProfileDetail> {
  return api.get<CompanyProfileDetail>('/api/CompanyProfile');
}

export function updateProfile(id: string, company: CompanyProfileDetail): Promise<void> {
  return api.put(`/api/CompanyProfile/${id}`, company);
}

export function registerCompany(company: CompanyProfileDetail): Promise<void> {
  return api.post('/api/CompanyProfile', company);
}

// Profile Locations
export function getProfileLocations(): Promise<CompanyProfileLocationDetail[]> {
  return api.get<CompanyProfileLocationDetail[]>('/api/CompanyProfile/Location');
}

export function createProfileLocation(model: CompanyProfileLocationDetail): Promise<void> {
  return api.post('/api/CompanyProfile/Location', model);
}

export function updateProfileLocation(id: string, model: CompanyProfileLocationDetail): Promise<void> {
  return api.put(`/api/CompanyProfile/Location/${id}`, model);
}

export function deleteProfileLocation(id: string): Promise<void> {
  return api.del(`/api/CompanyProfile/Location/${id}`);
}

// Locations
export function getLocations(): Promise<CompanyProfileLocationDetail[]> {
  return api.get<CompanyProfileLocationDetail[]>('/api/CompanyLocation');
}

// Job Positions
export function getCompanyJobPositions(): Promise<CompanyProfileJobPositionRate[]> {
  return api.get<CompanyProfileJobPositionRate[]>('/api/CompanyJobPosition');
}

export function getCompanyJobPositionById(id: string): Promise<CompanyProfileJobPositionRate> {
  return api.get<CompanyProfileJobPositionRate>(`/api/CompanyJobPosition/${id}`);
}

export function requestNewPosition(data: { title: string; name: string; email: string; phone: string; message: string; subject: string }): Promise<void> {
  return api.post('/api/CompanyJobPosition/request-new-position', data);
}

// Requests
export function getRequests(filter: CompanyRequestFilter): Promise<PaginatedList<CompanyRequestListItem>> {
  return api.get<PaginatedList<CompanyRequestListItem>>('/api/CompanyRequest', { params: { ...filter } });
}

export function getRequest(id: string): Promise<CompanyRequestListItem> {
  return api.get<CompanyRequestListItem>(`/api/CompanyRequest/${id}`);
}

export function createRequest(request: CreateAgencyRequestModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/CompanyRequest', request);
}

export function editRequest(id: string, model: { requirements: string }): Promise<void> {
  return api.put(`/api/CompanyRequest/${id}`, model);
}

export function cancelRequest(id: string, cancellationReasonId: string, otherCancellationReason: string): Promise<void> {
  return api.put(`/api/CompanyRequest/${id}/Cancel`, { cancellationReasonId, otherCancellationReason });
}

// Request Workers
export function getRequestWorkers(filter: CompanyRequestWorkerFilter): Promise<PaginatedList<CompanyRequestWorker>> {
  return api.get<PaginatedList<CompanyRequestWorker>>(`/api/CompanyRequest/${filter.requestId}/Worker`, { params: { ...filter } });
}

export function getRequestWorker(requestId: string, workerProfileId: string): Promise<CompanyRequestWorker> {
  return api.get<CompanyRequestWorker>(`/api/CompanyRequest/${requestId}/Worker/${workerProfileId}`);
}

export function rejectCompanyRequestWorker(requestId: string, workerProfileId: string, model: CommentsModel): Promise<void> {
  return api.put(`/api/CompanyRequest/${requestId}/Worker/${workerProfileId}/Reject`, model);
}

export function requestAnotherWorker(requestId: string, comment: CommentsModel): Promise<void> {
  return api.post(`/api/CompanyRequest/${requestId}/Worker/RequestNewWorker`, comment);
}

// TimeSheet
export function getCompanyWorkerTimeSheetByDate(requestId: string, workerProfileId: string, date: { startDate: string; endDate: string }): Promise<TimeSheetListItem[]> {
  return api.get<TimeSheetListItem[]>(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet`, { params: { ...date } });
}

export function postCompanyWorkerTimeSheet(requestId: string, workerProfileId: string, model: TimeSheetModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet`, model);
}

export function validateHoursTimeSheet(requestId: string, workerProfileId: string, id: string, model: TimeSheetModel): Promise<void> {
  return api.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet/${id}`, model);
}

export function validateAllHoursTimeSheet(requestId: string, workerProfileId: string): Promise<void> {
  return api.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet`);
}

export function updateCompanyRequestWorkerTimeSheet(requestId: string, workerProfileId: string, id: string, model: TimeSheetModel): Promise<void> {
  return api.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet/${id}`, model);
}

export function deleteCompanyWorkerTimeSheet(requestId: string, workerProfileId: string, id: string): Promise<void> {
  return api.del(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet/${id}`);
}

export function companyTimeSheetClockIn(requestId: string, workerProfileId: string, model: ClockInModel): Promise<ClockInResult> {
  return api.post<ClockInResult>(`/api/v2/CompanyRequest/${requestId}/Worker/${workerProfileId}/TimeSheet/ClockIn`, model);
}

// Comment worker (called dynamically)
export function companyCommentWorker(workerProfileId: string, comment: WorkerCommentCreateModel): Promise<void> {
  return api.post(`/api/CompanyWorker/${workerProfileId}/Comment`, comment);
}

// Company Users
export function getCompanyUser(): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>('/api/CompanyUser');
}

export function getCompanyUserDetail(): Promise<CompanyUserModel> {
  return api.get<CompanyUserModel>('/api/CompanyUser/detail');
}

export function createCompanyUser(model: CreateCompanyUserModel): Promise<void> {
  return api.post('/api/CompanyUser', model);
}

export function updateCompanyUser(id: string, user: CompanyUserModel): Promise<void> {
  return api.put(`/api/CompanyUser/${id}`, user);
}

export function deleteCompanyUser(id: string): Promise<void> {
  return api.del(`/api/CompanyUser/${id}`);
}

// Contact People
export function getContactPeople(): Promise<CompanyContactPersonModel[]> {
  return api.get<CompanyContactPersonModel[]>('/api/CompanyProfileContactPerson');
}

export function saveContactPerson(model: CompanyContactPersonModel): Promise<void> {
  return api.post('/api/CompanyProfileContactPerson', model);
}

export function deleteContactPerson(id: string): Promise<void> {
  return api.del(`/api/CompanyProfileContactPerson/${id}`);
}

// Invoices
export function getCompanyInvoice(filter: CompanyInvoiceFilter): Promise<PaginatedList<CompanyInvoiceListItem>> {
  return api.get<PaginatedList<CompanyInvoiceListItem>>('/api/CompanyInvoice', { params: { ...filter } });
}

export function getCompanyInvoiceDetail(id: string): Promise<InvoiceSummaryModel> {
  return api.get<InvoiceSummaryModel>(`/api/CompanyInvoice/${id}`);
}

// Request timesheets
export function getCompanyRequestTimeSheetFile(requestId: string): Promise<Blob> {
  return api.get<Blob>(`/api/CompanyRequest/${requestId}/TimeSheets/File`, { responseType: 'blob' });
}
