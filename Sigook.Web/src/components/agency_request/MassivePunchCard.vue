
<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <Export :url="getTimeSheetUrl" :params="serverParams" :fileName="'Timesheet'"
        @onDataLoading="(value) => isLoading = value"></Export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        detailed show-detail-icon pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
        detail-transition="fade" default-sort="name" v-model:current-page="serverParams.pageIndex"
        @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
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
          <b-table-column field="totalHoursApproved" label="Approved Hours" sortable v-slot="props">
            {{ hour(props.row.totalHoursApproved) }}
          </b-table-column>
          <b-table-column field="totalHoursWorker" label="Total Hours" sortable v-slot="props">
            {{ hour(props.row.totalHoursWorker) }}
          </b-table-column>
          <b-table-column field="status" label="Status" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="filteredStatuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @typing="filterStatuses" @update:modelValue="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag rounded :type="props.row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">{{ props.row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
            </template>
          </b-table-column>
        </template>
        <template #detail="props">
          <PunchCard :workerId="props.row.workerId" :worker="props.row"
            :requestId="serverParams.requestId" :request="request" />
        </template>
      </b-table>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { hour } from '@/utils/filters';
import { getAgencyRequestsWorkers } from "@/api/agencyRequestApi";
import PunchCard from '@/components/agency_request/AgencyPunchCardWorkerContainer.vue';
import Export from '@/components/Export.vue';

defineProps<{ request: any }>();

const route = useRoute();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const statuses = [
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
];
const statusesSelected = ref<any[]>([]);
const filteredStatuses = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 2,
  requestId: route.params.id,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true
});

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

function filterStatuses(text: string) {
  filteredStatuses.value = statuses.filter(s =>
    !statusesSelected.value.some(ss => ss.id === s.id) &&
    s.value.toLowerCase().includes(text.toLowerCase())
  );
}

function onStatusSelected() {
  filteredStatuses.value = statuses.filter(s =>
    !statusesSelected.value.some(ss => ss.id === s.id)
  );
  serverParams.statuses = statusesSelected.value.map(ss => ss.id);
  loadRequestWorkers();
}

function loadRequestWorkers() {
  isLoading.value = true;
  getAgencyRequestsWorkers(serverParams)
    .then(response => {
      rows.value = response.items.map((i: any) => ({ ...i, actions: null }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

const getTimeSheetUrl = computed(() => `/api/AgencyRequest/${serverParams.requestId}/TimeSheet`);

filteredStatuses.value = statuses;
loadRequestWorkers();
</script>
