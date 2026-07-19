import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  CompanyInteraction,
  CompanyInteractionFilter,
  CreateCompanyInteractionModel,
  UpdateCompanyInteractionModel,
} from '@/types/companyInteraction';

const base = '/api/agency/sales/companyinteractions';

export function getCompanyInteractions(filter: CompanyInteractionFilter): Promise<PaginatedList<CompanyInteraction>> {
  return api.get<PaginatedList<CompanyInteraction>>(base, { params: { ...filter } });
}

export function createCompanyInteraction(model: CreateCompanyInteractionModel): Promise<string> {
  return api.post<string>(base, model);
}

export function updateCompanyInteraction(id: string, model: UpdateCompanyInteractionModel): Promise<void> {
  return api.put(`${base}/${id}`, model);
}

export function deleteCompanyInteraction(id: string): Promise<void> {
  return api.del(`${base}/${id}`);
}
