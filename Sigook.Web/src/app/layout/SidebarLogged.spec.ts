import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { createPinia } from 'pinia';
import SidebarLogged from '@/app/layout/SidebarLogged.vue';
import { getMyProfile } from '@/modules/worker/profile/api';
import { getAccessibleAgencies, getAgencyProfile } from '@/modules/agency/profile/api';
import { getCompanyProfile } from '@/modules/company/profile/api';
import { getAccount } from '@/shared/api/accountApi';
import { createTestRouter } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { AgencyDetail } from '@/modules/agency/profile/types';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';
import type { AccountResponse, UserProfile } from '@/shared/types/security';
import type { NotificationGroup } from '@/modules/agency/shared/notifications/types';
import type { WorkerRequirement } from '@/shared/worker-profile/types';

const state = await vi.hoisted(async () => {
  const { ref } = await import('vue');
  return {
    isTouch: ref(false),
    isCompactDesktop: ref(false),
    backTo: ref<string | null>(null),
    grouped: ref<NotificationGroup[]>([]),
    hasNotifications: ref(false),
    loadNotifications: vi.fn<() => Promise<void>>(),
  };
});

const security = vi.hoisted(() => ({
  userRoles: [] as string[],
  getUser: vi.fn<() => Promise<UserProfile | null>>(),
  signOut: vi.fn<() => Promise<void>>(),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));
vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: state.isTouch, isCompactDesktop: state.isCompactDesktop }),
}));
vi.mock('@/shared/composables/useMobileBack', () => ({ useMobileBack: () => state.backTo }));
vi.mock('@/modules/agency/shared/notifications/useNotifications', () => ({
  useNotifications: () => ({ grouped: state.grouped, hasNotifications: state.hasNotifications, load: state.loadNotifications }),
}));
vi.mock('@/modules/worker/profile/api', () => ({ getMyProfile: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAgencyProfile: vi.fn(), getAccessibleAgencies: vi.fn() }));
vi.mock('@/modules/company/profile/api', () => ({ getCompanyProfile: vi.fn() }));
vi.mock('@/shared/api/accountApi', () => ({ getAccount: vi.fn() }));
vi.mock('@/modules/agency/profile/components/AttendanceClockButton.vue', () => ({
  default: { name: 'AttendanceClockButton', template: '<div class="attendance-stub" />' },
}));

const requirement: WorkerRequirement = 'SocialInsuranceNumber';

async function setup(roles: string[], path = '/') {
  security.userRoles = roles;
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  const push = vi.spyOn(router, 'push');
  const wrapper = mount(SidebarLogged, { attachTo: document.body, global: { plugins: [Buefy, router, createPinia()] } });
  await flushPromises();
  return { wrapper, router, push };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

function menuLabels(wrapper: Wrapper) {
  return wrapper.findAll('.sidebar-nav a').map((a) => a.text().trim());
}

function dropdownItem(text: string) {
  const item = [...document.body.querySelectorAll<HTMLElement>('.dropdown-item')].find((el) => el.textContent.trim().startsWith(text));
  if (!item) throw new Error(`Dropdown item ${text} not found`);
  return item;
}

function profileHref() {
  return document.body.querySelector('.dropdown-menu .has-link a')?.getAttribute('href');
}

describe('SidebarLogged', () => {
  beforeEach(() => {
    state.isTouch.value = false;
    state.isCompactDesktop.value = false;
    state.backTo.value = null;
    state.grouped.value = [];
    state.hasNotifications.value = false;
    state.loadNotifications.mockResolvedValue();
    vi.mocked(getAgencyProfile).mockResolvedValue({ agencyType: 1 } as AgencyDetail);
    vi.mocked(getAccessibleAgencies).mockResolvedValue([]);
    vi.mocked(getAccount).mockResolvedValue({ name: 'Ann Admin' } as AccountResponse);
    vi.mocked(getCompanyProfile).mockResolvedValue({ fullName: 'Acme Corp' } as CompanyProfileDetail);
    vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ firstName: 'Wes', lastName: 'Worker', missingRequirements: [] }));
  });

  it('builds the admin menu and loads the agency', async () => {
    const { wrapper } = await setup(['admin'], '/recruiting/requests');

    expect(wrapper.find('.sidebar-user-name').text()).toBe('Ann Admin');
    expect(wrapper.find('.sidebar-user-avatar text').text()).toBe('AN');
    expect(wrapper.findAll('.sidebar-group-label').map((g) => g.text())).toEqual(['Accounting', 'Recruiting', 'Sales']);
    expect(menuLabels(wrapper)).toEqual(expect.arrayContaining(['Requests', 'Weekly Board', 'Invoices', 'Agencies']));
    expect(state.loadNotifications).toHaveBeenCalledOnce();
    expect(wrapper.find('.attendance-stub').exists()).toBe(true);
    expect(profileHref()).toBe('/agency-profile');
  });

  it('expands the group of the active route', async () => {
    const { wrapper } = await setup(['admin'], '/recruiting/requests/abc');

    const active = wrapper.findAll('.sidebar-nav a.is-active').map((a) => a.text().trim());
    expect(active).toEqual(expect.arrayContaining(['Recruiting', 'Requests']));
    expect(active).not.toContain('Weekly Board');
  });

  it('hides the agencies menu for non master agencies', async () => {
    vi.mocked(getAgencyProfile).mockResolvedValue({ agencyType: 2 } as AgencyDetail);
    const { wrapper } = await setup(['superadmin']);

    expect(menuLabels(wrapper)).not.toContain('Agencies');
  });

  it('shows only the sales menu for sales staff', async () => {
    const { wrapper } = await setup(['sales']);

    expect(wrapper.findAll('.sidebar-group-label').map((g) => g.text())).toEqual(['Sales']);
    expect(menuLabels(wrapper)).not.toContain('Agencies');
  });

  it('lists the agency notifications and opens their route', async () => {
    state.hasNotifications.value = true;
    state.grouped.value = [{ type: 'missingDocuments', label: 'Missing documents', route: '/recruiting/workers', items: [] } as unknown as NotificationGroup];
    const { push } = await setup(['recruiting']);

    expect(document.body.querySelector('.sidebar-user-notification-label')?.textContent).toBe('Missing documents');
    expect(document.body.textContent).not.toContain('Nothing to review');
    dropdownItem('Missing documents').click();
    expect(push).toHaveBeenCalledWith('/recruiting/workers');
  });

  it('says there is nothing to review', async () => {
    await setup(['recruiting']);

    expect(document.body.textContent).toContain('Nothing to review');
  });

  it('shows the company menu', async () => {
    const { wrapper } = await setup(['company']);

    expect(menuLabels(wrapper)).toEqual(['Staff Requests', 'Accounting']);
    expect(wrapper.find('.sidebar-user-name').text()).toBe('Acme Corp');
    expect(state.loadNotifications).not.toHaveBeenCalled();
    expect(wrapper.find('.attendance-stub').exists()).toBe(false);
    expect(document.body.textContent).not.toContain('Notifications');
    expect(profileHref()).toBe('/company-profile');
  });

  it('uses the signed-in name for company users', async () => {
    security.getUser.mockResolvedValue({ token_type: 'Bearer', access_token: 't', profile: { email: 'c@acme.com', role: 'company.user', sub: 'u2', name: 'Cora User' } });
    const { wrapper } = await setup(['company.user']);

    expect(menuLabels(wrapper)).toEqual(['Staff Requests']);
    expect(wrapper.find('.sidebar-user-name').text()).toBe('Cora User');
    expect(profileHref()).toBe('/company-user-profile');
  });

  it('keeps the profile link empty when the company user is gone', async () => {
    security.getUser.mockResolvedValue(null);
    const { wrapper } = await setup(['company.user']);

    expect(wrapper.find('.sidebar-user-name').text()).toBe('');
  });

  it('flags missing worker requirements', async () => {
    vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ firstName: 'Wes', lastName: 'Worker', missingRequirements: [requirement, 'ProfileImage'] }));
    const { wrapper } = await setup(['worker']);

    expect(menuLabels(wrapper)).toEqual(['Jobs', 'Activity']);
    expect(wrapper.find('.sidebar-user-name').text()).toBe('Wes Worker');
    expect(wrapper.find('.mobile-topbar-burger').attributes('aria-label')).toBe('Open menu, 2 profile items missing');
    expect(wrapper.find('.sidebar-user-trigger').attributes('title')).toBe('Wes Worker · 2 profile items missing');
    expect(document.body.querySelector('.sidebar-user-notification-count.is-danger')?.textContent).toBe('2');
  });

  it('uses the singular label for one missing requirement', async () => {
    vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ missingRequirements: [requirement] }));
    const { wrapper } = await setup(['worker']);

    expect(wrapper.find('.mobile-topbar-burger').attributes('aria-label')).toBe('Open menu, 1 profile item missing');
  });

  it('has plain labels without missing requirements', async () => {
    const { wrapper } = await setup(['worker']);

    expect(wrapper.find('.mobile-topbar-burger').attributes('aria-label')).toBe('Open menu');
    expect(wrapper.find('.sidebar-user-trigger').attributes('title')).toBe('Wes Worker');
  });

  it('shows a back link on mobile pages that register one', async () => {
    state.backTo.value = '/worker-requests';
    const { wrapper } = await setup(['worker']);

    expect(wrapper.find('a.mobile-topbar-burger').attributes('href')).toBe('/worker-requests');
    expect(wrapper.find('a.mobile-topbar-burger').attributes('aria-label')).toBe('Go back');
  });

  it('collapses into a rail and expands again on desktop', async () => {
    const { wrapper } = await setup(['admin']);
    await wrapper.find('.sidebar-toggle').trigger('click');

    expect(wrapper.find('.sidebar-logged').classes()).toContain('is-collapsed');
    expect(wrapper.findAll('.sidebar-rail-group-label').map((g) => g.text())).toEqual(['Accounting', 'Recruiting', 'Sales']);
    expect(wrapper.find('.sidebar-toggle').attributes('title')).toBe('Expand');
    await wrapper.find('.sidebar-toggle').trigger('click');
    expect(wrapper.find('.sidebar-rail').exists()).toBe(false);
  });

  it('follows the compact desktop breakpoint', async () => {
    state.isCompactDesktop.value = true;
    const { wrapper } = await setup(['admin']);
    expect(wrapper.find('.sidebar-logged').classes()).toContain('is-collapsed');

    state.isCompactDesktop.value = false;
    await flushPromises();
    expect(wrapper.find('.sidebar-logged').classes()).not.toContain('is-collapsed');
  });

  it('opens and closes the mobile drawer', async () => {
    state.isTouch.value = true;
    state.isCompactDesktop.value = true;
    const { wrapper } = await setup(['worker']);
    expect(wrapper.find('.sidebar-logged').classes()).not.toContain('is-collapsed');

    await wrapper.find('button.mobile-topbar-burger').trigger('click');
    expect(wrapper.find('.sidebar-logged').classes()).toContain('is-open');
    expect(document.body.classList.contains('has-drawer-open')).toBe(true);

    await wrapper.find('.sidebar-overlay').trigger('click');
    expect(wrapper.find('.sidebar-logged').classes()).not.toContain('is-open');
    expect(document.body.classList.contains('has-drawer-open')).toBe(false);

    await wrapper.find('button.mobile-topbar-burger').trigger('click');
    await wrapper.find('.sidebar-toggle[aria-label="Close menu"]').trigger('click');
    expect(wrapper.find('.sidebar-overlay').exists()).toBe(false);
  });

  it('closes the drawer on navigation and when leaving touch mode', async () => {
    state.isTouch.value = true;
    const { wrapper, router } = await setup(['worker']);
    await wrapper.find('button.mobile-topbar-burger').trigger('click');
    await router.push('/worker-activity');
    await flushPromises();
    expect(wrapper.find('.sidebar-logged').classes()).not.toContain('is-open');

    await wrapper.find('button.mobile-topbar-burger').trigger('click');
    state.isTouch.value = false;
    await flushPromises();
    expect(wrapper.find('.sidebar-logged').classes()).not.toContain('is-open');
  });

  it('removes the drawer class on unmount', async () => {
    state.isTouch.value = true;
    const { wrapper } = await setup(['worker']);
    await wrapper.find('button.mobile-topbar-burger').trigger('click');
    wrapper.unmount();

    expect(document.body.classList.contains('has-drawer-open')).toBe(false);
  });

  it('signs out and goes home', async () => {
    security.signOut.mockResolvedValue();
    const { push } = await setup(['worker']);
    dropdownItem('Log Out').click();
    await flushPromises();

    expect(security.signOut).toHaveBeenCalledOnce();
    expect(push).toHaveBeenCalledWith('/');
  });
});
