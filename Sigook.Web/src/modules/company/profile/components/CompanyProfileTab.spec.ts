import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import CompanyProfileTab from '@/modules/company/profile/components/CompanyProfileTab.vue';
import { getContactPeople, getProfileLocations } from '@/modules/company/profile/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';

const { cardStub } = vi.hoisted(() => ({
  cardStub: (name: string, prop: string) => ({ name, props: { [prop]: Function }, emits: ['loaded', 'showAll'], template: `<div class="${name}" />` }),
}));

vi.mock('@/modules/company/profile/api', () => ({ getContactPeople: vi.fn(), getProfileLocations: vi.fn() }));
vi.mock('@/shared/company-profile/CompanyLocationsCard.vue', () => ({ default: cardStub('CompanyLocationsCard', 'fetchLocations') }));
vi.mock('@/shared/company-profile/CompanyMainContactCard.vue', () => ({ default: cardStub('CompanyMainContactCard', 'fetchContacts') }));
vi.mock('@/modules/company/profile/components/ProfileBusiness.vue', () => ({
  default: { name: 'ProfileBusiness', props: { companyData: Object }, emits: ['update:companyData', 'saved'], template: '<div class="profile-business" />' },
}));

function setup(overrides: Partial<CompanyProfileDetail> = {}) {
  return mountWithPlugins(CompanyProfileTab, { props: { company: createAgencyCompany(overrides) } });
}

function fields(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-field').map((f) => [f.find('dt').text(), f.find('dd').text()]);
}

describe('CompanyProfileTab', () => {
  it('shows the business facts', () => {
    const wrapper = setup({ phoneExt: 12, fax: '416 555-0199', faxExt: 3 });

    expect(fields(wrapper)).toEqual([
      ['Company name', 'ACME LOGISTICS'],
      ['Industry', 'Logistics'],
      ['Website', 'www.acme.com'],
      ['Phone', '416 555-0100 Ext. 12'],
      ['Fax', '416 555-0199 ext 3'],
      ['Email', 'ops@acme.com'],
    ]);
    expect(wrapper.find('.detail-field a[target="_blank"]').attributes('href')).toBe('https://www.acme.com');
  });

  it('uses dashes for missing facts', () => {
    const wrapper = setup({ fullName: '', industry: null, website: '', phone: null, fax: null, email: '' });

    expect(fields(wrapper).map(([, value]) => value)).toEqual(['—', '—', '—', '—', '—', '—']);
  });

  it('wires the rail cards to the profile api and relays their events', () => {
    const wrapper = setup();
    const locations = wrapper.findComponent({ name: 'CompanyLocationsCard' });
    const contacts = wrapper.findComponent({ name: 'CompanyMainContactCard' });

    expect(locations.props('fetchLocations')).toBe(getProfileLocations);
    expect(contacts.props('fetchContacts')).toBe(getContactPeople);
    locations.vm.$emit('loaded', 2);
    contacts.vm.$emit('loaded', 3);
    locations.vm.$emit('showAll');
    contacts.vm.$emit('showAll');

    expect(wrapper.emitted('locationsLoaded')).toEqual([[2]]);
    expect(wrapper.emitted('contactsLoaded')).toEqual([[3]]);
    expect(wrapper.emitted('showTab')).toEqual([['Locations'], ['Contacts']]);
  });

  it('edits the business in a modal and closes it once saved', async () => {
    const company = createAgencyCompany();
    const wrapper = mountWithPlugins(CompanyProfileTab, { props: { company } });
    expect(wrapper.findComponent({ name: 'ProfileBusiness' }).exists()).toBe(false);

    await wrapper.find('.detail-link').trigger('click');
    const form = wrapper.findComponent({ name: 'ProfileBusiness' });
    expect(form.props('companyData')).toEqual(company);

    const updated = { ...company, fullName: 'ACME 2' };
    form.vm.$emit('update:companyData', updated);
    form.vm.$emit('saved');
    await nextTick();

    expect(wrapper.emitted('update:company')).toEqual([[updated]]);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });
});
