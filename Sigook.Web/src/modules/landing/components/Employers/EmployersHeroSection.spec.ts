import { describe, expect, it } from 'vitest';
import EmployersHeroSection from '@/modules/landing/components/Employers/EmployersHeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('EmployersHeroSection', () => {
  it('renders the hero copy, chips and scroll target', () => {
    const wrapper = mountWithPlugins(EmployersHeroSection);

    expect(wrapper.find('.landing-hero__eyebrow').text()).toBe('For Employers');
    expect(wrapper.find('.landing-hero__heading').text()).not.toBe('');
    expect(wrapper.find('.labeled-chip-list__label').text()).toBe('We staff for');
    expect(wrapper.findAll('.labeled-chip-list__chip').length).toBeGreaterThan(1);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#employers-solutions');
  });
});
