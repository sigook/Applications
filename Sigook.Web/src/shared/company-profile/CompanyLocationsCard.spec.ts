import { describe, expect, it, vi } from 'vitest';
import CompanyLocationsCard from '@/shared/company-profile/CompanyLocationsCard.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { LocationDetailModel } from '@/shared/types/common';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

type FetchLocations = () => Promise<LocationDetailModel[]>;

const toronto = { id: 'c1', value: 'Toronto', province: { id: 'p1', value: 'Ontario', code: 'ON' } };

const plant: LocationDetailModel = { id: 'l1', address: '1 Plant Rd', city: toronto, postalCode: 'M1M 1M1', isBilling: false, latitude: 43.7, longitude: -79.4 };
const office: LocationDetailModel = { id: 'l2', address: '2 Office Ave', city: toronto, postalCode: 'M2M 2M2', isBilling: true };

async function setup(fetchLocations: FetchLocations) {
  const wrapper = mountWithPlugins(CompanyLocationsCard, { props: { fetchLocations }, attachTo: document.createElement('div') });
  await vi.waitFor(() => { expect(fetchLocations).toHaveBeenCalled(); });
  return wrapper;
}

describe('CompanyLocationsCard', () => {
  it('shows the empty state and asks to add a location', async () => {
    const wrapper = await setup(vi.fn<FetchLocations>().mockResolvedValue([]));

    await vi.waitFor(() => { expect(wrapper.emitted('loaded')).toEqual([[0]]); });
    expect(wrapper.find('.detail-empty').text()).toBe('No locations');
    expect(wrapper.find('iframe').exists()).toBe(false);
    expect(wrapper.find('.detail-card-actions').exists()).toBe(false);

    await wrapper.find('.company-locations-empty button').trigger('click');
    expect(wrapper.emitted('showAll')).toHaveLength(1);
  });

  it('features the billing location and counts the rest', async () => {
    const wrapper = await setup(vi.fn<FetchLocations>().mockResolvedValue([plant, office]));

    await vi.waitFor(() => { expect(wrapper.emitted('loaded')).toEqual([[2]]); });
    expect(wrapper.find('.company-location-address').text()).toBe('2 Office Ave');
    expect(wrapper.find('.detail-tag').text()).toBe('Billing');
    expect(wrapper.find('.detail-list-item .detail-muted').text()).toBe('Toronto, ON, M2M 2M2');
    expect(wrapper.find('iframe').exists()).toBe(false);
    expect(wrapper.text()).toContain('+ 1 more');

    const all = wrapper.find('.detail-card-actions button');
    expect(all.text()).toBe('All 2');
    await all.trigger('click');
    expect(wrapper.emitted('showAll')).toHaveLength(1);
  });

  it('shows the map for the first location when none is billing', async () => {
    const wrapper = await setup(vi.fn<FetchLocations>().mockResolvedValue([plant]));

    await vi.waitFor(() => { expect(wrapper.find('iframe').exists()).toBe(true); });
    expect(wrapper.find('iframe').attributes('src')).toContain('q=43.7,-79.4');
    expect(wrapper.find('.detail-tag').exists()).toBe(false);
    expect(wrapper.text()).not.toContain('more');
  });

  it('reloads through the exposed load method', async () => {
    const fetchLocations = vi.fn<FetchLocations>().mockResolvedValueOnce([]).mockResolvedValueOnce([plant]);
    const wrapper = await setup(fetchLocations);
    await vi.waitFor(() => { expect(wrapper.emitted('loaded')).toHaveLength(1); });

    await (wrapper.vm as unknown as { load: () => Promise<void> }).load();

    expect(wrapper.emitted('loaded')).toEqual([[0], [1]]);
    expect(wrapper.find('.company-location-address').text()).toBe('1 Plant Rd');
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    const wrapper = await setup(vi.fn<FetchLocations>().mockRejectedValue(error));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('loaded')).toBeUndefined();
  });
});
