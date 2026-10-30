import { LocationDetailModel } from '@/shared/types/common';
import type { ClockType } from '@/shared/constants/enums';

// Worker Request types
export enum WorkerRequestSortBy {
  NumberId = 0,
  Rate = 1,
}

export interface WorkerRequestSortOption {
  key: string;
  label: string;
  sortBy: WorkerRequestSortBy;
  isDescending: boolean;
}

export const WorkerRequestSortOptions: WorkerRequestSortOption[] = [
  { key: 'newest', label: 'Date: Newest first', sortBy: WorkerRequestSortBy.NumberId, isDescending: true },
  { key: 'oldest', label: 'Date: Oldest first', sortBy: WorkerRequestSortBy.NumberId, isDescending: false },
  { key: 'highestRate', label: 'Pay rate: Highest', sortBy: WorkerRequestSortBy.Rate, isDescending: true },
  { key: 'lowestRate', label: 'Pay rate: Lowest', sortBy: WorkerRequestSortBy.Rate, isDescending: false },
];

export interface WorkerRequestFilter {
  pageIndex?: number;
  pageSize?: number;
  sortBy?: WorkerRequestSortBy;
  isDescending?: boolean;
  numberId?: number;
  jobTitle?: string;
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

export interface PunchCheckpoint {
  latitude: number;
  longitude: number;
  radiusMeters: number;
}

export interface WorkerRequestDetail extends WorkerRequestListItem {
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
  punchCheckpoint?: PunchCheckpoint | null;
  isApplicant?: boolean;
  punchCardOptionEnabled?: boolean;
}

// Matches API PunchStatusModel.
export interface PunchStatus {
  clockType: ClockType;
  clockIn?: string | null;
  clockOut?: string | null;
  dayHours: number;
  weekHours: number;
}
