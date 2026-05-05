import type {
  CatalogItem,
  City,
  CovenantFileModel,
  LocationDetailModel,
  Province,
  WsibGroup,
} from './common';

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
  location?: LocationDetailModel;
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

// Mirrors backend AgencyContactInformationModel (Covenant.Common.Models.Agency).
export interface AgencyContactInformation {
  id?: string;
  title?: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  position?: string;
  mobileNumber?: string;
  officeNumber?: string;
  officeNumberExt?: number | null;
  email?: string;
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

// Body for POST /api/Agency. Mirrors backend AgencyModel
// (Covenant.Common.Models.Agency.AgencyModel).
export interface CreateAgencyModel {
  id?: string;
  fullName: string;
  hstNumber?: string;
  businessNumber?: string;
  webPage?: string;
  phonePrincipal?: string;
  phonePrincipalExt?: number | null;
  logo?: CovenantFileModel | null;
  email: string;
  agencyType?: AgencyType;
  wsibGroup?: WsibGroup[];
  locations?: LocationDetailModel[];
  contactInformation?: AgencyContactInformation[];
}

// ---------------------------------------------------------------------------
// Agency-managed worker (agency back-office actions on workers)
// ---------------------------------------------------------------------------

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

// Tax / external id update payload.
// The callers pass the full `WorkerProfile` object; the backend reads only the
// field relevant to each endpoint from `WorkerProfileDetailModel`.
export interface UpdateWorkerProfileFieldsPayload {
  id: string;
  externalId?: string | null;
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
  agencyLogo?: string;
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
  companyId: string;
}

// ---------------------------------------------------------------------------
// Agency-managed company (agency back-office actions on companies)
// ---------------------------------------------------------------------------

// Filter for GET /api/v2/AgencyCompanyProfile. Mirrors backend GetCompanyForAgencyFilter.
export interface AgencyCompanyFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  businessInfo?: string;
  contactInfo?: string;
  industry?: string;
  createdBy?: string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  updatedBy?: string;
  updatedAtFrom?: string | null;
  updatedAtTo?: string | null;
  companyStatuses?: string[];
  salesRepresentative?: string;
}

// Item returned by GET /api/v2/AgencyCompanyProfile.
export interface AgencyCompanyListItem {
  id: string;
  fullName: string;
  email: string;
  numberId?: number;
  companyStatus?: string;
  active?: boolean;
  createdAt?: string;
}

// Contact person attached to a company.
// Mirrors backend CompanyProfileContactPersonModel.
export interface AgencyCompanyContactPerson {
  id?: string;
  companyProfileId?: string;
  title?: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  position?: string;
  mobileNumber?: string;
  officeNumber?: string;
  officeNumberExt?: number | null;
  email: string;
}

// Company profile location.
// Mirrors backend CompanyProfileLocationDetailModel (extends LocationDetailModel).
export interface AgencyCompanyLocationModel extends LocationDetailModel {
  province?: Province;
}

// Company job position rate.
// Mirrors backend CompanyProfileJobPositionRateModel.
export interface AgencyCompanyJobPosition {
  id?: string;
  companyProfileId?: string;
  jobPosition?: CatalogItem | null;
  rate?: number;
  asapRate?: number | null;
  otherJobPosition?: string;
  workerRate?: number;
  workerRateMin?: number | null;
  workerRateMax?: number | null;
  description?: string;
  shift?: RequestShiftModel | null;
  createdAt?: string | null;
  createdBy?: string;
  value?: string;
  displayShift?: string;
}

export interface VaccinationRequiredModel {
  vaccinationRequired: boolean;
  vaccinationRequiredComments?: string;
}

// Body for PUT /api/CompanyProfile/{id}/InvoiceNotes.
// Mirrors backend CompanyProfileInvoiceNotesModel.
export interface InvoiceNotesModel {
  htmlNotes?: string;
}

// Recipient for company invoices.
// Mirrors backend CompanyProfileInvoiceRecipientModel.
export interface InvoiceRecipientModel {
  id?: string;
  name?: string;
  email: string;
}

export interface BulkUploadPayload {
  agencyId: string;
  file: File;
}

// Response item of GET /api/v2/AgencyCompanyProfile/{id}/company-provinces-taxes.
// Mirrors backend ProvinceModel (with settings/country populated).
export interface CompanyProvinceWithTaxes extends Province {
  taxes?: { name: string; rate: number }[];
}

// ---------------------------------------------------------------------------
// Agency requests (job orders)
// ---------------------------------------------------------------------------

// Filter for the paginated list of agency requests (job orders).
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
  companyId?: string;
  agencyId?: string;
  filter?: string;
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
  displayShift?: string;
  salesRepresentative?: string;
  notesCount: number;
  vaccinationRequired: boolean;
  punchCardOptionEnabled: boolean;
  hasPermissionToSeeInternalOrders: boolean;
  location: string;
  locationAddress: string;
}

// Detail returned by GET /api/AgencyRequest/{id}. Mirrors AgencyRequestDetailModel.
export interface AgencyRequestDetail {
  id: string;
  numberId: number;
  jobTitle: string;
  billingTitle?: string;
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
  status: string;
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
  vaccinationRequired?: boolean | null;
  punchCardOptionEnabled: boolean;
  internalRequirements?: string;
  salesRepresentativeId?: string | null;
  companyUserIds?: string[];
}

// Payload for POST/PUT /api/AgencyRequest.
// Mirrors backend RequestCreateModel.
export interface CreateAgencyRequestModel {
  jobTitle: string;
  billingTitle?: string;
  workersQuantity: number;
  description?: string;
  durationBreak: string;
  breakIsPaid: boolean;
  incentive?: number | null;
  incentiveDescription?: string;
  requirements?: string;
  internalRequirements?: string;
  responsibilities?: string;
  isAsap: boolean;
  jobIsOnBranchOffice: boolean;
  anotherLocation?: LocationDetailModel | null;
  locationId?: string | null;
  jobPositionRateId?: string | null;
  durationTerm: number;
  employmentType: number;
  companyProfileId: string;
  agencyId: string;
  startAt: string;
  finishAt?: string | null;
  shift?: RequestShiftModel | null;
  punchCardOptionEnabled: boolean;
  workerSalary?: number | null;
  salesRepresentativeId?: string | null;
  companyUserIds?: string[];
}

// Payload for PUT /api/AgencyRequest/{id}/Shift. Mirrors backend ShiftModel.
export interface RequestShiftModel {
  sunday?: boolean | null;
  monday?: boolean | null;
  tuesday?: boolean | null;
  wednesday?: boolean | null;
  thursday?: boolean | null;
  friday?: boolean | null;
  saturday?: boolean | null;
  sundayStart?: string | null;
  sundayFinish?: string | null;
  mondayStart?: string | null;
  mondayFinish?: string | null;
  tuesdayStart?: string | null;
  tuesdayFinish?: string | null;
  wednesdayStart?: string | null;
  wednesdayFinish?: string | null;
  thursdayStart?: string | null;
  thursdayFinish?: string | null;
  fridayStart?: string | null;
  fridayFinish?: string | null;
  saturdayStart?: string | null;
  saturdayFinish?: string | null;
  comments?: string;
}

// Payload for PUT /api/AgencyRequest/{id}/Cancel. Mirrors RequestCancellationDetailModel.
export interface CancelRequestPayload {
  cancellationReasonId: string;
  otherCancellationReason?: string;
}

// Payload for PUT /api/AgencyRequest/bulk-cancel. Mirrors BulkRequestCancellation.
export interface BulkCancelRequestsPayload {
  ids: string[];
  cancellationReasonId: string;
  otherCancellationReason?: string;
}

export interface BulkCancelRequestsResult {
  cancelled: number;
  skipped: number;
}

// Payload for PUT /api/AgencyRequest/bulk-recruiters. Mirrors BulkRequestRecruiters.
// Replace semantics: clears existing recruiters on each request id and assigns the provided ones.
// Empty recruiterIds = unassign all.
export interface BulkUpdateRecruitersPayload {
  ids: string[];
  recruiterIds: string[];
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
}

// Body for POST /api/AgencyRequest/{requestId}/Applicant. Mirrors RequestApplicantModel.
export interface CreateRequestApplicantModel {
  workerProfileId?: string | null;
  candidateId?: string | null;
  comments?: string;
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
}

// Body for PUT /api/AgencyRequest/{requestId}/Applicant/{id}. Mirrors CommentsModel.
export interface UpdateApplicantCommentsPayload {
  comments?: string;
}

export interface AgencyRequestRecruiterModel {
  recruiterId: string;
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

// Recruiter attached to a request. Mirrors RequestRecruiterDetailModel.
export interface AgencyRequestRecruiterItem {
  recruiterId: string;
  email?: string;
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

// Note item returned by any GET /api/.../Note endpoint.
// Mirrors backend NoteModel (Covenant.Common.Models.NoteModel).
export interface NoteItem {
  id: string;
  note: string;
  color?: string;
  createdAt?: string;
  createdBy?: string;
}

export interface NotePagination {
  page: number;
  size: number;
}

// Response returned by POST /api/.../Note. Same shape as NoteModel / NoteItem.
export type CreateNoteResponse = NoteItem;

// ---------------------------------------------------------------------------
// Notes callback payloads — used by ModalNotes and similar note-manager widgets
// that receive CRUD callbacks as props. Two variants:
//   - Standard: scoped to a single owner (userId) — worker / candidate / company / request
//   - RequestScoped: scoped to a worker within a specific request (requestId + userId)
// ---------------------------------------------------------------------------

export interface NotesFetchPayload {
  userId: string;
  pagination: NotePagination;
}

export interface NotesCreatePayload {
  userId: string;
  model: NoteModel;
}

export interface NotesUpdatePayload {
  userId: string;
  id: string;
  model: NoteModel;
}

export interface NotesDeletePayload {
  userId: string;
  id: string;
}

export interface RequestNotesFetchPayload {
  requestId: string;
  userId: string;
  pagination: NotePagination;
}

export interface RequestNotesCreatePayload {
  requestId: string;
  userId: string;
  model: NoteModel;
}

export interface RequestNotesUpdatePayload {
  requestId: string;
  userId: string;
  id: string;
  model: NoteModel;
}

export interface RequestNotesDeletePayload {
  requestId: string;
  userId: string;
  id: string;
}

// ---------------------------------------------------------------------------
// Agency reports
// ---------------------------------------------------------------------------

// Filter used by agency accounting reports endpoints.
// Mirrors backend HoursWorkedFilter (+ the weekly-payroll `weekEnding` shortcut).
export interface AgencyReportFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  startDate?: string;
  endDate?: string;
  companyId?: string;
  jobPositionRateId?: string;
  weekEnding?: string;
}

// Response shape of GET /api/agency/accounting/reports/hours-worked.
// Mirrors backend HoursWorkedResume.
export interface HoursWorkedResume {
  totalRegularHours: number;
  totalOvertimeHours: number;
  totalHolidayHours: number;
  totalNightHours: number;
  totalHours: number;
  totalPayRegular: number;
  totalPayOvertime: number;
  totalPayHoliday: number;
  totalPayNight: number;
  totalPay: number;
  detail: HoursWorkedResponseItem[];
}

// Loose detail row inside HoursWorkedResume.detail (backend HoursWorkedResponse).
export interface HoursWorkedResponseItem {
  workerFullName?: string;
  jobTitle?: string;
  totalHours?: number;
  totalPay?: number;
  regularHours?: number;
  overtimeHours?: number;
  holidayHours?: number;
  nightHours?: number;
}

// Payment report row returned by GET /api/agency/accounting/reports/payments.
// Mirrors backend WeeklyPayrollModel.
export interface WeeklyPayrollItem {
  totalNet: number;
  weekEnding: string;
  numberOfPayStubs: number;
  displayWeekEnding: string;
}
