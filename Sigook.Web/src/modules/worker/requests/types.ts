import { LocationDetailModel } from '@/shared/types/common';

// Worker Request types
export interface WorkerRequestFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
}

export interface WorkerRequestApplyModel {
  comments?: string;
}

export interface WorkerRegisterTimeModel {
  latitude: number;
  longitude: number;
}

export interface WorkerRequestListItem {
  id: string;
  numberId?: number;
  jobTitle?: string;
  agencyFullName?: string;
  location?: string;
  entrance?: string;
  workersQuantity?: number;
  status?: string;
  isAsap?: boolean;
  workerApprovedToWork?: string;
  workerRate?: number | null;
  workerSalary?: number | null;
  createdAt?: string;
  startAt?: string;
  finishAt?: string | null;
  durationTerm?: string;
}

export interface WorkerRequestDetail extends WorkerRequestListItem {
  agencyLogo?: string;
  description?: string;
  requirements?: string;
  responsibilities?: string;
  workerRate?: number | null;
  workerSalary?: number | null;
  displayShift?: string;
  skills?: string[];
  jobPosition?: string;
  holidayIsPaid?: boolean;
  breakIsPaid?: boolean;
  durationBreak?: string;
  incentive?: number | null;
  incentiveDescription?: string;
  requestStatus?: string;
  jobLocation?: LocationDetailModel | null;
  isApplicant?: boolean;
  punchCardOptionEnabled?: boolean;
}

// Matches API TimeSheetListModel.
export interface WorkerTimeSheetItem {
  id: string;
  day: string;
  clockIn?: string | null;
  clockOut?: string | null;
  timeIn: string;
  timeOut?: string | null;
  totalHours: number;
  totalHoursApproved: number;
  wasApproved: boolean;
  canUpdate: boolean;
}
