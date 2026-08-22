import { ref } from 'vue';
import type { RequestApplicantStatus } from '@/types/requestApplicant';

// The applicant the Compliance modal acts on in the order's Applicants tab.
// Status actions (start, cancel, reopen, confirm) live inside the modal.
export interface ApplicantActionTarget {
  requestId: string;
  applicantId: string;
  name: string;
  status: RequestApplicantStatus;
  workerProfileId?: string | null;
  candidateId?: string | null;
}

export function useApplicantActions() {
  const target = ref<ApplicantActionTarget | null>(null);
  const showCompliance = ref(false);

  function openCompliance(applicant: ApplicantActionTarget): void {
    target.value = applicant;
    showCompliance.value = true;
  }

  return { target, showCompliance, openCompliance };
}
