import type { LocationDetailModel } from '@/shared/types/common';
import type { CreateAgencyRequestModel, RequestShiftModel } from '@/shared/request-detail/types';
import { DurationTerm, EmploymentType, RequestStatus } from '@/shared/constants/enums';

// Company Request models
export interface CompanyRequestFilter {
  sortBy: number;
  isDescending: boolean;
  pageIndex: number;
  pageSize: number;
  numberId?: number;
  jobTitle?: string;
  location?: string;
  companyUserId?: string;
}

export interface CompanyRequestListItem {
  id: string;
  numberId: number;
  jobTitle: string;
  location: string;
  entrance: string;
  displayShift: string;
  workersQuantity: number;
  workersQuantityWorking: number;
  requestStatus: RequestStatus;
  isAsap: boolean;
  isDirectHiring: boolean;
  createdAt: string;
}

export interface CompanyRequestDetail {
  id: string;
  numberId: number;
  jobTitle: string;
  workersQuantity: number;
  workersQuantityWorking: number;
  description?: string;
  requirements?: string;
  responsibilities?: string;
  durationBreak: string;
  breakIsPaid: boolean;
  holidayIsPaid: boolean;
  incentive?: number | null;
  incentiveDescription?: string;
  isAsap: boolean;
  jobIsOnBranchOffice: boolean;
  jobLocation?: LocationDetailModel | null;
  jobPositionRate?: { id: string; value: string } | null;
  agencyRate?: number | null;
  status: RequestStatus;
  durationTerm: number;
  displayShift?: string;
  createdAt: string;
  startAt?: string | null;
  finishAt?: string | null;
  workerSalary?: number | null;
}

export type CreateCompanyRequestModel = Pick<CreateAgencyRequestModel,
  'jobTitle' | 'workersQuantity' | 'workerSalary' | 'description' | 'requirements' | 'responsibilities' |
  'incentive' | 'incentiveDescription' | 'startAt' | 'finishAt' | 'durationBreak' | 'breakIsPaid' | 'isAsap' |
  'durationTerm' | 'employmentType' | 'punchCardOptionEnabled' | 'shift' | 'jobPositionRateId' | 'locationId'>;

export interface CompanyRequestFormSettings {
  durationBreak: Date;
  durationTerm: DurationTerm;
  employmentType: EmploymentType;
  punchCardOptionEnabled: boolean;
  isAsap?: boolean;
  breakIsPaid?: boolean;
  responsibilities?: string;
  finishAt?: Date | null;
  shift?: RequestShiftModel | null;
  rate?: number | null;
  jobPositionRateId?: string | null;
  locationId?: string | null;
}

// Company Request Worker models
export interface CompanyRequestWorkerFilter {
  sortBy: number;
  requestId: string;
  pageIndex: number;
  pageSize: number;
  isDescending?: boolean;
  numberId?: number;
  name?: string;
  statuses?: number[];
  startWorkingFrom?: Date;
  startWorkingTo?: Date;
}

export interface CompanyRequestWorker {
  numberId: number;
  requestId: string;
  id: string;
  workerId: string;
  workerProfileId: string;
  name: string;
  workerRequestStatus: number;
  status?: string;
  profileImage: string;
  isSubcontractor: boolean;
  totalHoursApproved: number;
  totalHoursWorker: number;
  startWorking: string | null;
  rejectedAt: string | null;
}

export interface ClockInModel {
  clockIn: string;
}

export interface ClockInResult {
  timeSheetId: string;
  workerFullName: string;
  finish: boolean;
}

// Reject / Request new worker
export interface CommentsModel {
  comments: string;
}
