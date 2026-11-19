import { describe, expect, it } from 'vitest';
import RequestLocationCard from '@/shared/request-detail/RequestLocationCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { LocationDetailModel } from '@/shared/types/common';

const location: LocationDetailModel = {
  address: '12 King St',
  city: { id: 'c1', value: 'Toronto', province: { id: 'p1', value: 'Ontario', code: 'ON' } },
  postalCode: 'M5V 1A1',
  mainIntersection: 'King & Bay',
  entrance: 'Door 3',
  latitude: 43.6532,
  longitude: -79.3832,
};

function setup(value?: LocationDetailModel | null) {
  return mountWithPlugins(RequestLocationCard, { props: { location: value }, attachTo: document.createElement('div') });
}

describe('RequestLocationCard', () => {
  it('renders nothing without a location', () => {
    expect(setup(null).find('.detail-card').exists()).toBe(false);
  });

  it('shows the address, map, coordinates and maps link', () => {
    const wrapper = setup(location);
    const text = wrapper.findAll('.request-location-text > span').map((s) => s.text());

    expect(text).toEqual(['12 King St, Toronto, ON, M5V 1A1', 'King & Bay', 'Door 3', '43.653200, -79.383200']);
    expect(wrapper.find('iframe').attributes('src')).toContain('q=43.6532,-79.3832');
    expect(wrapper.find('a.request-location-link').attributes('href')).toBe('https://www.google.com/maps/search/?api=1&query=43.6532,-79.3832');
  });

  it('prefers the formatted address and hides map details without coordinates', () => {
    const wrapper = setup({ address: '12 King St', formattedAddress: '12 King St W, Toronto' });

    expect(wrapper.findAll('.request-location-text > span').map((s) => s.text())).toEqual(['12 King St W, Toronto']);
    expect(wrapper.find('iframe').exists()).toBe(false);
    expect(wrapper.find('.request-location-link').exists()).toBe(false);
  });
});
