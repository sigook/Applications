import { describe, expect, it } from 'vitest';
import EmployersWhyChooseSection from '@/modules/landing/components/Employers/EmployersWhyChooseSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('EmployersWhyChooseSection', () => {
  it('renders the three reasons', () => {
    const wrapper = mountWithPlugins(EmployersWhyChooseSection);

    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(["Shortlists in days, not months", "Recruiters who speak your language", "A bench you didn't have to build"]);
  });
});
