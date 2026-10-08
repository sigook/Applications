<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="title is-5 mb-1">Edit punch</h2>
    <p class="mb-4">{{ attendance.name }} · {{ dayjs(attendance.date).format('ddd, MMM D, YYYY') }}</p>
    <b-message v-if="attendance.isMissingClockOut" type="is-warning" has-icon>
      Missing clock-out. Hours count as 0 until fixed.
    </b-message>
    <div class="columns is-multiline">
      <div class="column is-6">
        <b-field label="Clock in *" :type="formErrors.clockIn ? 'is-danger' : ''" :message="formErrors.clockIn">
          <b-timepicker v-model="clockIn" hour-format="24" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Clock out *" :type="formErrors.clockOut ? 'is-danger' : ''" :message="formErrors.clockOut">
          <b-timepicker v-model="clockOut" hour-format="24" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field label="Lunch (minutes) *" :type="formErrors.lunchMinutes ? 'is-danger' : ''"
          :message="formErrors.lunchMinutes">
          <b-numberinput v-model="lunchMinutes" :min="0" :max="240" :step="15" controls-alignment="right" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field label="Reason *" :type="formErrors.reason ? 'is-danger' : ''" :message="formErrors.reason">
          <b-input v-model="reason" type="textarea" maxlength="500" rows="3" />
        </b-field>
      </div>
      <div class="column is-12">
        <nav class="level is-mobile">
          <div class="level-item has-text-centered">
            <div>
              <p class="heading">Worked</p>
              <p class="title is-5">{{ formatHours(preview.workedHours) }}</p>
            </div>
          </div>
          <div class="level-item has-text-centered">
            <div>
              <p class="heading">Lunch</p>
              <p class="title is-5">{{ formatHours(preview.lunchHours) }}</p>
            </div>
          </div>
          <div class="level-item has-text-centered">
            <div>
              <p class="heading">Regular</p>
              <p class="title is-5">{{ formatHours(preview.regularHours) }}</p>
            </div>
          </div>
          <div class="level-item has-text-centered">
            <div>
              <p class="heading">Overtime</p>
              <p class="title is-5">{{ formatHours(preview.overtimeHours) }}</p>
            </div>
          </div>
        </nav>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="save">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import * as yup from 'yup';
import dayjs from 'dayjs';
import { updateAttendance } from '@/modules/agency/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { calculateAttendanceHours, formatHours, toLocalDateTime } from '@/modules/agency/profile/attendance';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import type { UserAttendanceListItem } from '@/modules/agency/profile/types';

const props = defineProps<{ attendance: UserAttendanceListItem }>();
const emit = defineEmits<{ (e: 'saved'): void }>();

const schema = yup.object({
  clockIn: yup.date().required('Clock in is required'),
  clockOut: yup.date().required('Clock out is required')
    .test('after', 'Clock out must be after clock in', (value, context) =>
      !value || !context.parent.clockIn || value > context.parent.clockIn),
  lunchMinutes: yup.number().typeError('Lunch is required').required('Lunch is required')
    .min(0, 'Min 0 minutes').max(240, 'Max 240 minutes'),
  reason: yup.string().required('Reason is required').max(500, 'Max 500 characters'),
});

const form = useStickyForm<{ clockIn: Date | null; clockOut: Date | null; lunchMinutes: number; reason: string }>({
  schema,
  initialValues: {
    clockIn: dayjs(props.attendance.clockIn).toDate(),
    clockOut: props.attendance.clockOut ? dayjs(props.attendance.clockOut).toDate() : null,
    lunchMinutes: props.attendance.lunchMinutes,
    reason: props.attendance.editReason ?? '',
  },
});
const { clockIn, clockOut, lunchMinutes, reason } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);

function onAttendanceDay(time: Date | null): dayjs.Dayjs | null {
  if (!time) return null;
  const value = dayjs(time);
  return dayjs(props.attendance.date).hour(value.hour()).minute(value.minute()).second(0);
}

const preview = computed(() =>
  calculateAttendanceHours(onAttendanceDay(clockIn.value), onAttendanceDay(clockOut.value), lunchMinutes.value ?? 0));

async function save() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  isLoading.value = true;
  updateAttendance(props.attendance.id, {
    clockIn: toLocalDateTime(onAttendanceDay(clockIn.value)),
    clockOut: toLocalDateTime(onAttendanceDay(clockOut.value)),
    lunchMinutes: lunchMinutes.value,
    reason: reason.value,
  })
    .then(() => emit('saved'))
    .catch(showAlertError)
    .finally(() => { isLoading.value = false; });
}
</script>
