import { describe, expect, it, vi } from 'vitest';
import Partner from '@/modules/landing/pages/Partner/Partner.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/Partner/PartnerHeroSection.vue', () => ({ default: { name: 'PartnerHeroSection', template: '<section data-stub="PartnerHeroSection" />' } }));
vi.mock('@/modules/landing/components/Partner/PartnerTypesSection.vue', () => ({ default: { name: 'PartnerTypesSection', template: '<section data-stub="PartnerTypesSection" />' } }));
vi.mock('@/modules/landing/components/Partner/PartnerBenefitsSection.vue', () => ({ default: { name: 'PartnerBenefitsSection', template: '<section data-stub="PartnerBenefitsSection" />' } }));
vi.mock('@/modules/landing/components/Partner/PartnerProcessSection.vue', () => ({ default: { name: 'PartnerProcessSection', template: '<section data-stub="PartnerProcessSection" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));

describe('Partner', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(Partner);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['PartnerHeroSection', 'PartnerTypesSection', 'PartnerBenefitsSection', 'PartnerProcessSection', 'ContactSection']);
    expect(wrapper.find('#partner-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
