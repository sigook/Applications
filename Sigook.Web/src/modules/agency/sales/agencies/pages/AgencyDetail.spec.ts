import { describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import AgencyDetail from '@/modules/agency/sales/agencies/pages/AgencyDetail.vue';
import { getAgency } from '@/modules/agency/sales/agencies/api';
import { showAlertError } from '@/shared/utils/toast';
import { createTestRouter } from '@/testing/mountWithPlugins';
import type { AgencyDetail as AgencyDetailModel } from '@/modules/agency/profile/types';

vi.mock('@/modules/agency/sales/agencies/api', () => ({ getAgency: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/shared/AgencyRequests.vue', () => ({
  default: { name: 'AgencyRequests', props: { agency: Object }, template: '<div class="agency-requests" />' },
}));

const agency = { id: 'agency-1', fullName: 'NORTH STAFFING' } as AgencyDetailModel;

async function setup(path = '/sales/agencies/agency-1') {
  const router = createTestRouter();
  router.addRoute({ path: '/sales/agencies/:id', component: { render: () => null } });
  await router.push(path);
  const wrapper = mount(AgencyDetail, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, router };
}

describe('AgencyDetail', () => {
  it('loads the agency from the route and shows its requests', async () => {
    vi.mocked(getAgency).mockResolvedValue(agency);
    const { wrapper } = await setup();

    expect(getAgency).toHaveBeenCalledExactlyOnceWith('agency-1');
    expect(wrapper.find('h2').text()).toBe('north staffing');
    expect(wrapper.findComponent({ name: 'AgencyRequests' }).props('agency')).toEqual(agency);
  });

  it('keeps the tab in the url', async () => {
    vi.mocked(getAgency).mockResolvedValue(agency);
    const { wrapper, router } = await setup('/sales/agencies/agency-1?tab=Requests');
    wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', 'Requests');
    await flushPromises();

    expect(router.currentRoute.value.fullPath).toBe('/sales/agencies/agency-1?tab=Requests');
  });

  it('shows the error when the agency cannot load', async () => {
    const error = new Error('not found');
    vi.mocked(getAgency).mockRejectedValue(error);
    const { wrapper } = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('h2').exists()).toBe(false);
  });

  it('shows the loading overlay until the agency arrives', async () => {
    let resolve: (value: AgencyDetailModel) => void = () => undefined;
    vi.mocked(getAgency).mockReturnValue(new Promise((r) => { resolve = r; }));
    const { wrapper } = await setup();

    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(true);
    resolve(agency);
    await flushPromises();

    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('hides the loading overlay when the agency cannot load', async () => {
    vi.mocked(getAgency).mockRejectedValue(new Error('not found'));
    const { wrapper } = await setup();

    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
