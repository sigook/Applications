import { describe, expect, it } from 'vitest';
import DualCtaSection from '@/modules/landing/components/Home/DualCtaSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('DualCtaSection', () => {
  it('renders the talent and employer cards', () => {
    const wrapper = mountWithPlugins(DualCtaSection);

    expect(wrapper.findAll('.primary-card__title').map((t) => t.text())).toEqual(['Find Work', 'Find Talent']);
    expect(wrapper.findAll('.primary-card__list li')).toHaveLength(6);
    expect(wrapper.findAll('a.arrow-pill').map((a) => a.attributes('href'))).toEqual(['/open-positions', '/#home-contact']);
  });

  it('exposes the anchor the hero scroll indicator points to', () => {
    const wrapper = mountWithPlugins(DualCtaSection);
    expect(wrapper.find('#dual-cta').exists()).toBe(true);
  });
});
