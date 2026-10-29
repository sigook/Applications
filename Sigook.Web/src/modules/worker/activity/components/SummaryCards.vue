<template>
  <section class="summary-cards" aria-label="Summary">
    <div v-if="showLastPay" class="summary-card">
      <span class="summary-card__label">Last pay (net)</span>
      <template v-if="summary?.lastPayStub">
        <span class="summary-card__value">{{ currency(summary.lastPayStub.totalPaid) }}</span>
        <span class="summary-card__sub">
          Week ending {{ dateMonth(summary.lastPayStub.weekEnding) }} ·
          <a href="#" @click.prevent="emit('download', summary.lastPayStub)">PDF</a>
        </span>
      </template>
      <span v-else class="summary-card__sub">No pay stubs yet</span>
    </div>
    <div class="summary-card">
      <span class="summary-card__label">Hours this week</span>
      <span class="summary-card__value">{{ hoursLabel(summary?.weekHours.total) }} h</span>
      <span class="summary-card__sub">
        {{ hoursLabel(summary?.weekHours.regular) }} regular · {{ hoursLabel(summary?.weekHours.overtime) }} OT
        <template v-if="summary?.weekHours.pendingApproval"> · {{ hoursLabel(summary.weekHours.pendingApproval) }} pending approval</template>
      </span>
    </div>
  </section>
</template>

<script setup lang="ts">
import { currency, dateMonth } from '@/shared/format';
import { hoursLabel } from '@/modules/worker/activity/activityFormat';
import type { WorkerActivitySummary, WorkerPayStubListItem } from '@/modules/worker/activity/types';

defineProps<{
  summary: WorkerActivitySummary | null;
  showLastPay: boolean;
}>();

const emit = defineEmits<{
  (e: 'download', payStub: WorkerPayStubListItem): void;
}>();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
}

.summary-card {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 16px 18px;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 10px;

  &__label {
    font-size: 0.75rem;
    font-weight: 500;
    color: $grey-font;
  }

  &__value {
    font-size: 1.6rem;
    font-weight: 700;
    color: $navy;
  }

  &__sub {
    font-size: 0.8rem;
    color: $grey-font;
  }
}
</style>
