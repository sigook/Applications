import { describe, expect, it } from 'vitest';
import AboutIntroSection from '@/modules/landing/components/About/AboutIntroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('AboutIntroSection', () => {
  it('renders the story, credentials and values', () => {
    const wrapper = mountWithPlugins(AboutIntroSection);

    expect(wrapper.find('.about-intro__eyebrow').text()).toBe('Our Story');
    expect(wrapper.findAll('.about-intro__credential').map((c) => c.text())).toEqual(['Since 2008', 'Florida → coast to coast', 'Talent Management since 2016']);
    expect(wrapper.findAll('.about-intro__credential-dot')).toHaveLength(2);
    expect(wrapper.findAll('.labeled-chip-list__chip').map((c) => c.text())).toEqual(['Fairness', 'Respect', 'Integrity', 'Trust', 'Care']);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#about-numbers');
  });
});
