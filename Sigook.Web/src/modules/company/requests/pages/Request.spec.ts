import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Request from '@/modules/company/requests/pages/Request.vue';
import { cancelRequest, editRequest, getRequest, getRequestShift, requestAnotherWorker } from '@/modules/company/requests/api';
import { DurationTerm, RequestStatus } from '@/shared/constants/enums';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyRequestDetail } from '@/modules/company/requests/types';

const { route, push } = vi.hoisted(() => ({
  route: { params: { id: 'request-1' }, query: {} as Record<string, string> },
  push: vi.fn(),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/modules/company/requests/api', () => ({
  getRequest: vi.fn(),
  getRequestShift: vi.fn(),
  cancelRequest: vi.fn(),
  editRequest: vi.fn(),
  requestAnotherWorker: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/request-detail/CompanyCancelList.vue', () => ({
  default: { name: 'CancelList', emits: ['sendReason'], template: '<div class="cancel-list" />' },
}));
vi.mock('@/modules/company/requests/components/DialogRequestWorker.vue', () => ({
  default: { name: 'RequestAnotherWorker', emits: ['sendAnotherWorker'], template: '<div class="request-another" />' },
}));
vi.mock('@/modules/company/requests/components/CompanyRequestWorkers.vue', () => ({
  default: { name: 'Workers', props: { request: Object }, template: '<div class="workers" />' },
}));
vi.mock('@/shared/request-detail/AgencyShiftDetail.vue', () => ({
  default: { name: 'AgencyShift', props: { requestId: String, displayShift: String, fetchShift: Function }, template: '<span class="shift">{{ displayShift }}</span>' },
}));

const QuillEditor = {
  name: 'QuillEditor',
  props: { content: String },
  emits: ['update:content'],
  template: '<textarea class="quill" :value="content" @input="$emit(\'update:content\', $event.target.value)" />',
};

const longText = `<p>${'Steel-toe boots and forklift licence required. '.repeat(3)}</p>`;

function detail(overrides: Partial<CompanyRequestDetail> = {}): CompanyRequestDetail {
  return {
    id: 'request-1', numberId: 501, jobTitle: 'Forklift operator', workersQuantity: 4, workersQuantityWorking: 0,
    description: '<p>Warehouse work</p>', requirements: '<p>Boots</p>', durationBreak: '00:30:00', breakIsPaid: false,
    holidayIsPaid: true, isAsap: false, jobIsOnBranchOffice: false, jobPositionRate: { id: 'jp-1', value: 'Forklift' },
    status: RequestStatus.Open, durationTerm: DurationTerm.LongTerm, displayShift: 'Mon-Fri 08:00-16:00',
    createdAt: '2026-11-01T00:00:00', startAt: '2026-11-02T00:00:00', finishAt: null, ...overrides,
  };
}

async function setup(request: CompanyRequestDetail = detail()) {
  vi.mocked(getRequest).mockResolvedValue(request);
  const wrapper = mountWithPlugins(Request, { global: { components: { QuillEditor } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function menuItem(text: string) {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
}

function fields(wrapper: Wrapper) {
  return wrapper.findAll('.detail-field').map((f) => [f.find('dt').text(), f.find('dd').text()]);
}

beforeEach(() => {
  route.query = {};
  push.mockResolvedValue(undefined);
});

describe('Request', () => {
  it('loads the request and shows its detail', async () => {
    const wrapper = await setup();

    expect(getRequest).toHaveBeenCalledExactlyOnceWith('request-1');
    expect(wrapper.find('.detail-header-title').text()).toBe('501 · Forklift operator');
    expect(wrapper.find('.detail-chip').text()).toBe('Open');
    expect(fields(wrapper)).toEqual([
      ['Role (position)', 'Forklift'],
      ['Term', 'Long Term'],
      ['Break', '30 min break · unpaid'],
      ['Holiday', 'Paid'],
    ]);
    expect(wrapper.find('.detail-rich-text').text()).toBe('Warehouse work');
    expect(wrapper.findComponent({ name: 'AgencyShift' }).props()).toEqual({ requestId: 'request-1', displayShift: 'Mon-Fri 08:00-16:00', fetchShift: getRequestShift });
    expect(wrapper.findComponent({ name: 'Workers' }).exists()).toBe(false);
  });

  it('adds the salary and finish date to the kpis of a short-term direct hire', async () => {
    const longTerm = await setup();
    const longTermKpis = longTerm.findAll('.detail-kpi dt').map((k) => k.text());
    longTerm.unmount();
    const wrapper = await setup(detail({ workerSalary: 60000, durationTerm: DurationTerm.ShortTerm, finishAt: '2026-12-20T00:00:00', isAsap: true }));

    const kpis = wrapper.findAll('.detail-kpi').map((k) => k.text());
    expect(kpis.length).toBe(longTermKpis.length + 1);
    expect(kpis.join(' ')).toContain('60,000');
    expect(kpis.join(' ')).toContain('20-Dec-2026');
  });

  it('opens the workers tab from the query', async () => {
    route.query = { tab: 'Workers' };
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'Workers' }).props('request')).toMatchObject({ id: 'request-1' });
  });

  it('switches tabs and keeps it in the url', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', 'Workers');
    await flushPromises();

    expect(push).toHaveBeenCalledWith({ path: '/company-requests/request-1', query: { tab: 'Workers' } });
    expect(wrapper.findComponent({ name: 'Workers' }).exists()).toBe(true);
  });

  it('hides the actions of a cancelled request', async () => {
    const wrapper = await setup(detail({ status: RequestStatus.Cancelled }));

    expect(wrapper.find('.detail-header-actions').exists()).toBe(false);
    expect(wrapper.find('.request-link').exists()).toBe(false);
  });

  it('cannot cancel a request with working workers', async () => {
    const wrapper = await setup(detail({ workersQuantityWorking: 1 }));

    expect(wrapper.find('.detail-header-actions').exists()).toBe(true);
    expect(menuItem('Edit requirements')).toBeDefined();
    expect(menuItem('Cancel request')).toBeUndefined();
  });

  it('cancels the request with the chosen reason', async () => {
    vi.mocked(cancelRequest).mockResolvedValue();
    const wrapper = await setup();
    menuItem('Cancel request')?.click();
    await nextTick();
    wrapper.findComponent({ name: 'CancelList' }).vm.$emit('sendReason', { reasonId: 'reason-1', otherMessage: 'Budget' });
    await flushPromises();

    expect(cancelRequest).toHaveBeenCalledExactlyOnceWith('request-1', 'reason-1', 'Budget');
    expect(showAlertSuccess).toHaveBeenCalledWith('Cancelled');
    expect(push).toHaveBeenCalledWith('/company-requests');
  });

  it('shows the error when cancelling fails', async () => {
    const error = new Error('cancel failed');
    vi.mocked(cancelRequest).mockRejectedValue(error);
    const wrapper = await setup();
    menuItem('Cancel request')?.click();
    await nextTick();
    wrapper.findComponent({ name: 'CancelList' }).vm.$emit('sendReason', { reasonId: 'reason-1', otherMessage: '' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(push).not.toHaveBeenCalled();
  });

  it('requests another worker with a comment', async () => {
    vi.mocked(requestAnotherWorker).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.detail-header-actions button.is-primary').trigger('click');
    wrapper.findComponent({ name: 'RequestAnotherWorker' }).vm.$emit('sendAnotherWorker', 'One more please');
    await flushPromises();

    expect(requestAnotherWorker).toHaveBeenCalledExactlyOnceWith('request-1', { comments: 'One more please' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Requested');
  });

  it('shows the error when requesting another worker fails', async () => {
    const error = new Error('request failed');
    vi.mocked(requestAnotherWorker).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('.detail-header-actions button.is-primary').trigger('click');
    wrapper.findComponent({ name: 'RequestAnotherWorker' }).vm.$emit('sendAnotherWorker', 'One more please');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('edits the requirements', async () => {
    vi.mocked(editRequest).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.request-link').trigger('click');
    const editor = wrapper.find('.modal.is-active textarea.quill');
    expect((editor.element as HTMLTextAreaElement).value).toBe('<p>Boots</p>');

    await editor.setValue(longText);
    await wrapper.find('.modal.is-active button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertSuccess).toHaveBeenCalledWith('Updated'); });
    expect(editRequest).toHaveBeenCalledExactlyOnceWith('request-1', { requirements: longText });
    expect(wrapper.findAll('.detail-rich-text').map((r) => r.text())).toContain(longText.replace(/<[^>]*>/g, '').trim());
  });

  it('requires 100 characters of requirements', async () => {
    const wrapper = await setup();
    menuItem('Edit requirements')?.click();
    await nextTick();
    await wrapper.find('.modal.is-active button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.modal.is-active .help.is-danger').text()).toBe('Requirements must be at least 100 characters'); });
    expect(editRequest).not.toHaveBeenCalled();
  });

  it('shows the error when saving the requirements fails', async () => {
    vi.mocked(editRequest).mockRejectedValue({ data: 'edit failed' });
    const wrapper = await setup();
    await wrapper.find('.request-link').trigger('click');
    await wrapper.find('.modal.is-active textarea.quill').setValue(longText);
    await wrapper.find('.modal.is-active button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledExactlyOnceWith('edit failed'); });
  });

  it('shows the error when the request cannot load', async () => {
    vi.mocked(getRequest).mockRejectedValue({ data: 'not found' });
    const wrapper = mountWithPlugins(Request, { global: { components: { QuillEditor } } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('not found');
    expect(wrapper.find('.detail-header').exists()).toBe(false);
  });
});
