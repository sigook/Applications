import { Location } from './common';

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
  location?: Location;
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

export interface CovenantFileModel {
  id: string;
  pathFile: string;
  fileName: string;
  description: string;
  canDownload: boolean;
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
  status: string;
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
  status: string;
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
