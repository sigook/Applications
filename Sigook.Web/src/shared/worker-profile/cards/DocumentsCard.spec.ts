import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import DocumentsCard from '@/shared/worker-profile/cards/DocumentsCard.vue';
import { deleteWorkerCertificates, deleteWorkerLicenses, deleteWorkerOtherDocuments } from '@/shared/worker-profile/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const { formStub } = vi.hoisted(() => ({
  formStub: (name: string) => ({ name, props: { data: Object }, emits: ['closeModal', 'closeAndUpdate'], template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/worker-profile/api', () => ({
  deleteWorkerCertificates: vi.fn(),
  deleteWorkerLicenses: vi.fn(),
  deleteWorkerOtherDocuments: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/worker-profile/forms/WorkDocumentsForm.vue', () => ({ default: formStub('DocumentsForm') }));
vi.mock('@/shared/worker-profile/forms/WorkResumeForm.vue', () => ({ default: formStub('ResumeForm') }));
vi.mock('@/shared/worker-profile/forms/WorkLicenseForm.vue', () => ({ default: formStub('LicenseForm') }));
vi.mock('@/shared/worker-profile/forms/WorkCertificatesForm.vue', () => ({ default: formStub('CertificateForm') }));
vi.mock('@/shared/worker-profile/forms/WorkerOtherDocumentsForm.vue', () => ({ default: formStub('OtherDocumentsForm') }));

const fullWorker: Partial<WorkerProfileDetail> = {
  identificationType1: { id: 'i1', value: 'Passport' },
  identificationNumber1: 'P123',
  identificationType1File: { id: 'f1', fileName: 'passport.pdf', pathFile: 'https://files/passport.pdf' },
  identificationType2: { id: 'i2', value: 'Driver licence' },
  identificationNumber2: 'D456',
  havePoliceCheckBackground: true,
  resume: { id: 'r1', fileName: 'resume.pdf', pathFile: 'https://files/resume.pdf' },
  licenses: [{ license: { id: 'l1', fileName: 'forklift_card.pdf', pathFile: 'https://files/forklift.pdf' }, number: 'FL-1', issued: null, expires: '2026-05-01T12:00:00' }],
  certificates: [{ id: 'c1', description: 'WHMIS', pathFile: 'https://files/whmis.pdf' }],
  otherDocuments: [{ id: 'o1', pathFile: 'https://files/other.pdf' }],
};

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(DocumentsCard, { props: { worker: createWorkerProfileDetail(overrides) } });
}

function rows(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 5).map((cell) => cell.text()));
}

function row(wrapper: ReturnType<typeof setup>, type: string) {
  const found = wrapper.findAll('tbody tr').find((r) => r.find('td').text() === type);
  if (!found) throw new Error(`${type} row not found`);
  return found;
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('DocumentsCard', () => {
  it('lists the missing identification and resume for an empty profile', () => {
    const wrapper = setup();

    expect(rows(wrapper)).toEqual([
      ['Identification 1', 'Not uploaded', '—', '—', 'Missing'],
      ['Resume', 'Not uploaded', '—', '—', 'Missing'],
    ]);
    expect(wrapper.findAll('tbody button').map((b) => b.text())).toEqual(['Upload', 'Upload']);
  });

  it('lists every document with its status', () => {
    vi.useFakeTimers({ now: new Date('2026-06-01T12:00:00'), toFake: ['Date'] });
    const wrapper = setup(fullWorker);

    expect(rows(wrapper)).toEqual([
      ['Identification 1', 'Passport', 'P123', '—', 'On file'],
      ['Identification 2', 'Driver licence', 'D456', '—', 'Missing'],
      ['Background', 'Police check', '—', '—', 'Missing'],
      ['Resume', 'Resume', '—', '—', 'On file'],
      ['License', 'FORKLIFT.PDF', 'FL-1', '01-May-2026', 'Expired'],
      ['Certificate', 'WHMIS', '—', '—', 'On file'],
      ['Other', 'Document', '—', '—', 'On file'],
    ]);
    expect(row(wrapper, 'Identification 1').find('a').attributes('href')).toBe('https://files/passport.pdf');
    expect(row(wrapper, 'License').find('.profile-status').classes()).toContain('is-danger');
  });

  it('names an identification without type after its file', () => {
    const wrapper = setup({ identificationType1File: { id: 'f1', pathFile: 'https://files/id.pdf' } });

    expect(rows(wrapper)[0]).toEqual(['Identification 1', 'Identification', '—', '—', 'On file']);
  });

  it('offers replace only for identification, resume and police check files', () => {
    const wrapper = setup({ ...fullWorker, policeCheckBackGround: { id: 'p1', pathFile: 'https://files/police.pdf' } });

    expect(row(wrapper, 'Identification 1').find('button[aria-label="Replace document"]').exists()).toBe(true);
    expect(row(wrapper, 'Background').find('button[aria-label="Replace document"]').exists()).toBe(true);
    expect(row(wrapper, 'Resume').find('button[aria-label="Replace document"]').exists()).toBe(true);
    expect(row(wrapper, 'License').find('button[aria-label="Replace document"]').exists()).toBe(false);
    expect(row(wrapper, 'License').find('button[aria-label="Delete document"]').exists()).toBe(true);
    expect(row(wrapper, 'Resume').find('button[aria-label="Delete document"]').exists()).toBe(false);
  });

  it.each([
    ['Identification 2', 'DocumentsForm'],
    ['Background', 'DocumentsForm'],
    ['Resume', 'ResumeForm'],
  ])('uploads the missing %s with the %s', async (type, name) => {
    const wrapper = setup({ ...fullWorker, resume: null });
    await row(wrapper, type).find('button').trigger('click');
    await nextTick();

    expect(wrapper.findComponent({ name }).exists()).toBe(true);
  });

  it.each([
    ['Identification', 'DocumentsForm', 'closeModal'],
    ['Resume', 'ResumeForm', 'closeModal'],
    ['License', 'LicenseForm', 'closeModal'],
    ['Certificate', 'CertificateForm', 'closeModal'],
    ['Other document', 'OtherDocumentsForm', 'closeAndUpdate'],
  ])('adds a %s and emits updateProfile once saved', async (item, name, event) => {
    const wrapper = setup();
    menuItem(item).click();
    await nextTick();

    const form = wrapper.findComponent({ name });
    expect(form.exists()).toBe(true);
    form.vm.$emit(event);
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });

  it.each([
    ['License', deleteWorkerLicenses, 'l1'],
    ['Certificate', deleteWorkerCertificates, 'c1'],
    ['Other', deleteWorkerOtherDocuments, 'o1'],
  ])('deletes the %s once confirmed', async (type, request, id) => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(request).mockResolvedValue();
    const wrapper = setup(fullWorker);
    await row(wrapper, type).find('button[aria-label="Delete document"]').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updateProfile')).toHaveLength(1); });
    expect(request).toHaveBeenCalledExactlyOnceWith('worker-profile-1', id);
    expect(wrapper.emitted('loading')).toEqual([[true]]);
  });

  it('keeps the document when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = setup(fullWorker);
    await row(wrapper, 'Certificate').find('button[aria-label="Delete document"]').trigger('click');

    await vi.waitFor(() => { expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'You want to delete this document'); });
    expect(deleteWorkerCertificates).not.toHaveBeenCalled();
    expect(wrapper.emitted('loading')).toBeUndefined();
  });

  it('stops loading and shows the error when the deletion fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteWorkerOtherDocuments).mockRejectedValue(error);
    const wrapper = setup(fullWorker);
    await row(wrapper, 'Other').find('button[aria-label="Delete document"]').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('loading')).toEqual([[true], [false]]);
    expect(wrapper.emitted('updateProfile')).toBeUndefined();
  });
});
