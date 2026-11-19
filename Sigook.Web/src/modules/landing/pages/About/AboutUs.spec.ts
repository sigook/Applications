import { describe, expect, it, vi } from 'vitest';
import AboutUs from '@/modules/landing/pages/About/AboutUs.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/About/AboutIntroSection.vue', () => ({ default: { name: 'AboutIntroSection', template: '<section data-stub="AboutIntroSection" />' } }));
vi.mock('@/modules/landing/components/About/OurHistorySection.vue', () => ({ default: { name: 'OurHistorySection', template: '<section data-stub="OurHistorySection" />' } }));
vi.mock('@/modules/landing/components/About/AboutMissionSection.vue', () => ({ default: { name: 'AboutMissionSection', template: '<section data-stub="AboutMissionSection" />' } }));
vi.mock('@/modules/landing/components/About/AboutNumbersSection.vue', () => ({ default: { name: 'AboutNumbersSection', template: '<section data-stub="AboutNumbersSection" />' } }));
vi.mock('@/modules/landing/components/About/OurTeamSection.vue', () => ({ default: { name: 'OurTeamSection', template: '<section data-stub="OurTeamSection" />' } }));
vi.mock('@/modules/landing/components/About/AboutFormSection.vue', () => ({ default: { name: 'AboutFormSection', template: '<section data-stub="AboutFormSection" />' } }));

describe('AboutUs', () => {
  it('composes the page sections in order', () => {
    const wrapper = mountWithPlugins(AboutUs);

    expect(wrapper.findAll('[data-stub]').map((s) => s.attributes('data-stub'))).toEqual(['AboutIntroSection', 'OurHistorySection', 'AboutMissionSection', 'AboutNumbersSection', 'OurTeamSection', 'AboutFormSection']);
  });
});
