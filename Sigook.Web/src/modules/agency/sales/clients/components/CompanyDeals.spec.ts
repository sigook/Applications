import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { computed, nextTick, ref } from 'vue';
import CompanyDeals from '@/modules/agency/sales/clients/components/CompanyDeals.vue';
import { deleteDeal, getDeals } from '@/modules/agency/sales/clients/api';
import { DealSortBy, DealStatus, DealType } from '@/modules/agency/sales/clients/types';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { Deal, DealFilter } from '@/modules/agency/sales/clients/types';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const admin = ref(false);
const owners = ref<AgencyPersonnelListItem[]>([]);
const loadOwners = vi.fn();

vi.mock('@/modules/agency/shared/useSalesOwners', () => ({
  useSalesOwners: () => ({ isAdmin: computed(() => admin.value), owners, loadOwners }),
}));
vi.mock('@/modules/agency/sales/clients/api', () => ({ deleteDeal: vi.fn(), getDeals: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/sales/clients/components/DealModal.vue', () => ({
  default: { name: 'DealModal', props: { modelValue: Boolean, deal: Object, client: Object }, emits: ['saved', 'update:modelValue'], template: '<div class="deal-modal-stub" />' },
}));

const deal: Deal = {
  id: 'deal-1',
  title: 'Warehouse staffing',
  companyProfileId: 'company-1',
  companyName: 'Acme',
  ownerId: 'user-1',
  owner: 'Rita Sales',
  date: '2026-11-10T12:00:00',
  value: 25000,
  type: DealType.TempToPerm,
  status: DealStatus.UnderReview,
  createdAt: '2026-11-01T12:00:00',
  updatedAt: '2026-11-01T12:00:00',
};

const company = { id: 'company-1', fullName: 'Acme' };

async function setup() {
  vi.mocked(getDeals).mockResolvedValue({ items: [deal], totalItems: 1, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(CompanyDeals, { props: { company } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function lastFilter(): DealFilter {
  const call = vi.mocked(getDeals).mock.calls.at(-1);
  if (!call) throw new Error('not called');
  return call[1];
}

function modal(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'DealModal' });
}

function rowButtons(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr td:last-child button');
}

beforeEach(() => {
  admin.value = false;
  owners.value = [];
});

describe('CompanyDeals', () => {
  it('loads the company deals newest first', async () => {
    const wrapper = await setup();

    expect(loadOwners).toHaveBeenCalledOnce();
    expect(getDeals).toHaveBeenCalledWith('company-1', expect.objectContaining({ sortBy: DealSortBy.Date, isDescending: true, pageIndex: 1, ownerId: null, type: null }));
    expect(wrapper.findAll('tbody tr td').map((c) => c.text()).slice(0, 6)).toEqual([
      'Warehouse staffing', '$25,000.00', 'Temp to Perm', 'Under Review', 'Rita Sales', 'Tue 10 November, 2026',
    ]);
  });

  it('sorts by value', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTable' }).vm.$emit('sort', 'value', 'asc');
    await flushPromises();

    expect(lastFilter()).toMatchObject({ sortBy: DealSortBy.Value, isDescending: false });
  });

  it('filters by type, statuses and date range', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BSelect' }).vm.$emit('update:modelValue', DealType.Permanent);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ type: DealType.Permanent, pageIndex: 1 });

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: DealStatus.Sent, value: 'Sent' }]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ statuses: [DealStatus.Sent] });

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', []);
    await flushPromises();
    expect(lastFilter().statuses).toBeUndefined();

    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 30);
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ dateFrom: from.toISOString(), dateTo: to.toISOString() });

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ dateFrom: null, dateTo: null });
  });

  it('lets admins filter by owner', async () => {
    admin.value = true;
    owners.value = [
      { id: 'p-1', userId: 'user-1', name: 'Rita Sales', email: 'rita@test', role: 'sales', agencyIds: [], agencies: [] },
      { id: 'p-2', userId: 'user-2', name: '', email: 'sam@test', role: 'sales', agencyIds: [], agencies: [] },
    ];
    const wrapper = await setup();
    const ownerSelect = wrapper.findAllComponents({ name: 'BSelect' })[1];

    expect(ownerSelect.findAll('option').map((o) => o.text())).toEqual(['All owners', 'Rita Sales', 'sam@test']);
    ownerSelect.vm.$emit('update:modelValue', 'user-2');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ ownerId: 'user-2' });
  });

  it('hides the owner filter from non admins', async () => {
    const wrapper = await setup();
    expect(wrapper.findAll('option').map((o) => o.text())).not.toContain('All owners');
  });

  it('opens the modal to add and to edit, and reloads when saved', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');

    expect(modal(wrapper).props()).toEqual({ modelValue: true, deal: null, client: company });

    modal(wrapper).vm.$emit('update:modelValue', false);
    await nextTick();
    await rowButtons(wrapper)[0].trigger('click');
    expect(modal(wrapper).props()).toEqual({ modelValue: true, deal, client: company });

    modal(wrapper).vm.$emit('saved');
    await flushPromises();
    expect(getDeals).toHaveBeenCalledTimes(2);
  });

  it('deletes a deal once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteDeal).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Delete deal', 'This action cannot be undone.', 'Delete');
    expect(deleteDeal).toHaveBeenCalledExactlyOnceWith('company-1', 'deal-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Deal deleted');
    expect(getDeals).toHaveBeenCalledTimes(2);
  });

  it('keeps the deal when deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(deleteDeal).not.toHaveBeenCalled();
  });

  it('shows the error when deletion fails', async () => {
    const error = new Error('delete');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteDeal).mockRejectedValue(error);
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
