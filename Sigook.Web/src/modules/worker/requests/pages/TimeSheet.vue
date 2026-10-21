<template>
  <div>
    <SigookGrid :data="data.items || []" :paginated="false" empty-text="No timesheet records. Your completed shifts will appear here.">
      <template #mobile-card="{ row }">
        <TimeSheetCard :entry="row" @select="openDetail" />
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

    <TimeSheetDetailSheet v-model="showDetail" :entry="selected" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { date, time } from '@/shared/format';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import TimeSheetCard from '@/modules/worker/requests/components/TimeSheetCard.vue';
import TimeSheetDetailSheet from '@/modules/worker/requests/components/TimeSheetDetailSheet.vue';
import type { WorkerTimeSheetItem } from '@/modules/worker/requests/types';

defineProps<{
  data: { items?: WorkerTimeSheetItem[] };
}>();

const selected = ref<WorkerTimeSheetItem | null>(null);
const showDetail = ref(false);

function openDetail(entry: WorkerTimeSheetItem) {
  selected.value = entry;
  showDetail.value = true;
}
</script>
