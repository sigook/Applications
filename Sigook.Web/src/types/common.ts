// Common types shared across the application

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface PaginationFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
}

export interface Location {
  id: string;
  fullAddress: string;
  streetNumber: string;
  streetName: string;
  unit: string;
  cityId: string;
  postalCode: string;
  latitude: number | null;
  longitude: number | null;
}

export interface Country {
  id: string;
  value: string;
  code: string;
}

export interface ProvinceSettings {
  paidHolidays: boolean | null;
  overtimeStartsAfter: number | null;
}

export interface Province {
  id: string;
  value: string;
  code: string;
  country: Country;
  settings: ProvinceSettings | null;
}

export interface City {
  id: string;
  value: string;
  code: string;
  province: Province;
}

export interface FileReference {
  pathFile: string;
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
