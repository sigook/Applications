import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { defineComponent, h, ref } from 'vue';
import { useCarousel, type UseCarouselOptions, type UseCarouselReturn } from '@/modules/landing/composables/useCarousel';

function mountCarousel<T>(items: readonly T[] | (() => readonly T[]), options?: UseCarouselOptions) {
  let api: UseCarouselReturn<T> | undefined;
  const Host = defineComponent({
    setup() {
      api = useCarousel(items, options);
      return () => h('div');
    },
  });
  const wrapper = mount(Host);
  if (!api) throw new Error('Composable did not run');
  return { wrapper, api };
}

function stubReducedMotion(matches: boolean): void {
  vi.stubGlobal('matchMedia', vi.fn((query: string) => ({ matches, media: query })));
}

describe('useCarousel', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    stubReducedMotion(false);
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('starts on the first item and exposes the count', () => {
    const { api } = mountCarousel(['a', 'b', 'c']);
    expect(api.currentIndex.value).toBe(0);
    expect(api.currentItem.value).toBe('a');
    expect(api.count.value).toBe(3);
  });

  it('auto-advances every interval and wraps around', () => {
    const { api } = mountCarousel(['a', 'b', 'c'], { intervalMs: 1000 });
    vi.advanceTimersByTime(1000);
    expect(api.currentItem.value).toBe('b');
    vi.advanceTimersByTime(2000);
    expect(api.currentItem.value).toBe('a');
  });

  it('uses a 5 second default interval', () => {
    const { api } = mountCarousel(['a', 'b']);
    vi.advanceTimersByTime(4999);
    expect(api.currentIndex.value).toBe(0);
    vi.advanceTimersByTime(1);
    expect(api.currentIndex.value).toBe(1);
  });

  it('does not start when autoStart is false until start is called', () => {
    const { api } = mountCarousel(['a', 'b'], { intervalMs: 100, autoStart: false });
    vi.advanceTimersByTime(500);
    expect(api.currentIndex.value).toBe(0);
    api.start();
    vi.advanceTimersByTime(100);
    expect(api.currentIndex.value).toBe(1);
  });

  it('respects prefers-reduced-motion', () => {
    stubReducedMotion(true);
    const { api } = mountCarousel(['a', 'b'], { intervalMs: 100 });
    vi.advanceTimersByTime(1000);
    expect(api.currentIndex.value).toBe(0);
    expect(window.matchMedia).toHaveBeenCalledWith('(prefers-reduced-motion: reduce)');
  });

  it('auto-advances when matchMedia is unavailable', () => {
    vi.stubGlobal('matchMedia', undefined);
    const { api } = mountCarousel(['a', 'b'], { intervalMs: 100 });
    vi.advanceTimersByTime(100);
    expect(api.currentIndex.value).toBe(1);
  });

  it('jumps to a valid index and restarts the timer', () => {
    const { api } = mountCarousel(['a', 'b', 'c'], { intervalMs: 1000 });
    vi.advanceTimersByTime(900);
    api.goTo(2);
    expect(api.currentItem.value).toBe('c');
    vi.advanceTimersByTime(900);
    expect(api.currentIndex.value).toBe(2);
    vi.advanceTimersByTime(100);
    expect(api.currentIndex.value).toBe(0);
  });

  it('ignores out of range indexes', () => {
    const { api } = mountCarousel(['a', 'b'], { autoStart: false });
    api.goTo(-1);
    api.goTo(2);
    expect(api.currentIndex.value).toBe(0);
  });

  it('advances manually with next', () => {
    const { api } = mountCarousel(['a', 'b'], { autoStart: false });
    api.next();
    expect(api.currentItem.value).toBe('b');
    api.next();
    expect(api.currentItem.value).toBe('a');
  });

  it('does nothing on next when there are no items', () => {
    const { api } = mountCarousel<string>([], { autoStart: false });
    api.next();
    expect(api.currentIndex.value).toBe(0);
    expect(api.count.value).toBe(0);
  });

  it('reads items from a reactive getter', () => {
    const items = ref(['a']);
    const { api } = mountCarousel(() => items.value, { autoStart: false });
    expect(api.count.value).toBe(1);
    items.value = ['x', 'y'];
    expect(api.count.value).toBe(2);
    expect(api.currentItem.value).toBe('x');
  });

  it('stops advancing after stop and after unmount', () => {
    const first = mountCarousel(['a', 'b'], { intervalMs: 100 });
    first.api.stop();
    first.api.stop();
    vi.advanceTimersByTime(500);
    expect(first.api.currentIndex.value).toBe(0);

    const second = mountCarousel(['a', 'b'], { intervalMs: 100 });
    second.wrapper.unmount();
    vi.advanceTimersByTime(500);
    expect(second.api.currentIndex.value).toBe(0);
    expect(vi.getTimerCount()).toBe(0);
  });
});
