import { describe, expect, it, vi } from 'vitest';
import WorkContactInformationForm from '@/shared/worker-profile/forms/WorkContactInformationForm.vue';
import LocationAddress from '@/shared/ui/Address.vue';
import { createWorkerContactInformation } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { LocationDetailModel } from '@/shared/types/common';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerContactInformation: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

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

const location: LocationDetailModel = {
  address: '100 King St W',
  postalCode: 'M5X 1A9',
  city: { id: 'city-1', value: 'Toronto', province: { id: 'province-on', value: 'Ontario' } },
};

function setup(mobileNumber: string | null = '4169791234', phone: string | null = null) {
  const data = createWorkerProfileDetail({ mobileNumber, phone, phoneExt: 12, location });
  return mountWithPlugins(WorkContactInformationForm, {
    props: { data },
  });
}

type Wrapper = ReturnType<typeof setup>;

function phoneInputs(wrapper: Wrapper) {
  return wrapper.findAll('input[type="tel"]');
}

function saveButton(wrapper: Wrapper) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('WorkContactInformationForm', () => {
  it('labels each phone input', () => {
    const wrapper = setup('4169791234', '9055551234');

    expect(wrapper.findAll('label.label').map((l) => l.text().replace('*', '').trim())).toEqual(expect.arrayContaining(['Mobile Number', 'Phone']));
  });

  it('passes the worker location and phones to the inputs', () => {
    const wrapper = setup('4169791234', '9055551234');

    expect(wrapper.findComponent(LocationAddress).props('model')).toEqual(location);
    expect(phoneInputs(wrapper).map((i) => (i.element as HTMLInputElement).value)).toEqual(['416 979-1234', '905 555-1234']);
  });

  it('does not save when the address is invalid', async () => {
    validateAddress.mockResolvedValue(false);
    const wrapper = setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerContactInformation).not.toHaveBeenCalled();
  });

  it('requires the mobile number', async () => {
    validateAddress.mockResolvedValue(true);
    const wrapper = setup(null);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Phone is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createWorkerContactInformation).not.toHaveBeenCalled();
  });

  it('rejects an invalid optional phone', async () => {
    validateAddress.mockResolvedValue(true);
    const wrapper = setup();
    await phoneInputs(wrapper)[1].setValue('123');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Invalid phone number'); });
    expect(createWorkerContactInformation).not.toHaveBeenCalled();
  });

  it('saves the edited contact information and closes the modal', async () => {
    validateAddress.mockResolvedValue(true);
    vi.mocked(createWorkerContactInformation).mockResolvedValue();
    const wrapper = setup();
    const newLocation: LocationDetailModel = { ...location, address: '200 Bay St' };
    wrapper.findComponent(LocationAddress).vm.$emit('update:model', newLocation);
    await phoneInputs(wrapper)[0].setValue('6475551234');
    await phoneInputs(wrapper)[1].setValue('9055551234');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerContactInformation).toHaveBeenCalledExactlyOnceWith('worker-profile-1', {
      mobileNumber: '647 555-1234',
      phone: '905 555-1234',
      phoneExt: 12,
      location: newLocation,
    });
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    validateAddress.mockResolvedValue(true);
    vi.mocked(createWorkerContactInformation).mockRejectedValue(error);
    const wrapper = setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
