import { CatalogItem, CovenantFileModel, DayOfWeek, FileReference, LanguageProficiency, LocationDetailModel } from './common';

export interface WorkerProfileLicenseDetail {
  license: CovenantFileModel;
  number: string | null;
  issued: string | null;
  expires: string | null;
}

export interface WorkerProfileSkill {
  id: string | null;
  skill: string;
}

export interface WorkerProfileJobExperienceDetail {
  id: string;
  company: string;
  supervisor: string | null;
  duties: string | null;
  startDate: string;
  endDate: string | null;
  isCurrentJobPosition: boolean;
}

export interface WorkerProfileDetail {
  id: string;
  numberId: number;
  profileImage: CovenantFileModel | null;
  firstName: string;
  middleName: string | null;
  lastName: string;
  secondLastName: string | null;
  birthDay: string;
  gender: CatalogItem | null;
  socialInsurance: string | null;
  socialInsuranceExpire: boolean;
  dueDate: string | null;
  socialInsuranceFile: CovenantFileModel | null;
  identificationNumber1: string | null;
  identificationNumber2: string | null;
  havePoliceCheckBackground: boolean;
  identificationType1File: CovenantFileModel | null;
  identificationType2File: CovenantFileModel | null;
  identificationType1: CatalogItem | null;
  identificationType2: CatalogItem | null;
  policeCheckBackGround: CovenantFileModel | null;
  mobileNumber: string | null;
  phone: string | null;
  phoneExt: number | null;
  location: LocationDetailModel | null;
  hasVehicle: boolean;
  licenses: WorkerProfileLicenseDetail[];
  certificates: CovenantFileModel[];
  otherDocuments: CovenantFileModel[];
  availabilities: CatalogItem[];
  availabilityTimes: CatalogItem[];
  availabilityDays: CatalogItem[];
  locationPreferences: CatalogItem[];
  lift: CatalogItem | null;
  languages: CatalogItem[];
  skills: WorkerProfileSkill[];
  resume: CovenantFileModel | null;
  haveAnyHealthProblem: boolean;
  healthProblem: string | null;
  otherHealthProblem: string | null;
  contactEmergencyName: string | null;
  contactEmergencyLastName: string | null;
  contactEmergencyPhone: string | null;
  jobExperiences: WorkerProfileJobExperienceDetail[];
  email: string;
  approvedToWork: boolean;
  workerId: string;
  isSubcontractor: boolean;
  isContractor: boolean;
  federalTaxCategory: number | null;
  provincialTaxCategory: number | null;
  cpp: number | null;
  ei: number | null;
  dnu: boolean;
  createdBy: string | null;
  punchCardId: string | null;
  externalId: string | null;
  wcCode: string | null;
}

export type WorkerAttentionSeverity = 'danger' | 'warning';

export type WorkerStatusTone = 'success' | 'warning' | 'danger' | 'neutral';

export interface WorkerExpiryStatus {
  label: string;
  tone: WorkerStatusTone;
}

export type WorkerDocumentKind = 'identification' | 'policeCheck' | 'resume' | 'license' | 'certificate' | 'other';

export interface WorkerDocumentRow {
  key: string;
  kind: WorkerDocumentKind;
  type: string;
  name: string;
  number: string | null;
  file: CovenantFileModel | null;
  expires: string | null;
  status: WorkerExpiryStatus;
  deletableId: string | null;
}

export interface WorkerAttentionItem {
  key: string;
  severity: WorkerAttentionSeverity;
  title: string;
  detail: string;
  sectionId: WorkerProfileSectionId;
}

export type WorkerProfileSectionId =
  | 'personal'
  | 'contact'
  | 'documents'
  | 'preferences'
  | 'skills'
  | 'experience'
  | 'comments';

export interface WorkerProfileSection {
  id: WorkerProfileSectionId;
  label: string;
  isComplete: boolean | null;
  pendingCount: number;
}

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
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
}

export interface WorkerRequestApplyModel {
  comments?: string;
}

export interface WorkerRegisterTimeModel {
  latitude: number;
  longitude: number;
}

export interface WorkerCommentFilter {
  size: number;
  pageIndex: number;
}

export interface WorkerCommentCreateModel {
  comment: string;
  rate: number;
}

export interface WorkerCommentList {
  items: WorkerComment[];
  totalItems: number;
}

export interface WorkerComment {
  id: string;
  comment: string;
  rate: number;
  numberId: number;
  createdAt: string;
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
  agencyFullName?: string;
  location?: string;
  entrance?: string;
  workersQuantity?: number;
  status?: string;
  isAsap?: boolean;
  workerApprovedToWork?: string;
  workerRate?: number | null;
  workerSalary?: number | null;
  createdAt?: string;
  startAt?: string;
  finishAt?: string | null;
  durationTerm?: string;
}

export interface WorkerRequestDetail extends WorkerRequestListItem {
  agencyLogo?: string;
  description?: string;
  requirements?: string;
  responsibilities?: string;
  workerRate?: number | null;
  workerSalary?: number | null;
  displayShift?: string;
}

// Matches API TimeSheetListModel.
export interface WorkerTimeSheetItem {
  id: string;
  day: string;
  clockIn?: string | null;
  clockOut?: string | null;
  timeIn: string;
  timeOut?: string | null;
  totalHours: number;
  totalHoursApproved: number;
  wasApproved: boolean;
  canUpdate: boolean;
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
