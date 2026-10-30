<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms v-if="!directHiring"></DataEntryTerms>
    <SigookGrid ref="grid" :fetch="loadWorkers" v-model:params="serverParams" :sort-map="sortMap"
      :fit-viewport="directHiring" :detailed="!directHiring" show-detail-icon
      @update:loading="(value) => isLoading = value">
      <template #actions>
        <template v-if="isTouch">
          <b-input v-model="serverParams.name" placeholder="Search name..." icon="magnify"
            @keyup.enter="onSearch"></b-input>
          <b-badge class="is-flex-shrink-0" :value="activeFilterCount || undefined" type="is-info" rounded>
            <b-button icon-left="filter-variant" @click="showFilters = true" />
          </b-badge>
        </template>
      </template>
      <template v-if="!directHiring" #dropdown-actions>
        <b-dropdown-item aria-role="listitem" @click="downloadTimeSheetDocument">
          <b-icon icon="file-excel"></b-icon>
          <span>Export Punch Card</span>
        </b-dropdown-item>
      </template>
      <template #mobile-card="{ row }">
        <div class="rcard">
          <div class="rcard__head" :class="{ 'is-clickable': !directHiring }" @click="toggleExpanded(row.workerProfileId)">
            <div class="rcard-worker">
              <img v-if="row.profileImage" :src="row.profileImage" alt="profile image" class="img-30 img-rounded" />
              <default-image v-else :name="row.name" class="img-30"></default-image>
              <div>
                <p class="rcard__title">{{ row.name }}</p>
                <p class="rcard__sub" @click.stop><CopyLink kind="email" :value="row.email" /></p>
                <p class="rcard__sub" :class="row.isSubcontractor ? 'Blue' : ''">#{{ row.numberId }}</p>
              </div>
            </div>
            <div class="rcard__actions" @click.stop>
              <b-tag rounded :type="WorkerRequestStatusTagTypes[row.workerRequestStatus]">{{ row.status }}</b-tag>
              <template v-if="row.workerRequestStatus === WorkerRequestStatus.Booked">
                <b-button size="is-small" type="is-primary" outlined rounded icon-right="flag-checkered"
                  @click="onShowModalFinish(row)"></b-button>
                <b-button size="is-small" type="is-danger" outlined rounded icon-right="close"
                  @click="confirmDelete(row)"></b-button>
              </template>
              <b-icon v-if="!directHiring" :icon="expandedIds.has(row.workerProfileId) ? 'chevron-up' : 'chevron-down'"
                @click="toggleExpanded(row.workerProfileId)"></b-icon>
            </div>
          </div>
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">Start Working</span>
              <span>{{ dateMonth(row.startWorking) }}</span>
            </div>
            <div v-if="!directHiring" class="rcard__row">
              <span class="rcard__label">Hours</span>
              <span>{{ hour(row.totalHoursApproved) }} of {{ hour(row.totalHoursWorker) }}</span>
            </div>
          </div>
          <div v-if="!directHiring && expandedIds.has(row.workerProfileId)" class="rcard-detail">
            <TablePunchCard :workerProfileId="row.workerProfileId" :requestId="serverParams.requestId"
              :request="request" :worker="row"></TablePunchCard>
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
          <b-input v-model="serverParams.name" placeholder="Name, email or phone" icon="magnify"
            size="is-small"></b-input>
        </template>
        <template v-slot="props">
          {{ props.row.name }}
          <span class="fz-1 block"><CopyLink kind="email" :value="props.row.email" /></span>
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
      <b-table-column v-if="!directHiring" field="hours" label="Hours" cell-class="is-nowrap" v-slot="props">
        {{ hour(props.row.totalHoursApproved) }}
        <span class="fz-1 block">of {{ hour(props.row.totalHoursWorker) }}</span>
      </b-table-column>
      <b-table-column field="status" label="Status" sortable searchable>
        <template v-slot:searchable>
          <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
            field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
          </b-taginput>
        </template>
        <template v-slot="props">
          <b-tag rounded :type="WorkerRequestStatusTagTypes[props.row.workerRequestStatus]">{{ props.row.status }}</b-tag>
        </template>
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-field v-if="props.row.workerRequestStatus === WorkerRequestStatus.Booked" grouped>
          <b-tooltip label="End assignment" type="is-dark" position="is-top" append-to-body>
            <b-button size="is-small" type="is-primary" outlined rounded icon-right="flag-checkered"
              @click="onShowModalFinish(props.row)"></b-button>
          </b-tooltip>
          <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body class="ml-2">
            <b-button size="is-small" type="is-danger" outlined rounded icon-right="close"
              @click="confirmDelete(props.row)"></b-button>
          </b-tooltip>
        </b-field>
      </b-table-column>
      <template v-if="!directHiring" #detail="props">
        <TablePunchCard :workerProfileId="props.row.workerProfileId" :requestId="serverParams.requestId"
          :request="request" :worker="props.row"></TablePunchCard>
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

    <b-modal custom-content-class="card" v-model="modalFinish" width="560px" destroy-on-hide>
      <FinishAssignmentModal :subject="currentWorker?.name ?? ''" :start-working="currentWorker?.startWorking"
        @submit="onFinishWorker" @close="modalFinish = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import EditTextarea from "@/shared/ui/EditTextarea.vue";
import { showAlertError } from "@/shared/utils/toast";
import { downloadFile } from '@/shared/utils/downloadFile';
import { dateMonth, hour } from '@/shared/format';
import { isDirectHiring } from '@/shared/punch-card/directHiring';
import {
  finishCompanyRequestWorker,
  getCompanyRequestTimeSheetFile,
  getRequestWorkers,
  rejectCompanyRequestWorker
} from '@/modules/company/requests/api';
import {
  WorkerRequestStatus,
  WorkerRequestStatusLabels,
  WorkerRequestStatusOptions,
  WorkerRequestStatusTagTypes
} from '@/shared/constants/enums';
import FinishAssignmentModal from '@/shared/request-detail/FinishAssignmentModal.vue';
import type { FinishAssignmentModel } from '@/shared/request-detail/types';
import TablePunchCard from '@/modules/company/requests/components/CompanyPunchCardWorkerContainer.vue';
import DataEntryTerms from '@/shared/punch-card/DataEntryTerms.vue';
import MobileFiltersPanel from '@/shared/ui/MobileFiltersPanel.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';
import { useBreakpoint } from '@/shared/composables/useBreakpoint';
import type { CompanyRequestDetail, CompanyRequestWorker, CompanyRequestWorkerFilter } from '@/modules/company/requests/types';
import type { GridHandle } from '@/shared/types/common';

const props = defineProps<{ request: CompanyRequestDetail }>();

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
const expandedIds = ref<Set<string>>(new Set());
const statuses = ref(WorkerRequestStatusOptions);
const statusesSelected = ref<{ id: number; value: string }[]>([]);
const startWorkingDatesSelected = ref<Date[]>([]);
const modalRejectWorker = ref(false);
const modalFinish = ref(false);
const currentWorker = ref<CompanyRequestWorker | null>(null);
const directHiring = computed(() => isDirectHiring(props.request));
const serverParams = ref<CompanyRequestWorkerFilter>({
  sortBy: 1,
  requestId: props.request.id,
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

function toggleExpanded(workerProfileId: string) {
  if (directHiring.value) return;
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

function confirmDelete(worker: CompanyRequestWorker) {
  currentWorker.value = worker;
  modalRejectWorker.value = true;
}

function onShowModalFinish(worker: CompanyRequestWorker) {
  currentWorker.value = worker;
  modalFinish.value = true;
}

function onFinishWorker(model: FinishAssignmentModel) {
  modalFinish.value = false;
  isLoading.value = true;
  finishCompanyRequestWorker(serverParams.value.requestId, currentWorker.value?.workerProfileId ?? '', model)
    .then(() => {
      isLoading.value = false;
      grid.value?.reload();
    }).catch((error: unknown) => {
      isLoading.value = false;
      showAlertError((error as { data?: unknown }).data);
    });
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
