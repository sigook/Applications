import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import OurHistorySection from '@/modules/landing/components/About/OurHistorySection.vue';
import milestones from '@/modules/landing/data/historyMilestones.json';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

class FakeIntersectionObserver {
  static last: FakeIntersectionObserver | null = null;
  readonly disconnect = vi.fn();

  constructor(readonly callback: IntersectionObserverCallback) {
    FakeIntersectionObserver.last = this;
  }

  observe(): void {
    return undefined;
  }

  emit(isIntersecting: boolean): void {
    this.callback([{ isIntersecting } as IntersectionObserverEntry], this as unknown as IntersectionObserver);
  }
}

describe('OurHistorySection', () => {
  beforeEach(() => {
    FakeIntersectionObserver.last = null;
    vi.stubGlobal('IntersectionObserver', FakeIntersectionObserver);
  });

  it('renders every milestone with a staggered delay', () => {
    const wrapper = mountWithPlugins(OurHistorySection);
    const items = wrapper.findAll('.our-history__milestone');

    expect(items.map((m) => m.find('.our-history__year').text())).toEqual(milestones.map((m) => m.year));
    expect(items[1].attributes('style')).toContain('transition-delay: 100ms');
    expect(wrapper.findAll('.our-history__letter-mark').map((l) => l.text())).toEqual(['SI', 'GO', 'OK']);
  });

  it('reveals the timeline once it scrolls into view', async () => {
    const wrapper = mountWithPlugins(OurHistorySection);
    expect(wrapper.find('.our-history__milestone').classes()).not.toContain('is-revealed');

    FakeIntersectionObserver.last?.emit(true);
    await nextTick();
    expect(wrapper.findAll('.our-history__milestone').every((m) => m.classes().includes('is-revealed'))).toBe(true);
    expect(FakeIntersectionObserver.last?.disconnect).toHaveBeenCalled();
  });
});
