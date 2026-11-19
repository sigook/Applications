import { describe, expect, it, vi } from 'vitest';
import WorkerOtherDocumentsForm from '@/shared/worker-profile/forms/WorkerOtherDocumentsForm.vue';
import { createWorkerOtherDocuments } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerOtherDocuments: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const documentFile = new File(['doc'], 'reference letter.docx');

function setup() {
  return mountWithPlugins(WorkerOtherDocumentsForm, { props: { data: createWorkerProfileDetail() } });
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
  const formData = vi.mocked(createWorkerOtherDocuments).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerOtherDocuments was not called');
  return formData;
}

describe('WorkerOtherDocumentsForm', () => {
  it('requires a file and a description', async () => {
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.findAll('.help.is-danger').map((e) => e.text())).toEqual(['File is required', 'Description is required']);
    });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerOtherDocuments).not.toHaveBeenCalled();
  });

  it('requires a file even when the description is valid', async () => {
    const wrapper = setup();
    await wrapper.find('input[name="Description"]').setValue('Forklift');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('File is required'); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(createWorkerOtherDocuments).not.toHaveBeenCalled();
  });

  it('clears the file error once a file is selected', async () => {
    const wrapper = setup();
    await save(wrapper);
    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('File is required'); });
    selectFile(wrapper, documentFile);

    await vi.waitFor(() => { expect(wrapper.findAll('.help.is-danger').map((e) => e.text())).not.toContain('File is required'); });
  });

  it('limits the description to 20 characters', async () => {
    const wrapper = setup();
    selectFile(wrapper, documentFile);
    await wrapper.find('input[name="Description"]').setValue('A very long document name');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Max 20 characters'); });
    expect(createWorkerOtherDocuments).not.toHaveBeenCalled();
  });

  it('uploads the document with its description and emits closeAndUpdate', async () => {
    vi.mocked(createWorkerOtherDocuments).mockResolvedValue();
    const wrapper = setup();
    selectFile(wrapper, documentFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').text()).toBe('OTHERDOC.DOCX'); });
    await wrapper.find('input[name="Description"]').setValue('Reference');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeAndUpdate')).toEqual([[true]]); });
    expect(vi.mocked(createWorkerOtherDocuments).mock.calls[0]?.[0]).toBe('worker-profile-1');
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual({ fileName: expect.stringMatching(/^OtherDoc_[0-9a-f]{32}\.docx$/), description: 'Reference' });
    const { fileName } = payload as { fileName: string };
    expect(formData.get(fileName)).toBeInstanceOf(File);
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    selectFile(wrapper, oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit'); });
    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
  });

  it('lets the selected file be removed', async () => {
    const wrapper = setup();
    selectFile(wrapper, documentFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').exists()).toBe(true); });
    await wrapper.find('.selected-file-display button').trigger('click');

    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BUpload' }).exists()).toBe(true);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerOtherDocuments).mockRejectedValue(error);
    const wrapper = setup();
    selectFile(wrapper, documentFile);
    await wrapper.find('input[name="Description"]').setValue('Reference');
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeAndUpdate')).toBeUndefined();
  });
});
