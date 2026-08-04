<template>
  <div>
    <b-loading v-model="isLoading" />
    <b-message v-if="!canEdit" type="is-warning" size="is-small" has-icon>
      This order does not use runners. The list is read-only.
    </b-message>
    <b-field v-if="canEdit" grouped position="is-right">
      <b-button type="is-primary" icon-left="plus" @click="showCreate = true">Add Runner</b-button>
    </b-field>

    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated
      pagination-size="is-small" backend-pagination backend-sorting pagination-rounded :total="totalItems"
      :per-page="serverParams.pageSize" v-model:current-page="serverParams.pageIndex" default-sort="createdAt"
      @page-change="onPageChange" @sort="onSortChange" @cellclick="onCellClick">
      <template #empty>
        <p class="container text-center">No runners yet</p>
      </template>

      <b-table-column field="name" label="Name" sortable searchable>
        <template #searchable>
          <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
            @keypress="onInputEntered" />
        </template>
        <template v-slot="props">
          <span class="d-block">
            {{ props.row.name }}
          </span>
          <i class="fz-2 ellipsis-150 text-lowercase">{{ props.row.email }}</i>
        </template>
      </b-table-column>

      <b-table-column field="type" label="Type" sortable searchable>
        <template #searchable>
          <b-select v-model="serverParams.type" size="is-small" expanded @update:modelValue="loadRunners">
            <option :value="null">All</option>
            <option v-for="t in runnerTypes" :key="t" :value="t">{{ typeLabel(t) }}</option>
          </b-select>
        </template>
        <template v-slot="props">
          <b-tag>{{ typeLabel(props.row.type) }}</b-tag>
        </template>
      </b-table-column>

      <b-table-column field="status" label="Status" searchable>
        <template #searchable>
          <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
            field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange" append-to-body />
        </template>
        <template v-slot="props">
          <b-tag :type="statusType(props.row.status)">{{ statusLabel(props.row.status) }}</b-tag>
        </template>
      </b-table-column>

      <b-table-column field="interviewsCount" label="Interviews" centered v-slot="props">
        {{ props.row.interviewsCount }}
      </b-table-column>

      <b-table-column field="startDate" label="Start Date" v-slot="props">
        <span v-if="props.row.startDate">{{ dateMonth(props.row.startDate) }}</span>
      </b-table-column>

      <b-table-column field="createdAt" label="Created" sortable v-slot="props">
        <i class="fz-2">{{ dateMonth(props.row.createdAt) }}</i>
      </b-table-column>

      <b-table-column field="actions" v-slot="props">
        <runner-actions-dropdown :status="props.row.status" :can-edit="canEdit"
          @open="action => open(toTarget(props.row), action)" @delete="confirmDelete(toTarget(props.row))" />
      </b-table-column>
    </b-table>

    <b-modal v-model="showCreate" width="640px">
      <create-runner :request-id="requestId" :is-saving="isCreating" @create="onCreate" @close="showCreate = false" />
    </b-modal>

    <runner-action-modals :target="target" v-model:status-open="showStatus" v-model:interview-open="showInterview"
      v-model:history-open="showHistory" @updated="loadRunners" />
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { dateMonth } from '@/utils/filters';
import { getAgencyRunners, createAgencyRunner } from '@/api/agencyRunnerApi';
import { useRunnerActions } from '@/composables/useRunnerActions';
import type { RunnerActionTarget } from '@/composables/useRunnerActions';
import {
  RUNNER_STATUSES,
  RUNNER_STATUS_LABELS,
  RUNNER_TYPES,
  RUNNER_TYPE_LABELS,
  RunnerSortBy,
  RunnerStatus,
  runnerStatusLabel,
  runnerStatusTagType,
} from '@/types/runner';
import type { AgencyRunnerFilter, CreateRunnerModel, RunnerListItem, RunnerType } from '@/types/runner';
import type { CatalogItem } from '@/types/common';
import type { AgencyRequestDetail } from '@/types/agency';
import CreateRunner from '@/components/runner/CreateRunner.vue';
import RunnerActionsDropdown from '@/components/runner/RunnerActionsDropdown.vue';
import RunnerActionModals from '@/components/runner/RunnerActionModals.vue';

const props = defineProps<{ request?: AgencyRequestDetail | null }>();

const route = useRoute();
const requestId = route.params.id as string;

const canEdit = computed(() => !!props.request?.usesRunners);

const runnerTypes = RUNNER_TYPES;
const statusOptions: CatalogItem<RunnerStatus>[] = RUNNER_STATUSES.map(s => ({ id: s, value: RUNNER_STATUS_LABELS[s] }));

const isLoading = ref(false);
const rows = ref<RunnerListItem[]>([]);
const totalItems = ref(0);
const showCreate = ref(false);
const isCreating = ref(false);
const statusesSelected = ref<CatalogItem<RunnerStatus>[]>([]);

const serverParams = reactive<AgencyRunnerFilter>({
  requestId,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true,
  sortBy: RunnerSortBy.CreatedAt,
});

const { target, showStatus, showInterview, showHistory, open, confirmDelete } = useRunnerActions(loadRunners);

const statusLabel = runnerStatusLabel;

function typeLabel(type: RunnerType): string {
  return RUNNER_TYPE_LABELS[type];
}

const statusType = runnerStatusTagType;

function onPageChange(page: number) {
  serverParams.pageIndex = page;
  loadRunners();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'name':
      serverParams.sortBy = RunnerSortBy.Name;
      break;
    case 'status':
      serverParams.sortBy = RunnerSortBy.Status;
      break;
    case 'type':
      serverParams.sortBy = RunnerSortBy.Type;
      break;
    default:
      serverParams.sortBy = RunnerSortBy.CreatedAt;
  }
  serverParams.isDescending = order !== 'asc';
  loadRunners();
}

function onStatusChange() {
  serverParams.statuses = statusesSelected.value.length ? statusesSelected.value.map(s => s.id) : undefined;
  loadRunners();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') loadRunners();
}

function onCellClick(row: RunnerListItem, column: { field: string }) {
  if (column.field !== 'actions') open(toTarget(row), 'history');
}

function toTarget(row: RunnerListItem): RunnerActionTarget {
  return { requestId, runnerId: row.id, name: row.name, status: row.status };
}

function loadRunners() {
  isLoading.value = true;
  getAgencyRunners(requestId, serverParams)
    .then(response => {
      rows.value = response.items;
      totalItems.value = response.totalItems;
    })
    .catch(err => showAlertError(err))
    .finally(() => {
      isLoading.value = false;
    });
}

function onCreate(model: CreateRunnerModel) {
  isCreating.value = true;
  createAgencyRunner(requestId, model)
    .then(() => {
      showCreate.value = false;
      loadRunners();
    })
    .catch(err => showAlertError(err))
    .finally(() => {
      isCreating.value = false;
    });
}

loadRunners();
</script>
