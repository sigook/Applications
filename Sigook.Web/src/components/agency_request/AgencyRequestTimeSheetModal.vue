<template>
  <div class="p-3 time-sheet-input">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> {{ localEditableDay.day | dateMonth }} </h2>
    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock In</span>
              <span v-if="localEditableDay.clockIn">{{ localEditableDay.clockIn | dateHHmm }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ localEditableDay.clockInRounded | dateHHmm }}</span>
            </div>
          </div>
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock Out</span>
              <span v-if="localEditableDay.clockOut">{{ localEditableDay.clockOut | dateHHmm }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ localEditableDay.clockOutRounded | dateHHmm }}</span>
            </div>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Hours</span>
            <span v-if="localEditableDay.clockOut && localEditableDay.totalHours">
              {{ localEditableDay.totalHours | hour }}
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
        <b-field :label="$t('HoursApproved')" :type="errors.has('Hours Approved') ? 'is-danger' : ''"
          :message="errors.has('Hours Approved') ? errors.first('Hours Approved') : ''">
          <b-timepicker v-model="localEditableDay.hoursApprovedToDate" name="Hours Approved" hour-format="24"
            :max-time="maximumDailyHours" v-validate="'required'">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="$t('MissingHours')">
          <b-timepicker v-model="localEditableDay.missinghoursToDate" name="Missing Hours" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="localEditableDay.missingHoursOvertimeToDate" name="Missing Hours" hour-format="24"
            :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Worker Rate" :type="errors.has('deductionsW') ? 'is-danger' : ''"
          :message="errors.has('deductionsW') ? errors.first('deductionsW') : ''">
          <b-numberinput v-model="localEditableDay.missingRateWorker" step="0.01" name="deductionsW" controls-alignment="right"
            v-validate="'max_value:100|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Agency Rate" :type="errors.has('deductionsC') ? 'is-danger' : ''"
          :message="errors.has('deductionsC') ? errors.first('deductionsC') : ''">
          <b-numberinput v-model="localEditableDay.missingRateAgency" step="0.01" name="deductionsC" controls-alignment="right"
            v-validate="'max_value:100|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others" :type="errors.has('deductions') ? 'is-danger' : ''"
          :message="errors.has('deductions') ? errors.first('deductions') : ''">
          <b-numberinput v-model="localEditableDay.deductionsOthers" step="0.01" name="deductions" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others Description" :type="errors.has('deductionsDes') ? 'is-danger' : ''"
          :message="errors.has('deductionsDes') ? errors.first('deductionsDes') : ''">
          <b-input v-model="localEditableDay.deductionsOthersDescription" type="text" name="deductionsDes"
            v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others" :type="errors.has('bonus') ? 'is-danger' : ''"
          :message="errors.has('bonus') ? errors.first('bonus') : ''">
          <b-numberinput v-model="localEditableDay.bonusOrOthers" step="0.01" name="bonus" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others Description" :type="errors.has('bonusDes') ? 'is-danger' : ''"
          :message="errors.has('bonusDes') ? errors.first('bonusDes') : ''">
          <b-input v-model="localEditableDay.bonusOrOthersDescription" type="text" name="bonusDes" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements" :type="errors.has('reimbursements') ? 'is-danger' : ''"
          :message="errors.has('reimbursements') ? errors.first('reimbursements') : ''">
          <b-numberinput v-model="localEditableDay.reimbursements" step="0.01" name="reimbursements" controls-alignment="right"
            v-validate="'max_value:1000|min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements Description" :type="errors.has('reimbursementsDes') ? 'is-danger' : ''"
          :message="errors.has('reimbursementsDes') ? errors.first('reimbursementsDes') : ''">
          <b-input v-model="localEditableDay.reimbursementsDescription" type="text" name="reimbursementsDes" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Comments" :type="errors.has('comments') ? 'is-danger' : ''"
          :message="errors.has('comments') ? errors.first('comments') : ''">
          <b-input v-model="localEditableDay.comment" type="textarea" name="comments" v-validate="'max:1000'"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding mt-5">
        <b-button type="is-primary" @click="validateHours(localEditableDay)">{{ $t("Save") }}</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import dayjs from "dayjs";
import { useCatalog } from "@/composables/useCatalog";

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
      localEditableDay: JSON.parse(JSON.stringify(this.editableDay)),
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
  created() {
    // no-op
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
          this.$emit('update:editableDay', this.localEditableDay);
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
      const maxHours = useCatalog().maximumHoursPerDay;
      if (maxHours) {
        const maximum = new Date();
        maximum.setHours(maxHours);
        maximum.setMinutes(0);
        maximum.setSeconds(0);
        return maximum;
      }
      return this.maximumMissing;
    },
    maximumDailyDecimal() {
      return useCatalog().maximumHoursPerDay;
    }
  }
}
</script>