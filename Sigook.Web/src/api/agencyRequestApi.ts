import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyRequestFilter,
  AgencyRequestListItem,
  AgencyRequestDetail,
  CreateAgencyRequestModel,
  RequestShiftModel,
  CancelRequestPayload,
  BulkCancelRequestsPayload,
  BulkCancelRequestsResult,
  BulkUpdateRecruitersPayload,
  AgencyRequestWorkerFilter,
  AgencyRequestWorker,
  BookWorkerModel,
  RejectWorkerModel,
  AgencyRequestApplicantFilter,
  AgencyRequestApplicant,
  CreateRequestApplicantModel,
  UpdateApplicantCommentsPayload,
  AgencyRequestRecruiterModel,
  AgencyRequestRecruiterItem,
  AgencyRequestSkillModel,
  AgencyRequestPersonItem,
} from '@/types/agency';

// ---------------------------------------------------------------------------
// Request CRUD
// ---------------------------------------------------------------------------

export function postAgencyRequest(model: CreateAgencyRequestModel): Promise<AgencyRequestDetail> {
  return http.post('api/AgencyRequest', model).then(r => r.data);
}

export function getAgencyRequests(filter: AgencyRequestFilter): Promise<PaginatedList<AgencyRequestListItem>> {
  return http.get('/api/AgencyRequest', { params: { ...filter } }).then(r => r.data);
}

export function getAllAgencyRequests(filter: AgencyRequestFilter): Promise<PaginatedList<AgencyRequestListItem>> {
  return http.get('/api/AgencyRequest/all', { params: { ...filter } }).then(r => r.data);
}

export function getAgencyRequest(id: string): Promise<AgencyRequestDetail> {
  return http.get(`/api/AgencyRequest/${id}`).then(r => r.data);
}

export function updateAgencyRequest(requestId: string, model: CreateAgencyRequestModel): Promise<AgencyRequestDetail> {
  return http.put(`/api/AgencyRequest/${requestId}`, model).then(r => r.data);
}

export function cancelAgencyRequest(id: string, payload: CancelRequestPayload): Promise<void> {
  return http.put(`/api/AgencyRequest/${id}/Cancel`, payload).then(() => {});
}

export function bulkCancelRequests(payload: BulkCancelRequestsPayload): Promise<BulkCancelRequestsResult> {
  return http.put('/api/AgencyRequest/bulk-cancel', payload).then(r => r.data);
}

export function bulkUpdateRecruiters(payload: BulkUpdateRecruitersPayload): Promise<void> {
  return http.put('/api/AgencyRequest/bulk-recruiters', payload).then(() => {});
}

export function agencyRequestOpen(id: string): Promise<void> {
  return http.put(`/api/AgencyRequest/${id}/Open`, id).then(() => {});
}

export function agencyRequestSendInvitation(requestId: string): Promise<void> {
  return http.post(`/api/AgencyRequest/${requestId}/SendInvitation`).then(() => {});
}

export function updateAgencyRequestIsAsap(requestId: string): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/IsAsap`).then(() => {});
}

export function updateAgencyPunchCardVisibilityStatusInApp(requestId: string): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/PunchCardVisibilityStatusInApp`).then(() => {});
}

export function updateAgencyRequestShift(requestId: string, model: RequestShiftModel): Promise<{ id: string; displayShift?: string }> {
  return http.put(`/api/AgencyRequest/${requestId}/Shift`, model).then(r => r.data);
}

export function increaseWorkersQuantityByOne(requestId: string): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/IncreaseWorkersQuantityByOne`).then(() => {});
}

export function reduceWorkersQuantityByOne(requestId: string): Promise<void> {
  return http.put(`api/AgencyRequest/${requestId}/ReduceWorkersQuantityByOne`).then(() => {});
}

// ---------------------------------------------------------------------------
// Request → Workers
// ---------------------------------------------------------------------------

export function getAgencyRequestsWorkers(filter: AgencyRequestWorkerFilter): Promise<PaginatedList<AgencyRequestWorker>> {
  return http.get(`/api/AgencyRequest/${filter.requestId}/Worker`, { params: { ...filter } }).then(r => r.data);
}

export function bookAgencyRequestWorker(requestId: string, workerId: string, model: BookWorkerModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyRequest/${requestId}/Worker/${workerId}/Book`, model).then(r => r.data);
}

export function updateAgencyRequestWorkerStartDate(requestId: string, id: string, model: BookWorkerModel): Promise<void> {
  return http.put(`api/AgencyRequest/${requestId}/Worker/${id}`, model).then(() => {});
}

export function rejectAgencyRequestWorker(requestId: string, workerId: string, model: RejectWorkerModel): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/Worker/${workerId}/Reject`, model).then(() => {});
}

// ---------------------------------------------------------------------------
// Applicants
// ---------------------------------------------------------------------------

export function searchAgencyRequestApplicants(requestId: string, searchTerm: string): Promise<AgencyRequestApplicant[]> {
  return http.get(`/api/AgencyRequest/${requestId}/Applicant/Search`, { params: { searchTerm } }).then(r => r.data);
}

export function getAgencyRequestApplicant(filter: AgencyRequestApplicantFilter): Promise<PaginatedList<AgencyRequestApplicant>> {
  return http.get(`/api/AgencyRequest/${filter.requestId}/Applicant`, { params: { ...filter } }).then(r => r.data);
}

export function postAgencyRequestApplicant(requestId: string, model: CreateRequestApplicantModel): Promise<AgencyRequestApplicant> {
  return http.post(`/api/AgencyRequest/${requestId}/Applicant`, model).then(r => r.data);
}

export function deleteAgencyRequestApplicant(requestId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/Applicant/${id}`).then(() => {});
}

export function updateAgencyRequestApplicant(requestId: string, id: string, model: UpdateApplicantCommentsPayload): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/Applicant/${id}`, model).then(() => {});
}

// ---------------------------------------------------------------------------
// Requested by / Report to (contact persons attached to a request)
// ---------------------------------------------------------------------------

export function getAgencyRequestRequestedBy(requestId: string): Promise<PaginatedList<AgencyRequestPersonItem>> {
  return http.get(`/api/AgencyRequest/${requestId}/RequestedBy`).then(r => r.data);
}

export function postAgencyRequestRequestedBy(requestId: string, contactPersonId: string): Promise<void> {
  return http.post(`/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`).then(() => {});
}

export function deleteAgencyRequestRequestedBy(requestId: string, contactPersonId: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/RequestedBy/${contactPersonId}`).then(() => {});
}

export function getAgencyRequestReportTo(requestId: string): Promise<PaginatedList<AgencyRequestPersonItem>> {
  return http.get(`/api/AgencyRequest/${requestId}/ReportTo`).then(r => r.data);
}

export function postAgencyRequestReportTo(requestId: string, contactPersonId: string): Promise<void> {
  return http.post(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`).then(() => {});
}

export function deleteAgencyRequestReportTo(requestId: string, contactPersonId: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/ReportTo/${contactPersonId}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Recruiters
// ---------------------------------------------------------------------------

export function getAgencyRequestRecruiter(requestId: string): Promise<PaginatedList<AgencyRequestRecruiterItem>> {
  return http.get(`/api/AgencyRequest/${requestId}/Recruiter`).then(r => r.data);
}

export function postAgencyRequestRecruiter(requestId: string, model: AgencyRequestRecruiterModel): Promise<void> {
  return http.post(`/api/AgencyRequest/${requestId}/Recruiter`, model).then(() => {});
}

export function deleteAgencyRequestRecruiter(requestId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/Recruiter/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function getAgencyRequestSkill(requestId: string): Promise<{ id: string; skill: string }[]> {
  return http.get(`/api/AgencyRequest/${requestId}/Skill`).then(r => r.data);
}

export function postAgencyRequestSkill(requestId: string, model: AgencyRequestSkillModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyRequest/${requestId}/Skill`, model).then(r => r.data);
}

export function deleteAgencyRequestSkill(requestId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/Skill/${id}`).then(() => {});
}
