import type { CovenantFileModel } from '@/shared/types/common';
import type { CompanyProfileDetail, CompanyProfileSummary } from '@/shared/company-profile/types';
import type { CompanyStatus } from '@/shared/constants/enums';
import type { LocationDetailModel, Province } from '@/shared/types/common';
import type { RequestShiftModel } from '@/shared/request-detail/types';

// Matches AgencyCompanyProfileDetailModel
export interface AgencyCompanyProfileDetail extends CompanyProfileDetail {
  summary: CompanyProfileSummary;
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

// PATCH /api/agency/recruiting/clients/{id}/{PaidHolidays|Overtime|RequiresPermissionToSeeRequests}
// Matches CompanyProfileSettingsUpdateModel — .NET binds only the fields it needs
export interface CompanyProfileSettingsUpdate {
  requiresPermissionToSeeRequests?: boolean;
  overtimeStartsAfter?: number;
  paidHolidays?: boolean;
}

// Filter for GET /api/agency/recruiting/clients. Mirrors backend GetCompanyForAgencyFilter.
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
  companyStatuses?: CompanyStatus[];
  salesRepresentative?: string;
  salesPersonnelId?: string;
}

// Item returned by GET /api/agency/recruiting/clients.
export interface AgencyCompanyListItem {
  id: string;
  fullName: string;
  email: string;
  numberId?: number;
  companyStatus?: string;
  active?: boolean;
  createdAt?: string;
}

// Related records that prevent a company from being deleted.
// Mirrors backend CompanyDeletionBlockerModel.
export interface CompanyDeletionBlocker {
  entity: string;
  count: number;
}

// Result of asking the backend whether a company can be deleted.
// Mirrors backend CompanyDeletionCheckModel.
export interface CompanyDeletionCheck {
  id: string;
  fullName: string;
  canDelete: boolean;
  blockers: CompanyDeletionBlocker[];
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
  email?: string | null;
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
  jobPosition?: string;
  rate?: number;
  workerRate?: number;
  workerRateMin?: number | null;
  workerRateMax?: number | null;
  overtimeStartsAfter?: number | null;
  description?: string;
  shift?: RequestShiftModel | null;
  createdAt?: string | null;
  createdBy?: string;
  displayShift?: string;
}

export interface AgencyCompanyJobPositionFilter {
  role?: string;
}

export interface VaccinationRequiredModel {
  vaccinationRequired: boolean;
  vaccinationRequiredComments?: string;
}

// Body for PUT /api/agency/recruiting/clients/{profileId}/invoicenotes.
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

export interface CompanyProfileFormState extends Partial<CompanyProfileDetail> {
  companyProfileId?: string | null;
  password?: string;
}
