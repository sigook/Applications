<template>
  <div class="sigook-grid" @keyup.enter="onFilterEnter">
    <div v-if="showToolbar" class="sigook-grid-toolbar">
      <b-button v-if="canRefresh" icon-left="refresh" title="Refresh" aria-label="Refresh" @click="reload()" />
      <slot name="filters"></slot>
      <div class="sigook-grid-actions">
        <slot name="actions"></slot>
        <b-dropdown v-if="hasDropdown" aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="chevron-down" icon-left="dots-vertical">Actions</b-button>
          </template>
          <slot name="dropdown-actions"></slot>
          <b-dropdown-item v-if="props.export" aria-role="listitem" @click="onExport">
            <b-icon icon="file-excel"></b-icon>
            <span>Export</span>
          </b-dropdown-item>
        </b-dropdown>
      </div>
    </div>

    <template v-if="showMobileCards">
      <div v-if="tableData.length > 0" class="sigook-grid-cards">
        <div v-for="(row, index) in mobileRows" :key="index">
          <slot name="mobile-card" :row="row"></slot>
        </div>
      </div>
      <p v-else class="container has-text-centered">{{ emptyText }}</p>
      <b-pagination v-if="paginated && totalRows > pageSize" :model-value="pageIndex" :total="totalRows"
        :per-page="pageSize" size="is-small" rounded @update:model-value="onPageChange" />
    </template>

    <b-table v-else v-bind="$attrs" :data="tableData" narrowed hoverable :mobile-cards="false"
      :sticky-header="fitViewport" :height="fitViewport ? 'var(--grid-height)' : undefined"
      :paginated="paginated" pagination-size="is-small" pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex" :backend-pagination="isServer" :backend-sorting="isServer"
      :total="isServer ? totalRows : undefined" :default-sort="tableDefaultSort" :focusable="focusable"
      :row-class="rowClass" :detailed="detailed" :detail-key="detailKey" detail-transition="fade"
      :show-detail-icon="showDetailIcon" :has-detailed-visible="hasDetailedVisible"
      v-model:opened-detailed="openedDetailed" :checkable="checkable" :checkbox-position="checkboxPosition"
      :is-row-checkable="isRowCheckable" v-model:checked-rows="checkedRows"
      @page-change="onPageChange" @sort="onSort" @cellclick="onCellClick" @click="onRowClick">
      <template #empty>
        <slot name="empty">
          <p class="container has-text-centered">{{ emptyText }}</p>
        </slot>
      </template>
      <template v-if="paginated" #bottom-left>
        <b-select :model-value="pageSize" size="is-small" @update:model-value="onPageSizeChange">
          <option v-for="size in PAGE_SIZE_OPTIONS" :key="size" :value="size">{{ size }} / page</option>
        </b-select>
      </template>
      <template v-if="$slots.detail" #detail="slotProps">
        <slot name="detail" v-bind="slotProps"></slot>
      </template>
      <template v-if="$slots.footer" #footer>
        <slot name="footer"></slot>
      </template>
      <slot></slot>
    </b-table>
  </div>
</template>

<script setup lang="ts" generic="T, P extends GridParams">
import { computed, onMounted, ref, shallowRef, useSlots } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import { downloadFile } from '@/shared/utils/downloadFile';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { useBreakpoint } from '@/shared/composables/useBreakpoint';
import type { GridExport, GridParams, PaginatedList, TableColumnRef } from '@/shared/types/common';

defineOptions({ inheritAttrs: false });

const PAGE_SIZE_OPTIONS = [30, 60, 90];
const DEFAULT_PAGE_SIZE = 30;

const props = withDefaults(defineProps<{
  fetch?: (params: P) => Promise<PaginatedList<T>>;
  data?: T[];
  refresh?: () => unknown;
  sortMap?: Record<string, number>;
  export?: GridExport;
  paginated?: boolean;
  emptyText?: string;
  fitViewport?: boolean;
  focusable?: boolean;
  defaultSort?: string | [string, string];
  rowClass?: (row: T, index: number) => string;
  detailed?: boolean;
  detailKey?: string;
  showDetailIcon?: boolean;
  hasDetailedVisible?: (row: T) => boolean;
  checkable?: boolean;
  checkboxPosition?: 'left' | 'right';
  isRowCheckable?: (row: T) => boolean;
}>(), {
  paginated: true,
  emptyText: 'No records available',
  fitViewport: true,
  showDetailIcon: true,
  hasDetailedVisible: () => true,
  checkboxPosition: 'left',
  isRowCheckable: () => true,
  rowClass: () => '',
});

const params = defineModel<P>('params');
const checkedRows = defineModel<T[]>('checkedRows', { default: () => [] });
const openedDetailed = defineModel<unknown[]>('openedDetailed', { default: () => [] });

const emit = defineEmits<{
  (e: 'update:loading', value: boolean): void;
  (e: 'loaded', total: number): void;
  (e: 'cellclick', row: T, column: TableColumnRef): void;
  (e: 'click', row: T): void;
}>();

const slots = useSlots();
const { isTouch } = useBreakpoint();

const rows = shallowRef<T[]>([]);
const serverTotal = ref(0);
const pageIndex = ref(params.value?.pageIndex ?? 1);
const pageSize = ref(params.value?.pageSize ?? DEFAULT_PAGE_SIZE);

const isServer = computed(() => !!props.fetch);
const canRefresh = computed(() => isServer.value || !!props.refresh);
const hasDropdown = computed(() => !!props.export || !!slots['dropdown-actions']);
const showToolbar = computed(() => canRefresh.value || hasDropdown.value || !!slots.actions || !!slots.filters);
const showMobileCards = computed(() => isTouch.value && !!slots['mobile-card']);
const tableData = computed<T[]>(() => (isServer.value ? rows.value : props.data ?? []));
const totalRows = computed(() => (isServer.value ? serverTotal.value : tableData.value.length));

const mobileRows = computed<T[]>(() => {
  if (isServer.value || !props.paginated) return tableData.value;
  const start = (pageIndex.value - 1) * pageSize.value;
  return tableData.value.slice(start, start + pageSize.value);
});

const tableDefaultSort = computed<string | [string, string] | undefined>(() => {
  if (!isServer.value || !props.sortMap) return props.defaultSort;
  const map = props.sortMap;
  const field = Object.keys(map).find((key) => map[key] === params.value?.sortBy);
  return field ? [field, params.value?.isDescending ? 'desc' : 'asc'] : undefined;
});

function patchParams(patch: GridParams): P {
  if (patch.pageIndex !== undefined) pageIndex.value = patch.pageIndex;
  if (patch.pageSize !== undefined) pageSize.value = patch.pageSize;
  const next = { ...params.value, pageIndex: pageIndex.value, pageSize: pageSize.value, ...patch } as P;
  params.value = next;
  return next;
}

function reload(patch: GridParams = {}) {
  if (!props.fetch) {
    props.refresh?.();
    return;
  }
  const next = patchParams(patch);
  emit('update:loading', true);
  props.fetch(next)
    .then((response) => {
      rows.value = response.items;
      serverTotal.value = response.totalItems;
      checkedRows.value = [];
      emit('loaded', response.totalItems);
    })
    .catch((error) => showAlertError(error))
    .finally(() => emit('update:loading', false));
}

function onPageChange(page: number) {
  pageIndex.value = page;
  if (isServer.value) reload();
}

function onPageSizeChange(size: number) {
  pageSize.value = size;
  pageIndex.value = 1;
  if (isServer.value) reload();
}

function search() {
  reload({ pageIndex: 1 });
}

function onSort(field: string, order: string) {
  if (!isServer.value) return;
  const sortBy = props.sortMap?.[field];
  reload({ ...(sortBy !== undefined ? { sortBy } : {}), isDescending: order !== 'asc' });
}

function onFilterEnter(event: KeyboardEvent) {
  if (!isServer.value) return;
  if (!(event.target instanceof HTMLElement) || !event.target.closest('th')) return;
  if (event.target.closest('.taginput, .datepicker')) return;
  reload({ pageIndex: 1 });
}

function onCellClick(row: T, column: TableColumnRef) {
  emit('cellclick', row, column);
}

function onRowClick(row: T) {
  emit('click', row);
}

function onExport() {
  if (!props.export) return;
  const { url, fileName } = props.export;
  emit('update:loading', true);
  downloadAgencyReport(url, { ...params.value })
    .then((file) => downloadFile(file, `${fileName}_${new Date().toLocaleDateString()}`))
    .catch((error) => showAlertError(error))
    .finally(() => emit('update:loading', false));
}

onMounted(() => {
  if (isServer.value) reload();
});

defineExpose({ reload, search });
</script>
