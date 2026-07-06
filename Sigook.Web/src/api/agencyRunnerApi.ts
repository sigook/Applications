import { api } from '@/security/apiService';
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
  return api.get<PaginatedList<RunnerListItem>>(base(requestId), { params: { ...filter } });
}

export function searchAgencyRunnerProspects(requestId: string, searchTerm: string): Promise<ApplicantSearchResult[]> {
  return api.get<ApplicantSearchResult[]>(`${base(requestId)}/Search`, { params: { searchTerm } });
}

export function getAgencyRunner(requestId: string, id: string): Promise<RunnerDetail> {
  return api.get<RunnerDetail>(`${base(requestId)}/${id}`);
}

export function createAgencyRunner(requestId: string, model: CreateRunnerModel): Promise<string> {
  return api.post<string>(base(requestId), model);
}

export function changeRunnerStatus(requestId: string, id: string, model: ChangeRunnerStatusModel): Promise<void> {
  return api.put(`${base(requestId)}/${id}/Status`, model);
}

export function createRunnerInterview(requestId: string, id: string, model: CreateRunnerInterviewModel): Promise<string> {
  return api.post<string>(`${base(requestId)}/${id}/Interview`, model);
}

export function rescheduleRunnerInterview(requestId: string, id: string, interviewId: string, model: RescheduleRunnerInterviewModel): Promise<void> {
  return api.put(`${base(requestId)}/${id}/Interview/${interviewId}/Reschedule`, model);
}
