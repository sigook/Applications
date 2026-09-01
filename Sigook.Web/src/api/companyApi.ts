import { api } from '@/security/apiService';
import { buildMultipartFormData } from '@/utils/multipart';
import type { PaginatedList } from '@/types/common';
import type { CreateAgencyRequestModel, RequestShiftModel } from '@/types/agency';
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
  Deal,
  DealFilter,
  CreateDealModel,
  UpdateDealModel,
  CompanyInteraction,
  CompanyInteractionFilter,
  CreateCompanyInteractionModel,
  UpdateCompanyInteractionModel,
} from '@/types/company';
import type { WorkerCommentCreateModel } from '@/types/worker';

// Profile
export function getCompanyProfile(): Promise<CompanyProfileDetail> {
  return api.get<CompanyProfileDetail>('/api/Company');
}

export function updateProfile(id: string, company: CompanyProfileDetail): Promise<void> {
  return api.put(`/api/Company/${id}`, company);
}

export function registerCompany(company: CompanyProfileDetail): Promise<void> {
  return api.post('/api/Company', company);
}

// Profile Locations
export function getProfileLocations(): Promise<CompanyProfileLocationDetail[]> {
  return api.get<CompanyProfileLocationDetail[]>('/api/company/profile/Locations');
}

export function createProfileLocation(model: CompanyProfileLocationDetail): Promise<void> {
  return api.post('/api/company/profile/Locations', model);
}

export function updateProfileLocation(id: string, model: CompanyProfileLocationDetail): Promise<void> {
  return api.put(`/api/company/profile/Locations/${id}`, model);
}

export function deleteProfileLocation(id: string): Promise<void> {
  return api.del(`/api/company/profile/Locations/${id}`);
}

// Job Positions
export function getCompanyJobPositions(): Promise<CompanyProfileJobPositionRate[]> {
  return api.get<CompanyProfileJobPositionRate[]>('/api/company/profile/JobPositions');
}

export function getCompanyJobPositionById(id: string): Promise<CompanyProfileJobPositionRate> {
  return api.get<CompanyProfileJobPositionRate>(`/api/company/profile/JobPositions/${id}`);
}

export function requestNewPosition(data: { title: string; name: string; email: string; phone: string; message: string; subject: string }): Promise<void> {
  return api.post('/api/company/profile/JobPositions/request-new-position', data);
}

// Requests
export function getRequests(filter: CompanyRequestFilter): Promise<PaginatedList<CompanyRequestListItem>> {
  return api.get<PaginatedList<CompanyRequestListItem>>('/api/company/requests', { params: { ...filter } });
}

export function getRequest(id: string): Promise<CompanyRequestListItem> {
  return api.get<CompanyRequestListItem>(`/api/company/requests/${id}`);
}

export function createRequest(request: CreateAgencyRequestModel): Promise<{ id: string }> {
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

export function getRequestWorker(requestId: string, workerProfileId: string): Promise<CompanyRequestWorker> {
  return api.get<CompanyRequestWorker>(`/api/company/requests/${requestId}/Workers/${workerProfileId}`);
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

export function validateAllHoursTimeSheet(requestId: string, workerProfileId: string): Promise<void> {
  return api.put(`/api/company/requests/${requestId}/Workers/${workerProfileId}/TimeSheets`);
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

// Comment worker (called dynamically)
export function companyCommentWorker(workerProfileId: string, comment: WorkerCommentCreateModel): Promise<void> {
  return api.post(`/api/company/workers/${workerProfileId}/Comments`, comment);
}

// Company Users
export function getCompanyUser(): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>('/api/company/Users');
}

export function getCompanyUserDetail(): Promise<CompanyUserModel> {
  return api.get<CompanyUserModel>('/api/company/Users/detail');
}

export function createCompanyUser(model: CreateCompanyUserModel): Promise<void> {
  return api.post('/api/company/Users', model);
}

export function updateCompanyUser(id: string, user: CompanyUserModel): Promise<void> {
  return api.put(`/api/company/Users/${id}`, user);
}

export function deleteCompanyUser(id: string): Promise<void> {
  return api.del(`/api/company/Users/${id}`);
}

// Contact People
export function getContactPeople(): Promise<CompanyContactPersonModel[]> {
  return api.get<CompanyContactPersonModel[]>('/api/company/profile/ContactPeople');
}

export function saveContactPerson(model: CompanyContactPersonModel): Promise<void> {
  return api.post('/api/company/profile/ContactPeople', model);
}

export function deleteContactPerson(id: string): Promise<void> {
  return api.del(`/api/company/profile/ContactPeople/${id}`);
}

// Invoices
export function getCompanyInvoice(filter: CompanyInvoiceFilter): Promise<PaginatedList<CompanyInvoiceListItem>> {
  return api.get<PaginatedList<CompanyInvoiceListItem>>('/api/company/accounting/Invoices', { params: { ...filter } });
}

export function getCompanyInvoiceDetail(id: string): Promise<InvoiceSummaryModel> {
  return api.get<InvoiceSummaryModel>(`/api/company/accounting/Invoices/${id}`);
}

// Request timesheets
export function getCompanyRequestTimeSheetFile(requestId: string): Promise<Blob> {
  return api.get<Blob>(`/api/company/requests/${requestId}/TimeSheets/File`, { responseType: 'blob' });
}

// Sales - Deals
const dealsBase = '/api/agency/sales/deals';

export function getDeals(filter: DealFilter): Promise<PaginatedList<Deal>> {
  return api.get<PaginatedList<Deal>>(dealsBase, { params: { ...filter } });
}

export function createDeal(model: CreateDealModel, file?: File | null): Promise<string> {
  return api.post<string>(
    dealsBase,
    buildMultipartFormData(model, file && model.fileName ? { [model.fileName]: file } : {}),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function updateDeal(id: string, model: UpdateDealModel): Promise<void> {
  return api.put(`${dealsBase}/${id}`, model);
}

export function deleteDeal(id: string): Promise<void> {
  return api.del(`${dealsBase}/${id}`);
}

// Sales - Company Interactions
const companyInteractionsBase = '/api/agency/sales/companyinteractions';

export function getCompanyInteractions(filter: CompanyInteractionFilter): Promise<PaginatedList<CompanyInteraction>> {
  return api.get<PaginatedList<CompanyInteraction>>(companyInteractionsBase, { params: { ...filter } });
}

export function createCompanyInteraction(model: CreateCompanyInteractionModel): Promise<string> {
  return api.post<string>(companyInteractionsBase, model);
}

export function updateCompanyInteraction(id: string, model: UpdateCompanyInteractionModel): Promise<void> {
  return api.put(`${companyInteractionsBase}/${id}`, model);
}

export function deleteCompanyInteraction(id: string): Promise<void> {
  return api.del(`${companyInteractionsBase}/${id}`);
}
