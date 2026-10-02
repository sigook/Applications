<template>
  <div>
    <SigookGrid :data="data.items || []" :paginated="false">
      <template #mobile-card="{ row }">
        <div class="rcard">
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">Day</span>
              <span v-if="row.day">{{ date(row.day) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Clock In</span>
              <span v-if="row.timeIn">{{ time(row.clockIn) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Clock Out</span>
              <span v-if="row.clockOut !== null">{{ time(row.clockOut) }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Worked hours</span>
              <span>{{ row.totalHours.toFixed(2) }}</span>
            </div>
          </div>
        </div>
      </template>
      <b-table-column field="day" :label="'Day'" v-slot="props">
        <span v-if="props.row.day">{{ date(props.row.day) }}</span>
      </b-table-column>
      <b-table-column field="clockIn" label="Clock In" v-slot="props">
        <span v-if="props.row.timeIn">{{ time(props.row.clockIn) }}</span>
      </b-table-column>
      <b-table-column field="clockOut" label="Clock Out" v-slot="props">
        <span v-if="props.row.clockOut !== null">{{ time(props.row.clockOut) }}</span>
      </b-table-column>
      <b-table-column field="totalHours" :label="'Worked hours'" v-slot="props">
        {{ props.row.totalHours.toFixed(2) }}
      </b-table-column>
    </SigookGrid>
  </div>
</template>

<script setup lang="ts">
import { date, time } from '@/utils/filters';
import SigookGrid from '@/components/SigookGrid.vue';
import type { WorkerTimeSheetItem } from '@/types/worker';

defineProps<{
  data: { items?: WorkerTimeSheetItem[] };
}>();
</script>
