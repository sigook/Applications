import { describe, expect, it } from 'vitest';
import BulkSelectionBar from '@/modules/agency/shared/request-groups/BulkSelectionBar.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(count: number) {
  return mountWithPlugins(BulkSelectionBar, {
    props: { count },
    slots: { default: '<button class="bulk-action">Approve</button>' },
  });
}

describe('BulkSelectionBar', () => {
  it('is hidden when nothing is selected', () => {
    expect(setup(0).find('.bulk-selection').exists()).toBe(false);
  });

  it('shows the count and the slotted actions', () => {
    const wrapper = setup(3);
    expect(wrapper.find('.bulk-selection-count').text()).toBe('3 selected');
    expect(wrapper.find('.bulk-action').exists()).toBe(true);
  });

  it('emits clear', async () => {
    const wrapper = setup(2);
    const clear = wrapper.findAll('button').find((b) => b.text() === 'Clear');
    await clear?.trigger('click');
    expect(wrapper.emitted('clear')).toHaveLength(1);
  });
});
