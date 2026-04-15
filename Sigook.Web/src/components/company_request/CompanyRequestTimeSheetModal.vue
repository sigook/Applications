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
    <b-message type="is-info" v-if="this.localEditableDay.clockIn && !this.localEditableDay.clockOut" has-icon>
        The worker didn't clock out. Please enter the total hours worked in the "Hours Approved" field.
    </b-message>
    <div class="container-flex">
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Hours Approved'">
          <b-timepicker v-model="localEditableDay.hoursApprovedToDate" name="timeOut" hour-format="24"
            :max-time="maximumDailyHours">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Missing Hours'">
          <b-timepicker v-model="localEditableDay.missinghoursToDate" name="timeOut" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="localEditableDay.missingHoursOvertimeToDate" name="timeOut" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateHours(localEditableDay)">{{ "Save" }}</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { dateMonth, dateHHmm, hour } from '@/utils/filters';
import dayjs from "dayjs";
import { maximumHoursPerDay } from "@/constants/catalog";
import { validateHoursTimeSheet } from "@/api/companyApi";

export default {
  props: ['editableDay', 'worker'],
  data() {
    const maximumMissing = new Date();
    maximumMissing.setHours(12);
    maximumMissing.setMinutes(0);
    maximumMissing.setSeconds(0);
    return {
      timeOutInvalid: false,
      maximumMissing: maximumMissing,
      isLoading: false,
      localEditableDay: JSON.parse(JSON.stringify(this.editableDay))
    }
  },
  watch: {
    editableDay: {
      handler(newVal) {
        this.localEditableDay = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  methods: {
    dateMonth,
    dateHHmm,
    hour,
    validateHours(item) {
      this.$validator.validateAll().then((result) => {
        if (result && !this.timeOutInvalid) {
          this.sendValidation(item);
        }
      });
    },
    sendValidation(item) {
      this.isLoading = true;
      let timeInZero = dayjs(item.timeIn).hour(0).minute(0).second(0);
      let model = {
        hours: dayjs(item.hoursApprovedToDate).format('HH:mm:ss'),
        timeIn: timeInZero.format('YYYY-MM-DDTHH:mm:ss'),
        missingHours: dayjs(item.missinghoursToDate).format("HH:mm:ss"),
        missingHoursOvertime: dayjs(item.missingHoursOvertimeToDate).format("HH:mm:ss")
      };
      validateHoursTimeSheet(this.$route.params.id, this.worker.workerId, item.id, model)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Updated');
          this.$emit("updateData")
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  },
  computed: {
    maximumDailyHours() {
      const maxHours = maximumHoursPerDay;
      if (maxHours) {
        const maximum = new Date();
        maximum.setHours(maxHours);
        maximum.setMinutes(0);
        maximum.setSeconds(0);
        return maximum;
      }
      return this.maximumMissing;
    }
  }
}
</script>