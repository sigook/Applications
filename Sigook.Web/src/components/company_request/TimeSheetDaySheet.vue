<template>
  <SheetPanel :model-value="modelValue" :title="title" @update:modelValue="(v) => emit('update:modelValue', v)">
    <template v-if="localEditableDay">
      <div class="container-worker-report" v-if="!isCreate">
        <h3 class="fz-0">Worker Report</h3>
        <div class="is-flex is-justify-content-space-between">
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Clock In</span>
            <span v-if="localEditableDay.clockIn">{{ dateHHmm(localEditableDay.clockIn) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Clock Out</span>
            <span v-if="localEditableDay.clockOut">{{ dateHHmm(localEditableDay.clockOut) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Hours</span>
            <span v-if="localEditableDay.clockOut && localEditableDay.totalHours">
              {{ hour(localEditableDay.totalHours) }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
      <b-message type="is-info" v-if="localEditableDay.clockIn && !localEditableDay.clockOut" has-icon>
        The worker didn't clock out. Please enter the total hours worked in the "Hours Approved" field.
      </b-message>
      <template v-if="isCreate">
        <b-field label="Hours Approved" :type="hoursError ? 'is-danger' : ''" :message="hoursError">
          <b-numberinput v-model="newHours" placeholder="Hours" step="0.01" :controls="false"
            title="Approved hours"></b-numberinput>
        </b-field>
      </template>
      <template v-else>
        <b-field label="Hours Approved">
          <b-timepicker v-model="localEditableDay.hoursApprovedToDate" name="hoursApproved" hour-format="24"
            :max-time="maximumDailyHours" append-to-body>
          </b-timepicker>
        </b-field>
        <div class="columns is-mobile is-multiline">
          <div class="column is-6-mobile is-6">
            <b-field label="Missing Hours">
              <b-timepicker v-model="localEditableDay.missinghoursToDate" name="missingHours" hour-format="24"
                :max-time="maximumMissing" append-to-body>
              </b-timepicker>
            </b-field>
          </div>
          <div class="column is-6-mobile is-6">
            <b-field label="Missing Hours OT">
              <b-timepicker v-model="localEditableDay.missingHoursOvertimeToDate" name="missingHoursOvertime"
                hour-format="24" :max-time="maximumMissing" append-to-body>
              </b-timepicker>
            </b-field>
          </div>
        </div>
      </template>
    </template>
    <template #foot>
      <b-button v-if="!isCreate && localEditableDay?.canUpdate" type="is-danger is-light"
        @click="emit('delete')">Delete</b-button>
      <b-button type="is-primary" @click="save">Save</b-button>
    </template>
  </SheetPanel>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import dayjs from 'dayjs';
import SheetPanel from '@/components/responsive/SheetPanel.vue';
import { dateHHmm, hour } from '@/utils/filters';
import { maximumHoursPerDay } from '@/constants/catalog';
import type { PunchCardEditableDay, TimeSheetModel } from '@/types/company';

const props = defineProps<{
  modelValue: boolean;
  editableDay: PunchCardEditableDay | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'save', model: TimeSheetModel): void;
  (e: 'post', hours: number): void;
  (e: 'delete'): void;
}>();

function cloneEditable(src: PunchCardEditableDay | null): PunchCardEditableDay | null {
  if (!src) return src;
  const copy = JSON.parse(JSON.stringify(src)) as PunchCardEditableDay;
  const dateFields = ['hoursApprovedToDate', 'missinghoursToDate', 'missingHoursOvertimeToDate'] as const;
  for (const key of dateFields) {
    if (copy[key]) copy[key] = new Date(copy[key]);
  }
  return copy;
}

const maximumMissing = new Date();
maximumMissing.setHours(12);
maximumMissing.setMinutes(0);
maximumMissing.setSeconds(0);

const localEditableDay = ref<PunchCardEditableDay | null>(cloneEditable(props.editableDay));
const newHours = ref<number | null>(null);
const hoursError = ref('');

watch(() => props.editableDay, (newVal) => {
  localEditableDay.value = cloneEditable(newVal);
  newHours.value = null;
  hoursError.value = '';
}, { deep: true });

const isCreate = computed(() => !localEditableDay.value?.id);

const title = computed(() =>
  localEditableDay.value ? dayjs(localEditableDay.value.day).format('dddd, MMMM D') : '',
);

const maximumDailyHours = computed(() => {
  const maximum = new Date();
  maximum.setHours(maximumHoursPerDay);
  maximum.setMinutes(0);
  maximum.setSeconds(0);
  return maximum;
});

function save() {
  const item = localEditableDay.value;
  if (!item) return;
  if (isCreate.value) {
    const value = Number(newHours.value);
    if (newHours.value == null || isNaN(value) || value < 0 || value > maximumHoursPerDay) {
      hoursError.value = `Hours must be between 0 and ${maximumHoursPerDay}`;
      return;
    }
    if (!/^\d+(\.\d{1,2})?$/.test(String(newHours.value))) {
      hoursError.value = 'Max 2 decimal places';
      return;
    }
    hoursError.value = '';
    emit('post', value);
    return;
  }
  const timeInZero = dayjs(item.timeIn).hour(0).minute(0).second(0);
  emit('save', {
    hours: dayjs(item.hoursApprovedToDate).format('HH:mm:ss'),
    timeIn: timeInZero.format('YYYY-MM-DDTHH:mm:ss'),
    missingHours: dayjs(item.missinghoursToDate).format('HH:mm:ss'),
    missingHoursOvertime: dayjs(item.missingHoursOvertimeToDate).format('HH:mm:ss'),
  });
}
</script>
