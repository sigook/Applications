import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import type { RouteLocationNormalized, RouterScrollBehavior } from 'vue-router';
import router from '@/app/router';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  getUser: vi.fn<() => Promise<UserProfile | null>>(),
  userRoles: [] as string[],
}));

vi.mock('@/app/stores', () => ({ default: {} }));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));

vi.mock('@/app/pages/NotFound.vue', () => ({ default: { name: 'NotFoundStub', render: () => null } }));
vi.mock('@/app/pages/SilentRefresh.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/Unauthorized.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/EmailPreferences.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/Callback.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/auth/Login.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/auth/ForgotPassword.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/auth/ConfirmEmail.vue', () => ({ default: { render: () => null } }));
vi.mock('@/app/pages/auth/CreatePassword.vue', () => ({ default: { render: () => null } }));

vi.mock('@/modules/agency/routes', () => ({
  default: [
    { path: '/admin-only', component: { render: () => null }, meta: { requiresAuth: true, role: ['admin', 'superadmin'] } },
    { path: '/any-user', component: { render: () => null }, meta: { requiresAuth: true } },
    { path: '/empty-roles', component: { render: () => null }, meta: { requiresAuth: true, role: [] } },
  ],
}));
vi.mock('@/modules/company/routes', () => ({
  default: [{ path: '/public', component: { render: () => null } }],
}));
vi.mock('@/modules/worker/routes', () => ({
  default: [
    { path: '/open-positions', name: 'open-positions', component: { render: () => null }, meta: { resolveUser: true } },
    { path: '/position/:id', component: { render: () => null }, meta: { resolveUser: true } },
  ],
}));
vi.mock('@/modules/landing/routes', () => ({
  default: [
    {
      path: '/landing-page',
      component: { render: () => null },
      meta: { layout: 'landing', title: 'Find Jobs', description: 'Landing description' },
    },
    { path: '/landing-untitled', component: { render: () => null }, meta: { layout: 'landing' } },
    { path: '/titled-portal', component: { render: () => null }, meta: { title: 'Ignored', description: 'Ignored' } },
  ],
}));

const DEFAULT_TITLE = 'Sigook® - Connecting Talent with Opportunity';

function user(): UserProfile {
  return { token_type: 'Bearer', access_token: 't', profile: { email: 'a@b.com', role: 'admin', sub: 'u1', name: 'Ann' } };
}

function meta(selector: string): HTMLMetaElement {
  const el = document.head.querySelector<HTMLMetaElement>(selector);
  if (!el) throw new Error(`missing ${selector}`);
  return el;
}

function canonical(): string | undefined {
  return document.head.querySelector<HTMLLinkElement>('link[rel="canonical"]')?.href;
}

async function flushTimers() {
  await new Promise((resolve) => setTimeout(resolve, 0));
}

beforeEach(async () => {
  document.head.innerHTML = [
    '<meta name="description" content="">',
    '<meta property="og:title" content="">',
    '<meta property="og:description" content="">',
    '<meta property="og:url" content="">',
    '<meta name="twitter:title" content="">',
    '<meta name="twitter:description" content="">',
  ].join('');
  security.getUser.mockReset();
  security.userRoles = [];
  vi.spyOn(window, 'scrollTo').mockImplementation(() => undefined);
  await router.push('/public');
  await flushTimers();
  vi.mocked(window.scrollTo).mockClear();
});

afterEach(() => {
  document.head.innerHTML = '';
});

describe('auth guard', () => {
  it('lets public routes through without asking for the user', async () => {
    await router.push('/forgot-password');

    expect(router.currentRoute.value.path).toBe('/forgot-password');
    expect(security.getUser).not.toHaveBeenCalled();
  });

  it('redirects anonymous users to login with the full return url', async () => {
    security.getUser.mockResolvedValue(null);

    await router.push('/admin-only?tab=users#top');

    expect(router.currentRoute.value.path).toBe('/login');
    expect(router.currentRoute.value.query.returnUrl).toBe('/admin-only?tab=users#top');
  });

  it('sends users without an allowed role to unauthorized', async () => {
    security.getUser.mockResolvedValue(user());
    security.userRoles = ['recruiting'];

    await router.push('/admin-only');

    expect(router.currentRoute.value.path).toBe('/unauthorized');
  });

  it('allows users having any of the route roles', async () => {
    security.getUser.mockResolvedValue(user());
    security.userRoles = ['sales', 'superadmin'];

    await router.push('/admin-only');

    expect(router.currentRoute.value.path).toBe('/admin-only');
  });

  it('compares roles case-sensitively', async () => {
    security.getUser.mockResolvedValue(user());
    security.userRoles = ['Admin'];

    await router.push('/admin-only');

    expect(router.currentRoute.value.path).toBe('/unauthorized');
  });

  it.each([['/any-user'], ['/empty-roles']])('%s only needs a signed-in user', async (path) => {
    security.getUser.mockResolvedValue(user());
    security.userRoles = [];

    await router.push(path);

    expect(router.currentRoute.value.path).toBe(path);
  });

  it('resolves the user on resolveUser routes without requiring it', async () => {
    security.getUser.mockResolvedValue(null);

    await router.push('/open-positions');

    expect(security.getUser).toHaveBeenCalledOnce();
    expect(router.currentRoute.value.path).toBe('/open-positions');
  });

  it('still navigates when resolving the user fails', async () => {
    security.getUser.mockRejectedValue(new Error('network'));

    await router.push('/position/7');

    expect(router.currentRoute.value.path).toBe('/position/7');
  });

  it('routes unknown paths to not-found', async () => {
    await router.push('/does/not/exist');

    expect(router.currentRoute.value.name).toBe('not-found');
  });
});

describe('scroll handling', () => {
  it('scrolls to the top after a regular navigation', async () => {
    await router.push('/forgot-password');
    await flushTimers();

    expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
  });

  it('does not scroll to the top when leaving open positions', async () => {
    security.getUser.mockResolvedValue(null);
    await router.push('/open-positions');
    await flushTimers();
    vi.mocked(window.scrollTo).mockClear();

    await router.push('/position/3');
    await flushTimers();

    expect(window.scrollTo).not.toHaveBeenCalledWith(0, 0);
  });

  it('scrollBehavior targets the hash with a header offset', () => {
    const scrollBehavior = router.options.scrollBehavior as RouterScrollBehavior;
    const to = { hash: '#faq' } as RouteLocationNormalized;
    const from = { hash: '' } as RouteLocationNormalized;

    expect(scrollBehavior(to, from, null)).toEqual({ el: '#faq', top: 90, behavior: 'smooth' });
    expect(scrollBehavior(from, to, null)).toBe(false);
  });
});

describe('page meta', () => {
  it('uses the landing title and description on landing pages', async () => {
    await router.push('/landing-page');

    expect(document.title).toBe('Find Jobs | Sigook');
    expect(meta('meta[name="description"]').content).toBe('Landing description');
    expect(meta('meta[property="og:title"]').content).toBe('Find Jobs | Sigook');
    expect(meta('meta[property="og:description"]').content).toBe('Landing description');
    expect(meta('meta[name="twitter:title"]').content).toBe('Find Jobs | Sigook');
    expect(meta('meta[name="twitter:description"]').content).toBe('Landing description');
  });

  it('falls back to defaults on a landing page without title', async () => {
    await router.push('/landing-untitled');

    expect(document.title).toBe(DEFAULT_TITLE);
    expect(meta('meta[name="description"]').content).toContain('Sigook connects job seekers');
  });

  it('ignores title meta outside the landing layout', async () => {
    await router.push('/titled-portal');

    expect(document.title).toBe(DEFAULT_TITLE);
    expect(meta('meta[name="description"]').content).not.toBe('Ignored');
  });

  it('sets the canonical and og:url to the www origin without query', async () => {
    await router.push('/landing-page?utm=x');

    expect(canonical()).toBe('https://www.sigook.com/landing-page');
    expect(meta('meta[property="og:url"]').content).toBe('https://www.sigook.com/landing-page');
  });

  it('reuses the existing canonical link', async () => {
    await router.push('/landing-page');
    await router.push('/landing-untitled');

    expect(document.head.querySelectorAll('link[rel="canonical"]')).toHaveLength(1);
    expect(canonical()).toBe('https://www.sigook.com/landing-untitled');
  });

  it('does not set a canonical on not-found pages', async () => {
    document.head.innerHTML = '';

    await router.push('/missing-page');

    expect(canonical()).toBeUndefined();
    expect(document.title).toBe(DEFAULT_TITLE);
  });
});
