import http from '@/security/apiService';
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
  JobPosition,
  Skill,
  CancellationReason,
  CatalogItem,
  TaxCategory,
} from '@/types/common';

export function getGenders(): Promise<Gender[]> {
  return http.get('/api/Catalog/gender').then(r => r.data);
}

export function getIdentificationTypes(): Promise<IdentificationType[]> {
  return http.get('/api/Catalog/identificationType').then(r => r.data);
}

export function getAvailability(): Promise<Availability[]> {
  return http.get('/api/Catalog/availability').then(r => r.data);
}

export function getAvailabilityTimes(): Promise<AvailabilityTime[]> {
  return http.get('/api/Catalog/availabilityTime').then(r => r.data);
}

export function getDays(): Promise<Day[]> {
  return http.get('/api/Catalog/day').then(r => r.data);
}

export function fetchLifts(): Promise<Lift[]> {
  return http.get('/api/Catalog/lift').then(r => r.data);
}

export function fetchLanguages(): Promise<Language[]> {
  return http.get('/api/Catalog/language').then(r => r.data);
}

export function getWsibGroups(): Promise<WsibGroup[]> {
  return http.get('/api/Catalog/wsibgroup').then(r => r.data);
}

export function getJobPositions(): Promise<JobPosition[]> {
  return http.get('/api/Catalog/jobPosition').then(r => r.data);
}

export function getSkills(): Promise<Skill[]> {
  return http.get('/api/Catalog/skills').then(r => r.data);
}

export function getIndustries(): Promise<Industry[]> {
  return http.get('/api/Catalog/industry').then(r => r.data);
}

export function getReasonCancellationRequest(): Promise<CancellationReason[]> {
  return http.get('/api/Catalog/reasonCancellationRequest').then(r => r.data);
}

export function getCompanyStatus(): Promise<CatalogItem<number>[]> {
  return http.get('/api/Catalog/companyStatus').then(r => r.data);
}

export function getTaxCategories(): Promise<TaxCategory[]> {
  return http.get('/api/Catalog/tax-categories').then(r => r.data);
}

export function addIndustry(industry: { id?: string; value: string }): Promise<Industry> {
  return http.post('/api/Catalog/industry', industry).then(r => r.data);
}
