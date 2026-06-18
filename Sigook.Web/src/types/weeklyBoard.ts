// Types for the Recruiting → Weekly Board feature.
// Mirror the backend models in
// Covenant.Common.Models.Request.WeeklyBoard.
import type { RequestStatus } from '@/constants/enums';

// Mirrors WeeklyBoardDispatchModel — a worker sent by the recruiter to an
// order on a given work day.
export interface WeeklyBoardDispatch {
  workerProfileId: string;
  fullName: string;
  email: string;
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
  workDate: string;
  status: RequestStatus;
  isAsap: boolean;
  workerSalary: number | null;
  workersSent: number;
  dispatches: WeeklyBoardDispatch[];
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

// Body for POST — move an assignment (with its dispatched workers) to another
// recruiter and/or work day. Mirrors MoveAssignmentModel.
export interface MoveAssignmentPayload {
  requestId: string;
  fromRecruiterId: string;
  fromWorkDate: string;
  toRecruiterId: string;
  toWorkDate: string;
}

// Body for POST — the current recruiter sends one or more workers to an order
// on a work day. Mirrors DispatchWorkersModel.
export interface DispatchWorkersPayload {
  requestId: string;
  workDate: string;
  workerProfileIds: string[];
}

// Query for DELETE — remove a single worker the recruiter sent to an order.
export interface RemoveWorkerPayload {
  requestId: string;
  workDate: string;
  workerProfileId: string;
}
