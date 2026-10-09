import type { RequestStatus } from '@/shared/constants/enums';
import type { ComplianceDocumentTarget, RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import type { CompanyUserModel } from '@/shared/company-profile/types';
import type { LocationDetailModel, PaginatedList } from '@/shared/types/common';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';
import type { AgencyCompanyLocationModel, AgencyCompanyJobPosition } from '@/modules/agency/recruiting/clients/types';
import type { RequestComplianceItem, RequestShiftModel } from '@/shared/request-detail/types';

// Filter for the paginated list of agency requests.
// Mirrors backend GetRequestForAgencyFilter (Covenant.Common.Models.Request).
export interface AgencyRequestFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  numberId?: number | string;
  companyFullName?: string;
  location?: string;
  jobTitle?: string;
  displayRecruiters?: string;
  statuses?: number[];
  onlyMine?: boolean;
  recruiter?: string;
  salesRepresentative?: string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  rateFrom?: number | string | null;
  rateTo?: number | string | null;
  companyProfileId?: string;
  agencyId?: string;
  filter?: string;
  jobBoardIds?: string[];
}

// Job board (Source) attached to a request. Mirrors RequestSourceDetailModel.
export interface RequestJobBoard {
  sourceId: string;
  value: string;
  publishedAt?: string | null;
  externalUrl?: string | null;
}

// Payload item for PUT /api/AgencyRequest/{id}/sources. Mirrors CreateRequestSourceModel.
export interface SetRequestJobBoardItem {
  sourceId: string;
  publishedAt?: string | null;
  externalUrl?: string | null;
}

// Aggregated count of requests per job board. Mirrors RequestSourceSummaryModel.
export interface RequestJobBoardSummary {
  sourceId: string;
  value: string;
  count: number;
}

// Combined response for GET /api/AgencyRequest. Mirrors AgencyRequestsPagedResponse.
// PaginatedList<AgencyRequestListItem> + JobBoardsSummary aligned with the same filter.
export interface AgencyRequestsPagedResponse {
  pageIndex: number;
  totalPages: number;
  totalItems: number;
  items: AgencyRequestListItem[];
  jobBoardsSummary: RequestJobBoardSummary[];
}

// Item returned by GET /api/AgencyRequest. Mirrors AgencyRequestListModel.
export interface AgencyRequestListItem {
  id: string;
  agencyId: string;
  numberId: number;
  jobTitle: string;
  billingTitle?: string;
  createdAt: string;
  address?: string;
  city?: string;
  provinceName?: string;
  postalCode?: string;
  entrance?: string;
  companyFullName?: string;
  companyProfileId: string;
  requestStatus: number;
  workersQuantity: number;
  workersQuantityWorking: number;
  isAsap: boolean;
  workerRate?: number | null;
  workerSalary?: number | null;
  displayRecruiters?: string;
  displayReportTo?: string;
  displayShift?: string;
  salesRepresentative?: string;
  notesCount: number;
  vaccinationRequired: boolean;
  punchCardOptionEnabled: boolean;
  hasPermissionToSeeInternalRequests: boolean;
  location: string;
  locationAddress: string;
  jobBoards: RequestJobBoard[];
}

// Detail returned by GET /api/AgencyRequest/{id}. Mirrors AgencyRequestDetailModel.
export interface AgencyRequestDetail {
  id: string;
  numberId: number;
  jobTitle: string;
  billingTitle?: string;
  jobCosting?: string;
  companyLogo?: string;
  fullName?: string;
  companyProfileId: string;
  description?: string;
  requirements?: string;
  responsibilities?: string;
  jobLocation?: LocationDetailModel | null;
  workersQuantity: number;
  workersQuantityWorking: number;
  jobPositionId?: string | null;
  jobPosition?: string;
  holidayIsPaid: boolean;
  breakIsPaid: boolean;
  status: RequestStatus;
  cancellationDetail?: string;
  createdAt: string;
  createdBy?: string;
  startAt?: string | null;
  finishAt?: string | null;
  invitationSentItAt?: string | null;
  durationBreak: string;
  incentive?: number | null;
  incentiveDescription?: string;
  agencyRate?: number | null;
  workerRate?: number | null;
  workerSalary?: number | null;
  durationTerm: number;
  employmentType: number;
  displayRecruiters?: string;
  displayShift?: string;
  isAsap: boolean;
  usesRunners: boolean;
  vaccinationRequired?: boolean | null;
  punchCardOptionEnabled: boolean;
  internalRequirements?: string;
  salesRepresentativeId?: string | null;
  companyUserIds?: string[];
  complianceItems?: RequestComplianceItem[];
  skills: AgencyRequestSkillModel[];
  requestedBy: AgencyRequestPersonItem[];
  reportTo: AgencyRequestPersonItem[];
  applicantsCount: number;
  runnersCount: number;
  workersCount: number;
}

// Compliance item with the completion state of one applicant.
// Mirrors backend ApplicantComplianceItemModel.
export interface ApplicantComplianceItem {
  id: string;
  name: string;
  isMandatory: boolean;
  documentTarget: ComplianceDocumentTarget;
  isCompleted: boolean;
  completedAt?: string | null;
  completedBy?: string;
  canUpload: boolean;
  existingFileUrl?: string | null;
}

// Body for PUT .../Applicants/{id}/Status. Mirrors backend ChangeRequestApplicantStatusModel.
export interface ChangeRequestApplicantStatusModel {
  status: RequestApplicantStatus;
}

// JSON "data" part for POST .../Applicants/{id}/ComplianceItems/{itemId} (multipart).
// Mirrors backend CompleteApplicantComplianceItemModel.
export interface CompleteApplicantComplianceItemModel {
  fileName?: string;
  identificationNumber?: string;
  identificationTypeId?: string | null;
  socialInsuranceNumber?: string;
}

// Payload for PUT /api/AgencyRequest/{id}/Cancel. Mirrors RequestCancellationDetailModel.
export interface CancelRequestPayload {
  cancellationReasonId: string;
  otherCancellationReason?: string;
}

// Payload for PUT /api/AgencyRequest/bulk-cancel. Mirrors BulkRequestCancellation.
export interface BulkUpdateRecruitersPayload {
  ids: string[];
  recruiterIds: string[];
}

export interface BulkCancelRequestsPayload {
  ids: string[];
  cancellationReasonId: string;
  otherCancellationReason?: string;
}

export interface BulkCancelRequestsResult {
  cancelled: number;
  skipped: number;
}

// Filter for GET /api/AgencyRequest/{requestId}/Worker.
// Mirrors backend GetWorkersRequestFilter.
export interface AgencyRequestWorkerFilter {
  requestId: string;
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  numberId?: number | string;
  name?: string;
  phone?: string;
  socialInsurance?: string;
  externalId?: string;
  createdBy?: string;
  rejectedBy?: string;
  statuses?: number[];
  startWorkingFrom?: string | null;
  startWorkingTo?: string | null;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  rejectedAtFrom?: string | null;
  rejectedAtTo?: string | null;
}

// Worker assigned to a request. Mirrors backend AgencyWorkerRequestModel.
export interface AgencyRequestWorker {
  id: string;
  numberId: number;
  requestId: string;
  workerId: string;
  workerProfileId: string;
  name: string;
  workerRequestStatus: number;
  rejectComments?: string;
  rejectedBy?: string;
  rejectedAt?: string | null;
  profileImage?: string;
  address?: string;
  approvedToWork: boolean;
  isSubcontractor: boolean;
  socialInsurance?: string;
  dueDate?: string | null;
  socialInsuranceExpire: boolean;
  mobileNumber?: string;
  notesCount: number;
  startWorking?: string | null;
  createdBy?: string;
  createdAt: string;
  totalHoursApproved: number;
  totalHoursWorker: number;
  externalId?: string;
}

// Body for POST /api/AgencyRequest/{id}/Worker/{workerId}/Book and
// PUT /api/AgencyRequest/{id}/Worker/{id}. Mirrors AgencyBookWorkerModel.
export interface BookWorkerModel {
  startWorking: string;
}

// Body for PUT /api/AgencyRequest/{id}/Worker/{id}/Reject. Mirrors CommentsModel.
export interface RejectWorkerModel {
  comments?: string;
}

// Filter for GET /api/AgencyRequest/{requestId}/Applicant.
// Mirrors backend GetRequestApplicantFilter (+ requestId used client-side to route).
export interface AgencyRequestApplicantFilter {
  requestId: string;
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  name?: string;
  phone?: string;
  createdBy?: string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  statuses?: RequestApplicantStatus[];
}

// Body for POST /api/AgencyRequest/{requestId}/Applicant. Mirrors RequestApplicantModel.
export interface CreateRequestApplicantModel {
  workerProfileId?: string | null;
  candidateId?: string | null;
  comments?: string;
}

// Result of GET /api/AgencyRequest/{id}/Applicant/Search. Mirrors backend ApplicantSearchResultModel.
export interface ApplicantSearchResult {
  workerProfileId?: string | null;
  candidateId?: string | null;
  numberId: number;
  name: string;
  email?: string;
  type: string;
  approvedToWork: boolean;
}

// Item returned for an applicant. Mirrors RequestApplicantDetailModel.
export interface AgencyRequestApplicant {
  id: string;
  workerProfileId?: string | null;
  candidateId?: string | null;
  workerId?: string | null;
  name?: string;
  phoneNumber?: string;
  email?: string;
  comments?: string;
  createdAt: string;
  createdBy?: string;
  status: RequestApplicantStatus;
  complianceTotal: number;
  complianceCompleted: number;
  mandatoryPending: number;
}

export interface AgencyRequestSkillModel {
  id?: string | null;
  skill: string;
}

// Contact person attached to a request (RequestedBy / ReportTo endpoints).
// Mirrors backend RequestContactPersonModel.
export interface AgencyRequestPersonItem {
  id: string;
  title?: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
}

// Everything the agency request form needs, in a single call.
// Mirrors backend RequestLookupModel. `request` is only filled when a requestId
// is sent (edit / duplicate).
export interface AgencyRequestLookup {
  request: AgencyRequestDetail | null;
  jobPositions: AgencyCompanyJobPosition[];
  locations: AgencyCompanyLocationModel[];
  personnel: AgencyPersonnelListItem[];
  companyUsers: CompanyUserModel[];
}

export interface AgencyRequestFormState {
  id?: string;
  numberId?: number;
  companyProfileId: string;
  jobCosting?: string;
  responsibilities?: string;
  internalRequirements?: string;
  durationBreak: Date;
  breakIsPaid: boolean;
  durationTerm: number;
  employmentType: number;
  isAsap: boolean;
  usesRunners: boolean;
  punchCardOptionEnabled: boolean;
  finishAt?: Date | null;
  rate?: number | null;
  jobPositionRateId?: string | null;
  locationId?: string | null;
  shift?: RequestShiftModel | null;
  salesRepresentativeId?: string | null;
  companyUserIds?: string[];
  complianceItems?: RequestComplianceItem[];
}

export interface TableRequestsConfig {
  showAssignedToMe: boolean;
  showQuickActions: boolean;
  enableCheckable: boolean;
  showSalesRepColumn: boolean;
  showNotesColumn: boolean;
}
