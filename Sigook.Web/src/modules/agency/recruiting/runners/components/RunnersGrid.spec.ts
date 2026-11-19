import { describe, expect, it, vi } from 'vitest';
import { defineComponent, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import RunnersGrid from '@/modules/agency/recruiting/runners/components/RunnersGrid.vue';
import { deleteAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRunnerFilter } from '@/modules/agency/recruiting/requests/runners/types';
import type { RunnerRow } from '@/modules/agency/recruiting/runners/types';
import type { PaginatedList } from '@/shared/types/common';

const { panelReload } = vi.hoisted(() => ({ panelReload: vi.fn() }));

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ deleteAgencyRunner: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { confirm: vi.fn() };
  return { getDialog: () => dialog };
});
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerActionsDropdown.vue', () => ({
  default: { name: 'RunnerActionsDropdown', props: ['status', 'canEdit'], emits: ['open', 'delete'], template: '<div class="actions-stub" />' },
}));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerActionModals.vue', () => ({
  default: {
    name: 'RunnerActionModals',
    props: ['target', 'statusOpen', 'interviewOpen', 'historyOpen'],
    emits: ['updated', 'update:statusOpen', 'update:interviewOpen', 'update:historyOpen'],
    template: '<div class="modals-stub" />',
  },
}));
vi.mock('@/modules/agency/recruiting/runners/components/RunnerDetailPanel.vue', () => ({
  default: {
    name: 'RunnerDetailPanel',
    props: ['requestId', 'runnerId', 'canEdit'],
    emits: ['updated'],
    methods: { reload: panelReload },
    template: '<div class="panel-stub" />',
  },
}));

interface ConfirmOptions {
  message: string;
  onConfirm: () => Promise<void>;
}

const runners: RunnerRow[] = [
  {
    id: 'run-1',
    requestId: 'req-1',
    workerProfileId: 'wp-1',
    name: 'John Smith',
    email: 'john@example.com',
    type: RunnerType.Active,
    status: RunnerStatus.SentToClient,
    startDate: null,
    interviewsCount: 2,
    lastInterviewAt: '2026-03-05T14:00:00',
    createdAt: '2026-03-01T12:00:00',
    createdByEmail: 'jane.doe@sigook.com',
  },
  {
    id: 'run-2',
    requestId: 'req-1',
    workerProfileId: 'wp-2',
    name: 'Ana Lopez',
    type: RunnerType.Passive,
    status: RunnerStatus.Hired,
    startDate: '2026-03-09T12:00:00',
    interviewsCount: 0,
    createdAt: '2026-03-01T12:00:00',
  },
];

function setupLocal(props: { canEdit?: boolean; checkable?: boolean; emptyText?: string; data?: RunnerRow[] } = {}) {
  return mountWithPlugins(RunnersGrid, { props: { data: runners, ...props } });
}

async function setupServer(initial: AgencyRunnerFilter = { requestId: 'req-1' }) {
  const fetch = vi.fn<(p: AgencyRunnerFilter) => Promise<PaginatedList<RunnerRow>>>().mockResolvedValue({ items: runners, totalItems: 2, pageIndex: 1, totalPages: 1 });
  const Host = defineComponent({
    components: { RunnersGrid },
    setup() {
      const params = ref<AgencyRunnerFilter | undefined>(initial);
      return { params, fetch };
    },
    template: '<RunnersGrid :fetch="fetch" v-model:params="params" :sort-map="{ name: 0 }" />',
  });
  const host = mountWithPlugins(Host);
  await flushPromises();
  return { wrapper: host.findComponent({ name: 'RunnersGrid' }), fetch };
}

type Wrapper = Pick<ReturnType<typeof setupLocal>, 'findComponent' | 'findAll'>;

function grid(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'SigookGrid' });
}

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').filter((r) => !r.classes('detail'));
}

async function openDetail(wrapper: Wrapper, row: RunnerRow) {
  grid(wrapper).vm.$emit('cellclick', row, { field: 'status' });
  await flushPromises();
}

function lastConfirm(): ConfirmOptions {
  const call = vi.mocked(getDialog().confirm).mock.calls.at(-1);
  if (!call) throw new Error('confirm not opened');
  return call[0] as ConfirmOptions;
}

describe('RunnersGrid with local rows', () => {
  it('renders the runners', async () => {
    const wrapper = setupLocal();
    await flushPromises();
    const [first, second] = rows(wrapper);

    expect(first.text()).toContain('John Smith');
    expect(first.text()).toContain('Active');
    expect(first.text()).toMatch(/2\s+·/);
    expect(first.text()).toContain('—');
    expect(second.text()).toContain('Passive');
    expect(second.text()).not.toContain('—');
    expect(grid(wrapper).props('paginated')).toBe(false);
    expect(wrapper.find('thead input').exists()).toBe(false);
  });

  it('shows its empty text', async () => {
    const wrapper = setupLocal({ data: [], emptyText: 'Nobody sent yet' });
    await flushPromises();

    expect(wrapper.text()).toContain('Nobody sent yet');
  });

  it('opens the worker from the name and toggles the detail elsewhere', async () => {
    const wrapper = setupLocal();
    await flushPromises();

    grid(wrapper).vm.$emit('cellclick', runners[0], { field: 'actions' });
    await openDetail(wrapper, runners[0]);
    expect(grid(wrapper).props('openedDetailed')).toEqual(['run-1']);
    expect(wrapper.findComponent({ name: 'RunnerDetailPanel' }).props()).toEqual({ requestId: 'req-1', runnerId: 'run-1', canEdit: true });

    await openDetail(wrapper, runners[0]);
    expect(grid(wrapper).props('openedDetailed')).toEqual([]);

    grid(wrapper).vm.$emit('cellclick', runners[0], { field: 'name' });
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/recruiting/workers/wp-1');
  });

  it('opens the detail for the history action and a modal for the others', async () => {
    const wrapper = setupLocal();
    await flushPromises();
    const dropdown = wrapper.findAllComponents({ name: 'RunnerActionsDropdown' })[0];

    dropdown.vm.$emit('open', 'history');
    dropdown.vm.$emit('open', 'history');
    await flushPromises();
    expect(grid(wrapper).props('openedDetailed')).toEqual(['run-1']);

    dropdown.vm.$emit('open', 'interview');
    await flushPromises();
    const modals = wrapper.findComponent({ name: 'RunnerActionModals' });
    expect(modals.props()).toEqual({
      target: { requestId: 'req-1', runnerId: 'run-1', name: 'John Smith', status: RunnerStatus.SentToClient },
      statusOpen: false,
      interviewOpen: true,
      historyOpen: false,
    });
  });

  it('changes the status and deletes from the detail row', async () => {
    const wrapper = setupLocal();
    await flushPromises();
    await openDetail(wrapper, runners[0]);

    await wrapper.findAll('.runners-detail-actions button').find((b) => b.text() === 'Change status')?.trigger('click');
    expect(wrapper.findComponent({ name: 'RunnerActionModals' }).props('statusOpen')).toBe(true);

    await wrapper.findAll('.runners-detail-actions button').find((b) => b.text() === 'Delete runner')?.trigger('click');
    expect(lastConfirm().message).toContain('<strong>John Smith</strong>');
  });

  it('does not offer a status change for a hired runner', async () => {
    const wrapper = setupLocal();
    await flushPromises();
    await openDetail(wrapper, runners[1]);

    expect(wrapper.findAll('.runners-detail-actions button').map((b) => b.text())).toEqual(['Delete runner']);
  });

  it('is read only when it cannot be edited', async () => {
    const wrapper = setupLocal({ canEdit: false });
    await flushPromises();
    await openDetail(wrapper, runners[0]);

    expect(wrapper.findAll('.runners-detail-actions button')).toHaveLength(0);
    expect(wrapper.findAllComponents({ name: 'RunnerActionsDropdown' })[0].props('canEdit')).toBe(false);
  });

  it('deletes a runner and reloads every open panel', async () => {
    vi.mocked(deleteAgencyRunner).mockResolvedValue();
    const wrapper = setupLocal();
    await flushPromises();
    await openDetail(wrapper, runners[0]);

    wrapper.findAllComponents({ name: 'RunnerActionsDropdown' })[0].vm.$emit('delete');
    await lastConfirm().onConfirm();
    await flushPromises();

    expect(deleteAgencyRunner).toHaveBeenCalledWith('req-1', 'run-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Runner deleted');
    expect(panelReload).toHaveBeenCalledOnce();
    expect(wrapper.emitted('updated')).toHaveLength(1);
  });

  it('forwards updates from the panel and the modals', async () => {
    const wrapper = setupLocal();
    await flushPromises();
    await openDetail(wrapper, runners[0]);

    wrapper.findComponent({ name: 'RunnerDetailPanel' }).vm.$emit('updated');
    wrapper.findComponent({ name: 'RunnerActionModals' }).vm.$emit('updated');
    await flushPromises();

    expect(wrapper.emitted('updated')).toHaveLength(2);
  });

  it('forwards the selection', async () => {
    const wrapper = setupLocal({ checkable: true });
    await flushPromises();
    grid(wrapper).vm.$emit('update:checkedRows', [runners[0]]);

    expect(wrapper.emitted('update:checkedRows')).toEqual([[[runners[0]]]]);
  });
});

describe('RunnersGrid with server rows', () => {
  it('fetches the runners and shows the filters', async () => {
    const { wrapper, fetch } = await setupServer();

    expect(fetch).toHaveBeenCalledWith(expect.objectContaining({ requestId: 'req-1', pageIndex: 1 }));
    expect(grid(wrapper).props('paginated')).toBe(true);
    expect(wrapper.find('thead input[placeholder="Search..."]').exists()).toBe(true);
  });

  it('searches by name', async () => {
    const { wrapper, fetch } = await setupServer();
    await wrapper.find('thead input[placeholder="Search..."]').setValue('ana');
    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    await flushPromises();

    expect(fetch).toHaveBeenLastCalledWith(expect.objectContaining({ name: 'ana' }));
  });

  it('searches again from the first page when the type changes', async () => {
    const { wrapper, fetch } = await setupServer();
    await wrapper.find('thead select').setValue(String(RunnerType.Passive));
    await flushPromises();

    expect(fetch.mock.calls.length).toBeGreaterThan(1);
    expect(fetch).toHaveBeenLastCalledWith(expect.objectContaining({ pageIndex: 1, type: RunnerType.Passive }));
  });

  it('shows the saved statuses and searches again when they change', async () => {
    const { wrapper, fetch } = await setupServer({ requestId: 'req-1', statuses: [RunnerStatus.Hired] });
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    expect(taginput.props('modelValue')).toEqual([{ id: RunnerStatus.Hired, value: 'Hired' }]);

    taginput.vm.$emit('update:modelValue', [{ id: RunnerStatus.Hired, value: 'Hired' }, { id: RunnerStatus.SentToClient, value: 'Sent' }]);
    await flushPromises();
    expect(fetch).toHaveBeenCalledTimes(2);
    expect(fetch).toHaveBeenLastCalledWith(expect.objectContaining({ statuses: [RunnerStatus.Hired, RunnerStatus.SentToClient] }));

    taginput.vm.$emit('update:modelValue', []);
    await flushPromises();
    expect(fetch).toHaveBeenCalledTimes(3);
    expect(fetch.mock.lastCall?.[0].statuses).toBeUndefined();
  });

  it('exposes reload and search', async () => {
    const { wrapper, fetch } = await setupServer();
    const exposed = wrapper.vm as unknown as { reload: () => void; search: () => void };
    exposed.reload();
    exposed.search();
    await flushPromises();

    expect(fetch).toHaveBeenCalledTimes(3);
  });
});
