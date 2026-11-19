import { describe, expect, it } from 'vitest';
import IndustriesHeroSection from '@/modules/landing/components/Industries/IndustriesHeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('IndustriesHeroSection', () => {
  it('renders the hero copy, chips and scroll target', () => {
    const wrapper = mountWithPlugins(IndustriesHeroSection);

    expect(wrapper.find('.landing-hero__eyebrow').text()).toBe('Our Areas of Expertise');
    expect(wrapper.find('.landing-hero__heading').text()).not.toBe('');
    expect(wrapper.find('.labeled-chip-list__label').text()).toBe('Sectors we serve');
    expect(wrapper.findAll('.labeled-chip-list__chip').length).toBeGreaterThan(1);
    expect(wrapper.find('.labeled-chip-list__chip--more').text()).toBe('+ many more');
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#industries-grid');
  });
});
