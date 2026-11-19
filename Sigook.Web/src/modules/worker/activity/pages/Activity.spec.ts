import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Activity from '@/modules/worker/activity/pages/Activity.vue';
import { downloadWorkerPayStubPdf, getActivitySummary } from '@/modules/worker/activity/api';
import { downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { useWorkerStore } from '@/modules/worker/store';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerActivitySummary, WorkerPayStubListItem } from '@/modules/worker/activity/types';

const { replace, query, stub } = vi.hoisted(() => ({
  replace: vi.fn(),
  query: { value: {} as Record<string, string> },
  stub: (name: string, props: Record<string, unknown> = {}, emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ query: query.value }),
  useRouter: () => ({ replace }),
}));
vi.mock('@/modules/worker/activity/api', () => ({ downloadWorkerPayStubPdf: vi.fn(), getActivitySummary: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadPDF: vi.fn(), openFileUrl: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/ui/PageHeader.vue', () => ({ default: stub('PageHeader', { title: String }) }));
vi.mock('@/modules/worker/activity/components/SummaryCards.vue', () => ({
  default: stub('SummaryCards', { summary: Object, showLastPay: Boolean }, ['download']),
}));
vi.mock('@/modules/worker/activity/components/PayStubsTab.vue', () => ({ default: stub('PayStubsTab', {}, ['download']) }));
vi.mock('@/modules/worker/activity/components/TimeSheetsTab.vue', () => ({ default: stub('TimeSheetsTab') }));
vi.mock('@/modules/worker/activity/components/HistoryTab.vue', () => ({ default: stub('HistoryTab') }));

let pinia: Pinia;

const summary: WorkerActivitySummary = { lastPayStub: null, weekHours: { total: 0, regular: 0, overtime: 0, holiday: 0, pendingApproval: 0 } };

const payStub: WorkerPayStubListItem = {
  id: 'ps1',
  payStubNumber: 'PS-100',
  weekEnding: '2026-05-30',
  paymentDate: '2026-06-05',
  hours: 40,
  totalEarnings: 800,
  totalDeductions: 120,
  totalPaid: 680,
  companies: [],
  pdfUrl: null,
};

async function setup(isUSA = false) {
  useWorkerStore().setWorkerProfile({ location: { isUSA } as never });
  const wrapper = mountWithPlugins(Activity, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

function activeTab(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findComponent({ name: 'BTabs' }).props('modelValue');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  query.value = {};
  vi.useFakeTimers({ now: new Date('2026-06-03T10:00:00'), toFake: ['Date'] });
  vi.mocked(getActivitySummary).mockResolvedValue(summary);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('Activity', () => {
  it('loads the summary of the current week and starts on pay stubs', async () => {
    const wrapper = await setup();
    expect(getActivitySummary).toHaveBeenCalledWith('2026-05-31');
    expect(wrapper.findComponent({ name: 'SummaryCards' }).props()).toEqual({ summary, showLastPay: true });
    expect(activeTab(wrapper)).toBe('paystubs');
    expect(wrapper.findComponent({ name: 'PayStubsTab' }).exists()).toBe(true);
  });

  it('opens the tab from the query', async () => {
    query.value = { tab: 'history' };
    const wrapper = await setup();
    expect(activeTab(wrapper)).toBe('history');
  });

  it('ignores an unknown tab', async () => {
    query.value = { tab: 'nope' };
    expect(activeTab(await setup())).toBe('paystubs');
  });

  it('hides pay stubs for USA workers', async () => {
    const wrapper = await setup(true);
    expect(activeTab(wrapper)).toBe('timesheets');
    expect(wrapper.findComponent({ name: 'SummaryCards' }).props('showLastPay')).toBe(false);
  });

  it('keeps the tab in the url', async () => {
    query.value = { from: 'menu' };
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', 'timesheets');
    expect(replace).toHaveBeenCalledWith({ query: { from: 'menu', tab: 'timesheets' } });
  });

  it('opens a stored pay stub pdf', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'PayStubsTab' }).vm.$emit('download', { ...payStub, pdfUrl: 'https://files/ps.pdf' });
    await flushPromises();
    expect(openFileUrl).toHaveBeenCalledWith('https://files/ps.pdf');
    expect(downloadWorkerPayStubPdf).not.toHaveBeenCalled();
  });

  it('generates a pay stub pdf', async () => {
    const blob = new Blob(['pdf']);
    vi.mocked(downloadWorkerPayStubPdf).mockResolvedValue(blob);
    const wrapper = await setup();
    wrapper.findComponent({ name: 'SummaryCards' }).vm.$emit('download', payStub);
    await flushPromises();
    expect(downloadWorkerPayStubPdf).toHaveBeenCalledWith('ps1');
    expect(downloadPDF).toHaveBeenCalledWith(blob, 'PS-100');
  });

  it('reports failures', async () => {
    vi.mocked(getActivitySummary).mockRejectedValue('summary down');
    vi.mocked(downloadWorkerPayStubPdf).mockRejectedValue('pdf down');
    const wrapper = await setup();
    expect(showAlertError).toHaveBeenCalledWith('summary down');
    wrapper.findComponent({ name: 'PayStubsTab' }).vm.$emit('download', payStub);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('pdf down');
  });
});
