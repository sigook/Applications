import { afterEach, vi } from 'vitest';
import { enableAutoUnmount } from '@vue/test-utils';

const WAIT_FOR_TIMEOUT_MS = 5000;
const waitFor = vi.waitFor;

vi.waitFor = ((callback, options) =>
  waitFor(callback, typeof options === 'number'
    ? { timeout: Math.max(options, WAIT_FOR_TIMEOUT_MS) }
    : { timeout: WAIT_FOR_TIMEOUT_MS, ...options })) as typeof vi.waitFor;

enableAutoUnmount(afterEach);

afterEach(() => {
  document.body.innerHTML = '';
});
