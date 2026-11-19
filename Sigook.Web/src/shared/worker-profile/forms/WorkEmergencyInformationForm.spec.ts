import { describe, expect, it, vi } from 'vitest';
import WorkEmergencyInformationForm from '@/shared/worker-profile/forms/WorkEmergencyInformationForm.vue';
import { createWorkerEmergencyInformation } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerEmergencyInformation: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const requiredFieldsMessage = 'Please make sure all required fields are filled out correctly';
const filledContact: Partial<WorkerProfileDetail> = {
  contactEmergencyName: 'Mary',
  contactEmergencyLastName: 'Smith',
  contactEmergencyPhone: '4165551234',
};

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(WorkEmergencyInformationForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function inputValue(wrapper: ReturnType<typeof setup>, selector: string) {
  return (wrapper.find(selector).element as HTMLInputElement).value;
}

function errorMessages(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

describe('WorkEmergencyInformationForm', () => {
  it('starts with the saved emergency contact and health information', async () => {
    const wrapper = setup({ ...filledContact, haveAnyHealthProblem: true, healthProblem: 'Peanuts', otherHealthProblem: 'Dust' });

    await vi.waitFor(() => { expect(inputValue(wrapper, 'input[name="contact emergency"]')).toBe('Mary'); });
    expect(inputValue(wrapper, 'input[name="contact emergency lastname"]')).toBe('Smith');
    expect(inputValue(wrapper, 'input[name="health problem"]')).toBe('Peanuts');
    expect(inputValue(wrapper, 'input[type="tel"]')).toBe('416 555-1234');
    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('Yes');
  });

  it('hides the health problem fields when there are none', () => {
    const wrapper = setup();

    expect(wrapper.find('input[name="health problem"]').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('No');
  });

  it('requires the contact name, last name and phone', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => {
      expect(errorMessages(wrapper)).toEqual(['Name is required', 'Last Name is required', 'Phone is required']);
    });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerEmergencyInformation).not.toHaveBeenCalled();
  });

  it('rejects an invalid phone number', async () => {
    const wrapper = setup({ ...filledContact, contactEmergencyPhone: '1234567890' });
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Invalid phone number']); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerEmergencyInformation).not.toHaveBeenCalled();
  });

  it('requires the health problem when the worker has one', async () => {
    const wrapper = setup(filledContact);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await vi.waitFor(() => { expect(wrapper.find('input[name="health problem"]').exists()).toBe(true); });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerEmergencyInformation).not.toHaveBeenCalled();
  });

  it('saves the emergency information and emits closeModal', async () => {
    vi.mocked(createWorkerEmergencyInformation).mockResolvedValue();
    const wrapper = setup();
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await vi.waitFor(() => { expect(wrapper.find('input[name="health problem"]').exists()).toBe(true); });
    await wrapper.find('input[name="health problem"]').setValue('Peanuts');
    const otherAllergies = wrapper.findAll('input[type="text"]').find((i) => i.attributes('name') === undefined);
    if (!otherAllergies) throw new Error('Other allergies input not found');
    await otherAllergies.setValue('Dust');
    await wrapper.find('input[name="contact emergency"]').setValue('Mary');
    await wrapper.find('input[name="contact emergency lastname"]').setValue('Smith');
    await wrapper.find('input[type="tel"]').setValue('4165551234');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerEmergencyInformation).toHaveBeenCalledExactlyOnceWith('worker-profile-1', {
      haveAnyHealthProblem: true,
      healthProblem: 'Peanuts',
      otherHealthProblem: 'Dust',
      contactEmergencyName: 'Mary',
      contactEmergencyLastName: 'Smith',
      contactEmergencyPhone: '416 555-1234',
    });
  });

  it('drops the health problem when the worker no longer has one', async () => {
    vi.mocked(createWorkerEmergencyInformation).mockResolvedValue();
    const wrapper = setup({ ...filledContact, haveAnyHealthProblem: true, healthProblem: 'Peanuts' });
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);
    await save(wrapper);

    await vi.waitFor(() => { expect(createWorkerEmergencyInformation).toHaveBeenCalledTimes(1); });
    expect(vi.mocked(createWorkerEmergencyInformation).mock.calls[0]?.[1]).toMatchObject({
      haveAnyHealthProblem: false,
      healthProblem: null,
    });
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerEmergencyInformation).mockRejectedValue(error);
    const wrapper = setup(filledContact);
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
