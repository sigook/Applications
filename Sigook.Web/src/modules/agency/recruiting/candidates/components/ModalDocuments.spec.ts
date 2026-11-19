import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import ModalDocuments from '@/modules/agency/recruiting/candidates/components/ModalDocuments.vue';
import { deleteCandidateDocument, getCandidateDocuments } from '@/modules/agency/recruiting/candidates/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CandidateDocument } from '@/modules/agency/recruiting/candidates/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ getCandidateDocuments: vi.fn(), deleteCandidateDocument: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/components/DocumentsForm.vue', () => ({
  default: { name: 'DocumentsForm', props: ['candidateId'], emits: ['created', 'close'], template: '<div class="form-stub" />' },
}));

const resume: CandidateDocument = { id: 'd1', fileName: 'resume.pdf', pathFile: 'https://files/resume.pdf', description: 'Resume', canDownload: true };
const contract: CandidateDocument = { id: 'd2', fileName: 'contract.pdf', pathFile: 'https://files/contract.pdf', description: 'Contract', canDownload: true };

function page(items: CandidateDocument[]): PaginatedList<CandidateDocument> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages: 1 };
}

async function setup(candidateId: number | string = 42) {
  const wrapper = mountWithPlugins(ModalDocuments, { props: { candidateId } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function documents(wrapper: Wrapper) {
  return wrapper.findAll('.panel-block a').map((a) => [a.text(), a.attributes('href')]);
}

beforeEach(() => {
  vi.mocked(getCandidateDocuments).mockResolvedValue(page([resume, contract]));
});

describe('ModalDocuments', () => {
  it('lists the candidate documents', async () => {
    const wrapper = await setup();

    expect(getCandidateDocuments).toHaveBeenCalledWith('42');
    expect(documents(wrapper)).toEqual([['Resume', 'https://files/resume.pdf'], ['Contract', 'https://files/contract.pdf']]);
  });

  it('says when there are no documents', async () => {
    vi.mocked(getCandidateDocuments).mockResolvedValue(page([]));
    const wrapper = await setup();

    expect(wrapper.text()).toContain('No documents yet');
  });

  it('deletes a document', async () => {
    vi.mocked(deleteCandidateDocument).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('.panel-block button')[1].trigger('click');
    await flushPromises();

    expect(deleteCandidateDocument).toHaveBeenCalledExactlyOnceWith('42', 'd2');
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
    expect(documents(wrapper)).toEqual([['Resume', 'https://files/resume.pdf']]);
  });

  it('keeps the document when the delete fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(deleteCandidateDocument).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('.panel-block button')[0].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(documents(wrapper)).toHaveLength(2);
  });

  it('adds a document and reloads the list', async () => {
    const wrapper = await setup('cand-1');
    await wrapper.findAll('button').find((b) => b.text() === 'Add Document')?.trigger('click');
    await nextTick();

    const form = wrapper.findComponent({ name: 'DocumentsForm' });
    expect(form.props('candidateId')).toBe('cand-1');

    vi.mocked(getCandidateDocuments).mockResolvedValue(page([resume, contract, { ...resume, id: 'd3', description: 'Licence' }]));
    form.vm.$emit('created');
    form.vm.$emit('close');
    await flushPromises();

    expect(getCandidateDocuments).toHaveBeenCalledTimes(2);
    expect(documents(wrapper)).toHaveLength(3);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('closes', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Close')?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getCandidateDocuments).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.text()).toContain('No documents yet');
  });
});
