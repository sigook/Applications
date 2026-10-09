// Company Invoice models
export interface CompanyInvoiceFilter {
  sortBy: number;
  isDescending: boolean;
  pageIndex: number;
  pageSize: number;
}

export interface CompanyInvoiceListItem {
  id: string;
  numberId: number;
  invoiceNumber: string;
  createdAt: string;
  weekEnding: string | null;
  totalNet: number;
}
