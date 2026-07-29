import { ref } from 'vue';
import { deleteAgencyRunner } from '@/api/agencyRunnerApi';
import { getDialog } from '@/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import type { RunnerStatus } from '@/types/runner';

// The runner a menu acts on, regardless of where it is listed (order's Runners
// tab or a weekly board card).
export interface RunnerActionTarget {
  requestId: string;
  runnerId: string;
  name: string;
  status: RunnerStatus;
}

export type RunnerAction = 'status' | 'interview' | 'history';

export function useRunnerActions(onUpdated: () => void) {
  const target = ref<RunnerActionTarget | null>(null);
  const showStatus = ref(false);
  const showInterview = ref(false);
  const showHistory = ref(false);
  const isDeleting = ref(false);

  function open(runner: RunnerActionTarget, action: RunnerAction): void {
    target.value = runner;
    showStatus.value = action === 'status';
    showInterview.value = action === 'interview';
    showHistory.value = action === 'history';
  }

  function confirmDelete(runner: RunnerActionTarget): void {
    getDialog().confirm({
      title: 'Delete runner',
      message: `Delete <strong>${runner.name}</strong> from this order? Their status history and interviews will be deleted too.`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      type: 'is-danger',
      hasIcon: true,
      onConfirm: () => remove(runner),
    });
  }

  function remove(runner: RunnerActionTarget): void {
    isDeleting.value = true;
    deleteAgencyRunner(runner.requestId, runner.runnerId)
      .then(() => {
        showAlertSuccess('Runner deleted');
        onUpdated();
      })
      .catch(error => showAlertError(error))
      .finally(() => {
        isDeleting.value = false;
      });
  }

  return { target, showStatus, showInterview, showHistory, isDeleting, open, confirmDelete };
}
