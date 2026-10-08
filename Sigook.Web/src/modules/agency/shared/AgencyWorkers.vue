<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="loadRequestWorkers" v-model:params="serverParams" :sort-map="sortMap" focusable
        @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
        <template #actions>
          <b-button icon-left="plus" @click="modalManageWorkers = true">Manage Workers</b-button>
        </template>
        <template #dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="downloadWorkersReportDocument">
            <b-icon icon="file-excel"></b-icon>
            <span>Export</span>
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
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.name }}
          </template>
        </b-table-column>
        <b-table-column field="mobileNumber" label="Phone" searchable cell-class="is-nowrap">
          <template v-slot:searchable>
            <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
              @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
          </template>
          <template v-slot="props">{{ props.row.mobileNumber }}</template>
        </b-table-column>
        <b-table-column field="socialInsurance" label="SIN/SSN" searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.socialInsurance" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
              <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"></b-input>
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
              <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.rejectedBy"></b-input>
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
            <span class="is-uppercase has-text-weight-bold fz-1" :class="props.row.status">{{ props.row.status }}</span>
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

    <b-modal custom-content-class="card" v-model="modalStartWorking" width="415px">
      <datepicker-modal v-if="currentWorker" v-model:start-working="currentWorker.startWorking"
        @onSelectCalendar="(date) => onUpdateRequestWorkerStartDate(date)" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from "@/shared/utils/toast";
import { downloadFile } from "@/shared/utils/downloadFile";
import { formatPhone } from '@/shared/utils/phoneFormat';
import {
  getAgencyRequestsWorkers,
  rejectAgencyRequestWorker,
  updateAgencyRequestWorkerStartDate
} from "@/modules/agency/recruiting/requests/api";
import { WorkerRequestStatusLabels } from "@/shared/constants/enums";
import { getWorkersReportDocument } from '@/modules/agency/recruiting/requests/api';
import { dateMonth, emailName } from '@/shared/format';
import {
  getAgencyRequestWorkerNotes,
  createAgencyRequestWorkerNote,
  updateAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote
} from "@/modules/agency/shared/notes/api";
import type { AgencyRequestWorker, AgencyRequestWorkerFilter } from '@/modules/agency/recruiting/requests/types';
import type { RequestNotesFetchPayload, RequestNotesCreatePayload, RequestNotesUpdatePayload, RequestNotesDeletePayload } from '@/modules/agency/shared/notes/types';
import type { CatalogItem, GridHandle, PaginatedList, TableColumnRef } from '@/shared/types/common';
import WorkersList from "@/modules/agency/shared/AgencyWorkersList.vue";
import NotesPopover from "@/modules/agency/shared/notes/NotesPopover.vue";
import EditTextarea from "@/shared/ui/EditTextarea.vue";
import DatepickerModal from "@/modules/agency/recruiting/requests/components/DatepickerModal.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{
  id?: string;
  showTitle?: boolean;
}>();

const emit = defineEmits<{ (e: 'refreshRequest'): void }>();

const route = useRoute();
const router = useRouter();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  name: 1,
  status: 2,
  startWorking: 3,
  createdBy: 4,
  rejectedBy: 5,
  externalId: 6,
};

const isLoading = ref(true);
const statuses = ref<CatalogItem<number>[]>([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<CatalogItem<number>[]>([]);
const startWorkingDatesSelected = ref<Date[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const rejectedAtDatesSelected = ref<Date[]>([]);
const modalManageWorkers = ref(false);
const currentWorker = ref<AgencyRequestWorker | null>(null);
const modalRejectWorker = ref(false);
const modalStartWorking = ref(false);

const getNotes = ({ requestId, userId, pagination }: RequestNotesFetchPayload) => getAgencyRequestWorkerNotes(requestId, userId, pagination);
const createNote = ({ requestId, userId, model }: RequestNotesCreatePayload) => createAgencyRequestWorkerNote(requestId, userId, model);
const updateNote = ({ requestId, userId, id, model }: RequestNotesUpdatePayload) => updateAgencyRequestWorkerNote(requestId, userId, id, model);
const deleteNote = ({ requestId, userId, id }: RequestNotesDeletePayload) => deleteAgencyRequestWorkerNote(requestId, userId, id);

const serverParams = ref<AgencyRequestWorkerFilter>({
  sortBy: 2,
  requestId: (props.id || route.params.id) as string,
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

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onRejectedAtSelected() {
  serverParams.value.rejectedAtFrom = rejectedAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.rejectedAtTo = rejectedAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onRejectedAtCleared() {
  rejectedAtDatesSelected.value = [];
  onRejectedAtSelected();
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

function onWorkerBooked() {
  modalManageWorkers.value = false;
  grid.value?.reload();
  emit('refreshRequest');
}
</script>
