import { describe, expect, it } from 'vitest';
import HeroSection from '@/modules/landing/components/Home/HeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('HeroSection', () => {
  it('renders the hero copy and industries', () => {
    const wrapper = mountWithPlugins(HeroSection);

    expect(wrapper.find('.hero__eyebrow').text()).toBe('Workforce Platform');
    expect(wrapper.find('.hero__logo').attributes('alt')).toBe('Sigook Work Factory');
    expect(wrapper.findAll('.labeled-chip-list__chip:not(.labeled-chip-list__chip--more)').map((c) => c.text())).toEqual([
      'Cybersecurity', 'Artificial Intelligence', 'Manufacturing', 'Logistics', 'Retail', 'Construction',
    ]);
    expect(wrapper.find('a.labeled-chip-list__chip--more').attributes('href')).toBe('/industries');
    expect(wrapper.find('.scroll-indicator').exists()).toBe(true);
  });
});
