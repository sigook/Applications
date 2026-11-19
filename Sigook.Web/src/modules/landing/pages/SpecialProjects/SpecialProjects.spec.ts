import { describe, expect, it, vi } from 'vitest';
import SpecialProjects from '@/modules/landing/pages/SpecialProjects/SpecialProjects.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/SpecialProjects/SpecialProjectsHeroSection.vue', () => ({ default: { name: 'SpecialProjectsHeroSection', template: '<section data-stub="SpecialProjectsHeroSection" />' } }));
vi.mock('@/modules/landing/components/SpecialProjects/SpecialProjectsFocusSection.vue', () => ({ default: { name: 'SpecialProjectsFocusSection', template: '<section data-stub="SpecialProjectsFocusSection" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));

describe('SpecialProjects', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(SpecialProjects);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['SpecialProjectsHeroSection', 'SpecialProjectsFocusSection', 'ContactSection']);
    expect(wrapper.find('#sp-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
