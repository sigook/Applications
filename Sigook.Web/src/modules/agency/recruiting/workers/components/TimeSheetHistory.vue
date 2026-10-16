<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="getWorkerProfileTimeSheetHistory" v-model:params="serverParams" :sort-map="sortMap"
      focusable @update:loading="(value) => isLoading = value">
      <b-table-column field="businessName" label="Company" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.companyName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="tableProps">{{ tableProps.row.businessName }}</template>
      </b-table-column>
      <b-table-column field="numberId" label="Request" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="tableProps">
          <router-link :to="{ path: requestBase + '/' + tableProps.row.requestId }">
            {{ tableProps.row.numberId }}
          </router-link>
          <i class="fz-2 block">{{ tableProps.row.jobTitle }}</i>
        </template>
      </b-table-column>
      <b-table-column field="date" label="Date" sortable searchable>
        <template v-slot:searchable>
          <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
            :icon-right="datesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
            @icon-right-click="onDatesCleared" range v-model="datesSelected"
            @update:modelValue="onDatesSelected" append-to-body>
          </b-datepicker>
        </template>
        <template v-slot="tableProps">
          {{ dateMonth(tableProps.row.date) }} <i v-if="tableProps.row.isHoliday" class="holiday-text">Holiday</i>
        </template>
      </b-table-column>
      <b-table-column field="regularHours" label="Regular" v-slot="tableProps">
        {{ hour(tableProps.row.regularHours) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedRegularHours) }}</i>
      </b-table-column>
      <b-table-column field="holidayHours" label="Holiday" v-slot="tableProps">
        {{ hour(tableProps.row.holidayHours) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedHolidayHours) }}</i>
      </b-table-column>
      <b-table-column field="overtimeHours" label="Overtime" v-slot="tableProps">
        {{ hour(tableProps.row.overtimeHours) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedOvertimeHours) }}</i>
      </b-table-column>
      <b-table-column field="missingHours" label="Missing" v-slot="tableProps">
        {{ hour(tableProps.row.missingHours) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedMissingHours) }}</i>
      </b-table-column>
      <b-table-column field="missingHoursOvertime" label="Missing Overtime" v-slot="tableProps">
        {{ hour(tableProps.row.missingHoursOvertime) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedMissingHoursOvertime) }}</i>
      </b-table-column>
      <b-table-column field="totalHours" label="Total" v-slot="tableProps">
        {{ hour(tableProps.row.totalHours) }}
        <i class="fz-2 block">Acc. {{ hour(tableProps.row.accumulatedTotalHours) }}</i>
      </b-table-column>
    </SigookGrid>
  </div>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { dateMonth, hour } from '@/shared/format';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import { getWorkerProfileTimeSheetHistory } from '@/modules/agency/recruiting/workers/api';
import type { WorkerTimeSheetHistoryFilter } from '@/modules/agency/recruiting/workers/types';
import type { GridHandle } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ workerId: string }>();

const { requestBase } = useModuleBase();
const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  businessName: 0,
  numberId: 1,
  date: 2,
};

const isLoading = ref(false);
const datesSelected = ref<Date[]>([]);
const serverParams = ref<WorkerTimeSheetHistoryFilter>({
  profileId: props.workerId,
  sortBy: 2,
  isDescending: true,
});

function onDatesSelected() {
  serverParams.value.dateFrom = datesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.dateTo = datesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onDatesCleared() {
  datesSelected.value = [];
  onDatesSelected();
}
</script>
