<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button type="is-primary" icon-left="plus" @click="modalManageWorkers = true">
          {{ "Manage Workers" }}
        </b-button>
        <b-button type="is-ghost" icon-right="file-excel"
          @click="downloadWorkersReportDocument">Export</b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="name"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image"
              class="img-30 img-rounded" />
            <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="numberId" label="ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
          </b-table-column>
          <b-table-column field="externalId" label="External ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.externalId }}</template>
          </b-table-column>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.name }}
            </template>
          </b-table-column>
          <b-table-column field="mobileNumber" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered" @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
            </template>
            <template v-slot="props">{{ props.row.mobileNumber }}</template>
          </b-table-column>
          <b-table-column field="socialInsurance" label="SIN/SSN" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.socialInsurance" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.socialInsurance">
                {{ props.row.socialInsurance }}
                <i class="fz-2 block">{{ dateMonth(props.row.dueDate) }}</i>
              </div>
              <span v-else class="op3">SIN/SNN</span>
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
          <b-table-column field="createdBy" label="Created By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"
                  @keypress="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                  @update:modelValue="onCreatedAtSelected" append-to-body></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              {{ emailName(props.row.createdBy) }}
              <i class="fz-2 block">{{ dateMonth(props.row.createdAt) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="rejectedBy" label="Rejected By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.rejectedBy"
                  @keypress="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="rejectedAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="rejectedAtDatesSelected" icon-right-clickable @icon-right-click="onRejectedAtCleared"
                  @update:modelValue="onRejectedAtSelected" append-to-body></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div v-if="props.row.rejectedBy">
                {{ emailName(props.row.rejectedBy) }}
                <i class="fz-2 block">{{ dateMonth(props.row.rejectedAt) }}</i>
              </div>
              <span v-else class="op3">Rejected by</span>
            </template>
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
              <span class="text-uppercase fw-bold fz-1" :class="props.row.status">{{ props.row.status }}</span>
              <i class="fz-1 block" v-html="props.row.rejectComments"></i>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-danger" outlined rounded icon-right="close"
                  v-if="props.row.status === 'Booked'" @click="confirmDelete(props.row)"></b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <!-- custom modal Manage Workers-->
    <b-modal v-model="modalManageWorkers" width="500px">
      <workers-list :request-id="serverParams.requestId" @workerBooked="onWorkerBooked"></workers-list>
    </b-modal>

    <b-modal v-model="modalRejectWorker" width="500px">
      <edit-textarea title="Reject Worker" :subtitle="'Please indicate the reason.'" :min-length="10"
        class="sm-edit-textarea" @updateContent="(data) => rejectWorker(data)" />
    </b-modal>

    <b-modal v-model="modalStartWorking" width="415px">
      <datepicker-modal v-if="currentWorker" v-model:start-working="currentWorker.startWorking"
        @onSelectCalendar="(date) => onUpdateRequestWorkerStartDate(date)" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { downloadFile } from "@/utils/downloadFile";
import { formatPhone } from '@/utils/phoneFormat';
import {
  getAgencyRequestsWorkers,
  rejectAgencyRequestWorker,
  updateAgencyRequestWorkerStartDate
} from "@/api/agencyRequestApi";
import { WorkerRequestStatusLabels } from "@/constants/enums";
import { getWorkersReportDocument } from "@/api/agencyReportApi";
import { dateMonth, emailName } from '@/utils/filters';
import {
  getAgencyRequestWorkerNotes,
  createAgencyRequestWorkerNote,
  updateAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote
} from "@/api/agencyNoteApi";
import type { RequestNotesFetchPayload, RequestNotesCreatePayload, RequestNotesUpdatePayload, RequestNotesDeletePayload } from '@/types/agency';
import WorkersList from "./AgencyWorkersList.vue";
import NotesPopover from "../../components/notes/NotesPopover.vue";
import EditTextarea from "../../components/agency_request/EditTextarea.vue";
import DatepickerModal from "@/components/agency_request/DatepickerModal.vue";

const props = defineProps<{
  request?: any;
  id?: any;
  showTitle?: boolean;
}>();

const emit = defineEmits<{ (e: 'refreshRequest'): void }>();

const route = useRoute();
const router = useRouter();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const statuses = ref<any[]>([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<any[]>([]);
const startWorkingDatesSelected = ref<any[]>([]);
const createdAtDatesSelected = ref<any[]>([]);
const rejectedAtDatesSelected = ref<any[]>([]);
const modalManageWorkers = ref(false);
const currentWorker = ref<any>(null);
const modalRejectWorker = ref(false);
const modalStartWorking = ref(false);

const getNotes = ({ requestId, userId, pagination }: RequestNotesFetchPayload) => getAgencyRequestWorkerNotes(requestId, userId, pagination);
const createNote = ({ requestId, userId, model }: RequestNotesCreatePayload) => createAgencyRequestWorkerNote(requestId, userId, model);
const updateNote = ({ requestId, userId, id, model }: RequestNotesUpdatePayload) => updateAgencyRequestWorkerNote(requestId, userId, id, model);
const deleteNote = ({ requestId, userId, id }: RequestNotesDeletePayload) => deleteAgencyRequestWorkerNote(requestId, userId, id);

const serverParams = reactive<any>({
  sortBy: 2,
  requestId: props.id || route.params.id,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true,
});

function onCellClick(row: any, column: any) {
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

function onPageChange(params: any) {
  serverParams.pageIndex = params;
  loadRequestWorkers();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'numberId':
      serverParams.sortBy = 0;
      break;
    case 'name':
      serverParams.sortBy = 1;
      break;
    case 'status':
      serverParams.sortBy = 2;
      break;
    case 'startWorking':
      serverParams.sortBy = 3;
      break;
    case 'createdBy':
      serverParams.sortBy = 4;
      break;
    case 'rejectedBy':
      serverParams.sortBy = 5;
      break;
    case 'externalId':
      serverParams.sortBy = 6;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  loadRequestWorkers();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadRequestWorkers();
  }
}

function onStatusSelected() {
  serverParams.statuses = statusesSelected.value.map(ss => ss.id);
  loadRequestWorkers();
}

function onStartWorkingSelected() {
  serverParams.startWorkingFrom = startWorkingDatesSelected.value[0];
  serverParams.startWorkingTo = startWorkingDatesSelected.value[1];
  loadRequestWorkers();
}

function onStartWorkingCleared() {
  startWorkingDatesSelected.value = [];
  onStartWorkingSelected();
}

function onCreatedAtSelected() {
  serverParams.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.createdAtTo = createdAtDatesSelected.value[1];
  loadRequestWorkers();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onRejectedAtSelected() {
  serverParams.rejectedAtFrom = rejectedAtDatesSelected.value[0];
  serverParams.rejectedAtTo = rejectedAtDatesSelected.value[1];
  loadRequestWorkers();
}

function onRejectedAtCleared() {
  rejectedAtDatesSelected.value = [];
  onRejectedAtSelected();
}

function loadRequestWorkers() {
  isLoading.value = true;
  getAgencyRequestsWorkers(serverParams)
    .then((response) => {
      rows.value = response.items.map(i => ({
        ...i,
        status: WorkerRequestStatusLabels[i.workerRequestStatus],
        actions: null,
      }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function confirmDelete(worker: any) {
  currentWorker.value = worker;
  modalRejectWorker.value = true;
}

function rejectWorker(comments: string) {
  modalRejectWorker.value = false;
  isLoading.value = true;
  rejectAgencyRequestWorker(serverParams.requestId, currentWorker.value.workerId, { comments }).then(() => {
    isLoading.value = false;
    loadRequestWorkers();
    emit('refreshRequest');
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function onShowModalStartWorking(worker: any) {
  currentWorker.value = worker;
  modalStartWorking.value = true;
}

function onUpdateRequestWorkerStartDate(date: any) {
  modalStartWorking.value = false;
  isLoading.value = true;
  updateAgencyRequestWorkerStartDate(serverParams.requestId, currentWorker.value.id, { startWorking: date }).then(() => {
    isLoading.value = false;
    loadRequestWorkers();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function downloadWorkersReportDocument() {
  isLoading.value = true;
  getWorkersReportDocument(serverParams.requestId)
    .then((response) => {
      isLoading.value = false;
      downloadFile(response, `WorkersReport_${serverParams.requestId}`);
    })
    .catch((err) => {
      isLoading.value = false;
      showAlertError(err);
    });
}

function onWorkerBooked() {
  modalManageWorkers.value = false;
  loadRequestWorkers();
  // Emit event to refresh request status (Open/Filled state may have changed)
  emit('refreshRequest');
}

loadRequestWorkers();
</script>
