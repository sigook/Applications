<template>
  <div class="shift-card" :class="{ 'is-approved': entry.wasApproved }" role="button" tabindex="0"
    @click="emit('select', entry)" @keydown.enter="emit('select', entry)">
    <div class="shift-card__head">
      <b-icon icon="calendar-blank-outline" size="is-small"></b-icon>
      <span class="shift-card__date">{{ date(entry.day) }}</span>
      <span v-if="entry.wasApproved" class="shift-card__pill">
        <b-icon icon="check-circle" size="is-small"></b-icon>
        Approved
      </span>
    </div>

    <div class="shift-card__clocks">
      <div class="shift-card__clock">
        <span class="shift-card__clock-icon is-in">
          <b-icon icon="login" size="is-small"></b-icon>
        </span>
        <div>
          <div class="shift-card__clock-label">Clock In</div>
          <div class="shift-card__clock-value">{{ entry.clockIn ? time(entry.clockIn) : 'N/A' }}</div>
        </div>
      </div>
      <div class="shift-card__clock-sep"></div>
      <div class="shift-card__clock">
        <span class="shift-card__clock-icon is-out">
          <b-icon icon="logout" size="is-small"></b-icon>
        </span>
        <div>
          <div class="shift-card__clock-label">Clock Out</div>
          <div class="shift-card__clock-value" :class="{ 'is-progress': inProgress }">{{ clockOutText }}</div>
        </div>
      </div>
    </div>

    <div class="shift-card__hours">
      <div class="shift-card__tile is-total">
        <span class="shift-card__tile-label">Total Hours</span>
        <span class="shift-card__tile-value">{{ hours(entry.totalHours) }}</span>
      </div>
      <div class="shift-card__tile is-approved">
        <span class="shift-card__tile-label">Approved</span>
        <span class="shift-card__tile-value">{{ hours(entry.totalHoursApproved) }}</span>
      </div>
    </div>

    <div v-if="entry.comment" class="shift-card__comment">
      <b-icon icon="comment-text-outline" size="is-small"></b-icon>
      <span>{{ entry.comment }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { date, time } from '@/shared/format';
import type { WorkerTimeSheetItem } from '@/modules/worker/requests/types';

const props = defineProps<{
  entry: WorkerTimeSheetItem;
}>();

const emit = defineEmits<{
  (e: 'select', entry: WorkerTimeSheetItem): void;
}>();

const inProgress = computed(() => !!props.entry.clockIn && !props.entry.clockOut);

const clockOutText = computed(() => {
  if (props.entry.clockOut) return time(props.entry.clockOut);
  return inProgress.value ? 'In Progress' : 'N/A';
});

function hours(value: number): string {
  return `${(value > 0 ? value : 0).toFixed(1)} hrs`;
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.shift-card {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 16px;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  cursor: pointer;

  &.is-approved {
    border-color: rgba($green, 0.4);
  }

  &:focus-visible {
    outline: 2px solid $blue;
    outline-offset: 2px;
  }
}

.shift-card__head {
  display: flex;
  align-items: center;
  gap: 8px;
  color: $blue;
}

.shift-card__date {
  flex: 1;
  min-width: 0;
  font-size: 0.95rem;
  font-weight: 600;
  color: $navy;
}

.shift-card__pill {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
  padding: 2px 10px;
  border: 1px solid $green;
  border-radius: 999px;
  background: rgba($green, 0.1);
  font-size: 0.7rem;
  font-weight: 600;
  color: $green-text;
}

.shift-card__clocks {
  display: flex;
  align-items: stretch;
  padding: 12px;
  border-radius: 8px;
  background: $gray-bg;
}

.shift-card__clock {
  flex: 1 1 0;
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 0;
}

.shift-card__clock-sep {
  width: 1px;
  margin: 0 12px;
  background: $gray-border;
}

.shift-card__clock-icon {
  display: inline-flex;
  flex-shrink: 0;
  padding: 6px;
  border-radius: 6px;

  &.is-in {
    background: rgba($green, 0.12);
    color: $green-text;
  }

  &.is-out {
    background: rgba($danger, 0.1);
    color: $danger-hover;
  }
}

.shift-card__clock-label {
  font-size: 0.7rem;
  color: $grey-light;
}

.shift-card__clock-value {
  font-size: 0.9rem;
  font-weight: 600;
  color: $navy;
  font-variant-numeric: tabular-nums;

  &.is-progress {
    color: $accent-text;
  }
}

.shift-card__hours {
  display: flex;
  gap: 8px;
}

.shift-card__tile {
  flex: 1 1 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  padding: 12px;
  border-radius: 8px;

  &.is-total {
    background: rgba($blue, 0.08);

    .shift-card__tile-value {
      color: $blue;
    }
  }

  &.is-approved {
    background: rgba($green, 0.08);

    .shift-card__tile-value {
      color: $green-text;
    }
  }
}

.shift-card__tile-label {
  font-size: 0.7rem;
  color: $grey-font;
}

.shift-card__tile-value {
  font-size: 1.15rem;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

.shift-card__comment {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  padding: 10px;
  border-radius: 8px;
  background: rgba($blue, 0.06);
  color: $blue;

  span {
    font-size: 0.8rem;
    line-height: 1.4;
    color: $grey-font;
    overflow-wrap: anywhere;
  }
}
</style>
