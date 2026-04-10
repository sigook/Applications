import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyDetail,
  AgencyListFilter,
  AgencyListItem,
  AgencyLocationDetail,
  AgencyPersonnelCreateModel,
  AgencyPersonnelListItem,
  CreateAgencyModel,
  PersonnelAgencyItem,
} from '@/types/agency';

// Profile (current logged-in agency)
export function getAgencyProfile(): Promise<AgencyDetail> {
  return http.get('/api/Agency/Profile').then(r => r.data);
}

// Agency CRUD
export function getAgency(id: string): Promise<AgencyDetail> {
  return http.get(`/api/Agency/${id}`).then(r => r.data);
}

export function getAgenciesList(filter: AgencyListFilter): Promise<PaginatedList<AgencyListItem>> {
  return http.get('/api/Agency', { params: { ...filter } }).then(r => r.data);
}

export function createAgency(model: CreateAgencyModel): Promise<{ id: string }> {
  return http.post('/api/Agency', model).then(r => r.data);
}

export function updateAgency(agency: AgencyDetail): Promise<void> {
  return http.put('/api/Agency', agency).then(() => {});
}

// Agency Personnel (users of the agency back-office)
export function getAgencyPersonnel(): Promise<AgencyPersonnelListItem[]> {
  return http.get('/api/AgencyPersonnel').then(r => r.data);
}

export function createAgencyPersonnel(model: AgencyPersonnelCreateModel): Promise<void> {
  return http.post('/api/AgencyPersonnel', model).then(() => {});
}

export function deleteAgencyPersonnel(id: string): Promise<void> {
  return http.delete(`/api/AgencyPersonnel/${id}`).then(() => {});
}

// Agency Locations (billing addresses)
export function getAgencyLocations(): Promise<AgencyLocationDetail[]> {
  return http.get('/api/Agency/Location').then(r => r.data);
}

export function createAgencyLocation(model: AgencyLocationDetail): Promise<{ id: string }> {
  return http.post('/api/Agency/Location', model).then(r => r.data);
}

export function updateAgencyLocation(id: string, model: AgencyLocationDetail): Promise<void> {
  return http.put(`/api/Agency/Location/${id}`, model).then(() => {});
}

export function deleteAgencyLocation(id: string): Promise<void> {
  return http.delete(`/api/Agency/Location/${id}`).then(() => {});
}

// Personnel Agencies (agencies a user has access to + switching)
export function getPersonnelAgencies(): Promise<PersonnelAgencyItem[]> {
  return http.get('/api/PersonnelAgency').then(r => r.data);
}

export function switchPersonnelAgency(id: string): Promise<void> {
  return http.put(`/api/PersonnelAgency/${id}`).then(() => {});
}
