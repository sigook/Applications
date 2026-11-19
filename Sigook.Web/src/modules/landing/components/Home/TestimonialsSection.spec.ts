import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import TestimonialsSection from '@/modules/landing/components/Home/TestimonialsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup({ hover = false, reducedMotion = false } = {}) {
  vi.useFakeTimers();
  vi.stubGlobal('matchMedia', vi.fn((query: string) => ({
    matches: query === '(hover: hover)' ? hover : query === '(prefers-reduced-motion: reduce)' ? reducedMotion : false,
    media: query,
  })));
  return mountWithPlugins(TestimonialsSection);
}

type Wrapper = ReturnType<typeof setup>;

function author(wrapper: Wrapper) {
  return wrapper.find('.testimonials__author').text();
}

async function tick(ms: number) {
  vi.advanceTimersByTime(ms);
  await nextTick();
}

describe('TestimonialsSection', () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it('shows the first testimonial', () => {
    const wrapper = setup();

    expect(author(wrapper)).toBe('HR Manager, Manufacturer');
    expect(wrapper.findAll('.testimonials__dot').map((d) => d.attributes('aria-selected'))).toEqual(['true', 'false', 'false']);
    expect(wrapper.findAll('.testimonials__bg-img')).toHaveLength(3);
  });

  it('rotates every six seconds and wraps around', async () => {
    const wrapper = setup();
    await tick(6000);
    expect(author(wrapper)).toBe('Business Owner, Retail');

    await tick(12000);
    expect(author(wrapper)).toBe('HR Manager, Manufacturer');
  });

  it('does not rotate with reduced motion', async () => {
    const wrapper = setup({ reducedMotion: true });
    await tick(12000);

    expect(author(wrapper)).toBe('HR Manager, Manufacturer');
  });

  it('jumps to the clicked dot and restarts the timer', async () => {
    const wrapper = setup();
    await tick(5000);
    await wrapper.findAll('.testimonials__dot')[2].trigger('click');
    expect(author(wrapper)).toBe('Operations Manager, Logistics');
    expect(wrapper.findAll('.testimonials__bg')[2].classes()).toContain('testimonials__bg--active');

    await tick(5000);
    expect(author(wrapper)).toBe('Operations Manager, Logistics');
    await tick(1000);
    expect(author(wrapper)).toBe('HR Manager, Manufacturer');
  });

  it('pauses on hover on hover-capable devices', async () => {
    const wrapper = setup({ hover: true });
    await wrapper.trigger('mouseenter');
    await tick(12000);
    expect(author(wrapper)).toBe('HR Manager, Manufacturer');

    await wrapper.trigger('mouseleave');
    await tick(6000);
    expect(author(wrapper)).toBe('Business Owner, Retail');
  });

  it('keeps rotating on hover for touch devices', async () => {
    const wrapper = setup();
    await wrapper.trigger('mouseenter');
    await tick(6000);

    expect(author(wrapper)).toBe('Business Owner, Retail');
  });

  it('pauses while focused', async () => {
    const wrapper = setup();
    await wrapper.trigger('focusin');
    await tick(12000);
    expect(author(wrapper)).toBe('HR Manager, Manufacturer');

    await wrapper.trigger('focusout');
    await tick(6000);
    expect(author(wrapper)).toBe('Business Owner, Retail');
  });

  it('swipes in both directions', async () => {
    const wrapper = setup();
    await wrapper.trigger('pointerdown', { clientX: 100, clientY: 0 });
    await wrapper.trigger('pointerup', { clientX: 300, clientY: 0 });
    expect(author(wrapper)).toBe('Operations Manager, Logistics');

    await wrapper.trigger('pointerdown', { clientX: 300, clientY: 0 });
    await wrapper.trigger('pointerup', { clientX: 100, clientY: 0 });
    expect(author(wrapper)).toBe('HR Manager, Manufacturer');
  });

  it('stops rotating once unmounted', () => {
    const wrapper = setup();
    wrapper.unmount();

    expect(vi.getTimerCount()).toBe(0);
  });
});
