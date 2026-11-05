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
