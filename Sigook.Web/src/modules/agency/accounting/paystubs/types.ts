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

export interface PayStubFilter {
  weekEnding?: string;
  workerProfileId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

// Filter for GET /api/agency/accounting/paystubs. Mirrors backend GetPayStubsFilter.
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

// Item returned by GET /api/agency/accounting/paystubs. Mirrors backend PayStubListModel.
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

// Payload for POST /api/agency/accounting/paystubs/skip-payroll-number.
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

export interface PayrollSubContractorRow extends PayrollSubContractorListItem {
  reportDownloading?: boolean;
}

// Payload for POST /api/agency/accounting/paystubs. Mirrors backend CreatePayStubModel.
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

// Worker returned by GET /api/agency/accounting/paystubs/WorkersReadyForPayStub.
// Mirrors backend WorkerReadyForPayStubModel.
export interface WorkerReadyForPayStubModel {
  workerId: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  secondLastName?: string;
  businessName?: string;
}

export interface AgencyPayStubRow extends AgencyPayStubListItem {
  emailSending?: boolean;
  emailSent?: boolean;
}
