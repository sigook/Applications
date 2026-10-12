import type { GridParams } from '@/shared/types/common';
import type { RequestStatus, WorkerRequestStatus } from '@/shared/constants/enums';

export interface WorkerFilter {
  page: number;
  pageSize: number;
  approvedToWork?: boolean | null;
  dnu?: boolean | null;
  searchTerm?: string;
  skillIds?: string[];
  cityId?: string | null;
  sortBy?: string;
  sortDesc?: boolean;
}

// Worker Profile History types
// Mirrors backend GetWorkerPayStubHistoryFilter (SortBy = WorkerPayStubHistorySortBy).
export interface WorkerPayStubHistoryFilter extends GridParams {
  profileId: string;
  payStubNumber?: string;
  weekEndingFrom?: string | null;
  weekEndingTo?: string | null;
}

// Mirrors backend GetWorkerTimeSheetHistoryFilter (SortBy = WorkerTimeSheetHistorySortBy).
export interface WorkerTimeSheetHistoryFilter extends GridParams {
  profileId: string;
  companyName?: string;
  numberId?: number | string;
  dateFrom?: string | null;
  dateTo?: string | null;
}

// Mirrors backend PayStubItemHistoryModel.
export interface WorkerWageHistoryLine {
  description: string;
  quantity: number;
  total: number;
}

// Item returned by GET /api/agency/recruiting/workers/{id}/WageHistory. Mirrors backend PayStubHistoryModel.
export interface WorkerWageHistoryItem {
  id: string;
  numberId: number;
  payStubNumber: string;
  weekEnding: string;
  totalEarnings: number;
  vacations: number;
  totalPaid: number;
  start: string;
  end: string;
  accumulatedQuantity: number;
  accumulatedTotal: number;
  accumulatedVacations: number;
  accumulatedTotalEarnings: number;
  accumulatedTotalPaid: number;
  items: WorkerWageHistoryLine[];
  companies: string[];
}

// Item returned by GET /api/agency/recruiting/workers/{id}/TimeSheetHistory. Mirrors backend TimeSheetHistoryModel.
export interface WorkerTimeSheetHistoryItem {
  id: string;
  requestId: string;
  numberId: number;
  businessName: string;
  jobTitle: string;
  date: string;
  isHoliday: boolean;
  regularHours: number;
  holidayHours: number;
  overtimeHours: number;
  missingHours: number;
  missingHoursOvertime: number;
  totalHours: number;
  accumulatedRegularHours: number;
  accumulatedHolidayHours: number;
  accumulatedOvertimeHours: number;
  accumulatedMissingHours: number;
  accumulatedMissingHoursOvertime: number;
  accumulatedTotalHours: number;
}

// Filter for paginated worker list (agency view)
// Mirrors backend GetWorkerProfileFilter (Covenant.Common.Models.Worker)
export interface AgencyWorkerFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  fullName?: string;
  phone?: string;
  numberId?: number | string;
  externalId?: string;
  requestId?: string;
  location?: string;
  skills?: string;
  features?: number[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  companyProfileId?: string;
  approvedToWork?: boolean;
  isSubcontractor?: boolean | null;
}

// Item returned by GET /api/agency/recruiting/workers (paginated list)
export interface AgencyWorkerListItem {
  id: string;
  fullName: string;
  email: string;
  approvedToWork: boolean;
  dnu: boolean;
  numberId?: number;
  createdAt?: string;
}

// Lightweight item used by autocomplete (Dropdown endpoint)
export interface AgencyWorkerDropdownItem {
  id: string;
  workerProfileId: string;
  fullName: string;
  email: string;
  socialInsurance: string;
  approvedToWork: boolean;
}

// Worker comment payload (POST /api/agency/recruiting/workers/{id}/Comments)
// Worker email update payload
export interface UpdateWorkerEmailModel {
  newEmail: string;
}

// Tax / external id update payload.
// The callers pass the full `WorkerProfile` object; the backend reads only the
// field relevant to each endpoint from `WorkerProfileDetailModel`.
export interface UpdateWorkerProfileFieldsPayload {
  id: string;
  externalId?: string | null;
  wcCode?: string | null;
  workerProfileTaxCategoryId?: string | null;
  taxRate?: number | null;
}

// Holiday assigned to a worker profile.
// Mirrors backend WorkerProfileHolidayModel (Covenant.Common.Models.Worker).
export interface AgencyWorkerHoliday {
  workerProfileId?: string;
  holidayId?: string;
  date: string;
  statPaidWorker?: number;
}

export interface AddNewHolidayPayload {
  workerProfileId: string;
  date: string;
}

// Mirrors backend GetWorkerRequestHistoryFilter (SortBy = WorkerRequestHistorySortBy).
export interface WorkerRequestHistoryFilter extends GridParams {
  profileId: string;
  numberId?: number | string;
  companyFullName?: string;
  jobTitle?: string;
  location?: string;
  statuses?: RequestStatus[];
  workerRequestStatuses?: WorkerRequestStatus[];
  startWorkingFrom?: string | null;
  startWorkingTo?: string | null;
}

// Request history item returned for the Worker → Requests tab.
// Mirrors backend WorkerRequestHistoryModel (Covenant.Common.Models.Request.Agency).
export interface WorkerRequestHistoryItem {
  id: string;
  numberId: number;
  jobTitle: string;
  companyFullName: string;
  companyProfileId: string;
  location: string;
  entrance?: string;
  createdAt: string;
  requestStatus: RequestStatus;
  workerRequestStatus: WorkerRequestStatus;
  isAsap: boolean;
  startWorking: string | null;
  finishWorking: string | null;
}
