import { describe, expect, it } from 'vitest';
import NumbersSection from '@/modules/landing/components/Home/NumbersSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('NumbersSection', () => {
  it('renders the three stats', () => {
    const wrapper = mountWithPlugins(NumbersSection);

    expect(wrapper.findAll('.numbers__num').map((n) => n.text())).toEqual(['+650', '+2,000', '+15,000']);
    expect(wrapper.classes()).not.toContain('is-visible');
  });
});
