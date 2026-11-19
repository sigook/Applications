import { afterEach, describe, expect, it, vi } from 'vitest';
import IndustriesCarousel from '@/modules/landing/components/shared/sections/IndustriesCarousel.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(hover = false, props: { eyebrow?: string; learnMoreTitle?: string; learnMoreSubtitle?: string } = {}) {
  vi.useFakeTimers();
  vi.stubGlobal('matchMedia', vi.fn((query: string) => ({ matches: query === '(hover: hover)' ? hover : false, media: query })));
  return mountWithPlugins(IndustriesCarousel, {
    props: { heading: 'Industries we serve.', headingAccent: 'Every sector.', subtitle: 'Pick yours.', ...props },
  });
}

type Wrapper = ReturnType<typeof setup>;

function track(wrapper: Wrapper) {
  return wrapper.find('.industries-carousel__track').attributes('style');
}

describe('IndustriesCarousel', () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it('renders the header and groups the industries in slides of three', () => {
    const wrapper = setup();

    expect(wrapper.find('.industries-carousel__eyebrow').text()).toBe('Industries');
    expect(wrapper.find('.industries-carousel__heading').text()).toBe('Industries we serve. Every sector.');
    expect(wrapper.findAll('.industries-carousel__slide')).toHaveLength(4);
    expect(wrapper.findAll('.industries-carousel__slide')[0].findAll('.tertiary-card__title').map((t) => t.text())).toEqual(['Automotive', 'Aviation', 'Construction']);
    expect(wrapper.findAll('.slider-dots__dot')).toHaveLength(4);
    expect(wrapper.find('.industries-carousel__cta-link').attributes('href')).toBe('/industries');
    expect(wrapper.find('.industries-carousel__cta-link').text()).toContain('Learn more');
    expect(wrapper.find('.industries-carousel__cta-link').text()).toContain('Explore all sectors');
  });

  it('uses custom eyebrow and learn more texts', () => {
    const wrapper = setup(false, { eyebrow: 'By Industry', learnMoreTitle: 'Full directory', learnMoreSubtitle: 'See all sectors' });

    expect(wrapper.find('.industries-carousel__eyebrow').text()).toBe('By Industry');
    expect(wrapper.find('.industries-carousel__cta-link').text()).toContain('Full directory');
    expect(wrapper.find('.industries-carousel__cta-link').text()).toContain('See all sectors');
  });

  it('moves with the arrows and wraps around', async () => {
    const wrapper = setup();
    await wrapper.find('.industries-carousel__nav--prev').trigger('click');
    expect(track(wrapper)).toContain('translateX(-300%)');

    await wrapper.find('.industries-carousel__nav--next').trigger('click');
    expect(track(wrapper)).toContain('translateX(-0%)');
    await wrapper.find('.industries-carousel__nav--next').trigger('click');
    expect(track(wrapper)).toContain('translateX(-100%)');
    expect(wrapper.findAll('.industries-carousel__slide').map((s) => s.attributes('aria-hidden'))).toEqual(['true', 'false', 'true', 'true']);
  });

  it('jumps to the clicked dot', async () => {
    const wrapper = setup();
    await wrapper.findAll('.slider-dots__dot')[2].trigger('click');

    expect(track(wrapper)).toContain('translateX(-200%)');
  });

  it('auto-advances', async () => {
    const wrapper = setup();
    vi.advanceTimersByTime(6500);
    await wrapper.vm.$nextTick();

    expect(track(wrapper)).toContain('translateX(-100%)');
  });

  it('pauses on hover on hover-capable devices', async () => {
    const wrapper = setup(true);
    await wrapper.find('.industries-carousel__row').trigger('mouseenter');
    vi.advanceTimersByTime(13000);
    await wrapper.vm.$nextTick();
    expect(track(wrapper)).toContain('translateX(-0%)');

    await wrapper.find('.industries-carousel__row').trigger('mouseleave');
    vi.advanceTimersByTime(6500);
    await wrapper.vm.$nextTick();
    expect(track(wrapper)).toContain('translateX(-100%)');
  });

  it('ignores hover on touch devices', async () => {
    const wrapper = setup(false);
    await wrapper.find('.industries-carousel__row').trigger('mouseenter');
    vi.advanceTimersByTime(6500);
    await wrapper.vm.$nextTick();

    expect(track(wrapper)).toContain('translateX(-100%)');
  });

  it('pauses while focused', async () => {
    const wrapper = setup();
    await wrapper.find('.industries-carousel__row').trigger('focusin');
    vi.advanceTimersByTime(13000);
    await wrapper.vm.$nextTick();
    expect(track(wrapper)).toContain('translateX(-0%)');

    await wrapper.find('.industries-carousel__row').trigger('focusout');
    vi.advanceTimersByTime(6500);
    await wrapper.vm.$nextTick();
    expect(track(wrapper)).toContain('translateX(-100%)');
  });

  it('swipes between slides', async () => {
    const wrapper = setup();
    const viewport = wrapper.find('.industries-carousel__viewport');
    await viewport.trigger('pointerdown', { clientX: 300, clientY: 100 });
    await viewport.trigger('pointerup', { clientX: 100, clientY: 100 });
    expect(track(wrapper)).toContain('translateX(-100%)');

    await viewport.trigger('pointerdown', { clientX: 100, clientY: 100 });
    await viewport.trigger('pointerup', { clientX: 300, clientY: 100 });
    expect(track(wrapper)).toContain('translateX(-0%)');
  });
});
