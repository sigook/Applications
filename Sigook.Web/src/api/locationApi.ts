import http from '@/security/apiService';
import type { Country, Province, City } from '@/types/common';

export function getCountries(): Promise<Country[]> {
  return http.get('/api/Location/country').then(r => r.data);
}

export function getProvinces(countryId: string): Promise<Province[]> {
  return http.get(`/api/Location/province/${countryId}`).then(r => r.data);
}

export function getCities(provinceId: string): Promise<City[]> {
  return http.get(`/api/Location/city/${provinceId}`).then(r => r.data);
}

export function createCity(city: { value: string; code?: string; province?: { id: string } }): Promise<City> {
  return http.post('/api/Location/city', city).then(r => r.data);
}

export function addProvinceSetting(provinceId: string, settings: { paidHolidays?: boolean; overtimeStartsAfter?: number }): Promise<void> {
  return http.post(`/api/Location/province/${provinceId}/settings`, settings).then(r => r.data);
}
