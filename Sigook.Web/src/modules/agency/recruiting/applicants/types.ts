import type { RequestStatus } from '@/shared/constants/enums';
import type { RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import type { PaginatedList } from '@/shared/types/common';

// Filter for GET /api/agency/recruiting/applicants. Mirrors GetAgencyApplicantsFilter.
export interface AgencyApplicantsFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: AgencyApplicantSortBy;
  name?: string;
  numberId?: number | null;
  jobTitle?: string;
  companyProfileId?: string | null;
  statuses?: RequestApplicantStatus[];
  requestStatuses?: RequestStatus[];
  onlyMine?: boolean;
  createdBy?: string;
  startAtFrom?: string | null;
  startAtTo?: string | null;
}

// Requests always come sorted by their number; this sorts the applicants inside.
export enum AgencyApplicantSortBy {
  Name = 0,
  CreatedAt = 1,
}

// One applicant of any request of the agency. Mirrors AgencyApplicantListModel.
export interface AgencyApplicant {
  id: string;
  candidateId?: string | null;
  workerProfileId?: string | null;
  workerId?: string | null;
  name: string;
  email?: string;
  phoneNumber?: string;
  comments?: string;
  status: RequestApplicantStatus;
  createdBy?: string;
  createdAt: string;
  requestId: string;
  requestNumberId: number;
  companyFullName: string;
  jobTitle: string;
  startAt?: string | null;
  isAsap: boolean;
  isDirectHiring: boolean;
  complianceTotal: number;
  complianceCompleted: number;
  mandatoryPending: number;
}

// One request with every applicant of it that matches the filter.
// Mirrors AgencyRequestApplicantsModel.
export interface AgencyRequestApplicants {
  requestId: string;
  numberId: number;
  companyFullName: string;
  jobTitle: string;
  city?: string | null;
  provinceName?: string | null;
  displayShift?: string | null;
  startAt?: string | null;
  isAsap: boolean;
  isDirectHiring: boolean;
  workersQuantity: number;
  confirmedApplicants: number;
  totalApplicants: number;
  applicants: AgencyApplicant[];
}

// Mirrors AgencyApplicantsPagedResponse: the page is a page of REQUESTS, so a
// request never gets split across pages. totalApplicants counts the whole filter.
export interface AgencyApplicantsPagedResponse extends PaginatedList<AgencyRequestApplicants> {
  totalApplicants: number;
}

// Body for PUT /api/agency/recruiting/applicants/Status. Mirrors ChangeApplicantsStatusModel.
export interface ChangeApplicantsStatusModel {
  applicantIds: string[];
  status: RequestApplicantStatus;
}

export interface SkippedApplicant {
  applicantId: string;
  reason: string;
}

// The endpoint changes what it can and reports the rest instead of failing.
export interface ChangeApplicantsStatusResult {
  updated: number;
  skipped: SkippedApplicant[];
}

// Body for PUT /api/AgencyRequest/{requestId}/Applicant/{id}. Mirrors CommentsModel.
export interface UpdateApplicantCommentsPayload {
  comments?: string;
}
