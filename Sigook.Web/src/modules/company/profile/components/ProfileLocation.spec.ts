import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ProfileLocation from '@/modules/company/profile/components/ProfileLocation.vue';
import { createProfileLocation, deleteProfileLocation, getProfileLocations, updateProfileLocation } from '@/modules/company/profile/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyProfileLocationDetail } from '@/shared/company-profile/types';

const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/modules/company/profile/api', () => ({
  getProfileLocations: vi.fn(),
  createProfileLocation: vi.fn(),
  updateProfileLocation: vi.fn(),
  deleteProfileLocation: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/ui/Address.vue', () => ({
  default: {
    name: 'CvnAddress',
    props: { model: Object, enableProvinceSettings: Boolean },
    emits: ['update:model', 'isLoading'],
    methods: { validateAddress: () => validateAddress() },
    template: '<div class="address-stub" />',
  },
}));

const billing: CompanyProfileLocationDetail = {
  id: 'loc-1', address: '1 King St', postalCode: 'M5H 1A1', isBilling: true, latitude: 43.6, longitude: -79.3,
  entrance: '', mainIntersection: '', city: { id: 'c1', value: 'Toronto', province: { id: 'p1', code: 'ON', value: 'Ontario' } },
};
const plant: CompanyProfileLocationDetail = {
  id: 'loc-2', address: '9 Queen St', postalCode: 'L5B 2C3', isBilling: false, latitude: null, longitude: null,
  formattedAddress: '9 Queen St, Mississauga', entrance: '', mainIntersection: '',
};

async function setup(locations: CompanyProfileLocationDetail[] = [billing, plant]) {
  vi.mocked(getProfileLocations).mockResolvedValue(locations);
  const wrapper = mountWithPlugins(ProfileLocation);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 2).map((cell) => cell.text()));
}

function address(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'CvnAddress' });
}

async function save(wrapper: Wrapper) {
  await wrapper.find('.modal button.is-primary').trigger('click');
  await flushPromises();
}

describe('ProfileLocation', () => {
  it('lists the locations and reports how many there are', async () => {
    const wrapper = await setup();

    expect(rows(wrapper)).toEqual([
      ['1 King St, Toronto, ON, M5H 1A1', 'Yes'],
      ['9 Queen St, Mississauga', 'No'],
    ]);
    expect(wrapper.emitted('loaded')).toEqual([[2]]);
  });

  it('shows the error when the locations cannot load', async () => {
    vi.mocked(getProfileLocations).mockRejectedValue({ data: 'load failed' });
    mountWithPlugins(ProfileLocation);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('load failed');
  });

  it('creates a new location once the address is valid', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(createProfileLocation).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');

    expect(wrapper.find('.modal h2').text()).toBe('New location');
    address(wrapper).vm.$emit('update:model', { ...plant, id: undefined, address: '5 Bay St', isBilling: false });
    await wrapper.find('.modal input[type="checkbox"]').setValue(true);
    await save(wrapper);

    expect(createProfileLocation).toHaveBeenCalledExactlyOnceWith(expect.objectContaining({ address: '5 Bay St', isBilling: true }));
    expect(updateProfileLocation).not.toHaveBeenCalled();
    expect(getProfileLocations).toHaveBeenCalledTimes(2);
  });

  it('does not save an invalid address', async () => {
    validateAddress.mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    await save(wrapper);

    expect(createProfileLocation).not.toHaveBeenCalled();
  });

  it('updates an edited location', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(updateProfileLocation).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Edit location"]').trigger('click');

    expect(wrapper.find('.modal h2').text()).toBe('Edit location');
    expect(address(wrapper).props('model')).toEqual(billing);
    await save(wrapper);

    expect(updateProfileLocation).toHaveBeenCalledExactlyOnceWith('loc-1', billing);
    expect(createProfileLocation).not.toHaveBeenCalled();
    expect(getProfileLocations).toHaveBeenCalledTimes(2);
  });

  it('does not change the grid row while editing or after closing without saving', async () => {
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Edit location"]').trigger('click');
    await wrapper.find('.modal input[type="checkbox"]').setValue(false);

    expect(rows(wrapper)[0][1]).toBe('Yes');

    await wrapper.findComponent({ name: 'BModal' }).vm.$emit('update:modelValue', false);
    await flushPromises();

    expect(rows(wrapper)[0][1]).toBe('Yes');
    expect(billing.isBilling).toBe(true);
  });

  it.each([
    ['create', createProfileLocation, 0],
    ['update', updateProfileLocation, 1],
  ] as const)('shows the error when the %s fails', async (_, request, editIndex) => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(request).mockRejectedValue({ data: 'save failed' });
    const wrapper = await setup();
    if (editIndex) await wrapper.find('button[aria-label="Edit location"]').trigger('click');
    else await wrapper.find('.sigook-grid-actions button').trigger('click');
    await save(wrapper);

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('save failed');
    expect(getProfileLocations).toHaveBeenCalledTimes(1);
  });

  it('deletes a location once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteProfileLocation).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('button[aria-label="Delete location"]')[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this location', 'Yes');
    expect(deleteProfileLocation).toHaveBeenCalledExactlyOnceWith('loc-2');
    expect(getProfileLocations).toHaveBeenCalledTimes(2);
  });

  it('keeps the location when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete location"]').trigger('click');
    await flushPromises();

    expect(deleteProfileLocation).not.toHaveBeenCalled();
  });

  it('shows the error when the deletion fails', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteProfileLocation).mockRejectedValue({ data: 'delete failed' });
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete location"]').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('delete failed');
  });
});
