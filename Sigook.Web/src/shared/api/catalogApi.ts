import { api } from '@/app/security/apiService';
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
} from '@/shared/types/common';

export function getGenders(): Promise<Gender[]> {
  return api.get<Gender[]>('/api/catalog/gender');
}

export function getIdentificationTypes(): Promise<IdentificationType[]> {
  return api.get<IdentificationType[]>('/api/catalog/identificationType');
}

export function getAvailability(): Promise<Availability[]> {
  return api.get<Availability[]>('/api/catalog/availability');
}

export function getAvailabilityTimes(): Promise<AvailabilityTime[]> {
  return api.get<AvailabilityTime[]>('/api/catalog/availabilityTime');
}

export function getDays(): Promise<Day[]> {
  return api.get<Day[]>('/api/catalog/day');
}

export function fetchLifts(): Promise<Lift[]> {
  return api.get<Lift[]>('/api/catalog/lift');
}

export function fetchLanguages(): Promise<Language[]> {
  return api.get<Language[]>('/api/catalog/language');
}

export function getWsibGroups(): Promise<WsibGroup[]> {
  return api.get<WsibGroup[]>('/api/catalog/wsibgroup');
}

export function getSkills(): Promise<Skill[]> {
  return api.get<Skill[]>('/api/catalog/skills');
}

export function getIndustries(): Promise<Industry[]> {
  return api.get<Industry[]>('/api/catalog/industry');
}

export function getReasonCancellationRequest(): Promise<CancellationReason[]> {
  return api.get<CancellationReason[]>('/api/catalog/reasonCancellationRequest');
}

export function getCompanyStatus(): Promise<CatalogItem<number>[]> {
  return api.get<CatalogItem<number>[]>('/api/catalog/companyStatus');
}

export function getSources(): Promise<Source[]> {
  return api.get<Source[]>('/api/catalog/source');
}

export function getSourcesForRequests(): Promise<Source[]> {
  return api.get<Source[]>('/api/catalog/source/requests');
}

export function getTaxCategories(): Promise<TaxCategory[]> {
  return api.get<TaxCategory[]>('/api/catalog/tax-categories');
}

export function addIndustry(industry: { id?: string; value: string }): Promise<Industry> {
  return api.post<Industry>('/api/catalog/industry', industry);
}
