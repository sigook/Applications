import { describe, expect, it } from 'vitest';
import TalentsWhyWorkSection from '@/modules/landing/components/Talents/TalentsWhyWorkSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TalentsWhyWorkSection', () => {
  it('renders the three reasons', () => {
    const wrapper = mountWithPlugins(TalentsWhyWorkSection);

    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(["Working where you are", "We care about you", "Focused on your experience"]);
  });
});
