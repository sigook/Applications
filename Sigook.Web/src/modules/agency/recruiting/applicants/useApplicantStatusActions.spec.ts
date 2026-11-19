import { beforeEach, describe, expect, it, vi } from 'vitest';
import { effectScope } from 'vue';
import {
  useApplicantStatusActions,
  type ApplicantStatusTarget,
} from '@/modules/agency/recruiting/applicants/useApplicantStatusActions';
import { changeApplicantStatus } from '@/modules/agency/recruiting/requests/api';
import { RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ changeApplicantStatus: vi.fn() }));
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
  onConfirm: () => Promise<void>;
}

const dialogConfirm = vi.mocked(getDialog().confirm);

const target: ApplicantStatusTarget = { requestId: 'req-1', applicantId: 'app-1', name: 'Tom & "Jerry"' };

function setup() {
  const onChanged = vi.fn();
  const scope = effectScope();
  const actions = scope.run(() => useApplicantStatusActions(onChanged));
  if (!actions) throw new Error('scope did not run');
  return { actions, onChanged };
}

function lastConfirm(): ConfirmOptions {
  const call = dialogConfirm.mock.calls.at(-1);
  if (!call) throw new Error('confirm was not opened');
  return call[0] as ConfirmOptions;
}

async function flush() {
  await new Promise((resolve) => setTimeout(resolve, 0));
}

beforeEach(() => {
  vi.clearAllMocks();
});

describe('useApplicantStatusActions', () => {
  it.each([
    ['start', RequestApplicantStatus.InProgress, 'Applicant started'],
    ['reopen', RequestApplicantStatus.InProgress, 'Applicant reopened'],
    ['confirm', RequestApplicantStatus.Confirmed, 'Applicant confirmed'],
  ] as const)('%s sends status %s', async (action, status, message) => {
    vi.mocked(changeApplicantStatus).mockResolvedValue();
    const { actions, onChanged } = setup();

    actions[action](target);
    expect(actions.isChangingStatus.value).toBe(true);
    await flush();

    expect(changeApplicantStatus).toHaveBeenCalledWith('req-1', 'app-1', { status });
    expect(showAlertSuccess).toHaveBeenCalledWith(message);
    expect(onChanged).toHaveBeenCalledWith(status);
    expect(actions.isChangingStatus.value).toBe(false);
  });

  it('reports the error and does not notify when the change fails', async () => {
    const error = new Error('boom');
    vi.mocked(changeApplicantStatus).mockRejectedValue(error);
    const { actions, onChanged } = setup();

    actions.confirm(target);
    await flush();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(onChanged).not.toHaveBeenCalled();
    expect(actions.isChangingStatus.value).toBe(false);
  });

  it('asks before cancelling and escapes the applicant name', () => {
    const { actions } = setup();

    actions.confirmCancel(target);

    const options = lastConfirm();
    expect(options.title).toBe('Cancel applicant');
    expect(options.confirmText).toBe('Cancel applicant');
    expect(options.cancelText).toBe('Keep');
    expect(options.type).toBe('is-danger');
    expect(options.message).toContain('<strong>Tom &amp; &quot;Jerry&quot;</strong>');
    expect(changeApplicantStatus).not.toHaveBeenCalled();
  });

  it('cancels the applicant once confirmed', async () => {
    vi.mocked(changeApplicantStatus).mockResolvedValue();
    const { actions, onChanged } = setup();

    actions.confirmCancel(target);
    await lastConfirm().onConfirm();

    expect(changeApplicantStatus).toHaveBeenCalledWith('req-1', 'app-1', { status: RequestApplicantStatus.Cancelled });
    expect(showAlertSuccess).toHaveBeenCalledWith('Applicant cancelled');
    expect(onChanged).toHaveBeenCalledWith(RequestApplicantStatus.Cancelled);
  });
});
