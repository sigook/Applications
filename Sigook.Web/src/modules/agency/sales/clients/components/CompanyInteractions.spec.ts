import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { computed, nextTick, ref } from 'vue';
import CompanyInteractions from '@/modules/agency/sales/clients/components/CompanyInteractions.vue';
import { deleteCompanyInteraction, getCompanyInteractions } from '@/modules/agency/sales/clients/api';
import { CompanyInteractionSortBy, InteractionPurpose, InteractionStatus, InteractionType } from '@/modules/agency/sales/clients/types';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyInteraction, CompanyInteractionFilter } from '@/modules/agency/sales/clients/types';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const admin = ref(false);
const owners = ref<AgencyPersonnelListItem[]>([]);
const loadOwners = vi.fn();

vi.mock('@/modules/agency/shared/useSalesOwners', () => ({
  useSalesOwners: () => ({ isAdmin: computed(() => admin.value), owners, loadOwners }),
}));
vi.mock('@/modules/agency/sales/clients/api', () => ({ deleteCompanyInteraction: vi.fn(), getCompanyInteractions: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/sales/clients/components/InteractionModal.vue', () => ({
  default: { name: 'InteractionModal', props: { modelValue: Boolean, interaction: Object, client: Object }, emits: ['saved', 'update:modelValue'], template: '<div class="interaction-modal-stub" />' },
}));

const interaction: CompanyInteraction = {
  id: 'int-1',
  companyProfileId: 'company-1',
  companyName: 'Acme',
  ownerId: 'user-1',
  owner: 'Rita Sales',
  description: 'Called the plant manager',
  interactionPurpose: InteractionPurpose.FollowUp,
  interactionType: InteractionType.Sms,
  interactionStatus: InteractionStatus.InProgress,
  createdAt: '2026-11-12T12:00:00',
  updatedAt: '2026-11-12T12:00:00',
};

const company = { id: 'company-1', fullName: 'Acme' };

async function setup() {
  vi.mocked(getCompanyInteractions).mockResolvedValue({ items: [interaction], totalItems: 1, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(CompanyInteractions, { props: { company } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function lastFilter(): CompanyInteractionFilter {
  const call = vi.mocked(getCompanyInteractions).mock.calls.at(-1);
  if (!call) throw new Error('not called');
  return call[1];
}

function modal(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'InteractionModal' });
}

function rowButtons(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr td:last-child button');
}

beforeEach(() => {
  admin.value = false;
  owners.value = [];
});

describe('CompanyInteractions', () => {
  it('loads the company interactions newest first', async () => {
    const wrapper = await setup();

    expect(loadOwners).toHaveBeenCalledOnce();
    expect(getCompanyInteractions).toHaveBeenCalledWith('company-1', expect.objectContaining({ sortBy: CompanyInteractionSortBy.CreatedAt, isDescending: true, pageIndex: 1 }));
    expect(wrapper.findAll('tbody tr td').map((c) => c.text()).slice(0, 6)).toEqual([
      'Called the plant manager', 'SMS', 'Follow-up', 'In progress', 'Rita Sales', 'Thu 12 November, 2026',
    ]);
  });

  it('sorts by status', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTable' }).vm.$emit('sort', 'status', 'asc');
    await flushPromises();

    expect(lastFilter()).toMatchObject({ sortBy: CompanyInteractionSortBy.Status, isDescending: false });
  });

  it('filters by type, purpose, statuses and created range', async () => {
    const wrapper = await setup();
    const [typeSelect, purposeSelect] = wrapper.findAllComponents({ name: 'BSelect' });
    typeSelect.vm.$emit('update:modelValue', InteractionType.Mail);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ interactionType: InteractionType.Mail });

    purposeSelect.vm.$emit('update:modelValue', InteractionPurpose.Closing);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ interactionPurpose: InteractionPurpose.Closing });

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: InteractionStatus.Completed, value: 'Completed' }]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ statuses: [InteractionStatus.Completed] });

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', []);
    await flushPromises();
    expect(lastFilter().statuses).toBeUndefined();

    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 30);
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() });

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ createdAtFrom: null, createdAtTo: null });
  });

  it('lets admins filter by owner', async () => {
    admin.value = true;
    owners.value = [{ id: 'p-1', userId: 'user-1', name: 'Rita Sales', email: 'rita@test', role: 'sales', agencyIds: [], agencies: [] }];
    const wrapper = await setup();
    const ownerSelect = wrapper.findAllComponents({ name: 'BSelect' })[2];

    expect(ownerSelect.findAll('option').map((o) => o.text())).toEqual(['All owners', 'Rita Sales']);
    ownerSelect.vm.$emit('update:modelValue', 'user-1');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ ownerId: 'user-1' });
  });

  it('opens the modal to log and to edit, and reloads when saved', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');

    expect(modal(wrapper).props()).toEqual({ modelValue: true, interaction: null, client: company });

    modal(wrapper).vm.$emit('update:modelValue', false);
    await nextTick();
    await rowButtons(wrapper)[0].trigger('click');
    expect(modal(wrapper).props()).toEqual({ modelValue: true, interaction, client: company });

    modal(wrapper).vm.$emit('saved');
    await flushPromises();
    expect(getCompanyInteractions).toHaveBeenCalledTimes(2);
  });

  it('deletes an interaction once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyInteraction).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Delete interaction', 'This action cannot be undone.', 'Delete');
    expect(deleteCompanyInteraction).toHaveBeenCalledExactlyOnceWith('company-1', 'int-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Interaction deleted');
    expect(getCompanyInteractions).toHaveBeenCalledTimes(2);
  });

  it('keeps the interaction when deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(deleteCompanyInteraction).not.toHaveBeenCalled();
  });

  it('shows the error when deletion fails', async () => {
    const error = new Error('delete');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyInteraction).mockRejectedValue(error);
    const wrapper = await setup();
    await rowButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
