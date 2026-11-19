import { describe, expect, it, vi } from 'vitest';
import LocationForm from '@/shared/company-profile/LocationForm.vue';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { LocationDetailModel } from '@/shared/types/common';

const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/ui/Address.vue', () => ({
  default: {
    name: 'CvnAddress',
    props: { model: Object, enableProvinceSettings: Boolean },
    emits: ['update:model'],
    methods: { validateAddress: () => validateAddress() },
    template: '<div class="address-stub" />',
  },
}));

type Save = (location: LocationDetailModel, id?: string) => Promise<unknown>;

const existing: LocationDetailModel = {
  id: 'loc-1',
  address: '12 King St',
  postalCode: 'M5V 1A1',
  mainIntersection: 'King & Bay',
  entrance: 'Door 3',
  latitude: 43.6,
  longitude: -79.3,
  isBilling: false,
};

function setup(save: Save, currentLocation?: LocationDetailModel | null, enableProvinceSettings?: boolean) {
  return mountWithPlugins(LocationForm, { props: { save, currentLocation, enableProvinceSettings } });
}

function input(wrapper: ReturnType<typeof setup>, label: string) {
  const field = wrapper.findAll('.field').find((f) => f.find('label.label').exists() && f.find('label.label').text() === label);
  if (!field) throw new Error(`${label} not found`);
  return field.find('input');
}

async function submit(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button.is-primary').trigger('click');
}

describe('LocationForm', () => {
  it('starts empty for a new location', () => {
    const wrapper = setup(vi.fn<Save>(), null, true);

    expect(wrapper.find('h2').text()).toBe('New location');
    expect(wrapper.find('button.is-primary').text()).toBe('Create');
    expect(wrapper.findComponent({ name: 'CvnAddress' }).props()).toEqual({ model: {}, enableProvinceSettings: true });
  });

  it('fills the form with the location to edit', () => {
    const wrapper = setup(vi.fn<Save>(), existing);

    expect(wrapper.find('h2').text()).toBe('Edit location');
    expect(wrapper.find('button.is-primary').text()).toBe('Save');
    expect((input(wrapper, 'Main Intersection').element as HTMLInputElement).value).toBe('King & Bay');
    expect((input(wrapper, 'Entrance').element as HTMLInputElement).value).toBe('Door 3');
    expect((input(wrapper, 'Latitude').element as HTMLInputElement).value).toBe('43.6');
    expect(wrapper.findComponent({ name: 'CvnAddress' }).props('model')).toEqual(existing);
  });

  it('creates the location with numeric coordinates', async () => {
    validateAddress.mockResolvedValue(true);
    const save = vi.fn<Save>().mockResolvedValue(undefined);
    const wrapper = setup(save);
    await input(wrapper, 'Latitude').setValue('43.65');
    await input(wrapper, 'Longitude').setValue('-79.38');
    await input(wrapper, 'Main Intersection').setValue('Queen & Spadina');
    await input(wrapper, 'Entrance').setValue('Rear');
    await wrapper.find('input[type="checkbox"]').setValue(true);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toHaveLength(1); });
    expect(save).toHaveBeenCalledExactlyOnceWith({
      latitude: 43.65,
      longitude: -79.38,
      mainIntersection: 'Queen & Spadina',
      entrance: 'Rear',
      isBilling: true,
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Created');
  });

  it('sends empty coordinates as null', async () => {
    validateAddress.mockResolvedValue(true);
    const save = vi.fn<Save>().mockResolvedValue(undefined);
    const wrapper = setup(save, { ...existing, latitude: null, longitude: null });
    await submit(wrapper);

    await vi.waitFor(() => { expect(save).toHaveBeenCalled(); });
    expect(save.mock.calls[0][0]).toMatchObject({ latitude: null, longitude: null });
  });

  it('updates an existing location', async () => {
    validateAddress.mockResolvedValue(true);
    const save = vi.fn<Save>().mockResolvedValue(undefined);
    const wrapper = setup(save, existing);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toHaveLength(1); });
    expect(save).toHaveBeenCalledExactlyOnceWith({ ...existing }, 'loc-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('blocks saving when the address is invalid', async () => {
    validateAddress.mockResolvedValue(false);
    const save = vi.fn<Save>();
    const wrapper = setup(save);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(save).not.toHaveBeenCalled();
  });

  it('validates the entrance length', async () => {
    validateAddress.mockResolvedValue(true);
    const save = vi.fn<Save>();
    const wrapper = setup(save);
    await input(wrapper, 'Entrance').setValue('A');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Min 2 characters'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(save).not.toHaveBeenCalled();
  });

  it.each([
    ['creating', null],
    ['updating', existing],
  ])('shows the error when %s fails', async (_, currentLocation) => {
    validateAddress.mockResolvedValue(true);
    const error = new Error('save failed');
    const wrapper = setup(vi.fn<Save>().mockRejectedValue(error), currentLocation);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });
});
