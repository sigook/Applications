import { Currency, DurationTerm, EmploymentType } from './common';

export enum RequestStatus {
  Open = 'Open',
  Filled = 'Filled',
  Cancelled = 'Cancelled',
}

export enum WorkerRequestStatus {
  None = 'None',
  Applied = 'Applied',
  Declined = 'Declined',
  Rejected = 'Rejected',
  Booked = 'Booked',
  InQueue = 'InQueue',
  NotWorking = 'NotWorking',
  Working = 'Working',
}

export interface Shift {
  start: string;
  end: string;
  break: string;
}

export interface JobLocation {
  fullAddress: string;
  city: string;
  province: string;
}

export interface Request {
  id: string;
  agencyId: string;
  companyProfileId: string;
  jobTitle: string;
  description: string;
  requirements: string;
  workersQuantity: number;
  workersQuantityWorking: number;
  startAt: string;
  finishAt: string | null;
  isAsap: boolean;
  durationTerm: DurationTerm;
  employmentType: EmploymentType;
  jobLocationId: string;
  jobPositionRateId: string | null;
  workerRate: number | null;
  agencyRate: number | null;
  currency: Currency;
  shiftStart: string | null;
  shiftEnd: string | null;
  durationBreak: string | null;
  incentive: number | null;
  incentiveDescription: string;
  requestStatus: RequestStatus;
  isOpen: boolean;
  createdAt: string;
  updatedAt: string | null;
  companyName: string;
  location: string;
  assignedWorkers: WorkerRequest[];
  shift: Shift;
  jobLocation: JobLocation;
}

export interface WorkerRequest {
  id: string;
  requestId: string;
  workerProfileId: string;
  status: WorkerRequestStatus;
  startWorking: string;
  weekStartWorking: number;
  rejectComments: string;
  createdAt: string;
  firstName: string;
  lastName: string;
  timeSheets: TimeSheet[];
}

export interface TimeSheet {
  id: string;
  workerRequestId: string;
  date: string;
  clockIn: string | null;
  clockOut: string | null;
  clockInRounded: string | null;
  clockOutRounded: string | null;
  timeIn: string | null;
  timeOut: string | null;
  timeInApproved: string | null;
  timeOutApproved: string | null;
  isHoliday: boolean;
  deductionsOthers: number | null;
  bonusOrOthers: number | null;
  reimbursements: number | null;
  comment: string;
  createdAt: string;
  total: TimeSheetTotal | null;
}

export interface TimeSheetTotal {
  id: string;
  timeSheetId: string;
  totalHours: string;
  regularHours: string;
  overtimeHours: string;
  nightShiftHours: string;
  holidayHours: string;
  otherRegularHours: string;
  accumulateWeekHours: string;
}

export interface RequestFilter {
  page: number;
  pageSize: number;
  status?: RequestStatus;
  companyProfileId?: string | null;
  searchTerm?: string;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface TimeSheetFilter {
  requestId: string;
  workerId: string;
  weekEnding?: string;
  startDate?: string | null;
  endDate?: string | null;
}
