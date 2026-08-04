import { api } from '@/security/apiService';
import type { Country, Province, City, LocationTax, ProvinceSettings } from '@/types/common';

export function getCountries(): Promise<Country[]> {
  return api.get<Country[]>('/api/Location/country');
}

export function getProvinces(countryId: string): Promise<Province[]> {
  return api.get<Province[]>(`/api/Location/province/${countryId}`);
}

export function getCities(provinceId: string): Promise<City[]> {
  return api.get<City[]>(`/api/Location/city/${provinceId}`);
}

export function createCity(city: { value: string; code?: string; province?: { id: string } }): Promise<City> {
  return api.post<City>('/api/Location/city', city);
}

export function updateProvinceSettings(provinceId: string, settings: ProvinceSettings): Promise<void> {
  return api.put(`/api/Location/province/${provinceId}/settings`, settings);
}

export function getLocationTax(locationId: string): Promise<LocationTax | null> {
  return api.get<LocationTax | null>(`/api/Location/${locationId}/tax`);
}

export function upsertLocationTax(locationId: string, model: LocationTax): Promise<void> {
  return api.put(`/api/Location/${locationId}/tax`, model);
}
