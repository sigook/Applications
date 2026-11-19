import { beforeEach, describe, expect, it, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { defineComponent, h } from 'vue';
import {
  useRevealOnScroll,
  type UseRevealOnScrollOptions,
  type UseRevealOnScrollReturn,
} from '@/modules/landing/composables/useRevealOnScroll';

class FakeIntersectionObserver {
  static instances: FakeIntersectionObserver[] = [];
  readonly observed: Element[] = [];
  readonly disconnect = vi.fn();

  constructor(
    readonly callback: IntersectionObserverCallback,
    readonly options: IntersectionObserverInit = {},
  ) {
    FakeIntersectionObserver.instances.push(this);
  }

  observe(target: Element): void {
    this.observed.push(target);
  }

  emit(isIntersecting: boolean): void {
    const entry = { isIntersecting } as IntersectionObserverEntry;
    this.callback([entry], this as unknown as IntersectionObserver);
  }
}

function mountHost(options?: UseRevealOnScrollOptions, bindEl = true) {
  let api: UseRevealOnScrollReturn | undefined;
  const Host = defineComponent({
    setup() {
      api = useRevealOnScroll(options);
      const capture = (el: unknown): void => {
        if (api && el instanceof HTMLElement) api.el.value = el;
      };
      return () => h('div', bindEl ? { ref: capture } : {});
    },
  });
  const wrapper = mount(Host);
  if (!api) throw new Error('Composable did not run');
  return { wrapper, api, observer: FakeIntersectionObserver.instances.at(0) };
}

describe('useRevealOnScroll', () => {
  beforeEach(() => {
    FakeIntersectionObserver.instances = [];
    vi.stubGlobal('IntersectionObserver', FakeIntersectionObserver);
  });

  it('observes the element with the default options', () => {
    const { wrapper, api, observer } = mountHost();
    expect(api.visible.value).toBe(false);
    expect(observer?.observed).toEqual([wrapper.element]);
    expect(observer?.options).toEqual({ threshold: 0.15, rootMargin: '0px 0px -10% 0px' });
  });

  it('passes custom threshold and root margin', () => {
    const { observer } = mountHost({ threshold: 0.5, rootMargin: '10px' });
    expect(observer?.options).toEqual({ threshold: 0.5, rootMargin: '10px' });
  });

  it('tracks intersection both ways by default', () => {
    const { api, observer } = mountHost();
    observer?.emit(true);
    expect(api.visible.value).toBe(true);
    observer?.emit(false);
    expect(api.visible.value).toBe(false);
    expect(observer?.disconnect).not.toHaveBeenCalled();
  });

  it('reveals once and disconnects when once is set', () => {
    const { api, observer } = mountHost({ once: true });
    observer?.emit(false);
    expect(api.visible.value).toBe(false);
    expect(observer?.disconnect).not.toHaveBeenCalled();
    observer?.emit(true);
    expect(api.visible.value).toBe(true);
    expect(observer?.disconnect).toHaveBeenCalledTimes(1);
  });

  it('does not observe without a bound element', () => {
    const { observer } = mountHost({}, false);
    expect(observer).toBeUndefined();
  });

  it('does nothing when IntersectionObserver is unavailable', () => {
    vi.stubGlobal('IntersectionObserver', undefined);
    const { api, wrapper } = mountHost();
    expect(api.visible.value).toBe(false);
    expect(() => { wrapper.unmount(); }).not.toThrow();
  });

  it('disconnects the observer on unmount', () => {
    const { wrapper, observer } = mountHost();
    wrapper.unmount();
    expect(observer?.disconnect).toHaveBeenCalledTimes(1);
  });
});
