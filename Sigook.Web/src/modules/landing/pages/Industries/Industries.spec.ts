import { describe, expect, it, vi } from 'vitest';
import Industries from '@/modules/landing/pages/Industries/Industries.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/Industries/IndustriesHeroSection.vue', () => ({ default: { name: 'IndustriesHeroSection', template: '<section data-stub="IndustriesHeroSection" />' } }));
vi.mock('@/modules/landing/components/Industries/IndustriesGridSection.vue', () => ({ default: { name: 'IndustriesGridSection', template: '<section data-stub="IndustriesGridSection" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));
vi.mock('@/modules/landing/components/shared/forms/CandidateApplyModal.vue', () => ({ default: { name: 'CandidateApplyModal', template: '<section data-stub="CandidateApplyModal" />' } }));

describe('Industries', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(Industries);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['IndustriesHeroSection', 'IndustriesGridSection', 'ContactSection', 'CandidateApplyModal']);
    expect(wrapper.find('#industries-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
