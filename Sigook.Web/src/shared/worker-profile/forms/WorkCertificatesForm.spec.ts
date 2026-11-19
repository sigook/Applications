import { describe, expect, it, vi } from 'vitest';
import WorkCertificatesForm from '@/shared/worker-profile/forms/WorkCertificatesForm.vue';
import { createWorkerCertificates } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerCertificates: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const certificateFile = new File(['cert'], 'forklift.png', { type: 'image/png' });
const existingCertificate = { fileName: 'Certificate_old.pdf', description: 'WHMIS' };

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(WorkCertificatesForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function selectFile(wrapper: ReturnType<typeof setup>, file: File) {
  wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', file);
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

function sentFormData(): FormData {
  const formData = vi.mocked(createWorkerCertificates).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerCertificates was not called');
  return formData;
}

describe('WorkCertificatesForm', () => {
  it('requires a file and a description', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.findAll('.help.is-danger').map((e) => e.text())).toEqual(['File is required', 'Description is required']);
    });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerCertificates).not.toHaveBeenCalled();
  });

  it('requires a file even when the description is valid', async () => {
    const wrapper = setup();
    await wrapper.find('input[name="certificate description"]').setValue('Forklift');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('File is required'); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerCertificates).not.toHaveBeenCalled();
  });

  it('clears the file error once a file is selected', async () => {
    const wrapper = setup();
    await save(wrapper);
    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('File is required'); });
    selectFile(wrapper, certificateFile);

    await vi.waitFor(() => { expect(wrapper.findAll('.help.is-danger').map((e) => e.text())).not.toContain('File is required'); });
  });

  it('limits the description to 20 characters', async () => {
    const wrapper = setup();
    selectFile(wrapper, certificateFile);
    await wrapper.find('input[name="certificate description"]').setValue('A very long certificate name');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Max 20 characters'); });
    expect(createWorkerCertificates).not.toHaveBeenCalled();
  });

  it('appends the new certificate to the existing ones and emits closeModal', async () => {
    vi.mocked(createWorkerCertificates).mockResolvedValue();
    const wrapper = setup({ certificates: [existingCertificate] });
    selectFile(wrapper, certificateFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').text()).toBe('CERTIFICATE.PNG'); });
    await wrapper.find('input[name="certificate description"]').setValue('Forklift');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(vi.mocked(createWorkerCertificates).mock.calls[0]?.[0]).toBe('worker-profile-1');
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual([
      existingCertificate,
      { fileName: expect.stringMatching(/^Certificate_[0-9a-f]{32}\.png$/), description: 'Forklift' },
    ]);
    const [, added] = payload as { fileName: string }[];
    expect(formData.get(added.fileName)).toBeInstanceOf(File);
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    selectFile(wrapper, oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit'); });
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('lets the selected file be removed', async () => {
    const wrapper = setup();
    selectFile(wrapper, certificateFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').exists()).toBe(true); });
    await wrapper.find('.selected-file-display button').trigger('click');

    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BUpload' }).exists()).toBe(true);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerCertificates).mockRejectedValue(error);
    const wrapper = setup();
    selectFile(wrapper, certificateFile);
    await wrapper.find('input[name="certificate description"]').setValue('Forklift');
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
