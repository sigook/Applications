<template>
  <SheetPanel :model-value="modelValue" :title="entry ? dayjs(entry.date).format('ddd, MMM D, YYYY') : ''"
    @update:modelValue="(value) => emit('update:modelValue', value)">
    <template v-if="entry">
      <div class="detail-head">
        <span class="detail-head__sub">{{ entry.clientName }} · {{ entry.jobTitle }}</span>
        <b-tag :class="statusTagType(timeSheetStatusLabel(entry))">{{ timeSheetStatusLabel(entry) }}</b-tag>
      </div>

      <section v-for="section in sections" :key="section.title" class="detail-section">
        <h3 class="detail-section__title">{{ section.title }}</h3>
        <div v-for="row in section.rows" :key="row.label" class="detail-row">
          <div>
            <div class="detail-row__label">{{ row.label }}</div>
            <div v-if="row.hint" class="detail-row__hint">{{ row.hint }}</div>
          </div>
          <span class="detail-row__value" :class="{ 'has-text-weight-semibold': row.isBold }">{{ row.value }}</span>
        </div>
      </section>
    </template>
    <template #foot>
      <b-button type="is-link" rounded @click="emit('update:modelValue', false)">Close</b-button>
    </template>
  </SheetPanel>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import dayjs from 'dayjs';
import { currency } from '@/shared/format';
import SheetPanel from '@/shared/ui/SheetPanel.vue';
import {
  breakLabel,
  hoursLabel,
  spanMinutes,
  statusTagType,
  timeSheetRangeLabel,
  timeSheetStatusLabel,
} from '@/modules/worker/activity/activityFormat';
import type { WorkerWeekTimeSheet } from '@/modules/worker/activity/types';

interface DetailRow {
  label: string;
  value: string;
  hint?: string | null;
  isBold?: boolean;
}

interface DetailSection {
  title: string;
  rows: DetailRow[];
}

const props = defineProps<{
  modelValue: boolean;
  entry: WorkerWeekTimeSheet | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
}>();

function spanHoursLabel(value: string): string {
  return `${hoursLabel(spanMinutes(value) / 60)} h`;
}

const sections = computed<DetailSection[]>(() => {
  const entry = props.entry;
  if (!entry) return [];

  const result: DetailSection[] = [{
    title: 'Hours',
    rows: [
      { label: 'In – Out', value: timeSheetRangeLabel(entry) },
      { label: 'Break', value: breakLabel(entry) },
      { label: 'Regular', value: hoursLabel(entry.regularHours) },
      { label: 'Overtime', value: hoursLabel(entry.overtimeHours) },
      { label: 'Total', value: hoursLabel(entry.totalHours), isBold: true },
    ],
  }];

  const adjustments: DetailRow[] = [];
  if (spanMinutes(entry.missingHours)) adjustments.push({ label: 'Missing hours', value: spanHoursLabel(entry.missingHours) });
  if (spanMinutes(entry.missingHoursOvertime)) {
    adjustments.push({ label: 'Missing overtime', value: spanHoursLabel(entry.missingHoursOvertime) });
  }
  if (entry.bonusOrOthers > 0) {
    adjustments.push({ label: 'Bonus / others', value: currency(entry.bonusOrOthers), hint: entry.bonusOrOthersDescription });
  }
  if (entry.deductionsOthers > 0) {
    adjustments.push({ label: 'Deductions', value: `−${currency(entry.deductionsOthers)}`, hint: entry.deductionsOthersDescription });
  }
  if (entry.reimbursements > 0) {
    adjustments.push({ label: 'Reimbursements', value: currency(entry.reimbursements), hint: entry.reimbursementsDescription });
  }
  if (adjustments.length) result.push({ title: 'Adjustments', rows: adjustments });

  return result;
});
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.detail-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 8px;

  &__sub {
    font-size: 0.85rem;
    color: $grey-font;
  }
}

.detail-section {
  margin-top: 16px;

  &__title {
    font-size: 0.9rem;
    font-weight: 700;
    color: $navy;
    margin-bottom: 4px;
  }
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  gap: 8px;
  padding: 2px 0;
  font-size: 0.9rem;

  &__label {
    color: $grey-font;
    font-size: 0.85rem;
  }

  &__hint {
    color: $grey-font;
    font-size: 0.8rem;
  }

  &__value {
    color: $navy;
  }
}
</style>
