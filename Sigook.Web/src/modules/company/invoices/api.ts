import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { CompanyInvoiceFilter, CompanyInvoiceListItem } from '@/modules/company/invoices/types';

export function getCompanyInvoice(filter: CompanyInvoiceFilter): Promise<PaginatedList<CompanyInvoiceListItem>> {
  return api.get<PaginatedList<CompanyInvoiceListItem>>('/api/company/invoices', { params: { ...filter } });
}
