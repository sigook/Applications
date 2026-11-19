import { describe, expect, it } from 'vitest';
import PartnerTypesSection from '@/modules/landing/components/Partner/PartnerTypesSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PartnerTypesSection', () => {
  it('renders both partner tracks linking to the contact form', () => {
    const wrapper = mountWithPlugins(PartnerTypesSection);

    expect(wrapper.attributes('id')).toBe('partner-types');
    expect(wrapper.findAll('.primary-card__title').map((t) => t.text())).toEqual(['Recruiter Partner', 'Business Partner']);
    expect(wrapper.findAll('a.arrow-pill').map((a) => a.attributes('href'))).toEqual(['#partner-contact', '#partner-contact']);
    expect(wrapper.findAll('.primary-card__list li').length).toBeGreaterThan(0);
  });
});
