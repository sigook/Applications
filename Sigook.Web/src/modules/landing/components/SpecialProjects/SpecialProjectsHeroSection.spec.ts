import { describe, expect, it } from 'vitest';
import SpecialProjectsHeroSection from '@/modules/landing/components/SpecialProjects/SpecialProjectsHeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('SpecialProjectsHeroSection', () => {
  it('renders the hero copy, chips and scroll target', () => {
    const wrapper = mountWithPlugins(SpecialProjectsHeroSection);

    expect(wrapper.find('.landing-hero__eyebrow').text()).toBe('Special Projects');
    expect(wrapper.find('.landing-hero__heading').text()).not.toBe('');
    expect(wrapper.find('.labeled-chip-list__label').text()).toBe('Focus areas');
    expect(wrapper.findAll('.labeled-chip-list__chip').length).toBeGreaterThan(1);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#sp-focus');
  });
});
