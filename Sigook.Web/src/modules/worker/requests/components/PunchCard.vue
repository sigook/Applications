<template>
  <div class="punch">
    <b-loading v-model="isLoading"></b-loading>
    <DataEntryTerms></DataEntryTerms>

    <div class="punch-card">
      <div class="punch-card__head">
        <span class="punch-card__head-label">Punch card</span>
        <span class="punch-card__head-date">{{ date(today) }}</span>
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

        <div class="punch-geo" :class="'is-' + geo.state">
          <b-icon :icon="geo.icon" size="is-small"></b-icon>
          <div>
            <div class="punch-geo__title">{{ geo.title }}</div>
            <div v-if="geo.subtitle" class="punch-geo__sub">{{ geo.subtitle }}</div>
            <div v-if="geo.coordinates" class="punch-geo__coords">
              <b-icon icon="crosshairs-gps" size="is-small"></b-icon>
              {{ geo.coordinates }}
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
          <b-button class="punch-cta" :class="{ 'punch-end': phase === 'working', 'punch-retry': phase === 'error' }"
            type="is-link" rounded expanded :disabled="!canPunch && phase !== 'error'" @click="onCta">
            {{ ctaLabel }}
          </b-button>
          <div v-if="ctaHint" class="punch-hint">{{ ctaHint }}</div>
        </template>

        <div v-if="punchRejected" class="punch-rejected">
          <b-icon icon="map-marker-off-outline" size="is-small"></b-icon>
          <div class="punch-rejected__body">
            <div class="punch-rejected__title">Too far from workplace</div>
            <div class="punch-rejected__message">{{ punchRejected.message }}</div>
            <div class="punch-rejected__ref">{{ punchRejected.reference }}</div>
          </div>
          <button type="button" class="punch-rejected__close" aria-label="Dismiss" @click="punchRejected = null">
            <b-icon icon="close" size="is-small"></b-icon>
          </button>
        </div>

        <div class="punch-stamps">
          <div class="punch-stamps__cell">
            <div class="punch-stamps__label">Clock in</div>
            <div class="punch-stamps__value" :class="{ 'is-empty': !status.clockIn }">
              {{ status.clockIn ? time(status.clockIn) : '--:--' }}
            </div>
          </div>
          <div class="punch-stamps__sep"></div>
          <div class="punch-stamps__cell">
            <div class="punch-stamps__label">Clock out</div>
            <div class="punch-stamps__value" :class="{ 'is-empty': !status.clockOut }">
              {{ status.clockOut ? time(status.clockOut) : '--:--' }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="punch-foot">
      <span>This week: {{ hour(status.weekHours) }} h</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from 'vue';
import { useAppStore } from '@/app/stores/app';
import { useWorkerStore } from '@/modules/worker/store';
import { getErrorMessage, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import dayjs from 'dayjs';
import { workerRegisterTime, getPunchStatus } from '@/modules/worker/requests/api';
import { ClockType } from '@/shared/constants/enums';
import { date, time, hour } from '@/shared/format';
import { distanceInMeters, cardinalDirection, formatDistance } from '@/shared/utils/geo';
import type { PunchStatus, WorkerRequestDetail } from '@/modules/worker/requests/types';
import DataEntryTerms from '@/shared/punch-card/DataEntryTerms.vue';

type GeoState = 'checking' | 'denied' | 'neutral' | 'ok' | 'edge' | 'far';

interface GeoStrip {
  state: GeoState;
  icon: string;
  title: string;
  subtitle: string;
  coordinates?: string;
}

interface PunchRejected {
  message: string;
  reference: string;
}

const props = defineProps<{
  requestId: string;
  request?: WorkerRequestDetail;
}>();

const emptyStatus = (): PunchStatus => ({ clockType: ClockType.None, dayHours: 0, weekHours: 0 });

const emit = defineEmits<{
  (e: 'refreshTimeSheet'): void;
  (e: 'refreshRequest'): void;
}>();

const appStore = useAppStore();
const workerStore = useWorkerStore();

const isLoading = ref(false);
const punchRejected = ref<PunchRejected | null>(null);
const today = ref<Date | null>(null);
const now = ref(dayjs());
const status = ref<PunchStatus>(emptyStatus());
const clockType = computed(() => status.value.clockType);
const clockTypeFailed = ref(false);
const position = ref<GeolocationPosition | null>(null);
const positionResolved = ref(false);
const confirmAction = ref<'in' | 'out' | null>(null);

let markPositionReady: () => void = () => undefined;
const positionReady = new Promise<void>((resolve) => (markPositionReady = resolve));

const hasPosition = computed(() => position.value !== null);

const phase = computed(() => {
  if (clockTypeFailed.value) return 'error';
  if (clockType.value === ClockType.ClockOut) return 'working';
  if (status.value.clockIn && status.value.clockOut) return 'done';
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
    case 'error':
      return 'Status unavailable';
    default:
      return 'Not started';
  }
});

const geo = computed<GeoStrip>(() => {
  if (!positionResolved.value) {
    return { state: 'checking', icon: 'crosshairs-gps', title: 'Finding your location...', subtitle: '' };
  }
  const current = position.value;
  if (!current) {
    return {
      state: 'denied',
      icon: 'alert-outline',
      title: 'Location is turned off',
      subtitle: 'Allow location in your browser so we can confirm you are at the check point.'
    };
  }
  const { latitude, longitude, accuracy } = current.coords;
  const coordinates = `${latitude.toFixed(6)}, ${longitude.toFixed(6)} (±${Math.round(accuracy)} m)`;
  const checkpoint = props.request?.punchCheckpoint;
  if (!checkpoint) {
    return {
      state: 'neutral',
      icon: 'map-marker-outline',
      title: 'Location shared',
      subtitle: `${props.request?.location ?? 'Check point'} · your location is checked when you punch.`,
      coordinates
    };
  }
  const meters = distanceInMeters(checkpoint.latitude, checkpoint.longitude, latitude, longitude);
  if (meters + accuracy <= checkpoint.radiusMeters) {
    return { state: 'ok', icon: 'check-circle-outline', title: 'At the check point', subtitle: '', coordinates };
  }
  if (meters <= checkpoint.radiusMeters) {
    return {
      state: 'edge',
      icon: 'crosshairs-gps',
      title: `About ${Math.round(meters)} m from the check point (GPS ±${Math.round(accuracy)} m)`,
      subtitle: 'Move a bit closer to be safe.',
      coordinates
    };
  }
  const direction = cardinalDirection(checkpoint.latitude, checkpoint.longitude, latitude, longitude);
  return {
    state: 'far',
    icon: 'map-marker-off-outline',
    title: `About ${formatDistance(meters)} ${direction} of the check point`,
    subtitle: 'Get closer before you clock in.',
    coordinates
  };
});

function punchReference(): string {
  const profile = workerStore.workerProfile;
  const worker = profile.numberId
    ? `Worker #${profile.numberId} · ${[profile.firstName, profile.lastName].filter(Boolean).join(' ')}`
    : 'Worker: unknown';
  return `${worker} · Request #${props.request?.numberId ?? '-'} · ${dayjs().format('MMM DD, YYYY hh:mm A')}`;
}

function elapsed(seconds: number): string {
  const pad = (value: number) => String(value).padStart(2, '0');
  return `${pad(Math.floor(seconds / 3600))}:${pad(Math.floor(seconds / 60) % 60)}:${pad(seconds % 60)}`;
}

const clockValue = computed(() => {
  if (phase.value === 'working' && status.value.clockIn) {
    return elapsed(Math.max(0, now.value.diff(dayjs(status.value.clockIn), 'second')));
  }
  if (phase.value === 'done') return `${hour(status.value.dayHours)} h`;
  if (phase.value === 'none') return '--:--';
  return now.value.format('HH:mm:ss');
});

const clockLabel = computed(() => {
  if (phase.value === 'working' && status.value.clockIn) return `Since ${time(status.value.clockIn)}`;
  if (phase.value === 'done') return 'Sent to your agency for approval';
  if (phase.value === 'none') return 'No shift to punch on this day';
  if (phase.value === 'error') return 'Could not check your shift status';
  if (props.request?.startAt && props.request?.finishAt) {
    return `Request runs ${date(props.request.startAt)} - ${date(props.request.finishAt)}`;
  }
  return 'Current time';
});

const ctaLabel = computed(() => {
  if (phase.value === 'error') return 'Retry';
  if (phase.value === 'working') return 'End shift';
  if (phase.value === 'done') return 'Shift closed';
  return 'Start shift';
});

const ctaHint = computed(() => {
  if (phase.value === 'error') return 'We could not reach the server. Check your connection and retry.';
  if (!hasPosition.value) return 'Turn on location to punch.';
  if (phase.value === 'done') return '';
  if (clockType.value === ClockType.None) return 'There is nothing to punch for this day.';
  return '';
});

function askConfirm() {
  confirmAction.value = phase.value === 'working' ? 'out' : 'in';
}

function onCta() {
  if (phase.value === 'error') {
    retry();
    return;
  }
  askConfirm();
}

async function refreshClockType() {
  await positionReady;
  clockTypeFailed.value = false;
  if (!position.value) {
    status.value = emptyStatus();
    return;
  }
  try {
    status.value = await getPunchStatus(
      props.requestId,
      position.value.coords.latitude,
      position.value.coords.longitude
    );
  } catch {
    status.value = emptyStatus();
    clockTypeFailed.value = true;
  }
}

async function retry() {
  punchRejected.value = null;
  isLoading.value = true;
  emit('refreshRequest');
  await refreshClockType();
  isLoading.value = false;
}

function registerHour() {
  if (!position.value) return;
  const wasClockIn = confirmAction.value === 'in';
  confirmAction.value = null;
  punchRejected.value = null;
  isLoading.value = true;
  return workerRegisterTime(props.requestId, position.value.coords.latitude, position.value.coords.longitude)
    .then(async () => {
      emit('refreshTimeSheet');
      emit('refreshRequest');
      await refreshClockType();
      isLoading.value = false;
      showAlertSuccess(wasClockIn ? 'Enjoy your shift!' : 'Thanks for your job!');
    })
    .catch(async (error: unknown) => {
      isLoading.value = false;
      const data = (error as { data?: unknown }).data;
      const message = (await getErrorMessage(data)).replace(/<br\/>/g, ' ').trim();
      if (/too far/i.test(message)) {
        punchRejected.value = { message, reference: punchReference() };
        return;
      }
      showAlertError(data);
    });
}

function onVisibilityChange() {
  if (document.visibilityState === 'visible') emit('refreshRequest');
}

const timerId = setInterval(() => (now.value = dayjs()), 1000);
document.addEventListener('visibilitychange', onVisibilityChange);

(async () => {
  today.value = await appStore.getCurrentDate();
  navigator.geolocation.watchPosition(
    (p) => {
      position.value = p;
      positionResolved.value = true;
      markPositionReady();
    },
    () => {
      position.value = null;
      positionResolved.value = true;
      markPositionReady();
    },
    { enableHighAccuracy: true, timeout: 10000 }
  );
})();

onUnmounted(() => {
  clearInterval(timerId);
  document.removeEventListener('visibilitychange', onVisibilityChange);
});

watch(today, async (value) => {
  if (value) {
    isLoading.value = true;
    await refreshClockType();
    isLoading.value = false;
  }
});
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

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

  &.is-error {
    background: rgba($danger, 0.08);
    color: $danger-hover;

    .punch-pill__dot {
      background: $danger;
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

  &.is-neutral,
  &.is-checking {
    background: $gray-bg;
    border-color: $border-input;

    .punch-geo__title {
      color: $grey-font;
    }
  }

  &.is-ok {
    background: rgba($green, 0.1);
    border-color: rgba($green, 0.3);

    .punch-geo__title {
      color: $green-text;
    }
  }

  &.is-edge {
    background: rgba($accent, 0.1);
    border-color: rgba($accent, 0.3);

    .punch-geo__title {
      color: $accent-text;
    }
  }

  &.is-far {
    background: rgba($danger, 0.08);
    border-color: rgba($danger, 0.3);

    .punch-geo__title {
      color: $danger-hover;
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

.punch-geo__coords {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  margin-top: 4px;
  font-size: 12px;
  color: $grey-light;
  font-variant-numeric: tabular-nums;
}

.punch-rejected {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px;
  border: 1px solid rgba($danger, 0.3);
  border-radius: 10px;
  background: rgba($danger, 0.08);
  color: $danger-hover;
}

.punch-rejected__body {
  flex: 1;
  min-width: 0;
}

.punch-rejected__title {
  font-size: 13px;
  font-weight: 600;
}

.punch-rejected__message {
  margin-top: 2px;
  font-size: 13px;
  color: $navy;
  text-wrap: pretty;
}

.punch-rejected__ref {
  margin-top: 4px;
  font-size: 11px;
  color: $grey-light;
}

.punch-rejected__close {
  display: inline-flex;
  padding: 0;
  border: 0;
  background: none;
  color: $grey-light;
  cursor: pointer;

  &:hover,
  &:focus-visible {
    color: $navy;
  }
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

.punch-retry {
  background-color: $accent;
  border-color: $accent;
  color: $white;

  &:hover,
  &:focus {
    background-color: $accent;
    border-color: $accent;
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
