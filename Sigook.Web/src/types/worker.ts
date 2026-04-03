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
