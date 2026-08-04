<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>
    <div>
      <b-field grouped position="is-right">
        <b-button size="is-small" type="is-ghost" icon-right="file-excel"
          @click="downloadTimeSheetDocument">Export</b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
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
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag rounded :type="props.row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">{{ props.row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
            </template>
          </b-table-column>
        </template>
        <template #detail="props">
          <TablePunchCard :workerProfileId="props.row.workerProfileId" :requestId="serverParams.requestId" :request="request"
            :worker="props.row.worker"></TablePunchCard>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute } from 'vue-router';
import TablePunchCard from "@/components/company_request/CompanyPunchCardWorkerContainer.vue";
import DataEntryTerms from "@/components/DataEntryTerms.vue";
import { showAlertError } from "@/utils/toast";
import { hour } from '@/utils/filters';
import { downloadFile } from '@/utils/downloadFile';
import { getRequestWorkers, getCompanyRequestTimeSheetFile } from '@/api/companyApi';
import { WorkerRequestStatusLabels } from "@/constants/enums";

defineProps<{ request: any }>();

const route = useRoute();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const statuses = ref([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<any[]>([]);
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

function getWorkers() {
  isLoading.value = true;
  getRequestWorkers(serverParams)
    .then((response: any) => {
      rows.value = response.items.map((i: any) => ({
        ...i,
        status: WorkerRequestStatusLabels[i.workerRequestStatus],
      }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      showAlertError((error as { data?: unknown }).data);
      isLoading.value = false;
    });
}

function downloadTimeSheetDocument() {
  isLoading.value = true;
  getCompanyRequestTimeSheetFile(serverParams.requestId)
    .then((response: any) => {
      isLoading.value = false;
      downloadFile(response, `TimeSheet_${serverParams.requestId}`);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

getWorkers();
</script>
