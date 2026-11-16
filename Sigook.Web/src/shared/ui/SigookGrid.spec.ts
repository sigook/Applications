import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PaginatedList } from '@/shared/types/common';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface Row {
  id: number;
  name: string;
}

const COLUMNS = `
  <b-table-column field="name" label="Name" sortable v-slot="props">
    <span class="cell-name">{{ props.row.name }}</span>
  </b-table-column>`;

function rowsOf(count: number): Row[] {
  return Array.from({ length: count }, (_, i) => ({ id: i + 1, name: `Worker ${i + 1}` }));
}

function page(items: Row[], totalItems = items.length): PaginatedList<Row> {
  return { items, totalItems, pageIndex: 1, totalPages: 1 };
}

function setup(props: Record<string, unknown> = {}, slots: Record<string, string> = {}) {
  return mountWithPlugins(SigookGrid, {
    props: props,
    slots: { default: COLUMNS, ...slots },
  });
}

function names(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.cell-name').map((c) => c.text());
}

beforeEach(() => {
  touch.value = false;
});

describe('SigookGrid with local data', () => {
  it('renders the rows', async () => {
    const wrapper = setup({ data: rowsOf(2) });
    await flushPromises();
    expect(names(wrapper)).toEqual(['Worker 1', 'Worker 2']);
  });

  it('shows the empty text', () => {
    const wrapper = setup({ data: [], emptyText: 'No workers' });
    expect(wrapper.text()).toContain('No workers');
  });

  it('paginates 30 rows per page', async () => {
    const wrapper = setup({ data: rowsOf(35) });
    await flushPromises();
    expect(names(wrapper)).toHaveLength(30);
  });

  it('hides the toolbar when there is nothing to show', () => {
    expect(setup({ data: [] }).find('.sigook-grid-toolbar').exists()).toBe(false);
  });

  it('calls refresh from the toolbar', async () => {
    const refresh = vi.fn();
    const wrapper = setup({ data: [], refresh });
    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    expect(refresh).toHaveBeenCalledOnce();
  });

  it('forwards row clicks', async () => {
    const wrapper = setup({ data: rowsOf(1) });
    await flushPromises();
    await wrapper.find('tbody tr td').trigger('click');
    expect(wrapper.emitted('click')?.[0]).toEqual([{ id: 1, name: 'Worker 1' }]);
    expect(wrapper.emitted('cellclick')?.[0][0]).toEqual({ id: 1, name: 'Worker 1' });
  });
});

describe('SigookGrid with server data', () => {
  it('fetches the first page on mount and reports loading', async () => {
    const fetch = vi.fn().mockResolvedValue(page(rowsOf(2), 42));
    const wrapper = setup({ fetch });
    await flushPromises();

    expect(fetch).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 30 });
    expect(names(wrapper)).toEqual(['Worker 1', 'Worker 2']);
    expect(wrapper.emitted('update:loading')).toEqual([[true], [false]]);
    expect(wrapper.emitted('loaded')).toEqual([[42]]);
    expect(wrapper.emitted('update:params')?.[0]).toEqual([{ pageIndex: 1, pageSize: 30 }]);
  });

  it('keeps the filters from params', async () => {
    const fetch = vi.fn().mockResolvedValue(page([]));
    setup({ fetch, params: { pageIndex: 2, pageSize: 60, name: 'ana' } });
    await flushPromises();

    expect(fetch).toHaveBeenCalledWith({ pageIndex: 2, pageSize: 60, name: 'ana' });
  });

  it('shows an error when the fetch fails', async () => {
    const error = new Error('boom');
    const wrapper = setup({ fetch: vi.fn().mockRejectedValue(error) });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('update:loading')?.at(-1)).toEqual([false]);
  });

  it('reloads from the refresh button', async () => {
    const fetch = vi.fn().mockResolvedValue(page([]));
    const wrapper = setup({ fetch });
    await flushPromises();

    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    await flushPromises();
    expect(fetch).toHaveBeenCalledTimes(2);
  });

  it('search goes back to the first page', async () => {
    const fetch = vi.fn().mockResolvedValue(page([]));
    const wrapper = setup({ fetch, params: { pageIndex: 3, pageSize: 30 } });
    await flushPromises();

    (wrapper.vm as unknown as { search: () => void }).search();
    await flushPromises();
    expect(fetch).toHaveBeenLastCalledWith({ pageIndex: 1, pageSize: 30 });
  });

  it('maps the sorted column to the backend sort id', async () => {
    const fetch = vi.fn().mockResolvedValue(page(rowsOf(2)));
    const wrapper = setup({ fetch, sortMap: { name: 4 } });
    await flushPromises();

    wrapper.findComponent({ name: 'BTable' }).vm.$emit('sort', 'name', 'desc');
    await flushPromises();
    expect(fetch).toHaveBeenLastCalledWith({ pageIndex: 1, pageSize: 30, sortBy: 4, isDescending: true });
  });

  it('restores the default sort from params', async () => {
    const fetch = vi.fn().mockResolvedValue(page([]));
    const wrapper = setup({ fetch, sortMap: { name: 4 }, params: { sortBy: 4, isDescending: false } });
    await flushPromises();

    expect(wrapper.findComponent({ name: 'BTable' }).props('defaultSort')).toEqual(['name', 'asc']);
  });

  it('reloads the first page when the page size changes', async () => {
    const fetch = vi.fn().mockResolvedValue(page(rowsOf(2), 100));
    const wrapper = setup({ fetch, params: { pageIndex: 2, pageSize: 30 } });
    await flushPromises();

    await wrapper.find('select').setValue('60');
    await flushPromises();
    expect(fetch).toHaveBeenLastCalledWith({ pageIndex: 1, pageSize: 60 });
  });

  it('reloads when the page changes', async () => {
    const fetch = vi.fn().mockResolvedValue(page(rowsOf(2), 100));
    const wrapper = setup({ fetch });
    await flushPromises();

    wrapper.findComponent({ name: 'BTable' }).vm.$emit('page-change', 3);
    await flushPromises();
    expect(fetch).toHaveBeenLastCalledWith({ pageIndex: 3, pageSize: 30 });
  });

  it('ignores sorting for local data', async () => {
    const wrapper = setup({ data: rowsOf(2), sortMap: { name: 4 } });
    wrapper.findComponent({ name: 'BTable' }).vm.$emit('sort', 'name', 'desc');
    await flushPromises();
    expect(wrapper.emitted('update:loading')).toBeUndefined();
  });
});

describe('SigookGrid export', () => {
  function exportItem(): HTMLElement {
    const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.includes('Export'));
    if (!item) throw new Error('Export item not found');
    return item;
  }

  it('downloads the report with the current params', async () => {
    const blob = new Blob(['x']);
    vi.mocked(downloadAgencyReport).mockResolvedValue(blob);
    const fetch = vi.fn().mockResolvedValue(page([]));
    setup({ fetch, params: { pageIndex: 1, pageSize: 30, name: 'ana' }, export: { url: 'api/report', fileName: 'Workers' } });
    await flushPromises();

    exportItem().click();
    await flushPromises();

    expect(downloadAgencyReport).toHaveBeenCalledWith('api/report', { pageIndex: 1, pageSize: 30, name: 'ana' });
    expect(vi.mocked(downloadFile).mock.calls[0][0]).toBe(blob);
    expect(vi.mocked(downloadFile).mock.calls[0][1]).toMatch(/^Workers_/);
  });

  it('shows an error when the export fails', async () => {
    const error = new Error('nope');
    vi.mocked(downloadAgencyReport).mockRejectedValue(error);
    const wrapper = setup({ data: [], export: { url: 'api/report', fileName: 'Workers' } });

    exportItem().click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('update:loading')).toEqual([[true], [false]]);
  });
});

describe('SigookGrid on touch screens', () => {
  const mobileSlot = { 'mobile-card': '<template #mobile-card="{ row }"><div class="card-row">{{ row.name }}</div></template>' };

  it('renders cards instead of the table', () => {
    touch.value = true;
    const wrapper = setup({ data: rowsOf(2) }, mobileSlot);

    expect(wrapper.find('table').exists()).toBe(false);
    expect(wrapper.findAll('.card-row').map((c) => c.text())).toEqual(['Worker 1', 'Worker 2']);
  });

  it('paginates local cards', () => {
    touch.value = true;
    const wrapper = setup({ data: rowsOf(35) }, mobileSlot);
    expect(wrapper.findAll('.card-row')).toHaveLength(30);
    expect(wrapper.find('.pagination').exists()).toBe(true);
  });

  it('shows the empty text without rows', () => {
    touch.value = true;
    const wrapper = setup({ data: [], emptyText: 'Nothing here' }, mobileSlot);
    expect(wrapper.text()).toContain('Nothing here');
  });

  it('keeps the table when there is no card template', () => {
    touch.value = true;
    expect(setup({ data: rowsOf(1) }).find('table').exists()).toBe(true);
  });
});

