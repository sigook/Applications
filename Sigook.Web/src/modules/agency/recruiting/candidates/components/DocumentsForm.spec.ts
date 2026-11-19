import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import DocumentsForm from '@/modules/agency/recruiting/candidates/components/DocumentsForm.vue';
import { addCandidateDocument } from '@/modules/agency/recruiting/candidates/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ addCandidateDocument: vi.fn() }));

const pdf = new File(['%PDF'], 'contract.pdf', { type: 'application/pdf' });
const exe = new File(['MZ'], 'virus.exe');

function setup() {
  return mountWithPlugins(DocumentsForm, { props: { candidateId: 'cand-1' } });
}

type Wrapper = ReturnType<typeof setup>;

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function pick(wrapper: Wrapper, file: File | null) {
  wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', file);
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

describe('DocumentsForm', () => {
  it('rejects a file with a forbidden extension', async () => {
    const wrapper = setup();
    await pick(wrapper, exe);

    expect(errors(wrapper)[0]).toContain('File extension not allowed');
    expect(wrapper.find('.file-name').exists()).toBe(false);
  });

  it('requires the file and the description', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['File is required', 'Description is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(addCandidateDocument).not.toHaveBeenCalled();
  });

  it('requires the file even with a description', async () => {
    const wrapper = setup();
    await wrapper.find('input[name="documentDescription"]').setValue('Contract');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(errors(wrapper)).toEqual(['File is required']);
    expect(addCandidateDocument).not.toHaveBeenCalled();
  });

  it('uploads the document and closes', async () => {
    vi.mocked(addCandidateDocument).mockResolvedValue('doc-1');
    const wrapper = setup();
    await pick(wrapper, pdf);
    expect(wrapper.find('.file-name').text()).toBe('contract.pdf');
    await wrapper.find('input[name="documentDescription"]').setValue('Contract');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('close')).toHaveLength(1); });
    expect(addCandidateDocument).toHaveBeenCalledExactlyOnceWith(
      'cand-1',
      { fileName: expect.stringMatching(/^Candidate_[0-9a-f]{32}\.pdf$/) as string, description: 'Contract' },
      pdf,
    );
    expect(showAlertSuccess).toHaveBeenCalledWith('Document added');
    expect(wrapper.emitted('created')).toHaveLength(1);
  });

  it('shows the error when the upload fails', async () => {
    const error = new Error('upload failed');
    vi.mocked(addCandidateDocument).mockRejectedValue(error);
    const wrapper = setup();
    await pick(wrapper, pdf);
    await wrapper.find('input[name="documentDescription"]').setValue('Contract');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('created')).toBeUndefined();
    expect(wrapper.emitted('close')).toBeUndefined();
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Cancel')?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
