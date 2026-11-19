import { describe, expect, it } from 'vitest';
import DashboardListRow from '@/modules/agency/sales/dashboard/components/DashboardListRow.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('DashboardListRow', () => {
  it('renders every slot', () => {
    const wrapper = mountWithPlugins(DashboardListRow, {
      slots: { avatar: 'AB', title: 'Acme', meta: 'ap@acme.test', trailing: '2h' },
    });

    expect(wrapper.find('.sd-row__avatar').text()).toBe('AB');
    expect(wrapper.find('.sd-row__title').text()).toBe('Acme');
    expect(wrapper.find('.sd-row__meta').text()).toBe('ap@acme.test');
    expect(wrapper.find('.sd-row__trailing').text()).toBe('2h');
  });

  it('omits avatar and trailing without slots', () => {
    const wrapper = mountWithPlugins(DashboardListRow, { slots: { title: 'Acme' } });

    expect(wrapper.find('.sd-row__avatar').exists()).toBe(false);
    expect(wrapper.find('.sd-row__trailing').exists()).toBe(false);
    expect(wrapper.find('.sd-row__meta').text()).toBe('');
  });
});
