import { describe, expect, it, vi } from 'vitest';
import WorkBasicInformationForm from '@/shared/worker-profile/forms/WorkBasicInformationForm.vue';
import { createWorkerBasicInformation } from '@/shared/worker-profile/api';
import { getGenders } from '@/shared/api/catalogApi';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerBasicInformation: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getGenders: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/app/stores/app', () => ({
  useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date(2026, 10, 19)) }),
}));

const female = { id: 'gender-f', value: 'Female' };
const male = { id: 'gender-m', value: 'Male' };
const birthDay = new Date(1995, 6, 20);

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  vi.mocked(getGenders).mockResolvedValue([female, male]);
  return mountWithPlugins(WorkBasicInformationForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function inputValue(wrapper: ReturnType<typeof setup>, name: string) {
  return (wrapper.find(`input[name="${name}"]`).element as HTMLInputElement).value;
}

function genderSelect(wrapper: ReturnType<typeof setup>) {
  const select = wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find('select[name="gender"]').exists());
  if (!select) throw new Error('Gender select not found');
  return select;
}

function errorMessages(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function save(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('form').trigger('submit');
}

describe('WorkBasicInformationForm', () => {
  it('starts with the worker basic information', async () => {
    const wrapper = setup({
      firstName: 'John',
      middleName: 'Paul',
      lastName: 'Smith',
      secondLastName: 'Jones',
      birthDay: '1990-05-15T04:00:00.000Z',
      gender: male,
      hasVehicle: true,
    });

    await vi.waitFor(() => { expect(inputValue(wrapper, 'name')).toBe('John'); });
    expect(inputValue(wrapper, 'middle name')).toBe('Paul');
    expect(inputValue(wrapper, 'lastname')).toBe('Smith');
    expect(inputValue(wrapper, 'second lastname')).toBe('Jones');
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual(new Date('1990-05-15T04:00:00.000Z'));
    expect(genderSelect(wrapper).props('modelValue')).toEqual(male);
    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('Yes');
  });

  it('lists the genders from the catalog', async () => {
    const wrapper = setup();

    await vi.waitFor(() => { expect(wrapper.findAll('select[name="gender"] option').map((o) => o.text())).toEqual(['Select gender', 'Female', 'Male']); });
  });

  it('limits the date of birth to workers at least 18 years old', async () => {
    const wrapper = setup();

    await vi.waitFor(() => { expect(wrapper.findComponent({ name: 'BDatepicker' }).props('maxDate')).toEqual(new Date(2008, 10, 19)); });
  });

  it('requires name, last name, date of birth and gender', async () => {
    const wrapper = setup({ firstName: '', lastName: '', birthDay: '' });
    await save(wrapper);

    await vi.waitFor(() => {
      expect(errorMessages(wrapper)).toEqual([
        'Name is required',
        'Last Name is required',
        'Date of birth is required',
        'Gender is required',
      ]);
    });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerBasicInformation).not.toHaveBeenCalled();
  });

  it('requires an other family name of at least 2 characters when given', async () => {
    const wrapper = setup({ gender: female });
    await wrapper.find('input[name="second lastname"]').setValue('J');
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Min 2 characters']); });
    expect(createWorkerBasicInformation).not.toHaveBeenCalled();
  });

  it('saves the basic information and emits closeModal', async () => {
    vi.mocked(createWorkerBasicInformation).mockResolvedValue();
    const wrapper = setup({ firstName: '', lastName: '', birthDay: '' });
    await vi.waitFor(() => { expect(wrapper.findAll('select[name="gender"] option')).toHaveLength(3); });
    await wrapper.find('input[name="name"]').setValue('Anna');
    await wrapper.find('input[name="lastname"]').setValue('Lee');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', birthDay);
    genderSelect(wrapper).vm.$emit('update:modelValue', female);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerBasicInformation).toHaveBeenCalledExactlyOnceWith('worker-profile-1', {
      firstName: 'Anna',
      middleName: '',
      lastName: 'Lee',
      secondLastName: '',
      birthDay: birthDay.toISOString(),
      gender: female,
      hasVehicle: true,
    });
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerBasicInformation).mockRejectedValue(error);
    const wrapper = setup({ gender: female });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
