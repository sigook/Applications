import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import ClientDetail from '@/modules/agency/recruiting/clients/pages/ClientDetail.vue';
import { getAgencyCompany } from '@/modules/agency/recruiting/clients/api';
import { useAgencyStore } from '@/modules/agency/store';
import { CompanyStatus } from '@/shared/constants/enums';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany, createCompanySummary } from '@/testing/agencyCompanyFixtures';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';

const { route, push, securityStore, stub } = vi.hoisted(() => ({
  route: { path: '', params: { id: 'company-profile-1' } as Record<string, string>, query: {} as Record<string, string> },
  push: vi.fn(),
  securityStore: { userRoles: [] as string[] },
  stub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}-stub" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompany: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyDetailTab.vue', () => ({ default: stub('CompanyDetailTab', ['company'], ['update:company', 'showTab']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyLocationsGrid.vue', () => ({ default: stub('CompanyLocationsGrid', ['profileId'], ['changed']) }));
vi.mock('@/modules/agency/recruiting/clients/components/UserList.vue', () => ({ default: stub('UserList', ['company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/ContactPersonList.vue', () => ({ default: stub('ContactPersonList', ['company'], ['changed']) }));
vi.mock('@/modules/agency/recruiting/clients/components/JobPositionList.vue', () => ({ default: stub('JobPositionList', ['company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyRequests.vue', () => ({ default: stub('CompanyRequests', ['company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyWorkers.vue', () => ({ default: stub('CompanyWorkers', ['company']) }));
vi.mock('@/modules/agency/sales/clients/components/CompanyInteractions.vue', () => ({ default: stub('CompanyInteractions', ['company']) }));
vi.mock('@/modules/agency/sales/clients/components/CompanyDeals.vue', () => ({ default: stub('CompanyDeals', ['company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyUpdateLogo.vue', () => ({ default: stub('CompanyUpdateLogo', ['profileId', 'logo'], ['updated', 'cancel']) }));

let pinia: Pinia;

async function setup(overrides: Partial<AgencyCompanyProfileDetail> = {}) {
  vi.mocked(getAgencyCompany).mockResolvedValue(createAgencyCompany(overrides));
  const wrapper = mountWithPlugins(ClientDetail, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function tabs(wrapper: Wrapper) {
  return wrapper.findAll('.tabs li').map((li) => li.text().replace(/\s+/g, ' '));
}

function kpis(wrapper: Wrapper) {
  return wrapper.findAll('.detail-kpi').map((k) => [k.find('dt').text(), k.find('.detail-kpi-value').text(), k.find('.detail-kpi-hint').exists() ? k.find('.detail-kpi-hint').text() : '']);
}

function chips(wrapper: Wrapper) {
  return wrapper.findAll('.detail-chip').map((c) => c.text());
}

function button(wrapper: Wrapper, text: string) {
  return wrapper.findAll('button').find((b) => b.text() === text);
}

async function changeTab(wrapper: Wrapper, tab: string) {
  wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', tab);
  await flushPromises();
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  route.path = '/recruiting/companies/company-profile-1';
  route.query = {};
  securityStore.userRoles = ['recruiting'];
});

describe('ClientDetail', () => {
  it('loads the routed company and shows its header', async () => {
    const wrapper = await setup();

    expect(getAgencyCompany).toHaveBeenCalledExactlyOnceWith('company-profile-1');
    expect(wrapper.find('.detail-header-title').text()).toBe('acme logistics');
    expect(wrapper.find('.detail-header-meta').text()).toBe('#501 · Logistics');
    expect(chips(wrapper)).toEqual(['Client']);
    expect(wrapper.find('.detail-header-logo img').attributes('src')).toBe('https://files/logo.png');
    expect(wrapper.find('.page-header-breadcrumb').text()).toContain('Recruiting');
  });

  it('shows the kpis', async () => {
    const wrapper = await setup({
      paidHolidays: true,
      summary: createCompanySummary({ asapRequestsCount: 2, salesRepresentativeName: 'Rita Sales' }),
    });

    expect(kpis(wrapper)).toEqual([
      ['Open requests', '4', '2 ASAP'],
      ['Workers', '12', 'Assigned'],
      ['Roles', '3', ''],
      ['Overtime after', '44 h', 'Paid holidays'],
      ['Payment terms', 'Net 30', 'Days'],
      ['Sales owner', 'Rita Sales', ''],
      ['Created', 'Tue 10 March, 2026', ''],
    ]);
  });

  it('shows no kpis without a summary', async () => {
    const wrapper = await setup({ summary: null });

    expect(kpis(wrapper)).toEqual([]);
    expect(tabs(wrapper)).toContain('Roles0');
  });

  it('lists the tabs with their counts and opens Detail by default', async () => {
    const wrapper = await setup();

    expect(tabs(wrapper)).toEqual(['Detail', 'Roles3', 'Requests4', 'Workers12', 'Locations2', 'Contacts2', 'Users1']);
    expect(wrapper.findComponent({ name: 'CompanyDetailTab' }).props('company')).toMatchObject({ id: 'company-profile-1' });
    expect(wrapper.findComponent({ name: 'JobPositionList' }).exists()).toBe(false);
  });

  it('hides the requests and roles for non admins when the company requires permission', async () => {
    const wrapper = await setup({ requiresPermissionToSeeRequests: true });

    expect(tabs(wrapper)).toEqual(['Detail', 'Workers12', 'Locations2', 'Contacts2', 'Users1']);
    expect(kpis(wrapper).map(([label]) => label)).not.toContain('Open requests');
    expect(kpis(wrapper).map(([label]) => label)).not.toContain('Roles');
    expect(chips(wrapper)).toContain('Needs permission to see requests');
  });

  it('keeps the requests and roles for admins even when permission is required', async () => {
    securityStore.userRoles = ['admin'];
    const wrapper = await setup({ requiresPermissionToSeeRequests: true });

    expect(tabs(wrapper)).toContain('Roles3');
    expect(tabs(wrapper)).toContain('Requests4');
  });

  it('shows the agency and vaccination chips', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }, { id: 'agency-2', name: 'Sigook' }]);
    const wrapper = await setup({ vaccinationRequired: true });

    expect(chips(wrapper)).toEqual(['Client', 'Covenant', 'Vaccination required']);
  });

  it('offers creating a request only for clients', async () => {
    const client = await setup();
    await button(client, 'Create request')?.trigger('click');
    expect(push).toHaveBeenCalledWith({ path: '/recruiting/requests/create/company-profile-1' });

    const lead = await setup({ companyStatus: CompanyStatus.Lead });
    expect(button(lead, 'Create request')).toBeUndefined();
  });

  it('edits the company', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Edit company')?.trigger('click');

    expect(push).toHaveBeenCalledWith({ path: '/recruiting/companies/update/company-profile-1' });
  });

  it('opens the tab from the query string', async () => {
    route.query = { tab: 'Workers' };
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'BTabs' }).props('modelValue')).toBe('Workers');
    expect(wrapper.findComponent({ name: 'CompanyWorkers' }).exists()).toBe(true);
  });

  it('ignores the removed Settings tab in the query string', async () => {
    route.query = { tab: 'Settings' };
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'BTabs' }).props('modelValue')).toBe('Detail');
  });

  it('mounts a tab when it is visited and records it in the url', async () => {
    const wrapper = await setup();
    await changeTab(wrapper, 'JobPosition');

    expect(wrapper.findComponent({ name: 'JobPositionList' }).exists()).toBe(true);
    expect(push).toHaveBeenCalledWith({ path: '/recruiting/companies/company-profile-1', query: { tab: 'JobPosition' } });

    await changeTab(wrapper, 'Detail');
    await changeTab(wrapper, 'JobPosition');
    expect(wrapper.findAllComponents({ name: 'JobPositionList' })).toHaveLength(1);
  });

  it('switches tabs when the detail tab asks for it', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'CompanyDetailTab' }).vm.$emit('showTab', 'Locations');
    await flushPromises();

    expect(wrapper.findComponent({ name: 'CompanyLocationsGrid' }).props('profileId')).toBe('company-profile-1');
  });

  it('updates the location and contact counts', async () => {
    const wrapper = await setup();
    await changeTab(wrapper, 'Locations');
    await changeTab(wrapper, 'ContactPerson');
    wrapper.findComponent({ name: 'CompanyLocationsGrid' }).vm.$emit('changed', 9);
    wrapper.findComponent({ name: 'ContactPersonList' }).vm.$emit('changed', 7);
    await flushPromises();

    expect(tabs(wrapper)).toContain('Locations9');
    expect(tabs(wrapper)).toContain('Contacts7');
  });

  it('applies company updates from the detail tab', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'CompanyDetailTab' }).vm.$emit('update:company', createAgencyCompany({ fullName: 'ACME RENAMED' }));
    await flushPromises();

    expect(wrapper.find('.detail-header-title').text()).toBe('acme renamed');
  });

  it('shows the sales tabs only in the sales view for sales users', async () => {
    route.path = '/sales/companies/company-profile-1';
    securityStore.userRoles = ['sales'];
    const wrapper = await setup();

    expect(tabs(wrapper)).toEqual(expect.arrayContaining(['Interactions', 'Deals']));
    await button(wrapper, 'Edit company')?.trigger('click');
    expect(push).toHaveBeenCalledWith({ path: '/sales/companies/update/company-profile-1' });

    await changeTab(wrapper, 'Deals');
    expect(wrapper.findComponent({ name: 'CompanyDeals' }).exists()).toBe(true);
    await changeTab(wrapper, 'Interactions');
    expect(wrapper.findComponent({ name: 'CompanyInteractions' }).exists()).toBe(true);
  });

  it('hides the sales tabs in the recruiting view', async () => {
    securityStore.userRoles = ['sales'];
    const wrapper = await setup();

    expect(tabs(wrapper)).not.toContain('Deals');
  });

  it('changes the logo and reloads the company', async () => {
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Change logo"]').trigger('click');
    const logo = wrapper.findComponent({ name: 'CompanyUpdateLogo' });
    expect(logo.props()).toEqual({ profileId: 'company-profile-1', logo: { id: 'logo-1', pathFile: 'https://files/logo.png' } });

    logo.vm.$emit('updated');
    await flushPromises();

    expect(getAgencyCompany).toHaveBeenCalledTimes(2);
    expect(wrapper.findAllComponents({ name: 'BModal' }).at(-1)?.props('modelValue')).toBe(false);
  });

  it('closes the logo modal on cancel', async () => {
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Change logo"]').trigger('click');
    wrapper.findComponent({ name: 'CompanyUpdateLogo' }).vm.$emit('cancel');
    await flushPromises();

    expect(wrapper.findAllComponents({ name: 'BModal' }).at(-1)?.props('modelValue')).toBe(false);
    expect(getAgencyCompany).toHaveBeenCalledTimes(1);
  });

  it('shows the error when the company cannot be loaded', async () => {
    const error = new Error('not found');
    vi.mocked(getAgencyCompany).mockRejectedValue(error);
    const wrapper = mountWithPlugins(ClientDetail, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('.detail-header').exists()).toBe(false);
  });
});
