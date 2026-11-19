import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { nextTick } from 'vue';
import Header from '@/modules/landing/layout/Header.vue';
import { createTestRouter } from '@/testing/mountWithPlugins';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  user: null as UserProfile | null,
  userRoles: [] as string[],
  isReady: true,
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));

class FakeIntersectionObserver {
  static instances: FakeIntersectionObserver[] = [];
  readonly observed: Element[] = [];
  readonly disconnect = vi.fn();

  constructor(readonly callback: IntersectionObserverCallback) {
    FakeIntersectionObserver.instances.push(this);
  }

  observe(target: Element): void {
    this.observed.push(target);
  }

  emit(isIntersecting: boolean): void {
    this.callback([{ isIntersecting } as IntersectionObserverEntry], this as unknown as IntersectionObserver);
  }
}

async function setup(path = '/') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  const push = vi.spyOn(router, 'push');
  const wrapper = mount(Header, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, router, push };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

function cta(wrapper: Wrapper) {
  return wrapper.find('.nav__actions button');
}

describe('Header', () => {
  beforeEach(() => {
    security.user = null;
    security.userRoles = [];
    security.isReady = true;
    FakeIntersectionObserver.instances = [];
    vi.stubGlobal('IntersectionObserver', FakeIntersectionObserver);
  });

  it('lists the primary links', async () => {
    const { wrapper } = await setup();

    expect(wrapper.findAll('.nav__links a').map((a) => [a.text(), a.attributes('href')])).toEqual([
      ['Open Positions', '/open-positions'],
      ['Talents', '/talents'],
      ['Employers', '/employers'],
      ['Industries', '/industries'],
      ['Special Projects', '/special-projects'],
      ['About Us', '/about'],
    ]);
  });

  it('sends visitors to the login page', async () => {
    const { wrapper, push } = await setup();

    expect(cta(wrapper).text()).toBe('Sign In');
    await cta(wrapper).trigger('click');
    expect(push).toHaveBeenCalledWith('/login');
  });

  it('sends signed-in users to their portal', async () => {
    security.user = { token_type: 'Bearer', access_token: 't', profile: { email: 'a@b.com', role: 'worker', sub: 'u1', name: 'Ann' } };
    security.userRoles = ['worker'];
    const { wrapper, push } = await setup();

    expect(cta(wrapper).text()).toBe('Go to Portal');
    await cta(wrapper).trigger('click');
    expect(push).toHaveBeenCalledWith('/worker-requests');
  });

  it('disables the button until the session is resolved', async () => {
    security.isReady = false;
    const { wrapper } = await setup();

    expect(cta(wrapper).attributes('disabled')).toBeDefined();
  });

  it('turns solid after scrolling', async () => {
    const { wrapper } = await setup();
    const scrollY = vi.spyOn(window, 'scrollY', 'get').mockReturnValue(120);
    window.dispatchEvent(new Event('scroll'));
    await nextTick();
    expect(wrapper.classes()).toContain('nav--solid');

    scrollY.mockReturnValue(10);
    window.dispatchEvent(new Event('scroll'));
    await nextTick();
    expect(wrapper.classes()).not.toContain('nav--solid');
  });

  it('opens the drawer, locks the page and closes with Escape', async () => {
    const { wrapper } = await setup();
    await wrapper.find('.nav__hamburger').trigger('click');

    expect(wrapper.find('.nav__drawer').exists()).toBe(true);
    expect(wrapper.find('.nav__hamburger').attributes('aria-expanded')).toBe('true');
    expect(wrapper.findAll('.nav__drawer-link')).toHaveLength(6);
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Tab' }));
    await nextTick();
    expect(wrapper.find('.nav__drawer').exists()).toBe(true);

    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }));
    await nextTick();
    expect(wrapper.find('.nav__drawer').exists()).toBe(false);
  });

  it('closes the drawer from the close button, the overlay and a link', async () => {
    const { wrapper } = await setup();
    await wrapper.find('.nav__hamburger').trigger('click');
    await wrapper.find('.nav__drawer-close').trigger('click');
    expect(wrapper.find('.nav__drawer').exists()).toBe(false);

    await wrapper.find('.nav__hamburger').trigger('click');
    await wrapper.find('.nav__drawer').trigger('click');
    expect(wrapper.find('.nav__drawer').exists()).toBe(true);
    await wrapper.find('.nav__overlay').trigger('click');
    expect(wrapper.find('.nav__drawer').exists()).toBe(false);

    await wrapper.find('.nav__hamburger').trigger('click');
    await wrapper.find('.nav__drawer-link').trigger('click');
    expect(wrapper.find('.nav__drawer').exists()).toBe(false);
  });

  it('closes the drawer when signing in from it', async () => {
    const { wrapper, push } = await setup();
    await wrapper.find('.nav__hamburger').trigger('click');
    await wrapper.find('.nav__drawer-actions button').trigger('click');

    expect(push).toHaveBeenCalledWith('/login');
    expect(wrapper.find('.nav__drawer').exists()).toBe(false);
  });

  it('hides its logo while the hero logo is visible', async () => {
    const hero = document.createElement('div');
    hero.className = 'hero__logo';
    document.body.appendChild(hero);
    const { wrapper, router } = await setup();
    const observer = FakeIntersectionObserver.instances.at(0);

    expect(observer?.observed).toEqual([hero]);
    observer?.emit(true);
    await nextTick();
    expect(wrapper.find('.nav__logo').classes()).toContain('nav__logo--hidden');

    hero.remove();
    await router.push('/talents');
    await flushPromises();
    expect(observer?.disconnect).toHaveBeenCalled();
    expect(wrapper.find('.nav__logo').classes()).not.toContain('nav__logo--hidden');
    expect(FakeIntersectionObserver.instances).toHaveLength(1);
  });

  it('stops listening on unmount', async () => {
    const hero = document.createElement('div');
    hero.className = 'hero__logo';
    document.body.appendChild(hero);
    const { wrapper } = await setup();
    const remove = vi.spyOn(window, 'removeEventListener');
    wrapper.unmount();

    expect(remove).toHaveBeenCalledWith('scroll', expect.any(Function));
    expect(FakeIntersectionObserver.instances.at(0)?.disconnect).toHaveBeenCalled();
  });
});
