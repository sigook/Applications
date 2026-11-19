export interface CatalogItem {
  id: string;
  value: string;
}

export interface ProvinceModel {
  id: string;
  value: string;
  code?: string | null;
}

export interface CityModel {
  id: string;
  value: string;
  code?: string | null;
  province?: ProvinceModel | null;
}

export interface PaginatedList<T> {
  pageIndex: number;
  totalPages: number;
  totalItems: number;
  items: T[];
}

export interface IdResponse {
  id: string;
}

export interface AgencyRequestListModel {
  id: string;
  numberId: number;
  jobTitle: string;
  companyFullName: string;
  address?: string | null;
  city?: string | null;
  provinceName?: string | null;
  postalCode?: string | null;
  requestStatus: number;
  workersQuantity: number;
  workersQuantityWorking: number;
}

export interface PhoneNumberModel {
  id?: string | null;
  phoneNumber: string;
}

export interface SkillModel {
  id?: string | null;
  skill: string;
}

export interface CandidateListModel {
  id: string;
  name: string;
  email?: string | null;
  city?: string | null;
  phoneNumbers?: PhoneNumberModel[] | null;
  recruiter?: string | null;
  createdAt: string;
  dnu: boolean;
  source?: string | null;
  requests?: CatalogItem[] | null;
}

export interface CandidateCreateModel {
  name: string;
  email?: string | null;
  address?: string | null;
  postalCode?: string | null;
  city?: CityModel | null;
  gender?: CatalogItem | null;
  hasVehicle: boolean;
  residencyStatus?: string | null;
  sourceId?: string | null;
  dnu: boolean;
  phoneNumbers: PhoneNumberModel[];
  skills: SkillModel[];
  fileName?: string | null;
}

export interface CovenantFileModel {
  fileName: string;
  description: string;
}

export interface RequestApplicantModel {
  candidateId: string;
  comments?: string | null;
}

export interface NoteModel {
  note: string;
}

export interface UserInfo {
  sub?: string | null;
  name?: string | null;
  email?: string | null;
  nickname?: string | null;
  role?: unknown;
}

export interface CompanyProfileListModel {
  id: string;
  numberId: number;
  fullName: string;
  locations?: string[] | null;
  active: boolean;
  industry?: string | null;
}

export interface JobPositionModel {
  id: string;
  jobPosition: string | null;
  rate: number;
  workerRate: number;
  [field: string]: unknown;
}

export interface TeamsFileModel {
  name: string;
  webUrl?: string | null;
  size?: number | null;
  lastModified?: string | null;
  lastModifiedBy?: string | null;
}

export interface IdentificationTypeModel {
  id: string;
  value: string;
  code: number;
}

export interface WorkerListModel {
  id: string;
  workerId: string;
  numberId: number;
  fullName: string;
  email?: string | null;
  mobileNumber?: string | null;
  approvedToWork: boolean;
  dnu: boolean;
}

export interface WorkerFileModel {
  fileName: string;
  description?: string | null;
}

export interface WorkerLicenseModel {
  license: WorkerFileModel;
  number?: string | null;
  issued?: string | null;
  expires?: string | null;
}

export interface WorkerDocumentsDetail {
  id: string;
  firstName: string;
  lastName: string;
  socialInsurance?: string | null;
  socialInsuranceExpire: boolean;
  dueDate?: string | null;
  socialInsuranceFile?: WorkerFileModel | null;
  identificationNumber1?: string | null;
  identificationNumber2?: string | null;
  identificationType1?: CatalogItem | null;
  identificationType2?: CatalogItem | null;
  identificationType1File?: WorkerFileModel | null;
  identificationType2File?: WorkerFileModel | null;
  havePoliceCheckBackground: boolean;
  policeCheckBackGround?: WorkerFileModel | null;
  resume?: WorkerFileModel | null;
  licenses: WorkerLicenseModel[];
  certificates: WorkerFileModel[];
  otherDocuments: WorkerFileModel[];
}

export interface WorkerIdentificationModel {
  identificationNumber1?: string | null;
  identificationType1?: { id: string } | null;
  identificationType1File?: WorkerFileModel | null;
  identificationNumber2?: string | null;
  identificationType2?: { id: string } | null;
  identificationType2File?: WorkerFileModel | null;
  havePoliceCheckBackground: boolean;
  policeCheckBackGround?: WorkerFileModel | null;
  resume?: WorkerFileModel | null;
}

export interface WorkerSinModel {
  socialInsurance: string;
  socialInsuranceExpire: boolean;
  dueDate?: string | null;
  socialInsuranceFile: WorkerFileModel;
}
