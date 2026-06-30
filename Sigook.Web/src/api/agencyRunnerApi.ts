import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { ApplicantSearchResult } from '@/types/agency';
import type {
  AgencyRunnerFilter,
  RunnerListItem,
  RunnerDetail,
  CreateRunnerModel,
  ChangeRunnerStatusModel,
  CreateRunnerInterviewModel,
  RescheduleRunnerInterviewModel,
} from '@/types/runner';

const base = (requestId: string): string => `/api/agency/requests/${requestId}/Runners`;

export function getAgencyRunners(requestId: string, filter: AgencyRunnerFilter): Promise<PaginatedList<RunnerListItem>> {
  return http.get(base(requestId), { params: { ...filter } }).then(r => r.data);
}

export function searchAgencyRunnerProspects(requestId: string, searchTerm: string): Promise<ApplicantSearchResult[]> {
  return http.get(`${base(requestId)}/Search`, { params: { searchTerm } }).then(r => r.data);
}

export function getAgencyRunner(requestId: string, id: string): Promise<RunnerDetail> {
  return http.get(`${base(requestId)}/${id}`).then(r => r.data);
}

export function createAgencyRunner(requestId: string, model: CreateRunnerModel): Promise<string> {
  return http.post(base(requestId), model).then(r => r.data);
}

export function changeRunnerStatus(requestId: string, id: string, model: ChangeRunnerStatusModel): Promise<void> {
  return http.put(`${base(requestId)}/${id}/Status`, model).then(() => {});
}

export function createRunnerInterview(requestId: string, id: string, model: CreateRunnerInterviewModel): Promise<string> {
  return http.post(`${base(requestId)}/${id}/Interview`, model).then(r => r.data);
}

export function rescheduleRunnerInterview(requestId: string, id: string, interviewId: string, model: RescheduleRunnerInterviewModel): Promise<void> {
  return http.put(`${base(requestId)}/${id}/Interview/${interviewId}/Reschedule`, model).then(() => {});
}
