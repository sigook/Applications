import { describe, expect, it } from 'vitest';
import PartnerHeroSection from '@/modules/landing/components/Partner/PartnerHeroSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PartnerHeroSection', () => {
  it('renders the program stats and scroll target', () => {
    const wrapper = mountWithPlugins(PartnerHeroSection);

    expect(wrapper.find('.partner-hero__eyebrow').text()).toBe('Partner Program');
    expect(wrapper.findAll('.partner-hero__stat-value').map((s) => s.text())).toEqual(['12+', '+35%', '~4 wks']);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#partner-types');
  });
});
