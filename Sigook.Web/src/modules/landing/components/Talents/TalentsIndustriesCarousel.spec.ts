import { describe, expect, it } from 'vitest';
import TalentsIndustriesCarousel from '@/modules/landing/components/Talents/TalentsIndustriesCarousel.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TalentsIndustriesCarousel', () => {
  it('renders the industries carousel inside the panel', () => {
    const wrapper = mountWithPlugins(TalentsIndustriesCarousel);

    expect(wrapper.find('.industries-carousel-panel').exists()).toBe(true);
    expect(wrapper.find('.industries-carousel__eyebrow').text()).toBe('Roles We Recruit');
    expect(wrapper.findAll('.industries-carousel__slide')).toHaveLength(4);
  });
});
