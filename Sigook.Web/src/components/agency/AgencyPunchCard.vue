<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ workerName }}</h2>
    <div class="submenu-modal-tabs">
      <ul>
        <li @click="punchCard = true" :class="punchCard == true ? 'active' : ''">
          {{ "Punch Card" }}
        </li>
        <li @click="showTimeSheet()" :class="punchCard == false ? 'active' : ''">
          {{ "Time sheet" }}
        </li>
      </ul>
    </div>
    <transition name="fadeHeight">
      <div class="container-flex" v-if="punchCard">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.newDate ? 'is-danger' : ''" label="Date"
            :message="formErrors.newDate">
            <b-datepicker v-model="newDate.date" indicators="bars" append-to-body>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Daily hours" :type="formErrors.newDateHour ? 'is-danger' : ''"
            :message="formErrors.newDateHour">
            <b-timepicker placeholder="Select a time..." v-model="newDate.hours"
              :disabled="!newDate.date" hour-format="24">
            </b-timepicker>
          </b-field>
        </div>
        <div class="col-12 col-padding">
          <b-switch v-model="showOptions">
            {{ showOptions ? 'Hide Options' : 'Show Options' }}
          </b-switch>
        </div>
        <div class="container-flex" v-if="showOptions">
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Hours">
              <b-timepicker v-model="newDate.missingHours" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Hours Over time">
              <b-timepicker v-model="newDate.missingHoursOvertime" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Worker Rate" :type="formErrors.deductionsW ? 'is-danger' : ''"
              :message="formErrors.deductionsW">
              <b-numberinput v-model="newDate.missingRateWorker" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Agency Rate" :type="formErrors.deductionsC ? 'is-danger' : ''"
              :message="formErrors.deductionsC">
              <b-numberinput v-model="newDate.missingRateAgency" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions" :type="formErrors.deductions ? 'is-danger' : ''"
              :message="formErrors.deductions">
              <b-numberinput v-model="newDate.deductionsOthers" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions Description" :type="formErrors.deductionsDes ? 'is-danger' : ''"
              :message="formErrors.deductionsDes">
              <b-input v-model="newDate.deductionsOthersDescription" type="text"></b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others" :type="formErrors.bonus ? 'is-danger' : ''"
              :message="formErrors.bonus">
              <b-numberinput v-model="newDate.bonusOrOthers" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others Description" :type="formErrors.bonusDes ? 'is-danger' : ''"
              :message="formErrors.bonusDes">
              <b-input v-model="newDate.bonusOrOthersDescription" type="text"></b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements" :type="formErrors.reimbursements ? 'is-danger' : ''"
              :message="formErrors.reimbursements">
              <b-numberinput v-model="newDate.reimbursements" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements Description"
              :type="formErrors.reimbursementsDescription ? 'is-danger' : ''"
              :message="formErrors.reimbursementsDescription">
              <b-input v-model="newDate.reimbursementsDescription" type="text"></b-input>
            </b-field>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-button type="is-primary" @click="reportWorkerTimSheet">
            {{ newDate.id ? "Update" : "Save" }}
          </b-button>
        </div>
      </div>
    </transition>
    <transition name="fadeHeight">
      <b-table v-if="!punchCard" :data="rows" :per-page="size" :current-page="currentPage" narrowed hoverable
        :mobile-cards="false" paginated pagination-rounded>
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="day" label="Day" v-slot="props">
            {{ date(props.row.day) }}
          </b-table-column>
          <b-table-column field="timeIn" label="Start Time" v-slot="props">
            {{ time(props.row.timeIn) }}
          </b-table-column>
          <b-table-column field="totalHoursApproved" label="Hours Approved" v-slot="props">
            <span class="big-decimal">{{ props.row.totalHoursApproved }}</span>
          </b-table-column>
        </template>
      </b-table>
    </transition>
  </div>
</template>
<script lang="ts">
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import dayjs from "dayjs";
import { getAgencyWorkerTimeSheet, postAgencyWorkerTimeSheet } from "@/api/agencyTimeSheetApi";
import { date, time } from '@/utils/filters';

const optionalNumber = (max: number) =>
  yup
    .number()
    .transform((v, o) => (o === '' || o === null || o === undefined ? undefined : v))
    .min(0, 'Minimum is 0')
    .max(max, `Maximum is ${max}`)
    .nullable()
    .notRequired();

const optionalText = (max: number) =>
  yup.string().max(max, `Max ${max} characters`).nullable().notRequired();

const schema = yup.object({
  newDate: yup.mixed().required('Date is required'),
  newDateHour: yup.mixed().required('Daily hours is required'),
  deductionsW: optionalNumber(100),
  deductionsC: optionalNumber(100),
  deductions: optionalNumber(1000),
  deductionsDes: optionalText(1000),
  bonus: optionalNumber(1000),
  bonusDes: optionalText(1000),
  reimbursements: optionalNumber(1000),
  reimbursementsDescription: optionalText(1000),
});

export default {
  props: ["workerId", "workerName", "requestId"],
  data() {
    const emptyTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();
    const maximumMissing = dayjs().hour(12).minute(0).second(0).millisecond(0).toDate();
    return {
      showOptions: false,
      punchCard: true,
      isLoading: false,
      newDate: {
        hours: emptyTime,
        missingHours: emptyTime,
        missingHoursOvertime: emptyTime,
      } as any,
      size: 30,
      currentPage: 1,
      maximumMissing,
      rows: [] as any[],
      formErrors: {} as Record<string, string>,
    };
  },
  methods: {
    date,
    time,
    showTimeSheet() {
      this.isLoading = true;
      getAgencyWorkerTimeSheet(this.requestId, this.workerId)
        .then((response: any) => {
          this.isLoading = false;
          this.rows = response;
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
      this.punchCard = false;
    },
    reportWorkerTimSheet() {
      const values = {
        newDate: this.newDate.date,
        newDateHour: this.newDate.hours,
        deductionsW: this.newDate.missingRateWorker,
        deductionsC: this.newDate.missingRateAgency,
        deductions: this.newDate.deductionsOthers,
        deductionsDes: this.newDate.deductionsOthersDescription,
        bonus: this.newDate.bonusOrOthers,
        bonusDes: this.newDate.bonusOrOthersDescription,
        reimbursements: this.newDate.reimbursements,
        reimbursementsDescription: this.newDate.reimbursementsDescription,
      };
      schema
        .validate(values, { abortEarly: false })
        .then(() => {
          this.formErrors = {};
          this.submitTimeSheet();
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
    submitTimeSheet() {
      this.isLoading = true;
      const dateStr = dayjs(this.newDate.date).format('YYYY-MM-DD');
      const hours = dayjs(this.newDate.hours).format('HH:mm:ss');
      const payload = {
        ...this.newDate,
        hours,
        timeIn: dayjs(dateStr + ' ' + hours).format('YYYY-MM-DDTHH:mm:ss'),
        missingHours: dayjs(this.newDate.missingHours).format('HH:mm:ss'),
        missingHoursOvertime: dayjs(this.newDate.missingHoursOvertime).format('HH:mm:ss'),
      };
      postAgencyWorkerTimeSheet(this.requestId, this.workerId, payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Created');
          this.$emit('created');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
};
</script>
