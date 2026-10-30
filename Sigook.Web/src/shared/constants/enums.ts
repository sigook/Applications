// Numeric enums aligned with the backend (Covenant.Common.Enums).
// Use these for agency/company endpoints — payload values sent to the API
// and values received from agency/company-facing response models.
//
// NOTE: worker-facing endpoints (WorkerRequestListModel / WorkerRequestDetailModel)
// still return these fields as PascalCase strings for SigookApp compatibility.
// For those, continue using the string globals declared in src/varaibles.ts.

export enum EmploymentType {
  FullTime = 1,
  PartTime = 2,
  Contractor = 3,
  Temporary = 4,
}

export const EmploymentTypeLabels: Record<EmploymentType, string> = {
  [EmploymentType.FullTime]: 'Full Time',
  [EmploymentType.PartTime]: 'Part Time',
  [EmploymentType.Contractor]: 'Contractor',
  [EmploymentType.Temporary]: 'Temporary',
};

export enum DurationTerm {
  LongTerm = 0,
  ShortTerm = 1,
}

export const DurationTermLabels: Record<DurationTerm, string> = {
  [DurationTerm.LongTerm]: 'Long Term',
  [DurationTerm.ShortTerm]: 'Short Term',
};

export enum RequestStatus {
  Open = 1,
  Filled = 3,
  Cancelled = 4,
  PendingReview = 5,
}

export const RequestStatusLabels: Record<RequestStatus, string> = {
  [RequestStatus.Open]: 'Open',
  [RequestStatus.Filled]: 'Filled',
  [RequestStatus.Cancelled]: 'Cancelled',
  [RequestStatus.PendingReview]: 'Pending Review',
};

export enum CompanyStatus {
  Lead = 1,
  Potential = 2,
  Prospect = 3,
  Quoted = 4,
  Client = 5,
  Blocked = 6,
  Inactive = 7,
}

export enum WorkerRequestStatus {
  Rejected = 2,
  Booked = 3,
  Finished = 4,
}

export const WorkerRequestStatusLabels: Record<WorkerRequestStatus, string> = {
  [WorkerRequestStatus.Rejected]: 'Rejected',
  [WorkerRequestStatus.Booked]: 'Booked',
  [WorkerRequestStatus.Finished]: 'Finished',
};

export const WorkerRequestStatusTagTypes: Record<WorkerRequestStatus, string> = {
  [WorkerRequestStatus.Rejected]: 'is-danger',
  [WorkerRequestStatus.Booked]: 'is-success',
  [WorkerRequestStatus.Finished]: 'is-info',
};

export const WorkerRequestStatusOptions = [
  WorkerRequestStatus.Booked,
  WorkerRequestStatus.Finished,
  WorkerRequestStatus.Rejected,
].map(id => ({ id, value: WorkerRequestStatusLabels[id] }));

export enum WorkerRequestFinishReason {
  ContractCompleted = 1,
  ClientReducedStaff = 2,
  WorkerResigned = 3,
  Other = 4,
}

export const WorkerRequestFinishReasonLabels: Record<WorkerRequestFinishReason, string> = {
  [WorkerRequestFinishReason.ContractCompleted]: 'Contract completed',
  [WorkerRequestFinishReason.ClientReducedStaff]: 'Client reduced staff',
  [WorkerRequestFinishReason.WorkerResigned]: 'Worker resigned',
  [WorkerRequestFinishReason.Other]: 'Other',
};

export enum WorkerStatus {
  NotWorking = 0,
  Working = 1,
}

export enum ClockType {
  None = 0,
  ClockIn = 1,
  ClockOut = 2,
}
