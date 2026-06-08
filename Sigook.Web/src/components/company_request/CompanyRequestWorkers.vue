<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      v-model:current-page="serverParams.pageIndex" default-sort="name" @page-change="onPageChange" @sort="onSortChange">
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
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.name }}
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
            {{ dateMonth(props.row.startWorking) }}
          </template>
        </b-table-column>
        <b-table-column field="status" label="Status" sortable searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <span class="uppercase fw-700 fz-1" :class="props.row.status">{{ props.row.status }}</span>
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body>
            <b-button size="is-small" type="is-danger" outlined rounded icon-right="close"
              v-if="props.row.status === 'Booked'" @click="confirmDelete(props.row)"></b-button>
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>

    <!-- Reject worker Message -->
    <transition name="modal">
      <div v-if="modalRejectWorker" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container border-radius">
              <button @click="modalRejectWorker = false" class="cross-icon">{{ 'Close' }}</button>
              <EditTextarea title="Reject Worker" :subtitle="'Please indicate the reason.'" :min-length="10"
                class="sm-edit-textarea" @updateContent="(data: string) => onRejectWorker(data)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- Reject worker Message -->
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute } from 'vue-router';
import EditTextarea from "../../components/agency_request/EditTextarea.vue";
import { showAlertError } from "@/utils/toast";
import { dateMonth } from '@/utils/filters';
import { getRequestWorkers, rejectCompanyRequestWorker } from '@/api/companyApi';
import { WorkerRequestStatusLabels } from '@/constants/enums';

const route = useRoute();

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const statuses = ref([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<any[]>([]);
const startWorkingDatesSelected = ref<any[]>([]);
const modalRejectWorker = ref(false);
const currentWorker = ref<any>(null);
const serverParams = reactive<any>({
  sortBy: 1,
  requestId: route.params.id,
  pageIndex: 1,
  pageSize: 30,
});

function onPageChange(params: number) {
  serverParams.pageIndex = params;
  getWorkers();
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
  }
  serverParams.isDescending = order !== 'asc';
  getWorkers();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    getWorkers();
  }
}

function onStatusSelected() {
  serverParams.statuses = statusesSelected.value.map((ss: any) => ss.id);
  getWorkers();
}

function onStartWorkingSelected() {
  serverParams.startWorkingFrom = startWorkingDatesSelected.value[0];
  serverParams.startWorkingTo = startWorkingDatesSelected.value[1];
  getWorkers();
}

function onStartWorkingCleared() {
  startWorkingDatesSelected.value = [];
  onStartWorkingSelected();
}

function getWorkers() {
  isLoading.value = true;
  getRequestWorkers(serverParams)
    .then((response: any) => {
      rows.value = response.items.map((i: any) => ({
        ...i,
        status: WorkerRequestStatusLabels[i.workerRequestStatus],
        actions: null,
      }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function confirmDelete(worker: any) {
  currentWorker.value = worker;
  modalRejectWorker.value = true;
}

function onRejectWorker(comments: string) {
  modalRejectWorker.value = false;
  isLoading.value = true;
  rejectCompanyRequestWorker(serverParams.requestId, currentWorker.value.workerId, { comments })
    .then(() => {
      isLoading.value = false;
      getWorkers();
    }).catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

getWorkers();
</script>
