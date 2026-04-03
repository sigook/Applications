import { ref } from 'vue';
import * as locationApi from '@/api/locationApi';
import type { Country, Province, City } from '@/types/common';

export function useLocation() {
  const countries = ref<Country[]>([]);
  const provinces = ref<Province[]>([]);
  const cities = ref<City[]>([]);

  async function loadCountries(): Promise<Country[]> {
    countries.value = await locationApi.getCountries();
    return countries.value;
  }

  async function loadProvinces(countryId: string): Promise<Province[]> {
    provinces.value = await locationApi.getProvinces(countryId);
    return provinces.value;
  }

  async function loadCities(provinceId: string): Promise<City[]> {
    cities.value = await locationApi.getCities(provinceId);
    return cities.value;
  }

  async function addCity(city: { value: string; code?: string; province?: { id: string } }): Promise<City> {
    const created = await locationApi.createCity(city);
    cities.value = [...cities.value, created];
    return created;
  }

  return {
    countries,
    provinces,
    cities,
    loadCountries,
    loadProvinces,
    loadCities,
    addCity,
  };
}
