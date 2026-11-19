import { describe, expect, it } from 'vitest';
import IndustriesGridSection from '@/modules/landing/components/Industries/IndustriesGridSection.vue';
import IndustryCard from '@/modules/landing/components/Industries/IndustryCard.vue';
import industries from '@/modules/landing/data/industries.json';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('IndustriesGridSection', () => {
  it('renders a card per industry', () => {
    const wrapper = mountWithPlugins(IndustriesGridSection);

    expect(wrapper.findAllComponents(IndustryCard)).toHaveLength(industries.length);
    expect(wrapper.findAll('.industry-card__title').map((t) => t.text())).toEqual(industries.map((i) => i.title));
  });

  it('expands one industry at a time', async () => {
    const wrapper = mountWithPlugins(IndustriesGridSection);
    const toggles = wrapper.findAll('.industry-card__toggle');
    const expanded = () => wrapper.findAllComponents(IndustryCard).map((c) => c.props('expanded'));

    await toggles[0].trigger('click');
    expect(expanded().filter(Boolean)).toHaveLength(1);
    expect(expanded()[0]).toBe(true);

    await toggles[1].trigger('click');
    expect(expanded()[0]).toBe(false);
    expect(expanded()[1]).toBe(true);

    await toggles[1].trigger('click');
    expect(expanded().some(Boolean)).toBe(false);
  });
});
