import { describe, expect, it, vi } from 'vitest';
import WorkSinForm from '@/shared/worker-profile/forms/WorkSinForm.vue';
import { createWorkerSin } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerSin: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const sinFile = new File(['sin'], 'sin card.jpg', { type: 'image/jpeg' });
const dueDate = new Date(2028, 2, 1);
const requiredFieldsMessage = 'Please make sure all required fields are filled out correctly';

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(WorkSinForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function sinInput(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('input[name="social insurance #"]');
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

function sentFormData(): FormData {
  const formData = vi.mocked(createWorkerSin).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerSin was not called');
  return formData;
}

describe('WorkSinForm', () => {
  it('starts with the saved SIN, file and expiry date', async () => {
    const wrapper = setup({
      socialInsurance: '123456789',
      socialInsuranceExpire: true,
      dueDate: '2028-03-01T05:00:00.000Z',
      socialInsuranceFile: { fileName: 'SIN-SSN_abc.pdf', description: '' },
    });

    await vi.waitFor(() => { expect((sinInput(wrapper).element as HTMLInputElement).value).toBe('123456789'); });
    expect(wrapper.find('.selected-file-name').text()).toBe('SIN-SSN.PDF');
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual(new Date('2028-03-01T05:00:00.000Z'));
  });

  it('starts empty when the worker has no SIN', () => {
    const wrapper = setup();

    expect((sinInput(wrapper).element as HTMLInputElement).value).toBe('');
    expect(wrapper.findComponent({ name: 'BDatepicker' }).exists()).toBe(false);
  });

  it('requires the SIN', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('SIN/SSN is required'); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerSin).not.toHaveBeenCalled();
  });

  it('requires at least 9 characters', async () => {
    const wrapper = setup();
    await sinInput(wrapper).setValue('12345');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Min 9 characters'); });
    expect(createWorkerSin).not.toHaveBeenCalled();
  });

  it('requires the expiry date when the SIN expires', async () => {
    const wrapper = setup();
    await sinInput(wrapper).setValue('123456789');
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerSin).not.toHaveBeenCalled();
  });

  it('uploads the SIN with its file and expiry date and emits closeModal', async () => {
    vi.mocked(createWorkerSin).mockResolvedValue();
    const wrapper = setup();
    await sinInput(wrapper).setValue('123456789');
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', sinFile);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await vi.waitFor(() => { expect(wrapper.findComponent({ name: 'BDatepicker' }).exists()).toBe(true); });
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', dueDate);
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(vi.mocked(createWorkerSin).mock.calls[0]?.[0]).toBe('worker-profile-1');
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual({
      socialInsurance: '123456789',
      socialInsuranceExpire: true,
      dueDate: dueDate.toISOString(),
      socialInsuranceFile: { fileName: expect.stringMatching(/^SIN-SSN_[0-9a-f]{32}\.jpg$/), description: '' },
    });
    const { socialInsuranceFile } = payload as { socialInsuranceFile: { fileName: string } };
    expect(formData.get(socialInsuranceFile.fileName)).toBeInstanceOf(File);
  });

  it('drops the expiry date when the SIN stops expiring', async () => {
    vi.mocked(createWorkerSin).mockResolvedValue();
    const wrapper = setup({ socialInsurance: '123456789', socialInsuranceExpire: true, dueDate: '2028-03-01T05:00:00.000Z' });
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);
    await save(wrapper);

    await vi.waitFor(() => { expect(createWorkerSin).toHaveBeenCalledTimes(1); });
    expect(formDataPayload(sentFormData())).toEqual({
      socialInsurance: '123456789',
      socialInsuranceExpire: false,
      dueDate: null,
      socialInsuranceFile: { fileName: '', description: '' },
    });
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit'); });
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('lets the current file be removed', async () => {
    const wrapper = setup({ socialInsurance: '123456789', socialInsuranceFile: { fileName: 'SIN-SSN_abc.pdf', description: '' } });
    await wrapper.find('.selected-file-display button').trigger('click');

    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BUpload' }).exists()).toBe(true);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerSin).mockRejectedValue(error);
    const wrapper = setup({ socialInsurance: '123456789' });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
