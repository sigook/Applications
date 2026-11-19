import { beforeEach, describe, expect, it, vi } from 'vitest';
import { effectScope } from 'vue';
import { useRunnerActions, type RunnerActionTarget } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';
import { deleteAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ deleteAgencyRunner: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { confirm: vi.fn() };
  return { getDialog: () => dialog };
});

interface ConfirmOptions {
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  type: string;
  hasIcon: boolean;
  onConfirm: () => Promise<void>;
}

const dialogConfirm = vi.mocked(getDialog().confirm);

const runner: RunnerActionTarget = {
  requestId: 'req-1',
  runnerId: 'run-1',
  name: 'Ana <b>Lopez</b>',
  status: RunnerStatus.SentToClient,
};

function setup() {
  const onUpdated = vi.fn();
  const scope = effectScope();
  const actions = scope.run(() => useRunnerActions(onUpdated));
  if (!actions) throw new Error('scope did not run');
  return { actions, onUpdated };
}

function lastConfirm(): ConfirmOptions {
  const call = dialogConfirm.mock.calls.at(-1);
  if (!call) throw new Error('confirm was not opened');
  return call[0] as ConfirmOptions;
}

beforeEach(() => {
  vi.clearAllMocks();
});

describe('useRunnerActions', () => {
  it('starts with no target and every modal closed', () => {
    const { actions } = setup();

    expect(actions.target.value).toBeNull();
    expect(actions.showStatus.value).toBe(false);
    expect(actions.showInterview.value).toBe(false);
    expect(actions.showHistory.value).toBe(false);
    expect(actions.isDeleting.value).toBe(false);
  });

  it.each([
    ['status', true, false, false],
    ['interview', false, true, false],
    ['history', false, false, true],
  ] as const)('open(%s) shows only that modal', (action, status, interview, history) => {
    const { actions } = setup();

    actions.open(runner, action);

    expect(actions.target.value).toEqual(runner);
    expect(actions.showStatus.value).toBe(status);
    expect(actions.showInterview.value).toBe(interview);
    expect(actions.showHistory.value).toBe(history);
  });

  it('switching action closes the previous modal', () => {
    const { actions } = setup();
    const other = { ...runner, runnerId: 'run-2', name: 'Other' };

    actions.open(runner, 'status');
    actions.open(other, 'history');

    expect(actions.target.value).toEqual(other);
    expect(actions.showStatus.value).toBe(false);
    expect(actions.showHistory.value).toBe(true);
  });

  it('asks for confirmation with an escaped name before deleting', () => {
    const { actions } = setup();

    actions.confirmDelete(runner);

    const options = lastConfirm();
    expect(options.title).toBe('Delete runner');
    expect(options.type).toBe('is-danger');
    expect(options.confirmText).toBe('Delete');
    expect(options.message).toContain('<strong>Ana &lt;b&gt;Lopez&lt;/b&gt;</strong>');
    expect(options.message).not.toContain('<b>');
    expect(deleteAgencyRunner).not.toHaveBeenCalled();
  });

  it('deletes the runner on confirm and notifies the caller', async () => {
    vi.mocked(deleteAgencyRunner).mockResolvedValue();
    const { actions, onUpdated } = setup();

    actions.confirmDelete(runner);
    const pending = lastConfirm().onConfirm();
    expect(actions.isDeleting.value).toBe(true);
    await pending;

    expect(deleteAgencyRunner).toHaveBeenCalledWith('req-1', 'run-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Runner deleted');
    expect(onUpdated).toHaveBeenCalledOnce();
    expect(showAlertError).not.toHaveBeenCalled();
    expect(actions.isDeleting.value).toBe(false);
  });

  it('shows the error and does not notify when the delete fails', async () => {
    const error = { data: 'Cannot delete' };
    vi.mocked(deleteAgencyRunner).mockRejectedValue(error);
    const { actions, onUpdated } = setup();

    actions.confirmDelete(runner);
    await lastConfirm().onConfirm();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(onUpdated).not.toHaveBeenCalled();
    expect(actions.isDeleting.value).toBe(false);
  });
});
