import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Agencies from '@/modules/agency/sales/agencies/pages/Agencies.vue';
import { getAgenciesList } from '@/modules/agency/sales/agencies/api';
import { useAgencyStore } from '@/modules/agency/store';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyListFilter, AgencyListItem } from '@/modules/agency/sales/agencies/types';

vi.mock('@/modules/agency/sales/agencies/api', () => ({ getAgenciesList: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const north: AgencyListItem = {
  id: 'agency-1',
  fullName: 'North Staffing',
  email: 'ops@north.test',
  phoneNumber: '4165550199',
  isActive: true,
  agencyType: 3,
  createdAt: '2026-01-01T12:00:00',
};

let pinia: Pinia;

async function setup(items: (AgencyListItem & { locations?: string[] })[] = [north]) {
  vi.mocked(getAgenciesList).mockResolvedValue({ items, totalItems: 7, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(Agencies, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

function lastFilter(): AgencyListFilter {
  const call = vi.mocked(getAgenciesList).mock.calls.at(-1);
  if (!call) throw new Error('not called');
  return call[0];
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Agencies', () => {
  it('loads agencies by name and keeps the filter', async () => {
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 0, isDescending: false, pageIndex: 1 });
    expect(useAgencyStore().agencyListFilter).toMatchObject({ sortBy: 0 });
    expect(wrapper.find('.page-header').text()).toContain('7');
  });

  it('restores the stored filter', async () => {
    useAgencyStore().updateAgencyListFilter({ sortBy: 1, isDescending: true, fullName: 'nor' });
    await setup();

    expect(lastFilter()).toMatchObject({ sortBy: 1, isDescending: true, fullName: 'nor' });
  });

  it('shows the stored agency types in the type filter', async () => {
    useAgencyStore().updateAgencyListFilter({ sortBy: 0, isDescending: false, agencyTypes: [3] });
    const wrapper = await setup();

    expect(lastFilter()).toMatchObject({ agencyTypes: [3] });
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([{ value: 3, label: 'Business Partner' }]);
  });

  it('renders the agency with a link, email and type', async () => {
    const wrapper = await setup();
    const row = wrapper.find('tbody tr');

    expect(row.find('a').attributes('href')).toBe('/sales/agencies/agency-1');
    expect(row.text()).toContain('North Staffing');
    expect(row.text()).toContain('ops@north.test');
    expect(row.find('.tag').text()).toBe('Business Partner');
  });

  it('shows two locations and a see details hint', async () => {
    const wrapper = await setup([{ ...north, locations: ['Toronto', 'Ottawa', 'Hamilton'] }]);
    const text = wrapper.find('tbody tr').text();

    expect(text).toContain('Toronto');
    expect(text).toContain('Ottawa');
    expect(text).not.toContain('Hamilton');
    expect(text).toContain('See details...');
  });

  it('filters by agency type', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ value: 2, label: 'Regular' }]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ agencyTypes: [2], pageIndex: 1 });
  });
});
