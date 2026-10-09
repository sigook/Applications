<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :fetch="getWorkerProfileTimeSheetHistory" v-model:params="serverParams" focusable
      @update:loading="(value) => isLoading = value">
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
    </SigookGrid>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import { dateMonth, hour } from '@/shared/format';
import { getWorkerProfileTimeSheetHistory, getWorkerProfileTimeSheetHistoryAccumulated } from '@/modules/agency/recruiting/workers/api';
import type { TimeSheetHistoryFilter, WorkerTimeSheetHistoryAccumulated } from '@/modules/agency/recruiting/workers/types';
import type { GridParams } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ workerId: string }>();

const isLoading = ref(false);
const serverParams = ref<TimeSheetHistoryFilter & GridParams>({
  profileId: props.workerId,
  sortBy: 3,
  isDescending: true,
});
const rowDetail = ref<Partial<WorkerTimeSheetHistoryAccumulated>>({});

function getAccumulated(row: { rowNumber: number }) {
  getWorkerProfileTimeSheetHistoryAccumulated(props.workerId, row.rowNumber)
    .then((response) => (rowDetail.value = response))
    .catch((error) => showAlertError(error));
}
</script>
