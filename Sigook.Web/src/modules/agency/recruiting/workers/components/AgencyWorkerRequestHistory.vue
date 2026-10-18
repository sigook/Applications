<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="getAgencyWorkerProfileRequestHistory" v-model:params="serverParams" :sort-map="sortMap"
      focusable @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
      <b-table-column field="numberId" label="ID" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          <div class="request-id-cell">
            <div v-if="props.row.isAsap" class="request-flags">
              <span class="request-flag request-flag--asap">Asap</span>
            </div>
            <router-link :to="{ path: requestBase + '/' + props.row.id }">
              <p>{{ props.row.numberId }}</p>
            </router-link>
          </div>
        </template>
      </b-table-column>
      <b-table-column field="companyFullName" label="Client" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          <router-link :to="{ path: companyBase + '/' + props.row.companyProfileId }">
            {{ props.row.companyFullName }}
          </router-link>
        </template>
      </b-table-column>
      <b-table-column field="jobTitle" label="Position" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">{{ props.row.jobTitle }}</template>
      </b-table-column>
      <b-table-column field="location" label="Location" searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          {{ props.row.location }}
          <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
        </template>
      </b-table-column>
      <b-table-column field="startWorking" label="Start" sortable searchable>
        <template v-slot:searchable>
          <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
            :icon-right="startWorkingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
            @icon-right-click="onStartWorkingCleared" range v-model="startWorkingDatesSelected"
            @update:modelValue="onStartWorkingSelected" append-to-body>
          </b-datepicker>
        </template>
        <template v-slot="props">
          <span v-if="props.row.startWorking">{{ dateMonth(props.row.startWorking) }}</span>
          <span v-else class="op3">—</span>
        </template>
      </b-table-column>
      <b-table-column field="finishWorking" label="Finish" sortable v-slot="props">
        <span v-if="props.row.finishWorking">{{ dateMonth(props.row.finishWorking) }}</span>
        <span v-else class="op3">—</span>
      </b-table-column>
      <b-table-column field="workerRequestStatus" label="Worker" searchable>
        <template v-slot:searchable>
          <b-taginput size="is-small" v-model="workerStatusesSelected" autocomplete :data="workerStatuses" open-on-focus
            field="value" icon="account" placeholder="Select Status" @update:modelValue="onWorkerStatusChange" append-to-body>
          </b-taginput>
        </template>
        <template v-slot="props">
          <b-tag :type="props.row.workerRequestStatus === WorkerRequestStatus.Booked ? 'is-success is-light' : 'is-danger is-light'" rounded>
            {{ WorkerRequestStatusLabels[props.row.workerRequestStatus] }}
          </b-tag>
        </template>
      </b-table-column>
      <b-table-column field="requestStatus" label="Status" searchable>
        <template v-slot:searchable>
          <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
            field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange" append-to-body>
          </b-taginput>
        </template>
        <template v-slot="props">
          <div class="has-text-centered">
            <b-tooltip :label="RequestStatusLabels[props.row.requestStatus]" type="is-dark" append-to-body>
              <div class="status-dot-container">
                <b-icon v-if="props.row.requestStatus === RequestStatus.Filled" icon="check" size="is-small"
                  type="is-white" class="request-check"></b-icon>
                <div class="dot-status" :class="'status-' + RequestStatusLabels[props.row.requestStatus].toLowerCase()"></div>
              </div>
            </b-tooltip>
          </div>
        </template>
      </b-table-column>
    </SigookGrid>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { dateMonth } from '@/shared/format';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import { getAgencyWorkerProfileRequestHistory } from '@/modules/agency/recruiting/workers/api';
import type { WorkerRequestHistoryFilter, WorkerRequestHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { CatalogItem, GridHandle, TableColumnRef } from '@/shared/types/common';
import { RequestStatus, RequestStatusLabels, WorkerRequestStatus, WorkerRequestStatusLabels } from '@/shared/constants/enums';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ workerId: string }>();

const router = useRouter();
const { requestBase, companyBase } = useModuleBase();
const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  numberId: 0,
  companyFullName: 1,
  jobTitle: 2,
  startWorking: 3,
  finishWorking: 4,
  createdAt: 5,
};
const statuses: CatalogItem<RequestStatus>[] = [
  { id: RequestStatus.Open, value: RequestStatusLabels[RequestStatus.Open] },
  { id: RequestStatus.Filled, value: RequestStatusLabels[RequestStatus.Filled] },
  { id: RequestStatus.Cancelled, value: RequestStatusLabels[RequestStatus.Cancelled] },
];
const workerStatuses: CatalogItem<WorkerRequestStatus>[] = [
  { id: WorkerRequestStatus.Booked, value: WorkerRequestStatusLabels[WorkerRequestStatus.Booked] },
  { id: WorkerRequestStatus.Rejected, value: WorkerRequestStatusLabels[WorkerRequestStatus.Rejected] },
];

const isLoading = ref(false);
const statusesSelected = ref<CatalogItem<RequestStatus>[]>([]);
const workerStatusesSelected = ref<CatalogItem<WorkerRequestStatus>[]>([]);
const startWorkingDatesSelected = ref<Date[]>([]);
const serverParams = ref<WorkerRequestHistoryFilter>({
  profileId: props.workerId,
  sortBy: 0,
  isDescending: true,
});

function onCellClick(row: WorkerRequestHistoryItem, column: TableColumnRef) {
  if (column.field === 'companyFullName') return;
  router.push(`${requestBase.value}/${row.id}`);
}

function onStatusChange() {
  serverParams.value.statuses = statusesSelected.value.map((s) => s.id);
  grid.value?.search();
}

function onWorkerStatusChange() {
  serverParams.value.workerRequestStatuses = workerStatusesSelected.value.map((s) => s.id);
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
</script>

<style scoped lang="scss">
.request-id-cell {
  position: relative;
  padding-top: 14px;
}

.request-flags {
  position: absolute;
  top: -8px;
  left: -10px;
  z-index: 1;
}

.request-flag {
  display: inline-block;
  padding: 2px 12px 2px 6px;
  font-size: 10px;
  font-weight: 700;
  line-height: 1;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  clip-path: polygon(0 0, calc(100% - 6px) 0, 100% 50%, calc(100% - 6px) 100%, 0 100%);

  &--asap {
    background: #ff9932;
    color: #1e1e1e;
  }
}
</style>
