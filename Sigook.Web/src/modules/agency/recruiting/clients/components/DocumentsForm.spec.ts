import { describe, expect, it, vi } from 'vitest';
import { toRaw } from 'vue';
import DocumentsForm from '@/modules/agency/recruiting/clients/components/DocumentsForm.vue';
import { createAgencyCompanyDocument } from '@/modules/agency/recruiting/clients/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ createAgencyCompanyDocument: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const contract = new File(['pdf'], 'contract.pdf', { type: 'application/pdf' });

function setup() {
  return mountWithPlugins(DocumentsForm, { props: { profileId: 'company-1' } });
}

async function selectFile(wrapper: ReturnType<typeof setup>, file: File | null) {
  wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', file);
  await wrapper.vm.$nextTick();
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('DocumentsForm', () => {
  it('requires a file and a description', async () => {
    const wrapper = setup();
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['File is required', 'Description is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyCompanyDocument).not.toHaveBeenCalled();
  });

  it('rejects a file with a forbidden extension and clears it', async () => {
    const wrapper = setup();
    await selectFile(wrapper, new File(['x'], 'virus.exe'));

    expect(errors(wrapper)[0]).toContain('File extension not allowed');
    expect(wrapper.find('.file-name').exists()).toBe(false);
  });

  it('uploads the document with a generated name, description and type', async () => {
    vi.mocked(createAgencyCompanyDocument).mockResolvedValue('doc-1');
    const wrapper = setup();
    await selectFile(wrapper, contract);
    expect(wrapper.find('.file-name').text()).toBe('contract.pdf');
    await wrapper.find('input[name="Description"]').setValue('Signed contract');
    await wrapper.find('select').setValue('1');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('onCreateDocument')).toHaveLength(1); });
    expect(showAlertSuccess).toHaveBeenCalledWith('Created');
    const [profileId, model, file] = vi.mocked(createAgencyCompanyDocument).mock.calls[0];
    expect(profileId).toBe('company-1');
    expect(model).toEqual({ fileName: expect.stringMatching(/^Company_[0-9a-f]{32}\.pdf$/) as string, description: 'Signed contract', documentType: '1' });
    expect(toRaw(file)).toBe(contract);
  });

  it('leaves the document type out when none is selected', async () => {
    vi.mocked(createAgencyCompanyDocument).mockResolvedValue('doc-1');
    const wrapper = setup();
    await selectFile(wrapper, contract);
    await wrapper.find('input[name="Description"]').setValue('Signed contract');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(createAgencyCompanyDocument).toHaveBeenCalled(); });
    expect(vi.mocked(createAgencyCompanyDocument).mock.calls[0][1].documentType).toBeUndefined();
  });

  it('shows the error when the upload fails', async () => {
    const error = new Error('upload failed');
    vi.mocked(createAgencyCompanyDocument).mockRejectedValue(error);
    const wrapper = setup();
    await selectFile(wrapper, contract);
    await wrapper.find('input[name="Description"]').setValue('Signed contract');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('onCreateDocument')).toBeUndefined();
  });
});
