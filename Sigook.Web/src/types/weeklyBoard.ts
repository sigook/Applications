// Types for the Recruiting → Weekly Board feature.
// Mirror the backend models in
// Covenant.Common.Models.Request.WeeklyBoard.
import type { RequestStatus } from '@/constants/enums';
import type { RunnerStatus, RunnerType } from '@/types/runner';

// Mirrors WeeklyBoardRunnerModel — a runner sent by the recruiter to an
// order on a given work day.
export interface WeeklyBoardRunner {
  runnerId: string;
  workerProfileId: string;
  fullName: string;
  email: string;
  type: RunnerType;
  status: RunnerStatus;
  sentAt?: string | null;
}

// Mirrors WeeklyBoardAssignmentModel — a single order card placed on a
// recruiter row for a given work day.
export interface WeeklyBoardAssignment {
  recruiterId: string;
  recruiterName: string;
  requestId: string;
  numberId: number;
  companyName: string;
  jobTitle: string;
  city: string | null;
  provinceCode: string | null;
  workDate: string;
  status: RequestStatus;
  isAsap: boolean;
  workerSalary: number | null;
  usesRunners: boolean;
  runnersSent: number;
  runners: WeeklyBoardRunner[];
}

// Mirrors WeeklyBoardRecruiterRowModel — one recruiter row with its cards.
export interface WeeklyBoardRecruiterRow {
  recruiterId: string;
  recruiterName: string;
  ordersCount: number;
  workersSent: number;
  assignments: WeeklyBoardAssignment[];
}

// Mirrors WeeklyBoardModel — the whole board for the requested range.
export interface WeeklyBoard {
  weekStart: string;
  weekEnd: string;
  totalAssignments: number;
  totalWorkersSent: number;
  recruiters: WeeklyBoardRecruiterRow[];
}

// Mirrors RecruiterWeeklyBoardModel — the current recruiter's own board with
// the workers sent to each order.
export interface RecruiterWeeklyBoard {
  recruiterName: string;
  weekStart: string;
  weekEnd: string;
  ordersCount: number;
  workersSent: number;
  assignments: WeeklyBoardAssignment[];
}

// Query for GET — the date range is computed in the front-end.
export interface WeeklyBoardFilter {
  from: string;
  to: string;
}

// A single day rendered in the board's date strip / day picker.
export interface WeekDay {
  date: string;
  weekday: string;
  monthShort: string;
  dayNum: number;
  isToday?: boolean;
}

// Pre-selected order/day when opening the assign-recruiter modal from a cell.
export interface AssignPreset {
  recruiterId?: string;
  workDate?: string;
}

// Body for POST — assign one or more recruiters to an order on one or
// more work days. Mirrors AssignRecruitersModel.
export interface AssignRecruitersPayload {
  requestId: string;
  workDates: string[];
  recruiterIds: string[];
}

// Query for DELETE — remove a single recruiter assignment for a work day.
export interface UnassignRecruiterPayload {
  requestId: string;
  recruiterId: string;
  workDate: string;
}

// Body for POST — move an assignment (with its runners) to another
// recruiter and/or work day. Mirrors MoveAssignmentModel.
export interface MoveAssignmentPayload {
  requestId: string;
  fromRecruiterId: string;
  fromWorkDate: string;
  toRecruiterId: string;
  toWorkDate: string;
}

// Body for POST — the current recruiter sends a runner to an order on a work
// day. Mirrors AddRunnerModel.
export interface AddRunnerPayload {
  requestId: string;
  workDate: string;
  workerProfileId: string;
  type: RunnerType;
}

// Deleting a runner goes through agencyRunnerApi.deleteAgencyRunner — the board
// and the order's Runners tab share the same endpoint.
