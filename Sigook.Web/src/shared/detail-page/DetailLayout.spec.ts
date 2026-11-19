import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import DetailLayout from '@/shared/detail-page/DetailLayout.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { DetailSection } from '@/shared/detail-page/types';

const sections: DetailSection[] = [
  { id: 'overview', label: 'Overview' },
  { id: 'workers', label: 'Workers', count: 3 },
  { id: 'notes', label: 'Notes', hint: 'New', count: 9 },
];

const cards = sections.map((s) => `<section id="${s.id}">${s.label}</section>`).join('');

class FakeObserver {
  static last: FakeObserver | null = null;
  observed: string[] = [];
  disconnect = vi.fn();
  constructor(readonly callback: (entries: Pick<IntersectionObserverEntry, 'isIntersecting' | 'target'>[]) => void) {
    FakeObserver.last = this;
  }
  observe(element: Element) {
    this.observed.push(element.id);
  }
  emit(id: string, isIntersecting = true) {
    const target = document.createElement('div');
    target.id = id;
    this.callback([{ isIntersecting, target }]);
  }
}

function observer() {
  if (!FakeObserver.last) throw new Error('observer not created');
  return FakeObserver.last;
}

function setup(props: { sections: DetailSection[]; ariaLabel?: string; railFirst?: boolean }, slots: Record<string, string> = { default: cards }) {
  vi.stubGlobal('IntersectionObserver', FakeObserver);
  return mountWithPlugins(DetailLayout, { props, slots });
}

function links(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-index-link');
}

function activeLabel(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('.detail-index-link.is-active span').text();
}

describe('DetailLayout', () => {
  it('renders the section index with counts and hints', () => {
    const wrapper = setup({ sections });

    expect(wrapper.find('nav').attributes('aria-label')).toBe('Page sections');
    expect(links(wrapper).map((l) => l.attributes('href'))).toEqual(['#overview', '#workers', '#notes']);
    expect(links(wrapper)[1].find('.detail-index-count').text()).toBe('3');
    expect(links(wrapper)[2].find('.detail-index-hint').text()).toBe('New');
    expect(links(wrapper)[2].find('.detail-index-count').exists()).toBe(false);
    expect(activeLabel(wrapper)).toBe('Overview');
    expect(wrapper.classes()).toContain('has-index');
  });

  it('hides the index for a single section', () => {
    const wrapper = setup({ sections: [sections[0]] });

    expect(wrapper.find('nav').exists()).toBe(false);
    expect(wrapper.classes()).not.toContain('has-index');
  });

  it('uses a custom aria label and rail', () => {
    const wrapper = setup({ sections, ariaLabel: 'Request sections', railFirst: true }, { default: cards, rail: '<p class="rail">Rail</p>' });

    expect(wrapper.find('nav').attributes('aria-label')).toBe('Request sections');
    expect(wrapper.find('aside .rail').exists()).toBe(true);
    expect(wrapper.classes()).toEqual(expect.arrayContaining(['has-rail', 'is-rail-first']));
  });

  it('has no rail without the slot', () => {
    const wrapper = setup({ sections });
    expect(wrapper.find('aside').exists()).toBe(false);
    expect(wrapper.classes()).not.toContain('has-rail');
  });

  it('scrolls to a section when its link is clicked', async () => {
    const scrollIntoView = vi.fn();
    const wrapper = setup({ sections });
    (document.getElementById('workers') as HTMLElement).scrollIntoView = scrollIntoView;

    await links(wrapper)[1].trigger('click');

    expect(scrollIntoView).toHaveBeenCalledWith({ behavior: 'smooth', block: 'start' });
    expect(activeLabel(wrapper)).toBe('Workers');
  });

  it('observes the sections and follows the visible one', async () => {
    const wrapper = setup({ sections });
    await flushPromises();

    expect(observer().observed).toEqual(['overview', 'workers', 'notes']);
    observer().emit('notes');
    await flushPromises();
    expect(activeLabel(wrapper)).toBe('Notes');
  });

  it('ignores hidden or unknown sections', async () => {
    const wrapper = setup({ sections });
    await flushPromises();

    observer().emit('notes', false);
    observer().emit('elsewhere');
    await flushPromises();
    expect(activeLabel(wrapper)).toBe('Overview');
  });

  it('re-observes when the sections change and disconnects on unmount', async () => {
    const wrapper = setup({ sections: sections.slice(0, 2) });
    await flushPromises();
    const current = observer();

    await wrapper.setProps({ sections });
    await flushPromises();
    expect(current.disconnect).toHaveBeenCalled();
    expect(current.observed).toEqual(['overview', 'workers', 'overview', 'workers', 'notes']);

    current.disconnect.mockClear();
    wrapper.unmount();
    expect(current.disconnect).toHaveBeenCalledOnce();
  });

  it('works without IntersectionObserver', async () => {
    vi.stubGlobal('IntersectionObserver', undefined);
    const wrapper = mountWithPlugins(DetailLayout, { props: { sections }, slots: { default: cards } });
    await wrapper.setProps({ sections: sections.slice(0, 2) });

    expect(links(wrapper)).toHaveLength(2);
  });
});
