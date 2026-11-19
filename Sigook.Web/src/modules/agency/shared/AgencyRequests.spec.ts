import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AgencyRequests from '@/modules/agency/shared/AgencyRequests.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyDetail } from '@/modules/agency/profile/types';

const { tableStub } = vi.hoisted(() => ({
  tableStub: { name: 'TableRequests', props: { agencyId: String, config: Object }, emits: ['onDataLoading'], template: '<div class="table-requests" />' },
}));

vi.mock('@/modules/agency/recruiting/requests/components/TableRequests.vue', () => ({ default: tableStub }));

function setup() {
  return mountWithPlugins(AgencyRequests, { props: { agency: { id: 'agency-1' } as AgencyDetail } });
}

describe('AgencyRequests', () => {
  it('lists the agency requests without the recruiting extras', () => {
    const table = setup().findComponent(tableStub);
    expect(table.props()).toEqual({
      agencyId: 'agency-1',
      config: {
        showAssignedToMe: false,
        showQuickActions: false,
        enableCheckable: false,
        showRecruiterModal: false,
        showSalesRepColumn: false,
        showNotesColumn: false,
      },
    });
  });

  it('follows the table loading state', async () => {
    const wrapper = setup();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(true);
    wrapper.findComponent(tableStub).vm.$emit('onDataLoading', false);
    await nextTick();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
