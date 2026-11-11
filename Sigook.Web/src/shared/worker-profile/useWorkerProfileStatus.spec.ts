import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ref } from 'vue';
import {
  expiryStatus,
  isWorkerProfileSectionId,
  useWorkerProfileStatus,
  workerRequirementLabel,
  workerRequirementSection,
  workerSectionAnchor,
} from '@/shared/worker-profile/useWorkerProfileStatus';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const catalog = [{ id: '1', value: 'Item' }];

function worker(overrides: Partial<WorkerProfileDetail> = {}): WorkerProfileDetail {
  return {
    firstName: 'Ana',
    lastName: 'Lopez',
    birthDay: '1990-05-01T00:00:00',
    mobileNumber: '4165550199',
    location: { address: '100 King St W' },
    contactEmergencyPhone: '4165550100',
    socialInsurance: null,
    socialInsuranceExpire: false,
    dueDate: null,
    licenses: [],
    availabilities: catalog,
    availabilityDays: catalog,
    locationPreferences: catalog,
    skills: [{ id: '1', skill: 'Forklift' }],
    jobExperiences: [{ id: '1' }],
    missingRequirements: [],
    ...overrides,
  } as WorkerProfileDetail;
}

describe('requirement helpers', () => {
  it('maps requirements to labels and sections', () => {
    expect(workerRequirementLabel('Identification')).toBe('Identification document');
    expect(workerRequirementSection('Identification')).toBe('documents');
    expect(workerRequirementSection('ProfileImage')).toBe('personal');
  });

  it('recognises section ids', () => {
    expect(isWorkerProfileSectionId('documents')).toBe(true);
    expect(isWorkerProfileSectionId('payroll')).toBe(false);
    expect(isWorkerProfileSectionId(3)).toBe(false);
  });

  it('builds the section anchor', () => {
    expect(workerSectionAnchor('skills')).toBe('worker-section-skills');
  });
});

describe('with a fixed clock', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-01-15T12:00:00'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  describe('expiryStatus', () => {
    it.each([
      [null, 'No expiry', 'neutral'],
      ['2026-01-14T00:00:00', 'Expired', 'danger'],
      ['2026-01-15T00:00:00', 'Expires in 0 days', 'warning'],
      ['2026-01-16T00:00:00', 'Expires in 1 day', 'warning'],
      ['2026-02-14T00:00:00', 'Expires in 30 days', 'warning'],
      ['2026-02-15T00:00:00', 'Valid', 'success'],
    ])('%s -> %s', (expiresAt, label, tone) => {
      expect(expiryStatus(expiresAt)).toEqual({ label, tone });
    });
  });

  describe('useWorkerProfileStatus', () => {
    it('returns nothing without a worker', () => {
      const status = useWorkerProfileStatus(ref(null));
      expect(status.attentionItems.value).toEqual([]);
      expect(status.sections.value).toEqual([]);
      expect(status.completeness.value).toBe(0);
    });

    it('reports a complete profile', () => {
      const status = useWorkerProfileStatus(ref(worker()));
      expect(status.attentionItems.value).toEqual([]);
      expect(status.completeness.value).toBe(100);
      expect(status.missingLabels.value).toEqual([]);
    });

    it('lists missing requirements and marks their sections incomplete', () => {
      const status = useWorkerProfileStatus(ref(worker({ missingRequirements: ['ProfileImage', 'Identification'] })));

      expect(status.attentionItems.value.map((i) => [i.title, i.severity, i.sectionId])).toEqual([
        ['Profile photo missing', 'danger', 'personal'],
        ['Identification document missing', 'danger', 'documents'],
      ]);
      expect(status.missingLabels.value).toEqual(['Personal', 'Documents']);
      expect(status.completeness.value).toBe(67);
    });

    it('warns about licenses expiring soon and sorts dangers first', () => {
      const status = useWorkerProfileStatus(ref(worker({
        licenses: [
          { license: { description: 'Forklift' }, number: null, issued: null, expires: '2026-01-25T00:00:00' },
          { license: { description: 'WHMIS' }, number: null, issued: null, expires: '2027-01-01T00:00:00' },
          { license: {}, number: null, issued: null, expires: '2026-01-10T00:00:00' },
          { license: { description: 'No expiry' }, number: null, issued: null, expires: null },
        ],
      })));

      expect(status.attentionItems.value.map((i) => [i.key, i.severity, i.title, i.detail])).toEqual([
        ['license-2', 'danger', 'License expired', 'Expired 10-Jan-2026'],
        ['license-0', 'warning', 'Forklift expires 25-Jan-2026', '10 days left'],
      ]);
    });

    it('warns about a SIN expiring soon', () => {
      const status = useWorkerProfileStatus(ref(worker({
        socialInsurance: '123456789',
        socialInsuranceExpire: true,
        dueDate: '2026-01-20T00:00:00',
      })));

      expect(status.attentionItems.value).toEqual([
        { key: 'sin-expiry', severity: 'warning', title: 'SIN/SSN expires 20-Jan-2026', detail: '5 days left', sectionId: 'personal' },
      ]);
    });

    it('leaves an expired SIN to the missing requirement', () => {
      const status = useWorkerProfileStatus(ref(worker({
        socialInsurance: '123456789',
        socialInsuranceExpire: true,
        dueDate: '2026-01-01T00:00:00',
        missingRequirements: ['SocialInsuranceExpired'],
      })));

      expect(status.attentionItems.value.map((i) => i.key)).toEqual(['requirement-SocialInsuranceExpired']);
    });

    it('marks sections without data as incomplete', () => {
      const status = useWorkerProfileStatus(ref(worker({ skills: [], jobExperiences: [], contactEmergencyPhone: null })));
      expect(status.missingLabels.value).toEqual(['Contact & emergency', 'Skills', 'Experience']);
      expect(status.completeness.value).toBe(50);
    });
  });
});
