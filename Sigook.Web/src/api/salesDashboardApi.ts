import dashboardData from '@/data/sales/salesDashboard.json';
import type { SalesDashboardModel } from '@/types/sales';

// ---------------------------------------------------------------------------
// Sales dashboard
//
// Static prototype: the payload is served from src/data/sales/salesDashboard.json
// and shaped exactly like the future endpoint response. To go live, replace the
// body with the commented call below — the signature and every caller stay as is.
//
//   import { api } from '@/security/apiService';
//   export function getSalesDashboard(): Promise<SalesDashboardModel> {
//     return api.get<SalesDashboardModel>('/api/agency/sales/dashboard');
//   }
// ---------------------------------------------------------------------------

export function getSalesDashboard(): Promise<SalesDashboardModel> {
  return Promise.resolve(dashboardData as unknown as SalesDashboardModel);
}
