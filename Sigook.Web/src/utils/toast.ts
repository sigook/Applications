import { getToast, getDialog } from '@/utils/buefyProgrammatic';

export async function getErrorMessage(errorMessage: any): Promise<string> {
  if (
    typeof errorMessage === "object" &&
    errorMessage.data !== null && errorMessage.data !== undefined
  ) {
    if (errorMessage.data instanceof Blob) {
      errorMessage = await errorMessage.data.text();
      try {
        const parsed = JSON.parse(errorMessage);
        return Object.values(parsed).flat().join(', ');
      } catch {
        return errorMessage;
      }
    }
    errorMessage = errorMessage.data;
  }
  if (typeof errorMessage === "object") {
    const objectMessage = errorMessage;
    errorMessage = "";
    for (const item in objectMessage) {
      if (
        typeof objectMessage[item] === "object" &&
        objectMessage[item].message
      ) {
        errorMessage += objectMessage[item].message + "<br/>";
      } else {
        errorMessage += objectMessage[item] + "<br/>";
      }
    }
  }
  return errorMessage;
}

export async function showAlertError(errorMessage: any): Promise<void> {
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
