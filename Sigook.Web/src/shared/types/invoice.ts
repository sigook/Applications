// Full invoice summary. Mirrors backend InvoiceSummaryModel.
// Returned by POST /Preview, GET /CompanyInvoice/{id}, and related endpoints.
export interface InvoiceSummaryModel {
  id: string;
  numberId: number;
  invoiceNumber: string;
  createdAt: string;
  companyProfileId: string;
  companyFullName?: string;
  email?: string;
  address?: string;
  htmlNotes?: string;
  fax?: string;
  faxExt?: number | null;
  hstNumber?: string;
  phonePrincipal?: string;
  phonePrincipalExt?: number | null;
  agencyFullName?: string;
  agencyLogoFileName?: string;
  agencyAddress?: string;
  agencyPhone?: string;
  agencyPhoneExt?: number | null;
  agencyFax?: string;
  agencyFaxExt?: number | null;
  agencyWebSite?: string;
  subTotal: number;
  tax: number;
  total: number;
  taxName?: string;
  weedEnding?: string | null;
  invoiceColor?: number;
  invoicePayroll?: number;
  clientSiteAddress?: string;
  discounts: InvoiceSummaryDiscountItem[];
  holidays: InvoiceSummaryHolidayItem[];
  additionalItems: InvoiceSummaryAdditionalItem[];
  items: InvoiceSummaryItem[];
}

// Result of POST /api/agency/accounting/invoices/Preview. Mirrors backend InvoicePreviewModel.
export interface InvoicePreviewModel {
  subTotal: number;
  tax: number;
  taxName?: string;
  total: number;
  items: InvoiceSummaryItem[];
  discounts: InvoiceSummaryItem[];
}

export interface InvoiceSummaryItem {
  description: string;
  quantity: number;
  total: number;
  unitPrice: number;
}

export interface InvoiceSummaryDiscountItem {
  amount: number;
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface InvoiceSummaryAdditionalItem {
  quantity: number;
  unitPrice: number;
  total: number;
  description: string;
}

export interface InvoiceSummaryHolidayItem {
  amount: number;
  hours: number;
  description: string;
  unitPrice: number;
}
