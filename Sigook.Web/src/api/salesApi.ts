import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyRequestFilter,
  AgencyRequestsPagedResponse,
  AgencyCompanyFilter,
  AgencyCompanyListItem,
} from '@/types/agency';

const requestsUrl = '/api/agency/sales/requests';
const companiesUrl = '/api/agency/sales/companyprofiles';

export function getSalesRequests(filter: AgencyRequestFilter): Promise<AgencyRequestsPagedResponse> {
  return api.get<AgencyRequestsPagedResponse>(requestsUrl, { params: { ...filter } });
}

export function getSalesRequestsFile(filter: AgencyRequestFilter): Promise<Blob> {
  return api.get<Blob>(`${requestsUrl}/File`, { params: { ...filter }, responseType: 'blob' });
}

export function getSalesCompanies(filter: AgencyCompanyFilter): Promise<PaginatedList<AgencyCompanyListItem>> {
  return api.get<PaginatedList<AgencyCompanyListItem>>(companiesUrl, { params: { ...filter } });
}

export function getSalesCompaniesFile(filter: AgencyCompanyFilter): Promise<Blob> {
  return api.get<Blob>(`${companiesUrl}/File`, { params: { ...filter }, responseType: 'blob' });
}
