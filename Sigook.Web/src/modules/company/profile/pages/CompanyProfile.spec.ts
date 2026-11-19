import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyProfile from '@/modules/company/profile/pages/CompanyProfile.vue';
import { getCompanyProfile, getCompanyUser } from '@/modules/company/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { CompanyUserModel } from '@/shared/company-profile/types';

const { route, push } = vi.hoisted(() => ({
  route: { query: {} as Record<string, string> },
  push: vi.fn(),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));

const { tabStub } = vi.hoisted(() => ({
  tabStub: (name: string, props: Record<string, unknown> = {}) => ({ name, props, emits: ['loaded', 'contactsLoaded', 'locationsLoaded', 'showTab', 'update:company'], template: `<div class="${name}" />` }),
}));

vi.mock('@/modules/company/profile/api', () => ({ getCompanyProfile: vi.fn(), getCompanyUser: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/company/profile/components/CompanyProfileTab.vue', () => ({
  default: tabStub('CompanyProfileTab', { company: Object, locationsVersion: Number, contactsVersion: Number }),
}));
vi.mock('@/modules/company/profile/components/ProfileLocation.vue', () => ({ default: tabStub('ProfileLocation') }));
vi.mock('@/modules/company/profile/components/ProfileContact.vue', () => ({ default: tabStub('ProfileContact') }));
vi.mock('@/modules/company/profile/components/CompanyUsers.vue', () => ({ default: tabStub('CompanyUsers', { companyData: Object }) }));

const user: CompanyUserModel = {
  id: 'user-1', companyId: 'company-1', createdAt: '2026-01-01T00:00:00', email: 'ann@acme.com',
  name: 'Ann', lastname: 'Lee', position: 'Manager', mobileNumber: '',
};

async function setup(tab?: string) {
  vi.mocked(getCompanyProfile).mockResolvedValue(createAgencyCompany());
  vi.mocked(getCompanyUser).mockResolvedValue([user, { ...user, id: 'user-2' }]);
  route.query = tab ? { tab } : {};
  push.mockResolvedValue(undefined);
  const wrapper = mountWithPlugins(CompanyProfile);
  await flushPromises();
  return { wrapper };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

function kpis(wrapper: Wrapper) {
  return wrapper.findAll('.detail-kpi').map((k) => [k.find('dt').text(), k.find('dd').text()]);
}

function tabLabels(wrapper: Wrapper) {
  return wrapper.findAll('.tabs li').map((li) => li.text());
}

describe('CompanyProfile', () => {
  it('shows the company header and opens the profile tab', async () => {
    const { wrapper } = await setup();

    expect(wrapper.find('.detail-header-title').text()).toBe('acme logistics');
    expect(wrapper.find('.detail-header-meta').text()).toBe('#501 · Logistics');
    expect(kpis(wrapper)).toEqual([['Member since', 'Tue 10 March, 2026']]);
    expect(tabLabels(wrapper)).toEqual(['Profile', 'Locations', 'Contacts', 'Users2']);
    expect(wrapper.findComponent({ name: 'CompanyProfileTab' }).props('company')).toEqual(createAgencyCompany());
    expect(wrapper.findComponent({ name: 'ProfileLocation' }).exists()).toBe(false);
  });

  it('counts locations and contacts reported by the profile tab', async () => {
    const { wrapper } = await setup();
    const tab = wrapper.findComponent({ name: 'CompanyProfileTab' });
    tab.vm.$emit('locationsLoaded', 3);
    tab.vm.$emit('contactsLoaded', 4);
    await flushPromises();

    expect(kpis(wrapper)).toEqual([['Locations', '3'], ['Contacts', '4'], ['Member since', 'Tue 10 March, 2026']]);
    expect(tabLabels(wrapper)).toEqual(['Profile', 'Locations3', 'Contacts4', 'Users2']);
  });

  it('opens the tab named in the query', async () => {
    const { wrapper } = await setup('Contacts');

    expect(wrapper.findComponent({ name: 'ProfileContact' }).exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'CompanyProfileTab' }).exists()).toBe(true);
  });

  it('falls back to the profile tab for an unknown query', async () => {
    const { wrapper } = await setup('Billing');

    expect(wrapper.findComponent({ name: 'ProfileContact' }).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'ProfileLocation' }).exists()).toBe(false);
  });

  it('opens a tab requested by the profile tab and keeps it in the url', async () => {
    const { wrapper } = await setup();
    wrapper.findComponent({ name: 'CompanyProfileTab' }).vm.$emit('showTab', 'Locations');
    await flushPromises();

    const locations = wrapper.findComponent({ name: 'ProfileLocation' });
    expect(locations.exists()).toBe(true);
    expect(push).toHaveBeenCalledWith({ path: '/company-profile', query: { tab: 'Locations' } });

    locations.vm.$emit('loaded', 5);
    await flushPromises();
    expect(wrapper.findComponent({ name: 'CompanyProfileTab' }).props('locationsVersion')).toBe(1);
    expect(kpis(wrapper)[0]).toEqual(['Locations', '5']);
  });

  it('bumps the contacts version when the contacts tab reloads', async () => {
    const { wrapper } = await setup('Contacts');
    wrapper.findComponent({ name: 'ProfileContact' }).vm.$emit('loaded', 2);
    await flushPromises();

    expect(wrapper.findComponent({ name: 'CompanyProfileTab' }).props('contactsVersion')).toBe(1);
  });

  it('hides the company header on the users tab and tracks its count', async () => {
    const { wrapper } = await setup('CompanyUsers');
    const users = wrapper.findComponent({ name: 'CompanyUsers' });
    users.vm.$emit('loaded', 7);
    await flushPromises();

    expect(wrapper.find('.detail-header').exists()).toBe(false);
    expect(tabLabels(wrapper).at(-1)).toBe('Users7');
  });

  it('shows the errors when loading fails', async () => {
    vi.mocked(getCompanyProfile).mockRejectedValue({ data: 'profile failed' });
    const usersError = new Error('users failed');
    vi.mocked(getCompanyUser).mockRejectedValue(usersError);
    route.query = {};
    const wrapper = mountWithPlugins(CompanyProfile);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('profile failed');
    expect(showAlertError).toHaveBeenCalledWith(usersError);
    expect(wrapper.find('.detail-page').exists()).toBe(false);
  });
});
