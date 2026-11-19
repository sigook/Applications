import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import DealForm from '@/modules/agency/sales/clients/components/DealForm.vue';
import { createDeal, updateDeal } from '@/modules/agency/sales/clients/api';
import { getAgencyCompaniesList } from '@/modules/agency/recruiting/clients/api';
import { DealStatus, DealType } from '@/modules/agency/sales/clients/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { Deal } from '@/modules/agency/sales/clients/types';
import type { SalesClientReference } from '@/modules/agency/sales/dashboard/types';

vi.mock('@/modules/agency/sales/clients/api', () => ({ createDeal: vi.fn(), updateDeal: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompaniesList: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const deal: Deal = {
  id: 'deal-1',
  title: 'Warehouse staffing',
  companyProfileId: 'company-1',
  companyName: 'Acme Logistics',
  ownerId: 'user-1',
  date: '2026-11-10T05:00:00.000Z',
  value: 25000,
  type: DealType.Permanent,
  status: DealStatus.Sent,
  documentId: 'doc-1',
  documentName: 'proposal.pdf',
  documentPath: 'https://files/proposal.pdf',
  createdAt: '2026-11-01T12:00:00',
  updatedAt: '2026-11-01T12:00:00',
};

const acme: SalesClientReference = { id: 'company-1', fullName: 'Acme Logistics' };

async function setup(props: { deal?: Deal | null; initialClient?: SalesClientReference | null } = {}) {
  const wrapper = mountWithPlugins(DealForm, { props });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function submit(wrapper: Wrapper) {
  return (wrapper.vm as unknown as { submit: () => Promise<boolean> }).submit();
}

async function fill(wrapper: Wrapper) {
  await wrapper.find('input[name="title"]').setValue('  Night shift crew  ');
  wrapper.findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 1500.5);
  const [typeSelect, statusSelect] = wrapper.findAllComponents({ name: 'BSelect' });
  typeSelect.vm.$emit('update:modelValue', DealType.TempToPerm);
  statusSelect.vm.$emit('update:modelValue', DealStatus.UnderReview);
  wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 20));
  await nextTick();
}

describe('DealForm', () => {
  it('lists type and status options', async () => {
    const wrapper = await setup();
    const [typeSelect, statusSelect] = wrapper.findAllComponents({ name: 'BSelect' });

    expect(typeSelect.findAll('option').map((o) => o.text())).toEqual(['Temporal', 'Permanent', 'Temp to Perm']);
    expect(statusSelect.findAll('option').map((o) => o.text())).toEqual(['To Send', 'Sent', 'Under Review', 'Accepted', 'Rejected', 'Closed', 'Completed']);
  });

  it('preselects the initial client', async () => {
    const wrapper = await setup({ initialClient: acme });
    const select = wrapper.findComponent({ name: 'SearchSelect' });

    expect(select.props('modelValue')).toBe('company-1');
    expect(select.props('options')).toEqual([{ value: 'company-1', label: 'Acme Logistics' }]);
  });

  it('searches clients from three characters', async () => {
    vi.mocked(getAgencyCompaniesList).mockResolvedValue([{ id: 'company-2', value: 'Globex' }]);
    const wrapper = await setup({ initialClient: acme });
    const select = wrapper.findComponent({ name: 'SearchSelect' });

    select.vm.$emit('search', 'gl');
    await flushPromises();
    expect(getAgencyCompaniesList).not.toHaveBeenCalled();
    expect(select.props('options')).toEqual([]);

    select.vm.$emit('search', ' glo ');
    await flushPromises();
    expect(getAgencyCompaniesList).toHaveBeenCalledExactlyOnceWith('glo');
    expect(select.props('options')).toEqual([{ value: 'company-2', label: 'Globex' }]);
  });

  it('shows the error when the client search fails', async () => {
    const error = new Error('search');
    vi.mocked(getAgencyCompaniesList).mockRejectedValue(error);
    const wrapper = await setup();
    wrapper.findComponent({ name: 'SearchSelect' }).vm.$emit('search', 'acme');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('creates a deal with a document and resets the form', async () => {
    vi.mocked(createDeal).mockResolvedValue('deal-9');
    const wrapper = await setup({ initialClient: acme });
    await fill(wrapper);
    const file = new File(['%PDF'], 'proposal.pdf');
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', file);
    await nextTick();
    expect(wrapper.find('.file-name').text()).toBe('proposal.pdf');

    await expect(submit(wrapper)).resolves.toBe(true);

    expect(createDeal).toHaveBeenCalledExactlyOnceWith('company-1', {
      title: 'Night shift crew',
      date: new Date(2026, 10, 20).toISOString(),
      value: 1500.5,
      type: DealType.TempToPerm,
      status: DealStatus.UnderReview,
      documentId: null,
      fileName: expect.stringMatching(/^Deal_\w+\.pdf$/) as string,
    }, file);
    expect(showAlertSuccess).toHaveBeenCalledWith('Deal created');
    await flushPromises();
    expect((wrapper.find('input[name="title"]').element as HTMLInputElement).value).toBe('');
  });

  it('creates a deal with zero value and no document', async () => {
    vi.mocked(createDeal).mockResolvedValue('deal-9');
    const wrapper = await setup({ initialClient: acme });
    await wrapper.find('input[name="title"]').setValue('Intro');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 20));
    await nextTick();

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(vi.mocked(createDeal).mock.calls[0][1]).toMatchObject({ value: 0, type: DealType.Temporal, status: DealStatus.ToSend, fileName: null });
    expect(vi.mocked(createDeal).mock.calls[0][2]).toBeNull();
  });

  it('rejects a file with a disallowed extension', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', new File(['x'], 'virus.exe'));
    await nextTick();

    expect(wrapper.text()).toContain('File extension not allowed');
    expect(wrapper.find('.file-name').exists()).toBe(false);
  });

  it('requires title, client and date', async () => {
    const wrapper = await setup();

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    await flushPromises();
    expect(wrapper.text()).toContain('Deal title is required');
    expect(wrapper.text()).toContain('Client is required');
    expect(wrapper.text()).toContain('Date is required');
    expect(createDeal).not.toHaveBeenCalled();
  });

  it('loads the deal to edit with its document link', async () => {
    const wrapper = await setup({ deal });

    expect((wrapper.find('input[name="title"]').element as HTMLInputElement).value).toBe('Warehouse staffing');
    expect(wrapper.findComponent({ name: 'SearchSelect' }).exists()).toBe(false);
    expect(wrapper.find('.sd-readonly').text()).toBe('Acme Logistics');
    expect(wrapper.find('a.sd-document').attributes('href')).toBe('https://files/proposal.pdf');
    expect(wrapper.text()).toContain('Click to replace');
  });

  it('says no document is attached when editing a deal without one', async () => {
    const wrapper = await setup({ deal: { ...deal, documentId: null, documentPath: null } });

    expect(wrapper.find('p.sd-document').text()).toBe('No document attached');
    expect(wrapper.text()).toContain('Click to upload');
  });

  it('updates the deal keeping its document', async () => {
    vi.mocked(updateDeal).mockResolvedValue();
    const wrapper = await setup({ deal });
    await wrapper.find('input[name="title"]').setValue('Warehouse staffing v2');

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(updateDeal).toHaveBeenCalledExactlyOnceWith('company-1', 'deal-1', {
      title: 'Warehouse staffing v2',
      date: '2026-11-10T05:00:00.000Z',
      value: 25000,
      type: DealType.Permanent,
      status: DealStatus.Sent,
      documentId: 'doc-1',
      fileName: null,
    }, null);
    expect(showAlertSuccess).toHaveBeenCalledWith('Deal updated');
  });

  it('returns false and shows the error when saving fails', async () => {
    const error = new Error('save');
    vi.mocked(updateDeal).mockRejectedValue(error);
    const wrapper = await setup({ deal });

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
