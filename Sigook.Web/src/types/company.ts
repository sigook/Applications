import type { CovenantFileModel, LocationDetailModel } from './common';
import { RequestStatus } from '@/constants/enums';

export enum CompanyStatus {
  Lead = 'Lead',
  Potential = 'Potential',
  Prospect = 'Prospect',
  Quoted = 'Quoted',
  Client = 'Client',
  Blocked = 'Blocked',
  Inactive = 'Inactive',
}

export interface CompanyProfile {
  id: string;
  agencyId: string;
  dbaName: string;
  businessNumber: string;
  hstNumber: string;
  companyStatus: CompanyStatus;
  email: string;
  phoneNumber: string;
  website: string;
  requiresPermissionToSeeRequests: boolean;
  createdAt: string;
  fullName: string;
  logo: string | null;
  locations: CompanyProfileLocation[];
  jobPositionRates: CompanyProfileJobPositionRate[];
  contactPersons: CompanyProfileContactPerson[];
}

export interface CompanyProfileLocation {
  id: string;
  companyProfileId: string;
  name: string;
  locationId: string;
  location?: LocationDetailModel;
  isActive: boolean;
}

export interface CompanyProfileJobPositionRate {
  id: string;
  companyProfileId: string;
  jobTitle: string;
  workerRate: number;
  agencyRate: number;
  nightShiftRate: number | null;
  holidayRate: number | null;
  overtimeRate: number | null;
  currency: string;
  isActive: boolean;
}

export interface CompanyProfileContactPerson {
  id: string;
  companyProfileId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  position: string;
  isPrimaryContact: boolean;
}

export interface CompanyUser {
  id: string;
  companyProfileId: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  canSeeRequests: boolean;
  isActive: boolean;
}

export interface CompanyFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
  companyStatus?: CompanyStatus;
  sortBy?: string;
  sortDesc?: boolean;
}

// Company Profile Detail (matches CompanyProfileDetailModel)
export interface CompanyProfileDetail {
  id: string;
  numberId: number;
  companyId: string;
  fullName: string;
  phone: string;
  phoneExt: number | null;
  fax: string;
  faxExt: number | null;
  email: string;
  website: string;
  about: string;
  internalInfo: string;
  companyStatus: CompanyStatus;
  active: boolean;
  paidHolidays: boolean;
  requiredPaymentMethod: boolean;
  createdAt: string;
  vaccinationRequired: boolean | null;
  vaccinationRequiredComments: string;
  requiresPermissionToSeeRequests: boolean;
  logo: CovenantFileModel;
  industry: CompanyProfileIndustryDetail;
  salesRepresentativeId: string | null;
  overtimeStartsAfter: number;
}

// Matches CompanyProfileDocumentModel (extends CovenantFileModel)
// Used by POST /api/AgencyCompanyProfile/{profileId}/Document and list responses
export interface CompanyProfileDocumentModel {
  id?: string;
  fileName: string;
  description?: string;
  pathFile?: string;
  canDownload?: boolean;
  documentType?: string | number;
}

// Matches CompanyProfileListModel — returned by GetCompaniesWithRequests
export interface CompanyProfileListItem {
  id: string;
  logo: string;
  fullName: string;
  numberId: number;
  locations: string[];
  active: boolean;
  companyId: string;
  agencyId: string;
  industry: string;
  companyStatus: CompanyStatus;
  contactName: string;
  contactRole: string;
  phone: string;
  email: string;
  website: string;
  createdBy: string;
  createdAt: string;
  updatedBy: string;
}

// PATCH /api/V2/AgencyCompanyProfile/{id}/{PaidHolidays|Overtime|RequiresPermissionToSeeRequests}
// Matches CompanyProfileSettingsUpdateModel — .NET binds only the fields it needs
export interface CompanyProfileSettingsUpdate {
  requiresPermissionToSeeRequests?: boolean;
  overtimeStartsAfter?: number;
  paidHolidays?: boolean;
}

export interface CompanyProfileIndustryDetail {
  id: string;
  industry: string;
}

// Company Profile Location (matches CompanyProfileLocationDetailModel)
export interface CompanyProfileLocationDetail {
  id?: string;
  address: string;
  postalCode: string;
  isBilling: boolean;
  latitude: number | null;
  longitude: number | null;
  entrance: string;
  mainIntersection: string;
}

// Company Request models
export interface CompanyRequestFilter {
  sortBy: number;
  isDescending: boolean;
  pageIndex: number;
  pageSize: number;
  numberId?: number;
  jobTitle?: string;
  location?: string;
  companyUserId?: string;
}

export interface CompanyRequestListItem {
  id: string;
  numberId: number;
  jobTitle: string;
  location: string;
  entrance: string;
  displayShift: string;
  workersQuantity: number;
  workersQuantityWorking: number;
  requestStatus: RequestStatus;
  isAsap: boolean;
  isDirectHiring: boolean;
  createdAt: string;
}

// Company Request Worker models
export interface CompanyRequestWorkerFilter {
  sortBy: number;
  requestId: string;
  pageIndex: number;
  pageSize: number;
  isDescending?: boolean;
  numberId?: number;
  name?: string;
  statuses?: number[];
  startWorkingFrom?: Date;
  startWorkingTo?: Date;
}

export interface CompanyRequestWorker {
  numberId: number;
  requestId: string;
  id: string;
  workerId: string;
  name: string;
  workerRequestStatus: number;
  status?: string;
  profileImage: string;
  isSubcontractor: boolean;
  totalHoursApproved: number;
  totalHoursWorker: number;
  startWorking: string | null;
}

// TimeSheet models
export interface TimeSheetListItem {
  id: string;
  day: string;
  clockIn: string | null;
  clockOut: string | null;
  timeIn: string;
  timeOut: string | null;
  totalHours: number;
  totalHoursApproved: number;
  canUpdate: boolean;
  wasApproved: boolean;
  missingHours: string;
  missingHoursOvertime: string;
  missingRateWorker: number;
  missingRateAgency: number;
  deductionsOthers: number;
  bonusOrOthers: number;
  deductionsOthersDescription: string;
  bonusOrOthersDescription: string;
  reimbursements: number;
  reimbursementsDescription: string;
  comment: string;
}

export interface TimeSheetModel {
  hours: string;
  timeIn: string;
  missingHours: string | number;
  missingHoursOvertime: string | number;
  deductionsOthers?: number;
  bonusOrOthers?: number;
  deductionsOthersDescription?: string;
  bonusOrOthersDescription?: string;
  comments?: string;
  missingRateWorker?: number;
  missingRateAgency?: number;
  reimbursements?: number;
  reimbursementsDescription?: string;
}

// Response shape of GET /api/v2/AgencyRequest/{}/Worker/{}/TimeSheet/{}/Usages.
// Mirrors backend TimeSheetUsagesModel — single object, not an array.
export interface TimeSheetUsagesModel {
  invoiceNumber?: number | null;
  payStubNumber?: string | null;
}

export interface ClockInModel {
  clockIn: string;
}

export interface ClockInResult {
  timeSheetId: string;
  workerFullName: string;
  finish: boolean;
}

// Company User (matches CompanyUserModel)
export interface CompanyUserModel {
  id: string;
  companyId: string | null;
  createdAt: string;
  email: string;
  name: string;
  lastname: string;
  position: string;
  mobileNumber: string;
}

export interface CreateCompanyUserModel {
  name: string;
  lastname: string;
  mobileNumber: string | null;
  position: string | null;
  email: string;
}

// Company Contact Person (matches CompanyProfileContactPersonModel)
export interface CompanyContactPersonModel {
  id?: string;
  companyProfileId?: string;
  title: string;
  firstName: string;
  middleName: string;
  lastName: string;
  position: string;
  mobileNumber: string;
  officeNumber: string;
  officeNumberExt: number | null;
  email: string;
}

// Company Invoice models
export interface CompanyInvoiceFilter {
  sortBy: number;
  isDescending: boolean;
  pageIndex: number;
  pageSize: number;
}

export interface CompanyInvoiceListItem {
  id: string;
  numberId: number;
  invoiceNumber: string;
  createdAt: string;
  weekEnding: string | null;
  totalNet: number;
}

// Reject / Request new worker
export interface CommentsModel {
  comments: string;
}

// ===========================================================================
// Deals & Company Interactions (sales)
// Mirror backend Covenant.Common.Models.Company. The API (System.Text.Json,
// no JsonStringEnumConverter) serializes enums as their NUMERIC value, so the
// enums below must match the backend int values exactly.
// ===========================================================================

// --- Deals -----------------------------------------------------------------

export enum DealType {
  Temporal = 0,
  Permanent = 1,
  TempToPerm = 2,
}

export enum DealStatus {
  ToSend = 0,
  Sent = 1,
  Rejected = 2,
  Accepted = 3,
}

// GetDealsSortBy on the backend: Date=0, Company=1, Value=2, Status=3.
export enum DealSortBy {
  Date = 0,
  Company = 1,
  Value = 2,
  Status = 3,
}

export const DEAL_TYPE_LABELS: Record<DealType, string> = {
  [DealType.Temporal]: 'Temporal',
  [DealType.Permanent]: 'Permanent',
  [DealType.TempToPerm]: 'Temp to Perm',
};

export const DEAL_TYPES: DealType[] = [DealType.Temporal, DealType.Permanent, DealType.TempToPerm];

export const DEAL_STATUS_LABELS: Record<DealStatus, string> = {
  [DealStatus.ToSend]: 'To Send',
  [DealStatus.Sent]: 'Sent',
  [DealStatus.Rejected]: 'Rejected',
  [DealStatus.Accepted]: 'Accepted',
};

export const DEAL_STATUSES: DealStatus[] = [
  DealStatus.ToSend,
  DealStatus.Sent,
  DealStatus.Rejected,
  DealStatus.Accepted,
];

export const DEAL_STATUS_COLORS: Record<DealStatus, string> = {
  [DealStatus.ToSend]: '#9ad6ff',
  [DealStatus.Sent]: '#21b7ff',
  [DealStatus.Rejected]: '#ff5c5c',
  [DealStatus.Accepted]: '#3eb800',
};

// Mirrors backend DealListModel.
export interface Deal {
  id: string;
  title: string;
  companyProfileId: string;
  companyName: string;
  ownerId: string;
  ownerName?: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
  documentName?: string | null;
}

// Filter for GET .../deals. Mirrors backend GetDealsFilter.
// OwnerId is forced server-side to the current sales user, so it is not exposed here.
export interface DealFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: DealSortBy;
  companyProfileId?: string | null;
  type?: DealType | null;
  statuses?: DealStatus[];
  dateFrom?: string | null;
  dateTo?: string | null;
}

// Body for POST. Mirrors backend CreateDealModel.
export interface CreateDealModel {
  title: string;
  companyProfileId: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
}

// Body for PUT .../{id}. Mirrors backend UpdateDealModel.
export interface UpdateDealModel {
  title: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
}

// --- Company interactions --------------------------------------------------

export enum InteractionType {
  Call = 0,
  Mail = 1,
  Sms = 2,
  LinkedIn = 3,
}

export enum InteractionPurpose {
  Intro = 0,
  FollowUp = 1,
  Proposal = 2,
  Negotiation = 3,
  Closing = 4,
}

export enum InteractionStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

// GetCompanyInteractionsSortBy on the backend: CreatedAt=0, Company=1, Status=2.
export enum CompanyInteractionSortBy {
  CreatedAt = 0,
  Company = 1,
  Status = 2,
}

export const INTERACTION_TYPE_LABELS: Record<InteractionType, string> = {
  [InteractionType.Call]: 'Call',
  [InteractionType.Mail]: 'Email',
  [InteractionType.Sms]: 'SMS',
  [InteractionType.LinkedIn]: 'LinkedIn',
};

export const INTERACTION_TYPES: InteractionType[] = [
  InteractionType.Call,
  InteractionType.Mail,
  InteractionType.Sms,
  InteractionType.LinkedIn,
];

export const INTERACTION_TYPE_ICONS: Record<InteractionType, string> = {
  [InteractionType.Call]: 'phone',
  [InteractionType.Mail]: 'email-outline',
  [InteractionType.Sms]: 'message-text-outline',
  [InteractionType.LinkedIn]: 'linkedin',
};

export const INTERACTION_TYPE_COLORS: Record<InteractionType, string> = {
  [InteractionType.Call]: '#21b7ff',
  [InteractionType.Mail]: '#3eb800',
  [InteractionType.Sms]: '#ff9932',
  [InteractionType.LinkedIn]: '#0a66c2',
};

export const INTERACTION_PURPOSE_LABELS: Record<InteractionPurpose, string> = {
  [InteractionPurpose.Intro]: 'Intro',
  [InteractionPurpose.FollowUp]: 'Follow-up',
  [InteractionPurpose.Proposal]: 'Proposal',
  [InteractionPurpose.Negotiation]: 'Negotiation',
  [InteractionPurpose.Closing]: 'Closing',
};

export const INTERACTION_PURPOSES: InteractionPurpose[] = [
  InteractionPurpose.Intro,
  InteractionPurpose.FollowUp,
  InteractionPurpose.Proposal,
  InteractionPurpose.Negotiation,
  InteractionPurpose.Closing,
];

export const INTERACTION_STATUS_LABELS: Record<InteractionStatus, string> = {
  [InteractionStatus.NotStarted]: 'Not started',
  [InteractionStatus.InProgress]: 'In progress',
  [InteractionStatus.Completed]: 'Completed',
};

export const INTERACTION_STATUSES: InteractionStatus[] = [
  InteractionStatus.NotStarted,
  InteractionStatus.InProgress,
  InteractionStatus.Completed,
];

// Mirrors backend CompanyInteractionListModel.
export interface CompanyInteraction {
  id: string;
  companyProfileId: string;
  companyName: string;
  ownerId: string;
  ownerName?: string;
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
  createdAt: string;
  updatedAt: string;
}

// Filter for GET .../companyinteractions. Mirrors backend GetCompanyInteractionsFilter.
// OwnerId is forced server-side to the current sales user, so it is not exposed here.
export interface CompanyInteractionFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: CompanyInteractionSortBy;
  companyProfileId?: string | null;
  interactionPurpose?: InteractionPurpose | null;
  interactionType?: InteractionType | null;
  statuses?: InteractionStatus[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
}

// Body for POST. Mirrors backend CreateCompanyInteractionModel.
export interface CreateCompanyInteractionModel {
  companyProfileId: string;
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
}

// Body for PUT .../{id}. Mirrors backend UpdateCompanyInteractionModel.
export interface UpdateCompanyInteractionModel {
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
}
