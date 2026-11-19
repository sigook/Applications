import { describe, expect, it, vi } from 'vitest';
import WorkResumeForm from '@/shared/worker-profile/forms/WorkResumeForm.vue';
import { createWorkerResume } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerResume: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const resumeFile = new File(['resume'], 'my resume.pdf', { type: 'application/pdf' });

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(WorkResumeForm, { props: { data: createWorkerProfileDetail(overrides) } });
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
  const formData = vi.mocked(createWorkerResume).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerResume was not called');
  return formData;
}

describe('WorkResumeForm', () => {
  it('shows the current resume file', () => {
    const wrapper = setup({ resume: { fileName: 'Resume_abc.pdf', description: '' } });

    expect(wrapper.find('.selected-file-name').text()).toBe('RESUME.PDF');
    expect(wrapper.findComponent({ name: 'BUpload' }).exists()).toBe(false);
  });

  it('offers an upload when there is no resume', () => {
    expect(setup().find('.file-label').text()).toContain('Add file');
  });

  it('uploads the selected file with a generated name and emits closeModal', async () => {
    vi.mocked(createWorkerResume).mockResolvedValue();
    const wrapper = setup();
    selectFile(wrapper, resumeFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').exists()).toBe(true); });
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual({ fileName: expect.stringMatching(/^Resume_[0-9a-f]{32}\.pdf$/), description: '' });
    const { fileName } = payload as { fileName: string };
    expect(formData.get(fileName)).toBeInstanceOf(File);
    expect(vi.mocked(createWorkerResume).mock.calls[0]?.[0]).toBe('worker-profile-1');
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    selectFile(wrapper, oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit'); });
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('clears the current file and saves an empty resume', async () => {
    vi.mocked(createWorkerResume).mockResolvedValue();
    const wrapper = setup({ resume: { fileName: 'Resume_abc.pdf', description: '' } });
    await wrapper.find('.selected-file-display button').trigger('click');
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
    await save(wrapper);

    await vi.waitFor(() => { expect(createWorkerResume).toHaveBeenCalledTimes(1); });
    expect(formDataPayload(sentFormData())).toEqual({ fileName: '', description: '' });
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerResume).mockRejectedValue(error);
    const wrapper = setup({ resume: { fileName: 'Resume_abc.pdf', description: '' } });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
