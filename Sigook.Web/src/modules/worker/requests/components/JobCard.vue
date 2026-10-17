<template>
  <div class="job-card" :class="{ 'is-clickable': clickable }" @click="emit('click')">
    <div class="job-card__head">
      <span class="job-card__logo">
        <b-icon icon="briefcase-outline"></b-icon>
      </span>
      <span class="job-card__title">{{ job.numberId }} · {{ job.jobTitle }}</span>
      <span v-if="job.isAsap" class="job-card__asap">ASAP</span>
    </div>

    <div class="job-card__row">
      <b-icon icon="map-marker" size="is-small"></b-icon>
      <span class="job-card__text">{{ address }}</span>
      <span v-if="status" class="job-card__status" :class="statusClass">{{ status }}</span>
    </div>

    <div class="job-card__row">
      <b-icon icon="calendar-blank-outline" size="is-small"></b-icon>
      <span class="job-card__text">{{ dates }}</span>
    </div>

    <div class="job-card__foot">
      <span class="job-card__chip is-rate">
        <b-icon icon="currency-usd" size="is-small"></b-icon>
        {{ pay }}
      </span>
      <span class="job-card__chip is-spots">
        <b-icon icon="account-group" size="is-small"></b-icon>
        {{ job.workersQuantity ?? 0 }}
      </span>
      <span class="job-card__chip is-term">{{ splitCapital(job.durationTerm) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { appGlobals } from '@/app/globals';
import { currency, dateMonth, splitCapital } from '@/shared/format';
import type { WorkerRequestListItem } from '@/modules/worker/requests/types';

const props = defineProps<{
  job: WorkerRequestListItem;
  clickable?: boolean;
}>();

const emit = defineEmits<{ (e: 'click'): void }>();

const address = computed(() => [props.job.location, props.job.entrance].filter(Boolean).join(' - ') || 'N/A');

const status = computed(() => (props.job.status && props.job.status !== 'None' ? props.job.status : ''));

const statusClass = computed(() => {
  switch (status.value.toLowerCase()) {
    case 'booked':
    case 'filled':
      return 'is-success';
    case 'pending':
    case 'applied':
    case 'apply':
    case 'open':
      return 'is-warning';
    case 'cancelled':
    case 'rejected':
    case 'decline':
    case 'declined':
      return 'is-danger';
    default:
      return '';
  }
});

const showFinish = computed(() =>
  props.job.durationTerm !== appGlobals.$longTerm
  || props.job.status === appGlobals.$statusFilled
  || props.job.status === appGlobals.$statusCancelled,
);

const dates = computed(() => {
  const start = dateMonth(props.job.startAt);
  return showFinish.value && props.job.finishAt ? `${start} - ${dateMonth(props.job.finishAt)}` : start;
});

const pay = computed(() =>
  props.job.workerSalary ? `${currency(props.job.workerSalary)}/year` : `${currency(props.job.workerRate)}/hr`,
);
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.job-card {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 16px;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);

  &.is-clickable {
    cursor: pointer;
  }
}

.job-card__head {
  display: flex;
  align-items: center;
  gap: 12px;
}

.job-card__logo {
  display: flex;
  flex-shrink: 0;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: rgba($blue, 0.1);
  color: $blue;
}

.job-card__title {
  flex: 1;
  min-width: 0;
  font-size: 1.1rem;
  font-weight: 700;
  line-height: 1.3;
  color: $navy;
  overflow-wrap: anywhere;
}

.job-card__asap {
  flex-shrink: 0;
  padding: 3px 8px;
  border-radius: 4px;
  background: $danger;
  font-size: 0.65rem;
  font-weight: 700;
  color: $white;
}

.job-card__row {
  display: flex;
  align-items: center;
  gap: 4px;
  color: $grey-font;
}

.job-card__text {
  flex: 1;
  min-width: 0;
  font-size: 0.9rem;
  overflow-wrap: anywhere;
}

.job-card__status {
  flex-shrink: 0;
  padding: 4px 12px;
  border-radius: 999px;
  background: $gray-bg;
  font-size: 0.75rem;
  font-weight: 600;
  color: $grey-light;

  &.is-success {
    background: rgba($green, 0.15);
    color: $green-text;
  }

  &.is-warning {
    background: rgba($accent, 0.15);
    color: $accent-text;
  }

  &.is-danger {
    background: rgba($danger, 0.1);
    color: $danger-hover;
  }
}

.job-card__foot {
  display: flex;
  align-items: center;
  gap: 8px;
  padding-top: 12px;
  border-top: 1px solid $gray-border;
}

.job-card__chip {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 10px;
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 600;

  &.is-rate {
    background: rgba($blue, 0.1);
    color: $blue;
    font-size: 1rem;
    font-weight: 700;
  }

  &.is-spots {
    background: $gray-bg;
    color: $grey-font;
  }

  &.is-term {
    margin-left: auto;
    background: rgba($purple, 0.1);
    color: $purple;
    font-size: 0.75rem;
  }
}
</style>
