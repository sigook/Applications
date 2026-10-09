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
export interface WageHistoryFilter {
  profileId: string;
  page?: number;
  pageSize?: number;
}

export interface TimeSheetHistoryFilter {
  profileId: string;
  page?: number;
  pageSize?: number;
}

// Mirrors backend PayStubItemHistoryModel.
export interface WorkerWageHistoryLine {
  description: string;
  quantity: number;
  total: number;
}

// Item returned by GET /api/agency/recruiting/workers/{id}/WageHistory. Mirrors backend PayStubHistoryModel.
export interface WorkerWageHistoryItem {
  rowNumber: number;
  id: string;
  payStubNumber: string;
  weekEnding: string;
  totalEarnings: number;
  vacations: number;
  totalPaid: number;
  start: string;
  end: string;
  items: WorkerWageHistoryLine[];
  companies: string[];
}

// Returned by GET /api/agency/recruiting/workers/{id}/WageHistory/{rowNumber}. Mirrors backend PayStubHistoryAccumulated.
export interface WorkerWageHistoryAccumulated {
  totalEarnings: number;
  vacations: number;
  totalPaid: number;
  quantity: number;
  total: number;
}

// Item returned by GET /api/agency/recruiting/workers/{id}/TimeSheetHistory. Mirrors backend TimeSheetHistoryModel.
export interface WorkerTimeSheetHistoryItem {
  rowNumber: number;
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
}

// Returned by GET /api/agency/recruiting/workers/{id}/TimeSheetHistory/{rowNumber}. Mirrors backend TimesheetHistoryAccumulated.
export interface WorkerTimeSheetHistoryAccumulated {
  regularHours: number;
  holidayHours: number;
  overtimeHours: number;
  missingHours: number;
  missingHoursOvertime: number;
  totalHours: number;
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

// Request history item returned for the Worker → Requests tab.
// Mirrors backend RequestListModel (Covenant.Common.Models.Request).
export interface AgencyWorkerRequestHistoryItem {
  id: string;
  numberId: number;
  jobTitle: string;
  companyFullName: string;
  agencyFullName?: string;
  logo?: string;
  location: string;
  entrance?: string;
  createdAt: string;
  finishAt?: string | null;
  startWorking: string;
  finishWorking: string;
  workersQuantity: number;
  workersQuantityWorking: number;
  displayShift?: string;
  isAsap: boolean;
  isDirectHiring: boolean;
  workerApprovedToWork?: string;
  status: string;
  companyProfileId: string;
}
