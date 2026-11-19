import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import Callback from '@/app/pages/Callback.vue';
import mgr from '@/app/security/securityService';
import { createTestRouter } from '@/testing/mountWithPlugins';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  user: null as UserProfile | null,
  userRoles: [] as string[],
  getUser: vi.fn<() => Promise<UserProfile | null>>(),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));
vi.mock('@/app/security/securityService', () => ({ default: { signinRedirectCallback: vi.fn() } }));

const profile: UserProfile = { token_type: 'Bearer', access_token: 't', profile: { email: 'a@b.com', role: 'worker', sub: 'u1', name: 'Ann' } };

async function setup() {
  const router = createTestRouter();
  const push = vi.spyOn(router, 'push');
  const wrapper = mount(Callback, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, push };
}

describe('Callback', () => {
  beforeEach(() => {
    security.user = null;
    security.userRoles = [];
  });

  afterEach(() => {
    window.history.replaceState(null, '', '/');
  });

  it('completes the sign-in and goes to the role home page', async () => {
    window.history.replaceState(null, '', '/callback?code=abc&state=xyz');
    vi.mocked(mgr.signinRedirectCallback).mockResolvedValue(profile as never);
    security.getUser.mockImplementation(() => {
      security.userRoles = ['worker'];
      return Promise.resolve(profile);
    });
    const { push } = await setup();

    expect(mgr.signinRedirectCallback).toHaveBeenCalledOnce();
    expect(security.getUser).toHaveBeenCalledOnce();
    expect(push).toHaveBeenCalledWith('/worker-requests');
  });

  it('goes home when there is no authorization code', async () => {
    window.history.replaceState(null, '', '/callback');
    const { push } = await setup();

    expect(mgr.signinRedirectCallback).not.toHaveBeenCalled();
    expect(push).toHaveBeenCalledWith('/');
  });

  it('skips the callback for a signed-in user', async () => {
    security.user = profile;
    security.userRoles = ['company'];
    const { push } = await setup();

    expect(mgr.signinRedirectCallback).not.toHaveBeenCalled();
    expect(push).toHaveBeenCalledWith('/company-requests');
  });

  it('goes home when the callback fails', async () => {
    window.history.replaceState(null, '', '/callback?code=abc');
    vi.mocked(mgr.signinRedirectCallback).mockRejectedValue(new Error('state mismatch'));
    const { push } = await setup();

    expect(security.getUser).not.toHaveBeenCalled();
    expect(push).toHaveBeenCalledWith('/');
  });

  it('hides the loader once done', async () => {
    security.user = profile;
    const { wrapper } = await setup();

    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
