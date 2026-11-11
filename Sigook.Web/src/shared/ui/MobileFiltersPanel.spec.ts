import { describe, expect, it } from 'vitest';
import MobileFiltersPanel from '@/shared/ui/MobileFiltersPanel.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(props: Record<string, unknown> = {}) {
  return mountWithPlugins(MobileFiltersPanel, {
    props: { modelValue: true, ...props },
    slots: { default: '<input class="filter-input" />' },
    global: { stubs: { teleport: true } },
  });
}

function buttonByText(wrapper: ReturnType<typeof setup>, text: string) {
  const button = wrapper.findAll('button').find((b) => b.text() === text);
  if (!button) throw new Error(`Button ${text} not found`);
  return button;
}

describe('MobileFiltersPanel', () => {
  it('renders the filters and the default title', () => {
    const wrapper = setup();
    expect(wrapper.text()).toContain('Filters');
    expect(wrapper.find('.filter-input').exists()).toBe(true);
  });

  it('shows the active filter count only when there are filters', () => {
    expect(setup().find('.filters-panel__count').exists()).toBe(false);
    expect(setup({ activeCount: 3 }).find('.filters-panel__count').text()).toBe('3');
  });

  it('applies and closes', async () => {
    const wrapper = setup();
    await buttonByText(wrapper, 'Apply').trigger('click');

    expect(wrapper.emitted('apply')).toHaveLength(1);
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });

  it('clears without closing', async () => {
    const wrapper = setup();
    await buttonByText(wrapper, 'Clear all').trigger('click');

    expect(wrapper.emitted('clear')).toHaveLength(1);
    expect(wrapper.emitted('update:modelValue')).toBeUndefined();
  });
});
