import { describe, expect, it } from 'vitest';
import SpecialProjectsFocusSection from '@/modules/landing/components/SpecialProjects/SpecialProjectsFocusSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('SpecialProjectsFocusSection', () => {
  it('renders the four focus areas', () => {
    const wrapper = mountWithPlugins(SpecialProjectsFocusSection);

    expect(wrapper.attributes('id')).toBe('sp-focus');
    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(['Workforce Development', 'Featured Initiatives', 'How We Collaborate', 'Future Initiatives']);
  });
});
