import { api } from '@/security/apiService';
import type { DealsByStatusFilter, DealsByStatusModel, SalesDashboardSummary, SalesRecentClient } from '@/types/sales';
import type { CompanyInteraction, Deal } from '@/types/company';

const baseUrl = '/api/agency/sales/dashboard';

// Deal count and total value per status for a period (day / week / month / quarter),
// resolved server-side in UTC. Sales users only see their own deals.
export function getDealsByStatus(filter: DealsByStatusFilter): Promise<DealsByStatusModel> {
  return api.get<DealsByStatusModel>(`${baseUrl}/deals-by-status`, { params: { ...filter } });
}

// Deals per status for the current quarter + interactions per type for the current week.
export function getSalesDashboardSummary(): Promise<SalesDashboardSummary> {
  return api.get<SalesDashboardSummary>(`${baseUrl}/summary`);
}

// The 10 clients with the most recent interactions. Sales users only count their own interactions.
export function getRecentClients(): Promise<SalesRecentClient[]> {
  return api.get<SalesRecentClient[]>(`${baseUrl}/recent-clients`);
}

// The 6 most recent interactions across all clients. Sales users only see their own.
export function getRecentInteractions(): Promise<CompanyInteraction[]> {
  return api.get<CompanyInteraction[]>(`${baseUrl}/recent-interactions`);
}

// The 6 most recent deals across all clients, by deal date. Sales users only see their own.
export function getRecentDeals(): Promise<Deal[]> {
  return api.get<Deal[]>(`${baseUrl}/recent-deals`);
}
