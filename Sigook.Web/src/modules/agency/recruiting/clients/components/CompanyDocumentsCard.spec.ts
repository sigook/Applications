import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyDocumentsCard from '@/modules/agency/recruiting/clients/components/CompanyDocumentsCard.vue';
import { deleteAgencyCompanyDocument, getAgencyCompanyDocument } from '@/modules/agency/recruiting/clients/api';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyProfileDocumentModel } from '@/modules/agency/recruiting/clients/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanyDocument: vi.fn(),
  deleteAgencyCompanyDocument: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/components/DocumentsForm.vue', () => ({
  default: { name: 'DocumentsForm', props: { profileId: String }, emits: ['onCreateDocument'], template: '<div class="documents-form-stub" />' },
}));

const contract: CompanyProfileDocumentModel = {
  id: 'd1',
  fileName: 'Company_abc.pdf',
  description: 'Signed contract',
  pathFile: 'https://files/contract.pdf',
  canDownload: true,
};
const policy: CompanyProfileDocumentModel = { id: 'd2', fileName: 'Company_def.pdf', canDownload: false };

function page(items: CompanyProfileDocumentModel[], totalItems = items.length): PaginatedList<CompanyProfileDocumentModel> {
  return { items, totalItems, pageIndex: 1, totalPages: 1 };
}

async function setup() {
  const wrapper = mountWithPlugins(CompanyDocumentsCard, { props: { profileId: 'company-1' } });
  await flushPromises();
  return wrapper;
}

describe('CompanyDocumentsCard', () => {
  it('shows a loading state, then an empty state', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([]));
    const wrapper = mountWithPlugins(CompanyDocumentsCard, { props: { profileId: 'company-1' } });
    expect(wrapper.text()).toContain('Loading…');

    await flushPromises();

    expect(getAgencyCompanyDocument).toHaveBeenCalledExactlyOnceWith('company-1', { size: 10, page: 1 });
    expect(wrapper.text()).toContain('No documents');
  });

  it('lists the documents, linking and deleting only downloadable ones', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract, policy]));
    const wrapper = await setup();

    const items = wrapper.findAll('.company-document');
    expect(items).toHaveLength(2);
    expect(items[0].find('a').attributes('href')).toBe('https://files/contract.pdf');
    expect(items[0].text()).toContain('Signed contract');
    expect(items[0].find('button[aria-label="Delete document"]').exists()).toBe(true);
    expect(items[1].find('a').exists()).toBe(false);
    expect(items[1].find('button[aria-label="Delete document"]').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BPagination' }).exists()).toBe(false);
  });

  it('paginates when there are more than ten documents', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract], 25));
    const wrapper = await setup();

    const pagination = wrapper.findComponent({ name: 'BPagination' });
    expect(pagination.props('total')).toBe(25);
    pagination.vm.$emit('update:modelValue', 2);
    pagination.vm.$emit('change', 2);
    await flushPromises();

    expect(getAgencyCompanyDocument).toHaveBeenLastCalledWith('company-1', { size: 10, page: 2 });
  });

  it('deletes a document after confirming', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract]));
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyDocument).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete document"]').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this document');
    expect(deleteAgencyCompanyDocument).toHaveBeenCalledExactlyOnceWith('company-1', 'd1');
    expect(wrapper.findAll('.company-document')).toHaveLength(0);
    expect(wrapper.emitted('changed')).toEqual([[-1]]);
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
  });

  it('keeps the document when the deletion is not confirmed', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract]));
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete document"]').trigger('click');
    await flushPromises();

    expect(deleteAgencyCompanyDocument).not.toHaveBeenCalled();
    expect(wrapper.findAll('.company-document')).toHaveLength(1);
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract]));
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyDocument).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete document"]').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('changed')).toBeUndefined();
  });

  it('reloads and counts a new document created in the modal', async () => {
    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([]));
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add document')?.trigger('click');
    const form = wrapper.findComponent({ name: 'DocumentsForm' });
    expect(form.props('profileId')).toBe('company-1');

    vi.mocked(getAgencyCompanyDocument).mockResolvedValue(page([contract]));
    form.vm.$emit('onCreateDocument');
    await flushPromises();

    expect(wrapper.emitted('changed')).toEqual([[1]]);
    expect(getAgencyCompanyDocument).toHaveBeenCalledTimes(2);
    expect(wrapper.findAll('.company-document')).toHaveLength(1);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCompanyDocument).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.text()).toContain('No documents');
  });
});
