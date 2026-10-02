<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>
    <div>
      <SigookGrid ref="grid" :fetch="loadWorkers" v-model:params="serverParams" :sort-map="sortMap" detailed
        show-detail-icon @update:loading="(value) => isLoading = value">
        <template #actions>
          <template v-if="isTouch">
            <b-input v-model="serverParams.name" placeholder="Search name..." icon="magnify"
              @keyup.enter="onSearch"></b-input>
            <div class="filter-trigger">
              <b-button icon-left="filter-variant" @click="showFilters = true" />
              <span v-if="activeFilterCount > 0" class="filter-count-badge">{{ activeFilterCount }}</span>
            </div>
          </template>
        </template>
        <template #dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="downloadTimeSheetDocument">
            <b-icon icon="file-excel"></b-icon>
            <span>Export</span>
          </b-dropdown-item>
        </template>
        <template #mobile-card="{ row }">
          <div class="rcard">
            <div class="rcard__head is-clickable" @click="toggleExpanded(row.workerProfileId)">
              <div class="rcard-worker">
                <img v-if="row.profileImage" :src="row.profileImage" alt="profile image" class="img-30" />
                <default-image v-else :name="row.name" class="img-30"></default-image>
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
                :request="request" :worker="row"></TablePunchCard>
            </div>
          </div>
        </template>
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
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
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.name }}
          </template>
        </b-table-column>
        <b-table-column field="totalHoursApproved" label="Approved Hours" v-slot="props">
          {{ hour(props.row.totalHoursApproved) }}
        </b-table-column>
        <b-table-column field="totalHoursWorker" label="Total Hours" v-slot="props">
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
        <template #detail="props">
          <TablePunchCard :workerProfileId="props.row.workerProfileId" :requestId="serverParams.requestId" :request="request"
            :worker="props.row"></TablePunchCard>
        </template>
      </SigookGrid>
      <MobileFiltersPanel v-if="isTouch" v-model="showFilters" :active-count="activeFilterCount" @apply="onSearch"
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import TablePunchCard from "@/components/company_request/CompanyPunchCardWorkerContainer.vue";
import DataEntryTerms from "@/components/DataEntryTerms.vue";
import MobileFiltersPanel from '@/components/responsive/MobileFiltersPanel.vue';
import SigookGrid from '@/components/SigookGrid.vue';
import { useBreakpoint } from '@/composables/useBreakpoint';
import { showAlertError } from "@/utils/toast";
import { hour } from '@/utils/filters';
import { downloadFile } from '@/utils/downloadFile';
import { getRequestWorkers, getCompanyRequestTimeSheetFile } from '@/api/companyApi';
import { WorkerRequestStatusLabels } from "@/constants/enums";
import type { CompanyRequestWorkerFilter } from '@/types/company';
import type { GridHandle } from '@/types/common';

defineProps<{ request: any }>();

const route = useRoute();
const { isTouch } = useBreakpoint();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  name: 1,
  status: 2,
};

const showFilters = ref(false);
const expandedIds = ref<Set<string>>(new Set());
const isLoading = ref(true);
const statuses = ref([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<{ id: number; value: string }[]>([]);
const serverParams = ref<CompanyRequestWorkerFilter>({
  sortBy: 1,
  requestId: String(route.params.id),
  pageIndex: 1,
  pageSize: 30,
});

function loadWorkers(params: CompanyRequestWorkerFilter) {
  return getRequestWorkers(params)
    .then((response) => ({
      ...response,
      items: response.items.map((i) => ({
        ...i,
        status: WorkerRequestStatusLabels[i.workerRequestStatus],
      })),
    }));
}

function onSearch() {
  grid.value?.search();
}

function onStatusSelected() {
  serverParams.value.statuses = statusesSelected.value.map((ss) => ss.id);
  onSearch();
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
  [serverParams.value.numberId, serverParams.value.name].filter((v: unknown) => !!v).length +
  (statusesSelected.value.length > 0 ? 1 : 0),
);

function clearFilters() {
  serverParams.value.numberId = undefined;
  serverParams.value.name = undefined;
  serverParams.value.statuses = [];
  statusesSelected.value = [];
  onSearch();
}

function downloadTimeSheetDocument() {
  isLoading.value = true;
  getCompanyRequestTimeSheetFile(serverParams.value.requestId)
    .then((response) => {
      isLoading.value = false;
      downloadFile(response, `TimeSheet_${serverParams.value.requestId}`);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
