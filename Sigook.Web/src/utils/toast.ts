import { getToast, getDialog } from '@/utils/buefyProgrammatic';

export async function getErrorMessage(errorMessage: unknown): Promise<string> {
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
        return Object.values(parsed).flat().join(', ');
      } catch {
        return text;
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
        message += String((entry as { message?: unknown }).message) + "<br/>";
      } else {
        message += String(entry) + "<br/>";
      }
    }
    return message;
  }
  return typeof value === "string" ? value : String(value ?? "");
}

export async function showAlertError(errorMessage: unknown): Promise<void> {
  const message = await getErrorMessage(errorMessage);
  if (message == null || message === "") return;
  getToast().open({
    message,
    type: 'is-danger',
    duration: 10000,
    position: 'is-top-right',
  });
}

export function showAlertSuccess(successMessage: string): void {
  getToast().open({
    message: successMessage,
    type: 'is-success',
    duration: 4000,
    position: 'is-top-right',
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
      onConfirm: () => resolve(true),
      onCancel: () => resolve(false),
    });
  });
}
