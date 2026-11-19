import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AgencyProfileTab from '@/modules/agency/profile/components/AgencyProfileTab.vue';
import { getAgencyLocations } from '@/modules/agency/profile/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';
import type { CompanyContactSummary } from '@/shared/company-profile/types';

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[]) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAgencyLocations: vi.fn() }));
vi.mock('@/shared/company-profile/CompanyLocationsCard.vue', () => ({ default: stub('CompanyLocationsCard', ['fetchLocations'], ['showAll']) }));
vi.mock('@/shared/company-profile/CompanyMainContactCard.vue', () => ({ default: stub('CompanyMainContactCard', ['fetchContacts'], ['showAll']) }));
vi.mock('@/modules/agency/profile/components/ProfileBusiness.vue', () => ({ default: stub('ProfileBusiness', ['agencyData'], ['update:agencyData', 'saved']) }));

const agency: AgencyDetail = {
  id: 'agency-1',
  fullName: 'Covenant Staffing',
  hstNumber: '123456789RT0001',
  businessNumber: '123456789',
  phonePrincipal: '416 555-1234',
  phonePrincipalExt: 12,
  webPage: 'covenant.ca',
  wsibGroup: [{ value: 'G1' }, { value: 'G2' }],
  contactInformation: [{ firstName: 'Jane', middleName: 'M', lastName: 'Doe', position: 'Manager', mobileNumber: '416 555-0000', email: 'jane@covenant.ca' }],
};

function setup(data: AgencyDetail = agency) {
  return mountWithPlugins(AgencyProfileTab, { props: { agency: data, locationsVersion: 1 } });
}

type Wrapper = ReturnType<typeof setup>;

function fields(wrapper: Wrapper) {
  return wrapper.findAll('.detail-field').map((f) => [f.find('dt').text(), f.find('dd').text()]);
}

describe('AgencyProfileTab', () => {
  it('shows the business facts', () => {
    const wrapper = setup();

    expect(fields(wrapper)).toEqual([
      ['Agency name', 'Covenant Staffing'],
      ['HST number', '123456789RT0001'],
      ['BN/EIN', '123456789'],
      ['Phone', '416 555-1234 Ext. 12'],
      ['Web page', 'covenant.ca'],
      ['WSIB groups', 'G1, G2'],
    ]);
    expect(wrapper.find('.detail-field a[target="_blank"]').attributes('href')).toBe('https://covenant.ca');
  });

  it('shows dashes for an empty agency', () => {
    const wrapper = setup({ id: 'agency-2', fullName: '' });

    expect(fields(wrapper).map(([, value]) => value)).toEqual(['—', '—', '—', '—', '—', '—']);
  });

  it('fetches the locations and contacts for the rail cards', async () => {
    const locations: AgencyLocationDetail[] = [{ id: 'loc-1', address: '1 King St', city: null, postalCode: 'M5H 1A1' }];
    vi.mocked(getAgencyLocations).mockResolvedValue(locations);
    const wrapper = setup();
    const fetchLocations = wrapper.findComponent({ name: 'CompanyLocationsCard' }).props('fetchLocations') as () => Promise<AgencyLocationDetail[]>;
    const fetchContacts = wrapper.findComponent({ name: 'CompanyMainContactCard' }).props('fetchContacts') as () => Promise<CompanyContactSummary[]>;

    await expect(fetchLocations()).resolves.toEqual(locations);
    expect(getAgencyLocations).toHaveBeenCalledWith('agency-1');
    await expect(fetchContacts()).resolves.toEqual([
      { firstName: 'Jane', middleName: 'M', lastName: 'Doe', position: 'Manager', mobileNumber: '416 555-0000', email: 'jane@covenant.ca' },
    ]);
  });

  it('maps contacts without names and an agency without contacts', async () => {
    const wrapper = setup({ id: 'agency-2', fullName: 'Sigook', contactInformation: [{ email: 'info@sigook.com' }] });
    const fetchContacts = wrapper.findComponent({ name: 'CompanyMainContactCard' }).props('fetchContacts') as () => Promise<CompanyContactSummary[]>;
    await expect(fetchContacts()).resolves.toEqual([{ firstName: '', middleName: undefined, lastName: '', position: undefined, mobileNumber: undefined, email: 'info@sigook.com' }]);

    await wrapper.setProps({ agency: { id: 'agency-3', fullName: 'Empty' } });
    const emptyFetch = wrapper.findComponent({ name: 'CompanyMainContactCard' }).props('fetchContacts') as () => Promise<CompanyContactSummary[]>;
    await expect(emptyFetch()).resolves.toEqual([]);
  });

  it('asks the page to show the full tabs', () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'CompanyLocationsCard' }).vm.$emit('showAll');
    wrapper.findComponent({ name: 'CompanyMainContactCard' }).vm.$emit('showAll');

    expect(wrapper.emitted('showTab')).toEqual([['Locations'], ['Contacts']]);
  });

  it('edits the business data in a modal', async () => {
    const wrapper = setup();
    await wrapper.find('.detail-link').trigger('click');
    await nextTick();

    const business = wrapper.findComponent({ name: 'ProfileBusiness' });
    expect(business.props('agencyData')).toEqual(agency);

    const updated = { ...agency, fullName: 'Covenant Group' };
    business.vm.$emit('update:agencyData', updated);
    business.vm.$emit('saved');
    await nextTick();

    expect(wrapper.emitted('update:agency')).toEqual([[updated]]);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });
});
