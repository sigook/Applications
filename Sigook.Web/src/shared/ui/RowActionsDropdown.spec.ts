import { describe, it, expect, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import RowActionsDropdown from '@/shared/ui/RowActionsDropdown.vue';

function mountActions() {
  return mountWithPlugins(RowActionsDropdown, {
    slots: { default: '<b-dropdown-item class="edit">Edit</b-dropdown-item>' },
  });
}

describe('RowActionsDropdown', () => {
  it('renders only the trigger until clicked', () => {
    const wrapper = mountActions();
    expect(wrapper.find('button[aria-label="Row actions"]').exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'BDropdown' }).exists()).toBe(false);
    expect(document.body.querySelector('.edit')).toBeNull();
  });

  it('mounts the dropdown and opens it on the first click', async () => {
    vi.useFakeTimers();
    const wrapper = mountActions();
    await wrapper.find('button').trigger('click');
    await flushPromises();
    vi.runAllTimers();
    await flushPromises();
    vi.useRealTimers();
    const dropdown = wrapper.findComponent({ name: 'BDropdown' });
    expect(dropdown.exists()).toBe(true);
    expect(dropdown.vm.$data.isActive).toBe(true);
    expect(document.body.querySelector('.edit')).not.toBeNull();
  });
});
