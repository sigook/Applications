import { computed, onBeforeUnmount, ref, type Ref } from 'vue';
import { dateMonth } from '@/shared/format';
import type {
  WorkerExpiryStatus,
  WorkerAttentionItem,
  WorkerProfileDetail,
  WorkerProfileSection,
  WorkerProfileSectionId,
  WorkerRequirement,
} from '@/shared/worker-profile/types';

const EXPIRING_SOON_DAYS = 30;
const DAY_MS = 24 * 60 * 60 * 1000;

const WORKER_REQUIREMENTS: Record<WorkerRequirement, { label: string; detail: string; sectionId: WorkerProfileSectionId }> = {
  ProfileImage: { label: 'Profile photo', detail: 'Upload a profile photo', sectionId: 'personal' },
  TaxCategory: { label: 'Tax category', detail: 'The agency must set the tax category', sectionId: 'personal' },
  SocialInsuranceNumber: { label: 'Social Insurance number', detail: 'Add the SIN/SSN number', sectionId: 'personal' },
  SocialInsuranceFile: { label: 'Social Insurance document', detail: 'Upload the SIN/SSN document', sectionId: 'personal' },
  SocialInsuranceExpired: { label: 'Social Insurance expired', detail: 'Renew the SIN/SSN', sectionId: 'personal' },
  Identification: { label: 'Identification document', detail: 'Add the ID number and upload the document', sectionId: 'documents' },
};

export function workerRequirementLabel(requirement: WorkerRequirement): string {
  return WORKER_REQUIREMENTS[requirement]?.label ?? requirement;
}

export function workerRequirementSection(requirement: WorkerRequirement): WorkerProfileSectionId {
  return WORKER_REQUIREMENTS[requirement]?.sectionId ?? 'personal';
}

export function isWorkerProfileSectionId(value: unknown): value is WorkerProfileSectionId {
  return typeof value === 'string' && ['personal', 'contact', 'documents', 'preferences', 'skills', 'experience', 'comments'].includes(value);
}

function daysUntil(value: string): number {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return Math.ceil((new Date(value).getTime() - today.getTime()) / DAY_MS);
}

function expiryItem(
  key: string,
  name: string,
  expiresAt: string,
  sectionId: WorkerProfileSectionId,
): WorkerAttentionItem | null {
  const days = daysUntil(expiresAt);
  if (days < 0) {
    return { key, severity: 'danger', title: `${name} expired`, detail: `Expired ${dateMonth(expiresAt)}`, sectionId };
  }
  if (days <= EXPIRING_SOON_DAYS) {
    const left = days === 1 ? '1 day left' : `${days} days left`;
    return { key, severity: 'warning', title: `${name} expires ${dateMonth(expiresAt)}`, detail: left, sectionId };
  }
  return null;
}

export function expiryStatus(expiresAt: string | null): WorkerExpiryStatus {
  if (!expiresAt) {
    return { label: 'No expiry', tone: 'neutral' };
  }
  const days = daysUntil(expiresAt);
  if (days < 0) {
    return { label: 'Expired', tone: 'danger' };
  }
  if (days <= EXPIRING_SOON_DAYS) {
    return { label: days === 1 ? 'Expires in 1 day' : `Expires in ${days} days`, tone: 'warning' };
  }
  return { label: 'Valid', tone: 'success' };
}

export function workerSectionAnchor(id: WorkerProfileSectionId): string {
  return `worker-section-${id}`;
}

export function scrollToWorkerSection(id: WorkerProfileSectionId) {
  document.getElementById(workerSectionAnchor(id))?.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

export function useActiveWorkerSection(sections: Ref<WorkerProfileSection[]>) {
  const activeSectionId = ref<WorkerProfileSectionId | null>('personal');
  let observer: IntersectionObserver | null = null;

  function selectSection(id: WorkerProfileSectionId) {
    activeSectionId.value = id;
    scrollToWorkerSection(id);
  }

  function observeSections() {
    if (observer || typeof IntersectionObserver === 'undefined') {
      return;
    }
    observer = new IntersectionObserver(
      (entries) => {
        const visible = entries.find((e) => e.isIntersecting);
        if (visible) {
          const id = sections.value.find((s) => workerSectionAnchor(s.id) === visible.target.id)?.id;
          if (id) {
            activeSectionId.value = id;
          }
        }
      },
      { rootMargin: '-80px 0px -70% 0px' },
    );
    sections.value.forEach((s) => {
      const el = document.getElementById(workerSectionAnchor(s.id));
      if (el) {
        observer?.observe(el);
      }
    });
  }

  onBeforeUnmount(() => observer?.disconnect());

  return { activeSectionId, selectSection, observeSections };
}

export function useWorkerProfileStatus(worker: Ref<WorkerProfileDetail | null>) {
  const attentionItems = computed<WorkerAttentionItem[]>(() => {
    const w = worker.value;
    if (!w) {
      return [];
    }
    const items: WorkerAttentionItem[] = w.missingRequirements.map((requirement) => {
      const info = WORKER_REQUIREMENTS[requirement];
      return { key: `requirement-${requirement}`, severity: 'danger', title: `${info.label} missing`, detail: info.detail, sectionId: info.sectionId };
    });

    if (w.socialInsurance && w.socialInsuranceExpire && w.dueDate && daysUntil(w.dueDate) >= 0) {
      const sinItem = expiryItem('sin-expiry', 'SIN/SSN', w.dueDate, 'personal');
      if (sinItem) {
        items.push(sinItem);
      }
    }

    w.licenses.forEach((item, index) => {
      if (!item.expires) {
        return;
      }
      const name = item.license.description || 'License';
      const licenseItem = expiryItem(`license-${index}`, name, item.expires, 'documents');
      if (licenseItem) {
        items.push(licenseItem);
      }
    });

    return items.sort((a, b) => (a.severity === b.severity ? 0 : a.severity === 'danger' ? -1 : 1));
  });

  const sections = computed<WorkerProfileSection[]>(() => {
    const w = worker.value;
    if (!w) {
      return [];
    }
    const pending = (id: WorkerProfileSectionId) => attentionItems.value.filter((i) => i.sectionId === id).length;
    const requirementsMet = (id: WorkerProfileSectionId) => !w.missingRequirements.some((r) => WORKER_REQUIREMENTS[r].sectionId === id);
    return [
      { id: 'personal', label: 'Personal', isComplete: !!(w.firstName && w.lastName && w.birthDay) && requirementsMet('personal'), pendingCount: pending('personal') },
      { id: 'contact', label: 'Contact & emergency', isComplete: !!(w.mobileNumber && w.location && w.contactEmergencyPhone), pendingCount: pending('contact') },
      { id: 'documents', label: 'Documents', isComplete: requirementsMet('documents'), pendingCount: pending('documents') },
      { id: 'preferences', label: 'Work preferences', isComplete: w.availabilities.length > 0 && w.availabilityDays.length > 0 && w.locationPreferences.length > 0, pendingCount: pending('preferences') },
      { id: 'skills', label: 'Skills', isComplete: w.skills.length > 0, pendingCount: pending('skills') },
      { id: 'experience', label: 'Experience', isComplete: w.jobExperiences.length > 0, pendingCount: pending('experience') },
      { id: 'comments', label: 'Comments', isComplete: null, pendingCount: 0 },
    ];
  });

  const completeness = computed(() => {
    const tracked = sections.value.filter((s) => s.isComplete !== null);
    if (tracked.length === 0) {
      return 0;
    }
    return Math.round((tracked.filter((s) => s.isComplete).length / tracked.length) * 100);
  });

  const missingLabels = computed(() => sections.value.filter((s) => s.isComplete === false).map((s) => s.label));

  return { attentionItems, sections, completeness, missingLabels };
}
