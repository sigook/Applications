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
  businessName: string;
  dbaName: string;
  businessNumber: string;
  hstNumber: string;
  companyStatus: CompanyStatus;
  email: string;
  phoneNumber: string;
  website: string;
  requiresPermissionToSeeOrders: boolean;
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
  canSeeOrders: boolean;
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
  businessName: string;
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
  requiresPermissionToSeeOrders: boolean;
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
  businessName: string;
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

// PATCH /api/V2/AgencyCompanyProfile/{id}/{PaidHolidays|Overtime|RequiresPermissionToSeeOrders}
// Matches CompanyProfileSettingsUpdateModel — .NET binds only the fields it needs
export interface CompanyProfileSettingsUpdate {
  requiresPermissionToSeeOrders?: boolean;
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
