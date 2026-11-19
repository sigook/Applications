import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick, ref } from 'vue';
import Dashboard from '@/modules/agency/sales/dashboard/pages/Dashboard.vue';
import { getDealsByStatus, getSalesDashboard } from '@/modules/agency/sales/dashboard/api';
import { DealStatus, InteractionType } from '@/modules/agency/sales/clients/types';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createDeal, createInteraction, createRecentClient } from '@/testing/salesFixtures';
import type { DealsByStatusModel, SalesDashboardModel } from '@/modules/agency/sales/dashboard/types';

const { modalStub } = vi.hoisted(() => ({
  modalStub: (name: string, props: Record<string, unknown>) => ({
    name,
    props: { modelValue: Boolean, ...props },
    emits: ['saved', 'update:modelValue'],
    template: `<div class="${name}" />`,
  }),
}));

const agentName = ref('');
const loadAgentName = vi.fn<() => Promise<void>>();

vi.mock('@/modules/agency/sales/dashboard/api', () => ({ getDealsByStatus: vi.fn(), getSalesDashboard: vi.fn() }));
vi.mock('@/modules/agency/sales/dashboard/useCurrentAgent', () => ({ useCurrentAgent: () => ({ agentName, loadAgentName }) }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/sales/dashboard/components/BarChart.vue', () => ({
  default: { name: 'BarChart', props: { title: String, points: Array }, template: '<div class="bar-chart-stub" />' },
}));
vi.mock('@/modules/agency/sales/clients/components/InteractionModal.vue', () => ({ default: modalStub('InteractionModal', { interaction: Object }) }));
vi.mock('@/modules/agency/sales/clients/components/DealModal.vue', () => ({ default: modalStub('DealModal', { deal: Object }) }));
vi.mock('@/modules/agency/sales/dashboard/components/ClientModal.vue', () => ({ default: modalStub('ClientModal', {}) }));
vi.mock('@/modules/agency/sales/dashboard/components/ClientInteractionsModal.vue', () => ({ default: modalStub('ClientInteractionsModal', { client: Object }) }));

const interaction = createInteraction();
const client = createRecentClient();
const deal = createDeal();

const dealsByStatus: DealsByStatusModel = {
  period: { from: '2026-11-01', to: '2026-11-30', label: 'November 2026' },
  totalCount: 5,
  totalValue: 42_500,
  items: [
    { status: DealStatus.Accepted, count: 2, totalValue: 30_000 },
    { status: DealStatus.Sent, count: 3, totalValue: 12_500 },
  ],
};

const dashboard: SalesDashboardModel = {
  summary: {
    asOf: '2026-11-19T10:00:00',
    quarter: { from: '2026-10-01T00:00:00', to: '2026-12-31T00:00:00', label: 'Q4 2026' },
    week: { from: '2026-11-16T00:00:00', to: '2026-11-22T00:00:00', label: 'Nov 16 – 22' },
    pipeline: [
      { status: DealStatus.Completed, count: 1, totalValue: 5000 },
      { status: DealStatus.ToSend, count: 4, totalValue: 1000 },
    ],
    activity: [
      { type: InteractionType.Mail, count: 6 },
      { type: InteractionType.Call, count: 3 },
    ],
  },
  dealsByStatus,
  recentClients: [client],
  recentInteractions: [interaction],
  recentDeals: [deal],
};

async function setup() {
  const wrapper = mountWithPlugins(Dashboard);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function card(wrapper: Wrapper, title: string) {
  const found = wrapper.findAllComponents({ name: 'DashboardCard' }).find((c) => c.props('title') === title);
  if (!found) throw new Error(`${title} not found`);
  return found;
}

function meters(wrapper: Wrapper, index: number) {
  return wrapper.findAllComponents({ name: 'MeterList' })[index];
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date'] });
  agentName.value = '';
  loadAgentName.mockResolvedValue();
  vi.mocked(getSalesDashboard).mockResolvedValue(dashboard);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('Dashboard', () => {
  it('loads every card for the current month', async () => {
    agentName.value = 'Rita Sales';
    const wrapper = await setup();

    expect(getSalesDashboard).toHaveBeenCalledExactlyOnceWith({ dateFrom: '2026-11-01', dateTo: '2026-11-30', statuses: undefined });
    expect(loadAgentName).toHaveBeenCalledOnce();
    expect(wrapper.find('h2').text()).toContain('· Rita Sales');
    expect(wrapper.find('.sd-period').text()).toContain('Q4 2026 · ');
    expect(wrapper.findComponent({ name: 'InteractionList' }).props('items')).toEqual([interaction]);
    expect(wrapper.findComponent({ name: 'ClientList' }).props('items')).toEqual([client]);
    expect(wrapper.findComponent({ name: 'DealList' }).props('items')).toEqual([deal]);
  });

  it('summarizes deals by status in status order', async () => {
    const wrapper = await setup();

    expect(card(wrapper, 'Deals by status').find('.sd-card__subtitle').text()).toBe('Total 5 · $42.5K');
    expect(wrapper.findComponent({ name: 'BarChart' }).props('points')).toEqual([
      { key: String(DealStatus.Sent), label: 'Sent', value: 3, color: '#21b7ff', caption: '$12.5K' },
      { key: String(DealStatus.Accepted), label: 'Accepted', value: 2, color: '#3eb800', caption: '$30K' },
    ]);
  });

  it('shows the quarter pipeline and the weekly activity', async () => {
    const wrapper = await setup();

    expect(meters(wrapper, 0).props('items')).toEqual([
      { label: 'To Send', count: 4, color: '#9ad6ff' },
      { label: 'Completed', count: 1, color: '#0f9d8a' },
    ]);
    expect(meters(wrapper, 1).props('title')).toBe('Activity this week · Nov 16 – 22');
    expect(meters(wrapper, 1).props('items')).toEqual([
      { label: 'Call', count: 3, color: '#21b7ff' },
      { label: 'Email', count: 6, color: '#3eb800' },
    ]);
  });

  it('says when no deals are dated in the period', async () => {
    vi.mocked(getSalesDashboard).mockResolvedValue({ ...dashboard, dealsByStatus: { ...dealsByStatus, totalCount: 0, totalValue: 0, items: [] } });
    const wrapper = await setup();

    expect(wrapper.text()).toContain('No deals dated in this period');
    expect(wrapper.findComponent({ name: 'BarChart' }).exists()).toBe(false);
  });

  it('shows the error and empty cards when the dashboard fails', async () => {
    const error = new Error('dashboard');
    vi.mocked(getSalesDashboard).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(card(wrapper, 'Deals by status').find('.sd-card__subtitle').text()).toBe('Total 0');
    expect(meters(wrapper, 1).props('title')).toBe('Activity this week');
  });

  it('reloads deals by status for the picked range and statuses', async () => {
    vi.mocked(getDealsByStatus).mockResolvedValue({ ...dealsByStatus, totalCount: 1, totalValue: 900, items: [{ status: DealStatus.Rejected, count: 1, totalValue: 900 }] });
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: DealStatus.Rejected, value: 'Rejected' }]);
    await flushPromises();

    expect(getDealsByStatus).toHaveBeenLastCalledWith({ dateFrom: '2026-11-01', dateTo: '2026-11-30', statuses: [DealStatus.Rejected] });

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date(2026, 9, 1), new Date(2026, 9, 31)]);
    await flushPromises();

    expect(getDealsByStatus).toHaveBeenLastCalledWith({ dateFrom: '2026-10-01', dateTo: '2026-10-31', statuses: [DealStatus.Rejected] });
    expect(card(wrapper, 'Deals by status').find('.sd-card__subtitle').text()).toBe('Total 1 · $900');
  });

  it('keeps only the latest deals by status response', async () => {
    let resolveFirst: (value: DealsByStatusModel) => void = () => undefined;
    vi.mocked(getDealsByStatus)
      .mockReturnValueOnce(new Promise((resolve) => { resolveFirst = resolve; }))
      .mockResolvedValueOnce({ ...dealsByStatus, totalCount: 7 });
    const wrapper = await setup();
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    taginput.vm.$emit('update:modelValue', [{ id: DealStatus.Sent, value: 'Sent' }]);
    taginput.vm.$emit('update:modelValue', []);
    await flushPromises();
    resolveFirst({ ...dealsByStatus, totalCount: 99 });
    await flushPromises();

    expect(card(wrapper, 'Deals by status').find('.sd-total').text()).toBe('7');
  });

  it('shows the error when deals by status fail', async () => {
    const error = new Error('status');
    vi.mocked(getDealsByStatus).mockRejectedValue(error);
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', []);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the interaction modal to log and to edit', async () => {
    const wrapper = await setup();
    card(wrapper, 'Log Interactions').vm.$emit('action');
    await nextTick();
    expect(wrapper.findComponent({ name: 'InteractionModal' }).props()).toEqual({ modelValue: true, interaction: null });

    wrapper.findComponent({ name: 'InteractionList' }).vm.$emit('edit', interaction);
    await nextTick();
    expect(wrapper.findComponent({ name: 'InteractionModal' }).props('interaction')).toEqual(interaction);
  });

  it('opens the deal modal to create and to edit', async () => {
    const wrapper = await setup();
    card(wrapper, 'Deals').vm.$emit('action');
    await nextTick();
    expect(wrapper.findComponent({ name: 'DealModal' }).props()).toEqual({ modelValue: true, deal: null });

    wrapper.findComponent({ name: 'DealList' }).vm.$emit('edit', deal);
    await nextTick();
    expect(wrapper.findComponent({ name: 'DealModal' }).props('deal')).toEqual(deal);
  });

  it('opens the new client modal and the client interactions', async () => {
    const wrapper = await setup();
    card(wrapper, 'Clients').vm.$emit('action');
    await nextTick();
    expect(wrapper.findComponent({ name: 'ClientModal' }).props('modelValue')).toBe(true);

    wrapper.findComponent({ name: 'ClientList' }).vm.$emit('select', client);
    await nextTick();
    expect(wrapper.findComponent({ name: 'ClientInteractionsModal' }).props()).toEqual({ modelValue: true, client });
  });

  it.each(['InteractionModal', 'DealModal', 'ClientModal', 'ClientInteractionsModal'])('refreshes the dashboard when %s saves', async (name) => {
    const wrapper = await setup();
    wrapper.findComponent({ name }).vm.$emit('saved');
    await nextTick();
    expect(wrapper.findComponent({ name: 'DealList' }).props('loading')).toBe(true);
    await flushPromises();

    expect(getSalesDashboard).toHaveBeenCalledTimes(2);
    expect(wrapper.findComponent({ name: 'DealList' }).props('loading')).toBe(false);
  });
});
