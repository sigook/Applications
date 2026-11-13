import { describe, it, expect } from 'vitest';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import HoverTooltip from '@/shared/ui/HoverTooltip.vue';

function mountTooltip(label?: string | null) {
  return mountWithPlugins(HoverTooltip, {
    props: { label },
    slots: { default: '<span class="target">x</span>' },
  });
}

function tooltip() {
  return document.body.querySelector('.hover-tooltip');
}

describe('HoverTooltip', () => {
  it('renders nothing in the body until hovered', () => {
    mountTooltip('Open');
    expect(tooltip()).toBeNull();
  });

  it('shows the label on hover and hides it on leave', async () => {
    const wrapper = mountTooltip('Open');
    await wrapper.trigger('mouseenter');
    expect(tooltip()?.textContent).toBe('Open');
    expect(tooltip()?.getAttribute('role')).toBe('tooltip');
    await wrapper.trigger('mouseleave');
    expect(tooltip()).toBeNull();
  });

  it('places itself below the trigger when there is no room above', async () => {
    const wrapper = mountTooltip('Open');
    await wrapper.trigger('mouseenter');
    expect(tooltip()?.classList.contains('is-bottom')).toBe(true);
  });

  it('opens on focus and closes on blur', async () => {
    const wrapper = mountTooltip('Open');
    await wrapper.trigger('focusin');
    expect(tooltip()).not.toBeNull();
    await wrapper.trigger('focusout');
    expect(tooltip()).toBeNull();
  });

  it('closes when the page scrolls', async () => {
    const wrapper = mountTooltip('Open');
    await wrapper.trigger('mouseenter');
    window.dispatchEvent(new Event('scroll'));
    await wrapper.vm.$nextTick();
    expect(tooltip()).toBeNull();
  });

  it('stays hidden without a label', async () => {
    const wrapper = mountTooltip(null);
    await wrapper.trigger('mouseenter');
    expect(tooltip()).toBeNull();
  });
});
