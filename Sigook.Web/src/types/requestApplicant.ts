// Request applicant types. Mirror backend Covenant.Common enums and models.
// The API (System.Text.Json, no JsonStringEnumConverter) serializes enums as
// their NUMERIC value, so these must match the backend int values exactly.

export enum RequestApplicantStatus {
  Pending = 1,
  InProgress = 2,
  Confirmed = 3,
  Cancelled = 4,
}

export enum ComplianceDocumentTarget {
  None = 0,
  Identification1 = 1,
  Identification2 = 2,
  SocialInsurance = 3,
  Resume = 4,
  PoliceCheck = 5,
  OtherDocument = 6,
}

export const REQUEST_APPLICANT_STATUS_LABELS: Record<RequestApplicantStatus, string> = {
  [RequestApplicantStatus.Pending]: 'Pending',
  [RequestApplicantStatus.InProgress]: 'In progress',
  [RequestApplicantStatus.Confirmed]: 'Confirmed',
  [RequestApplicantStatus.Cancelled]: 'Cancelled',
};

export const REQUEST_APPLICANT_STATUSES: RequestApplicantStatus[] = [
  RequestApplicantStatus.Pending,
  RequestApplicantStatus.InProgress,
  RequestApplicantStatus.Confirmed,
  RequestApplicantStatus.Cancelled,
];

export function requestApplicantStatusLabel(status: RequestApplicantStatus): string {
  return REQUEST_APPLICANT_STATUS_LABELS[status];
}

// Buefy tag type used to colour the status chip in the applicants list.
export function requestApplicantStatusTagType(status: RequestApplicantStatus): string {
  switch (status) {
    case RequestApplicantStatus.Pending:
      return 'is-warning';
    case RequestApplicantStatus.InProgress:
      return 'is-info';
    case RequestApplicantStatus.Confirmed:
      return 'is-success';
    case RequestApplicantStatus.Cancelled:
      return 'is-danger';
  }
}

export const COMPLIANCE_DOCUMENT_TARGET_LABELS: Record<ComplianceDocumentTarget, string> = {
  [ComplianceDocumentTarget.None]: 'No document',
  [ComplianceDocumentTarget.Identification1]: 'Identification 1',
  [ComplianceDocumentTarget.Identification2]: 'Identification 2',
  [ComplianceDocumentTarget.SocialInsurance]: 'Social insurance',
  [ComplianceDocumentTarget.Resume]: 'Resume',
  [ComplianceDocumentTarget.PoliceCheck]: 'Police check',
  [ComplianceDocumentTarget.OtherDocument]: 'Other documents',
};

export const COMPLIANCE_DOCUMENT_TARGETS: ComplianceDocumentTarget[] = [
  ComplianceDocumentTarget.None,
  ComplianceDocumentTarget.Identification1,
  ComplianceDocumentTarget.Identification2,
  ComplianceDocumentTarget.SocialInsurance,
  ComplianceDocumentTarget.Resume,
  ComplianceDocumentTarget.PoliceCheck,
  ComplianceDocumentTarget.OtherDocument,
];
