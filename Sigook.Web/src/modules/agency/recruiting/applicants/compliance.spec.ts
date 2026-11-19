import { describe, expect, it } from 'vitest';
import { buildDefaultComplianceItems, DEFAULT_COMPLIANCE_ITEMS } from '@/modules/agency/recruiting/applicants/compliance';
import { ComplianceDocumentTarget } from '@/shared/request-detail/requestApplicant';

describe('default compliance items', () => {
  it('requires every item except the work permit', () => {
    expect(DEFAULT_COMPLIANCE_ITEMS.map((item) => [item.name, item.isMandatory])).toEqual([
      ['ID', true],
      ['SIN/SSN', true],
      ['WP', false],
      ['W4', true],
      ['I9', true],
      ['Banking', true],
    ]);
  });

  it('links ID and SIN to their documents and the rest to other documents', () => {
    expect(DEFAULT_COMPLIANCE_ITEMS.map((item) => item.documentTarget)).toEqual([
      ComplianceDocumentTarget.Identification1,
      ComplianceDocumentTarget.SocialInsurance,
      ComplianceDocumentTarget.OtherDocument,
      ComplianceDocumentTarget.OtherDocument,
      ComplianceDocumentTarget.OtherDocument,
      ComplianceDocumentTarget.OtherDocument,
    ]);
  });

  it('has no ids because they are new items', () => {
    expect(DEFAULT_COMPLIANCE_ITEMS.every((item) => item.id === undefined)).toBe(true);
  });
});

describe('buildDefaultComplianceItems', () => {
  it('returns equal but independent copies', () => {
    const items = buildDefaultComplianceItems();

    expect(items).toEqual(DEFAULT_COMPLIANCE_ITEMS);
    expect(items).not.toBe(DEFAULT_COMPLIANCE_ITEMS);
    items.forEach((item, index) => expect(item).not.toBe(DEFAULT_COMPLIANCE_ITEMS[index]));
  });

  it('does not leak edits into the defaults or later builds', () => {
    const items = buildDefaultComplianceItems();
    items[2].isMandatory = true;
    items[0].name = 'Changed';
    items.push({ name: 'Extra', isMandatory: false, documentTarget: ComplianceDocumentTarget.Resume });

    const fresh = buildDefaultComplianceItems();
    expect(fresh).toHaveLength(6);
    expect(fresh[0].name).toBe('ID');
    expect(fresh[2].isMandatory).toBe(false);
    expect(DEFAULT_COMPLIANCE_ITEMS[2].isMandatory).toBe(false);
  });
});
