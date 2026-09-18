<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Applicants" :count="totalApplicants" :crumbs="moduleCrumbs" />

    <b-field grouped group-multiline class="applicants-filters">
      <b-field class="input-no-arrows applicants-filter-number">
        <b-input :model-value="serverParams.numberId" type="number" placeholder="Request #" icon="pound" size="is-small"
          @update:modelValue="onNumberIdChanged" @keypress="onInputEntered"></b-input>
      </b-field>
      <b-field>
        <b-input v-model="serverParams.name" placeholder="Search name or email" icon="magnify" size="is-small"
          @keypress="onInputEntered"></b-input>
      </b-field>
      <b-field>
        <b-autocomplete v-model="companyTerm" :data="companies" field="value" placeholder="Client" icon="domain"
          size="is-small" open-on-focus clearable append-to-body @typing="searchCompanies" @select="onCompanySelected">
        </b-autocomplete>
      </b-field>
      <b-field>
        <b-input v-model="serverParams.jobTitle" placeholder="Position" icon="briefcase-outline" size="is-small"
          @keypress="onInputEntered"></b-input>
      </b-field>
      <b-field>
        <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
          field="value" icon="label" placeholder="Status" append-to-body @update:modelValue="onStatusChange">
        </b-taginput>
      </b-field>
      <b-field>
        <b-datepicker size="is-small" :mobile-native="false" placeholder="Start date" range
          v-model="startAtSelected" :icon-right="startAtSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
          append-to-body @icon-right-click="onStartAtCleared" @update:modelValue="onStartAtSelected"></b-datepicker>
      </b-field>
      <b-field>
        <b-checkbox size="is-small" v-model="serverParams.onlyMine" @update:modelValue="loadApplicants">
          Only my requests
        </b-checkbox>
      </b-field>
    </b-field>

    <div class="applicants-toolbar">
      <template v-if="groups.length">
        <b-button size="is-small" icon-left="chevron-double-down" @click="expandAll">Expand all requests</b-button>
        <b-button size="is-small" icon-left="chevron-double-up" @click="collapseAll">Collapse all</b-button>
      </template>
      <export url="/api/agency/recruiting/applicants/File" :params="serverParams" file-name="Applicants"
        @onDataLoading="(value: boolean) => isLoading = value" />
    </div>

    <div v-if="selectedApplicants.length" class="applicants-selection">
      <span class="applicants-selection-count">{{ selectedApplicants.length }} selected</span>
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
      <b-button size="is-small" type="is-text" @click="clearSelection">Clear</b-button>
    </div>

    <p v-if="!groups.length && !isLoading" class="container has-text-centered p-4">No records available</p>

    <div v-for="group in groups" :key="group.requestId" class="applicants-request">
      <b-collapse :model-value="isRequestOpen(group.requestId)" animation="slide"
        @update:modelValue="(value: boolean) => setRequestOpen(group.requestId, value)">
        <template #trigger="props">
          <div class="applicants-request-header" role="button">
            <b-icon :icon="props.open ? 'chevron-down' : 'chevron-right'" size="is-small"></b-icon>
            <span class="applicants-request-info">
              <span class="applicants-request-title">
                {{ group.numberId }} &middot; {{ group.companyFullName }} &middot; {{ group.jobTitle }}
              </span>
              <span class="applicants-request-meta">
                <template v-if="group.applicants.length === group.totalApplicants">
                  {{ group.totalApplicants }} applicants
                </template>
                <template v-else>{{ group.applicants.length }} of {{ group.totalApplicants }} applicants</template>
                <template v-if="group.city">&middot; {{ group.city }}, {{ group.provinceName }}</template>
                <template v-if="group.displayShift">&middot; {{ group.displayShift }}</template>
              </span>
            </span>
            <span class="applicants-request-tags">
              <span v-if="group.isDirectHiring" class="asap">DH</span>
              <span v-if="group.isAsap" class="asap">ASAP</span>
              <b-tag v-if="group.startAt" :type="isStartingSoon(group.startAt) ? 'is-warning' : ''">
                <b-icon icon="calendar-blank-outline" size="is-small"></b-icon>
                Starts {{ dateMonth(group.startAt) }}
              </b-tag>
              <b-tag class="applicants-request-fill"
                :type="group.confirmedApplicants >= group.workersQuantity ? 'is-success' : 'is-warning'">
                {{ group.confirmedApplicants }}/{{ group.workersQuantity }} confirmed
              </b-tag>
            </span>
            <span class="applicants-request-actions" @click.stop>
              <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
                <template #trigger>
                  <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
                </template>
                <b-dropdown-item aria-role="listitem" @click="showAddApplicant(group.requestId)">
                  Add Applicant
                </b-dropdown-item>
                <b-dropdown-item aria-role="listitem" @click="openRequest(group.requestId)">
                  Open Request
                </b-dropdown-item>
              </b-dropdown>
            </span>
          </div>
        </template>

        <b-table :data="group.applicants" narrowed hoverable :mobile-cards="false" detailed detail-key="id"
          detail-transition="fade" v-model:opened-detailed="openedApplicants" checkable
          :checked-rows="checkedRows(group.requestId)"
          @update:checkedRows="(rows: AgencyApplicant[]) => onCheck(group.requestId, rows)" @cellclick="onCellClick">
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
              <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
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
        </b-table>
      </b-collapse>
    </div>

    <b-pagination v-if="totalRequests > serverParams.pageSize" class="applicants-pagination"
      v-model="serverParams.pageIndex" :total="totalRequests" :per-page="serverParams.pageSize" size="is-small" rounded
      @change="onPageChange">
    </b-pagination>

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
import { ref, reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess, showAlertSummary } from '@/utils/toast';
import { dateMonth, emailName } from '@/utils/filters';
import {
  getAgencyApplicants,
  postAgencyRequestApplicant,
  deleteAgencyRequestApplicant,
  updateAgencyRequestApplicant,
  changeAgencyApplicantsStatus,
} from '@/api/agencyRequestApi';
import { getAgencyCompaniesList } from '@/api/agencyCompanyApi';
import { convertCandidateToWorker } from '@/api/agencyCandidateApi';
import { createAgencyRunner } from '@/api/agencyRunnerApi';
import type { RunnerType } from '@/types/runner';
import type { CatalogItem, TableColumnRef } from '@/types/common';
import type {
  AgencyApplicant,
  AgencyRequestApplicants,
  AgencyApplicantsFilter,
  ChangeApplicantsStatusResult,
  CreateRequestApplicantModel,
} from '@/types/agency';
import { AgencyApplicantSortBy } from '@/types/agency';
import {
  REQUEST_APPLICANT_STATUSES,
  REQUEST_APPLICANT_STATUS_LABELS,
  RequestApplicantStatus,
  requestApplicantStatusLabel,
  requestApplicantStatusTagType,
} from '@/types/requestApplicant';
import { useAgencyStore } from '@/stores/agency';
import { useModuleBase } from '@/composables/useModuleBase';
import PageHeader from '@/components/PageHeader.vue';
import Export from '@/components/Export.vue';
import ManageApplicantsModal from '@/components/agency_request/ManageApplicantsModal.vue';
import ApplicantComplianceDetail from '@/components/agency_request/ApplicantComplianceDetail.vue';
import EditTextarea from '@/components/agency_request/EditTextarea.vue';
import DetailCandidate from '@/components/candidate/DetailCandidate.vue';
import SelectRunnerTypeModal from '@/components/runner/SelectRunnerTypeModal.vue';

const router = useRouter();
const agencyStore = useAgencyStore();
const { moduleCrumbs } = useModuleBase();

const isLoading = ref(false);
const totalApplicants = ref(0);
const totalRequests = ref(0);
const groups = ref<AgencyRequestApplicants[]>([]);
const openRequests = ref<Record<string, boolean>>({});
const openedApplicants = ref<string[]>([]);
const companies = ref<CatalogItem[]>([]);
const companyTerm = ref('');
const startAtSelected = ref<Date[]>([]);
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
const serverParams = reactive<AgencyApplicantsFilter>({
  pageIndex: 1,
  pageSize: 15,
  sortBy: AgencyApplicantSortBy.Name,
  isDescending: true,
  onlyMine: false,
});

const statusOptions: CatalogItem<RequestApplicantStatus>[] =
  REQUEST_APPLICANT_STATUSES.map((s) => ({ id: s, value: REQUEST_APPLICANT_STATUS_LABELS[s] }));
const statusesSelected = ref<CatalogItem<RequestApplicantStatus>[]>([]);
const statusLabel = requestApplicantStatusLabel;
const statusTagType = requestApplicantStatusTagType;

restoreFilter();
loadApplicants();

// The filter survives leaving the page, as in the candidates and requests grids.
function restoreFilter() {
  const stored = agencyStore.agencyApplicantsFilter;
  if (!stored) return;
  Object.assign(serverParams, stored);
  if (stored.statuses?.length) {
    statusesSelected.value = statusOptions.filter((option) => stored.statuses?.includes(option.id));
  }
  if (stored.startAtFrom && stored.startAtTo) {
    startAtSelected.value = [new Date(stored.startAtFrom), new Date(stored.startAtTo)];
  }
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
      reportBulkResult(result, applicants);
      loadApplicants();
    })
    .catch((error) => {
      isBulkUpdating.value = false;
      showAlertError(error);
    });
}

// The endpoint changes what it can and returns why it left the rest untouched.
// A clean run is just a toast; the skipped ones go to a dialog because the list
// grows with the selection and a toast that long covers the page.
function reportBulkResult(result: ChangeApplicantsStatusResult, applicants: AgencyApplicant[]) {
  if (!result.skipped.length) {
    showAlertSuccess(`${result.updated} applicants updated`);
    return;
  }
  const names = new Map(applicants.map((applicant) => [applicant.id, applicant.name]));
  const detail = result.skipped
    .map((skipped) => `<li><strong>${escapeHtml(names.get(skipped.applicantId) ?? 'Applicant')}</strong>: ${escapeHtml(skipped.reason)}</li>`)
    .join('');
  showAlertSummary(
    `${result.updated} updated, ${result.skipped.length} skipped`,
    `<ul class="applicants-skipped">${detail}</ul>`,
  );
}

function escapeHtml(value: string): string {
  const element = document.createElement('span');
  element.textContent = value;
  return element.innerHTML;
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

function daysUntil(startAt: string): number {
  const start = new Date(startAt);
  const today = new Date();
  start.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);
  return Math.round((start.getTime() - today.getTime()) / 86400000);
}

function isStartingSoon(startAt: string): boolean {
  return daysUntil(startAt) <= 2;
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

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    serverParams.pageIndex = 1;
    loadApplicants();
  }
}

function onNumberIdChanged(value: string | number) {
  serverParams.numberId = value === '' || value === null ? null : Number(value);
}

function onStatusChange() {
  serverParams.statuses = statusesSelected.value.length ? statusesSelected.value.map((s) => s.id) : undefined;
  serverParams.pageIndex = 1;
  loadApplicants();
}

function onStartAtSelected() {
  serverParams.startAtFrom = startAtSelected.value[0]?.toISOString() ?? null;
  serverParams.startAtTo = startAtSelected.value[1]?.toISOString() ?? null;
  serverParams.pageIndex = 1;
  loadApplicants();
}

function onStartAtCleared() {
  startAtSelected.value = [];
  onStartAtSelected();
}

function searchCompanies(term: string) {
  if (term.length < 3) {
    companies.value = [];
    return;
  }
  getAgencyCompaniesList(term)
    .then((response) => {
      companies.value = response;
    })
    .catch((error) => showAlertError(error));
}

function onCompanySelected(company: CatalogItem | null) {
  serverParams.companyProfileId = company?.id ?? null;
  serverParams.pageIndex = 1;
  loadApplicants();
}

function onPageChange(page: number) {
  serverParams.pageIndex = page;
  loadApplicants();
}

function loadApplicants() {
  isLoading.value = true;
  clearSelection();
  agencyStore.updateAgencyApplicantsFilter({ ...serverParams });
  getAgencyApplicants(serverParams)
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
