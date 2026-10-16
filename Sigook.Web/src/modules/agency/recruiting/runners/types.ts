import type { RequestStatus } from '@/shared/constants/enums';
import type { PaginatedList } from '@/shared/types/common';
import type { RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';

// What RunnersGrid needs from a row, whether it comes from the request's own
// list (RunnerListItem) or from the cross-request board (AgencyRunner).
export interface RunnerRow {
  id: string;
  requestId: string;
  workerProfileId: string;
  name: string;
  email?: string;
  type: RunnerType;
  status: RunnerStatus;
  startDate?: string | null;
  interviewsCount: number;
  lastInterviewAt?: string | null;
  createdAt: string;
  createdByEmail?: string;
}

// Requests always come sorted by their number; this sorts the runners inside.
export enum AgencyRunnerSortBy {
  Name = 0,
  CreatedAt = 1,
  Status = 2,
}

// Filter for GET /api/agency/recruiting/runners. Mirrors GetAgencyRunnersFilter.
export interface AgencyRunnersFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: AgencyRunnerSortBy;
  name?: string;
  numberId?: number | null;
  jobTitle?: string;
  companyProfileId?: string | null;
  type?: RunnerType | null;
  statuses?: RunnerStatus[];
  requestStatuses?: RequestStatus[];
  onlyMine?: boolean;
  startAtFrom?: string | null;
  startAtTo?: string | null;
}

// One runner of any request of the agency. Mirrors AgencyRunnerListModel.
export interface AgencyRunner extends RunnerRow {
  numberId: number;
  createdBy: string;
  requestNumberId: number;
  companyFullName: string;
  jobTitle: string;
  startAt?: string | null;
  isAsap: boolean;
  isDirectHiring: boolean;
  usesRunners: boolean;
}

// One request with every runner of it that matches the filter.
// Mirrors AgencyRequestRunnersModel.
export interface AgencyRequestRunners {
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
  usesRunners: boolean;
  workersQuantity: number;
  hiredRunners: number;
  totalRunners: number;
  runners: AgencyRunner[];
}

// Mirrors AgencyRunnersPagedResponse: a page of REQUESTS, so a request never
// gets split across pages. totalRunners counts the whole filter.
export interface AgencyRunnersPagedResponse extends PaginatedList<AgencyRequestRunners> {
  totalRunners: number;
}

// Body for PUT /api/agency/recruiting/runners/Status. Mirrors ChangeRunnersStatusModel.
export interface ChangeRunnersStatusModel {
  runnerIds: string[];
  status: RunnerStatus;
  comments?: string;
  startDate?: string | null;
}

export interface SkippedRunner {
  runnerId: string;
  reason: string;
}

// The endpoint changes what it can and reports the rest instead of failing.
export interface ChangeRunnersStatusResult {
  updated: number;
  skipped: SkippedRunner[];
}
