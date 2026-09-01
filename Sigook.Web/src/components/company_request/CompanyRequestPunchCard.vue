<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>
    <div>
      <b-field grouped position="is-right">
        <b-button size="is-small" type="is-ghost" icon-right="file-excel"
          @click="downloadTimeSheetDocument">Export</b-button>
      </b-field>
      <template v-if="isMobile">
        <div class="mobile-list-toolbar">
          <b-field>
            <b-input v-model="serverParams.name" placeholder="Search name..." icon="magnify" expanded
              @keypress="onInputEntered"></b-input>
          </b-field>
          <div class="filter-trigger">
            <b-button icon-left="filter-variant" @click="showFilters = true" />
            <span v-if="activeFilterCount > 0" class="filter-count-badge">{{ activeFilterCount }}</span>
          </div>
        </div>
        <div class="rcard-list">
          <div v-for="row in rows" :key="row.workerProfileId" class="rcard">
            <div class="rcard__head is-clickable" @click="toggleExpanded(row.workerProfileId)">
              <div class="rcard-worker">
                <img v-if="row.profileImage" :src="row.profileImage" alt="profile image" class="img-30" />
                <default-image v-else :name="row.fullName" class="img-30"></default-image>
                <div>
                  <p class="rcard__title">{{ row.name }}</p>
                  <p class="rcard__sub" :class="row.isSubcontractor ? 'Blue' : ''">#{{ row.numberId }}</p>
                </div>
              </div>
              <div class="rcard__actions">
                <b-tag rounded :type="row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">
                  {{ row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
                <b-icon :icon="expandedIds.has(row.workerProfileId) ? 'chevron-up' : 'chevron-down'"></b-icon>
              </div>
            </div>
            <div class="rcard__rows">
              <div class="rcard__row">
                <span class="rcard__label">Approved Hours</span>
                <span>{{ hour(row.totalHoursApproved) }}</span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Total Hours</span>
                <span>{{ hour(row.totalHoursWorker) }}</span>
              </div>
            </div>
            <div v-if="expandedIds.has(row.workerProfileId)" class="rcard-detail">
              <TablePunchCard :workerProfileId="row.workerProfileId" :requestId="serverParams.requestId"
                :request="request" :worker="row.worker"></TablePunchCard>
            </div>
          </div>
          <p v-if="rows.length === 0" class="has-text-centered">No records available</p>
        </div>
        <b-pagination v-model="serverParams.pageIndex" :total="totalItems" :per-page="serverParams.pageSize"
          size="is-small" rounded class="mt-4" @change="onPageChange" />
        <MobileFiltersPanel v-model="showFilters" :active-count="activeFilterCount" @apply="getWorkers"
          @clear="clearFilters">
          <b-field label="ID">
            <b-input v-model="serverParams.numberId"></b-input>
          </b-field>
          <b-field label="Name">
            <b-input v-model="serverParams.name"></b-input>
          </b-field>
          <b-field label="Status">
            <b-taginput v-model="statusesSelected" autocomplete :data="statuses" open-on-focus field="value"
              icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
            </b-taginput>
          </b-field>
        </MobileFiltersPanel>
      </template>
      <b-table v-else sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        detailed show-detail-icon pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
        detail-transition="fade" default-sort="name" v-model:current-page="serverParams.pageIndex"
        @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container has-text-centered">No records available</p>
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
import { ref, reactive, computed } from 'vue';
import { useRoute } from 'vue-router';
import TablePunchCard from "@/components/company_request/CompanyPunchCardWorkerContainer.vue";
import DataEntryTerms from "@/components/DataEntryTerms.vue";
import MobileFiltersPanel from '@/components/responsive/MobileFiltersPanel.vue';
import { useBreakpoint } from '@/composables/useBreakpoint';
import { showAlertError } from "@/utils/toast";
import { hour } from '@/utils/filters';
import { downloadFile } from '@/utils/downloadFile';
import { getRequestWorkers, getCompanyRequestTimeSheetFile } from '@/api/companyApi';
import { WorkerRequestStatusLabels } from "@/constants/enums";

defineProps<{ request: any }>();

const route = useRoute();
const { isMobile } = useBreakpoint();

const showFilters = ref(false);
const expandedIds = ref<Set<string>>(new Set());
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

function toggleExpanded(workerProfileId: string) {
  const next = new Set(expandedIds.value);
  if (next.has(workerProfileId)) {
    next.delete(workerProfileId);
  } else {
    next.add(workerProfileId);
  }
  expandedIds.value = next;
}

const activeFilterCount = computed(() =>
  [serverParams.numberId, serverParams.name].filter((v: unknown) => !!v).length +
  (statusesSelected.value.length > 0 ? 1 : 0),
);

function clearFilters() {
  serverParams.numberId = undefined;
  serverParams.name = undefined;
  serverParams.statuses = [];
  statusesSelected.value = [];
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
