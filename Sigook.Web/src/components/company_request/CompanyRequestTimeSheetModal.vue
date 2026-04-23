<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> {{ dateMonth(localEditableDay.day) }} </h2>
    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock In</span>
            <span v-if="localEditableDay.clockIn">{{ dateHHmm(localEditableDay.clockIn) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock Out</span>
            <span v-if="localEditableDay.clockOut">{{ dateHHmm(localEditableDay.clockOut) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Hours</span>
            <span v-if="localEditableDay.clockOut && localEditableDay.totalHours">
              {{ hour(localEditableDay.totalHours) }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
    </div>
    <b-message type="is-info" v-if="localEditableDay.clockIn && !localEditableDay.clockOut" has-icon>
        The worker didn't clock out. Please enter the total hours worked in the "Hours Approved" field.
    </b-message>
    <div class="container-flex">
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Hours Approved'">
          <b-timepicker v-model="localEditableDay.hoursApprovedToDate" name="hoursApproved" hour-format="24"
            :max-time="maximumDailyHours">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Missing Hours'">
          <b-timepicker v-model="localEditableDay.missinghoursToDate" name="missingHours" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="localEditableDay.missingHoursOvertimeToDate" name="missingHoursOvertime" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveHours">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { dateMonth, dateHHmm, hour } from '@/utils/filters';
import dayjs from "dayjs";
import { maximumHoursPerDay } from "@/constants/catalog";
import { validateHoursTimeSheet } from "@/api/companyApi";

const props = defineProps<{ editableDay: any; worker: any }>();
const emit = defineEmits<{ (e: 'updateData'): void }>();
const route = useRoute();

function cloneEditable(src: any): any {
  if (!src) return src;
  const copy = JSON.parse(JSON.stringify(src));
  const dateFields = ['hoursApprovedToDate', 'missinghoursToDate', 'missingHoursOvertimeToDate'];
  for (const key of dateFields) {
    if (copy[key]) copy[key] = new Date(copy[key]);
  }
  return copy;
}

const maximumMissing = new Date();
maximumMissing.setHours(12);
maximumMissing.setMinutes(0);
maximumMissing.setSeconds(0);

const isLoading = ref(false);
const localEditableDay = ref<any>(cloneEditable(props.editableDay));

watch(() => props.editableDay, (newVal) => {
  localEditableDay.value = cloneEditable(newVal);
}, { deep: true });

const maximumDailyHours = computed(() => {
  const maxHours = maximumHoursPerDay;
  if (maxHours) {
    const maximum = new Date();
    maximum.setHours(maxHours);
    maximum.setMinutes(0);
    maximum.setSeconds(0);
    return maximum;
  }
  return maximumMissing;
});

function saveHours() {
  const item = localEditableDay.value;
  isLoading.value = true;
  const timeInZero = dayjs(item.timeIn).hour(0).minute(0).second(0);
  const model = {
    hours: dayjs(item.hoursApprovedToDate).format('HH:mm:ss'),
    timeIn: timeInZero.format('YYYY-MM-DDTHH:mm:ss'),
    missingHours: dayjs(item.missinghoursToDate).format("HH:mm:ss"),
    missingHoursOvertime: dayjs(item.missingHoursOvertimeToDate).format("HH:mm:ss"),
  };
  validateHoursTimeSheet(route.params.id, props.worker.workerId, item.id, model)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      emit("updateData");
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
