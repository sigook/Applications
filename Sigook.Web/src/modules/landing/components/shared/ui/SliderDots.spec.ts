import { describe, expect, it } from 'vitest';
import SliderDots from '@/modules/landing/components/shared/ui/SliderDots.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('SliderDots', () => {
  it('renders one dot per slide and marks the active one', () => {
    const wrapper = mountWithPlugins(SliderDots, { props: { modelValue: 1, count: 3 } });
    const dots = wrapper.findAll('button');

    expect(wrapper.attributes('aria-label')).toBe('Carousel navigation');
    expect(dots.map((d) => d.attributes('aria-label'))).toEqual(['Slide 1', 'Slide 2', 'Slide 3']);
    expect(dots.map((d) => d.attributes('aria-selected'))).toEqual(['false', 'true', 'false']);
    expect(dots[1].classes()).toContain('slider-dots__dot--active');
  });

  it('emits the clicked index', async () => {
    const wrapper = mountWithPlugins(SliderDots, { props: { modelValue: 0, count: 3, ariaLabel: 'Testimonials' } });
    await wrapper.findAll('button')[2].trigger('click');

    expect(wrapper.attributes('aria-label')).toBe('Testimonials');
    expect(wrapper.emitted('update:modelValue')).toEqual([[2]]);
  });
});
