import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import MarkInvoicePaidModal from '@/modules/agency/accounting/invoices/components/MarkInvoicePaidModal.vue';
import { changeInvoiceStatus, updateInvoicePaidAt } from '@/modules/agency/accounting/invoices/api';
import { InvoiceStatus } from '@/modules/agency/accounting/invoices/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInvoiceListItem } from '@/testing/accountingFixtures';
import type { AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';

vi.mock('@/modules/agency/accounting/invoices/api', () => ({
  changeInvoiceStatus: vi.fn(),
  updateInvoicePaidAt: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function setup(overrides: Partial<AgencyInvoiceListItem> = {}) {
  return mountWithPlugins(MarkInvoicePaidModal, { props: { invoice: createInvoiceListItem(overrides) } });
}

function picker(wrapper: ReturnType<typeof setup>) {
  return wrapper.findComponent({ name: 'BDatepicker' });
}

afterEach(() => {
  vi.useRealTimers();
});

describe('MarkInvoicePaidModal', () => {
  it('marks a pending invoice as paid today by default', async () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T10:30:00'), toFake: ['Date'] });
    vi.mocked(changeInvoiceStatus).mockResolvedValue();
    const wrapper = setup();

    expect(wrapper.find('.modal-card-title').text()).toBe('Mark as Paid');
    expect(wrapper.text()).toContain('Invoice INV-0010 · Acme');
    await wrapper.find('form').trigger('submit');

    await vi.waitFor(() => { expect(wrapper.emitted('close')).toHaveLength(1); });
    expect(changeInvoiceStatus).toHaveBeenCalledExactlyOnceWith('inv-1', {
      status: InvoiceStatus.Paid,
      paidAt: new Date('2026-11-19T10:30:00').toISOString(),
    });
    expect(updateInvoicePaidAt).not.toHaveBeenCalled();
    expect(showAlertSuccess).toHaveBeenCalledWith('Invoice marked as Paid');
    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(wrapper.emitted('close')).toHaveLength(1);
  });

  it('edits the payment date of a paid invoice', async () => {
    vi.mocked(updateInvoicePaidAt).mockResolvedValue();
    const wrapper = setup({ status: InvoiceStatus.Paid, paidAt: '2026-11-10T05:00:00.000Z' });

    expect(wrapper.find('.modal-card-title').text()).toBe('Edit Payment Date');
    expect(picker(wrapper).props('modelValue')).toEqual(new Date('2026-11-10T05:00:00.000Z'));
    const picked = new Date(2026, 10, 12);
    picker(wrapper).vm.$emit('update:modelValue', picked);
    await flushPromises();
    await wrapper.find('form').trigger('submit');

    await vi.waitFor(() => { expect(updateInvoicePaidAt).toHaveBeenCalledExactlyOnceWith('inv-1', { paidAt: picked.toISOString() }); });
    expect(showAlertSuccess).toHaveBeenCalledWith('Payment date updated');
  });

  it('requires a payment date', async () => {
    const wrapper = setup({ status: InvoiceStatus.Paid, paidAt: null });
    await wrapper.find('form').trigger('submit');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please select the payment date'); });
    await vi.waitFor(() => { expect(wrapper.text()).toContain('Payment date is required'); });
    expect(updateInvoicePaidAt).not.toHaveBeenCalled();
  });

  it('keeps the modal open when saving fails', async () => {
    const error = new Error('nope');
    vi.mocked(changeInvoiceStatus).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('form').trigger('submit');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('close')).toBeUndefined();
  });

  it('closes from cancel', async () => {
    const wrapper = setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Cancel')?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
