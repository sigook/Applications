// Runner types. Mirror backend Covenant.Common.Models.Runner.
// The API (System.Text.Json, no JsonStringEnumConverter) serializes enums as
// their NUMERIC value, so these must match the backend int values exactly.

export enum RunnerType {
  Active = 1,
  Passive = 2,
}

export enum RunnerStatus {
  SentToClient = 1,
  InterviewScheduled = 2,
  InterviewRescheduled = 3,
  NoLongerAvailable = 4,
  NoShow = 5,
  WaitingForInterviewFeedback = 6,
  WaitingForFinalDecision = 7,
  Rejected = 8,
  InOnboardingProcess = 9,
  Hired = 10,
}

export enum InterviewType {
  Phone = 1,
  Video = 2,
  Onsite = 3,
}

export enum InterviewStatus {
  Scheduled = 1,
  Rescheduled = 2,
}

// GetRunnersSortBy on the backend: Name=0, Status=1, Type=2, CreatedAt=3.
export enum RunnerSortBy {
  Name = 0,
  Status = 1,
  Type = 2,
  CreatedAt = 3,
}

export const RUNNER_STATUS_LABELS: Record<RunnerStatus, string> = {
  [RunnerStatus.SentToClient]: 'Sent to Client',
  [RunnerStatus.InterviewScheduled]: 'Interview scheduled',
  [RunnerStatus.InterviewRescheduled]: 'Interview rescheduled',
  [RunnerStatus.NoLongerAvailable]: 'No longer available',
  [RunnerStatus.NoShow]: 'No show',
  [RunnerStatus.WaitingForInterviewFeedback]: 'Waiting for interview feedback',
  [RunnerStatus.WaitingForFinalDecision]: 'Waiting for final decision',
  [RunnerStatus.Rejected]: 'Rejected',
  [RunnerStatus.InOnboardingProcess]: 'In onboarding process',
  [RunnerStatus.Hired]: 'Hired',
};

export const RUNNER_STATUSES: RunnerStatus[] = [
  RunnerStatus.SentToClient,
  RunnerStatus.InterviewScheduled,
  RunnerStatus.InterviewRescheduled,
  RunnerStatus.NoLongerAvailable,
  RunnerStatus.NoShow,
  RunnerStatus.WaitingForInterviewFeedback,
  RunnerStatus.WaitingForFinalDecision,
  RunnerStatus.Rejected,
  RunnerStatus.InOnboardingProcess,
  RunnerStatus.Hired,
];

export const RUNNER_TYPE_LABELS: Record<RunnerType, string> = {
  [RunnerType.Active]: 'Active',
  [RunnerType.Passive]: 'Passive',
};

export const RUNNER_TYPES: RunnerType[] = [RunnerType.Active, RunnerType.Passive];

export const INTERVIEW_TYPE_LABELS: Record<InterviewType, string> = {
  [InterviewType.Phone]: 'Phone',
  [InterviewType.Video]: 'Video',
  [InterviewType.Onsite]: 'Onsite',
};

export const INTERVIEW_TYPES: InterviewType[] = [InterviewType.Phone, InterviewType.Video, InterviewType.Onsite];

export const INTERVIEW_STATUS_LABELS: Record<InterviewStatus, string> = {
  [InterviewStatus.Scheduled]: 'Scheduled',
  [InterviewStatus.Rescheduled]: 'Rescheduled',
};

// Mirror of the backend guard Runner.CanAddInterview: interviews can only be
// added when the runner is in 'Interview scheduled' or 'Interview rescheduled'.
export function canAddInterview(status: RunnerStatus): boolean {
  return status === RunnerStatus.InterviewScheduled || status === RunnerStatus.InterviewRescheduled;
}

export interface RunnerStatusHistoryItem {
  id: string;
  previousStatus?: RunnerStatus | null;
  newStatus: RunnerStatus;
  changedBy?: string;
  changedByEmail?: string;
  changedAt: string;
  comments?: string;
}

export interface RunnerInterview {
  id: string;
  scheduledDate: string;
  type: InterviewType;
  interviewer?: string;
  status: InterviewStatus;
  feedback?: string;
  notes?: string;
  rescheduleCount: number;
  createdAt: string;
  createdBy?: string;
  rescheduledAt?: string | null;
}

// Mirrors backend RunnerListModel.
export interface RunnerListItem {
  id: string;
  numberId: number;
  agencyId: string;
  requestId: string;
  workerProfileId?: string | null;
  candidateId?: string | null;
  name: string;
  email?: string;
  type: RunnerType;
  status: RunnerStatus;
  startDate?: string | null;
  interviewsCount: number;
  createdAt: string;
  createdBy?: string;
}

// Mirrors backend RunnerDetailModel.
export interface RunnerDetail {
  id: string;
  numberId: number;
  requestId: string;
  workerProfileId?: string | null;
  candidateId?: string | null;
  name: string;
  email?: string;
  type: RunnerType;
  status: RunnerStatus;
  startDate?: string | null;
  createdAt: string;
  createdBy?: string;
  updatedAt?: string | null;
  statusHistory: RunnerStatusHistoryItem[];
  interviews: RunnerInterview[];
}

// Filter for GET .../Runners. Mirrors backend GetRunnersFilter.
export interface AgencyRunnerFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: RunnerSortBy;
  requestId?: string;
  name?: string;
  type?: RunnerType | null;
  statuses?: RunnerStatus[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
}

// Body for POST .../Runners. Mirrors backend RunnerCreateModel.
export interface CreateRunnerModel {
  workerProfileId?: string | null;
  candidateId?: string | null;
  type: RunnerType;
}

// Body for PUT .../Runners/{id}/Status. Mirrors backend ChangeRunnerStatusModel.
// startDate is only used when status === RunnerStatus.Hired.
export interface ChangeRunnerStatusModel {
  status: RunnerStatus;
  comments?: string;
  startDate?: string | null;
}

// Body for POST .../Runners/{id}/Interview. Mirrors backend RunnerInterviewCreateModel.
export interface CreateRunnerInterviewModel {
  scheduledDate: string;
  type: InterviewType;
  interviewer?: string;
  notes?: string;
}

// Body for PUT .../Runners/{id}/Interview/{interviewId}/Reschedule.
export interface RescheduleRunnerInterviewModel {
  newDate: string;
}

// Mirrors backend RunnerStartingTodayModel. Hired runners (non direct-hiring)
// whose start date is within the first follow-up days, surfaced to the recruiter
// who hired them so they can confirm attendance on the punch card.
export interface RunnerStartingToday {
  runnerId: string;
  requestId: string;
  requestNumberId: number;
  jobTitle: string;
  companyName: string;
  workerProfileId: string;
  workerName: string;
  startDate: string;
  dayNumber: number;
}
