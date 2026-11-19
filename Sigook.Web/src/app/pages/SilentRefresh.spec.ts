import { describe, expect, it, vi } from 'vitest';
import SilentRefresh from '@/app/pages/SilentRefresh.vue';
import mgr from '@/app/security/securityService';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/app/security/securityService', () => ({ default: { signinSilentCallback: vi.fn() } }));

describe('SilentRefresh', () => {
  it('completes the silent sign-in', () => {
    vi.mocked(mgr.signinSilentCallback).mockResolvedValue();
    mountWithPlugins(SilentRefresh);

    expect(mgr.signinSilentCallback).toHaveBeenCalledOnce();
  });
});
