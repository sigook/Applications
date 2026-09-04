<template>
  <div>
    <div class="cal-head">
      <h2 class="fz1">{{ monthLabel }} <span class="has-text-weight-light">{{ yearLabel }}</span></h2>
      <div class="cal-nav">
        <b-button size="is-small" icon-left="chevron-left" @click="getPreviousMonth"></b-button>
        <b-button size="is-small" @click="getTodayMonth">Today</b-button>
        <b-button size="is-small" icon-right="chevron-right" @click="getNextMonth"></b-button>
      </div>
    </div>
    <div class="week-list">
      <div v-for="(week, indexWeek) in calendar" :key="'agendaWeek' + indexWeek" class="week-card"
        :class="{ 'is-collapsed': !expandedWeeks.has(indexWeek) }">
        <div class="week-header" @click="toggleWeek(indexWeek)">
          <div>
            <div class="week-title">Week {{ indexWeek + 1 }}</div>
            <div class="week-range">{{ weekRange(week) }}</div>
          </div>
          <div class="week-total" @click.stop>
            <template v-if="expandedWeeks.has(indexWeek)">
              <b-numberinput v-model="week.totalHoursWeek" step="0.01" :controls="false"
                class="input-no-arrows"></b-numberinput>
              <b-button type="is-primary" @click="distributeWeekHours(week)">ADD</b-button>
            </template>
            <span v-else class="week-sum">{{ hour(week.totalHoursWeek ?? 0) }} h</span>
            <span class="week-toggle" @click="toggleWeek(indexWeek)">
              <b-icon :icon="expandedWeeks.has(indexWeek) ? 'chevron-up' : 'chevron-down'"></b-icon>
            </span>
          </div>
        </div>
        <template v-if="expandedWeeks.has(indexWeek)">
          <div v-for="(item, indexDay) in week.days" :key="'agendaDay' + indexDay" class="day-row"
            :class="{ 'is-locked': !isDayEditable(item.day) }">
            <div class="day-chip">
              <span class="day-dow">{{ weekdays[indexDay] }}</span>
              <span class="day-num" :class="{
                'has-record': item.id,
                'is-today': isToday(item.day),
                'not-month': notCurrentMonth(item.day),
              }">{{ dayNumber(item.day) }}</span>
            </div>
            <template v-if="!isDayEditable(item.day)">
              <div class="day-info"><span class="day-empty">Outside request period</span></div>
            </template>
            <template v-else-if="item.id">
              <div class="day-info">
                <div class="day-times">
                  <template v-if="item.clockIn">
                    <span>{{ dateHHmm(item.clockIn) }}</span>
                    <span class="arrow">→</span>
                    <span>{{ item.clockOut ? dateHHmm(item.clockOut) : '—' }}</span>
                  </template>
                  <span v-else>Manual entry</span>
                </div>
                <div class="day-status">
                  <b-tag v-if="item.totalHoursApproved" type="is-success is-light">
                    Approved {{ hour(item.totalHoursApproved) }}</b-tag>
                  <b-tag v-else type="is-warning is-light">Pending</b-tag>
                  <span class="day-hours" v-if="item.totalHours">{{ hour(item.totalHours) }} h</span>
                </div>
              </div>
              <div class="day-actions">
                <b-button v-if="!item.canUpdate" type="is-ghost" icon-right="eye"
                  @click="emit('view', item)"></b-button>
                <template v-else>
                  <b-button v-if="!item.totalHoursApproved" type="is-ghost" icon-right="check"
                    @click="emit('approve', item)"></b-button>
                  <b-button type="is-ghost" icon-right="pencil" @click="emit('edit', item)"></b-button>
                </template>
              </div>
            </template>
            <template v-else>
              <div class="day-info">
                <b-field class="inline-hours input-no-arrows" :type="errors[dayKey(item.day)] ? 'is-danger' : ''"
                  :message="errors[dayKey(item.day)] || ''">
                  <b-numberinput v-model="item.totalHoursApproved" placeholder="Hours" step="0.01"
                    title="Approved hours" :controls="false"></b-numberinput>
                  <b-button type="is-ghost" icon-right="check" @click="emit('postHours', item)"></b-button>
                </b-field>
              </div>
              <div class="day-actions" v-if="isToday(item.day)">
                <b-button type="is-ghost" icon-right="clock-outline" @click="emit('clockIn')"></b-button>
              </div>
            </template>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import dayjs from 'dayjs';
import { dateHHmm, hour } from '@/utils/filters';
import { usePunchCardCalendar } from '@/composables/usePunchCardCalendar';
import type { PunchCardDay, PunchCardWorker } from '@/types/company';

const props = withDefaults(
  defineProps<{
    highlights: PunchCardDay[];
    startDate: string | Date;
    worker?: PunchCardWorker;
    errors?: Record<string, string>;
  }>(),
  { errors: () => ({}) },
);

const emit = defineEmits<{
  (e: 'monthChange', range: { startDate: string; endDate: string }): void;
  (e: 'view', day: PunchCardDay): void;
  (e: 'edit', day: PunchCardDay): void;
  (e: 'approve', day: PunchCardDay): void;
  (e: 'postHours', day: PunchCardDay): void;
  (e: 'clockIn'): void;
}>();

const expandedWeeks = ref<Set<number>>(new Set());

const {
  calendar,
  weekdays,
  monthLabel,
  yearLabel,
  getPreviousMonth,
  getNextMonth,
  getTodayMonth,
  isToday,
  notCurrentMonth,
  isDayEditable,
  dayKey,
  weekRange,
  distributeWeekHours,
} = usePunchCardCalendar({
  highlights: () => props.highlights,
  startDate: () => props.startDate,
  worker: () => props.worker,
  onMonthChange: (range) => emit('monthChange', range),
});

function dayNumber(date: string | Date) {
  return dayjs(date).format('DD');
}

function toggleWeek(index: number) {
  const next = new Set(expandedWeeks.value);
  if (next.has(index)) {
    next.delete(index);
  } else {
    next.add(index);
  }
  expandedWeeks.value = next;
}

watch(calendar, (weeks) => {
  const currentWeek = weeks.findIndex((week) => week.days.some((day) => isToday(day.day)));
  expandedWeeks.value = new Set([currentWeek >= 0 ? currentWeek : 0]);
});
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.cal-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 12px;
}

.cal-nav {
  display: flex;
  align-items: center;
  gap: 4px;
}

.week-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.week-card {
  border: 1px solid $border-input;
  border-radius: 12px;
  background: $white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.week-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
  padding: 10px 12px 10px 16px;
  background: $gray-bg;
  border-bottom: 1px solid $border-input;
  min-height: 52px;
  cursor: pointer;
}

.week-card.is-collapsed .week-header {
  border-bottom: 0;
}

.week-title {
  font-weight: 600;
  font-size: 16px;
  line-height: 1.2;
}

.week-range {
  font-size: 12px;
  color: $grey-light;
}

.week-total {
  display: flex;
  align-items: center;
  gap: 6px;

  :deep(.input) {
    width: 72px;
    height: 32px;
    font-size: 14px;
    text-align: center;
  }

  :deep(.button) {
    height: 32px;
  }
}

.week-sum {
  font-size: 14px;
  font-weight: 600;
}

.week-toggle {
  display: inline-flex;
  align-items: center;
  cursor: pointer;
  color: $grey-font;
}

.day-row {
  display: flex;
  align-items: center;
  gap: 10px;
  min-height: 64px;
  padding: 8px 8px 8px 12px;
  border-top: 1px solid $gray-border;

  &:first-of-type {
    border-top: 0;
  }

  &.is-locked {
    background: $gray-bg;

    > * {
      opacity: 0.6;
    }
  }
}

.day-chip {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 44px;
  flex-shrink: 0;
}

.day-dow {
  font-size: 11px;
  font-weight: 600;
  color: $grey-light;
  text-transform: uppercase;
  line-height: 1;
  margin-bottom: 4px;
}

.day-num {
  font-size: 16px;
  font-weight: 600;
  line-height: 22px;

  &.has-record {
    width: 22px;
    height: 22px;
    border-radius: 50%;
    background: $accent;
    color: $white;
    font-size: 13px;
    text-align: center;
  }

  &.is-today {
    color: $blue;
  }

  &.has-record.is-today {
    color: $white;
    box-shadow: 0 0 0 2px $blue;
  }

  &.not-month {
    color: $grey-light;
    opacity: 0.6;
  }
}

.day-info {
  flex: 1 1 auto;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.day-times {
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 6px;

  .arrow {
    color: $grey-light;
  }
}

.day-status {
  display: flex;
  align-items: center;
  gap: 6px;
}

.day-hours {
  font-size: 12px;
  color: $grey-light;
}

.day-empty {
  font-size: 13px;
  color: $grey-light;
}

.day-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;

  :deep(.button) {
    width: 44px;
    height: 44px;
  }
}

.inline-hours {
  margin-bottom: 0;

  :deep(.input) {
    width: 84px;
    height: 40px;
    font-size: 14px;
  }

  :deep(.button) {
    height: 40px;
  }
}
</style>
