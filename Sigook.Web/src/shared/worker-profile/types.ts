import { CatalogItem, CovenantFileModel, LocationDetailModel } from '@/shared/types/common';

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

// Payload for POST/PUT /api/worker/profile/{id}/JobExperience.
// Mirrors backend WorkerProfileJobExperienceModel.
export interface WorkerJobExperienceModel {
  company: string;
  supervisor?: string;
  duties?: string;
  startDate: string;
  endDate?: string | null;
  isCurrentJobPosition: boolean;
}
