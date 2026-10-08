import type { CompanyStatus } from '@/shared/constants/enums';
import type { CatalogItem, CovenantFileModel, LocationDetailModel } from '@/shared/types/common';
import type { RequestShiftModel } from '@/shared/request-detail/types';

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
  id?: string;
  companyProfileId?: string;
  jobPosition?: string;
  rate: number;
  workerRate: number;
  workerRateMin?: number | null;
  workerRateMax?: number | null;
  overtimeStartsAfter?: number | null;
  description?: string;
  shift?: RequestShiftModel | null;
  createdAt?: string | null;
  createdBy?: string;
  displayShift?: string;
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

// Matches CompanyProfileSummaryModel
export interface CompanyProfileSummary {
  openRequestsCount: number;
  asapRequestsCount: number;
  workersWorkingCount: number;
  rolesCount: number;
  contactsCount: number;
  usersCount: number;
  locationsCount: number;
  documentsCount: number;
  salesRepresentativeName: string | null;
}

export interface CompanyProfileIndustryDetail {
  id: string;
  industry: CatalogItem | null;
  otherIndustry: string | null;
}

// Company Profile Location (matches CompanyProfileLocationDetailModel)
export interface CompanyProfileLocationDetail extends LocationDetailModel {
  id?: string;
  address: string;
  postalCode: string;
  isBilling: boolean;
  latitude: number | null;
  longitude: number | null;
  entrance: string;
  mainIntersection: string;
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
export interface CompanyContactSummary {
  firstName: string;
  middleName?: string | null;
  lastName: string;
  position?: string | null;
  mobileNumber?: string | null;
  email?: string | null;
}
