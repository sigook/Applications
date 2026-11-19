import { describe, expect, it } from 'vitest';
import TalentsSolutionsSection from '@/modules/landing/components/Talents/TalentsSolutionsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TalentsSolutionsSection', () => {
  it('renders the three hiring options', () => {
    const wrapper = mountWithPlugins(TalentsSolutionsSection);

    expect(wrapper.attributes('id')).toBe('talents-solutions');
    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(['Direct Hiring', 'Temp to Perm', 'Contract']);
    expect(wrapper.findAll('a.arrow-pill')).toHaveLength(2);
  });
});
