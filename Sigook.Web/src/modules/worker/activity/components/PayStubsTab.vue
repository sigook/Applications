<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid v-if="year" ref="grid" :fetch="getWorkerPayStubs" v-model:params="serverParams"
      empty-text="No pay stubs for this year" @update:loading="(value) => isLoading = value">
      <template #actions>
        <b-select v-model="serverParams.year" aria-label="Year" @update:modelValue="onYearChanged">
          <option v-for="option in years" :key="option" :value="option">{{ option }}</option>
        </b-select>
        <span class="ytd">YTD net: <strong>{{ currency(ytd?.totalPaid ?? 0) }}</strong></span>
      </template>
      <template #mobile-card="{ row }">
        <div class="rcard">
          <div class="rcard__head">
            <div>
              <p class="rcard__title">Week ending {{ dateMonth(row.weekEnding) }}</p>
              <p class="rcard__sub">{{ row.payStubNumber }} · {{ row.companies.join(', ') }}</p>
            </div>
            <b-button size="is-small" icon-left="download" @click="emit('download', row)">PDF</b-button>
          </div>
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">Hours</span>
              <span>{{ hoursLabel(row.hours) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Gross</span>
              <span>{{ currency(row.totalEarnings) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Deductions</span>
              <span>−{{ currency(row.totalDeductions) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Net</span>
              <span class="has-text-weight-semibold">{{ currency(row.totalPaid) }}</span>
            </div>
          </div>
        </div>
      </template>
      <b-table-column field="payStubNumber" label="Pay stub" v-slot="props">
        {{ props.row.payStubNumber }}
      </b-table-column>
      <b-table-column field="weekEnding" label="Week ending" v-slot="props">
        <span class="has-text-weight-medium">{{ dateMonth(props.row.weekEnding) }}</span>
      </b-table-column>
      <b-table-column field="companies" label="Client" v-slot="props">
        {{ props.row.companies.join(', ') }}
      </b-table-column>
      <b-table-column field="hours" label="Hours" numeric v-slot="props">
        {{ hoursLabel(props.row.hours) }}
      </b-table-column>
      <b-table-column field="totalEarnings" label="Gross" numeric v-slot="props">
        {{ currency(props.row.totalEarnings) }}
      </b-table-column>
      <b-table-column field="totalDeductions" label="Deductions" numeric v-slot="props">
        −{{ currency(props.row.totalDeductions) }}
      </b-table-column>
      <b-table-column field="totalPaid" label="Net" numeric v-slot="props">
        <span class="has-text-weight-semibold">{{ currency(props.row.totalPaid) }}</span>
      </b-table-column>
      <b-table-column field="actions" numeric v-slot="props">
        <b-button size="is-small" icon-left="download" @click="emit('download', props.row)">PDF</b-button>
      </b-table-column>
    </SigookGrid>
    <p v-else-if="!isLoading" class="has-text-centered p-5">You have no pay stubs yet</p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef } from 'vue';
import { currency, dateMonth } from '@/shared/format';
import { showAlertError } from '@/shared/utils/toast';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import { getWorkerPayStubs, getWorkerPayStubYears, getWorkerPayStubYtd } from '@/modules/worker/activity/api';
import { hoursLabel } from '@/modules/worker/activity/activityFormat';
import type { WorkerPayStubFilter, WorkerPayStubListItem, WorkerPayStubYtd } from '@/modules/worker/activity/types';
import type { GridHandle } from '@/shared/types/common';

const emit = defineEmits<{
  (e: 'download', payStub: WorkerPayStubListItem): void;
}>();

const grid = useTemplateRef<GridHandle>('grid');
const isLoading = ref(true);
const years = ref<number[]>([]);
const ytd = ref<WorkerPayStubYtd | null>(null);
const serverParams = ref<WorkerPayStubFilter>({
  pageIndex: 1,
  pageSize: 30,
});
const year = computed(() => serverParams.value.year);

function loadYtd() {
  if (!serverParams.value.year) return;
  getWorkerPayStubYtd(serverParams.value.year)
    .then((response) => { ytd.value = response; })
    .catch((error: unknown) => showAlertError(error));
}

function onYearChanged() {
  grid.value?.search();
  loadYtd();
}

getWorkerPayStubYears()
  .then((response) => {
    years.value = response;
    serverParams.value.year = response[0];
    isLoading.value = false;
    loadYtd();
  })
  .catch((error: unknown) => {
    isLoading.value = false;
    showAlertError(error);
  });
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.ytd {
  font-size: 0.85rem;
  color: $grey-font;
  align-self: center;
  margin-left: 12px;

  strong {
    color: $navy;
  }
}
</style>
