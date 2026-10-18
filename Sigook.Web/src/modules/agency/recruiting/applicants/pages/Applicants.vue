<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Applicants" :count="totalApplicants" :crumbs="moduleCrumbs" />

    <RequestGroupFilters v-model:filter="serverParams" :status-options="statusOptions" @search="loadApplicants" />

    <RequestGroupsToolbar v-model:only-mine="serverParams.onlyMine" :has-groups="groups.length > 0"
      export-url="/api/agency/recruiting/applicants/File" :export-params="serverParams" export-file-name="Applicants"
      @search="loadApplicants" @expand-all="expandAll" @collapse-all="collapseAll"
      @loading="(value: boolean) => isLoading = value" />

    <BulkSelectionBar :count="selectedApplicants.length" @clear="clearSelection">
      <b-button size="is-small" icon-left="play-outline" :loading="isBulkUpdating"
        @click="bulkChangeStatus(RequestApplicantStatus.InProgress)">
        Start
      </b-button>
      <b-button size="is-small" type="is-primary" icon-left="check" :loading="isBulkUpdating"
        @click="bulkChangeStatus(RequestApplicantStatus.Confirmed)">
        Confirm
      </b-button>
      <b-button size="is-small" type="is-danger" outlined icon-left="close" :loading="isBulkUpdating"
        @click="bulkChangeStatus(RequestApplicantStatus.Cancelled)">
        Cancel
      </b-button>
    </BulkSelectionBar>

    <p v-if="!groups.length && !isLoading" class="container has-text-centered p-4">No records available</p>

    <RequestGroup v-for="group in groups" :key="group.requestId" :group="group" :count-label="countLabel(group)"
      :fill="{ value: group.confirmedApplicants, total: group.workersQuantity, label: 'confirmed' }"
      :open="isRequestOpen(group.requestId)" @update:open="(value: boolean) => setRequestOpen(group.requestId, value)">
      <template #menu>
        <b-dropdown-item aria-role="listitem" @click="showAddApplicant(group.requestId)">Add Applicant</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="openRequest(group.requestId)">Open Request</b-dropdown-item>
      </template>

      <SigookGrid :data="group.applicants" :paginated="false" :fit-viewport="false" detailed detail-key="id"
        v-model:opened-detailed="openedApplicants" checkable :checked-rows="checkedRows(group.requestId)"
        @update:checked-rows="(rows: AgencyApplicant[]) => onCheck(group.requestId, rows)" @cellclick="onCellClick">
        <b-table-column field="name" label="Applicant" v-slot="props">
          <span class="is-block">
            {{ props.row.name }}
            <b-tooltip label="Candidate" type="is-dark" append-to-body>
              <b-icon v-if="props.row.candidateId" icon="account-hard-hat-outline" size="is-small"></b-icon>
            </b-tooltip>
            <b-tooltip label="Worker" type="is-dark" append-to-body>
              <b-icon v-if="props.row.workerProfileId" icon="badge-account-outline" size="is-small"></b-icon>
            </b-tooltip>
          </span>
          <i class="fz-2 is-lowercase">
            <copy-link kind="email" :value="props.row.email" />
          </i>
        </b-table-column>
        <b-table-column field="status" label="Status" width="140" v-slot="props">
          <b-tag :type="statusTagType(props.row.status)">{{ statusLabel(props.row.status) }}</b-tag>
        </b-table-column>
        <b-table-column field="compliance" label="Compliance" width="180" v-slot="props">
          <div class="applicants-compliance">
            <span class="applicants-compliance-bar">
              <span :class="{ 'is-complete': !props.row.mandatoryPending }"
                :style="{ width: `${compliancePercent(props.row)}%` }"></span>
            </span>
            <span class="fz-1">{{ props.row.complianceCompleted }}/{{ props.row.complianceTotal }}</span>
          </div>
        </b-table-column>
        <b-table-column field="comments" label="Comments" width="260" v-slot="props">
          <div class="applicants-comments">
            <span class="applicants-comments-text" :title="props.row.comments">{{ props.row.comments }}</span>
            <b-button type="is-ghost" size="is-small" icon-right="pencil" @click="showComments(props.row)" />
          </div>
        </b-table-column>
        <b-table-column field="createdBy" label="Added By" width="180" v-slot="props">
          <div class="is-capitalized" v-if="props.row.createdBy">{{ emailName(props.row.createdBy) }}</div>
          <i class="fz-2">{{ dateMonth(props.row.createdAt) }}</i>
        </b-table-column>
        <b-table-column field="actions" width="60" v-slot="props">
          <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item aria-role="listitem" v-if="props.row.candidateId"
              @click="showCandidateDetail(props.row.candidateId)">
              Edit Candidate
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" v-if="props.row.candidateId"
              @click="convertToWorker(props.row.candidateId)">
              Convert to Worker
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" v-if="props.row.workerProfileId"
              @click="openWorker(props.row.workerProfileId)">
              Open Worker
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" v-if="props.row.workerProfileId" @click="showAddRunner(props.row)">
              Add Runner
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" @click="removeApplicant(props.row)">
              Delete
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>

        <template #detail="props">
          <ApplicantComplianceDetail :request-id="props.row.requestId" :applicant-id="props.row.id"
            :name="props.row.name" :status="props.row.status" :worker-profile-id="props.row.workerProfileId"
            @loaded="(value) => onComplianceLoaded(props.row, value)" @status-changed="loadApplicants" />
        </template>
      </SigookGrid>
    </RequestGroup>

    <RequestGroupsPagination :page="serverParams.pageIndex ?? 1" :total="totalRequests"
      :per-page="serverParams.pageSize ?? PAGE_SIZE" @change="onPageChange" />

    <b-modal custom-content-class="card" v-model="modalAddApplicant" width="800px" :destroy-on-hide="true">
      <ManageApplicantsModal :request-id="addApplicantRequestId"
        @updateApplicants="(args) => addApplicant(args.model)" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalComments" width="500px" :destroy-on-hide="true">
      <EditTextarea v-if="currentApplicant" title="Comments" :min-length="0" :data="currentApplicant.comments"
        @updateContent="saveComments" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalCandidateDetail" width="500px" :destroy-on-hide="true">
      <DetailCandidate v-if="candidateDetailId" :candidate-id="candidateDetailId" @onUpdateWorker="onCandidateUpdated" />
    </b-modal>

    <b-modal has-modal-card v-model="modalAddRunner" width="420px" :destroy-on-hide="true">
      <SelectRunnerTypeModal v-if="currentApplicant" :name="currentApplicant.name" @select="addRunner"
        @close="modalAddRunner = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { dateMonth, emailName } from '@/shared/format';
import {
  getAgencyApplicants,
  postAgencyRequestApplicant,
  deleteAgencyRequestApplicant,
  updateAgencyRequestApplicant,
  changeAgencyApplicantsStatus,
} from '@/modules/agency/recruiting/requests/api';
import { convertCandidateToWorker } from '@/modules/agency/recruiting/candidates/api';
import { createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { reportBulkResult } from '@/modules/agency/shared/request-groups/bulkReport';
import type { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import type { CatalogItem, TableColumnRef } from '@/shared/types/common';
import type { AgencyApplicant, AgencyRequestApplicants, AgencyApplicantsFilter } from '@/modules/agency/recruiting/applicants/types';
import type { CreateRequestApplicantModel } from '@/modules/agency/recruiting/requests/types';
import { AgencyApplicantSortBy } from '@/modules/agency/recruiting/applicants/types';
import {
  REQUEST_APPLICANT_STATUSES,
  REQUEST_APPLICANT_STATUS_LABELS,
  RequestApplicantStatus,
  requestApplicantStatusLabel,
  requestApplicantStatusTagType,
} from '@/shared/request-detail/requestApplicant';
import { useAgencyStore } from '@/modules/agency/store';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import PageHeader from '@/shared/ui/PageHeader.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';
import RequestGroup from '@/modules/agency/shared/request-groups/RequestGroup.vue';
import RequestGroupFilters from '@/modules/agency/shared/request-groups/RequestGroupFilters.vue';
import RequestGroupsToolbar from '@/modules/agency/shared/request-groups/RequestGroupsToolbar.vue';
import RequestGroupsPagination from '@/modules/agency/shared/request-groups/RequestGroupsPagination.vue';
import BulkSelectionBar from '@/modules/agency/shared/request-groups/BulkSelectionBar.vue';
import ManageApplicantsModal from '@/modules/agency/recruiting/requests/components/ManageApplicantsModal.vue';
import ApplicantComplianceDetail from '@/modules/agency/recruiting/applicants/components/ApplicantComplianceDetail.vue';
import EditTextarea from '@/shared/ui/EditTextarea.vue';
import DetailCandidate from '@/modules/agency/recruiting/candidates/components/DetailCandidate.vue';
import SelectRunnerTypeModal from '@/modules/agency/recruiting/requests/runners/SelectRunnerTypeModal.vue';

const PAGE_SIZE = 15;

const router = useRouter();
const agencyStore = useAgencyStore();
const { moduleCrumbs } = useModuleBase();

const isLoading = ref(false);
const totalApplicants = ref(0);
const totalRequests = ref(0);
const groups = ref<AgencyRequestApplicants[]>([]);
const openRequests = ref<Record<string, boolean>>({});
const openedApplicants = ref<string[]>([]);
const modalAddApplicant = ref(false);
const addApplicantRequestId = ref<string | null>(null);
const modalComments = ref(false);
const modalCandidateDetail = ref(false);
const modalAddRunner = ref(false);
const currentApplicant = ref<AgencyApplicant | null>(null);
const candidateDetailId = ref<string | null>(null);
// One table per request, so the selection is kept by request and flattened to
// act on applicants of different requests in a single call.
const selection = ref<Record<string, AgencyApplicant[]>>({});
const isBulkUpdating = ref(false);
const selectedApplicants = computed(() => Object.values(selection.value).flat());

// pageSize counts REQUESTS: every applicant of a request travels with it.
// Requests come by number, newest first, like the requests grid.
const serverParams = ref<AgencyApplicantsFilter>({
  pageIndex: 1,
  pageSize: PAGE_SIZE,
  sortBy: AgencyApplicantSortBy.Name,
  isDescending: true,
  onlyMine: false,
  ...agencyStore.agencyApplicantsFilter,
});

const statusOptions: CatalogItem<RequestApplicantStatus>[] =
  REQUEST_APPLICANT_STATUSES.map((s) => ({ id: s, value: REQUEST_APPLICANT_STATUS_LABELS[s] }));
const statusLabel = requestApplicantStatusLabel;
const statusTagType = requestApplicantStatusTagType;

loadApplicants();

function countLabel(group: AgencyRequestApplicants): string {
  return group.applicants.length === group.totalApplicants
    ? `${group.totalApplicants} applicants`
    : `${group.applicants.length} of ${group.totalApplicants} applicants`;
}

function checkedRows(requestId: string): AgencyApplicant[] {
  return selection.value[requestId] ?? [];
}

function onCheck(requestId: string, rows: AgencyApplicant[]) {
  selection.value = { ...selection.value, [requestId]: rows };
}

function clearSelection() {
  selection.value = {};
}

function bulkChangeStatus(status: RequestApplicantStatus) {
  const applicants = selectedApplicants.value;
  if (!applicants.length) return;
  if (status === RequestApplicantStatus.Cancelled) {
    showAlertConfirm('Are you sure', `You want to cancel ${applicants.length} applicants`)
      .then((response) => {
        if (response) sendBulkStatus(applicants, status);
      })
      .catch((error) => showAlertError(error));
    return;
  }
  sendBulkStatus(applicants, status);
}

function sendBulkStatus(applicants: AgencyApplicant[], status: RequestApplicantStatus) {
  isBulkUpdating.value = true;
  changeAgencyApplicantsStatus({ applicantIds: applicants.map((applicant) => applicant.id), status })
    .then((result) => {
      isBulkUpdating.value = false;
      const names = new Map(applicants.map((applicant) => [applicant.id, applicant.name]));
      reportBulkResult(result.updated, result.skipped.map((s) => ({ id: s.applicantId, reason: s.reason })), names, 'applicants');
      loadApplicants();
    })
    .catch((error) => {
      isBulkUpdating.value = false;
      showAlertError(error);
    });
}

// The checklist reports its state after every change, so the row's progress
// stays live without reloading the whole page of requests.
function onComplianceLoaded(row: AgencyApplicant, value: { mandatoryCompleted: boolean; itemsCount: number; completedCount: number }) {
  row.complianceTotal = value.itemsCount;
  row.complianceCompleted = value.completedCount;
  row.mandatoryPending = value.mandatoryCompleted ? 0 : Math.max(row.mandatoryPending, 1);
}

// Requests land collapsed: the page opens as a scannable list of requests and
// you expand the ones you are working on.
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

function compliancePercent(row: AgencyApplicant): number {
  return row.complianceTotal ? Math.round((row.complianceCompleted / row.complianceTotal) * 100) : 0;
}

function onCellClick(row: AgencyApplicant, column: TableColumnRef) {
  if (column.field === 'name' && row.workerProfileId) {
    router.push(`/recruiting/workers/${row.workerProfileId}`);
    return;
  }
  if (column.field === 'compliance' || column.field === 'status') {
    toggleCompliance(row);
  }
}

function toggleCompliance(row: AgencyApplicant) {
  openedApplicants.value = openedApplicants.value.includes(row.id)
    ? openedApplicants.value.filter((id) => id !== row.id)
    : [...openedApplicants.value, row.id];
}

function onPageChange(page: number) {
  serverParams.value.pageIndex = page;
  loadApplicants();
}

function loadApplicants() {
  isLoading.value = true;
  clearSelection();
  agencyStore.updateAgencyApplicantsFilter({ ...serverParams.value });
  getAgencyApplicants(serverParams.value)
    .then((response) => {
      groups.value = response.items;
      totalRequests.value = response.totalItems;
      totalApplicants.value = response.totalApplicants;
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

function showAddApplicant(requestId: string) {
  addApplicantRequestId.value = requestId;
  modalAddApplicant.value = true;
}

function addApplicant(model: CreateRequestApplicantModel) {
  if (!addApplicantRequestId.value) return;
  modalAddApplicant.value = false;
  isLoading.value = true;
  postAgencyRequestApplicant(addApplicantRequestId.value, model)
    .then(() => {
      isLoading.value = false;
      loadApplicants();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function showComments(row: AgencyApplicant) {
  currentApplicant.value = row;
  modalComments.value = true;
}

function saveComments(comments: string) {
  const applicant = currentApplicant.value;
  if (!applicant) return;
  modalComments.value = false;
  isLoading.value = true;
  updateAgencyRequestApplicant(applicant.requestId, applicant.id, { comments })
    .then(() => {
      applicant.comments = comments;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function showCandidateDetail(candidateId: string) {
  candidateDetailId.value = candidateId;
  modalCandidateDetail.value = true;
}

function onCandidateUpdated() {
  modalCandidateDetail.value = false;
  loadApplicants();
}

function openWorker(workerProfileId: string) {
  router.push(`/recruiting/workers/${workerProfileId}`);
}

function showAddRunner(row: AgencyApplicant) {
  currentApplicant.value = row;
  modalAddRunner.value = true;
}

function addRunner(type: RunnerType) {
  const applicant = currentApplicant.value;
  if (!applicant?.workerProfileId) return;
  modalAddRunner.value = false;
  isLoading.value = true;
  createAgencyRunner(applicant.requestId, { workerProfileId: applicant.workerProfileId, type })
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Runner added');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function convertToWorker(candidateId: string) {
  isLoading.value = true;
  convertCandidateToWorker(candidateId)
    .then(() => {
      isLoading.value = false;
      loadApplicants();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function removeApplicant(row: AgencyApplicant) {
  showAlertConfirm('Are you sure', `You want to remove ${row.name} from request ${row.requestNumberId}`)
    .then((response) => {
      if (!response) return;
      isLoading.value = true;
      deleteAgencyRequestApplicant(row.requestId, row.id)
        .then(() => {
          isLoading.value = false;
          loadApplicants();
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    })
    .catch((error) => showAlertError(error));
}
</script>
