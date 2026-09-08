import { api } from '@/security/apiService';
import type { DealsByStatusFilter, DealsByStatusModel, SalesDashboardSummary } from '@/types/sales';

const baseUrl = '/api/agency/sales/dashboard';

// Deal count and total value per status for a period (day / week / month / quarter),
// resolved server-side in the business time zone. Sales users only see their own deals.
export function getDealsByStatus(filter: DealsByStatusFilter): Promise<DealsByStatusModel> {
  return api.get<DealsByStatusModel>(`${baseUrl}/deals-by-status`, { params: { ...filter } });
}

// Deals per status for the current quarter + interactions per type for the current week.
export function getSalesDashboardSummary(): Promise<SalesDashboardSummary> {
  return api.get<SalesDashboardSummary>(`${baseUrl}/summary`);
}
