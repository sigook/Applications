import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { AgencyDetail } from '@/modules/agency/profile/types';
import type { AgencyListFilter, AgencyListItem, CreateAgencyModel } from '@/modules/agency/sales/agencies/types';

// Agency CRUD
export function getAgency(id: string): Promise<AgencyDetail> {
  return api.get<AgencyDetail>(`/api/agency/sales/agencies/${id}`);
}

export function getAgenciesList(filter: AgencyListFilter): Promise<PaginatedList<AgencyListItem>> {
  return api.get<PaginatedList<AgencyListItem>>('/api/agency/sales/agencies', { params: { ...filter } });
}

export function createAgency(model: CreateAgencyModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/agency/sales/agencies', model);
}

export function updateAgency(agency: AgencyDetail): Promise<void> {
  return api.put('/api/agency/sales/agencies', agency);
}
