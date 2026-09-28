import { computed, type Ref } from 'vue';
import { dateMonth } from '@/utils/filters';
import type {
  WorkerExpiryStatus,
  WorkerAttentionItem,
  WorkerProfileDetail,
  WorkerProfileSection,
  WorkerProfileSectionId,
} from '@/types/worker';

const EXPIRING_SOON_DAYS = 30;
const DAY_MS = 24 * 60 * 60 * 1000;

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

export function useWorkerProfileStatus(worker: Ref<WorkerProfileDetail | null>) {
  const attentionItems = computed<WorkerAttentionItem[]>(() => {
    const w = worker.value;
    if (!w) {
      return [];
    }
    const items: WorkerAttentionItem[] = [];

    if (!w.socialInsurance) {
      items.push({ key: 'sin-missing', severity: 'danger', title: 'SIN/SSN missing', detail: 'Add the SIN/SSN number', sectionId: 'personal' });
    } else if (w.socialInsuranceExpire && w.dueDate) {
      const sinItem = expiryItem('sin-expiry', 'SIN/SSN', w.dueDate, 'personal');
      if (sinItem) {
        items.push(sinItem);
      }
    }

    if (!w.identificationType1File) {
      items.push({ key: 'id-missing', severity: 'danger', title: 'Identification missing', detail: 'Upload an ID document', sectionId: 'documents' });
    }

    if (!w.resume) {
      items.push({ key: 'resume-missing', severity: 'danger', title: 'Resume missing', detail: 'Upload a resume', sectionId: 'documents' });
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
    return [
      { id: 'personal', label: 'Personal', isComplete: !!(w.firstName && w.lastName && w.birthDay && w.socialInsurance), pendingCount: pending('personal') },
      { id: 'contact', label: 'Contact & emergency', isComplete: !!(w.mobileNumber && w.location && w.contactEmergencyPhone), pendingCount: pending('contact') },
      { id: 'documents', label: 'Documents', isComplete: !!(w.identificationType1File && w.resume), pendingCount: pending('documents') },
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
