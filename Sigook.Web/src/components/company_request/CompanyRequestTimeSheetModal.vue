<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> {{ editableDay.day | dateMonth }} </h2>
    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock In</span>
            <span v-if="editableDay.clockIn">{{ editableDay.clockIn | dateHHmm }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock Out</span>
            <span v-if="editableDay.clockOut">{{ editableDay.clockOut | dateHHmm }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Hours</span>
            <span v-if="editableDay.clockOut && editableDay.totalHours">
              {{ editableDay.totalHours | hour }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
    </div>
    <b-message type="is-info" v-if="this.editableDay.clockIn && !this.editableDay.clockOut" has-icon>
        The worker didn't clock out. Please enter the total hours worked in the "Hours Approved" field.
    </b-message>
    <div class="container-flex">
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="$t('HoursApproved')">
          <b-timepicker v-model="editableDay.hoursApprovedToDate" name="timeOut" hour-format="24"
            :max-time="maximumDailyHours">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="$t('MissingHours')">
          <b-timepicker v-model="editableDay.missinghoursToDate" name="timeOut" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="editableDay.missingHoursOvertimeToDate" name="timeOut" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateHours(editableDay)">{{ $t("Save") }}</b-button>
      </div>
    </div>
  </div>
</template>
<script>
import dayjs from "dayjs";

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
      isLoading: false
    }
  },
  methods: {
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
        missingHoursOvertime: dayjs(item.missingHoursOvertimeToDate).format("HH:mm:ss"),
      };
      this.$store.dispatch('company/validateHoursTimeSheet', { requestId: this.$route.params.id, workerId: this.worker.workerId, id: item.id, model: model })
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess('Updated');
          this.$emit("updateData")
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
  computed: {
    maximumDailyHours() {
      if (this.$store.state.catalog.maximumHoursPerDay) {
        const maximum = new Date();
        maximum.setHours(this.$store.state.catalog.maximumHoursPerDay);
        maximum.setMinutes(0);
        maximum.setSeconds(0);
        return maximum;
      }
      return this.maximumMissing;
    }
  }
}
</script>