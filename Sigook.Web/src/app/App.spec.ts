import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import axios from 'axios';
import { createMemoryHistory, createRouter } from 'vue-router';
import App from '@/app/App.vue';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  user: null as UserProfile | null,
  getUser: vi.fn<() => Promise<UserProfile | null>>(),
}));
const appStore = vi.hoisted(() => ({ showMobile: vi.fn() }));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));
vi.mock('@/app/stores/app', () => ({ useAppStore: () => appStore }));
vi.mock('axios', () => ({ default: { get: vi.fn() } }));
vi.mock('@/app/layout/SidebarLogged.vue', () => ({ default: { name: 'SidebarLogged', template: '<nav class="sidebar-stub" />' } }));
vi.mock('@/modules/landing/layout/Header.vue', () => ({ default: { name: 'LandingHeader', template: '<header class="landing-header-stub" />' } }));
vi.mock('@/modules/landing/layout/Footer.vue', () => ({ default: { name: 'LandingFooter', template: '<footer class="landing-footer-stub" />' } }));
vi.mock('@/modules/landing/layout/GlobalBackground.vue', () => ({ default: { name: 'LandingBackground', template: '<div class="landing-bg-stub" />' } }));
vi.mock('@/modules/landing/layout/AppVersionToast.vue', () => ({
  default: {
    name: 'AppVersionToast',
    props: { modelValue: Boolean },
    emits: ['update', 'update:modelValue'],
    template: '<div class="toast-stub" :data-open="modelValue" />',
  },
}));

const Page = { template: '<div class="page">page</div>' };

const profile: UserProfile = { token_type: 'Bearer', access_token: 't', profile: { email: 'a@b.com', role: 'admin', sub: 'u1', name: 'Ann' } };

function createAppRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/', component: Page },
      { path: '/callback', name: 'callback', component: Page },
      { path: '/login', component: Page, meta: { layout: 'auth' } },
      { path: '/secure', component: Page, meta: { requiresAuth: true } },
      { path: '/home', component: Page, meta: { layout: 'landing' } },
      { path: '/jobs', component: Page, meta: { layout: 'landing-anonymous' } },
    ],
  });
}

async function setup(path: string) {
  const router = createAppRouter();
  await router.push(path);
  await router.isReady();
  const wrapper = mount(App, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, router };
}

function layout(wrapper: Awaited<ReturnType<typeof setup>>['wrapper']) {
  if (wrapper.find('.logged-layout').exists()) return 'logged';
  if (wrapper.find('.landing-page').exists()) return 'landing';
  if (wrapper.find('.web-layout').exists()) return 'web';
  if (wrapper.findComponent({ name: 'BLoading' }).exists()) return 'pending';
  return 'bare';
}

describe('App', () => {
  beforeEach(() => {
    security.user = null;
    security.getUser.mockResolvedValue(null);
    vi.mocked(axios.get).mockResolvedValue({ data: { version: 'v' } });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('renders the callback and auth pages without chrome', async () => {
    expect(layout((await setup('/callback')).wrapper)).toBe('bare');
    expect(layout((await setup('/login')).wrapper)).toBe('bare');
  });

  it('renders the logged layout for an authenticated user on a secure route', async () => {
    security.getUser.mockResolvedValue(profile);
    const { wrapper } = await setup('/secure');

    expect(layout(wrapper)).toBe('logged');
    expect(wrapper.find('.sidebar-stub').exists()).toBe(true);
    expect(wrapper.find('.logged-content .page').exists()).toBe(true);
  });

  it('keeps a loader on a secure route without a user', async () => {
    const { wrapper } = await setup('/secure');

    expect(layout(wrapper)).toBe('pending');
  });

  it('treats a failing user lookup as signed out', async () => {
    security.getUser.mockRejectedValue(new Error('expired'));
    const { wrapper } = await setup('/secure');

    expect(layout(wrapper)).toBe('pending');
  });

  it('uses the landing layout for landing pages', async () => {
    const { wrapper } = await setup('/home');

    expect(layout(wrapper)).toBe('landing');
    expect(wrapper.find('.landing-header-stub').exists()).toBe(true);
    expect(wrapper.find('.landing-footer-stub').exists()).toBe(true);
    expect(wrapper.find('.landing-bg-stub').exists()).toBe(true);
  });

  it('uses the landing layout for anonymous visitors only', async () => {
    expect(layout((await setup('/jobs')).wrapper)).toBe('landing');

    security.user = profile;
    expect(layout((await setup('/jobs')).wrapper)).toBe('web');
  });

  it('uses the plain web layout otherwise', async () => {
    const { wrapper } = await setup('/');

    expect(layout(wrapper)).toBe('web');
    expect(wrapper.find('.web-layout__brand').attributes('href')).toBe('/');
  });

  it('leaves the logged layout when navigating to a public route', async () => {
    security.getUser.mockResolvedValue(profile);
    const { wrapper, router } = await setup('/secure');
    await router.push('/');
    await flushPromises();

    expect(layout(wrapper)).toBe('web');
  });

  it('flags a new version when the deployed build differs', async () => {
    vi.mocked(axios.get).mockResolvedValue({ data: { version: 'v2026.11.19' } });
    const { wrapper } = await setup('/');

    expect(axios.get).toHaveBeenCalledWith('/version.json', { params: { t: expect.any(Number) as number } });
    expect(wrapper.findComponent({ name: 'AppVersionToast' }).props('modelValue')).toBe(true);
  });

  it('stays quiet when the version matches or cannot be read', async () => {
    const { wrapper } = await setup('/');
    expect(wrapper.findComponent({ name: 'AppVersionToast' }).props('modelValue')).toBe(false);

    vi.mocked(axios.get).mockRejectedValue(new Error('offline'));
    const second = await setup('/');
    expect(second.wrapper.findComponent({ name: 'AppVersionToast' }).props('modelValue')).toBe(false);
  });

  it('checks the version periodically and when the tab becomes visible', async () => {
    vi.useFakeTimers({ toFake: ['setInterval', 'clearInterval'] });
    const { wrapper } = await setup('/');
    expect(axios.get).toHaveBeenCalledTimes(1);

    vi.advanceTimersByTime(5 * 60 * 1000);
    expect(axios.get).toHaveBeenCalledTimes(2);

    const visibility = vi.spyOn(document, 'visibilityState', 'get').mockReturnValue('hidden');
    document.dispatchEvent(new Event('visibilitychange'));
    expect(axios.get).toHaveBeenCalledTimes(2);
    visibility.mockReturnValue('visible');
    document.dispatchEvent(new Event('visibilitychange'));
    expect(axios.get).toHaveBeenCalledTimes(3);

    wrapper.unmount();
    vi.advanceTimersByTime(5 * 60 * 1000);
    document.dispatchEvent(new Event('visibilitychange'));
    expect(axios.get).toHaveBeenCalledTimes(3);
  });

  it('clears the caches and reloads on update', async () => {
    const reload = vi.fn();
    vi.spyOn(window, 'location', 'get').mockReturnValue({ ...window.location, reload });
    const remove = vi.fn<(name: string) => Promise<boolean>>().mockResolvedValue(true);
    vi.stubGlobal('caches', { keys: () => Promise.resolve(['a', 'b']), delete: remove });
    const { wrapper } = await setup('/');
    wrapper.findComponent({ name: 'AppVersionToast' }).vm.$emit('update');
    await flushPromises();

    expect(remove.mock.calls).toEqual([['a'], ['b']]);
    expect(reload).toHaveBeenCalledOnce();
  });

  it('marks mobile devices', async () => {
    vi.spyOn(navigator, 'userAgent', 'get').mockReturnValue('Mozilla/5.0 (iPhone; CPU iPhone OS 17_0)');
    await setup('/');

    expect(appStore.showMobile).toHaveBeenCalledOnce();
  });

  it('does not mark desktop browsers as mobile', async () => {
    vi.spyOn(navigator, 'userAgent', 'get').mockReturnValue('Mozilla/5.0 (Windows NT 10.0; Win64; x64)');
    await setup('/');

    expect(appStore.showMobile).not.toHaveBeenCalled();
  });
});
