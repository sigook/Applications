import { describe, expect, it, vi } from 'vitest';
import Employers from '@/modules/landing/pages/Employers/Employers.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/Employers/EmployersHeroSection.vue', () => ({ default: { name: 'EmployersHeroSection', template: '<section data-stub="EmployersHeroSection" />' } }));
vi.mock('@/modules/landing/components/Employers/EmployersSolutionsSection.vue', () => ({ default: { name: 'EmployersSolutionsSection', template: '<section data-stub="EmployersSolutionsSection" />' } }));
vi.mock('@/modules/landing/components/Employers/EmployersWhyChooseSection.vue', () => ({ default: { name: 'EmployersWhyChooseSection', template: '<section data-stub="EmployersWhyChooseSection" />' } }));
vi.mock('@/modules/landing/components/Employers/EmployersIndustriesCarousel.vue', () => ({ default: { name: 'EmployersIndustriesCarousel', template: '<section data-stub="EmployersIndustriesCarousel" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));

describe('Employers', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(Employers);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['EmployersHeroSection', 'EmployersSolutionsSection', 'EmployersWhyChooseSection', 'EmployersIndustriesCarousel', 'ContactSection']);
    expect(wrapper.find('#employers-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
