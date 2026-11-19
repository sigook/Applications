import { describe, expect, it, vi } from 'vitest';
import WorkExperienceForm from '@/shared/worker-profile/forms/WorkExperienceForm.vue';
import { createWorkerWorkExperience, editWorkerWorkExperience } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerProfileJobExperienceDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({
  createWorkerWorkExperience: vi.fn(),
  editWorkerWorkExperience: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/app/stores/app', () => ({
  useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date(2026, 10, 19)) }),
}));

const startDate = new Date(2022, 3, 1);
const endDate = new Date(2024, 7, 31);
const experience: WorkerProfileJobExperienceDetail = {
  id: 'experience-1',
  company: 'Acme Logistics',
  supervisor: 'Jane Doe',
  duties: 'Picking and packing',
  startDate: '2022-04-01T04:00:00.000Z',
  endDate: '2024-08-31T04:00:00.000Z',
  isCurrentJobPosition: false,
};

function setup(data?: WorkerProfileJobExperienceDetail) {
  return mountWithPlugins(WorkExperienceForm, { props: { workerId: 'worker-profile-1', data } });
}

function inputValue(wrapper: ReturnType<typeof setup>, selector: string) {
  return (wrapper.find(selector).element as HTMLInputElement | HTMLTextAreaElement).value;
}

function datepickers(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAllComponents({ name: 'BDatepicker' });
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

async function fillRequired(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('input[name="company"]').setValue('Acme Logistics');
  await wrapper.find('textarea[name="duties"]').setValue('Forklift operation');
  datepickers(wrapper)[0]?.vm.$emit('update:modelValue', startDate);
}

describe('WorkExperienceForm', () => {
  it('starts as a current job with no end date', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('Yes');
    expect(datepickers(wrapper)).toHaveLength(1);
  });

  it('starts with the experience being edited', async () => {
    const wrapper = setup(experience);

    await vi.waitFor(() => { expect(inputValue(wrapper, 'input[name="company"]')).toBe('Acme Logistics'); });
    expect(inputValue(wrapper, 'input[name="supervisor"]')).toBe('Jane Doe');
    expect(inputValue(wrapper, 'textarea[name="duties"]')).toBe('Picking and packing');
    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('No');
    expect(datepickers(wrapper).map((d) => d.props('modelValue'))).toEqual([
      new Date('2022-04-01T04:00:00.000Z'),
      new Date('2024-08-31T04:00:00.000Z'),
    ]);
  });

  it('requires company, duties and start date', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.findAll('.help.is-danger').map((e) => e.text())).toEqual([
        'Company is required',
        'Duties is required',
        'Start date is required',
      ]);
    });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerWorkExperience).not.toHaveBeenCalled();
  });

  it('requires a supervisor of at least 2 characters when given', async () => {
    const wrapper = setup();
    await fillRequired(wrapper);
    await wrapper.find('input[name="supervisor"]').setValue('J');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Min 2 characters'); });
    expect(createWorkerWorkExperience).not.toHaveBeenCalled();
  });

  it('requires the end date for a past job', async () => {
    const wrapper = setup();
    await fillRequired(wrapper);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);
    await vi.waitFor(() => { expect(datepickers(wrapper)).toHaveLength(2); });
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('End date is required'); });
    expect(createWorkerWorkExperience).not.toHaveBeenCalled();
  });

  it('creates a current job without end date and emits updateExperience', async () => {
    vi.mocked(createWorkerWorkExperience).mockResolvedValue();
    const wrapper = setup();
    await fillRequired(wrapper);
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateExperience')).toHaveLength(1); });
    expect(createWorkerWorkExperience).toHaveBeenCalledExactlyOnceWith('worker-profile-1', {
      isCurrentJobPosition: true,
      company: 'Acme Logistics',
      supervisor: '',
      duties: 'Forklift operation',
      startDate: startDate.toISOString(),
      endDate: null,
    });
  });

  it('updates the edited experience with its end date', async () => {
    vi.mocked(editWorkerWorkExperience).mockResolvedValue();
    const wrapper = setup(experience);
    await vi.waitFor(() => { expect(inputValue(wrapper, 'input[name="company"]')).toBe('Acme Logistics'); });
    datepickers(wrapper)[1]?.vm.$emit('update:modelValue', endDate);
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateExperience')).toHaveLength(1); });
    expect(editWorkerWorkExperience).toHaveBeenCalledExactlyOnceWith('worker-profile-1', 'experience-1', {
      isCurrentJobPosition: false,
      company: 'Acme Logistics',
      supervisor: 'Jane Doe',
      duties: 'Picking and packing',
      startDate: '2022-04-01T04:00:00.000Z',
      endDate: endDate.toISOString(),
    });
    expect(createWorkerWorkExperience).not.toHaveBeenCalled();
  });

  it('shows the error when creating fails', async () => {
    const error = new Error('create failed');
    vi.mocked(createWorkerWorkExperience).mockRejectedValue(error);
    const wrapper = setup();
    await fillRequired(wrapper);
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateExperience')).toBeUndefined();
  });

  it('shows the error when updating fails', async () => {
    const error = new Error('update failed');
    vi.mocked(editWorkerWorkExperience).mockRejectedValue(error);
    const wrapper = setup(experience);
    await vi.waitFor(() => { expect(inputValue(wrapper, 'input[name="company"]')).toBe('Acme Logistics'); });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateExperience')).toBeUndefined();
  });
});
