<template>
  <div class="punch">
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>

    <div class="punch-card">
      <div class="punch-card__head">
        <span class="punch-card__head-label">Punch card</span>
        <span class="punch-card__head-date">{{ date(punchDate) }}</span>
      </div>

      <div class="punch-card__body">
        <span class="punch-pill" :class="'is-' + phase">
          <span class="punch-pill__dot"></span>
          {{ pillText }}
        </span>

        <div class="punch-clock">
          <div class="punch-clock__value" :class="{ 'is-empty': phase === 'none' }">{{ clockValue }}</div>
          <div class="punch-clock__label">{{ clockLabel }}</div>
        </div>

        <div class="punch-geo" :class="hasPosition ? 'is-neutral' : 'is-denied'">
          <b-icon :icon="hasPosition ? 'map-marker-outline' : 'alert-outline'" size="is-small"></b-icon>
          <div>
            <div class="punch-geo__title">{{ hasPosition ? 'Location shared' : 'Location is turned off' }}</div>
            <div class="punch-geo__sub">
              <template v-if="hasPosition">
                {{ request && request.location ? request.location : 'Check point' }} &middot; your location is checked when you punch.
              </template>
              <template v-else>
                Allow location in your browser so we can confirm you are at the check point.
              </template>
            </div>
          </div>
        </div>

        <div v-if="confirmAction" class="punch-confirm">
          <div class="punch-confirm__question">
            {{ confirmAction === 'in' ? 'Start your shift now?' : 'End your shift now?' }}
          </div>
          <div class="punch-confirm__actions">
            <b-button rounded expanded @click="confirmAction = null">Cancel</b-button>
            <b-button type="is-link" :class="{ 'punch-end': confirmAction === 'out' }" rounded expanded
              @click="registerHour">
              Confirm
            </b-button>
          </div>
        </div>

        <template v-else>
          <b-button class="punch-cta" :class="{ 'punch-end': phase === 'working' }" type="is-link" rounded expanded
            :disabled="!canPunch" @click="askConfirm">
            {{ ctaLabel }}
          </b-button>
          <div v-if="ctaHint" class="punch-hint">{{ ctaHint }}</div>
        </template>

        <div class="punch-stamps">
          <div class="punch-stamps__cell">
            <div class="punch-stamps__label">Clock in</div>
            <div class="punch-stamps__value" :class="{ 'is-empty': !activeItem || !activeItem.clockIn }">
              {{ activeItem && activeItem.clockIn ? time(activeItem.clockIn) : '--:--' }}
            </div>
          </div>
          <div class="punch-stamps__sep"></div>
          <div class="punch-stamps__cell">
            <div class="punch-stamps__label">Clock out</div>
            <div class="punch-stamps__value" :class="{ 'is-empty': !activeItem || !activeItem.clockOut }">
              {{ activeItem && activeItem.clockOut ? time(activeItem.clockOut) : '--:--' }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="punch-foot">
      <span>This week: {{ hour(weekHours) }} h</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from 'vue';
import { useAppStore } from '@/stores/app';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import dayjs from 'dayjs';
import { workerRegisterTime, getClockType as getClockTypeApi } from '@/api/workerApi';
import { ClockType } from '@/constants/enums';
import { date, time, hour } from '@/utils/filters';
import type { PaginatedList } from '@/types/common';
import type { WorkerRequestDetail, WorkerTimeSheetItem } from '@/types/worker';
import DataEntryTerms from '../../components/DataEntryTerms.vue';

const props = defineProps<{
  requestId: string;
  timesheet?: PaginatedList<WorkerTimeSheetItem>;
  request?: WorkerRequestDetail;
}>();

const emit = defineEmits<{
  (e: 'refreshTimeSheet'): void;
}>();

const appStore = useAppStore();

const isLoading = ref(false);
const today = ref<Date | null>(null);
const now = ref(dayjs());
const clockType = ref<ClockType>(ClockType.None);
const position = ref<GeolocationPosition | null>(null);
const confirmAction = ref<'in' | 'out' | null>(null);

let markPositionReady: () => void = () => undefined;
const positionReady = new Promise<void>((resolve) => (markPositionReady = resolve));

const items = computed<WorkerTimeSheetItem[]>(() => props.timesheet?.items ?? []);

const hasPosition = computed(() => position.value !== null);

const openItem = computed(() => {
  return (
    items.value.find((item) => item.clockIn && !item.clockOut && dayjs().diff(dayjs(item.clockIn), 'hour') < 14) ?? null
  );
});

const todayItem = computed(() => {
  if (!today.value) return null;
  return items.value.find((item) => dayjs(item.day).isSame(today.value, 'day')) ?? null;
});

const activeItem = computed(() => openItem.value ?? todayItem.value);

const punchDate = computed(() => (openItem.value ? dayjs(openItem.value.day).toDate() : today.value));

const phase = computed(() => {
  if (openItem.value) return 'working';
  if (todayItem.value?.clockIn && todayItem.value?.clockOut) return 'done';
  if (hasPosition.value && clockType.value === ClockType.None) return 'none';
  return 'idle';
});

const canPunch = computed(() => hasPosition.value && clockType.value !== ClockType.None);

const pillText = computed(() => {
  switch (phase.value) {
    case 'working':
      return 'Working';
    case 'done':
      return 'Shift completed';
    case 'none':
      return 'Nothing to punch';
    default:
      return 'Not started';
  }
});

function elapsed(seconds: number): string {
  const pad = (value: number) => String(value).padStart(2, '0');
  return `${pad(Math.floor(seconds / 3600))}:${pad(Math.floor(seconds / 60) % 60)}:${pad(seconds % 60)}`;
}

const clockValue = computed(() => {
  if (phase.value === 'working' && activeItem.value?.clockIn) {
    return elapsed(Math.max(0, now.value.diff(dayjs(activeItem.value.clockIn), 'second')));
  }
  if (phase.value === 'done') return `${hour(activeItem.value?.totalHours ?? 0)} h`;
  if (phase.value === 'none') return '--:--';
  return now.value.format('HH:mm:ss');
});

const clockLabel = computed(() => {
  if (phase.value === 'working' && activeItem.value?.clockIn) return `Since ${time(activeItem.value.clockIn)}`;
  if (phase.value === 'done') return 'Sent to your agency for approval';
  if (phase.value === 'none') return 'No shift to punch on this day';
  if (props.request?.startAt && props.request?.finishAt) {
    return `Request runs ${date(props.request.startAt)} - ${date(props.request.finishAt)}`;
  }
  return 'Current time';
});

const ctaLabel = computed(() => {
  if (phase.value === 'working') return 'End shift';
  if (phase.value === 'done') return 'Shift closed';
  return 'Start shift';
});

const ctaHint = computed(() => {
  if (!hasPosition.value) return 'Turn on location to punch.';
  if (phase.value === 'done') return '';
  if (clockType.value === ClockType.None) return 'There is nothing to punch for this day.';
  return '';
});

const weekHours = computed(() => {
  const start = dayjs(today.value ?? undefined).startOf('week');
  return items.value
    .filter((item) => dayjs(item.day).isAfter(start))
    .reduce((total, item) => total + (item.totalHours ?? 0), 0);
});

function askConfirm() {
  confirmAction.value = phase.value === 'working' ? 'out' : 'in';
}

async function refreshClockType() {
  await positionReady;
  if (!position.value || !punchDate.value) {
    clockType.value = ClockType.None;
    return;
  }
  clockType.value = await getClockTypeApi(
    props.requestId,
    position.value.coords.latitude,
    position.value.coords.longitude,
    dayjs(punchDate.value).format('YYYY-MM-DD')
  );
}

function registerHour() {
  if (!position.value) return;
  const wasClockIn = confirmAction.value === 'in';
  confirmAction.value = null;
  isLoading.value = true;
  return workerRegisterTime(props.requestId, position.value.coords.latitude, position.value.coords.longitude)
    .then(async () => {
      emit('refreshTimeSheet');
      await refreshClockType();
      isLoading.value = false;
      showAlertSuccess(wasClockIn ? 'Enjoy your shift!' : 'Thanks for your job!');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError((error as { data?: unknown }).data);
    });
}

const timerId = setInterval(() => (now.value = dayjs()), 1000);

(async () => {
  today.value = await appStore.getCurrentDate();
  navigator.geolocation.watchPosition(
    (p) => {
      position.value = p;
      markPositionReady();
    },
    () => {
      position.value = null;
      markPositionReady();
    },
    { timeout: 10000 }
  );
})();

onUnmounted(() => {
  clearInterval(timerId);
});

watch(punchDate, async (value) => {
  if (value) {
    isLoading.value = true;
    await refreshClockType();
    isLoading.value = false;
  }
});
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';
@import '../../assets/scss/breakpoints';

.punch {
  max-width: 460px;
  margin: 24px auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.punch-card {
  background: $white;
  border: 1px solid $border-input;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.punch-card__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  min-height: 52px;
  padding: 10px 16px;
  background: $gray-bg;
  border-bottom: 1px solid $border-input;
}

.punch-card__head-label {
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: $grey-light;
}

.punch-card__head-date {
  font-size: 13px;
  font-weight: 600;
  color: $navy;
}

.punch-card__body {
  padding: 20px 16px;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.punch-pill {
  align-self: center;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  height: 26px;
  padding: 0 12px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 600;

  .punch-pill__dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
  }

  &.is-idle {
    background: rgba($accent, 0.12);
    color: $accent-text;

    .punch-pill__dot {
      background: $accent;
    }
  }

  &.is-working {
    background: rgba($primary, 0.12);
    color: $blue;

    .punch-pill__dot {
      background: $primary;
      animation: pulse-blue 2s infinite;
    }
  }

  &.is-done {
    background: rgba($green, 0.12);
    color: $green-text;

    .punch-pill__dot {
      background: $green;
    }
  }

  &.is-none {
    background: $gray-bg;
    color: $grey-light;

    .punch-pill__dot {
      background: #bbbaba;
    }
  }
}

.punch-clock {
  text-align: center;
}

.punch-clock__value {
  font-size: 46px;
  font-weight: 300;
  letter-spacing: 2px;
  line-height: 1.05;
  color: $navy;
  font-variant-numeric: tabular-nums;

  &.is-empty {
    color: rgba($grey-light, 0.5);
  }
}

.punch-clock__label {
  font-size: 12px;
  color: $grey-light;
  margin-top: 6px;
}

.punch-geo {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px;
  border: 1px solid;
  border-radius: 10px;

  &.is-neutral {
    background: $gray-bg;
    border-color: $border-input;

    .punch-geo__title {
      color: $grey-font;
    }
  }

  &.is-denied {
    background: rgba($danger, 0.08);
    border-color: rgba($danger, 0.3);

    .punch-geo__title {
      color: $danger-hover;
    }
  }
}

.punch-geo__title {
  font-size: 13px;
  font-weight: 600;
}

.punch-geo__sub {
  font-size: 12px;
  color: $grey-light;
  margin-top: 2px;
  text-wrap: pretty;
}

.punch-confirm {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 14px;
  border: 1px solid $border-input;
  border-radius: 12px;
  background: $gray-bg;
}

.punch-confirm__question {
  font-size: 14px;
  font-weight: 600;
  text-align: center;
  color: $navy;
}

.punch-confirm__actions {
  display: flex;
  gap: 8px;

  :deep(.button) {
    height: 48px;
  }
}

.punch-cta {
  height: 56px;
  font-size: 15px;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.punch-end {
  background-color: $navy;
  border-color: $navy;
  color: $white;

  &:hover,
  &:focus {
    background-color: $navy;
    border-color: $navy;
    color: $white;
    opacity: 0.9;
  }
}

.punch-hint {
  font-size: 12px;
  color: $grey-light;
  text-align: center;
  margin-top: -6px;
  text-wrap: pretty;
}

.punch-stamps {
  display: flex;
  align-items: stretch;
  border-top: 1px solid $gray-border;
  padding-top: 14px;
}

.punch-stamps__cell {
  flex: 1 1 0;
  text-align: center;
}

.punch-stamps__sep {
  width: 1px;
  background: $gray-border;
}

.punch-stamps__label {
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: $grey-light;
}

.punch-stamps__value {
  font-size: 20px;
  font-weight: 600;
  color: $navy;
  font-variant-numeric: tabular-nums;
  margin-top: 2px;

  &.is-empty {
    color: rgba($grey-light, 0.5);
  }
}

.punch-foot {
  display: flex;
  justify-content: flex-end;
  padding: 0 4px;
  font-size: 12px;
  color: $grey-light;
}

@include mobile {
  .punch {
    margin: 12px 0;
  }
}
</style>
