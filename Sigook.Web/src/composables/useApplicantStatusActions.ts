import { ref, type Ref } from 'vue';
import { getDialog } from '@/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { changeApplicantStatus } from '@/api/agencyRequestApi';
import { RequestApplicantStatus } from '@/types/requestApplicant';

export interface ApplicantStatusTarget {
  requestId: string;
  applicantId: string;
  name: string;
}

// Status transitions of a request applicant. Shared by the compliance modal of
// the request detail and by the applicants workspace, so the rules and the
// wording of the cancel confirmation live in one place.
export function useApplicantStatusActions(onChanged: (status: RequestApplicantStatus) => void): {
  isChangingStatus: Ref<boolean>;
  start: (target: ApplicantStatusTarget) => void;
  reopen: (target: ApplicantStatusTarget) => void;
  confirm: (target: ApplicantStatusTarget) => void;
  confirmCancel: (target: ApplicantStatusTarget) => void;
} {
  const isChangingStatus = ref(false);

  function changeStatus(target: ApplicantStatusTarget, status: RequestApplicantStatus, successMessage: string) {
    isChangingStatus.value = true;
    changeApplicantStatus(target.requestId, target.applicantId, { status })
      .then(() => {
        showAlertSuccess(successMessage);
        onChanged(status);
      })
      .catch((error) => showAlertError(error))
      .finally(() => {
        isChangingStatus.value = false;
      });
  }

  function start(target: ApplicantStatusTarget) {
    changeStatus(target, RequestApplicantStatus.InProgress, 'Applicant started');
  }

  function reopen(target: ApplicantStatusTarget) {
    changeStatus(target, RequestApplicantStatus.InProgress, 'Applicant reopened');
  }

  function confirm(target: ApplicantStatusTarget) {
    changeStatus(target, RequestApplicantStatus.Confirmed, 'Applicant confirmed');
  }

  function confirmCancel(target: ApplicantStatusTarget) {
    getDialog().confirm({
      title: 'Cancel applicant',
      message: `Cancel <strong>${target.name}</strong>? Their compliance checks are kept and the applicant can be reopened later.`,
      confirmText: 'Cancel applicant',
      cancelText: 'Keep',
      type: 'is-danger',
      hasIcon: true,
      onConfirm: () => changeStatus(target, RequestApplicantStatus.Cancelled, 'Applicant cancelled'),
    });
  }

  return { isChangingStatus, start, reopen, confirm, confirmCancel };
}
