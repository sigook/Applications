import { describe, expect, it } from 'vitest';
import WhyChooseUsSection from '@/modules/landing/components/Home/WhyChooseUsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('WhyChooseUsSection', () => {
  it('renders the approach and the three point groups', () => {
    const wrapper = mountWithPlugins(WhyChooseUsSection);

    expect(wrapper.find('.why__hero-title').text()).toBe('Why Choose Us?');
    expect(wrapper.text()).toContain('Recruiters and coverage across all major US markets');
    expect(wrapper.text()).toContain('Dedicated recruiters who know your local industry and market');
    expect(wrapper.text()).toContain('A partner invested in long-term success for workers and employers alike');
  });
});
