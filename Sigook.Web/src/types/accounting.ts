import type { PaginatedList } from './common';

export interface PayStub {
  id: string;
  workerProfileId: string;
  payStubNumber: number;
  payStubNumberId: number;
  typeOfWork: string;
  dateWorkBegins: string;
  dateWorkEnd: string;
  regularWage: number;
  grossPayment: number;
  vacations: number;
  publicHolidayPay: number;
  totalEarnings: number;
  cpp: number;
  ei: number;
  federalTax: number;
  provincialTax: number;
  otherDeductions: number;
  totalDeductions: number;
  totalPaid: number;
  createdAt: string;
  workerName: string;
  items: PayStubItem[];
  wageDetails: PayStubWageDetail[];
}

export interface PayStubItem {
  id: string;
  payStubId: string;
  date: string;
  hours: string;
  rate: number;
  amount: number;
  type: string;
}

export interface PayStubWageDetail {
  id: string;
  payStubId: string;
  type: string;
  hours: string;
  rate: number;
  amount: number;
}

export interface CreatePayStubModel {
  payStubNumber: number;
  workerProfileId: string;
  typeOfWork: string;
  workBegins: string;
  workEnd: string;
  otherDeductions: number;
  otherDeductionsDescription: string;
  regularHours: number;
  unitPriceRegularHours: number;
  overtimeHours: number;
  unitPriceOvertimeHours: number;
  missingHours: number;
  unitPriceMissingHours: number;
  missingOvertimeHours: number;
  unitPriceMissingOvertimeHours: number;
  statutoryWorkedHolidayPayHours: number;
  unitPriceStatutoryWorkedHolidayPayHours: number;
  other: number;
  unitPriceOther: number;
  bonusOthersDescription: string;
  other2: number;
  unitPriceOther2: number;
  bonusOthersDescription2: string;
  other3: number;
  unitPriceOther3: number;
  bonusOthersDescription3: string;
  payVacations: boolean;
  holidays: CreatePayStubPublicHolidayModel[];
}

export interface CreatePayStubPublicHolidayModel {
  date: string;
  name: string;
  hours: number;
  rate: number;
}

export interface Invoice {
  id: string;
  companyProfileId: string;
  invoiceNumber: number;
  nightShiftRate: number;
  holidayRate: number;
  overTimeRate: number;
  vacationsRate: number;
  hstRate: number;
  bonusRate: number;
  subTotal: number;
  hst: number;
  totalNet: number;
  createdAt: string;
  weekEnding: string;
  companyName: string;
  totals: InvoiceTotal[];
  holidays: InvoiceHoliday[];
  discounts: InvoiceDiscount[];
  additionalItems: InvoiceAdditionalItem[];
}

export interface InvoiceTotal {
  id: string;
  invoiceId: string;
  workerRequestId: string;
  workerName: string;
  regularHours: string;
  overtimeHours: string;
  nightShiftHours: string;
  holidayHours: string;
  regularAmount: number;
  overtimeAmount: number;
  nightShiftAmount: number;
  holidayAmount: number;
  total: number;
}

export interface InvoiceHoliday {
  id: string;
  invoiceId: string;
  name: string;
  date: string;
  hours: number;
  rate: number;
  amount: number;
}

export interface InvoiceDiscount {
  id: string;
  invoiceId: string;
  description: string;
  amount: number;
}

export interface InvoiceAdditionalItem {
  id: string;
  invoiceId: string;
  description: string;
  amount: number;
}

export interface InvoiceModel {
  id: string;
  totalNet: number;
  companyId: string;
  numberId: number;
  companyEmail: string;
}

export interface PayStubFilter {
  weekEnding?: string;
  workerProfileId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

export interface InvoiceFilter {
  page: number;
  pageSize: number;
  companyProfileId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

// ---------------------------------------------------------------------------
// Agency PayStub list / filters
// ---------------------------------------------------------------------------

// Filter for GET /api/agency/accounting/PayStubs. Mirrors backend GetPayStubsFilter.
export interface AgencyPayStubFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  payStubNumber?: string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  workerFullName?: string;
  numberId?: number | string;
}

// Item returned by GET /api/agency/accounting/PayStubs. Mirrors backend PayStubListModel.
export interface AgencyPayStubListItem {
  id: string;
  numberId: number;
  createdAt: string;
  totalPaid: number;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  secondLastName?: string;
  workerFullName: string;
  payStubNumber: string;
  payStubNumberId: number;
  canDelete: boolean;
}

// Skip payroll number autocomplete item. Mirrors backend BaseModel<Guid>.
export interface SkipPayrollNumberItem {
  id: string;
  value: string;
}

// Payload for POST /api/agency/accounting/PayStubs/skip-payroll-number.
// Backend expects a BaseModel<Guid>; only the Value is read server-side.
export interface CreateSkipPayrollNumberPayload {
  id?: string;
  value: string;
}

// Filter for GET /api/agency/accounting/reports/subcontractors.
// Backend only binds Pagination — sortBy etc. are ignored.
export interface SubcontractorPayrollFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
}

// Subcontractor payroll list item. Mirrors backend PayrollSubContractorListModel.
export interface PayrollSubContractorListItem {
  totalNet: number;
  weekEnding: string;
  numberOfWorkers: number;
  weekEndingDisplay: string;
}

// Payload for POST /api/v4/accounting/PayStub. Mirrors backend CreatePayStubModel.
export interface CreatePayStubPayload {
  workerProfileId: string;
  position?: string;
  workBegins: string;
  workEnd: string;
  otherDeductions: number;
  otherDeductionsDescription?: string | null;
  payVacations: boolean;
  items: CreatePayStubItemModel[];
}

// Single line item on a create-paystub payload. Mirrors backend CreatePayStubItemModel.
export interface CreatePayStubItemModel {
  type: number;
  quantity: number;
  unitPrice: number;
  description?: string | null;
}

// Worker returned by GET /api/agency/accounting/PayStubs/WorkersReadyForPayStub.
// Mirrors backend WorkerReadyForPayStubModel.
export interface WorkerReadyForPayStubModel {
  workerId: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  secondLastName?: string;
  businessName?: string;
}

// ---------------------------------------------------------------------------
// Agency Invoice list / filters / payloads
// ---------------------------------------------------------------------------

// Filter for GET /api/agency/accounting/Invoices. Mirrors GetInvoicesFilterV2.
export interface AgencyInvoiceFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  invoiceNumber?: string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  companyFullName?: string;
  salesRepresentative?: string;
}

// Item returned inside the list response. Mirrors backend InvoiceListModel.
export interface AgencyInvoiceListItem {
  id: string;
  numberId: number;
  createdAt: string;
  agencyFullName?: string;
  companyFullName?: string;
  email?: string;
  totalNet: number;
  invoiceNumberId: number;
  weekEnding?: string | null;
  companyProfileId: string;
  salesRepresentative?: string;
  invoiceNumber: string;
}

// Response shape of GET /api/agency/accounting/Invoices.
// Mirrors backend InvoiceListModelWithTotals.
export interface AgencyInvoiceListResponse {
  detail: PaginatedList<AgencyInvoiceListItem>;
  total: number;
}

// Payload for POST /api/agency/accounting/Invoices and /Preview.
// Mirrors backend CreateInvoiceModel.
export interface CreateAgencyInvoiceModel {
  companyProfileId: string;
  invoiceDate?: string | null;
  email?: string;
  requestIds: string[];
  taxPercentage?: number | null;
  from?: string | null;
  to?: string | null;
  discounts: CreateInvoiceItemModel[];
  additionalItems: CreateInvoiceItemModel[];
  directHiring: boolean;
  clientSiteAddress?: string;
}

// Line item for discounts / additional items on a create-invoice payload.
// Mirrors backend CreateInvoiceItemModel.
export interface CreateInvoiceItemModel {
  quantity: number;
  unitPrice: number;
  description: string;
  total?: number;
}

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
  hst: number;
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

// Pay stub warning item returned by GET /api/agency/accounting/Invoices/{id}/paystubs.
// Mirrors backend PayStubDeleteWarningListModel.
export interface PayStubDeleteWarningItem {
  payStubId: string;
  payStubNumber: string;
}

// Body for DELETE /api/agency/accounting/Invoices/{id}. Mirrors DeleteInvoiceModel,
// plus the client-side `invoiceId` used to build the URL.
export interface DeleteInvoicePayload {
  invoiceId: string;
  payStubs?: string[];
}

export interface SendInvoiceEmailPayload {
  invoiceId: string;
  recipients: string[];
  subject: string;
  body: string;
  attachments: File[];
}
