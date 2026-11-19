import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import TertiaryCard from '@/modules/landing/components/shared/cards/TertiaryCard.vue';
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

describe('TertiaryCard', () => {
  beforeEach(() => {
    FakeIntersectionObserver.last = null;
    vi.stubGlobal('IntersectionObserver', FakeIntersectionObserver);
  });

  it('renders only the provided parts', () => {
    const wrapper = mountWithPlugins(TertiaryCard);

    expect(wrapper.classes()).toContain('tertiary-card--cyan');
    expect(wrapper.find('.tertiary-card__icon').exists()).toBe(false);
    expect(wrapper.find('.tertiary-card__eyebrow').exists()).toBe(false);
    expect(wrapper.find('.tertiary-card__title').exists()).toBe(false);
    expect(wrapper.find('.tertiary-card__body').exists()).toBe(false);
    expect(wrapper.find('.tertiary-card__list').exists()).toBe(false);
    expect(wrapper.find('.tertiary-card__button-wrap').exists()).toBe(false);
  });

  it('renders every part', () => {
    const wrapper = mountWithPlugins(TertiaryCard, {
      props: { variant: 'blue', eyebrow: 'Sector', title: 'Logistics', list: ['Pickers'], delay: 100 },
      slots: { icon: '<i />', default: 'Body', button: '<button>Go</button>' },
    });

    expect(wrapper.classes()).toContain('tertiary-card--blue');
    expect(wrapper.attributes('style')).toContain('transition-delay: 100ms');
    expect(wrapper.find('.tertiary-card__eyebrow').text()).toBe('Sector');
    expect(wrapper.find('.tertiary-card__title').text()).toBe('Logistics');
    expect(wrapper.find('.tertiary-card__body').text()).toBe('Body');
    expect(wrapper.findAll('.tertiary-card__list li').map((li) => li.text())).toEqual(['Pickers']);
    expect(wrapper.find('.tertiary-card__button-wrap button').exists()).toBe(true);
  });

  it('reveals itself when scrolled into view', async () => {
    const wrapper = mountWithPlugins(TertiaryCard, { props: { title: 'Retail' } });
    FakeIntersectionObserver.last?.emit(true);
    await nextTick();

    expect(wrapper.classes()).toContain('tertiary-card--visible');
  });
});
