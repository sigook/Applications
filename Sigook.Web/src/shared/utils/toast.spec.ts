import { beforeEach, describe, expect, it, vi } from 'vitest';
import { getErrorMessage, showAlertConfirm, showAlertError, showAlertSuccess, showAlertSummary } from '@/shared/utils/toast';
import { getDialog, getToast } from '@/shared/utils/buefyProgrammatic';

vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const toast = { open: vi.fn() };
  const dialog = { alert: vi.fn(), confirm: vi.fn() };
  return { getToast: () => toast, getDialog: () => dialog };
});

const toastOpen = vi.mocked(getToast().open);
const dialogAlert = vi.mocked(getDialog().alert);
const dialogConfirm = vi.mocked(getDialog().confirm);

interface DialogCallbacks {
  onConfirm: () => void;
  onCancel?: () => void;
}

function lastDialogOptions(mock: typeof dialogConfirm | typeof dialogAlert): DialogCallbacks {
  const call = mock.mock.calls.at(-1);
  if (!call) throw new Error('dialog was not opened');
  return call[0] as DialogCallbacks;
}

beforeEach(() => {
  vi.clearAllMocks();
});

describe('getErrorMessage', () => {
  it('returns a plain string unchanged', async () => {
    expect(await getErrorMessage('Boom')).toBe('Boom');
  });

  it('unwraps a string inside data', async () => {
    expect(await getErrorMessage({ data: 'Not allowed' })).toBe('Not allowed');
  });

  it('joins object values and nested messages with line breaks', async () => {
    expect(await getErrorMessage({ data: { email: 'Invalid', name: { message: 'Required' } } }))
      .toBe('Invalid<br/>Required<br/>');
  });

  it('parses a JSON blob and flattens its values', async () => {
    const blob = new Blob([JSON.stringify({ file: ['Too big', 'Wrong type'] })]);
    expect(await getErrorMessage({ data: blob })).toBe('Too big, Wrong type');
  });

  it('returns the raw text of a non-JSON blob', async () => {
    expect(await getErrorMessage({ data: new Blob(['plain failure']) })).toBe('plain failure');
  });

  it('returns an empty string for null', async () => {
    expect(await getErrorMessage(null)).toBe('');
  });
});

describe('showAlertError', () => {
  it('opens a danger toast with the resolved message', async () => {
    showAlertError({ data: 'Failed' });
    await vi.waitFor(() => { expect(toastOpen).toHaveBeenCalled(); });
    expect(toastOpen).toHaveBeenCalledWith(expect.objectContaining({ message: 'Failed', type: 'is-danger' }));
  });

  it('does not open a toast when the message is empty', async () => {
    showAlertError('');
    await new Promise((resolve) => { setTimeout(resolve, 0); });
    expect(toastOpen).not.toHaveBeenCalled();
  });
});

describe('showAlertSuccess', () => {
  it('opens a success toast', () => {
    showAlertSuccess('Saved');
    expect(toastOpen).toHaveBeenCalledWith(expect.objectContaining({ message: 'Saved', type: 'is-success' }));
  });
});

describe('showAlertSummary', () => {
  it('resolves when the alert is confirmed', async () => {
    const result = showAlertSummary('Title', 'Body');
    lastDialogOptions(dialogAlert).onConfirm();
    await expect(result).resolves.toBeUndefined();
  });
});

describe('showAlertConfirm', () => {
  it('resolves true on confirm', async () => {
    const result = showAlertConfirm('Are you sure');
    lastDialogOptions(dialogConfirm).onConfirm();
    await expect(result).resolves.toBe(true);
  });

  it('resolves false on cancel', async () => {
    const result = showAlertConfirm('Are you sure', 'Delete?', 'Yes');
    lastDialogOptions(dialogConfirm).onCancel?.();
    await expect(result).resolves.toBe(false);
  });
});
