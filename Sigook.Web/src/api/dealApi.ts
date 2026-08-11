import { api } from '@/security/apiService';
import { buildMultipartFormData } from '@/utils/multipart';
import type { PaginatedList } from '@/types/common';
import type { Deal, DealFilter, CreateDealModel, UpdateDealModel } from '@/types/company';

const base = '/api/agency/sales/deals';

export function getDeals(filter: DealFilter): Promise<PaginatedList<Deal>> {
  return api.get<PaginatedList<Deal>>(base, { params: { ...filter } });
}

export function createDeal(model: CreateDealModel, file?: File | null): Promise<string> {
  return api.post<string>(
    base,
    buildMultipartFormData(model, file && model.fileName ? { [model.fileName]: file } : {}),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function updateDeal(id: string, model: UpdateDealModel): Promise<void> {
  return api.put(`${base}/${id}`, model);
}

export function deleteDeal(id: string): Promise<void> {
  return api.del(`${base}/${id}`);
}
