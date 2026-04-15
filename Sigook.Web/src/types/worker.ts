import { DayOfWeek, FileReference, LanguageProficiency } from './common';

export interface WorkerProfile {
  id: string;
  agencyId: string;
  userId: string | null;
  numberId: number;
  firstName: string;
  lastName: string;
  middleName: string;
  birthDay: string;
  genderId: string | null;
  profileImage: FileReference | null;
  socialInsurance: string;
  socialInsuranceDueDate: string | null;
  identificationNumber1: string;
  identificationType1: string | null;
  identificationNumber2: string;
  identificationType2: string | null;
  mobileNumber: string;
  phone: string;
  email: string;
  locationId: string | null;
  hasVehicle: boolean;
  approvedToWork: boolean;
  dnu: boolean;
  isSubcontractor: boolean;
  isContractor: boolean;
  workerProfileTaxCategoryId: string | null;
  workerProfileImage: string | null;
  createdAt: string;
  updatedAt: string | null;
  skills: WorkerSkill[];
  languages: WorkerLanguage[];
  licenses: WorkerLicense[];
  certificates: WorkerCertificate[];
  jobExperiences: WorkerJobExperience[];
  availabilities: WorkerAvailability[];
  locationPreferences: WorkerLocationPreference[];
}

export interface WorkerBasicInfo {
  approvedToWork: boolean;
  hasSocialInsurance: boolean;
  hasSocialInsuranceFile: boolean;
  hasIdentificationType1File: boolean;
  hasIdentificationNumber1: boolean;
  hasIdentificationType2File: boolean;
  hasIdentificationNumber2: boolean;
  hasResume: boolean;
  firstName: string;
  lastName: string;
  profileImage: FileReference | null;
}

export interface WorkerSkill {
  id: string;
  workerProfileId: string;
  skillId: string;
  experienceYears: number;
}

export interface WorkerLanguage {
  id: string;
  workerProfileId: string;
  languageId: string;
  proficiency: LanguageProficiency;
}

export interface WorkerLicense {
  id: string;
  workerProfileId: string;
  licenseName: string;
  licenseNumber: string;
  issueDate: string | null;
  expiryDate: string | null;
  fileId: string | null;
}

export interface WorkerCertificate {
  id: string;
  workerProfileId: string;
  certificateName: string;
  certificateNumber: string;
  issueDate: string | null;
  expiryDate: string | null;
  fileId: string | null;
}

export interface WorkerJobExperience {
  id: string;
  workerProfileId: string;
  companyName: string;
  jobTitle: string;
  startDate: string | null;
  endDate: string | null;
  description: string;
}

export interface WorkerAvailability {
  id: string;
  workerProfileId: string;
  dayOfWeek: DayOfWeek;
  startTime: string;
  endTime: string;
}

export interface WorkerLocationPreference {
  id: string;
  workerProfileId: string;
  cityId: string;
}

export interface WorkerExperienceForm {
  id: string | null;
  companyName: string;
  title: string;
  startDate: Date | null;
  endDate: Date | null;
  currentJob: boolean;
  description: string;
}

export interface WorkerDocumentFile {
  id: string | null;
  fileName: string;
}

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

// Worker Request types
export interface WorkerRequestFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface WorkerRequestApplyModel {
  comments?: string;
}

export interface WorkerRegisterTimeModel {
  latitude: number;
  longitude: number;
}

export interface WorkerCommentFilter {
  workerId: string;
  size: number;
  pageIndex: number;
}

export interface WorkerCommentList {
  items: WorkerComment[];
  totalItems: number;
}

export interface WorkerComment {
  id: string;
  comment: string;
  createdAt: string;
  createdBy: string;
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

export interface ClockTypeResult {
  clockType: string;
}

// ---------------------------------------------------------------------------
// Worker profile sub-info payloads (POST /api/WorkerProfile/{id}/...)
// Each mirrors the corresponding backend model in Covenant.Common.Models.Worker.
// ---------------------------------------------------------------------------

// Generic id/value pair used by many worker endpoints (backend BaseModel<Guid>).
export interface WorkerCatalogItem {
  id: string;
  value?: string;
}

export interface WorkerBasicInformationModel {
  firstName: string;
  middleName?: string;
  lastName: string;
  secondLastName?: string;
  birthDay: string;
  gender: WorkerCatalogItem | null;
  hasVehicle: boolean;
}

export interface WorkerContactInformationModel {
  mobileNumber: string;
  phone?: string;
  phoneExt?: number | null;
  location: {
    address?: string;
    city?: WorkerCatalogItem | null;
    province?: WorkerCatalogItem | null;
    postalCode?: string;
  } | null;
}

export interface WorkerEmergencyInformationModel {
  haveAnyHealthProblem: boolean;
  healthProblem?: string;
  otherHealthProblem?: string;
  contactEmergencyName?: string;
  contactEmergencyLastName?: string;
  contactEmergencyPhone?: string;
}

export interface WorkerOtherInformationModel {
  lift: WorkerCatalogItem | null;
}

// Payload for POST/PUT /api/WorkerProfile/{id}/JobExperience.
// Mirrors backend WorkerProfileJobExperienceModel.
export interface WorkerJobExperienceModel {
  company: string;
  supervisor?: string;
  duties?: string;
  startDate: string;
  endDate?: string | null;
  isCurrentJobPosition: boolean;
}

// ---------------------------------------------------------------------------
// Worker-facing request list / detail / timesheet items
// Loose shapes sourced from backend GetRequestsForWorker / GetRequestDetailForWorker.
// ---------------------------------------------------------------------------

export interface WorkerRequestListItem {
  id: string;
  numberId?: number;
  jobTitle?: string;
  companyFullName?: string;
  location?: string;
  startWorking?: string;
  finishWorking?: string;
  status?: string;
  isAsap?: boolean;
  logo?: string;
}

export interface WorkerRequestDetail extends WorkerRequestListItem {
  description?: string;
  requirements?: string;
  responsibilities?: string;
  workerRate?: number | null;
  workerSalary?: number | null;
  displayShift?: string;
}

export interface WorkerTimeSheetItem {
  id: string;
  date: string;
  clockIn?: string | null;
  clockOut?: string | null;
  totalHours?: number;
}

export interface WorkerWageHistoryItem {
  rowNumber: number;
  weekEnding: string;
  totalPaid: number;
  quantity?: number;
  total?: number;
  items?: { quantity: number; total: number }[];
}

export interface WorkerTimeSheetHistoryItem {
  rowNumber: number;
  weekEnding: string;
  totalHours: number;
}
