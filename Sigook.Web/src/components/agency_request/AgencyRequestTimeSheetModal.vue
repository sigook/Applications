<template>
  <div class="p-3 time-sheet-input">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> {{ dateMonth(localEditableDay.day) }} </h2>
    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock In</span>
              <span v-if="localEditableDay.clockIn">{{ dateHHmm(localEditableDay.clockIn) }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ dateHHmm(localEditableDay.clockInRounded) }}</span>
            </div>
          </div>
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 fw-700 d-block">Clock Out</span>
              <span v-if="localEditableDay.clockOut">{{ dateHHmm(localEditableDay.clockOut) }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 fw-700 d-block">Rounded</span>
              <span>{{ dateHHmm(localEditableDay.clockOutRounded) }}</span>
            </div>
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
        <b-field label="Hours Approved" :type="formErrors.hoursApproved ? 'is-danger' : ''"
          :message="formErrors.hoursApproved">
          <b-timepicker v-model="localEditableDay.hoursApprovedToDate" hour-format="24"
            :max-time="maximumDailyHours">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours">
          <b-timepicker v-model="localEditableDay.missinghoursToDate" hour-format="24" :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="localEditableDay.missingHoursOvertimeToDate" hour-format="24" :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Worker Rate" :type="formErrors.deductionsW ? 'is-danger' : ''"
          :message="formErrors.deductionsW">
          <b-numberinput v-model="localEditableDay.missingRateWorker" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Missing Agency Rate" :type="formErrors.deductionsC ? 'is-danger' : ''"
          :message="formErrors.deductionsC">
          <b-numberinput v-model="localEditableDay.missingRateAgency" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others" :type="formErrors.deductions ? 'is-danger' : ''"
          :message="formErrors.deductions">
          <b-numberinput v-model="localEditableDay.deductionsOthers" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Deductions Others Description" :type="formErrors.deductionsDes ? 'is-danger' : ''"
          :message="formErrors.deductionsDes">
          <b-input v-model="localEditableDay.deductionsOthersDescription" type="text"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others" :type="formErrors.bonus ? 'is-danger' : ''"
          :message="formErrors.bonus">
          <b-numberinput v-model="localEditableDay.bonusOrOthers" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Bonus or others Description" :type="formErrors.bonusDes ? 'is-danger' : ''"
          :message="formErrors.bonusDes">
          <b-input v-model="localEditableDay.bonusOrOthersDescription" type="text"></b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements" :type="formErrors.reimbursements ? 'is-danger' : ''"
          :message="formErrors.reimbursements">
          <b-numberinput v-model="localEditableDay.reimbursements" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Reimbursements Description" :type="formErrors.reimbursementsDes ? 'is-danger' : ''"
          :message="formErrors.reimbursementsDes">
          <b-input v-model="localEditableDay.reimbursementsDescription" type="text"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Comments" :type="formErrors.comments ? 'is-danger' : ''"
          :message="formErrors.comments">
          <b-input v-model="localEditableDay.comment" type="textarea"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding mt-5">
        <b-button type="is-primary" @click="validateHours(localEditableDay)">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { dateMonth, dateHHmm, hour } from '@/utils/filters';
import dayjs from "dayjs";
import { maximumHoursPerDay } from "@/constants/catalog";
import { updateAgencyWorkerTimeSheet } from "@/api/agencyTimeSheetApi";

const optionalNumber = (max: number) =>
  yup
    .number()
    .transform((v, o) => (o === '' || o === null || o === undefined ? undefined : v))
    .min(0, `Minimum is 0`)
    .max(max, `Maximum is ${max}`)
    .nullable()
    .notRequired();

const optionalText = (max: number) =>
  yup
    .string()
    .max(max, `Max ${max} characters`)
    .nullable()
    .notRequired();

const schema = yup.object({
  hoursApproved: yup.mixed().required('Hours Approved is required'),
  deductionsW: optionalNumber(100),
  deductionsC: optionalNumber(100),
  deductions: optionalNumber(1000),
  deductionsDes: optionalText(1000),
  bonus: optionalNumber(1000),
  bonusDes: optionalText(1000),
  reimbursements: optionalNumber(1000),
  reimbursementsDes: optionalText(1000),
  comments: optionalText(1000),
});

export default {
  props: ['editableDay', 'worker'],
  data() {
    const maximumMissing = new Date();
    maximumMissing.setHours(12);
    maximumMissing.setMinutes(0);
    maximumMissing.setSeconds(0);

    return {
      maximumMissing,
      isLoading: false,
      localEditableDay: this.cloneEditable(this.editableDay),
      formErrors: {} as Record<string, string>,
    };
  },
  watch: {
    editableDay: {
      handler(newVal) {
        this.localEditableDay = (this as any).cloneEditable(newVal);
      },
      deep: true,
    },
  },
  methods: {
    dateMonth,
    dateHHmm,
    hour,
    cloneEditable(src: any): any {
      if (!src) return src;
      const copy = JSON.parse(JSON.stringify(src));
      const dateFields = ['hoursApprovedToDate', 'missinghoursToDate', 'missingHoursOvertimeToDate', 'timeInApproved', 'timeOutApproved'];
      for (const key of dateFields) {
        if (copy[key]) copy[key] = new Date(copy[key]);
      }
      return copy;
    },
    validateHours(item: any) {
      const values = {
        hoursApproved: item.hoursApprovedToDate,
        deductionsW: item.missingRateWorker,
        deductionsC: item.missingRateAgency,
        deductions: item.deductionsOthers,
        deductionsDes: item.deductionsOthersDescription,
        bonus: item.bonusOrOthers,
        bonusDes: item.bonusOrOthersDescription,
        reimbursements: item.reimbursements,
        reimbursementsDes: item.reimbursementsDescription,
        comments: item.comment,
      };
      schema
        .validate(values, { abortEarly: false })
        .then(() => {
          this.formErrors = {};
          this.sendValidation(item);
        })
        .catch((err: any) => {
          const next: Record<string, string> = {};
          if (err.inner) {
            err.inner.forEach((e: any) => {
              if (e.path && !next[e.path]) next[e.path] = e.message;
            });
          }
          this.formErrors = next;
          showAlertError('Please make sure all required fields are filled out correctly');
        });
    },
    sendValidation(item: any) {
      this.isLoading = true;
      const timeInZero = dayjs(item.timeIn).hour(0).minute(0).second(0);
      const model = {
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
        reimbursementsDescription: item.reimbursementsDescription || '',
      };
      updateAgencyWorkerTimeSheet(this.$route.params.id, this.worker.workerId, item.id, model)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:editableDay', this.localEditableDay);
          showAlertSuccess('Updated');
          this.$emit("updateData");
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
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
      return (this as any).maximumMissing;
    },
    maximumDailyDecimal() {
      return maximumHoursPerDay;
    },
  },
};
</script>
