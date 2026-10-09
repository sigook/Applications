<template>
  <div>
    <b-loading v-model="isLoading" />
    <b-message v-if="!canEdit" type="is-warning" size="is-small" has-icon>
      This order does not use runners. The list is read-only.
    </b-message>

    <SigookGrid ref="grid" :fetch="loadRunners" v-model:params="serverParams" :sort-map="sortMap"
      empty-text="No runners yet" @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
      <template v-if="canEdit" #actions>
        <b-button icon-left="plus" @click="showCreate = true">Add Runner</b-button>
      </template>

      <b-table-column field="name" label="Name" sortable searchable>
        <template #searchable>
          <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small" />
        </template>
        <template v-slot="props">
          <span class="is-block">
            {{ props.row.name }}
          </span>
          <i class="fz-2 ellipsis-150 is-lowercase">{{ props.row.email }}</i>
        </template>
      </b-table-column>

      <b-table-column field="type" label="Type" sortable searchable>
        <template #searchable>
          <b-select v-model="serverParams.type" size="is-small" expanded @update:modelValue="onTypeChange">
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
    </SigookGrid>

    <b-modal has-modal-card v-model="showCreate" width="640px">
      <create-runner :request-id="requestId" :is-saving="isCreating" @create="onCreate" @close="showCreate = false" />
    </b-modal>

    <runner-action-modals :target="target" v-model:status-open="showStatus" v-model:interview-open="showInterview"
      v-model:history-open="showHistory" @updated="reloadRunners" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { dateMonth } from '@/shared/format';
import { getAgencyRunners, createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { useRunnerActions } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';
import type { RunnerActionTarget } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';
import {
  RUNNER_STATUSES,
  RUNNER_STATUS_LABELS,
  RUNNER_TYPES,
  RUNNER_TYPE_LABELS,
  RunnerSortBy,
  RunnerStatus,
  runnerStatusLabel,
  runnerStatusTagType,
} from '@/modules/agency/recruiting/requests/runners/types';
import type { AgencyRunnerFilter, CreateRunnerModel, RunnerListItem, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import type { CatalogItem, GridHandle, TableColumnRef } from '@/shared/types/common';
import type { AgencyRequestDetail } from '@/modules/agency/recruiting/requests/types';
import CreateRunner from '@/modules/agency/recruiting/requests/runners/CreateRunner.vue';
import RunnerActionsDropdown from '@/modules/agency/recruiting/requests/runners/RunnerActionsDropdown.vue';
import RunnerActionModals from '@/modules/agency/recruiting/requests/runners/RunnerActionModals.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ request?: AgencyRequestDetail | null }>();

const route = useRoute();
const requestId = route.params.id as string;
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  name: RunnerSortBy.Name,
  status: RunnerSortBy.Status,
  type: RunnerSortBy.Type,
  createdAt: RunnerSortBy.CreatedAt,
};

const canEdit = computed(() => !!props.request?.usesRunners);

const runnerTypes = RUNNER_TYPES;
const statusOptions: CatalogItem<RunnerStatus>[] = RUNNER_STATUSES.map(s => ({ id: s, value: RUNNER_STATUS_LABELS[s] }));

const isLoading = ref(false);
const showCreate = ref(false);
const isCreating = ref(false);
const statusesSelected = ref<CatalogItem<RunnerStatus>[]>([]);

const serverParams = ref<AgencyRunnerFilter>({
  requestId,
  isDescending: true,
  sortBy: RunnerSortBy.CreatedAt,
});

const { target, showStatus, showInterview, showHistory, open, confirmDelete } = useRunnerActions(reloadRunners);

const statusLabel = runnerStatusLabel;

function typeLabel(type: RunnerType): string {
  return RUNNER_TYPE_LABELS[type];
}

const statusType = runnerStatusTagType;

function onStatusChange() {
  serverParams.value.statuses = statusesSelected.value.length ? statusesSelected.value.map(s => s.id) : undefined;
  grid.value?.search();
}

function onTypeChange() {
  grid.value?.search();
}

function onCellClick(row: RunnerListItem, column: TableColumnRef) {
  if (column.field !== 'actions') open(toTarget(row), 'history');
}

function toTarget(row: RunnerListItem): RunnerActionTarget {
  return { requestId, runnerId: row.id, name: row.name, status: row.status };
}

function loadRunners(params: AgencyRunnerFilter) {
  return getAgencyRunners(requestId, params);
}

function reloadRunners() {
  grid.value?.reload();
}

function onCreate(model: CreateRunnerModel) {
  isCreating.value = true;
  createAgencyRunner(requestId, model)
    .then(() => {
      showCreate.value = false;
      reloadRunners();
    })
    .catch(err => showAlertError(err))
    .finally(() => {
      isCreating.value = false;
    });
}
</script>
