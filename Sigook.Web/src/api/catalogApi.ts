import { api } from '@/security/apiService';
import type {
  Gender,
  IdentificationType,
  Availability,
  AvailabilityTime,
  Day,
  Lift,
  Language,
  WsibGroup,
  Industry,
  Skill,
  CancellationReason,
  CatalogItem,
  TaxCategory,
  Source,
} from '@/types/common';

export function getGenders(): Promise<Gender[]> {
  return api.get<Gender[]>('/api/Catalog/gender');
}

export function getIdentificationTypes(): Promise<IdentificationType[]> {
  return api.get<IdentificationType[]>('/api/Catalog/identificationType');
}

export function getAvailability(): Promise<Availability[]> {
  return api.get<Availability[]>('/api/Catalog/availability');
}

export function getAvailabilityTimes(): Promise<AvailabilityTime[]> {
  return api.get<AvailabilityTime[]>('/api/Catalog/availabilityTime');
}

export function getDays(): Promise<Day[]> {
  return api.get<Day[]>('/api/Catalog/day');
}

export function fetchLifts(): Promise<Lift[]> {
  return api.get<Lift[]>('/api/Catalog/lift');
}

export function fetchLanguages(): Promise<Language[]> {
  return api.get<Language[]>('/api/Catalog/language');
}

export function getWsibGroups(): Promise<WsibGroup[]> {
  return api.get<WsibGroup[]>('/api/Catalog/wsibgroup');
}

export function getSkills(): Promise<Skill[]> {
  return api.get<Skill[]>('/api/Catalog/skills');
}

export function getIndustries(): Promise<Industry[]> {
  return api.get<Industry[]>('/api/Catalog/industry');
}

export function getReasonCancellationRequest(): Promise<CancellationReason[]> {
  return api.get<CancellationReason[]>('/api/Catalog/reasonCancellationRequest');
}

export function getCompanyStatus(): Promise<CatalogItem<number>[]> {
  return api.get<CatalogItem<number>[]>('/api/Catalog/companyStatus');
}

export function getSources(): Promise<Source[]> {
  return api.get<Source[]>('/api/Catalog/source');
}

export function getSourcesForRequests(): Promise<Source[]> {
  return api.get<Source[]>('/api/Catalog/source/requests');
}

export function getTaxCategories(): Promise<TaxCategory[]> {
  return api.get<TaxCategory[]>('/api/Catalog/tax-categories');
}

export function addIndustry(industry: { id?: string; value: string }): Promise<Industry> {
  return api.post<Industry>('/api/Catalog/industry', industry);
}
