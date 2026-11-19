import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import PartnerProcessSection from '@/modules/landing/components/Partner/PartnerProcessSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

class FakeIntersectionObserver {
  static last: FakeIntersectionObserver | null = null;
  readonly observed: Element[] = [];
  readonly disconnect = vi.fn();

  constructor(readonly callback: IntersectionObserverCallback) {
    FakeIntersectionObserver.last = this;
  }

  observe(target: Element): void {
    this.observed.push(target);
  }

  emit(entries: { target: Element; isIntersecting: boolean }[]): void {
    this.callback(entries as unknown as IntersectionObserverEntry[], this as unknown as IntersectionObserver);
  }
}

function visibility(wrapper: ReturnType<typeof mountWithPlugins>) {
  return wrapper.findAll('.partner-process__step').map((s) => s.classes().includes('partner-process__step--visible'));
}

describe('PartnerProcessSection', () => {
  beforeEach(() => {
    FakeIntersectionObserver.last = null;
    vi.stubGlobal('IntersectionObserver', FakeIntersectionObserver);
  });

  it('renders the four numbered steps', () => {
    const wrapper = mountWithPlugins(PartnerProcessSection);

    expect(wrapper.findAll('.partner-process__number').map((n) => n.text())).toEqual(['01', '02', '03', '04']);
    expect(wrapper.findAll('.partner-process__step-title').map((t) => t.text())).toEqual(['Apply', 'Onboard', 'Match', 'Earn']);
    expect(wrapper.findAll('.partner-process__step')[3].classes()).toContain('partner-process__step--red');
    expect(FakeIntersectionObserver.last?.observed).toHaveLength(4);
  });

  it('reveals each step as it enters the viewport', async () => {
    const wrapper = mountWithPlugins(PartnerProcessSection);
    const steps = wrapper.findAll('.partner-process__step').map((s) => s.element);
    FakeIntersectionObserver.last?.emit([
      { target: steps[0], isIntersecting: true },
      { target: steps[1], isIntersecting: false },
      { target: document.createElement('li'), isIntersecting: true },
    ]);
    await nextTick();

    expect(visibility(wrapper)).toEqual([true, false, false, false]);
  });

  it('shows the steps statically without IntersectionObserver support', () => {
    vi.stubGlobal('IntersectionObserver', undefined);
    const wrapper = mountWithPlugins(PartnerProcessSection);

    expect(visibility(wrapper)).toEqual([false, false, false, false]);
  });

  it('disconnects on unmount', () => {
    const wrapper = mountWithPlugins(PartnerProcessSection);
    wrapper.unmount();

    expect(FakeIntersectionObserver.last?.disconnect).toHaveBeenCalled();
  });
});
