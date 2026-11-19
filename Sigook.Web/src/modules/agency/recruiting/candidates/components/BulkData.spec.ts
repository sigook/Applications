import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import BulkData from '@/modules/agency/recruiting/candidates/components/BulkData.vue';
import { useAgencyStore } from '@/modules/agency/store';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));

type Upload = (agencyId: string, file: File) => Promise<Blob>;

let pinia: Pinia;

const csv = new File(['name,email'], 'candidates.csv', { type: 'text/csv' });

function setup(uploadFn: Upload = vi.fn<Upload>()) {
  return mountWithPlugins(BulkData, {
    props: { uploadFn, errorFileName: 'BulkCandidatesError', title: 'Bulk Candidates', fileLabel: 'Candidates File' },
    global: { plugins: [pinia] },
  });
}

type Wrapper = ReturnType<typeof setup>;

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function choose(wrapper: Wrapper) {
  await wrapper.find('select').setValue('a2');
  wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', csv);
  await nextTick();
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }, { id: 'a2', name: 'Sigook' }]);
});

describe('BulkData', () => {
  it('asks for the agency and the file before uploading', async () => {
    const wrapper = setup();

    expect(wrapper.find('h2').text()).toBe('Bulk Candidates');
    expect(wrapper.text()).toContain('Candidates File');
    expect(wrapper.findAll('select option').map((o) => o.text())).toEqual(['Covenant', 'Sigook']);
    expect(wrapper.text()).toContain('Click to upload');
    expect(button(wrapper, 'Upload').attributes('disabled')).toBeDefined();

    await choose(wrapper);

    expect(wrapper.text()).toContain('candidates.csv');
    expect(button(wrapper, 'Upload').attributes('disabled')).toBeUndefined();
  });

  it('closes when every record is uploaded', async () => {
    const upload = vi.fn<Upload>().mockResolvedValue(new Blob([]));
    const wrapper = setup(upload);
    await choose(wrapper);
    await button(wrapper, 'Upload').trigger('click');
    await flushPromises();

    expect(upload).toHaveBeenCalledExactlyOnceWith('a2', csv);
    expect(wrapper.emitted('close')).toHaveLength(1);
    expect(showAlertError).not.toHaveBeenCalled();
  });

  it('offers the error file when some records fail and lets the user try again', async () => {
    const errorFile = new Blob(['row 2: invalid email']);
    const wrapper = setup(vi.fn<Upload>().mockResolvedValue(errorFile));
    await choose(wrapper);
    await button(wrapper, 'Upload').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('Some records could not be uploaded, please check the file');
    expect(wrapper.emitted('close')).toBeUndefined();

    await button(wrapper, 'See Details').trigger('click');
    expect(downloadFile).toHaveBeenCalledExactlyOnceWith(errorFile, 'BulkCandidatesError');

    await button(wrapper, 'Try again').trigger('click');
    expect(wrapper.find('select').exists()).toBe(true);
    expect(button(wrapper, 'Upload').attributes('disabled')).toBeDefined();
  });

  it('shows the error when the upload fails', async () => {
    const error = new Error('upload failed');
    const wrapper = setup(vi.fn<Upload>().mockRejectedValue(error));
    await choose(wrapper);
    await button(wrapper, 'Upload').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('close')).toBeUndefined();
    expect(wrapper.text()).toContain('candidates.csv');
  });
});
