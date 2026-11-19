import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import ProfileBilling from '@/modules/agency/profile/components/ProfileBilling.vue';
import { createAgencyLocation, deleteAgencyLocation, getAgencyLocations, updateAgencyLocation } from '@/modules/agency/profile/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';
import type { City } from '@/shared/types/common';

const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/composables/useBreakpoint', () => ({ useBreakpoint: () => ({ isTouch: computed(() => false) }) }));
vi.mock('@/modules/agency/profile/api', () => ({
  getAgencyLocations: vi.fn(),
  createAgencyLocation: vi.fn(),
  updateAgencyLocation: vi.fn(),
  deleteAgencyLocation: vi.fn(),
}));
vi.mock('@/shared/ui/Address.vue', async () => {
  const { defineComponent } = await import('vue');
  return {
    default: defineComponent({
      name: 'LocationAddress',
      props: { model: { type: Object, default: null } },
      emits: ['update:model', 'isLoading'],
      setup(_, { expose }) {
        expose({ validateAddress });
      },
      template: '<div class="address-stub" />',
    }),
  };
});

const agency: AgencyDetail = { id: 'agency-1', fullName: 'Covenant' };
const toronto: City = { id: 'to', value: 'Toronto' };

const office: AgencyLocationDetail = { id: 'loc-1', address: '100 King St', city: toronto, postalCode: 'M5H 1A1', formattedAddress: '100 King St Toronto ON M5H 1A1', isBilling: true };
const warehouse: AgencyLocationDetail = { id: 'loc-2', address: '5 Bay St', city: toronto, postalCode: 'M5J 2R8', formattedAddress: '5 Bay St Toronto ON M5J 2R8', isBilling: false };

async function setup() {
  const wrapper = mountWithPlugins(ProfileBilling, { props: { agencyData: agency } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((r) => r.findAll('td').slice(0, 2).map((c) => c.text()));
}

function rowButtons(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr')[index].findAll('button');
}

function address(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'LocationAddress' });
}

async function saveModal(wrapper: Wrapper) {
  const save = wrapper.findAll('button').find((b) => b.text() === 'SAVE');
  if (!save) throw new Error('SAVE not found');
  await save.trigger('click');
  await flushPromises();
}

function lastChanged(wrapper: Wrapper) {
  return wrapper.emitted('changed')?.at(-1)?.[0] as AgencyLocationDetail[];
}

beforeEach(() => {
  vi.mocked(getAgencyLocations).mockResolvedValue([{ ...office }, { ...warehouse }]);
});

describe('ProfileBilling', () => {
  it('lists the agency locations and reports them', async () => {
    const wrapper = await setup();

    expect(getAgencyLocations).toHaveBeenCalledWith('agency-1');
    expect(rows(wrapper)).toEqual([
      ['100 King St Toronto ON M5H 1A1', 'Yes'],
      ['5 Bay St Toronto ON M5J 2R8', 'No'],
    ]);
    expect(lastChanged(wrapper)).toHaveLength(2);
  });

  it('does not save a location with an invalid address', async () => {
    validateAddress.mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    await nextTick();
    await saveModal(wrapper);

    expect(createAgencyLocation).not.toHaveBeenCalled();
    expect(updateAgencyLocation).not.toHaveBeenCalled();
  });

  it('creates a billing location and adds it to the list', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(createAgencyLocation).mockResolvedValue({ id: 'loc-3' });
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    await nextTick();

    expect(address(wrapper).props('model')).toEqual({ address: '', city: null, postalCode: '', isBilling: false });

    address(wrapper).vm.$emit('update:model', { address: '9 Queen St', city: toronto, postalCode: 'M5C 1A1', isBilling: false });
    await nextTick();
    await wrapper.find('.modal input[type="checkbox"]').setValue(true);
    await saveModal(wrapper);

    expect(createAgencyLocation).toHaveBeenCalledExactlyOnceWith('agency-1', expect.objectContaining({ address: '9 Queen St', city: toronto, isBilling: true }));
    expect(lastChanged(wrapper)).toHaveLength(3);
    expect(lastChanged(wrapper)[2]).toMatchObject({ id: 'loc-3', formattedAddress: '9 Queen St Toronto  M5C 1A1', isBilling: true });
    expect(rows(wrapper)[2]).toEqual(['9 Queen St Toronto  M5C 1A1', 'Yes']);
  });

  it('writes the edited address back to the row', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(updateAgencyLocation).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper, 1)[0].trigger('click');
    await nextTick();
    address(wrapper).vm.$emit('update:model', { ...warehouse, address: '7 Bay St' });
    await nextTick();
    await saveModal(wrapper);

    expect(updateAgencyLocation).toHaveBeenCalledExactlyOnceWith('agency-1', 'loc-2', expect.objectContaining({ address: '7 Bay St' }));
    expect(rows(wrapper)[1]).toEqual(['7 Bay St Toronto  M5J 2R8', 'No']);
    expect(lastChanged(wrapper)[1]).toMatchObject({ id: 'loc-2', address: '7 Bay St', formattedAddress: '7 Bay St Toronto  M5J 2R8' });
  });

  it('leaves the row untouched when the edit modal is closed without saving', async () => {
    const wrapper = await setup();
    await rowButtons(wrapper, 1)[0].trigger('click');
    await nextTick();
    await wrapper.find('.modal input[type="checkbox"]').setValue(true);
    wrapper.findComponent({ name: 'BModal' }).vm.$emit('update:modelValue', false);
    await flushPromises();

    expect(updateAgencyLocation).not.toHaveBeenCalled();
    expect(rows(wrapper)[1]).toEqual(['5 Bay St Toronto ON M5J 2R8', 'No']);
  });

  it('shows the error and hides the loader when loading the locations fails', async () => {
    vi.mocked(getAgencyLocations).mockRejectedValue(new Error('load failed'));
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(new Error('load failed'));
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('updates an existing location', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(updateAgencyLocation).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper, 1)[0].trigger('click');
    await nextTick();

    expect(address(wrapper).props('model')).toEqual(warehouse);

    await wrapper.find('.modal input[type="checkbox"]').setValue(true);
    await saveModal(wrapper);

    expect(updateAgencyLocation).toHaveBeenCalledExactlyOnceWith('agency-1', 'loc-2', expect.objectContaining({ id: 'loc-2', isBilling: true }));
    expect(rows(wrapper)[1]).toEqual(['5 Bay St Toronto  M5J 2R8', 'Yes']);
    expect(lastChanged(wrapper)[1].isBilling).toBe(true);
  });

  it('shows the error data when saving fails', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(updateAgencyLocation).mockRejectedValue({ data: 'Invalid address' });
    vi.mocked(createAgencyLocation).mockRejectedValue({ data: 'Duplicated address' });
    const wrapper = await setup();
    await rowButtons(wrapper, 0)[0].trigger('click');
    await nextTick();
    await saveModal(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Invalid address');

    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    await nextTick();
    await saveModal(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Duplicated address');
    expect(rows(wrapper)).toHaveLength(2);
  });

  it('deletes a location after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyLocation).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper, 0)[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this location', 'Yes');
    expect(deleteAgencyLocation).toHaveBeenCalledExactlyOnceWith('agency-1', 'loc-1');
    expect(rows(wrapper)).toEqual([['5 Bay St Toronto ON M5J 2R8', 'No']]);
    expect(lastChanged(wrapper)).toHaveLength(1);
  });

  it('keeps the location when the delete is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await rowButtons(wrapper, 0)[1].trigger('click');
    await flushPromises();

    expect(deleteAgencyLocation).not.toHaveBeenCalled();
    expect(rows(wrapper)).toHaveLength(2);
  });

  it('shows the error data when deleting fails', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyLocation).mockRejectedValue({ data: 'In use' });
    const wrapper = await setup();
    await rowButtons(wrapper, 0)[1].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('In use');
    expect(rows(wrapper)).toHaveLength(2);
  });

  it('reloads the locations from the refresh button', async () => {
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    await flushPromises();

    expect(getAgencyLocations).toHaveBeenCalledTimes(2);
  });
});
