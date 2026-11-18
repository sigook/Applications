import { getToast, getDialog } from '@/shared/utils/buefyProgrammatic';
import { escapeHtml } from '@/shared/utils/html';

export function getErrorMessage(errorMessage: unknown): Promise<string> {
  return formatError(errorMessage, escapeHtml, '<br/>');
}

export async function getErrorText(errorMessage: unknown): Promise<string> {
  return (await formatError(errorMessage, (text) => text, ' ')).trim();
}

async function formatError(errorMessage: unknown, encode: (text: string) => string, lineBreak: string): Promise<string> {
  let value: unknown = errorMessage;
  if (
    typeof value === "object" && value !== null &&
    "data" in value && (value as { data?: unknown }).data != null
  ) {
    const data = (value as { data?: unknown }).data;
    if (data instanceof Blob) {
      const text = await data.text();
      try {
        const parsed = JSON.parse(text) as Record<string, unknown>;
        return Object.values(parsed).flat().map((entry) => encode(String(entry))).join(', ');
      } catch {
        return encode(text);
      }
    }
    value = data;
  }
  if (typeof value === "object" && value !== null) {
    const objectMessage = value as Record<string, unknown>;
    let message = "";
    for (const item in objectMessage) {
      const entry = objectMessage[item];
      if (
        typeof entry === "object" && entry !== null &&
        (entry as { message?: unknown }).message
      ) {
        message += encode(String((entry as { message?: unknown }).message)) + lineBreak;
      } else {
        message += encode(String(entry)) + lineBreak;
      }
    }
    return message;
  }
  return encode(typeof value === "string" ? value : String(value ?? ""));
}

export function showAlertError(errorMessage: unknown): void {
  void getErrorMessage(errorMessage).then((message) => {
    if (message === "") return;
    getToast().open({
      message,
      type: 'is-danger',
      duration: 10000,
      position: 'is-top-right',
    });
  });
}

export function showAlertSuccess(successMessage: string): void {
  getToast().open({
    message: escapeHtml(successMessage),
    type: 'is-success',
    duration: 4000,
    position: 'is-top-right',
  });
}

export function showAlertSummary(title: string, message: string): Promise<void> {
  return new Promise((resolve) => {
    getDialog().alert({
      title,
      message,
      type: 'is-warning',
      hasIcon: true,
      confirmText: 'Ok',
      onConfirm: () => { resolve(); },
    });
  });
}

export function showAlertConfirm(title: string, message?: string, confirmBtnText?: string): Promise<boolean> {
  return new Promise((resolve) => {
    getDialog().confirm({
      title,
      message: message || '',
      confirmText: confirmBtnText || 'Ok',
      cancelText: 'Cancel',
      type: 'is-warning',
      hasIcon: true,
      onConfirm: () => { resolve(true); },
      onCancel: () => { resolve(false); },
    });
  });
}
