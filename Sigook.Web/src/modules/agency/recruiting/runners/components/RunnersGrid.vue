<template>
  <div>
    <SigookGrid ref="grid" :fetch="fetch" :data="data" v-model:params="params" :sort-map="sortMap" :paginated="isServer"
      :fit-viewport="isServer" detailed detail-key="id" v-model:opened-detailed="openedRunners" :checkable="checkable"
      v-model:checked-rows="checkedRows" :empty-text="emptyText" @update:loading="(value) => emit('update:loading', value)"
      @cellclick="onCellClick">
      <template v-if="$slots.actions" #actions>
        <slot name="actions"></slot>
      </template>

      <b-table-column field="name" label="Runner" :sortable="isServer" :searchable="isServer">
        <template v-if="isServer" #searchable>
          <b-input :model-value="params?.name" placeholder="Search..." icon="magnify" size="is-small"
            @update:modelValue="(value: string) => patchParams({ name: value })" />
        </template>
        <template v-slot="props">
          <span class="is-block">{{ props.row.name }}</span>
          <i class="fz-2 ellipsis-150 is-lowercase">{{ props.row.email }}</i>
        </template>
      </b-table-column>

      <b-table-column field="type" label="Type" width="90" :sortable="isServer" :searchable="isServer">
        <template v-if="isServer" #searchable>
          <b-select :model-value="params?.type ?? null" size="is-small" expanded @update:modelValue="onTypeChange">
            <option :value="null">All</option>
            <option v-for="t in RUNNER_TYPES" :key="t" :value="t">{{ RUNNER_TYPE_LABELS[t] }}</option>
          </b-select>
        </template>
        <template v-slot="props">
          <b-tag>{{ RUNNER_TYPE_LABELS[props.row.type] }}</b-tag>
        </template>
      </b-table-column>

      <b-table-column field="status" label="Status" width="210" :sortable="isServer" :searchable="isServer">
        <template v-if="isServer" #searchable>
          <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
            field="value" icon="label" placeholder="Select Status" append-to-body />
        </template>
        <template v-slot="props">
          <b-tag :type="runnerStatusTagType(props.row.status)">{{ runnerStatusLabel(props.row.status) }}</b-tag>
        </template>
      </b-table-column>

      <b-table-column field="interviewsCount" label="Interviews" width="170" v-slot="props">
        {{ props.row.interviewsCount }}
        <span v-if="props.row.lastInterviewAt" class="fz-2 op7">
          · {{ dateMonth(props.row.lastInterviewAt) }} {{ dateHHmm(props.row.lastInterviewAt) }}
        </span>
      </b-table-column>

      <b-table-column field="startDate" label="Start date" width="110" v-slot="props">
        <span v-if="props.row.startDate">{{ dateMonth(props.row.startDate) }}</span>
        <span v-else class="op3">—</span>
      </b-table-column>

      <b-table-column field="createdAt" label="Added" width="160" :sortable="isServer" v-slot="props">
        <div v-if="props.row.createdByEmail" class="is-capitalized">{{ emailName(props.row.createdByEmail) }}</div>
        <i class="fz-2">{{ dateMonth(props.row.createdAt) }}</i>
      </b-table-column>

      <b-table-column field="actions" width="60" v-slot="props">
        <runner-actions-dropdown :status="props.row.status" :can-edit="canEdit"
          @open="(action: RunnerAction) => onAction(props.row, action)" @delete="confirmDelete(toTarget(props.row))" />
      </b-table-column>

      <template #detail="props">
        <RunnerDetailPanel :ref="(panel) => registerPanel(props.row.id, panel)" :request-id="props.row.requestId"
          :runner-id="props.row.id" :can-edit="canEdit" @updated="onUpdated" />
        <div class="runners-detail-actions">
          <b-button v-if="canEdit" type="is-danger" outlined size="is-small" @click="confirmDelete(toTarget(props.row))">
            Delete runner
          </b-button>
          <b-button v-if="canEdit && props.row.status !== RunnerStatus.Hired" type="is-primary" size="is-small"
            @click="open(toTarget(props.row), 'status')">
            Change status
          </b-button>
        </div>
      </template>
    </SigookGrid>

    <runner-action-modals :target="target" v-model:status-open="showStatus" v-model:interview-open="showInterview"
      v-model:history-open="showHistory" @updated="onUpdated" />
  </div>
</template>

<script setup lang="ts" generic="T extends RunnerRow">
import { computed, ref, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { dateMonth, dateHHmm, emailName } from '@/shared/format';
import { useRunnerActions } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';
import type { RunnerAction, RunnerActionTarget } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';
import {
  RUNNER_STATUSES,
  RUNNER_STATUS_LABELS,
  RUNNER_TYPES,
  RUNNER_TYPE_LABELS,
  RunnerStatus,
  runnerStatusLabel,
  runnerStatusTagType,
} from '@/modules/agency/recruiting/requests/runners/types';
import type { AgencyRunnerFilter, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import type { RunnerRow } from '@/modules/agency/recruiting/runners/types';
import type { CatalogItem, GridHandle, PaginatedList, TableColumnRef } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import RunnerActionsDropdown from '@/modules/agency/recruiting/requests/runners/RunnerActionsDropdown.vue';
import RunnerActionModals from '@/modules/agency/recruiting/requests/runners/RunnerActionModals.vue';
import RunnerDetailPanel from '@/modules/agency/recruiting/runners/components/RunnerDetailPanel.vue';

interface DetailPanelHandle {
  reload: () => void;
}

const props = withDefaults(defineProps<{
  fetch?: (params: AgencyRunnerFilter) => Promise<PaginatedList<T>>;
  data?: T[];
  sortMap?: Record<string, number>;
  checkable?: boolean;
  canEdit?: boolean;
  emptyText?: string;
}>(), {
  canEdit: true,
  emptyText: 'No runners yet',
});

const params = defineModel<AgencyRunnerFilter>('params');
const checkedRows = defineModel<T[]>('checkedRows', { default: () => [] });

const emit = defineEmits<{
  (e: 'update:loading', value: boolean): void;
  (e: 'updated'): void;
}>();

const router = useRouter();
const grid = useTemplateRef<GridHandle>('grid');
const openedRunners = ref<string[]>([]);
const panels = new Map<string, DetailPanelHandle>();

const isServer = computed(() => !!props.fetch);
const statusOptions: CatalogItem<RunnerStatus>[] = RUNNER_STATUSES.map((s) => ({ id: s, value: RUNNER_STATUS_LABELS[s] }));

const statusesSelected = computed<CatalogItem<RunnerStatus>[]>({
  get: () => statusOptions.filter((option) => params.value?.statuses?.includes(option.id)),
  set: (options) => {
    patchParams({ statuses: options.length ? options.map((option) => option.id) : undefined });
    grid.value?.search();
  },
});

const { target, showStatus, showInterview, showHistory, open, confirmDelete } = useRunnerActions(onUpdated);

function patchParams(changes: Partial<AgencyRunnerFilter>) {
  params.value = { ...params.value, ...changes };
}

function onTypeChange(type: RunnerType | null) {
  patchParams({ type });
  grid.value?.search();
}

function toTarget(row: T): RunnerActionTarget {
  return { requestId: row.requestId, runnerId: row.id, name: row.name, status: row.status };
}

// The detail row already shows the history, so that menu entry just opens it.
function onAction(row: T, action: RunnerAction) {
  if (action === 'history') {
    if (!openedRunners.value.includes(row.id)) openedRunners.value = [...openedRunners.value, row.id];
    return;
  }
  open(toTarget(row), action);
}

function onCellClick(row: T, column: TableColumnRef) {
  if (column.field === 'actions') return;
  if (column.field === 'name') {
    router.push(`/recruiting/workers/${row.workerProfileId}`);
    return;
  }
  openedRunners.value = openedRunners.value.includes(row.id)
    ? openedRunners.value.filter((id) => id !== row.id)
    : [...openedRunners.value, row.id];
}

function registerPanel(id: string, panel: unknown) {
  if (panel) panels.set(id, panel as DetailPanelHandle);
  else panels.delete(id);
}

// A change made from a modal must show in the open detail rows too, so every
// mounted panel reloads besides the list that owns the grid.
function onUpdated() {
  panels.forEach((panel) => panel.reload());
  grid.value?.reload();
  emit('updated');
}

defineExpose({
  reload: () => grid.value?.reload(),
  search: () => grid.value?.search(),
});
</script>

<style scoped lang="scss">
.runners-detail-actions {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 8px;
  padding-top: 12px;
}
</style>
