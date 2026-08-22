import type { App } from 'vue';
import { ToastProgrammatic, DialogProgrammatic } from 'buefy';

interface ToastConfig {
  message: string;
  type?: string;
  duration?: number;
  position?: string;
}

interface DialogConfirmConfig {
  title?: string;
  message?: string;
  confirmText?: string;
  cancelText?: string;
  type?: string;
  hasIcon?: boolean;
  onConfirm?: () => void;
  onCancel?: () => void;
}

interface DialogPromptConfig {
  title?: string;
  message?: string;
  confirmText?: string;
  cancelText?: string;
  type?: string;
  hasIcon?: boolean;
  closeOnConfirm?: boolean;
  inputAttrs?: Record<string, unknown>;
  onConfirm?: (value: string, dialog: { close: () => void }) => void;
  onCancel?: () => void;
}

interface ToastInstance {
  open(config: ToastConfig): void;
}

interface DialogInstance {
  confirm(config: DialogConfirmConfig): void;
  prompt(config: DialogPromptConfig): void;
}

let toastInstance: ToastInstance | null = null;
let dialogInstance: DialogInstance | null = null;

export function setupBuefyProgrammatic(app: App): void {
  toastInstance = new (ToastProgrammatic as unknown as new (app: App) => ToastInstance)(app);
  dialogInstance = new (DialogProgrammatic as unknown as new (app: App) => DialogInstance)(app);
}

export function getToast(): ToastInstance {
  if (!toastInstance) {
    throw new Error('Buefy programmatic toast not initialized. Call setupBuefyProgrammatic(app) first.');
  }
  return toastInstance;
}

export function getDialog(): DialogInstance {
  if (!dialogInstance) {
    throw new Error('Buefy programmatic dialog not initialized. Call setupBuefyProgrammatic(app) first.');
  }
  return dialogInstance;
}
