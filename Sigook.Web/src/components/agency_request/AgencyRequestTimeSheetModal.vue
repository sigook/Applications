<template>
  <div class="p-3 time-sheet-input">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> {{ editableDay.day | dateMonth }} </h2>
    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock In</span>
              <span v-if="editableDay.clockIn">{{ editableDay.clockIn | dateHHmm }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="editableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ editableDay.clockInRounded | dateHHmm }}</span>
            </div>
          </div>
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock Out</span>
              <span v-if="editableDay.clockOut">{{ editableDay.clockOut | dateHHmm }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="editableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ editableDay.clockOutRounded | dateHHmm }}</span>
            </div>
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
        <b-field :label="$t('HoursApproved')" :type="errors.has('Hours Approved') ? 'is-danger' : ''"
          :message="errors.has('Hours Approved') ? errors.first('Hours Approved') : ''">
          <b-timepicker v-model="editableDay.hoursApprovedToDate" name="Hours Approved" hour-format="24"
            :max-time="maximumDailyHours" v-validate="'required'">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="$t('MissingHours')">
          <b-timepicker v-model="editableDay.missinghoursToDate" name="Missing Hours" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="editableDay.missingHoursOvertimeToDate" name="Missing Hours" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Worker Rate" :type="errors.has('deductionsW') ? 'is-danger' : ''"
          :message="errors.has('deductionsW') ? errors.first('deductionsW') : ''">
          <b-numberinput v-model="editableDay.missingRateWorker" step="0.01" name="deductionsW" controls-alignment="right"
            v-validate="'max_value:100|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Agency Rate" :type="errors.has('deductionsC') ? 'is-danger' : ''"
          :message="errors.has('deductionsC') ? errors.first('deductionsC') : ''">
          <b-numberinput v-model="editableDay.missingRateAgency" step="0.01" name="deductionsC" controls-alignment="right"
            v-validate="'max_value:100|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others" :type="errors.has('deductions') ? 'is-danger' : ''"
          :message="errors.has('deductions') ? errors.first('deductions') : ''">
          <b-numberinput v-model="editableDay.deductionsOthers" step="0.01" name="deductions" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others Description" :type="errors.has('deductionsDes') ? 'is-danger' : ''"
          :message="errors.has('deductionsDes') ? errors.first('deductionsDes') : ''">
          <b-input v-model="editableDay.deductionsOthersDescription" type="text" name="deductionsDes"
            v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others" :type="errors.has('bonus') ? 'is-danger' : ''"
          :message="errors.has('bonus') ? errors.first('bonus') : ''">
          <b-numberinput v-model="editableDay.bonusOrOthers" step="0.01" name="bonus" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others Description" :type="errors.has('bonusDes') ? 'is-danger' : ''"
          :message="errors.has('bonusDes') ? errors.first('bonusDes') : ''">
          <b-input v-model="editableDay.bonusOrOthersDescription" type="text" name="bonusDes" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements" :type="errors.has('reimbursements') ? 'is-danger' : ''"
          :message="errors.has('reimbursements') ? errors.first('reimbursements') : ''">
          <b-numberinput v-model="editableDay.reimbursements" step="0.01" name="reimbursements" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements Description" :type="errors.has('reimbursementsDes') ? 'is-danger' : ''"
          :message="errors.has('reimbursementsDes') ? errors.first('reimbursementsDes') : ''">
          <b-input v-model="editableDay.reimbursementsDescription" type="text" name="reimbursementsDes" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Comments" :type="errors.has('comments') ? 'is-danger' : ''"
          :message="errors.has('comments') ? errors.first('comments') : ''">
          <b-input v-model="editableDay.comment" type="textarea" name="comments" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding mt-5">
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
      maximumMissing: maximumMissing,
      isLoading: false,
    }
  },
  created() {
  },
  methods: {
    validateHours(item) {
      this.$validator.validateAll().then((result) => {
        if (result) {
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
        deductionsOthers: item.deductionsOthers,
        deductionsOthersDescription: item.deductionsOthersDescription,
        bonusOrOthers: item.bonusOrOthers || 0,
        bonusOrOthersDescription: item.bonusOrOthersDescription || '',
        comments: item.comment,
        missingRateAgency: item.missingRateAgency,
        missingRateWorker: item.missingRateWorker,
        reimbursements: item.reimbursements || 0,
        reimbursementsDescription: item.reimbursementsDescription || ''
      };
      this.$store.dispatch('agency/updateWorkerTimeSheet', { requestId: this.$route.params.id, workerId: this.worker.workerId, id: item.id, model: model })
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
    },
    maximumDailyDecimal() {
      return this.$store.state.catalog.maximumHoursPerDay
    }
  }
}
</script>