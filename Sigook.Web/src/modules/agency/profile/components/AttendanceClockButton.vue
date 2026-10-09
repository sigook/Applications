<template>
  <div v-if="attendance" class="attendance-clock">
    <b-tooltip :label="tooltip" type="is-dark" position="is-top" append-to-body>
      <button type="button" class="attendance-clock-button" :class="stateClass" :aria-label="tooltip"
        :disabled="isSaving" :aria-disabled="isClosed" @click="onClick">
        <b-icon :icon="icon" size="is-small"></b-icon>
      </button>
    </b-tooltip>

    <b-modal custom-content-class="card" v-model="showConfirm" width="440px">
      <div class="p-3">
        <h2 class="title is-5">Clock out for today?</h2>
        <div class="columns is-mobile">
          <div class="column">
            <p class="heading">Clock in</p>
            <p class="title is-4">{{ clockInLabel }}</p>
          </div>
          <div class="column">
            <p class="heading">Clock out (now)</p>
            <p class="title is-4">{{ nowLabel }}</p>
          </div>
        </div>
        <table class="table is-fullwidth is-narrow">
          <tbody>
            <tr>
              <th>Worked</th>
              <th class="has-text-right">{{ formatHours(preview.workedHours) }} h</th>
            </tr>
            <tr>
              <td>Regular</td>
              <td class="has-text-right">{{ formatHours(preview.regularHours) }} h</td>
            </tr>
            <tr class="attendance-clock-overtime">
              <th>Overtime</th>
              <th class="has-text-right">{{ formatHours(preview.overtimeHours) }} h</th>
            </tr>
          </tbody>
        </table>
        <p class="is-size-7 mb-4">You can only clock in once per day. Ask an admin to fix a wrong punch.</p>
        <div class="buttons is-right">
          <b-button @click="showConfirm = false">Cancel</b-button>
          <b-button type="is-danger" :loading="isSaving" @click="toggle">Clock out</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue';
import dayjs from 'dayjs';
import { getAttendanceToday, toggleAttendance } from '@/modules/agency/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { calculateAttendanceHours, formatElapsed, formatHours } from '@/modules/agency/profile/attendance';
import { AttendanceStatus, type UserAttendanceToday } from '@/modules/agency/profile/types';

const attendance = ref<UserAttendanceToday | null>(null);
const isSaving = ref(false);
const showConfirm = ref(false);
const now = ref(dayjs());
let timer: ReturnType<typeof setInterval> | undefined;

const isRunning = computed(() => attendance.value?.status === AttendanceStatus.ClockedIn);
const isClosed = computed(() => attendance.value?.status === AttendanceStatus.ClockedOut);

const icon = computed(() => {
  if (isRunning.value) return 'stop';
  if (isClosed.value) return 'check';
  return 'play';
});

const stateClass = computed(() => ({
  'is-idle': !isRunning.value && !isClosed.value,
  'is-running': isRunning.value,
  'is-closed': isClosed.value,
}));

const clockInLabel = computed(() => attendance.value?.clockIn ? dayjs(attendance.value.clockIn).format('HH:mm') : '');
const nowLabel = computed(() => now.value.format('HH:mm'));
const preview = computed(() => calculateAttendanceHours(attendance.value?.clockIn, now.value));

const tooltip = computed(() => {
  const current = attendance.value;
  if (!current) return '';
  if (current.status === AttendanceStatus.ClockedIn)
    return `Clock out · in since ${clockInLabel.value} (${formatElapsed(current.clockIn, now.value)})`;
  if (current.status === AttendanceStatus.ClockedOut)
    return `Day closed · ${formatHours(current.workedHours)} h worked`;
  return 'Clock in';
});

function load() {
  getAttendanceToday()
    .then(response => { attendance.value = response; })
    .catch(showAlertError);
}

function onClick() {
  if (isClosed.value) return;
  now.value = dayjs();
  if (isRunning.value) {
    showConfirm.value = true;
    return;
  }
  toggle();
}

function toggle() {
  isSaving.value = true;
  toggleAttendance()
    .then(response => {
      attendance.value = response;
      showConfirm.value = false;
    })
    .catch(showAlertError)
    .finally(() => { isSaving.value = false; });
}

onMounted(() => {
  load();
  timer = setInterval(() => { now.value = dayjs(); }, 30000);
});

onUnmounted(() => clearInterval(timer));
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";

.attendance-clock {
  flex: 0 0 auto;
  display: flex;
}

.attendance-clock-button {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  cursor: pointer;

  &.is-idle {
    background-color: $green-text;
  }

  &.is-running {
    background-color: $danger;
    box-shadow: 0 0 0 3px rgba($danger, 0.15);

    &:hover {
      background-color: $danger-hover;
    }
  }

  &.is-closed {
    background-color: $gray-bg;
    border: 1px solid $gray-border;
    color: $grey-font;
    cursor: default;
  }
}

.attendance-clock-overtime th {
  color: $accent-text;
}
</style>
