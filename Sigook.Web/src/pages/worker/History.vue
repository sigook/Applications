<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="fz1 pt-3">{{ "History" }}</h2>
    <div>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="numberId"
        v-model:current-page="serverParams.pageIndex">
        <template v-slot:empty>
          <p class="container has-text-centered">No records available</p>
        </template>
        <template>
          <b-table-column field="numberId" label="Request ID" v-slot="props">
            {{ props.row.numberId }}
          </b-table-column>
          <b-table-column field="jobTitle" label="Position" v-slot="props">
            {{ props.row.jobTitle }}
          </b-table-column>
          <b-table-column field="location" label="Location" v-slot="props">
            {{ props.row.location }}
            <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
          </b-table-column>
          <b-table-column field="startAt">
            <template v-slot:header>
              <p class="has-text-weight-semibold">Duration</p>
              <p class="has-text-weight-semibold">(Start - End)</p>
            </template>
            <template v-slot="props">
              {{ dateMonth(props.row.startAt) }}
              <span v-if="props.row.durationTerm !== appGlobals.$longTerm">
                - {{ dateMonth(props.row.finishAt) }}
              </span>
              <span
                v-if="(props.row.status === appGlobals.$statusFilled || props.row.status === appGlobals.$statusCancelled) && props.row.durationTerm === appGlobals.$longTerm">
                - {{ dateMonth(props.row.finishAt) }}
              </span>
              <i class="fz-2 block">{{ splitCapital(props.row.durationTerm) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="workerRate" label="Rate / Salary" v-slot="props">
            {{ currency(props.row.workerRate || props.row.workerSalary) }}
          </b-table-column>
          <b-table-column field="workersQuantity" label="Spots" v-slot="props">
            {{ props.row.workersQuantity }}
          </b-table-column>
          <b-table-column field="status" v-slot="props">
            <div v-if="props.row.status && props.row.status !== 'None'" class="capitailized has-text-weight-bold has-text-centered"
              :class="props.row.status">
              {{ props.row.status }}
            </div>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from '@/utils/toast';
import { getWorkerRequestHistory } from '@/api/workerApi';
import type { WorkerRequestFilter, WorkerRequestListItem } from '@/types/worker';
import { dateMonth, splitCapital, currency } from '@/utils/filters';
import { appGlobals } from '@/varaibles';

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<WorkerRequestListItem[]>([]);
const serverParams = reactive<WorkerRequestFilter>({
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});

function fetchWorkerRequestHistory() {
  isLoading.value = true;
  getWorkerRequestHistory(serverParams)
    .then((response) => {
      rows.value = response.items;
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

fetchWorkerRequestHistory();
</script>
