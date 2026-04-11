// Common types shared across the application

// Matches API PaginatedList<T>
export interface PaginatedList<T> {
  items: T[];
  pageIndex: number;
  totalPages: number;
  totalItems: number;
}

export interface PaginationFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
}

// Matches backend CountryModel.
export interface Country {
  id: string;
  value: string;
  code: string;
}

// Matches backend ProvinceSettingsModel.
export interface ProvinceSettings {
  paidHolidays: boolean | null;
  overtimeStartsAfter: number | null;
}

// Matches backend ProvinceModel (Covenant.Common.Models.Location.ProvinceModel).
export interface Province {
  id: string;
  value: string;
  code?: string;
  country?: Country | null;
  settings?: ProvinceSettings | null;
}

// Matches backend CityModel (Covenant.Common.Models.Location.CityModel).
export interface City {
  id: string;
  value: string;
  code?: string;
  province?: Province | null;
}

export interface FileReference {
  pathFile: string;
}

// Matches backend CovenantFileModel (Covenant.Common.Models.CovenantFileModel).
// Used for logos, identification files, resumes, etc.
export interface CovenantFileModel {
  id?: string;
  pathFile?: string;
  fileName?: string;
  description?: string;
  canDownload?: boolean;
}

// Matches backend LocationModel (Covenant.Common.Models.Location.LocationModel).
// Base location payload used by agencies/companies.
export interface LocationModel {
  id?: string;
  address?: string;
  city?: City | null;
  postalCode?: string;
  isBilling?: boolean;
}

// Matches backend LocationDetailModel. Extends LocationModel with geocoding
// and entrance/intersection fields.
export interface LocationDetailModel extends LocationModel {
  entrance?: string;
  mainIntersection?: string;
  latitude?: number | null;
  longitude?: number | null;
  formattedAddress?: string;
  isUSA?: boolean;
}

// Matches API BaseModel<T> { Id, Value }
export interface CatalogItem<T = string> {
  id: T;
  value: string;
}

export type Gender = CatalogItem;
export type IdentificationType = CatalogItem;
export type Availability = CatalogItem;
export type AvailabilityTime = CatalogItem;
export type Day = CatalogItem;
export type Lift = CatalogItem;
export type Language = CatalogItem;
export type WsibGroup = CatalogItem;
export type Industry = CatalogItem;
export type CompanyStatusCatalog = CatalogItem;
export type CancellationReason = CatalogItem;

export type TaxCategory = CatalogItem<number>;

// Matches API JobPositionDetailModel
export interface JobPosition {
  id: string;
  value: string;
  industry: string;
}

// Matches API skill endpoint { skill: string }
export interface Skill {
  skill: string;
}

export type Currency = 'CAD' | 'USD';

export enum DurationTerm {
  LongTerm = 'LongTerm',
  ShortTerm = 'ShortTerm',
}

export enum EmploymentType {
  FullTime = 'FullTime',
  PartTime = 'PartTime',
  Contractor = 'Contractor',
  Temporary = 'Temporary',
}

export enum DayOfWeek {
  Monday = 'Monday',
  Tuesday = 'Tuesday',
  Wednesday = 'Wednesday',
  Thursday = 'Thursday',
  Friday = 'Friday',
  Saturday = 'Saturday',
  Sunday = 'Sunday',
}

export enum LanguageProficiency {
  Basic = 'Basic',
  Intermediate = 'Intermediate',
  Advanced = 'Advanced',
  Native = 'Native',
}

export interface UnsubscribeRequest {
  userId: string;
  typeId: string;
}

// Mirrors backend UserNotificationListModel / UserNotificationUpdateModel.
// GET returns the full list shape; PUT only needs id + the notification flags.
export interface UserNotificationItem {
  id: number;
  title?: string;
  description?: string;
  emailNotification: boolean;
  pushNotification: boolean;
  smsNotification?: boolean;
}
