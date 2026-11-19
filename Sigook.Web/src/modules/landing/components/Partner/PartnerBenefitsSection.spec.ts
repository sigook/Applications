import { describe, expect, it } from 'vitest';
import PartnerBenefitsSection from '@/modules/landing/components/Partner/PartnerBenefitsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PartnerBenefitsSection', () => {
  it('renders the four benefits with their icons', () => {
    const wrapper = mountWithPlugins(PartnerBenefitsSection);
    const cards = wrapper.findAll('.secondary-card');

    expect(cards.map((c) => c.find('.secondary-card__title').text())).toEqual(['Full Autonomy', 'Nationwide Reach', 'Licensed Agency', 'Entrepreneurial Rewards']);
    expect(cards.map((c) => c.classes().find((cls) => /^secondary-card--(cyan|blue|red)$/.test(cls)))).toEqual([
      'secondary-card--cyan', 'secondary-card--blue', 'secondary-card--cyan', 'secondary-card--red',
    ]);
    expect(cards.map((c) => c.findAll('svg path').length)).toEqual([2, 3, 2, 4]);
  });
});
