import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import CompanyDetailTab from '@/modules/agency/recruiting/clients/components/CompanyDetailTab.vue';
import { getAgencyCompanyContactPerson, getAgencyCompanyLocation } from '@/modules/agency/recruiting/clients/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany, createCompanySummary } from '@/testing/agencyCompanyFixtures';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[]) => ({ name, props, emits, template: `<div class="${name}-stub" />` }),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanyContactPerson: vi.fn(),
  getAgencyCompanyLocation: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyInfoCard.vue', () => ({ default: stub('CompanyInfoCard', ['company'], ['update:company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyDocumentsCard.vue', () => ({ default: stub('CompanyDocumentsCard', ['profileId'], ['changed']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyInvoicingCard.vue', () => ({ default: stub('CompanyInvoicingCard', ['profileId'], []) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanySettingsCard.vue', () => ({ default: stub('CompanySettingsCard', ['company'], ['update:company']) }));
vi.mock('@/modules/agency/recruiting/clients/components/CompanyNotesCard.vue', () => ({ default: stub('CompanyNotesCard', ['profileId', 'createdAt'], []) }));
vi.mock('@/shared/company-profile/CompanyLocationsCard.vue', () => ({ default: stub('CompanyLocationsCard', ['fetchLocations'], ['showAll', 'loaded']) }));
vi.mock('@/shared/company-profile/CompanyMainContactCard.vue', () => ({ default: stub('CompanyMainContactCard', ['fetchContacts'], ['showAll', 'loaded']) }));

function setup(company: AgencyCompanyProfileDetail = createAgencyCompany()) {
  return mountWithPlugins(CompanyDetailTab, { props: { company } });
}

function sectionLinks(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-index-link').map((l) => l.text().replace(/\s+/g, ' '));
}

function updates(wrapper: ReturnType<typeof setup>) {
  return (wrapper.emitted('update:company') ?? []).map(([value]) => value as AgencyCompanyProfileDetail);
}

beforeEach(() => {
  securityStore.userRoles = ['recruiting'];
});

describe('CompanyDetailTab', () => {
  it('shows the sections without settings for non admins', () => {
    const wrapper = setup();

    expect(sectionLinks(wrapper)).toEqual(['Company', 'About', 'Internalagency only', 'Documents5', 'Invoicing']);
    expect(wrapper.findComponent({ name: 'CompanySettingsCard' }).exists()).toBe(false);
  });

  it('adds the settings card for admins', () => {
    securityStore.userRoles = ['admin'];
    const wrapper = setup();

    expect(sectionLinks(wrapper).at(-1)).toBe('Settings');
    expect(wrapper.findComponent({ name: 'CompanySettingsCard' }).props('company')).toMatchObject({ id: 'company-profile-1' });
  });

  it('shows the about and sanitized internal info', () => {
    const wrapper = setup(createAgencyCompany({ about: 'We move boxes', internalInfo: '<p>Pays late</p><script>alert(1)</script>' }));

    expect(wrapper.find('#company-section-about').text()).toContain('We move boxes');
    const internal = wrapper.find('#company-section-internal .detail-rich-text');
    expect(internal.html()).toContain('<p>Pays late</p>');
    expect(internal.html()).not.toContain('script');
  });

  it('shows empty states for about and internal info', () => {
    const wrapper = setup(createAgencyCompany({ about: '', internalInfo: '' }));

    expect(wrapper.text()).toContain('No description');
    expect(wrapper.text()).toContain('No internal info');
  });

  it('passes the company to the child cards', () => {
    const wrapper = setup(createAgencyCompany({ createdAt: '2026-03-10T15:00:00Z' }));

    expect(wrapper.findComponent({ name: 'CompanyDocumentsCard' }).props('profileId')).toBe('company-profile-1');
    expect(wrapper.findComponent({ name: 'CompanyInvoicingCard' }).props('profileId')).toBe('company-profile-1');
    expect(wrapper.findComponent({ name: 'CompanyNotesCard' }).props()).toEqual({ profileId: 'company-profile-1', createdAt: 'Tue 10 March, 2026' });
  });

  it('fetches the rail locations and contacts of the company', async () => {
    vi.mocked(getAgencyCompanyLocation).mockResolvedValue([]);
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([]);
    const wrapper = setup();

    await (wrapper.findComponent({ name: 'CompanyLocationsCard' }).props('fetchLocations') as () => Promise<unknown>)();
    await (wrapper.findComponent({ name: 'CompanyMainContactCard' }).props('fetchContacts') as () => Promise<unknown>)();

    expect(getAgencyCompanyLocation).toHaveBeenCalledWith('company-profile-1');
    expect(getAgencyCompanyContactPerson).toHaveBeenCalledWith('company-profile-1');
  });

  it('asks to show the locations and contacts tabs', () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'CompanyLocationsCard' }).vm.$emit('showAll');
    wrapper.findComponent({ name: 'CompanyMainContactCard' }).vm.$emit('showAll');

    expect(wrapper.emitted('showTab')).toEqual([['Locations'], ['ContactPerson']]);
  });

  it('forwards company updates from the info and settings cards', () => {
    securityStore.userRoles = ['admin'];
    const wrapper = setup();
    const changed = createAgencyCompany({ website: 'acme.ca' });
    wrapper.findComponent({ name: 'CompanyInfoCard' }).vm.$emit('update:company', changed);
    wrapper.findComponent({ name: 'CompanySettingsCard' }).vm.$emit('update:company', changed);

    expect(updates(wrapper)).toEqual([changed, changed]);
  });

  it('updates the documents count when documents change', () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'CompanyDocumentsCard' }).vm.$emit('changed', -1);

    expect(updates(wrapper)[0].summary).toEqual(createCompanySummary({ documentsCount: 4 }));
  });

  it('updates the locations count only when it differs', () => {
    const wrapper = setup();
    const locations = wrapper.findComponent({ name: 'CompanyLocationsCard' });
    locations.vm.$emit('loaded', 2);
    locations.vm.$emit('loaded', 3);

    expect(updates(wrapper).map((c) => c.summary?.locationsCount)).toEqual([3]);
  });

  it('ignores count changes without a summary', () => {
    const wrapper = setup(createAgencyCompany({ summary: null }));
    wrapper.findComponent({ name: 'CompanyDocumentsCard' }).vm.$emit('changed', 1);
    wrapper.findComponent({ name: 'CompanyLocationsCard' }).vm.$emit('loaded', 3);

    expect(updates(wrapper)).toEqual([]);
    expect(sectionLinks(wrapper)).toContain('Documents');
  });
});
