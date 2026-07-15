import { api } from '@/security/apiService';
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
  AgencyRequestWorkerFilter,
  AgencyRequestWorker,
  BookWorkerModel,
  RejectWorkerModel,
  AgencyRequestApplicantFilter,
  AgencyRequestApplicant,
  ApplicantSearchResult,
  CreateRequestApplicantModel,
  UpdateApplicantCommentsPayload,
  AgencyRequestSkillModel,
  AgencyRequestPersonItem,
  RequestJobBoard,
  SetRequestJobBoardItem,
  AgencyRequestsPagedResponse,
} from '@/types/agency';

const requestsUrl = '/api/agency/requests';
const recruitingRequestsUrl = '/api/agency/recruiting/requests';

// ---------------------------------------------------------------------------
// Request CRUD
// ---------------------------------------------------------------------------

export function postAgencyRequest(model: CreateAgencyRequestModel): Promise<AgencyRequestDetail> {
  return api.post<AgencyRequestDetail>(requestsUrl, model);
}

export function getAgencyRequests(filter: AgencyRequestFilter): Promise<AgencyRequestsPagedResponse> {
  return api.get<AgencyRequestsPagedResponse>(recruitingRequestsUrl, { params: { ...filter } });
}

export function getAllAgencyRequests(filter: AgencyRequestFilter): Promise<AgencyRequestListItem[]> {
  return api.get<AgencyRequestListItem[]>(`${recruitingRequestsUrl}/all`, { params: { ...filter } });
}

export function getAgencyRequest(id: string): Promise<AgencyRequestDetail> {
  return api.get<AgencyRequestDetail>(`${requestsUrl}/${id}`);
}

export function updateAgencyRequest(requestId: string, model: CreateAgencyRequestModel): Promise<AgencyRequestDetail> {
  return api.put<AgencyRequestDetail>(`${requestsUrl}/${requestId}`, model);
}

export function cancelAgencyRequest(id: string, payload: CancelRequestPayload): Promise<void> {
  return api.put(`${requestsUrl}/${id}/Cancel`, payload);
}

export function bulkCancelRequests(payload: BulkCancelRequestsPayload): Promise<BulkCancelRequestsResult> {
  return api.put<BulkCancelRequestsResult>(`${requestsUrl}/bulk-cancel`, payload);
}

export function agencyRequestOpen(id: string): Promise<void> {
  return api.put(`${requestsUrl}/${id}/Open`, id);
}

export function agencyRequestSendInvitation(requestId: string): Promise<void> {
  return api.post(`${requestsUrl}/${requestId}/SendInvitation`, undefined, { timeout: 120_000 });
}

export function updateAgencyRequestIsAsap(requestId: string): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/IsAsap`);
}

export function updateAgencyPunchCardVisibilityStatusInApp(requestId: string): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/PunchCardVisibilityStatusInApp`);
}

export function updateAgencyRequestShift(requestId: string, model: RequestShiftModel): Promise<{ id: string; displayShift?: string }> {
  return api.put<{ id: string; displayShift?: string }>(`${requestsUrl}/${requestId}/Shift`, model);
}

export function increaseWorkersQuantityByOne(requestId: string): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/IncreaseWorkersQuantityByOne`);
}

export function reduceWorkersQuantityByOne(requestId: string): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/ReduceWorkersQuantityByOne`);
}

// ---------------------------------------------------------------------------
// Request → Workers
// ---------------------------------------------------------------------------

export function getAgencyRequestsWorkers(filter: AgencyRequestWorkerFilter): Promise<PaginatedList<AgencyRequestWorker>> {
  return api.get<PaginatedList<AgencyRequestWorker>>(`${requestsUrl}/${filter.requestId}/Workers`, { params: { ...filter } });
}

export function bookAgencyRequestWorker(requestId: string, workerId: string, model: BookWorkerModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${requestsUrl}/${requestId}/Workers/${workerId}/Book`, model);
}

export function updateAgencyRequestWorkerStartDate(requestId: string, id: string, model: BookWorkerModel): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/Workers/${id}`, model);
}

export function rejectAgencyRequestWorker(requestId: string, workerId: string, model: RejectWorkerModel): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/Workers/${workerId}/Reject`, model);
}

// ---------------------------------------------------------------------------
// Applicants
// ---------------------------------------------------------------------------

export function searchAgencyRequestApplicants(requestId: string, searchTerm: string): Promise<ApplicantSearchResult[]> {
  return api.get<ApplicantSearchResult[]>(`${requestsUrl}/${requestId}/Applicants/Search`, { params: { searchTerm } });
}

export function getAgencyRequestApplicant(filter: AgencyRequestApplicantFilter): Promise<PaginatedList<AgencyRequestApplicant>> {
  return api.get<PaginatedList<AgencyRequestApplicant>>(`${requestsUrl}/${filter.requestId}/Applicants`, { params: { ...filter } });
}

export function postAgencyRequestApplicant(requestId: string, model: CreateRequestApplicantModel): Promise<AgencyRequestApplicant> {
  return api.post<AgencyRequestApplicant>(`${requestsUrl}/${requestId}/Applicants`, model);
}

export function deleteAgencyRequestApplicant(requestId: string, id: string): Promise<void> {
  return api.del(`${requestsUrl}/${requestId}/Applicants/${id}`);
}

export function updateAgencyRequestApplicant(requestId: string, id: string, model: UpdateApplicantCommentsPayload): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/Applicants/${id}`, model);
}

// ---------------------------------------------------------------------------
// Requested by / Report to (contact persons attached to a request)
// ---------------------------------------------------------------------------

export function getAgencyRequestRequestedBy(requestId: string): Promise<PaginatedList<AgencyRequestPersonItem>> {
  return api.get<PaginatedList<AgencyRequestPersonItem>>(`${requestsUrl}/${requestId}/RequestedBy`);
}

export function postAgencyRequestRequestedBy(requestId: string, contactPersonId: string): Promise<void> {
  return api.post(`${requestsUrl}/${requestId}/RequestedBy/${contactPersonId}`);
}

export function deleteAgencyRequestRequestedBy(requestId: string, contactPersonId: string): Promise<void> {
  return api.del(`${requestsUrl}/${requestId}/RequestedBy/${contactPersonId}`);
}

export function getAgencyRequestReportTo(requestId: string): Promise<PaginatedList<AgencyRequestPersonItem>> {
  return api.get<PaginatedList<AgencyRequestPersonItem>>(`${requestsUrl}/${requestId}/ReportTo`);
}

export function postAgencyRequestReportTo(requestId: string, contactPersonId: string): Promise<void> {
  return api.post(`${requestsUrl}/${requestId}/ReportTo/${contactPersonId}`);
}

export function deleteAgencyRequestReportTo(requestId: string, contactPersonId: string): Promise<void> {
  return api.del(`${requestsUrl}/${requestId}/ReportTo/${contactPersonId}`);
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function getAgencyRequestSkill(requestId: string): Promise<{ id: string; skill: string }[]> {
  return api.get<{ id: string; skill: string }[]>(`${requestsUrl}/${requestId}/Skills`);
}

export function postAgencyRequestSkill(requestId: string, model: AgencyRequestSkillModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${requestsUrl}/${requestId}/Skills`, model);
}

export function deleteAgencyRequestSkill(requestId: string, id: string): Promise<void> {
  return api.del(`${requestsUrl}/${requestId}/Skills/${id}`);
}

// ---------------------------------------------------------------------------
// Job Boards (Sources where the request is published)
// ---------------------------------------------------------------------------

export function getAgencyRequestSources(requestId: string): Promise<RequestJobBoard[]> {
  return api.get<RequestJobBoard[]>(`${requestsUrl}/${requestId}/sources`);
}

export function setAgencyRequestSources(requestId: string, items: SetRequestJobBoardItem[]): Promise<void> {
  return api.put(`${requestsUrl}/${requestId}/sources`, items);
}
