import { describe, expect, it } from 'vitest';
import DashboardList from '@/modules/agency/sales/dashboard/components/DashboardList.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(props: { isEmpty: boolean; loading?: boolean; emptyText?: string }) {
  return mountWithPlugins(DashboardList, { props, slots: { default: '<p class="content">Rows</p>' } });
}

describe('DashboardList', () => {
  it('shows a spinner while loading with nothing to show', () => {
    const wrapper = setup({ isEmpty: true, loading: true });

    expect(wrapper.find('.sd-list__loading').exists()).toBe(true);
    expect(wrapper.find('.sd-list__empty').exists()).toBe(false);
  });

  it('shows the default empty text', () => {
    expect(setup({ isEmpty: true }).find('.sd-list__empty').text()).toBe('Nothing yet');
  });

  it('shows a custom empty text', () => {
    expect(setup({ isEmpty: true, emptyText: 'No deals' }).find('.sd-list__empty').text()).toBe('No deals');
  });

  it('keeps the rows while reloading', () => {
    const wrapper = setup({ isEmpty: false, loading: true });

    expect(wrapper.find('.content').exists()).toBe(true);
    expect(wrapper.find('.sd-list__loading').exists()).toBe(false);
  });
});
