import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { Deal, DealFilter, CreateDealModel, UpdateDealModel } from '@/types/deal';

const base = '/api/agency/sales/deals';

export function getDeals(filter: DealFilter): Promise<PaginatedList<Deal>> {
  return api.get<PaginatedList<Deal>>(base, { params: { ...filter } });
}

export function createDeal(model: CreateDealModel): Promise<string> {
  return api.post<string>(base, model);
}

export function updateDeal(id: string, model: UpdateDealModel): Promise<void> {
  return api.put(`${base}/${id}`, model);
}

export function deleteDeal(id: string): Promise<void> {
  return api.del(`${base}/${id}`);
}
