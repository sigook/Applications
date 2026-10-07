<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="loadWorkers" v-model:params="serverParams" :sort-map="sortMap"
      @update:loading="(value) => isLoading = value">
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
      <template #mobile-card="{ row }">
        <div class="rcard">
          <div class="rcard__head">
            <div class="rcard-worker">
              <img v-if="row.profileImage" :src="row.profileImage" alt="profile image" class="img-30 img-rounded" />
              <default-image v-else :name="row.name" class="img-30"></default-image>
              <div>
                <p class="rcard__title">{{ row.name }}</p>
                <p class="rcard__sub" :class="row.isSubcontractor ? 'Blue' : ''">#{{ row.numberId }}</p>
              </div>
            </div>
            <div class="rcard__actions">
              <span class="is-uppercase has-text-weight-bold fz-1" :class="row.status">{{ row.status }}</span>
              <b-button v-if="row.status === 'Booked'" size="is-small" type="is-danger" outlined rounded
                icon-right="close" @click="confirmDelete(row)"></b-button>
            </div>
          </div>
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">Start Working</span>
              <span>{{ dateMonth(row.startWorking) }}</span>
            </div>
          </div>
        </div>
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
      <b-table-column field="name" label="Name" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
          <span class="is-uppercase has-text-weight-bold fz-1" :class="props.row.status">{{ props.row.status }}</span>
        </template>
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body>
          <b-button size="is-small" type="is-danger" outlined rounded icon-right="close"
            v-if="props.row.status === 'Booked'" @click="confirmDelete(props.row)"></b-button>
        </b-tooltip>
      </b-table-column>
    </SigookGrid>
    <MobileFiltersPanel v-if="isTouch" v-model="showFilters" :active-count="activeFilterCount" @apply="onSearch"
      @clear="clearFilters">
      <b-field label="ID">
        <b-input v-model="serverParams.numberId"></b-input>
      </b-field>
      <b-field label="Name">
        <b-input v-model="serverParams.name"></b-input>
      </b-field>
      <b-field label="Start Working">
        <b-datepicker :mobile-native="false" placeholder="Select range..."
          :icon-right="startWorkingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
          @icon-right-click="onStartWorkingCleared" range v-model="startWorkingDatesSelected"
          @update:modelValue="onStartWorkingSelected" append-to-body>
        </b-datepicker>
      </b-field>
      <b-field label="Status">
        <b-taginput v-model="statusesSelected" autocomplete :data="statuses" open-on-focus field="value" icon="label"
          placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
        </b-taginput>
      </b-field>
    </MobileFiltersPanel>

    <b-modal custom-content-class="card" v-model="modalRejectWorker" width="500px" :destroy-on-hide="true">
      <EditTextarea title="Reject Worker" :min-length="10" class="sm-edit-textarea"
        @updateContent="(data: string) => onRejectWorker(data)" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import EditTextarea from "../../components/agency_request/EditTextarea.vue";
import { showAlertError } from "@/utils/toast";
import { dateMonth } from '@/utils/filters';
import { getRequestWorkers, rejectCompanyRequestWorker } from '@/api/companyApi';
import { WorkerRequestStatusLabels } from '@/constants/enums';
import MobileFiltersPanel from '@/components/responsive/MobileFiltersPanel.vue';
import SigookGrid from '@/components/SigookGrid.vue';
import { useBreakpoint } from '@/composables/useBreakpoint';
import type { CompanyRequestWorkerFilter } from '@/types/company';
import type { GridHandle } from '@/types/common';

const route = useRoute();
const { isTouch } = useBreakpoint();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  name: 1,
  status: 2,
  startWorking: 3,
};

const showFilters = ref(false);
const isLoading = ref(false);
const statuses = ref([
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
]);
const statusesSelected = ref<{ id: number; value: string }[]>([]);
const startWorkingDatesSelected = ref<Date[]>([]);
const modalRejectWorker = ref(false);
const currentWorker = ref<any>(null);
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
        actions: null,
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

function onStartWorkingSelected() {
  serverParams.value.startWorkingFrom = startWorkingDatesSelected.value[0];
  serverParams.value.startWorkingTo = startWorkingDatesSelected.value[1];
  onSearch();
}

function onStartWorkingCleared() {
  startWorkingDatesSelected.value = [];
  onStartWorkingSelected();
}

const activeFilterCount = computed(() =>
  [serverParams.value.numberId, serverParams.value.name].filter((v: unknown) => !!v).length +
  (startWorkingDatesSelected.value.length > 0 ? 1 : 0) +
  (statusesSelected.value.length > 0 ? 1 : 0),
);

function clearFilters() {
  serverParams.value.numberId = undefined;
  serverParams.value.name = undefined;
  serverParams.value.startWorkingFrom = undefined;
  serverParams.value.startWorkingTo = undefined;
  serverParams.value.statuses = [];
  startWorkingDatesSelected.value = [];
  statusesSelected.value = [];
  onSearch();
}

function confirmDelete(worker: any) {
  currentWorker.value = worker;
  modalRejectWorker.value = true;
}

function onRejectWorker(comments: string) {
  modalRejectWorker.value = false;
  isLoading.value = true;
  rejectCompanyRequestWorker(serverParams.value.requestId, currentWorker.value.workerProfileId, { comments })
    .then(() => {
      isLoading.value = false;
      grid.value?.reload();
    }).catch((error: unknown) => {
      isLoading.value = false;
      showAlertError((error as { data?: unknown }).data);
    });
}
</script>
