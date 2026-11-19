import { describe, expect, it, vi } from 'vitest';
import OpenPositions from '@/modules/landing/pages/OpenPositions/OpenPositions.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/OpenPositions/OpenPositionsHeroSection.vue', () => ({ default: { name: 'OpenPositionsHeroSection', template: '<section data-stub="OpenPositionsHeroSection" />' } }));
vi.mock('@/modules/landing/components/OpenPositions/OpenPositionsListSection.vue', () => ({ default: { name: 'OpenPositionsListSection', template: '<section data-stub="OpenPositionsListSection" />' } }));
vi.mock('@/modules/landing/components/OpenPositions/OpenPositionsBrowseSection.vue', () => ({ default: { name: 'OpenPositionsBrowseSection', template: '<section data-stub="OpenPositionsBrowseSection" />' } }));
vi.mock('@/modules/landing/components/OpenPositions/OpenPositionsCtaSection.vue', () => ({ default: { name: 'OpenPositionsCtaSection', template: '<section data-stub="OpenPositionsCtaSection" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));
vi.mock('@/modules/landing/components/shared/forms/CandidateApplyModal.vue', () => ({ default: { name: 'CandidateApplyModal', template: '<section data-stub="CandidateApplyModal" />' } }));

describe('OpenPositions', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(OpenPositions);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['OpenPositionsHeroSection', 'OpenPositionsListSection', 'OpenPositionsBrowseSection', 'OpenPositionsCtaSection', 'ContactSection', 'CandidateApplyModal']);
    expect(wrapper.find('#openpos-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
