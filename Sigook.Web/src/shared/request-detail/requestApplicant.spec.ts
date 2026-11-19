import { describe, expect, it } from 'vitest';
import {
  COMPLIANCE_DOCUMENT_TARGET_LABELS,
  COMPLIANCE_DOCUMENT_TARGETS,
  ComplianceDocumentTarget,
  REQUEST_APPLICANT_STATUS_LABELS,
  REQUEST_APPLICANT_STATUSES,
  RequestApplicantStatus,
  requestApplicantStatusLabel,
  requestApplicantStatusTagType,
} from '@/shared/request-detail/requestApplicant';

describe('RequestApplicantStatus', () => {
  it('mirrors the backend numeric values', () => {
    expect(RequestApplicantStatus.Pending).toBe(1);
    expect(RequestApplicantStatus.InProgress).toBe(2);
    expect(RequestApplicantStatus.Confirmed).toBe(3);
    expect(RequestApplicantStatus.Cancelled).toBe(4);
  });

  it('lists every status in workflow order with a label', () => {
    expect(REQUEST_APPLICANT_STATUSES).toEqual([1, 2, 3, 4]);
    expect(REQUEST_APPLICANT_STATUSES.every((status) => REQUEST_APPLICANT_STATUS_LABELS[status])).toBe(true);
  });

  it.each([
    [RequestApplicantStatus.Pending, 'Pending', 'is-warning'],
    [RequestApplicantStatus.InProgress, 'In progress', 'is-info'],
    [RequestApplicantStatus.Confirmed, 'Confirmed', 'is-success'],
    [RequestApplicantStatus.Cancelled, 'Cancelled', 'is-danger'],
  ])('status %d -> %s / %s', (status, label, tag) => {
    expect(requestApplicantStatusLabel(status)).toBe(label);
    expect(requestApplicantStatusTagType(status)).toBe(tag);
  });
});

describe('ComplianceDocumentTarget', () => {
  it('mirrors the backend numeric values', () => {
    expect(COMPLIANCE_DOCUMENT_TARGETS).toEqual([0, 1, 2, 3, 4, 5, 6]);
    expect(ComplianceDocumentTarget.SocialInsurance).toBe(3);
    expect(ComplianceDocumentTarget.OtherDocument).toBe(6);
  });

  it('labels every target', () => {
    expect(COMPLIANCE_DOCUMENT_TARGETS.map((target) => COMPLIANCE_DOCUMENT_TARGET_LABELS[target])).toEqual([
      'No document',
      'Identification 1',
      'Identification 2',
      'Social insurance',
      'Resume',
      'Police check',
      'Other documents',
    ]);
  });
});
