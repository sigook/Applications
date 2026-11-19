import { describe, expect, it } from 'vitest';
import ArrowPillCta from '@/modules/landing/components/shared/ui/ArrowPillCta.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('ArrowPillCta', () => {
  it('renders a button with the default style and arrow', () => {
    const wrapper = mountWithPlugins(ArrowPillCta, { slots: { default: 'Apply' } });

    expect(wrapper.element.tagName).toBe('BUTTON');
    expect(wrapper.attributes('type')).toBe('button');
    expect(wrapper.classes()).toEqual(expect.arrayContaining(['arrow-pill--glass', 'arrow-pill--hover-navy', 'arrow-pill--md']));
    expect(wrapper.find('.arrow-pill__label').text()).toBe('Apply');
    expect(wrapper.find('.arrow-pill__arrow').exists()).toBe(true);
  });

  it('renders a router link', () => {
    const wrapper = mountWithPlugins(ArrowPillCta, { props: { to: '/apply', variant: 'solid', hoverVariant: 'red', size: 'lg' } });

    expect(wrapper.element.tagName).toBe('A');
    expect(wrapper.attributes('href')).toBe('/apply');
    expect(wrapper.classes()).toEqual(expect.arrayContaining(['arrow-pill--solid', 'arrow-pill--hover-red', 'arrow-pill--lg']));
  });

  it('renders an external anchor without arrow', () => {
    const wrapper = mountWithPlugins(ArrowPillCta, { props: { href: 'https://sigook.com', showArrow: false } });

    expect(wrapper.element.tagName).toBe('A');
    expect(wrapper.attributes('href')).toBe('https://sigook.com');
    expect(wrapper.find('.arrow-pill__arrow').exists()).toBe(false);
  });

  it('renders a submit button', () => {
    expect(mountWithPlugins(ArrowPillCta, { props: { type: 'submit' } }).attributes('type')).toBe('submit');
  });
});
