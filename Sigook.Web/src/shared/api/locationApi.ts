import { api } from '@/app/security/apiService';
import type { Country, Province, City, LocationTax, ProvinceSettings } from '@/shared/types/common';

export function getCountries(): Promise<Country[]> {
  return api.get<Country[]>('/api/location/country');
}

export function getProvinces(countryId: string): Promise<Province[]> {
  return api.get<Province[]>(`/api/location/province/${countryId}`);
}

export function getCities(provinceId: string): Promise<City[]> {
  return api.get<City[]>(`/api/location/city/${provinceId}`);
}

export function createCity(city: { value: string; code?: string; province?: { id: string } }): Promise<City> {
  return api.post<City>('/api/location/city', city);
}

export function updateProvinceSettings(provinceId: string, settings: ProvinceSettings): Promise<void> {
  return api.put(`/api/location/province/${provinceId}/settings`, settings);
}

export function getLocationTax(locationId: string): Promise<LocationTax | null> {
  return api.get<LocationTax | null>(`/api/agency/accounting/locations/${locationId}/tax`);
}

export function upsertLocationTax(locationId: string, model: LocationTax): Promise<void> {
  return api.put(`/api/agency/accounting/locations/${locationId}/tax`, model);
}
