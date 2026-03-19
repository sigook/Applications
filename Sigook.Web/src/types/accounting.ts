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
