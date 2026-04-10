import { Location, City, Province } from './common';

export enum AgencyType {
  Master = 1,
  Regular = 2,
  BusinessPartner = 3,
}

export interface AgencyProfile {
  id: string;
  fullName: string;
  businessNumber: string;
  hstNumber: string;
  agencyType: AgencyType;
  email: string;
  phoneNumber: string;
  website: string;
  isActive: boolean;
  createdAt: string;
  locations: AgencyLocation[];
  agencies: AgencyProfile[];
  usaAgency: boolean;
  masterAgency: boolean;
}

export interface AgencyLocation {
  id: string;
  agencyId: string;
  name: string;
  locationId: string;
  location?: Location;
  isBillingAddress: boolean;
  isActive: boolean;
}

export enum PersonnelType {
  Recruiter = 'Recruiter',
  SalesRepresentative = 'SalesRepresentative',
  AccountManager = 'AccountManager',
  Administrator = 'Administrator',
}

export interface AgencyPersonnel {
  id: string;
  agencyId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  type: PersonnelType;
  isActive: boolean;
}

// Filter used by Agencies list page
export interface AgencyListFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
  orderBy?: string;
  isDescending?: boolean;
  agencyTypes?: number[];
}

// Item returned by GET /api/Agency list
export interface AgencyListItem {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  agencyType: AgencyType;
  createdAt: string;
}

// Detail returned by GET /api/Agency/{id} and used by edit pages.
// Modeled to be permissive: many fields come from the API and are
// only used by specific tabs/components.
export interface AgencyDetail {
  id: string;
  fullName: string;
  hstNumber?: string;
  businessNumber?: string;
  phonePrincipal?: string;
  phonePrincipalExt?: string;
  webPage?: string;
  fax?: string;
  faxExt?: string;
  wsibGroup?: { value: string }[];
  contactInformation?: AgencyContactInformation[];
  locations?: AgencyLocationDetail[];
  agencies?: AgencyDetail[];
  agencyType?: AgencyType;
  logo?: { fileName?: string; pathFile?: string } | null;
  usaAgency?: boolean;
  masterAgency?: boolean;
}

export interface AgencyContactInformation {
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  position?: string;
}

// Used by ProfileBilling.vue and the /api/Agency/Location endpoint.
// Fields are flat (not wrapped in a Location object).
export interface AgencyLocationDetail {
  id?: string;
  address: string;
  city: City;
  province?: Province;
  postalCode: string;
  formattedAddress?: string;
  isBilling?: boolean;
}

// Body used by AgencyCreatePersonnelModal.vue (POST /api/AgencyPersonnel)
export interface AgencyPersonnelCreateModel {
  name: string | null;
  email: string | null;
}

// Item returned by GET /api/AgencyPersonnel
export interface AgencyPersonnelListItem {
  id: string;
  name: string;
  email: string;
}

// Item returned by GET /api/PersonnelAgency
export interface PersonnelAgencyItem {
  id: string;
  fullName: string;
  isPrimary: boolean;
}

// Used by CreateAgency.vue (POST /api/Agency)
export interface CreateAgencyModel {
  fullName: string;
  email: string;
  phonePrincipal?: string;
  agencyType?: AgencyType;
  [key: string]: unknown;
}

// ---------------------------------------------------------------------------
// Agency-managed worker (agency back-office actions on workers)
// ---------------------------------------------------------------------------

// Filter for paginated worker list (agency view)
export interface AgencyWorkerFilter {
  pageIndex?: number;
  pageSize?: number;
  searchTerm?: string;
  features?: string[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  isDescending?: boolean;
  sortBy?: number;
  [key: string]: unknown;
}

// Item returned by GET /api/AgencyWorkerProfile (paginated list)
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
  fullName: string;
  approvedToWork: boolean;
}

// Worker comment payload (POST /api/AgencyWorker/{id}/Comment)
export interface AgencyWorkerCommentModel {
  comment: string;
  rate: number;
}

// Worker email update payload
export interface UpdateWorkerEmailModel {
  newEmail: string;
}

// Tax/external id payloads — they reuse the worker entity itself
export interface UpdateWorkerProfileFieldsPayload {
  id: string;
  externalId?: string;
  workerProfileTaxCategoryId?: string;
  taxRate?: number;
  [key: string]: unknown;
}

// Holidays
export interface AgencyWorkerHoliday {
  id?: string;
  date: string;
  name?: string;
  hours?: number;
  [key: string]: unknown;
}

export interface AddNewHolidayPayload {
  workerProfileId: string;
  date: string;
}

// Request history (item returned for the Worker → Requests tab)
export interface AgencyWorkerRequestHistoryItem {
  id: string;
  jobPosition?: string;
  startDate?: string;
  endDate?: string;
  status?: string;
  [key: string]: unknown;
}

// ---------------------------------------------------------------------------
// Agency-managed company (agency back-office actions on companies)
// ---------------------------------------------------------------------------

export interface AgencyCompanyFilter {
  pageIndex?: number;
  pageSize?: number;
  searchTerm?: string;
  companyStatus?: string;
  industryId?: string;
  isDescending?: boolean;
  sortBy?: number;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  [key: string]: unknown;
}

export interface AgencyCompanyListItem {
  id: string;
  fullName: string;
  email: string;
  numberId?: number;
  companyStatus?: string;
  active?: boolean;
  createdAt?: string;
}

export interface AgencyCompanyContactPerson {
  id?: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  email: string;
  position?: string;
  mobileNumber?: string;
  officeNumber?: string;
  officeNumberExt?: number | null;
  isPrimaryContact?: boolean;
  [key: string]: unknown;
}

export interface AgencyCompanyLocationModel {
  id?: string;
  address?: string;
  city?: { id: string; value: string };
  province?: { id: string; value: string; code: string };
  postalCode?: string;
  isBilling?: boolean;
  formattedAddress?: string;
  [key: string]: unknown;
}

export interface AgencyCompanyJobPosition {
  id?: string;
  companyProfileId?: string;
  jobTitle?: string;
  workerRate?: number;
  agencyRate?: number;
  nightShiftRate?: number | null;
  holidayRate?: number | null;
  overtimeRate?: number | null;
  currency?: string;
  isActive?: boolean;
  [key: string]: unknown;
}

export interface VaccinationRequiredModel {
  vaccinationRequired: boolean;
  vaccinationRequiredComments?: string;
}

export interface InvoiceNotesModel {
  notes?: string;
  [key: string]: unknown;
}

export interface InvoiceRecipientModel {
  id?: string;
  email: string;
  name?: string;
  [key: string]: unknown;
}

export interface CompanyProfileUserPayload {
  companyId: string;
  user?: Record<string, unknown>;
  userId?: string;
}

export interface CompanySettingsPayload {
  companyId: string;
  settings: Record<string, unknown>;
}

export interface BulkUploadPayload {
  agencyId: string;
  file: File;
}

export interface CompanyProvinceWithTaxes {
  id: string;
  province: string;
  taxes?: { name: string; rate: number }[];
  [key: string]: unknown;
}

// ---------------------------------------------------------------------------
// Agency requests (job orders)
// ---------------------------------------------------------------------------

export interface AgencyRequestFilter {
  pageIndex?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string | number;
  isDescending?: boolean;
  sortBy?: number;
  startAt?: string | null;
  endAt?: string | null;
  companyId?: string;
  jobPositionId?: string;
  [key: string]: unknown;
}

export interface AgencyRequestListItem {
  id: string;
  numberId?: number;
  jobTitle?: string;
  status?: string;
  isAsap?: boolean;
  workersQuantity?: number;
  workersQuantityWorking?: number;
  startAt?: string;
  endAt?: string;
  [key: string]: unknown;
}

export interface AgencyRequestDetail {
  id: string;
  numberId?: number;
  jobTitle?: string;
  workersQuantity?: number;
  status?: string;
  isAsap?: boolean;
  punchCardVisibilityStatusInApp?: boolean;
  durationBreak?: string;
  [key: string]: unknown;
}

export interface CancelRequestPayload {
  cancellationReasonId: string;
  otherCancellationReason?: string;
}

export interface AgencyRequestWorkerFilter {
  requestId: string;
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  searchTerm?: string;
  [key: string]: unknown;
}

export interface AgencyRequestWorker {
  id: string;
  workerId: string;
  fullName?: string;
  status?: string;
  startWorking?: string;
  [key: string]: unknown;
}

export interface BookWorkerModel {
  startDate: string;
  [key: string]: unknown;
}

export interface RejectWorkerModel {
  comments?: string;
  [key: string]: unknown;
}

export interface AgencyRequestApplicantFilter {
  requestId: string;
  pageIndex?: number;
  pageSize?: number;
  [key: string]: unknown;
}

export interface AgencyRequestApplicant {
  id: string;
  workerId?: string;
  fullName?: string;
  [key: string]: unknown;
}

export interface AgencyRequestRecruiterModel {
  recruiterId: string;
}

export interface AgencyRequestSkillModel {
  skill: string;
}

export interface AgencyRequestPersonItem {
  id: string;
  email?: string;
  name?: string;
  [key: string]: unknown;
}

// POST /api/AgencyCompanyProfile/{profileId}/JobPosition/Petition
export interface PetitionJobPositionPayload {
  id: string | null;
  jobPosition: string | null;
  message: string;
}

// PUT /api/AgencyRequest/is-asap
export interface UpdateIsAsapRequestsPayload {
  ids: string[];
  isAsap: boolean;
}

// ---------------------------------------------------------------------------
// Agency notes (worker / candidate / company / request / request-worker)
// ---------------------------------------------------------------------------

export interface NoteModel {
  id?: string;
  note: string;
  color?: string;
}

export interface NoteItem {
  id: string;
  note: string;
  color?: string;
  createdAt?: string;
  createdBy?: string;
  [key: string]: unknown;
}

export interface NotePagination {
  page: number;
  size: number;
}

export interface CreateNoteResponse {
  id: string;
  createdAt?: string;
  createdBy?: string;
  [key: string]: unknown;
}

// ---------------------------------------------------------------------------
// Agency reports
// ---------------------------------------------------------------------------

export interface AgencyReportFilter {
  weekEnding?: string;
  startDate?: string;
  endDate?: string;
  companyId?: string;
  [key: string]: unknown;
}
