import { describe, expect, it, vi } from 'vitest';
import Home from '@/modules/landing/pages/Home/Home.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/Home/HeroSection.vue', () => ({ default: { name: 'HeroSection', template: '<section data-stub="HeroSection" />' } }));
vi.mock('@/modules/landing/components/Home/DualCtaSection.vue', () => ({ default: { name: 'DualCtaSection', template: '<section data-stub="DualCtaSection" />' } }));
vi.mock('@/modules/landing/components/Home/NumbersSection.vue', () => ({ default: { name: 'NumbersSection', template: '<section data-stub="NumbersSection" />' } }));
vi.mock('@/modules/landing/components/Home/WhyChooseUsSection.vue', () => ({ default: { name: 'WhyChooseUsSection', template: '<section data-stub="WhyChooseUsSection" />' } }));
vi.mock('@/modules/landing/components/Home/CertifiedSection.vue', () => ({ default: { name: 'CertifiedSection', template: '<section data-stub="CertifiedSection" />' } }));
vi.mock('@/modules/landing/components/Home/AppDownloadSection.vue', () => ({ default: { name: 'AppDownloadSection', template: '<section data-stub="AppDownloadSection" />' } }));
vi.mock('@/modules/landing/components/Home/TestimonialsSection.vue', () => ({ default: { name: 'TestimonialsSection', template: '<section data-stub="TestimonialsSection" />' } }));
vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({ default: { name: 'ContactSection', template: '<section data-stub="ContactSection" />' } }));

describe('Home', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(Home);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['HeroSection', 'DualCtaSection', 'NumbersSection', 'WhyChooseUsSection', 'CertifiedSection', 'AppDownloadSection', 'TestimonialsSection', 'ContactSection']);
    expect(wrapper.find('#home-contact [data-stub="ContactSection"]').exists()).toBe(true);
  });
});
