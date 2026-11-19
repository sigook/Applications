import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import SendInvoiceEmail from '@/modules/agency/accounting/invoices/components/SendInvoiceEmail.vue';
import { sendInvoiceEmail } from '@/modules/agency/accounting/invoices/api';
import { getCompanyInvoiceRecipients } from '@/modules/agency/recruiting/clients/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInvoiceListItem } from '@/testing/accountingFixtures';
import type { InvoiceRecipientModel } from '@/modules/agency/recruiting/clients/types';

vi.mock('@/modules/agency/accounting/invoices/api', () => ({ sendInvoiceEmail: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getCompanyInvoiceRecipients: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const QuillStub = {
  name: 'QuillEditor',
  props: { content: String },
  emits: ['update:content'],
  template: '<div class="quill-stub">{{ content }}</div>',
};

async function setup(recipients: InvoiceRecipientModel[] = [{ email: 'ap@acme.test' }]) {
  vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue(recipients);
  const wrapper = mountWithPlugins(SendInvoiceEmail, {
    props: { invoice: createInvoiceListItem() },
    global: { stubs: { QuillEditor: QuillStub } },
  });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function taginput(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BTaginput' });
}

function attachments(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BTaglist' }).findAllComponents({ name: 'BTag' });
}

async function send(wrapper: Wrapper) {
  await wrapper.findAll('button').find((b) => b.text() === 'Send Email')?.trigger('click');
}

afterEach(() => {
  vi.useRealTimers();
});

describe('SendInvoiceEmail', () => {
  it('prefills the recipient, subject and body from the invoice', async () => {
    const wrapper = await setup();

    expect(getCompanyInvoiceRecipients).toHaveBeenCalledExactlyOnceWith('company-1');
    expect((wrapper.find('input[disabled]').element as HTMLInputElement).value).toBe('billing@acme.test');
    expect((wrapper.find('input[name="subject"]').element as HTMLInputElement).value).toBe('Invoice INV-0010 - Acme');
    expect(taginput(wrapper).props('modelValue')).toEqual([{ email: 'ap@acme.test' }]);
    expect(wrapper.find('.quill-stub').text()).toContain('Find your invoice attached');
  });

  it('sends the email with cc recipients and attachments', async () => {
    vi.mocked(sendInvoiceEmail).mockResolvedValue();
    const wrapper = await setup();
    const file = new File(['x'], 'timesheet.pdf');
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', [file]);
    await nextTick();
    await wrapper.find('input[name="subject"]').setValue('Invoice for November');
    wrapper.findComponent(QuillStub).vm.$emit('update:content', '<p>Hello</p>');
    await send(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('sent')).toHaveLength(1); });
    expect(sendInvoiceEmail).toHaveBeenCalledExactlyOnceWith({
      invoiceId: 'inv-1',
      recipients: ['ap@acme.test'],
      subject: 'Invoice for November',
      body: '<p>Hello</p>',
      attachments: [file],
    });
  });

  it('lists attachments and removes them', async () => {
    const wrapper = await setup();
    const keep = new File(['a'], 'keep.pdf');
    const drop = new File(['b'], 'drop.pdf');
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', [keep, drop]);
    await nextTick();

    expect(attachments(wrapper).map((t) => t.text())).toEqual(['keep.pdf', 'drop.pdf']);
    attachments(wrapper)[1].vm.$emit('close');
    await nextTick();

    expect(attachments(wrapper).map((t) => t.text())).toEqual(['keep.pdf']);
  });

  it('accepts valid cc emails and rejects invalid ones for 3 seconds', async () => {
    vi.useFakeTimers({ toFake: ['setTimeout'] });
    const wrapper = await setup([]);
    const beforeAdding = taginput(wrapper).props('beforeAdding') as (email: string) => boolean;
    const createTag = taginput(wrapper).props('createTag') as (email: string) => InvoiceRecipientModel;

    expect(beforeAdding('ops@acme.test')).toBe(true);
    expect(createTag('ops@acme.test')).toEqual({ email: 'ops@acme.test' });
    expect(beforeAdding('not-an-email')).toBe(false);
    await nextTick();
    expect(wrapper.text()).toContain('Please enter a valid email address');

    vi.advanceTimersByTime(3000);
    await nextTick();
    expect(wrapper.text()).not.toContain('Please enter a valid email address');
  });

  it('includes created cc tags in the recipients', async () => {
    vi.mocked(sendInvoiceEmail).mockResolvedValue();
    const wrapper = await setup([]);
    const createTag = taginput(wrapper).props('createTag') as (email: string) => InvoiceRecipientModel;
    createTag('ops@acme.test');
    await send(wrapper);

    await vi.waitFor(() => { expect(sendInvoiceEmail).toHaveBeenCalledOnce(); });
    expect(vi.mocked(sendInvoiceEmail).mock.calls[0][0].recipients).toEqual(['ops@acme.test']);
  });

  it('requires subject and a non-empty body', async () => {
    const wrapper = await setup();
    await wrapper.find('input[name="subject"]').setValue('');
    wrapper.findComponent(QuillStub).vm.$emit('update:content', '<p> </p>');
    await send(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Subject is required'); });
    expect(wrapper.text()).toContain('Body is required');
    expect(sendInvoiceEmail).not.toHaveBeenCalled();
  });

  it('shows the error when the recipients cannot load', async () => {
    const error = new Error('offline');
    vi.mocked(getCompanyInvoiceRecipients).mockRejectedValue(error);
    mountWithPlugins(SendInvoiceEmail, {
      props: { invoice: createInvoiceListItem() },
      global: { stubs: { QuillEditor: QuillStub } },
    });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('shows the error when sending fails', async () => {
    const error = new Error('smtp');
    vi.mocked(sendInvoiceEmail).mockRejectedValue(error);
    const wrapper = await setup();
    await send(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('sent')).toBeUndefined();
  });
});
