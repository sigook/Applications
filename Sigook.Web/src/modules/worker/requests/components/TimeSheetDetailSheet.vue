<template>
  <SheetPanel :model-value="modelValue" title="Timesheet Details" @update:modelValue="(v) => emit('update:modelValue', v)">
    <template v-if="entry">
      <div class="sheet-head">
        <span class="sheet-head__date">{{ date(entry.day) }}</span>
        <span v-if="entry.wasApproved" class="sheet-head__pill">
          <b-icon icon="check-circle" size="is-small"></b-icon>
          Approved
        </span>
      </div>

      <section v-for="section in sections" :key="section.title" class="sheet-section" :class="'is-' + section.tone">
        <div class="sheet-section__head">
          <b-icon :icon="section.icon" size="is-small"></b-icon>
          <span>{{ section.title }}</span>
        </div>
        <div class="sheet-section__body">
          <div v-for="row in section.rows" :key="row.label" class="sheet-row">
            <span class="sheet-row__icon" :class="'is-' + (row.tone ?? 'grey')">
              <b-icon :icon="row.icon" size="is-small"></b-icon>
            </span>
            <div class="sheet-row__text">
              <div class="sheet-row__label">{{ row.label }}</div>
              <div class="sheet-row__value">{{ row.value }}</div>
              <div v-if="row.subtitle" class="sheet-row__sub">{{ row.subtitle }}</div>
            </div>
          </div>
        </div>
      </section>

      <div v-if="entry.comment" class="sheet-comment">
        <div class="sheet-comment__head">
          <b-icon icon="comment-text-outline" size="is-small"></b-icon>
          <span>Comment</span>
        </div>
        <p>{{ entry.comment }}</p>
      </div>
    </template>
    <template #foot>
      <b-button type="is-link" rounded @click="emit('update:modelValue', false)">Close</b-button>
    </template>
  </SheetPanel>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import SheetPanel from '@/shared/ui/SheetPanel.vue';
import { currency, date, hour, time } from '@/shared/format';
import type { TimeSheetDetailRow, TimeSheetDetailSection, WorkerTimeSheetItem } from '@/modules/worker/requests/types';

const props = defineProps<{
  modelValue: boolean;
  entry: WorkerTimeSheetItem | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
}>();

function hours(value: number): string {
  return `${(value > 0 ? value : 0).toFixed(2)} hrs`;
}

function spanHours(value: string | null | undefined): string | null {
  const parsed = hour(value ?? null);
  return parsed === 0 ? null : `${parsed} hrs`;
}

const sections = computed<TimeSheetDetailSection[]>(() => {
  const entry = props.entry;
  if (!entry) return [];

  const clockRows: TimeSheetDetailRow[] = [];
  if (entry.clockIn) clockRows.push({ label: 'Clock In', value: time(entry.clockIn), icon: 'login', tone: 'green' });
  if (entry.clockOut) clockRows.push({ label: 'Clock Out', value: time(entry.clockOut), icon: 'logout', tone: 'danger' });
  if (entry.clockInRounded) {
    clockRows.push({ label: 'Clock In (Rounded)', value: time(entry.clockInRounded), icon: 'clock-outline' });
  }
  if (entry.clockOutRounded) {
    clockRows.push({ label: 'Clock Out (Rounded)', value: time(entry.clockOutRounded), icon: 'clock-outline' });
  }

  const approvedRows: TimeSheetDetailRow[] = [];
  if (entry.timeInApproved) {
    approvedRows.push({ label: 'Time In (Approved)', value: time(entry.timeInApproved), icon: 'check-circle', tone: 'green' });
  }
  if (entry.timeOutApproved) {
    approvedRows.push({ label: 'Time Out (Approved)', value: time(entry.timeOutApproved), icon: 'check-circle', tone: 'green' });
  }

  const hoursRows: TimeSheetDetailRow[] = [
    { label: 'Total Hours', value: hours(entry.totalHours), icon: 'timer-outline', tone: 'blue' },
    { label: 'Approved Hours', value: hours(entry.totalHoursApproved), icon: 'check-circle', tone: 'green' },
  ];
  const missing = spanHours(entry.missingHours);
  if (missing) hoursRows.push({ label: 'Missing Hours', value: missing, icon: 'alert', tone: 'accent' });
  const missingOvertime = spanHours(entry.missingHoursOvertime);
  if (missingOvertime) {
    hoursRows.push({ label: 'Missing Overtime', value: missingOvertime, icon: 'alert-outline', tone: 'accent' });
  }

  const financialRows: TimeSheetDetailRow[] = [];
  if (entry.missingRateWorker > 0) {
    financialRows.push({ label: 'Worker Rate', value: currency(entry.missingRateWorker), icon: 'account', tone: 'blue' });
  }
  if (entry.bonusOrOthers > 0) {
    financialRows.push({
      label: 'Bonus/Others',
      value: currency(entry.bonusOrOthers),
      icon: 'gift-outline',
      tone: 'green',
      subtitle: entry.bonusOrOthersDescription,
    });
  }
  if (entry.deductionsOthers > 0) {
    financialRows.push({
      label: 'Deductions',
      value: currency(entry.deductionsOthers),
      icon: 'minus-circle',
      tone: 'danger',
      subtitle: entry.deductionsOthersDescription,
    });
  }
  if (entry.reimbursements > 0) {
    financialRows.push({
      label: 'Reimbursements',
      value: currency(entry.reimbursements),
      icon: 'receipt',
      tone: 'purple',
      subtitle: entry.reimbursementsDescription,
    });
  }

  const result: TimeSheetDetailSection[] = [];
  if (clockRows.length) result.push({ title: 'Clock Times', icon: 'clock-outline', tone: 'blue', rows: clockRows });
  if (approvedRows.length) result.push({ title: 'Approved Times', icon: 'check-decagram', tone: 'green', rows: approvedRows });
  result.push({ title: 'Hours Summary', icon: 'clock-outline', tone: 'blue', rows: hoursRows });
  if (financialRows.length) {
    result.push({ title: 'Financial Details', icon: 'currency-usd', tone: 'green', rows: financialRows });
  }
  return result;
});
</script>

<style lang="scss" scoped>
@use 'sass:list';
@import '@/assets/scss/variables';

$tones: (
  'blue': ($blue, $blue),
  'green': ($green, $green-text),
  'danger': ($danger, $danger-hover),
  'accent': ($accent, $accent-text),
  'purple': ($purple, $purple),
  'grey': ($grey-light, $grey-light),
);

.sheet-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 16px;
}

.sheet-head__date {
  font-size: 0.95rem;
  color: $grey-light;
}

.sheet-head__pill {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
  padding: 4px 12px;
  border: 1px solid $green;
  border-radius: 999px;
  background: rgba($green, 0.1);
  font-size: 0.8rem;
  font-weight: 600;
  color: $green-text;
}

.sheet-section {
  border: 1px solid $gray-border;
  border-radius: 12px;
  overflow: hidden;

  + .sheet-section,
  + .sheet-comment {
    margin-top: 16px;
  }
}

.sheet-section__head {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  font-size: 0.95rem;
  font-weight: 600;
}

.sheet-section__body {
  padding: 4px 12px;
}

.sheet-row {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 8px 0;
}

.sheet-row__icon {
  display: inline-flex;
  flex-shrink: 0;
  padding-top: 2px;
}

.sheet-row__text {
  flex: 1;
  min-width: 0;
}

.sheet-row__label {
  font-size: 0.8rem;
  color: $grey-light;
}

.sheet-row__value {
  font-size: 0.95rem;
  font-weight: 600;
  color: $navy;
  font-variant-numeric: tabular-nums;
}

.sheet-row__sub {
  font-size: 0.75rem;
  font-style: italic;
  color: $grey-light;
  overflow-wrap: anywhere;
}

@each $name, $pair in $tones {
  .sheet-section.is-#{$name} .sheet-section__head {
    background: rgba(list.nth($pair, 1), 0.08);
    color: list.nth($pair, 2);
  }

  .sheet-row__icon.is-#{$name} {
    color: list.nth($pair, 2);
  }
}

.sheet-comment {
  margin-top: 16px;
  padding: 16px;
  border: 1px solid rgba($blue, 0.25);
  border-radius: 12px;
  background: rgba($blue, 0.06);

  p {
    margin-top: 12px;
    font-size: 0.9rem;
    line-height: 1.5;
    color: $grey-font;
    overflow-wrap: anywhere;
  }
}

.sheet-comment__head {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.95rem;
  font-weight: 600;
  color: $navy;

  :deep(.icon) {
    color: $blue;
  }
}
</style>
