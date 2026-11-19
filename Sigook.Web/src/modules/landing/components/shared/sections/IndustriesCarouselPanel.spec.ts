import { describe, expect, it } from 'vitest';
import IndustriesCarouselPanel from '@/modules/landing/components/shared/sections/IndustriesCarouselPanel.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('IndustriesCarouselPanel', () => {
  it('wraps its content on a decorative surface', () => {
    const wrapper = mountWithPlugins(IndustriesCarouselPanel, { slots: { default: '<p class="content">Carousel</p>' } });

    expect(wrapper.find('.industries-carousel-panel__surface').attributes('aria-hidden')).toBe('true');
    expect(wrapper.find('.content').text()).toBe('Carousel');
  });
});
