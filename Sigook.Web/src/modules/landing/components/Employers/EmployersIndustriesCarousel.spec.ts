import { describe, expect, it } from 'vitest';
import EmployersIndustriesCarousel from '@/modules/landing/components/Employers/EmployersIndustriesCarousel.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('EmployersIndustriesCarousel', () => {
  it('renders the industries carousel inside the panel', () => {
    const wrapper = mountWithPlugins(EmployersIndustriesCarousel);

    expect(wrapper.find('.industries-carousel-panel').exists()).toBe(true);
    expect(wrapper.find('.industries-carousel__eyebrow').text()).toBe('Sectors We Staff');
    expect(wrapper.findAll('.industries-carousel__slide')).toHaveLength(4);
  });
});
