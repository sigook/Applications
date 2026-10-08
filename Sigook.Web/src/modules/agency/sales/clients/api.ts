import { api } from '@/app/security/apiService';
import { buildMultipartFormData } from '@/shared/utils/multipart';
import type { PaginatedList } from '@/shared/types/common';
import type { Deal, DealFilter, CreateDealModel, UpdateDealModel, CompanyInteraction, CompanyInteractionFilter, CreateCompanyInteractionModel, UpdateCompanyInteractionModel } from '@/modules/agency/sales/clients/types';

const companyProfilesUrl = '/api/agency/recruiting/clients';

export function getCompanyInteractions(profileId: string, filter: CompanyInteractionFilter): Promise<PaginatedList<CompanyInteraction>> {
  return api.get<PaginatedList<CompanyInteraction>>(`${companyProfilesUrl}/${profileId}/Interactions`, { params: { ...filter } });
}

export function createCompanyInteraction(profileId: string, model: CreateCompanyInteractionModel): Promise<string> {
  return api.post<string>(`${companyProfilesUrl}/${profileId}/Interactions`, model);
}

export function updateCompanyInteraction(profileId: string, id: string, model: UpdateCompanyInteractionModel): Promise<void> {
  return api.put(`${companyProfilesUrl}/${profileId}/Interactions/${id}`, model);
}

export function deleteCompanyInteraction(profileId: string, id: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/Interactions/${id}`);
}

export function getDeals(profileId: string, filter: DealFilter): Promise<PaginatedList<Deal>> {
  return api.get<PaginatedList<Deal>>(`${companyProfilesUrl}/${profileId}/Deals`, { params: { ...filter } });
}

export function createDeal(profileId: string, model: CreateDealModel, file?: File | null): Promise<string> {
  return api.post<string>(
    `${companyProfilesUrl}/${profileId}/Deals`,
    buildMultipartFormData(model, file && model.fileName ? { [model.fileName]: file } : {}),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function updateDeal(profileId: string, id: string, model: UpdateDealModel, file?: File | null): Promise<void> {
  return api.put(
    `${companyProfilesUrl}/${profileId}/Deals/${id}`,
    buildMultipartFormData(model, file && model.fileName ? { [model.fileName]: file } : {}),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function deleteDeal(profileId: string, id: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/Deals/${id}`);
}
