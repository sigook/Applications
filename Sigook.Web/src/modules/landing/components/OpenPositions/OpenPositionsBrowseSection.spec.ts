import { describe, expect, it } from 'vitest';
import OpenPositionsBrowseSection from '@/modules/landing/components/OpenPositions/OpenPositionsBrowseSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('OpenPositionsBrowseSection', () => {
  it('renders the industries carousel with the browse copy', () => {
    const wrapper = mountWithPlugins(OpenPositionsBrowseSection);

    expect(wrapper.find('.industries-carousel__eyebrow').text()).toBe('By Industry');
    expect(wrapper.find('.industries-carousel__cta-link').text()).toContain('Full directory');
  });
});
