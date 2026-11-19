import { describe, expect, it, vi } from 'vitest';
import WorkDocumentsForm from '@/shared/worker-profile/forms/WorkDocumentsForm.vue';
import { createWorkerDocuments } from '@/shared/worker-profile/api';
import { getIdentificationTypes } from '@/shared/api/catalogApi';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail, formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import { IdentificationTypeCode, type IdentificationType } from '@/shared/types/common';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ createWorkerDocuments: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getIdentificationTypes: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const sin: IdentificationType = { id: 'type-sin', value: 'SIN / SSN', code: IdentificationTypeCode.SinSsn };
const passport: IdentificationType = { id: 'type-passport', value: 'Passport', code: 1 };
const requiredFieldsMessage = 'Please make sure all required fields are filled out correctly';
const passportFile = new File(['passport'], 'passport scan.pdf', { type: 'application/pdf' });

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  vi.mocked(getIdentificationTypes).mockResolvedValue([sin, passport]);
  return mountWithPlugins(WorkDocumentsForm, { props: { data: createWorkerProfileDetail(overrides) } });
}

function typeSelect(wrapper: ReturnType<typeof setup>, name: string) {
  const select = wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find(`select[name="${name}"]`).exists());
  if (!select) throw new Error(`${name} select not found`);
  return select;
}

function uploads(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAllComponents({ name: 'BUpload' });
}

function inputValue(wrapper: ReturnType<typeof setup>, name: string) {
  return (wrapper.find(`input[name="${name}"]`).element as HTMLInputElement).value;
}

function errorMessages(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function catalogLoaded(wrapper: ReturnType<typeof setup>) {
  await vi.waitFor(() => { expect(wrapper.findAll('select[name="identificationType1"] option')).toHaveLength(3); });
}

async function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  await button.trigger('click');
}

function sentFormData(): FormData {
  const formData = vi.mocked(createWorkerDocuments).mock.lastCall?.[1];
  if (!formData) throw new Error('createWorkerDocuments was not called');
  return formData;
}

describe('WorkDocumentsForm', () => {
  it('starts with the saved identifications matched to the catalog', async () => {
    const wrapper = setup({
      identificationType1: { id: 'type-sin', value: 'SIN / SSN' },
      identificationNumber1: '123456789',
      identificationType1File: { fileName: 'Document_abc.pdf', description: '' },
      identificationType2: { id: 'type-passport', value: 'Passport' },
      identificationNumber2: 'P1234',
    });

    await vi.waitFor(() => { expect(typeSelect(wrapper, 'identificationType1').props('modelValue')).toEqual(sin); });
    expect(typeSelect(wrapper, 'identificationType2').props('modelValue')).toEqual(passport);
    expect(inputValue(wrapper, 'identificationNumber1')).toBe('123456789');
    expect(inputValue(wrapper, 'identificationNumber2')).toBe('P1234');
    expect(wrapper.find('.selected-file-name').text()).toBe('DOCUMENT.PDF');
    expect(uploads(wrapper)).toHaveLength(1);
  });

  it('does not offer the type already chosen for the other identification', async () => {
    const wrapper = setup({ identificationType1: { id: 'type-sin', value: 'SIN / SSN' } });
    await vi.waitFor(() => { expect(typeSelect(wrapper, 'identificationType1').props('modelValue')).toEqual(sin); });

    const options = wrapper.findAll('select[name="identificationType2"] option');
    expect(options.map((o) => [o.text(), o.attributes('disabled') !== undefined])).toEqual([
      ['Select', true],
      ['SIN / SSN', true],
      ['Passport', false],
    ]);
  });

  it('requires the first identification type', async () => {
    const wrapper = setup();
    await catalogLoaded(wrapper);
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Identification type is required']); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(requiredFieldsMessage); });
    expect(createWorkerDocuments).not.toHaveBeenCalled();
  });

  it('requires a SIN between 9 and 15 characters', async () => {
    const wrapper = setup();
    await catalogLoaded(wrapper);
    typeSelect(wrapper, 'identificationType1').vm.$emit('update:modelValue', sin);
    await wrapper.find('input[name="identificationNumber1"]').setValue('1234');
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['SIN / SSN must be between 9 and 15 characters']); });
    expect(createWorkerDocuments).not.toHaveBeenCalled();
  });

  it('requires the second identification type when its number is given', async () => {
    const wrapper = setup();
    await catalogLoaded(wrapper);
    typeSelect(wrapper, 'identificationType1').vm.$emit('update:modelValue', passport);
    await wrapper.find('input[name="identificationNumber1"]').setValue('P1234');
    await wrapper.find('input[name="identificationNumber2"]').setValue('999');
    await save(wrapper);

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Identification type is required']); });
    expect(createWorkerDocuments).not.toHaveBeenCalled();
  });

  it('uploads the identifications with their files and emits closeModal', async () => {
    vi.mocked(createWorkerDocuments).mockResolvedValue();
    const wrapper = setup({ havePoliceCheckBackground: true });
    await catalogLoaded(wrapper);
    typeSelect(wrapper, 'identificationType1').vm.$emit('update:modelValue', passport);
    await wrapper.find('input[name="identificationNumber1"]').setValue('P1234');
    uploads(wrapper)[0]?.vm.$emit('update:modelValue', passportFile);
    await vi.waitFor(() => { expect(wrapper.find('.selected-file-name').text()).toBe('DOCUMENT.PDF'); });
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(vi.mocked(createWorkerDocuments).mock.calls[0]?.[0]).toBe('worker-profile-1');
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toEqual({
      identificationType1: passport,
      identificationNumber1: 'P1234',
      identificationType1File: { fileName: expect.stringMatching(/^Document_[0-9a-f]{32}\.pdf$/), description: '' },
      identificationType2: null,
      identificationNumber2: '',
      identificationType2File: null,
      havePoliceCheckBackground: true,
    });
    const { identificationType1File } = payload as { identificationType1File: { fileName: string } };
    expect(formData.get(identificationType1File.fileName)).toBeInstanceOf(File);
  });

  it('uploads a file for the second identification', async () => {
    vi.mocked(createWorkerDocuments).mockResolvedValue();
    const wrapper = setup({ identificationType1: { id: 'type-passport', value: 'Passport' }, identificationNumber1: 'P1234' });
    await vi.waitFor(() => { expect(typeSelect(wrapper, 'identificationType1').props('modelValue')).toEqual(passport); });
    typeSelect(wrapper, 'identificationType2').vm.$emit('update:modelValue', sin);
    await wrapper.find('input[name="identificationNumber2"]').setValue('123456789');
    uploads(wrapper)[1]?.vm.$emit('update:modelValue', passportFile);
    await vi.waitFor(() => { expect(uploads(wrapper)).toHaveLength(1); });
    await save(wrapper);

    await vi.waitFor(() => { expect(createWorkerDocuments).toHaveBeenCalledTimes(1); });
    const formData = sentFormData();
    const payload = formDataPayload(formData);
    expect(payload).toMatchObject({
      identificationType1File: null,
      identificationType2: sin,
      identificationNumber2: '123456789',
      identificationType2File: { fileName: expect.stringMatching(/^Document_[0-9a-f]{32}\.pdf$/) },
    });
    const { identificationType2File } = payload as { identificationType2File: { fileName: string } };
    expect(formData.get(identificationType2File.fileName)).toBeInstanceOf(File);
  });

  it('rejects files over 15MB', async () => {
    const wrapper = setup();
    uploads(wrapper)[0]?.vm.$emit('update:modelValue', oversizedFile('big.pdf'));
    uploads(wrapper)[1]?.vm.$emit('update:modelValue', oversizedFile('big.pdf'));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledTimes(2); });
    expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit');
    expect(uploads(wrapper)).toHaveLength(2);
  });

  it('lets both identification files be removed', async () => {
    const wrapper = setup({
      identificationType1File: { fileName: 'Document_a.pdf', description: '' },
      identificationType2File: { fileName: 'Document_b.pdf', description: '' },
    });
    await catalogLoaded(wrapper);
    expect(uploads(wrapper)).toHaveLength(0);
    const removeButtons = wrapper.findAll('.selected-file-display button');
    await removeButtons[0]?.trigger('click');
    await removeButtons[1]?.trigger('click');

    expect(wrapper.find('.selected-file-name').exists()).toBe(false);
    expect(uploads(wrapper)).toHaveLength(2);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerDocuments).mockRejectedValue(error);
    const wrapper = setup({ identificationType1: { id: 'type-passport', value: 'Passport' }, identificationNumber1: 'P1234' });
    await vi.waitFor(() => { expect(typeSelect(wrapper, 'identificationType1').props('modelValue')).toEqual(passport); });
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
