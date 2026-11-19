import { describe, expect, it } from 'vitest';
import AboutNumbersSection from '@/modules/landing/components/About/AboutNumbersSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('AboutNumbersSection', () => {
  it('renders the about stats', () => {
    const wrapper = mountWithPlugins(AboutNumbersSection);

    expect(wrapper.attributes('id')).toBe('about-numbers');
    expect(wrapper.findAll('.numbers-grid__lbl').map((l) => l.text())).toEqual(['Years of Experience', 'Clients Served', 'Jobs Posted', 'Applications Filed']);
  });
});
