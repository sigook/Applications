<template>
  <div class="wage-container">
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="getWorkerProfileWageHistory" v-model:params="serverParams" :sort-map="sortMap"
      focusable @update:loading="(value) => isLoading = value">
      <b-table-column field="payStubNumber" label="Pay Stub" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.payStubNumber" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="tableProps">
          <i>{{ tableProps.row.payStubNumber }}</i>
          <p v-for="(company, idx) in tableProps.row.companies" :key="idx">
            {{ company }}
          </p>
        </template>
      </b-table-column>
      <b-table-column field="weekEnding" label="Week Ending" sortable searchable>
        <template v-slot:searchable>
          <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
            :icon-right="weekEndingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
            @icon-right-click="onWeekEndingCleared" range v-model="weekEndingDatesSelected"
            @update:modelValue="onWeekEndingSelected" append-to-body>
          </b-datepicker>
        </template>
        <template v-slot="tableProps">
          {{ dateMonth(tableProps.row.weekEnding) }}
          <br />
          <i class="fz-1">From: {{ dateMonth(tableProps.row.start) }}</i>
          <br />
          <i class="fz-1">To: {{ dateMonth(tableProps.row.end) }}</i>
        </template>
      </b-table-column>
      <b-table-column field="items" label="Items" v-slot="tableProps">
        <table class="no-border-bottom">
          <thead>
            <tr>
              <th width="120px">Description</th>
              <th width="80px">Qty</th>
              <th width="80px">Total</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, idx) in tableProps.row.items" :key="idx">
              <td width="120px">{{ item.description }}</td>
              <td width="80px">{{ item.quantity }}</td>
              <td width="80px">{{ currency(item.total) }}</td>
            </tr>
            <tr>
              <td width="120px">Total:</td>
              <td width="80px">{{ getTotalQuantity(tableProps.row.items) }}</td>
              <td width="80px">{{ currency(getTotal(tableProps.row.items)) }}</td>
            </tr>
            <tr>
              <td width="120px"><i class="fz-2">Acc.</i></td>
              <td width="80px"><i class="fz-2">{{ tableProps.row.accumulatedQuantity }}</i></td>
              <td width="80px"><i class="fz-2">{{ currency(tableProps.row.accumulatedTotal) }}</i></td>
            </tr>
          </tbody>
        </table>
      </b-table-column>
      <b-table-column field="vacations" label="Vacations" v-slot="tableProps">
        {{ currency(tableProps.row.vacations) }}
        <i class="fz-2 block">Acc. {{ currency(tableProps.row.accumulatedVacations) }}</i>
      </b-table-column>
      <b-table-column field="totalEarnings" label="Total Earnings" sortable v-slot="tableProps">
        {{ currency(tableProps.row.totalEarnings) }}
        <i class="fz-2 block">Acc. {{ currency(tableProps.row.accumulatedTotalEarnings) }}</i>
      </b-table-column>
      <b-table-column field="totalPaid" label="Total Paid" sortable v-slot="tableProps">
        {{ currency(tableProps.row.totalPaid) }}
        <i class="fz-2 block">Acc. {{ currency(tableProps.row.accumulatedTotalPaid) }}</i>
      </b-table-column>
    </SigookGrid>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { dateMonth, currency } from '@/shared/format';
import { getWorkerProfileWageHistory } from '@/modules/agency/recruiting/workers/api';
import type { WorkerPayStubHistoryFilter, WorkerWageHistoryLine } from '@/modules/agency/recruiting/workers/types';
import type { GridHandle } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ workerId: string }>();

const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  payStubNumber: 0,
  weekEnding: 1,
  totalEarnings: 2,
  totalPaid: 3,
};

const isLoading = ref(true);
const weekEndingDatesSelected = ref<Date[]>([]);
const serverParams = ref<WorkerPayStubHistoryFilter>({
  profileId: props.workerId,
  sortBy: 0,
  isDescending: true,
});

function getTotalQuantity(items: WorkerWageHistoryLine[]) {
  return items.reduce((acc, item) => acc + item.quantity, 0);
}

function getTotal(items: WorkerWageHistoryLine[]) {
  return items.reduce((acc, item) => acc + item.total, 0);
}

function onWeekEndingSelected() {
  serverParams.value.weekEndingFrom = weekEndingDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.weekEndingTo = weekEndingDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onWeekEndingCleared() {
  weekEndingDatesSelected.value = [];
  onWeekEndingSelected();
}
</script>
