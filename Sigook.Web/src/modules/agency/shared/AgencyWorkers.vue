<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="loadRequestWorkers" v-model:params="serverParams" :sort-map="sortMap" focusable
        :fit-viewport="directHiring" :detailed="!directHiring" show-detail-icon
        @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
        <template #actions>
          <b-button icon-left="plus" @click="modalManageWorkers = true">Manage Workers</b-button>
        </template>
        <template #dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="downloadWorkersReportDocument">
            <b-icon icon="file-excel"></b-icon>
            <span>Export</span>
          </b-dropdown-item>
          <b-dropdown-item v-if="!directHiring" aria-role="listitem" @click="downloadPunchCardDocument">
            <b-icon icon="file-excel"></b-icon>
            <span>Export Punch Card</span>
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="onShowModalFinishAll">
            <b-icon icon="flag-checkered"></b-icon>
            <span>End all assignments</span>
          </b-dropdown-item>
        </template>
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image"
            class="img-30 img-rounded" />
          <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
        </b-table-column>
        <b-table-column field="numberId" label="ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
          </template>
        </b-table-column>
        <b-table-column field="externalId" label="External ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">{{ props.row.externalId }}</template>
        </b-table-column>
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Name, email or phone" icon="magnify"
              size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <div class="worker-cell">
              <span>{{ props.row.name }}</span>
              <span class="fz-1"><copy-link kind="email" :value="props.row.email" /></span>
              <span v-if="props.row.mobileNumber" class="fz-1">
                <copy-link kind="phone" :value="props.row.mobileNumber" />
              </span>
              <span v-if="props.row.socialInsurance" class="fz-1">
                SIN/SSN {{ props.row.socialInsurance }}<i v-if="props.row.dueDate"> · {{ dateMonth(props.row.dueDate) }}</i>
              </span>
            </div>
          </template>
        </b-table-column>
        <b-table-column field="startWorking" label="Start Working" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="startWorkingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onStartWorkingCleared" range v-model="startWorkingDatesSelected"
              @update:modelValue="onStartWorkingSelected" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            <b-button type="is-ghost" icon-right="pencil" @click="onShowModalStartWorking(props.row)">
              {{ dateMonth(props.row.startWorking) }}
            </b-button>
          </template>
        </b-table-column>
        <b-table-column v-if="!directHiring" field="hours" label="Hours" cell-class="is-nowrap" v-slot="props">
          {{ hour(props.row.totalHoursApproved) }}
          <span class="fz-1 block">of {{ hour(props.row.totalHoursWorker) }}</span>
        </b-table-column>
        <b-table-column field="notesCount" label="Notes" v-slot="props">
          <NotesPopover :can-create="false" :user-id="props.row.id" :request-id="serverParams.requestId"
            :notes-count="props.row.notesCount" :on-get="getNotes" :on-create="createNote"
            :on-update="updateNote" :on-delete="deleteNote"
            @update:count="(size) => props.row.notesCount = size">
          </NotesPopover>
        </b-table-column>
        <b-table-column field="status" label="Status" sortable searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <b-tag rounded :type="WorkerRequestStatusTagTypes[props.row.workerRequestStatus]">{{ props.row.status }}</b-tag>
            <span v-if="props.row.finishReason" class="fz-1 block">{{ WorkerRequestFinishReasonLabels[props.row.finishReason] }}</span>
            <i v-if="props.row.finishComments" class="fz-1 block">{{ props.row.finishComments }}</i>
            <i class="fz-1 block" v-html="props.row.rejectComments"></i>
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-field v-if="props.row.workerRequestStatus === WorkerRequestStatus.Booked" grouped>
            <b-tooltip label="End assignment" type="is-dark" position="is-top" append-to-body>
              <b-button type="is-primary" outlined rounded icon-right="flag-checkered"
                @click="onShowModalFinish(props.row)"></b-button>
            </b-tooltip>
            <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body class="ml-2">
              <b-button type="is-danger" outlined rounded icon-right="close"
                @click="confirmDelete(props.row)"></b-button>
            </b-tooltip>
          </b-field>
        </b-table-column>
        <template v-if="!directHiring" #detail="props">
          <PunchCard :workerProfileId="props.row.workerProfileId" :worker="props.row"
            :requestId="serverParams.requestId" :request="request" />
        </template>
      </SigookGrid>
    </div>

    <!-- custom modal Manage Workers-->
    <b-modal custom-content-class="card" v-model="modalManageWorkers" width="500px">
      <workers-list :request-id="serverParams.requestId" @workerBooked="onWorkerBooked"></workers-list>
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalRejectWorker" width="500px">
      <edit-textarea title="Reject Worker" :subtitle="'Please indicate the reason.'" :min-length="10"
        class="sm-edit-textarea" @updateContent="(data) => rejectWorker(data)" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalFinish" width="560px" destroy-on-hide>
      <FinishAssignmentModal :subject="finishSubject" :start-working="finishAll ? null : currentWorker?.startWorking"
        @submit="finishAssignment" @close="modalFinish = false" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalStartWorking" width="415px">
      <datepicker-modal v-if="currentWorker" v-model:start-working="currentWorker.startWorking"
        @onSelectCalendar="(date) => onUpdateRequestWorkerStartDate(date)" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { showAlertError } from "@/shared/utils/toast";
import { downloadFile } from "@/shared/utils/downloadFile";
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { isDirectHiring } from '@/shared/punch-card/directHiring';
import {
  finishAgencyRequestWorker,
  finishAllAgencyRequestWorkers,
  getAgencyRequestsWorkers,
  rejectAgencyRequestWorker,
  updateAgencyRequestWorkerStartDate
} from "@/modules/agency/recruiting/requests/api";
import { WorkerRequestFinishReasonLabels, WorkerRequestStatus, WorkerRequestStatusLabels, WorkerRequestStatusTagTypes } from "@/shared/constants/enums";
import { getWorkersReportDocument } from '@/modules/agency/recruiting/requests/api';
import { dateMonth, hour } from '@/shared/format';
import {
  getAgencyRequestWorkerNotes,
  createAgencyRequestWorkerNote,
  updateAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote
} from "@/modules/agency/shared/notes/api";
import type { AgencyRequestDetail, AgencyRequestWorker, AgencyRequestWorkerFilter } from '@/modules/agency/recruiting/requests/types';
import type { RequestNotesFetchPayload, RequestNotesCreatePayload, RequestNotesUpdatePayload, RequestNotesDeletePayload } from '@/modules/agency/shared/notes/types';
import type { CatalogItem, GridHandle, PaginatedList, TableColumnRef } from '@/shared/types/common';
import type { FinishAssignmentModel } from '@/shared/request-detail/types';
import WorkersList from "@/modules/agency/shared/AgencyWorkersList.vue";
import NotesPopover from "@/modules/agency/shared/notes/NotesPopover.vue";
import EditTextarea from "@/shared/ui/EditTextarea.vue";
import FinishAssignmentModal from "@/shared/request-detail/FinishAssignmentModal.vue";
import PunchCard from '@/modules/agency/recruiting/requests/components/AgencyPunchCardWorkerContainer.vue';
import DatepickerModal from "@/modules/agency/recruiting/requests/components/DatepickerModal.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';

const props = defineProps<{ request: AgencyRequestDetail }>();

const emit = defineEmits<{ (e: 'refreshRequest'): void }>();

const router = useRouter();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  name: 1,
  status: 2,
  startWorking: 3,
  externalId: 6,
};

const isLoading = ref(true);
const statuses = ref<CatalogItem<number>[]>([
  { id: WorkerRequestStatus.Booked, value: WorkerRequestStatusLabels[WorkerRequestStatus.Booked] },
  { id: WorkerRequestStatus.Finished, value: WorkerRequestStatusLabels[WorkerRequestStatus.Finished] },
  { id: WorkerRequestStatus.Rejected, value: WorkerRequestStatusLabels[WorkerRequestStatus.Rejected] },
]);
const statusesSelected = ref<CatalogItem<number>[]>([]);
const startWorkingDatesSelected = ref<Date[]>([]);
const modalManageWorkers = ref(false);
const currentWorker = ref<AgencyRequestWorker | null>(null);
const modalRejectWorker = ref(false);
const modalStartWorking = ref(false);
const modalFinish = ref(false);
const finishAll = ref(false);
const finishSubject = ref('');
const directHiring = computed(() => isDirectHiring(props.request));

const getNotes = ({ requestId, userId, pagination }: RequestNotesFetchPayload) => getAgencyRequestWorkerNotes(requestId, userId, pagination);
const createNote = ({ requestId, userId, model }: RequestNotesCreatePayload) => createAgencyRequestWorkerNote(requestId, userId, model);
const updateNote = ({ requestId, userId, id, model }: RequestNotesUpdatePayload) => updateAgencyRequestWorkerNote(requestId, userId, id, model);
const deleteNote = ({ requestId, userId, id }: RequestNotesDeletePayload) => deleteAgencyRequestWorkerNote(requestId, userId, id);

const serverParams = ref<AgencyRequestWorkerFilter>({
  sortBy: 2,
  requestId: props.request.id,
  isDescending: true,
});

function onCellClick(row: AgencyRequestWorker, column: TableColumnRef) {
  switch (column.field) {
    case 'startWorking':
      onShowModalStartWorking(row);
      break;
    case 'actions':
      break;
    default:
      router.push(`/recruiting/workers/${row.workerProfileId}`);
      break;
  }
}

function onStatusSelected() {
  serverParams.value.statuses = statusesSelected.value.map(ss => ss.id);
  grid.value?.search();
}

function onStartWorkingSelected() {
  serverParams.value.startWorkingFrom = startWorkingDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.startWorkingTo = startWorkingDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onStartWorkingCleared() {
  startWorkingDatesSelected.value = [];
  onStartWorkingSelected();
}

function loadRequestWorkers(params: AgencyRequestWorkerFilter): Promise<PaginatedList<AgencyRequestWorker>> {
  return getAgencyRequestsWorkers(params)
    .then((response) => ({
      ...response,
      items: response.items.map(i => ({
        ...i,
        status: WorkerRequestStatusLabels[i.workerRequestStatus],
        actions: null,
      })),
    }));
}

function confirmDelete(worker: AgencyRequestWorker) {
  currentWorker.value = worker;
  modalRejectWorker.value = true;
}

function rejectWorker(comments: string) {
  modalRejectWorker.value = false;
  isLoading.value = true;
  rejectAgencyRequestWorker(serverParams.value.requestId, currentWorker.value.workerProfileId, { comments }).then(() => {
    isLoading.value = false;
    grid.value?.reload();
    emit('refreshRequest');
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function onShowModalFinish(worker: AgencyRequestWorker) {
  currentWorker.value = worker;
  finishAll.value = false;
  finishSubject.value = worker.name;
  modalFinish.value = true;
}

function onShowModalFinishAll() {
  finishAll.value = true;
  finishSubject.value = 'Every booked worker of this request';
  modalFinish.value = true;
}

function finishAssignment(model: FinishAssignmentModel) {
  modalFinish.value = false;
  isLoading.value = true;
  const requestId = serverParams.value.requestId;
  const call = finishAll.value
    ? finishAllAgencyRequestWorkers(requestId, model)
    : finishAgencyRequestWorker(requestId, currentWorker.value?.workerProfileId ?? '', model);
  call.then(() => {
    isLoading.value = false;
    grid.value?.reload();
    emit('refreshRequest');
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function onShowModalStartWorking(worker: AgencyRequestWorker) {
  currentWorker.value = worker;
  modalStartWorking.value = true;
}

function onUpdateRequestWorkerStartDate(date: Date) {
  modalStartWorking.value = false;
  isLoading.value = true;
  updateAgencyRequestWorkerStartDate(serverParams.value.requestId, currentWorker.value?.id ?? '', { startWorking: date.toISOString() }).then(() => {
    isLoading.value = false;
    grid.value?.reload();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function downloadWorkersReportDocument() {
  isLoading.value = true;
  getWorkersReportDocument(serverParams.value.requestId)
    .then((response) => {
      isLoading.value = false;
      downloadFile(response, `WorkersReport_${serverParams.value.requestId}`);
    })
    .catch((err) => {
      isLoading.value = false;
      showAlertError(err);
    });
}

function downloadPunchCardDocument() {
  isLoading.value = true;
  downloadAgencyReport(`/api/agency/recruiting/requests/${serverParams.value.requestId}/TimeSheets/File`, { ...serverParams.value })
    .then((response) => downloadFile(response, `Timesheet_${new Date().toLocaleDateString()}`))
    .catch((err) => showAlertError(err))
    .finally(() => isLoading.value = false);
}

function onWorkerBooked() {
  modalManageWorkers.value = false;
  grid.value?.reload();
  emit('refreshRequest');
}
</script>

<style scoped>
.worker-cell {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  line-height: 1.3;
}
</style>
