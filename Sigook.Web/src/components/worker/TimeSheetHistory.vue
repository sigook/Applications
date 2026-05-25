<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable
      v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <b-table-column field="businessName" label="Company" v-slot="tableProps">
        {{ tableProps.row.businessName }}
      </b-table-column>
      <b-table-column field="numberId" label="Request" v-slot="tableProps">
        {{ tableProps.row.numberId }}
        <i class="fz-2 block"> {{ tableProps.row.jobTitle }}</i>
      </b-table-column>
      <b-table-column field="date" label="Date" v-slot="tableProps">
        {{ dateMonth(tableProps.row.date) }} <i v-if="tableProps.row.isHoliday" class="holiday-text">Holiday</i>
      </b-table-column>
      <b-table-column field="regularHours" label="Regular" v-slot="tableProps">
        {{ hour(tableProps.row.regularHours) }}
      </b-table-column>
      <b-table-column field="holidayHours" label="Holiday" v-slot="tableProps">
        {{ tableProps.row.holidayHours }}
      </b-table-column>
      <b-table-column field="overtimeHours" label="Overtime" v-slot="tableProps">
        {{ tableProps.row.overtimeHours }}
      </b-table-column>
      <b-table-column field="missingHours" label="Missing" v-slot="tableProps">
        {{ tableProps.row.missingHours }}
      </b-table-column>
      <b-table-column field="missingHoursOvertime" label="Missing Overtime" v-slot="tableProps">
        {{ tableProps.row.missingHoursOvertime }}
      </b-table-column>
      <b-table-column field="totalHours" label="Total" v-slot="tableProps">
        {{ hour(tableProps.row.totalHours) }}
      </b-table-column>
      <b-table-column field="actions" v-slot="tableProps">
        <b-tooltip type="is-light" :triggers="['click']" :auto-close="['outside', 'escape']"
          @open="getAccumulated(tableProps.row)" @close="rowDetail = {}" append-to-body>
          <template v-slot:content>
            <div><strong>Regular: </strong>{{ rowDetail.regularHours }}</div>
            <div><strong>Holiday: </strong>{{ rowDetail.holidayHours }}</div>
            <div><strong>Overtime: </strong>{{ rowDetail.overtimeHours }}</div>
            <div><strong>Missing: </strong>{{ rowDetail.missingHours }}</div>
            <div><strong>Missing Overtime: </strong>{{ rowDetail.missingHoursOvertime }}</div>
            <div><strong>Total: </strong>{{ rowDetail.totalHours }}</div>
          </template>
          <b-button type="is-info" outlined rounded label="Accumulated" />
        </b-tooltip>
      </b-table-column>
    </b-table>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from '@/utils/toast';
import { dateMonth, hour } from '@/utils/filters';
import { getWorkerProfileTimeSheetHistory as apiGetWorkerProfileTimeSheetHistory, getWorkerProfileTimeSheetHistoryAccumulated } from '@/api/workerApi';

const props = defineProps<{ workerId?: any }>();

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive({
  profileId: props.workerId,
  sortBy: 3,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});
const rowDetail = ref<any>({});

function onPageChange(params: number) {
  serverParams.pageIndex = params;
  fetchTimeSheetHistory();
}

function fetchTimeSheetHistory() {
  isLoading.value = true;
  apiGetWorkerProfileTimeSheetHistory(serverParams)
    .then(response => {
      isLoading.value = false;
      rows.value = response.items.map((c: any) => ({ ...c, actions: null }));
      totalItems.value = response.totalItems;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function getAccumulated(row: any) {
  getWorkerProfileTimeSheetHistoryAccumulated(props.workerId, row.rowNumber)
    .then((response) => (rowDetail.value = response))
    .catch((error) => showAlertError(error));
}

fetchTimeSheetHistory();
</script>
