import { describe, expect, it, vi } from 'vitest';
import Talents from '@/modules/landing/pages/Talents/Talents.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/Talents/TalentsHeroSection.vue', () => ({ default: { name: 'TalentsHeroSection', template: '<section data-stub="TalentsHeroSection" />' } }));
vi.mock('@/modules/landing/components/Talents/TalentsSolutionsSection.vue', () => ({ default: { name: 'TalentsSolutionsSection', template: '<section data-stub="TalentsSolutionsSection" />' } }));
vi.mock('@/modules/landing/components/Talents/TalentsWhyWorkSection.vue', () => ({ default: { name: 'TalentsWhyWorkSection', template: '<section data-stub="TalentsWhyWorkSection" />' } }));
vi.mock('@/modules/landing/components/Talents/TalentsIndustriesCarousel.vue', () => ({ default: { name: 'TalentsIndustriesCarousel', template: '<section data-stub="TalentsIndustriesCarousel" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));

describe('Talents', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(Talents);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['TalentsHeroSection', 'TalentsSolutionsSection', 'TalentsWhyWorkSection', 'TalentsIndustriesCarousel', 'ContactSection']);
    expect(wrapper.find('#talents-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
