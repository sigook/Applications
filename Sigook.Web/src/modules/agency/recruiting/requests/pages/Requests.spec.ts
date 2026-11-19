import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import Requests from '@/modules/agency/recruiting/requests/pages/Requests.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/requests/components/TableRequests.vue', () => ({
  default: {
    name: 'TableRequests',
    props: { totalItems: Number },
    emits: ['onDataLoading', 'update:totalItems'],
    template: '<div class="table-stub" />',
  },
}));

function setup() {
  return mountWithPlugins(Requests);
}

describe('Requests', () => {
  it('shows the recruiting header and starts loading', () => {
    const wrapper = setup();

    expect(wrapper.find('.page-header-title').text()).toBe('Requests 0');
    expect(wrapper.text()).toContain('Recruiting');
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(true);
  });

  it('reflects the table total and loading state', async () => {
    const wrapper = setup();
    const table = wrapper.findComponent({ name: 'TableRequests' });
    table.vm.$emit('update:totalItems', 42);
    table.vm.$emit('onDataLoading', false);
    await nextTick();

    expect(wrapper.find('.page-header-count').text()).toBe('42');
    expect(table.props('totalItems')).toBe(42);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
