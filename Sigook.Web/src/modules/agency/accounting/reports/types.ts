// Filter used by agency accounting reports endpoints.
// Mirrors backend HoursWorkedFilter (+ the weekly-payroll `weekEnding` shortcut).
export interface AgencyReportFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  startDate?: string;
  endDate?: string;
  companyProfileId?: string;
  jobPositionRateId?: string;
  weekEnding?: string;
}

// Response shape of GET /api/agency/accounting/reports/hours-worked.
// Mirrors backend HoursWorkedResume.
export interface HoursWorkedResume {
  totalRegularHours: number;
  totalOvertimeHours: number;
  totalHolidayHours: number;
  totalNightHours: number;
  totalHours: number;
  totalPayRegular: number;
  totalPayOvertime: number;
  totalPayHoliday: number;
  totalPayNight: number;
  totalPay: number;
  detail: HoursWorkedResponseItem[];
}

export interface HoursWorkedReportView extends Partial<HoursWorkedResume> {
  rows: HoursWorkedResponseItem[];
}

// Detail row inside HoursWorkedResume.detail (mirrors backend HoursWorkedResponse).
export interface HoursWorkedResponseItem {
  workerName: string;
  jobPosition: string;
  payRate: number;
  billRate: number;
  regularHoursWorked: number;
  overtimeHoursWorked: number;
  holidayHoursWorked: number;
  nightHoursWorked: number;
  totalPayRegularRate: number;
  totalPayOvertimeRate: number;
  totalPayHolidayRate: number;
  totalPayNightRate: number;
  totalHoursWorked: number;
  totalPayRate: number;
}

// Payment report row returned by GET /api/agency/accounting/reports/payments.
// Mirrors backend WeeklyPayrollModel.
export interface WeeklyPayrollItem {
  totalNet: number;
  weekEnding: string;
  numberOfPayStubs: number;
  displayWeekEnding: string;
}

export interface WeeklyPayrollRow extends WeeklyPayrollItem {
  reportDownloading?: boolean;
}
