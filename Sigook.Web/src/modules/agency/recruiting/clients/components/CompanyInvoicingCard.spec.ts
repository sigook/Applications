import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { defineComponent } from 'vue';
import CompanyInvoicingCard from '@/modules/agency/recruiting/clients/components/CompanyInvoicingCard.vue';
import { deleteCompanyInvoiceRecipient, getCompanyInvoiceRecipients, getInvoiceNotes, postCompanyInvoiceRecipient, postInvoiceNotes } from '@/modules/agency/recruiting/clients/api';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getInvoiceNotes: vi.fn(),
  postInvoiceNotes: vi.fn(),
  getCompanyInvoiceRecipients: vi.fn(),
  postCompanyInvoiceRecipient: vi.fn(),
  deleteCompanyInvoiceRecipient: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const QuillEditorStub = defineComponent({
  name: 'QuillEditor',
  props: { content: { type: String, default: '' }, toolbar: { type: Array, default: () => [] } },
  emits: ['update:content'],
  template: '<div class="quill-stub">{{ content }}</div>',
});

async function setup() {
  const wrapper = mountWithPlugins(CompanyInvoicingCard, {
    props: { profileId: 'company-1' },
    global: { components: { QuillEditor: QuillEditorStub } },
  });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

function recipients(wrapper: Wrapper) {
  return wrapper.findAll('.company-recipient').map((r) => r.text().replace(/\s+/g, ' '));
}

async function setNotes(wrapper: Wrapper, html: string) {
  wrapper.findComponent(QuillEditorStub).vm.$emit('update:content', html);
  await wrapper.vm.$nextTick();
}

describe('CompanyInvoicingCard', () => {
  it('loads the invoice notes and recipients', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({ htmlNotes: '<p>Net 30</p>' });
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([{ id: 'r1', name: 'Billing', email: 'billing@acme.com' }]);
    const wrapper = await setup();

    expect(getInvoiceNotes).toHaveBeenCalledWith('company-1');
    expect(getCompanyInvoiceRecipients).toHaveBeenCalledWith('company-1');
    expect(wrapper.findComponent(QuillEditorStub).props('content')).toBe('<p>Net 30</p>');
    expect(wrapper.text()).toContain('6 / 500');
    expect(recipients(wrapper)).toEqual([expect.stringContaining('Billing ·billing@acme.com') as string]);
  });

  it('shows an empty state and treats missing notes as empty', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    const wrapper = await setup();

    expect(wrapper.findComponent(QuillEditorStub).props('content')).toBe('');
    expect(wrapper.text()).toContain('No recipients');
  });

  it('shows the errors when loading fails', async () => {
    const notesError = new Error('notes');
    const recipientsError = new Error('recipients');
    vi.mocked(getInvoiceNotes).mockRejectedValue(notesError);
    vi.mocked(getCompanyInvoiceRecipients).mockRejectedValue(recipientsError);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(notesError);
    expect(showAlertError).toHaveBeenCalledWith(recipientsError);
  });

  it('saves the invoice notes', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({ htmlNotes: '' });
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    vi.mocked(postInvoiceNotes).mockResolvedValue();
    const wrapper = await setup();
    await setNotes(wrapper, '<p>PO required</p>');
    await button(wrapper, 'Save notes').trigger('click');
    await flushPromises();

    expect(postInvoiceNotes).toHaveBeenCalledExactlyOnceWith('company-1', { htmlNotes: '<p>PO required</p>' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('refuses notes longer than 500 characters', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({ htmlNotes: '' });
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    const wrapper = await setup();
    await setNotes(wrapper, `<p>${'x'.repeat(501)}</p>`);

    expect(wrapper.find('.is-over').text()).toBe('501 / 500');
    await button(wrapper, 'Save notes').trigger('click');

    expect(showAlertError).toHaveBeenCalledWith("Notes can't be greater than 500 characters.");
    expect(postInvoiceNotes).not.toHaveBeenCalled();
  });

  it('shows the error when saving the notes fails', async () => {
    const error = new Error('save failed');
    vi.mocked(getInvoiceNotes).mockResolvedValue({ htmlNotes: '' });
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    vi.mocked(postInvoiceNotes).mockRejectedValue(error);
    const wrapper = await setup();
    await button(wrapper, 'Save notes').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('validates a new recipient', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    const wrapper = await setup();
    await button(wrapper, 'Add').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Name is required', 'Email is required']); });

    await wrapper.find('input[aria-label="Recipient name"]').setValue('Al');
    await wrapper.find('input[aria-label="Recipient email"]').setValue('nope');
    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Min 3 characters', 'Invalid email']); });
    expect(postCompanyInvoiceRecipient).not.toHaveBeenCalled();
  });

  it('adds a recipient and clears the form', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    vi.mocked(postCompanyInvoiceRecipient).mockResolvedValue({ id: 'r2' });
    const wrapper = await setup();
    await wrapper.find('input[aria-label="Recipient name"]').setValue('Payables');
    await wrapper.find('input[aria-label="Recipient email"]').setValue('ap@acme.com');
    await button(wrapper, 'Add').trigger('click');

    await vi.waitFor(() => { expect(recipients(wrapper)).toHaveLength(1); });
    expect(postCompanyInvoiceRecipient).toHaveBeenCalledExactlyOnceWith('company-1', { name: 'Payables', email: 'ap@acme.com' });
    expect(recipients(wrapper)[0]).toContain('Payables ·ap@acme.com');
    expect((wrapper.find('input[aria-label="Recipient name"]').element as HTMLInputElement).value).toBe('');
  });

  it('shows the error when adding a recipient fails', async () => {
    const error = new Error('duplicate');
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([]);
    vi.mocked(postCompanyInvoiceRecipient).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('input[aria-label="Recipient name"]').setValue('Payables');
    await wrapper.find('input[aria-label="Recipient email"]').setValue('ap@acme.com');
    await button(wrapper, 'Add').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(recipients(wrapper)).toEqual([]);
  });

  it('removes a recipient after confirming', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([
      { id: 'r1', name: 'Billing', email: '<b>billing@acme.com</b>' },
      { id: 'r2', name: 'Payables', email: 'ap@acme.com' },
    ]);
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyInvoiceRecipient).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Remove <b>billing@acme.com</b>"]').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'Remove &lt;b&gt;billing@acme.com&lt;/b&gt; from invoice recipients');
    expect(deleteCompanyInvoiceRecipient).toHaveBeenCalledExactlyOnceWith('company-1', 'r1');
    expect(recipients(wrapper)).toEqual([expect.stringContaining('Payables') as string]);
  });

  it('keeps the recipient when removal is cancelled or fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([{ id: 'r1', name: 'Billing', email: 'billing@acme.com' }]);
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(deleteCompanyInvoiceRecipient).mockRejectedValue(error);
    const wrapper = await setup();
    const remove = wrapper.find('button[aria-label="Remove billing@acme.com"]');

    await remove.trigger('click');
    await flushPromises();
    expect(deleteCompanyInvoiceRecipient).not.toHaveBeenCalled();

    await remove.trigger('click');
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(recipients(wrapper)).toHaveLength(1);
  });

  it('ignores removing a recipient without id', async () => {
    vi.mocked(getInvoiceNotes).mockResolvedValue({});
    vi.mocked(getCompanyInvoiceRecipients).mockResolvedValue([{ name: 'Billing', email: 'billing@acme.com' }]);
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Remove billing@acme.com"]').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).not.toHaveBeenCalled();
  });
});
