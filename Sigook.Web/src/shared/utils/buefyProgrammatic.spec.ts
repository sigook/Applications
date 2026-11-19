import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { App } from 'vue';

const { toastApps, dialogApps } = vi.hoisted(() => ({
  toastApps: [] as unknown[],
  dialogApps: [] as unknown[],
}));

vi.mock('buefy', () => ({
  ToastProgrammatic: class {
    readonly app: unknown;
    constructor(app: unknown) {
      this.app = app;
      toastApps.push(app);
    }
  },
  DialogProgrammatic: class {
    readonly app: unknown;
    constructor(app: unknown) {
      this.app = app;
      dialogApps.push(app);
    }
  },
}));

type BuefyProgrammaticModule = typeof import('@/shared/utils/buefyProgrammatic');

async function loadModule(): Promise<BuefyProgrammaticModule> {
  vi.resetModules();
  return import('@/shared/utils/buefyProgrammatic');
}

beforeEach(() => {
  toastApps.length = 0;
  dialogApps.length = 0;
});

describe('buefyProgrammatic', () => {
  it('throws when the toast is requested before setup', async () => {
    const { getToast } = await loadModule();
    expect(() => getToast()).toThrow('Buefy programmatic toast not initialized');
  });

  it('throws when the dialog is requested before setup', async () => {
    const { getDialog } = await loadModule();
    expect(() => getDialog()).toThrow('Buefy programmatic dialog not initialized');
  });

  it('creates toast and dialog instances bound to the app', async () => {
    const { setupBuefyProgrammatic, getToast, getDialog } = await loadModule();
    const app = {} as App;
    setupBuefyProgrammatic(app);
    expect(toastApps).toEqual([app]);
    expect(dialogApps).toEqual([app]);
    expect(getToast()).toHaveProperty('app', app);
    expect(getDialog()).toHaveProperty('app', app);
  });

  it('returns the same instances on every call', async () => {
    const { setupBuefyProgrammatic, getToast, getDialog } = await loadModule();
    setupBuefyProgrammatic({} as App);
    expect(getToast()).toBe(getToast());
    expect(getDialog()).toBe(getDialog());
  });
});
