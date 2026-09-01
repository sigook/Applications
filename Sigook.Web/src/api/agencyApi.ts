import { api } from '@/security/apiService';
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
  return api.get<AgencyDetail>('/api/Agency/Profile');
}

// Agency CRUD
export function getAgency(id: string): Promise<AgencyDetail> {
  return api.get<AgencyDetail>(`/api/Agency/${id}`);
}

export function getAgenciesList(filter: AgencyListFilter): Promise<PaginatedList<AgencyListItem>> {
  return api.get<PaginatedList<AgencyListItem>>('/api/Agency', { params: { ...filter } });
}

export function createAgency(model: CreateAgencyModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/Agency', model);
}

export function updateAgency(agency: AgencyDetail): Promise<void> {
  return api.put('/api/Agency', agency);
}

// Agency Personnel (users of the agency back-office)
export function getAgencyPersonnel(): Promise<AgencyPersonnelListItem[]> {
  return api.get<AgencyPersonnelListItem[]>('/api/agency/personnel');
}

export function createAgencyPersonnel(model: AgencyPersonnelCreateModel): Promise<void> {
  return api.post('/api/agency/personnel', model);
}

export function updateAgencyPersonnel(id: string, model: AgencyPersonnelCreateModel): Promise<void> {
  return api.put(`/api/agency/personnel/${id}`, model);
}

// Roles the logged-in user is allowed to assign when creating personnel
export function getAssignableRoles(): Promise<string[]> {
  return api.get<string[]>('/api/agency/personnel/Roles');
}

export function deleteAgencyPersonnel(id: string): Promise<void> {
  return api.del(`/api/agency/personnel/${id}`);
}

// Agency Locations (billing addresses)
export function getAgencyLocations(): Promise<AgencyLocationDetail[]> {
  return api.get<AgencyLocationDetail[]>('/api/Agency/Location');
}

export function createAgencyLocation(model: AgencyLocationDetail): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/Agency/Location', model);
}

export function updateAgencyLocation(id: string, model: AgencyLocationDetail): Promise<void> {
  return api.put(`/api/Agency/Location/${id}`, model);
}

export function deleteAgencyLocation(id: string): Promise<void> {
  return api.del(`/api/Agency/Location/${id}`);
}

// Personnel Agencies (agencies a user has access to + switching)
export function getPersonnelAgencies(): Promise<PersonnelAgencyItem[]> {
  return api.get<PersonnelAgencyItem[]>('/api/agency/personnel/Agencies');
}

export function switchPersonnelAgency(id: string): Promise<void> {
  return api.put(`/api/agency/personnel/Agencies/${id}`);
}
