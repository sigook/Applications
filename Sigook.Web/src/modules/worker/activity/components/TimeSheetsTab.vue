<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="week?.items ?? []" :paginated="false" empty-text="No hours recorded this week" @click="openDetail">
      <template #actions>
        <b-button icon-left="chevron-left" aria-label="Previous week" @click="moveWeek(-1)" />
        <span class="week-label">{{ weekLabel }}</span>
        <b-button icon-left="chevron-right" aria-label="Next week" :disabled="isCurrentWeek" @click="moveWeek(1)" />
        <span class="week-total">Week total: <strong>{{ hoursLabel(week?.hours.total) }} h</strong></span>
      </template>
      <template #mobile-card="{ row }">
        <div class="rcard is-clickable" @click="openDetail(row)">
          <div class="rcard__head">
            <div>
              <p class="rcard__title">{{ dayLabel(row.date) }}</p>
              <p class="rcard__sub">{{ row.clientName }} · {{ row.jobTitle }}</p>
            </div>
            <b-tag :class="statusTagType(timeSheetStatusLabel(row))">{{ timeSheetStatusLabel(row) }}</b-tag>
          </div>
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">In – Out</span>
              <span>{{ timeSheetRangeLabel(row) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Regular · OT</span>
              <span>{{ hoursLabel(row.regularHours) }} · {{ hoursLabel(row.overtimeHours) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Total</span>
              <span class="has-text-weight-semibold">{{ hoursLabel(row.totalHours) }}</span>
            </div>
          </div>
        </div>
      </template>
      <b-table-column field="date" label="Date" v-slot="props">
        <span class="has-text-weight-medium">{{ dayLabel(props.row.date) }}</span>
      </b-table-column>
      <b-table-column field="clientName" label="Client · Position" v-slot="props">
        {{ props.row.clientName }} · {{ props.row.jobTitle }}
      </b-table-column>
      <b-table-column field="clockIn" label="In" v-slot="props">
        {{ isManualEntry(props.row) ? 'Manual entry' : dateHHmm(props.row.clockIn ?? null) ?? '—' }}
      </b-table-column>
      <b-table-column field="clockOut" label="Out" v-slot="props">
        {{ dateHHmm(props.row.clockOut ?? null) ?? '—' }}
      </b-table-column>
      <b-table-column field="durationBreak" label="Break" v-slot="props">
        {{ breakLabel(props.row) }}
      </b-table-column>
      <b-table-column field="regularHours" label="Regular" numeric v-slot="props">
        {{ hoursLabel(props.row.regularHours) }}
      </b-table-column>
      <b-table-column field="overtimeHours" label="OT" numeric v-slot="props">
        {{ props.row.overtimeHours ? hoursLabel(props.row.overtimeHours) : '—' }}
      </b-table-column>
      <b-table-column field="totalHours" label="Total" numeric v-slot="props">
        <span class="has-text-weight-semibold">{{ hoursLabel(props.row.totalHours) }}</span>
      </b-table-column>
      <b-table-column field="status" label="Status" v-slot="props">
        <b-tag :class="statusTagType(timeSheetStatusLabel(props.row))">{{ timeSheetStatusLabel(props.row) }}</b-tag>
      </b-table-column>
    </SigookGrid>
    <TimeSheetDetailSheet v-model="showDetail" :entry="selected" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import dayjs from 'dayjs';
import { dateHHmm } from '@/shared/format';
import { showAlertError } from '@/shared/utils/toast';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import { getWorkerWeekTimeSheets } from '@/modules/worker/activity/api';
import {
  breakLabel,
  hoursLabel,
  isManualEntry,
  statusTagType,
  timeSheetRangeLabel,
  timeSheetStatusLabel,
  weekStartOf,
} from '@/modules/worker/activity/activityFormat';
import TimeSheetDetailSheet from '@/modules/worker/activity/components/TimeSheetDetailSheet.vue';
import type { WorkerWeekTimeSheet, WorkerWeekTimeSheets } from '@/modules/worker/activity/types';

const currentWeek = weekStartOf();
const weekStart = ref(currentWeek);
const week = ref<WorkerWeekTimeSheets | null>(null);
const isLoading = ref(false);
const selected = ref<WorkerWeekTimeSheet | null>(null);
const showDetail = ref(false);

const isCurrentWeek = computed(() => weekStart.value >= currentWeek);
const weekLabel = computed(() => {
  const start = dayjs(weekStart.value);
  return `${start.format('MMM D')} – ${start.add(6, 'day').format('MMM D, YYYY')}`;
});

function dayLabel(date: string) {
  return dayjs(date).format('ddd, MMM D');
}

function openDetail(row: WorkerWeekTimeSheet) {
  selected.value = row;
  showDetail.value = true;
}

function load() {
  isLoading.value = true;
  getWorkerWeekTimeSheets(weekStart.value)
    .then((response) => {
      week.value = response;
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function moveWeek(direction: number) {
  weekStart.value = dayjs(weekStart.value).add(direction, 'week').format('YYYY-MM-DD');
  load();
}

load();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.week-label {
  font-weight: 600;
  align-self: center;
  margin: 0 8px;
  color: $navy;
}

.week-total {
  font-size: 0.85rem;
  color: $grey-font;
  align-self: center;
  margin-left: 12px;

  strong {
    color: $navy;
  }
}
</style>
