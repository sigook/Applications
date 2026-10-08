import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { AgencyRequestFilter, AgencyRequestsPagedResponse } from '@/modules/agency/recruiting/requests/types';
import type { AgencyCompanyFilter, AgencyCompanyListItem } from '@/modules/agency/recruiting/clients/types';

const requestsUrl = '/api/agency/sales/requests';
const companiesUrl = '/api/agency/sales/companyprofiles';

export function getSalesRequests(filter: AgencyRequestFilter): Promise<AgencyRequestsPagedResponse> {
  return api.get<AgencyRequestsPagedResponse>(requestsUrl, { params: { ...filter } });
}

export function getSalesCompanies(filter: AgencyCompanyFilter): Promise<PaginatedList<AgencyCompanyListItem>> {
  return api.get<PaginatedList<AgencyCompanyListItem>>(companiesUrl, { params: { ...filter } });
}
