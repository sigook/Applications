<template>
  <div>
    <b-loading v-model="isLoading" />
    <b-field grouped position="is-right">
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
            <b-tooltip label="Candidate" type="is-dark" append-to-body>
              <b-icon v-if="props.row.candidateId" icon="account-hard-hat-outline" size="is-small" />
            </b-tooltip>
            <b-tooltip label="Worker" type="is-dark" append-to-body>
              <b-icon v-if="props.row.workerProfileId" icon="badge-account-outline" size="is-small" />
            </b-tooltip>
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
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item v-if="props.row.status !== RunnerStatus.Hired" aria-role="listitem"
            @click="openAction(props.row, 'status')">
            Change status
          </b-dropdown-item>
          <b-dropdown-item v-if="canAddInterview(props.row.status)" aria-role="listitem"
            @click="openAction(props.row, 'interview')">
            Add interview
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="openAction(props.row, 'history')">
            View history
          </b-dropdown-item>
          <b-dropdown-item v-if="props.row.candidateId" aria-role="listitem"
            @click="showCandidateDetail(props.row.candidateId)">
            Edit Candidate
          </b-dropdown-item>
          <b-dropdown-item v-if="props.row.candidateId && !props.row.workerProfileId" aria-role="listitem"
            @click="convertToWorker(props.row)">
            Convert to Worker
          </b-dropdown-item>
        </b-dropdown>
      </b-table-column>
    </b-table>

    <b-modal v-model="showCreate" width="640px">
      <create-runner :request-id="requestId" @create="onCreate" @close="showCreate = false" />
    </b-modal>

    <b-modal v-model="showStatus" width="520px">
      <runner-status-modal v-if="selectedRunner" :request-id="requestId" :runner-id="selectedRunner.id"
        :current-status="selectedRunner.status" :is-candidate="!!selectedRunner.candidateId" @updated="loadRunners"
        @close="showStatus = false" />
    </b-modal>

    <b-modal v-model="showInterview" width="540px">
      <runner-interview-modal v-if="selectedRunner" :request-id="requestId" :runner-id="selectedRunner.id"
        @updated="loadRunners" @close="showInterview = false" />
    </b-modal>

    <b-modal v-model="showHistory" width="760px">
      <runner-history-modal v-if="selectedRunner" :request-id="requestId" :runner-id="selectedRunner.id"
        @updated="loadRunners" @close="showHistory = false" />
    </b-modal>

    <b-modal v-model="showCandidateDetailModal" width="500px">
      <detail-candidate v-if="candidateDetailId" :candidate-id="candidateDetailId"
        @onUpdateWorker="onCandidateUpdated" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { dateMonth } from '@/utils/filters';
import { getAgencyRunners, createAgencyRunner } from '@/api/agencyRunnerApi';
import { convertCandidateToWorker } from '@/api/agencyCandidateApi';
import {
  RUNNER_STATUSES,
  RUNNER_STATUS_LABELS,
  RUNNER_TYPES,
  RUNNER_TYPE_LABELS,
  RunnerSortBy,
  RunnerStatus,
  canAddInterview,
} from '@/types/runner';
import type { AgencyRunnerFilter, CreateRunnerModel, RunnerListItem, RunnerType } from '@/types/runner';
import type { CatalogItem } from '@/types/common';
import CreateRunner from '@/components/runner/CreateRunner.vue';
import RunnerStatusModal from '@/components/runner/RunnerStatusModal.vue';
import RunnerInterviewModal from '@/components/runner/RunnerInterviewModal.vue';
import RunnerHistoryModal from '@/components/runner/RunnerHistoryModal.vue';
import DetailCandidate from '@/components/candidate/DetailCandidate.vue';

const route = useRoute();
const requestId = route.params.id as string;

const runnerTypes = RUNNER_TYPES;
const statusOptions: CatalogItem<RunnerStatus>[] = RUNNER_STATUSES.map(s => ({ id: s, value: RUNNER_STATUS_LABELS[s] }));

const isLoading = ref(false);
const rows = ref<RunnerListItem[]>([]);
const totalItems = ref(0);
const showCreate = ref(false);
const showStatus = ref(false);
const showInterview = ref(false);
const showHistory = ref(false);
const showCandidateDetailModal = ref(false);
const candidateDetailId = ref<string | null>(null);
const selectedRunner = ref<RunnerListItem | null>(null);
const statusesSelected = ref<CatalogItem<RunnerStatus>[]>([]);

const serverParams = reactive<AgencyRunnerFilter>({
  requestId,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true,
  sortBy: RunnerSortBy.CreatedAt,
});

function statusLabel(status: RunnerStatus): string {
  return RUNNER_STATUS_LABELS[status];
}

function typeLabel(type: RunnerType): string {
  return RUNNER_TYPE_LABELS[type];
}

function statusType(status: RunnerStatus): string {
  if (status === RunnerStatus.Hired) return 'is-success';
  if (status === RunnerStatus.Rejected || status === RunnerStatus.NoShow || status === RunnerStatus.NoLongerAvailable) return 'is-danger';
  if (status === RunnerStatus.WaitingForInterviewFeedback || status === RunnerStatus.WaitingForFinalDecision) return 'is-warning';
  return 'is-info';
}

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
  if (column.field !== 'actions') openAction(row, 'history');
}

function openAction(row: RunnerListItem, action: 'status' | 'interview' | 'history') {
  selectedRunner.value = row;
  if (action === 'status') showStatus.value = true;
  else if (action === 'interview') showInterview.value = true;
  else showHistory.value = true;
}

function showCandidateDetail(candidateId: string) {
  candidateDetailId.value = candidateId;
  showCandidateDetailModal.value = true;
}

function onCandidateUpdated() {
  showCandidateDetailModal.value = false;
  loadRunners();
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

function convertToWorker(row: RunnerListItem) {
  if (!row.candidateId) return;
  isLoading.value = true;
  convertCandidateToWorker(row.candidateId)
    .then(() => loadRunners())
    .catch(err => {
      isLoading.value = false;
      showAlertError(err);
    });
}

function onCreate(model: CreateRunnerModel) {
  showCreate.value = false;
  isLoading.value = true;
  createAgencyRunner(requestId, model)
    .then(() => loadRunners())
    .catch(err => {
      isLoading.value = false;
      showAlertError(err);
    });
}

loadRunners();
</script>
