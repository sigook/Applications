import type { PaginatedList } from '@/shared/types/common';

// Mirrors Covenant.Common.Enums.InvoiceStatus (serialized as numbers).
export enum InvoiceStatus {
  Pending = 1,
  Paid = 2,
  Due = 3,
}

export const INVOICE_STATUS_LABELS: Record<InvoiceStatus, string> = {
  [InvoiceStatus.Pending]: 'Pending',
  [InvoiceStatus.Paid]: 'Paid',
  [InvoiceStatus.Due]: 'Due',
};

export const INVOICE_STATUSES: InvoiceStatus[] = [InvoiceStatus.Pending, InvoiceStatus.Due, InvoiceStatus.Paid];

export function invoiceStatusTagType(status: InvoiceStatus): string {
  switch (status) {
    case InvoiceStatus.Paid: return 'is-success';
    case InvoiceStatus.Due: return 'is-danger';
    case InvoiceStatus.Pending:
    default: return 'is-warning';
  }
}

// Body for PUT /api/agency/accounting/invoices/{id}/status. Mirrors ChangeInvoiceStatusModel.
// paidAt (ISO instant, local midnight of the picked day) only applies when status is Paid; the API defaults it to now.
export interface ChangeInvoiceStatusModel {
  status: InvoiceStatus;
  paidAt?: string | null;
}

// Body for PUT /api/agency/accounting/invoices/{id}/paid-at. Mirrors UpdateInvoicePaidAtModel.
export interface UpdateInvoicePaidAtModel {
  paidAt: string;
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
  dueDateFrom?: string | null;
  dueDateTo?: string | null;
  paidAtFrom?: string | null;
  paidAtTo?: string | null;
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
  subTotal: number;
  tax: number;
  totalNet: number;
  invoiceNumberId: number;
  weekEnding?: string | null;
  companyProfileId: string;
  salesRepresentative?: string;
  invoiceNumber: string;
  status: InvoiceStatus;
  paymentTermDays: number;
  dueDate: string;
  dueDays: number;
  paidAt?: string | null;
  updatedAt?: string | null;
  updatedByName?: string | null;
  pdfUrl: string | null;
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
