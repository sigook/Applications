import type { WorkerRequestListItem } from '@/modules/worker/requests/types';

interface PageFilter {
  pageIndex?: number;
  pageSize?: number;
}

export type WorkerAssignmentStatus = 'Active' | 'Completed' | 'Ended';

export type WorkerHistoryStatusFilter = Exclude<WorkerAssignmentStatus, 'Active'> | 'Past';

export interface WorkerHistoryFilter extends PageFilter {
  status?: WorkerAssignmentStatus | WorkerHistoryStatusFilter;
}

export interface WorkerAssignmentListItem extends WorkerRequestListItem {
  clientName?: string;
  startWorking?: string | null;
  endedAt?: string | null;
  hoursWorked: number;
}

export interface WorkerPayStubFilter extends PageFilter {
  year?: number;
}

export interface WorkerPayStubListItem {
  id: string;
  payStubNumber: string;
  weekEnding: string;
  paymentDate: string;
  hours: number;
  totalEarnings: number;
  totalDeductions: number;
  totalPaid: number;
  companies: string[];
  pdfUrl: string | null;
}

export interface WorkerPayStubYtd {
  earnings: number;
  totalDeductions: number;
  totalPaid: number;
}

export enum WorkerTimeSheetStatus {
  Open = 1,
  Pending = 2,
  Approved = 3,
  Paid = 4,
}

export interface WorkerWeekTimeSheet {
  id: string;
  requestId: string;
  date: string;
  clientName: string;
  jobTitle: string;
  clockIn?: string | null;
  clockOut?: string | null;
  durationBreak: string;
  breakIsPaid: boolean;
  regularHours: number;
  overtimeHours: number;
  holidayHours: number;
  totalHours: number;
  status: WorkerTimeSheetStatus;
  missingHours: string;
  missingHoursOvertime: string;
  bonusOrOthers: number;
  bonusOrOthersDescription?: string | null;
  deductionsOthers: number;
  deductionsOthersDescription?: string | null;
  reimbursements: number;
  reimbursementsDescription?: string | null;
}

export interface WorkerWeekHours {
  total: number;
  regular: number;
  overtime: number;
  holiday: number;
  pendingApproval: number;
}

export interface WorkerWeekTimeSheets {
  weekStart: string;
  weekEnd: string;
  hours: WorkerWeekHours;
  items: WorkerWeekTimeSheet[];
}

export interface WorkerActivitySummary {
  lastPayStub: WorkerPayStubListItem | null;
  weekHours: WorkerWeekHours;
}
