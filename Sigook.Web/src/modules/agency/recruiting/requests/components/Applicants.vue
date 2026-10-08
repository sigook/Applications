<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="getAgencyRequestApplicant" v-model:params="serverParams" :sort-map="sortMap"
        focusable detailed detail-key="id" v-model:opened-detailed="openedApplicants"
        @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
        <template #actions>
          <b-button icon-left="plus" @click="modalManageWorkers = true">Manage Applicants</b-button>
        </template>
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
          <default-image v-else :name="props.row.name" class="img-30"></default-image>
        </b-table-column>
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span class="is-block">
              {{ props.row.name }}
              <b-tooltip label="Candidate" type="is-dark" append-to-body>
                <b-icon v-if="props.row.candidateId" icon="account-hard-hat-outline" size="is-small"></b-icon>
              </b-tooltip>
              <b-tooltip label="Worker" type="is-dark" append-to-body>
                <b-icon v-if="props.row.workerProfileId" icon="badge-account-outline" size="is-small"></b-icon>
              </b-tooltip>
            </span>
            <i class="fz-2 ellipsis-150 is-lowercase">
              <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
            </i>
          </template>
        </b-table-column>
        <b-table-column field="status" label="Status" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange"
              append-to-body />
          </template>
          <template v-slot="props">
            <b-tag :type="statusTagType(props.row.status)">{{ statusLabel(props.row.status) }}</b-tag>
          </template>
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
        <b-table-column field="phoneNumber" label="Phone" searchable>
          <template v-slot:searchable>
            <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
              @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
          </template>
          <template v-slot="props">
            <div v-if="props.row.phoneNumber">
              {{ props.row.phoneNumber }}
            </div>
            <div v-else class="op3">Phone</div>
          </template>
        </b-table-column>
        <b-table-column field="createdBy" label="Added By" sortable searchable>
          <template v-slot:searchable>
            <b-field>
              <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"></b-input>
              <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                @update:modelValue="onCreatedAtSelected" append-to-body></b-datepicker>
            </b-field>
          </template>
          <template v-slot="props">
            <div class="is-capitalized" v-if="props.row.createdBy">
              <p>{{ emailName(props.row.createdBy) }}</p>
            </div>
            <div v-else class="op3">Added by</div>
            <i class="fz-2">{{ dateMonth(props.row.createdAt) }}</i>
          </template>
        </b-table-column>
        <b-table-column field="comments" label="Comments" v-slot="props">
          <span v-html="props.row.comments"></span>
          <b-button type="is-ghost" icon-right="pencil" @click="showEditModal(props.row)">
          </b-button>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
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
              @click="showAddRunner(props.row)">
              Add Runner
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" @click="removeApplicant(props.row)">
              Delete
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>

        <template #detail="props">
          <ApplicantComplianceDetail :request-id="serverParams.requestId" :applicant-id="props.row.id"
            :name="props.row.name ?? ''" :status="props.row.status" :worker-profile-id="props.row.workerProfileId"
            @loaded="(value) => onComplianceLoaded(props.row, value)" @status-changed="reloadApplicants" />
        </template>
      </SigookGrid>
    </div>

    <b-modal custom-content-class="card" v-model="modalManageWorkers" width="800px">
      <ManageTabs @updateApplicants="(args) => addApplicant(args.model)" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalCandidateDetail" width="500px">
      <detail-candidate v-if="candidateDetailId" :candidate-id="candidateDetailId"
        @onUpdateWorker="onCandidateUpdated"></detail-candidate>
    </b-modal>

    <b-modal has-modal-card v-model="modalAddRunner" width="420px">
      <select-runner-type-modal v-if="runnerApplicant" :name="runnerApplicant.name" @select="addRunner"
        @close="modalAddRunner = false" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalComment" width="500px">
      <EditTextarea v-if="currentItem" :title="'Comments'" subtitle="Comments" :min-length="0" :data="currentItem.comments"
        @updateContent="(data) => saveApplicantComment(data)"></EditTextarea>
    </b-modal>

  </div>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import { emailName, dateMonth } from '@/shared/format';
import { formatPhone } from '@/shared/utils/phoneFormat';
import {
  getAgencyRequestApplicant,
  postAgencyRequestApplicant,
  deleteAgencyRequestApplicant,
  updateAgencyRequestApplicant
} from "@/modules/agency/recruiting/requests/api";
import { convertCandidateToWorker } from "@/modules/agency/recruiting/candidates/api";
import { createAgencyRunner } from "@/modules/agency/recruiting/requests/runners/api";
import type { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import type { CatalogItem, GridHandle, TableColumnRef } from '@/shared/types/common';
import type { AgencyRequestApplicant, AgencyRequestApplicantFilter, AgencyRequestDetail, CreateRequestApplicantModel } from '@/modules/agency/recruiting/requests/types';
import {
  REQUEST_APPLICANT_STATUSES,
  REQUEST_APPLICANT_STATUS_LABELS,
  RequestApplicantStatus,
  requestApplicantStatusLabel,
  requestApplicantStatusTagType,
} from '@/shared/request-detail/requestApplicant';
import ApplicantComplianceDetail from '@/modules/agency/recruiting/applicants/components/ApplicantComplianceDetail.vue';
import ManageTabs from '@/modules/agency/recruiting/requests/components/ManageApplicantsModal.vue';
import SelectRunnerTypeModal from '@/modules/agency/recruiting/requests/runners/SelectRunnerTypeModal.vue';
import EditTextarea from '@/shared/ui/EditTextarea.vue';
import DetailCandidate from '@/modules/agency/recruiting/candidates/components/DetailCandidate.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

defineProps<{ request?: AgencyRequestDetail | null }>();

const route = useRoute();
const router = useRouter();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  name: 0,
  createdBy: 1,
};

const isLoading = ref(false);
const currentItem = ref<AgencyRequestApplicant | null>(null);
const createdAtDatesSelected = ref<Date[]>([]);
const modalManageWorkers = ref(false);
const modalComment = ref(false);
const modalCandidateDetail = ref(false);
const modalAddRunner = ref(false);
const runnerApplicant = ref<AgencyRequestApplicant | null>(null);
const candidateDetailId = ref<string | null>(null);
const openedApplicants = ref<string[]>([]);
const serverParams = ref<AgencyRequestApplicantFilter>({
  sortBy: 1,
  requestId: route.params.id as string,
  isDescending: true
});

const statusOptions: CatalogItem<RequestApplicantStatus>[] =
  REQUEST_APPLICANT_STATUSES.map(s => ({ id: s, value: REQUEST_APPLICANT_STATUS_LABELS[s] }));
const statusesSelected = ref<CatalogItem<RequestApplicantStatus>[]>([]);
const statusLabel = requestApplicantStatusLabel;
const statusTagType = requestApplicantStatusTagType;

function compliancePercent(row: AgencyRequestApplicant): number {
  return row.complianceTotal ? Math.round((row.complianceCompleted / row.complianceTotal) * 100) : 0;
}

// The checklist reports its state after every change, so the row's progress
// stays live without reloading the grid.
function onComplianceLoaded(
  row: AgencyRequestApplicant,
  value: { mandatoryCompleted: boolean; itemsCount: number; completedCount: number },
) {
  row.complianceTotal = value.itemsCount;
  row.complianceCompleted = value.completedCount;
  row.mandatoryPending = value.mandatoryCompleted ? 0 : Math.max(row.mandatoryPending, 1);
}

function onStatusChange() {
  serverParams.value.statuses = statusesSelected.value.length ? statusesSelected.value.map(s => s.id) : undefined;
  grid.value?.search();
}

function onCellClick(row: AgencyRequestApplicant, column: TableColumnRef) {
  switch (column.field) {
    case 'comments':
    case 'actions':
      break;
    case 'compliance':
    case 'status':
      toggleCompliance(row);
      break;
    default:
      if (row.workerProfileId) {
        router.push(`/recruiting/workers/${row.workerProfileId}`);
      }
  }
}

function toggleCompliance(row: AgencyRequestApplicant) {
  openedApplicants.value = openedApplicants.value.includes(row.id)
    ? openedApplicants.value.filter((id) => id !== row.id)
    : [...openedApplicants.value, row.id];
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function reloadApplicants() {
  grid.value?.reload();
}

function addApplicant(model: CreateRequestApplicantModel) {
  modalManageWorkers.value = false;
  isLoading.value = true;
  postAgencyRequestApplicant(serverParams.value.requestId, model).then(() => {
    isLoading.value = false;
    reloadApplicants();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

function removeApplicant(item: AgencyRequestApplicant) {
  isLoading.value = true;
  deleteAgencyRequestApplicant(serverParams.value.requestId, item.id).then(() => {
    isLoading.value = false;
    reloadApplicants();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

function showEditModal(item: AgencyRequestApplicant) {
  currentItem.value = item;
  modalComment.value = true;
}

function saveApplicantComment(comment: string) {
  const applicant = currentItem.value;
  if (!applicant) return;
  modalComment.value = false;
  isLoading.value = true;
  updateAgencyRequestApplicant(serverParams.value.requestId, applicant.id, { comments: comment })
    .then(() => {
      isLoading.value = false;
      reloadApplicants();
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
  reloadApplicants();
}

function showAddRunner(item: AgencyRequestApplicant) {
  runnerApplicant.value = item;
  modalAddRunner.value = true;
}

function addRunner(type: RunnerType) {
  const applicant = runnerApplicant.value;
  if (!applicant?.workerProfileId) return;
  modalAddRunner.value = false;
  isLoading.value = true;
  createAgencyRunner(serverParams.value.requestId, {
    workerProfileId: applicant.workerProfileId,
    type
  })
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
      reloadApplicants();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
