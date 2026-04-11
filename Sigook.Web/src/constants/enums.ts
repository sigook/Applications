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
}

export const RequestStatusLabels: Record<RequestStatus, string> = {
  [RequestStatus.Open]: 'Open',
  [RequestStatus.Filled]: 'Filled',
  [RequestStatus.Cancelled]: 'Cancelled',
};

export enum WorkerRequestStatus {
  Rejected = 2,
  Booked = 3,
}

export const WorkerRequestStatusLabels: Record<WorkerRequestStatus, string> = {
  [WorkerRequestStatus.Rejected]: 'Rejected',
  [WorkerRequestStatus.Booked]: 'Booked',
};

export enum WorkerStatus {
  NotWorking = 0,
  Working = 1,
}
