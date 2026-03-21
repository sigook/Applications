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

export interface City {
  id: string;
  name: string;
  provinceId: string;
}

export interface Province {
  id: string;
  name: string;
  code: string;
  countryId: string;
}

export interface Country {
  id: string;
  name: string;
  code: string;
}

export interface FileReference {
  pathFile: string;
}

export interface CatalogItem {
  id: string;
  name: string;
}

export type Gender = CatalogItem;
export type IdentificationType = CatalogItem;
export type Skill = CatalogItem;
export type Language = CatalogItem;
export type JobPosition = CatalogItem;
export type WsibGroup = CatalogItem;
export type Industry = CatalogItem;
export type CompanyStatusCatalog = CatalogItem;
export type CancellationReason = CatalogItem;

export interface TaxCategory {
  id: string;
  name: string;
  federalClaim: number;
  provincialClaim: number;
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
