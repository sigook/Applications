import type { PaginatedList } from '@/shared/types/common';

// Mirrors Covenant.Common.Enums.InvoiceStatus (serialized as numbers).
export enum InvoiceStatus {
  Pending = 1,
  Paid = 2,
}

export const INVOICE_STATUS_LABELS: Record<InvoiceStatus, string> = {
  [InvoiceStatus.Pending]: 'Pending',
  [InvoiceStatus.Paid]: 'Paid',
};

export const INVOICE_STATUSES: InvoiceStatus[] = [InvoiceStatus.Pending, InvoiceStatus.Paid];

export function invoiceStatusTagType(status: InvoiceStatus): string {
  switch (status) {
    case InvoiceStatus.Paid: return 'is-success';
    case InvoiceStatus.Pending:
    default: return 'is-warning';
  }
}

// Body for PUT /api/agency/accounting/invoices/{id}/status. Mirrors ChangeInvoiceStatusModel.
export interface ChangeInvoiceStatusModel {
  status: InvoiceStatus;
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

export interface InvoiceFilter {
  page: number;
  pageSize: number;
  companyProfileId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

// Filter for GET /api/agency/accounting/invoices. Mirrors GetInvoicesFilterV2.
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
  status?: InvoiceStatus | null;
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
  status: InvoiceStatus;
  updatedAt?: string | null;
  updatedByName?: string | null;
}

// Response shape of GET /api/agency/accounting/invoices.
// Mirrors backend InvoiceListModelWithTotals.
export interface AgencyInvoiceListResponse {
  detail: PaginatedList<AgencyInvoiceListItem>;
  total: number;
}

// Payload for POST /api/agency/accounting/invoices and /Preview.
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

// Pay stub warning item returned by GET /api/agency/accounting/invoices/{id}/paystubs.
// Mirrors backend PayStubDeleteWarningListModel.
export interface PayStubDeleteWarningItem {
  payStubId: string;
  payStubNumber: string;
}

// Body for DELETE /api/agency/accounting/invoices/{id}. Mirrors DeleteInvoiceModel,
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
