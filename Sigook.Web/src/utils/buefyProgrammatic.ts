import type { App } from 'vue';
import { ToastProgrammatic, DialogProgrammatic } from '@ntohq/buefy-next';

let toastInstance: any = null;
let dialogInstance: any = null;

export function setupBuefyProgrammatic(app: App): void {
  toastInstance = new (ToastProgrammatic as any)(app);
  dialogInstance = new (DialogProgrammatic as any)(app);
}

export function getToast(): any {
  if (!toastInstance) {
    throw new Error('Buefy programmatic toast not initialized. Call setupBuefyProgrammatic(app) first.');
  }
  return toastInstance;
}

export function getDialog(): any {
  if (!dialogInstance) {
    throw new Error('Buefy programmatic dialog not initialized. Call setupBuefyProgrammatic(app) first.');
  }
  return dialogInstance;
}
