import { describe, expect, it } from 'vitest';
import EmployersSolutionsSection from '@/modules/landing/components/Employers/EmployersSolutionsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('EmployersSolutionsSection', () => {
  it('renders the three hiring options', () => {
    const wrapper = mountWithPlugins(EmployersSolutionsSection);

    expect(wrapper.attributes('id')).toBe('employers-solutions');
    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(['Direct Hiring', 'Temp to Perm', 'Contract']);
    expect(wrapper.findAll('a.arrow-pill')).toHaveLength(2);
  });
});
