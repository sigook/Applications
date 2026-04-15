import http from '@/security/apiService';
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

// Profile
export function getCompanyProfile(): Promise<CompanyProfileDetail> {
  return http.get('/api/CompanyProfile').then(r => r.data);
}

export function updateProfile(id: string, company: CompanyProfileDetail): Promise<void> {
  return http.put(`/api/CompanyProfile/${id}`, company).then(() => {});
}

export function registerCompany(company: CompanyProfileDetail): Promise<void> {
  return http.post('/api/CompanyProfile', company).then(() => {});
}

// Profile Locations
export function getProfileLocations(): Promise<CompanyProfileLocationDetail[]> {
  return http.get('/api/CompanyProfile/Location').then(r => r.data);
}

export function createProfileLocation(model: CompanyProfileLocationDetail): Promise<void> {
  return http.post('/api/CompanyProfile/Location', model).then(() => {});
}

export function updateProfileLocation(id: string, model: CompanyProfileLocationDetail): Promise<void> {
  return http.put(`/api/CompanyProfile/Location/${id}`, model).then(() => {});
}

export function deleteProfileLocation(id: string): Promise<void> {
  return http.delete(`/api/CompanyProfile/Location/${id}`).then(() => {});
}

// Locations
export function getLocations(): Promise<CompanyProfileLocationDetail[]> {
  return http.get('/api/CompanyLocation').then(r => r.data);
}

// Job Positions
export function getCompanyJobPositions(): Promise<CompanyProfileJobPositionRate[]> {
  return http.get('/api/CompanyJobPosition').then(r => r.data);
}

export function getCompanyJobPositionById(id: string): Promise<CompanyProfileJobPositionRate> {
  return http.get(`/api/CompanyJobPosition/${id}`).then(r => r.data);
}

export function requestNewPosition(data: { title: string; name: string; email: string; phone: string; message: string; subject: string }): Promise<void> {
  return http.post('/api/CompanyJobPosition/request-new-position', data).then(() => {});
}

// Requests
export function getRequests(filter: CompanyRequestFilter): Promise<PaginatedList<CompanyRequestListItem>> {
  return http.get('/api/CompanyRequest', { params: { ...filter } }).then(r => r.data);
}

export function getRequest(id: string): Promise<CompanyRequestListItem> {
  return http.get(`/api/CompanyRequest/${id}`).then(r => r.data);
}

export function createRequest(request: CreateAgencyRequestModel): Promise<{ id: string }> {
  return http.post('/api/CompanyRequest', request).then(r => r.data);
}

export function editRequest(id: string, model: { requirements: string }): Promise<void> {
  return http.put(`/api/CompanyRequest/${id}`, model).then(() => {});
}

export function cancelRequest(id: string, cancellationReasonId: string, otherCancellationReason: string): Promise<void> {
  return http.put(`/api/CompanyRequest/${id}/Cancel`, { cancellationReasonId, otherCancellationReason }).then(() => {});
}

// Request Workers
export function getRequestWorkers(filter: CompanyRequestWorkerFilter): Promise<PaginatedList<CompanyRequestWorker>> {
  return http.get(`/api/CompanyRequest/${filter.requestId}/Worker`, { params: { ...filter } }).then(r => r.data);
}

export function getRequestWorker(requestId: string, workerId: string): Promise<CompanyRequestWorker> {
  return http.get(`/api/CompanyRequest/${requestId}/Worker/${workerId}`).then(r => r.data);
}

export function rejectCompanyRequestWorker(requestId: string, workerId: string, model: CommentsModel): Promise<void> {
  return http.put(`/api/CompanyRequest/${requestId}/Worker/${workerId}/Reject`, model).then(() => {});
}

export function requestAnotherWorker(requestId: string, comment: CommentsModel): Promise<void> {
  return http.post(`/api/CompanyRequest/${requestId}/Worker/RequestNewWorker`, comment).then(() => {});
}

// TimeSheet
export function getCompanyWorkerTimeSheetByDate(requestId: string, workerId: string, date: { startDate: string; endDate: string }): Promise<TimeSheetListItem[]> {
  return http.get(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`, { params: { ...date } }).then(r => r.data);
}

export function postCompanyWorkerTimeSheet(requestId: string, workerId: string, model: TimeSheetModel): Promise<{ id: string }> {
  return http.post(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`, model).then(r => r.data);
}

export function validateHoursTimeSheet(requestId: string, workerId: string, id: string, model: TimeSheetModel): Promise<void> {
  return http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model).then(() => {});
}

export function validateAllHoursTimeSheet(requestId: string, workerId: string): Promise<void> {
  return http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet`).then(() => {});
}

export function updateCompanyRequestWorkerTimeSheet(requestId: string, workerId: string, id: string, model: TimeSheetModel): Promise<void> {
  return http.put(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`, model).then(() => {});
}

export function deleteCompanyWorkerTimeSheet(requestId: string, workerId: string, id: string): Promise<void> {
  return http.delete(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/${id}`).then(() => {});
}

export function companyTimeSheetClockIn(requestId: string, workerId: string, model: ClockInModel): Promise<ClockInResult> {
  return http.post(`/api/v2/CompanyRequest/${requestId}/Worker/${workerId}/TimeSheet/ClockIn`, model).then(r => r.data);
}

// Comment worker (called dynamically)
export function companyCommentWorker(id: string, comment: CommentsModel): Promise<void> {
  return http.post(`/api/CompanyWorker/${id}/Comment`, comment).then(() => {});
}

// Company Users
export function getCompanyUser(): Promise<CompanyUserModel[]> {
  return http.get('/api/CompanyUser').then(r => r.data);
}

export function getCompanyUserDetail(): Promise<CompanyUserModel> {
  return http.get('/api/CompanyUser/detail').then(r => r.data);
}

export function createCompanyUser(model: CreateCompanyUserModel): Promise<void> {
  return http.post('/api/CompanyUser', model).then(() => {});
}

export function updateCompanyUser(id: string, user: CompanyUserModel): Promise<void> {
  return http.put(`/api/CompanyUser/${id}`, user).then(() => {});
}

export function deleteCompanyUser(id: string): Promise<void> {
  return http.delete(`/api/CompanyUser/${id}`).then(() => {});
}

// Contact People
export function getContactPeople(): Promise<CompanyContactPersonModel[]> {
  return http.get('/api/CompanyProfileContactPerson').then(r => r.data);
}

export function saveContactPerson(model: CompanyContactPersonModel): Promise<void> {
  return http.post('/api/CompanyProfileContactPerson', model).then(() => {});
}

export function deleteContactPerson(id: string): Promise<void> {
  return http.delete(`/api/CompanyProfileContactPerson/${id}`).then(() => {});
}

// Invoices
export function getCompanyInvoice(filter: CompanyInvoiceFilter): Promise<PaginatedList<CompanyInvoiceListItem>> {
  return http.get('/api/CompanyInvoice', { params: { ...filter } }).then(r => r.data);
}

export function getCompanyInvoiceDetail(id: string): Promise<InvoiceSummaryModel> {
  return http.get(`/api/CompanyInvoice/${id}`).then(r => r.data);
}
