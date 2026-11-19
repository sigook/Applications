import { describe, expect, it, vi } from 'vitest';
import WorkLicenseForm from '@/shared/worker-profile/forms/WorkLicenseForm.vue';
import { createWorkerLicenses } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail, WorkerProfileLicenseDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerLicenses: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/app/stores/app', () => ({
  useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date(2026, 10, 19)) }),
}));

const licenseFile = new File(['license'], 'g-license.pdf', { type: 'application/pdf' });
const existingLicense: WorkerProfileLicenseDetail = {
  license: { fileName: 'License_old.pdf', description: 'G2' },
  number: 'G2-123',
  issued: '2020-01-01T05:00:00.000Z',
  expires: null,
};
const issuedDate = new Date(2024, 0, 10);
const expiresDate = new Date(2027, 5, 30);

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(WorkLicenseForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function datepicker(wrapper: ReturnType<typeof setup>, index: number) {
  const picker = wrapper.findAllComponents({ name: 'BDatepicker' }).at(index);
  if (!picker) throw new Error(`Datepicker ${index} not found`);
  return picker;
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

function sentFormData(): FormData {
  const formData = vi.mocked(createWorkerLicenses).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerLicenses was not called');
  return formData;
}

function errorMessages(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('WorkLicenseForm', () => {
  it('requires the file, the description and the expiry date by default', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['File is required', 'Description is required', 'Expires is required']); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerLicenses).not.toHaveBeenCalled();
  });

  it('hides the expiry date when the license does not expire', async () => {
    const wrapper = setup();
    expect(wrapper.findAllComponents({ name: 'BDatepicker' })).toHaveLength(2);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);

    await vi.waitFor(() => { expect(wrapper.findAllComponents({ name: 'BDatepicker' })).toHaveLength(1); });
    expect(wrapper.findComponent({ name: 'BSwitch' }).text()).toBe('No');
  });

  it('appends the new license to the existing ones and emits closeModal', async () => {
    vi.mocked(createWorkerLicenses).mockResolvedValue();
    const wrapper = setup({ licenses: [existingLicense] });
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', licenseFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').text()).toBe('LICENSE.PDF'); });
    await wrapper.find('input[name="license description"]').setValue('G license');
    const numberInput = wrapper.findAll('input').find((i) => i.attributes('name') === undefined && i.attributes('type') === 'text');
    if (!numberInput) throw new Error('Number input not found');
    await numberInput.setValue('G-998');
    datepicker(wrapper, 0).vm.$emit('update:modelValue', issuedDate);
    datepicker(wrapper, 1).vm.$emit('update:modelValue', expiresDate);
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(vi.mocked(createWorkerLicenses).mock.calls[0]?.[0]).toBe('worker-profile-1');
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual([
      existingLicense,
      {
        license: { fileName: expect.stringMatching(/^License_[0-9a-f]{32}\.pdf$/), description: 'G license' },
        number: 'G-998',
        issued: issuedDate.toISOString(),
        expires: expiresDate.toISOString(),
      },
    ]);
    const [, added] = payload as WorkerProfileLicenseDetail[];
    expect(formData.get(added.license.fileName ?? '')).toBeInstanceOf(File);
  });

  it('sends no expiry date when the license does not expire', async () => {
    vi.mocked(createWorkerLicenses).mockResolvedValue();
    const wrapper = setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', licenseFile);
    await wrapper.find('input[name="license description"]').setValue('Forklift');
    datepicker(wrapper, 1).vm.$emit('update:modelValue', expiresDate);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);
    await save(wrapper);

    await vi.waitFor(() => { expect(createWorkerLicenses).toHaveBeenCalledTimes(1); });
    expect(formDataPayload(sentFormData())).toEqual([
      {
        license: { fileName: expect.stringMatching(/^License_[0-9a-f]{32}\.pdf$/), description: 'Forklift' },
        number: '',
        issued: null,
        expires: null,
      },
    ]);
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit'); });
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('lets the selected file be removed', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', licenseFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').exists()).toBe(true); });
    await wrapper.find('.selected-file-display button').trigger('click');

    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerLicenses).mockRejectedValue(error);
    const wrapper = setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', licenseFile);
    await wrapper.find('input[name="license description"]').setValue('Forklift');
    datepicker(wrapper, 1).vm.$emit('update:modelValue', expiresDate);
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
