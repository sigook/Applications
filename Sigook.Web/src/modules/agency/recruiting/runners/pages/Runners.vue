<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Runners" :count="totalRunners" :crumbs="moduleCrumbs" />

    <RequestGroupFilters v-model:filter="serverParams" :status-options="statusOptions" @search="loadRunners">
      <b-field>
        <b-select :model-value="serverParams.type ?? null" size="is-small" icon="account-switch-outline"
          @update:modelValue="onTypeChange">
          <option :value="null">All types</option>
          <option v-for="t in RUNNER_TYPES" :key="t" :value="t">{{ RUNNER_TYPE_LABELS[t] }}</option>
        </b-select>
      </b-field>
    </RequestGroupFilters>

    <RequestGroupsToolbar v-model:only-mine="serverParams.onlyMine" :has-groups="groups.length > 0"
      export-url="/api/agency/recruiting/runners/File" :export-params="serverParams" export-file-name="Runners"
      @search="loadRunners" @expand-all="expandAll" @collapse-all="collapseAll"
      @loading="(value: boolean) => isLoading = value" />

    <BulkSelectionBar :count="selectedRunners.length" @clear="clearSelection">
      <b-button size="is-small" type="is-primary" icon-left="label-outline" @click="modalBulkStatus = true">
        Change status
      </b-button>
    </BulkSelectionBar>

    <p v-if="!groups.length && !isLoading" class="container has-text-centered p-4">No records available</p>

    <RequestGroup v-for="group in groups" :key="group.requestId" :group="group" :count-label="countLabel(group)"
      :fill="{ value: group.hiredRunners, total: group.workersQuantity, label: 'hired' }"
      :open="isRequestOpen(group.requestId)" @update:open="(value: boolean) => setRequestOpen(group.requestId, value)">
      <template v-if="!group.usesRunners" #tags>
        <b-tag>Read-only</b-tag>
      </template>
      <template #menu>
        <b-dropdown-item v-if="group.usesRunners" aria-role="listitem" @click="showAddRunner(group.requestId)">
          Add Runner
        </b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="openRequest(group.requestId)">Open Request</b-dropdown-item>
      </template>

      <RunnersGrid :data="group.runners" :can-edit="group.usesRunners" :checkable="group.usesRunners"
        :checked-rows="checkedRows(group.requestId)"
        @update:checked-rows="(rows: AgencyRunner[]) => onCheck(group.requestId, rows)" @updated="loadRunners" />
    </RequestGroup>

    <RequestGroupsPagination :page="serverParams.pageIndex ?? 1" :total="totalRequests"
      :per-page="serverParams.pageSize ?? PAGE_SIZE" @change="onPageChange" />

    <b-modal has-modal-card v-model="modalAddRunner" width="640px" :destroy-on-hide="true">
      <create-runner v-if="addRunnerRequestId" :request-id="addRunnerRequestId" :is-saving="isCreating"
        @create="addRunner" @close="modalAddRunner = false" />
    </b-modal>

    <b-modal has-modal-card v-model="modalBulkStatus" width="520px" :destroy-on-hide="true">
      <BulkRunnerStatusModal :count="selectedRunners.length" :is-saving="isBulkUpdating" @submit="bulkChangeStatus"
        @close="modalBulkStatus = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { getAgencyRunners, changeAgencyRunnersStatus } from '@/modules/agency/recruiting/runners/api';
import { createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { reportBulkResult } from '@/modules/agency/shared/request-groups/bulkReport';
import {
  RUNNER_STATUSES,
  RUNNER_STATUS_LABELS,
  RUNNER_TYPES,
  RUNNER_TYPE_LABELS,
  RunnerStatus,
} from '@/modules/agency/recruiting/requests/runners/types';
import type { CreateRunnerModel, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import type { CatalogItem } from '@/shared/types/common';
import type { AgencyRunner, AgencyRequestRunners, AgencyRunnersFilter } from '@/modules/agency/recruiting/runners/types';
import { AgencyRunnerSortBy } from '@/modules/agency/recruiting/runners/types';
import type { BulkRunnerStatus } from '@/modules/agency/recruiting/runners/components/BulkRunnerStatusModal.vue';
import { useAgencyStore } from '@/modules/agency/store';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import PageHeader from '@/shared/ui/PageHeader.vue';
import RequestGroup from '@/modules/agency/shared/request-groups/RequestGroup.vue';
import RequestGroupFilters from '@/modules/agency/shared/request-groups/RequestGroupFilters.vue';
import RequestGroupsToolbar from '@/modules/agency/shared/request-groups/RequestGroupsToolbar.vue';
import RequestGroupsPagination from '@/modules/agency/shared/request-groups/RequestGroupsPagination.vue';
import BulkSelectionBar from '@/modules/agency/shared/request-groups/BulkSelectionBar.vue';
import RunnersGrid from '@/modules/agency/recruiting/runners/components/RunnersGrid.vue';
import BulkRunnerStatusModal from '@/modules/agency/recruiting/runners/components/BulkRunnerStatusModal.vue';
import CreateRunner from '@/modules/agency/recruiting/requests/runners/CreateRunner.vue';

const PAGE_SIZE = 15;

const router = useRouter();
const agencyStore = useAgencyStore();
const { moduleCrumbs } = useModuleBase();

const isLoading = ref(false);
const totalRunners = ref(0);
const totalRequests = ref(0);
const groups = ref<AgencyRequestRunners[]>([]);
const openRequests = ref<Record<string, boolean>>({});
const modalAddRunner = ref(false);
const addRunnerRequestId = ref<string | null>(null);
const isCreating = ref(false);
const modalBulkStatus = ref(false);
// One table per request, so the selection is kept by request and flattened to
// act on runners of different requests in a single call.
const selection = ref<Record<string, AgencyRunner[]>>({});
const isBulkUpdating = ref(false);
const selectedRunners = computed(() => Object.values(selection.value).flat());

// pageSize counts REQUESTS: every runner of a request travels with it.
const serverParams = ref<AgencyRunnersFilter>({
  pageIndex: 1,
  pageSize: PAGE_SIZE,
  sortBy: AgencyRunnerSortBy.Name,
  isDescending: true,
  onlyMine: false,
  ...agencyStore.agencyRunnersFilter,
});

const statusOptions: CatalogItem<RunnerStatus>[] = RUNNER_STATUSES.map((s) => ({ id: s, value: RUNNER_STATUS_LABELS[s] }));

loadRunners();

function countLabel(group: AgencyRequestRunners): string {
  return group.runners.length === group.totalRunners
    ? `${group.totalRunners} runners`
    : `${group.runners.length} of ${group.totalRunners} runners`;
}

function checkedRows(requestId: string): AgencyRunner[] {
  return selection.value[requestId] ?? [];
}

function onCheck(requestId: string, rows: AgencyRunner[]) {
  selection.value = { ...selection.value, [requestId]: rows };
}

function clearSelection() {
  selection.value = {};
}

function bulkChangeStatus(value: BulkRunnerStatus) {
  const runners = selectedRunners.value;
  if (!runners.length) return;
  isBulkUpdating.value = true;
  changeAgencyRunnersStatus({ runnerIds: runners.map((runner) => runner.id), ...value })
    .then((result) => {
      isBulkUpdating.value = false;
      modalBulkStatus.value = false;
      const names = new Map(runners.map((runner) => [runner.id, runner.name]));
      reportBulkResult(result.updated, result.skipped.map((s) => ({ id: s.runnerId, reason: s.reason })), names, 'runners');
      loadRunners();
    })
    .catch((error) => {
      isBulkUpdating.value = false;
      showAlertError(error);
    });
}

function isRequestOpen(requestId: string): boolean {
  return !!openRequests.value[requestId];
}

function setRequestOpen(requestId: string, value: boolean) {
  openRequests.value = { ...openRequests.value, [requestId]: value };
}

function expandAll() {
  openRequests.value = Object.fromEntries(groups.value.map((group) => [group.requestId, true]));
}

function collapseAll() {
  openRequests.value = {};
}

function onTypeChange(type: RunnerType | null) {
  serverParams.value = { ...serverParams.value, type, pageIndex: 1 };
  loadRunners();
}

function onPageChange(page: number) {
  serverParams.value.pageIndex = page;
  loadRunners();
}

function loadRunners() {
  isLoading.value = true;
  clearSelection();
  agencyStore.updateAgencyRunnersFilter({ ...serverParams.value });
  getAgencyRunners(serverParams.value)
    .then((response) => {
      groups.value = response.items;
      totalRequests.value = response.totalItems;
      totalRunners.value = response.totalRunners;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openRequest(requestId: string) {
  router.push(`/recruiting/requests/${requestId}`);
}

function showAddRunner(requestId: string) {
  addRunnerRequestId.value = requestId;
  modalAddRunner.value = true;
}

function addRunner(model: CreateRunnerModel) {
  if (!addRunnerRequestId.value) return;
  isCreating.value = true;
  createAgencyRunner(addRunnerRequestId.value, model)
    .then(() => {
      modalAddRunner.value = false;
      loadRunners();
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isCreating.value = false;
    });
}
</script>
