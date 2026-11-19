import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import DeleteInvoice from '@/modules/agency/accounting/invoices/components/DeleteInvoice.vue';
import { deleteAgencyInvoice, getPayStubsByInvoice } from '@/modules/agency/accounting/invoices/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInvoiceListItem } from '@/testing/accountingFixtures';

vi.mock('@/modules/agency/accounting/invoices/api', () => ({
  deleteAgencyInvoice: vi.fn(),
  getPayStubsByInvoice: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));

async function setup() {
  const wrapper = mountWithPlugins(DeleteInvoice, { props: { invoice: createInvoiceListItem() } });
  await flushPromises();
  return wrapper;
}

function deleteButton(wrapper: Awaited<ReturnType<typeof setup>>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Delete');
  if (!button) throw new Error('Delete button not found');
  return button;
}

describe('DeleteInvoice', () => {
  it('warns about the invoice number without a pay stub table when none are linked', async () => {
    vi.mocked(getPayStubsByInvoice).mockResolvedValue([]);
    const wrapper = await setup();

    expect(getPayStubsByInvoice).toHaveBeenCalledExactlyOnceWith('inv-1');
    expect(wrapper.text()).toContain('You are about to delete the invoice INV-0010');
    expect(wrapper.text()).not.toContain('If you also want to delete the paystubs');
    expect(wrapper.find('.paystubs-table').exists()).toBe(false);
  });

  it('deletes only the invoice when no pay stub is checked', async () => {
    vi.mocked(getPayStubsByInvoice).mockResolvedValue([{ payStubId: 'ps-1', payStubNumber: 'PS-1' }]);
    vi.mocked(deleteAgencyInvoice).mockResolvedValue();
    const wrapper = await setup();

    expect(wrapper.find('.paystubs-table').text()).toContain('PS-1');
    await deleteButton(wrapper).trigger('click');
    await flushPromises();

    expect(deleteAgencyInvoice).toHaveBeenCalledExactlyOnceWith({ invoiceId: 'inv-1', payStubs: [] });
    expect(wrapper.emitted('deleted')).toHaveLength(1);
  });

  it('deletes the checked pay stubs with the invoice', async () => {
    vi.mocked(getPayStubsByInvoice).mockResolvedValue([
      { payStubId: 'ps-1', payStubNumber: 'PS-1' },
      { payStubId: 'ps-2', payStubNumber: 'PS-2' },
    ]);
    vi.mocked(deleteAgencyInvoice).mockResolvedValue();
    const wrapper = await setup();

    expect(wrapper.text()).toContain('If you also want to delete the paystubs');
    await wrapper.findAll('tbody .checkbox-cell label')[1].trigger('click');
    await deleteButton(wrapper).trigger('click');
    await flushPromises();

    expect(deleteAgencyInvoice).toHaveBeenCalledExactlyOnceWith({ invoiceId: 'inv-1', payStubs: ['ps-2'] });
  });

  it('shows the error and does not emit when the deletion fails', async () => {
    const error = new Error('locked');
    vi.mocked(getPayStubsByInvoice).mockResolvedValue([]);
    vi.mocked(deleteAgencyInvoice).mockRejectedValue(error);
    const wrapper = await setup();
    await deleteButton(wrapper).trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('deleted')).toBeUndefined();
  });
});
