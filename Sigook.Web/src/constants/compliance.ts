import type { RequestComplianceItem } from '@/types/agency';
import { ComplianceDocumentTarget } from '@/types/requestApplicant';

export const DEFAULT_COMPLIANCE_ITEMS: readonly RequestComplianceItem[] = [
  { name: 'ID', isMandatory: true, documentTarget: ComplianceDocumentTarget.Identification1 },
  { name: 'SIN/SSN', isMandatory: true, documentTarget: ComplianceDocumentTarget.SocialInsurance },
  { name: 'WP', isMandatory: false, documentTarget: ComplianceDocumentTarget.OtherDocument },
  { name: 'W4', isMandatory: true, documentTarget: ComplianceDocumentTarget.OtherDocument },
  { name: 'I9', isMandatory: true, documentTarget: ComplianceDocumentTarget.OtherDocument },
  { name: 'Banking', isMandatory: true, documentTarget: ComplianceDocumentTarget.OtherDocument },
];

export function buildDefaultComplianceItems(): RequestComplianceItem[] {
  return DEFAULT_COMPLIANCE_ITEMS.map((item) => ({ ...item }));
}
