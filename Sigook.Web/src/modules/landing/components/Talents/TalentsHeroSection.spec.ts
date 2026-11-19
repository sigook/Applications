import { describe, expect, it } from 'vitest';
import TalentsHeroSection from '@/modules/landing/components/Talents/TalentsHeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TalentsHeroSection', () => {
  it('renders the hero copy, chips and scroll target', () => {
    const wrapper = mountWithPlugins(TalentsHeroSection);

    expect(wrapper.find('.landing-hero__eyebrow').text()).toBe('Talents');
    expect(wrapper.find('.landing-hero__heading').text()).not.toBe('');
    expect(wrapper.find('.labeled-chip-list__label').text()).toBe('We recruit for');
    expect(wrapper.findAll('.labeled-chip-list__chip').length).toBeGreaterThan(1);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#talents-solutions');
  });
});
