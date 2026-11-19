import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import AgencyProfile from '@/modules/agency/profile/pages/AgencyProfile.vue';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import { getAccessibleAgencies, getAgencyPersonnel, getAgencyProfile } from '@/modules/agency/profile/api';
import { useAgencyStore } from '@/modules/agency/store';
import { lowercase } from '@/shared/format';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyDetail, AgencyLocationDetail, AgencyOption, AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const { route, stub } = vi.hoisted(() => ({
  route: { query: {} as Record<string, string> },
  stub: (name: string, props: string[], emits: string[]) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({
  getAgencyProfile: vi.fn(),
  getAccessibleAgencies: vi.fn(),
  getAgencyPersonnel: vi.fn(),
}));
vi.mock('@/modules/agency/profile/components/AgencyProfileTab.vue', () => ({ default: stub('AgencyProfileTab', ['agency', 'locationsVersion'], ['update:agency', 'showTab']) }));
vi.mock('@/modules/agency/profile/components/ProfileBilling.vue', () => ({ default: stub('ProfileBilling', ['agencyData'], ['changed']) }));
vi.mock('@/modules/agency/profile/components/ProfileContact.vue', () => ({ default: stub('ProfileContact', ['agencyData'], ['update:agencyData']) }));
vi.mock('@/modules/agency/profile/components/AgencyPersonnel.vue', () => ({ default: stub('AgencyPersonnel', [], ['loaded']) }));

const covenant: AgencyDetail = {
  id: 'a1',
  fullName: 'Covenant Staffing',
  masterAgency: true,
  logo: { pathFile: 'https://files/logo.png' },
  locations: [
    { id: 'l1', address: '5 Bay St', city: { id: 'to', value: 'Toronto', province: { id: 'on', value: 'Ontario', code: 'ON' } }, postalCode: 'M5J 2R8' },
    { id: 'l2', address: '100 King St', city: { id: 'to', value: 'Toronto' }, province: { id: 'on', value: 'Ontario', code: 'ON' }, postalCode: 'M5H 1A1', isBilling: true },
  ],
  contactInformation: [{ firstName: 'Jane' }],
};
const sigook: AgencyDetail = { id: 'a2', fullName: 'Sigook' };
const options: AgencyOption[] = [{ id: 'a1', name: 'Covenant' }, { id: 'a2', name: 'Sigook' }];
const personnel = [{ id: 'p1' }, { id: 'p2' }] as AgencyPersonnelListItem[];

let pinia: Pinia;

async function setup(query: Record<string, string> = {}) {
  route.query = query;
  const wrapper = mountWithPlugins(AgencyProfile, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function header(wrapper: Wrapper) {
  return wrapper.findComponent(DetailHeader);
}

function tabCounts(wrapper: Wrapper) {
  return wrapper.findAll('.detail-tab-count').map((c) => c.text());
}

async function openTab(wrapper: Wrapper, label: string) {
  const tab = wrapper.findAll('.tabs li a').find((a) => a.text().startsWith(label));
  if (!tab) throw new Error(`${label} tab not found`);
  await tab.trigger('click');
  await flushPromises();
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  vi.mocked(getAgencyProfile).mockResolvedValue(structuredClone(covenant));
  vi.mocked(getAgencyPersonnel).mockResolvedValue(personnel);
  vi.mocked(getAccessibleAgencies).mockResolvedValue([options[0]]);
});

describe('AgencyProfile', () => {
  it('loads the agency of the user and shows its header', async () => {
    const wrapper = await setup();

    expect(getAgencyProfile).toHaveBeenCalledExactlyOnceWith(undefined);
    expect(getAgencyPersonnel).toHaveBeenCalledWith();
    expect(useAgencyStore().agencies).toEqual([options[0]]);
    expect(header(wrapper).props()).toMatchObject({
      title: lowercase('Covenant Staffing'),
      meta: '100 King St · Toronto, ON · M5H 1A1',
      logo: 'https://files/logo.png',
      chips: [{ label: 'Master agency', variant: 'is-info' }],
      kpis: [
        { key: 'locations', label: 'Locations', value: '2' },
        { key: 'contacts', label: 'Contacts', value: '1' },
      ],
      headerHidden: false,
    });
    expect(tabCounts(wrapper)).toEqual(['2', '1', '2']);
    expect(wrapper.findComponent({ name: 'AgencyProfileTab' }).props()).toEqual({ agency: covenant, locationsVersion: 0 });
    expect(wrapper.findComponent({ name: 'ProfileBilling' }).exists()).toBe(false);
    expect(wrapper.text()).not.toContain('Switch agency');
  });

  it('uses the first location when none is the billing one and hides empty data', async () => {
    vi.mocked(getAgencyProfile).mockResolvedValue({ ...sigook, locations: [{ address: '1 Main St', city: null, postalCode: '' }] });
    const wrapper = await setup();

    expect(header(wrapper).props()).toMatchObject({ meta: '1 Main St', chips: [] });

    vi.mocked(getAgencyProfile).mockResolvedValue(sigook);
    const empty = await setup();
    expect(header(empty).props('meta')).toBeUndefined();
  });

  it('opens the tab from the query and remembers the agency', async () => {
    vi.mocked(getAgencyProfile).mockResolvedValue(sigook);
    const wrapper = await setup({ tab: 'Users', agencyId: 'a2' });

    expect(getAgencyProfile).toHaveBeenCalledWith('a2');
    expect(header(wrapper).props('headerHidden')).toBe(true);
    expect(wrapper.findComponent({ name: 'AgencyPersonnel' }).exists()).toBe(true);
  });

  it('ignores an unknown tab in the query', async () => {
    const wrapper = await setup({ tab: 'Unknown' });

    expect(header(wrapper).props('headerHidden')).toBe(false);
    expect(wrapper.findComponent({ name: 'AgencyProfileTab' }).exists()).toBe(true);
  });

  it('opens each tab lazily and keeps it in the url', async () => {
    const wrapper = await setup({ agencyId: 'a1' });
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    await openTab(wrapper, 'Locations');

    expect(push).toHaveBeenCalledWith({ path: '/agency-profile', query: { agencyId: 'a1', tab: 'Locations' } });
    expect(wrapper.findComponent({ name: 'ProfileBilling' }).props('agencyData')).toEqual(covenant);

    await openTab(wrapper, 'Contacts');
    expect(wrapper.findComponent({ name: 'ProfileContact' }).props('agencyData')).toEqual(covenant);

    await openTab(wrapper, 'Locations');
    expect(push).toHaveBeenCalledTimes(3);
  });

  it('opens the tab requested by the profile cards', async () => {
    const wrapper = await setup();
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    wrapper.findComponent({ name: 'AgencyProfileTab' }).vm.$emit('showTab', 'Contacts');
    await flushPromises();

    expect(push).toHaveBeenCalledWith({ path: '/agency-profile', query: { tab: 'Contacts' } });
    expect(wrapper.findComponent({ name: 'ProfileContact' }).exists()).toBe(true);
  });

  it('updates the counts when the locations or users change', async () => {
    const wrapper = await setup({ tab: 'Locations' });
    const changed: AgencyLocationDetail[] = [{ id: 'l9', address: '9 Queen St', city: null, postalCode: 'M5C 1A1' }];
    wrapper.findComponent({ name: 'ProfileBilling' }).vm.$emit('changed', changed);
    await nextTick();

    expect(tabCounts(wrapper)[0]).toBe('1');
    expect(wrapper.findComponent({ name: 'AgencyProfileTab' }).props('locationsVersion')).toBe(1);

    await openTab(wrapper, 'Users');
    wrapper.findComponent({ name: 'AgencyPersonnel' }).vm.$emit('loaded', 7);
    await nextTick();

    expect(tabCounts(wrapper)[2]).toBe('7');
  });

  it('updates the agency edited in the profile tab', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'AgencyProfileTab' }).vm.$emit('update:agency', { ...covenant, fullName: 'Covenant Group' });
    await nextTick();

    expect(header(wrapper).props('title')).toBe(lowercase('Covenant Group'));
  });

  it('switches between the accessible agencies', async () => {
    vi.mocked(getAccessibleAgencies).mockResolvedValue(options);
    const wrapper = await setup({ tab: 'Contacts' });

    expect(wrapper.text()).toContain('Switch agency');
    const replace = vi.spyOn(wrapper.vm.$router, 'replace');
    const dropdown = wrapper.findComponent({ name: 'BDropdown' });
    dropdown.vm.$emit('update:modelValue', 'a1');
    await flushPromises();

    expect(getAgencyProfile).toHaveBeenCalledTimes(1);

    vi.mocked(getAgencyProfile).mockResolvedValue(sigook);
    dropdown.vm.$emit('update:modelValue', 'a2');
    await flushPromises();

    expect(replace).toHaveBeenCalledExactlyOnceWith({ query: { tab: 'Contacts', agencyId: 'a2' } });
    expect(getAgencyProfile).toHaveBeenLastCalledWith('a2');
    expect(header(wrapper).props('title')).toBe(lowercase('Sigook'));
  });

  it('shows the errors when loading fails', async () => {
    const profileError = new Error('profile failed');
    const personnelError = new Error('personnel failed');
    const agenciesError = new Error('agencies failed');
    vi.mocked(getAgencyProfile).mockRejectedValue(profileError);
    vi.mocked(getAgencyPersonnel).mockRejectedValue(personnelError);
    vi.mocked(getAccessibleAgencies).mockRejectedValue(agenciesError);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(profileError);
    expect(showAlertError).toHaveBeenCalledWith(personnelError);
    expect(showAlertError).toHaveBeenCalledWith(agenciesError);
    expect(header(wrapper).exists()).toBe(false);
  });
});
