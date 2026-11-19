import { beforeEach, describe, expect, it, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { defineComponent, h, nextTick } from 'vue';
import { useElementSize, type UseElementSizeReturn } from '@/shared/composables/useElementSize';

class FakeResizeObserver {
  static instances: FakeResizeObserver[] = [];
  readonly observed: Element[] = [];
  readonly disconnect = vi.fn();

  constructor(readonly callback: ResizeObserverCallback) {
    FakeResizeObserver.instances.push(this);
  }

  observe(target: Element): void {
    this.observed.push(target);
  }

  unobserve(): void {}

  trigger(): void {
    this.callback([], this as unknown as ResizeObserver);
  }
}

let frames: FrameRequestCallback[] = [];

function setSize(el: HTMLElement, width: number, height: number): void {
  Object.defineProperty(el, 'clientWidth', { configurable: true, value: width });
  Object.defineProperty(el, 'clientHeight', { configurable: true, value: height });
}

function mountHost(options: { fallbackWidth?: number; fallbackHeight?: number; bindEl?: boolean; size?: [number, number] } = {}) {
  const { bindEl = true, size } = options;
  let api: UseElementSizeReturn | undefined;
  const Host = defineComponent({
    setup() {
      api = useElementSize(options.fallbackWidth, options.fallbackHeight);
      const capture = (el: unknown): void => {
        if (!(el instanceof HTMLElement)) return;
        if (size) setSize(el, size[0], size[1]);
        if (api) api.el.value = el;
      };
      return () => h('div', bindEl ? { ref: capture } : {});
    },
  });
  const wrapper = mount(Host, { attachTo: document.body });
  if (!api) throw new Error('Composable did not run');
  return { wrapper, api };
}

describe('useElementSize', () => {
  beforeEach(() => {
    FakeResizeObserver.instances = [];
    frames = [];
    vi.stubGlobal('ResizeObserver', FakeResizeObserver);
    vi.stubGlobal('requestAnimationFrame', vi.fn((cb: FrameRequestCallback) => frames.push(cb)));
    vi.stubGlobal('cancelAnimationFrame', vi.fn());
  });

  it('measures the element on mount and observes it', () => {
    const { api, wrapper } = mountHost({ size: [320, 180] });
    expect(api.width.value).toBe(320);
    expect(api.height.value).toBe(180);
    expect(FakeResizeObserver.instances).toHaveLength(1);
    expect(FakeResizeObserver.instances[0].observed).toEqual([wrapper.element]);
  });

  it('falls back to the given size when the element reports zero', () => {
    const { api } = mountHost({ fallbackWidth: 600, fallbackHeight: 300, size: [0, 0] });
    expect(api.width.value).toBe(600);
    expect(api.height.value).toBe(300);
  });

  it('re-measures on the next animation frame after a resize', async () => {
    const { api, wrapper } = mountHost({ size: [100, 50] });
    setSize(wrapper.element as HTMLElement, 400, 200);
    FakeResizeObserver.instances[0].trigger();
    expect(cancelAnimationFrame).toHaveBeenCalled();
    expect(api.width.value).toBe(100);
    frames.forEach((cb) => { cb(0); });
    await nextTick();
    expect(api.width.value).toBe(400);
    expect(api.height.value).toBe(200);
  });

  it('keeps the fallback values and skips observing without an element', () => {
    const { api } = mountHost({ fallbackWidth: 10, fallbackHeight: 20, bindEl: false });
    expect(api.width.value).toBe(10);
    expect(api.height.value).toBe(20);
    expect(FakeResizeObserver.instances).toHaveLength(0);
  });

  it('ignores a scheduled measure once the element ref is cleared', () => {
    const { api, wrapper } = mountHost({ size: [100, 50] });
    FakeResizeObserver.instances[0].trigger();
    api.el.value = null;
    setSize(wrapper.element as HTMLElement, 999, 999);
    frames.forEach((cb) => { cb(0); });
    expect(api.width.value).toBe(100);
  });

  it('measures once without ResizeObserver support', () => {
    vi.stubGlobal('ResizeObserver', undefined);
    const { api } = mountHost({ size: [250, 125] });
    expect(api.width.value).toBe(250);
    expect(FakeResizeObserver.instances).toHaveLength(0);
  });

  it('disconnects the observer and cancels the pending frame on unmount', () => {
    const { wrapper } = mountHost({ size: [100, 50] });
    const observer = FakeResizeObserver.instances[0];
    wrapper.unmount();
    expect(observer.disconnect).toHaveBeenCalledTimes(1);
    expect(cancelAnimationFrame).toHaveBeenCalled();
  });
});
