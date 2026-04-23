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
            <b-datepicker v-model="newDate" indicators="bars" append-to-body>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Daily hours" :type="formErrors.newDateHour ? 'is-danger' : ''"
            :message="formErrors.newDateHour">
            <b-timepicker placeholder="Select a time..." v-model="newDateHour"
              :disabled="!newDate" hour-format="24">
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
              <b-timepicker v-model="missingHours" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Hours Over time">
              <b-timepicker v-model="missingHoursOvertime" hour-format="24" :max-time="maximumMissing">
              </b-timepicker>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Worker Rate" :type="formErrors.deductionsW ? 'is-danger' : ''"
              :message="formErrors.deductionsW">
              <b-numberinput v-model="deductionsW" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Missing Agency Rate" :type="formErrors.deductionsC ? 'is-danger' : ''"
              :message="formErrors.deductionsC">
              <b-numberinput v-model="deductionsC" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions" :type="formErrors.deductions ? 'is-danger' : ''"
              :message="formErrors.deductions">
              <b-numberinput v-model="deductions" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Deductions Description" :type="formErrors.deductionsDes ? 'is-danger' : ''"
              :message="formErrors.deductionsDes">
              <b-input v-model="deductionsDes" type="text"></b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others" :type="formErrors.bonus ? 'is-danger' : ''"
              :message="formErrors.bonus">
              <b-numberinput v-model="bonus" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Bonus or others Description" :type="formErrors.bonusDes ? 'is-danger' : ''"
              :message="formErrors.bonusDes">
              <b-input v-model="bonusDes" type="text"></b-input>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements" :type="formErrors.reimbursements ? 'is-danger' : ''"
              :message="formErrors.reimbursements">
              <b-numberinput v-model="reimbursements" step="0.01" controls-alignment="right">
              </b-numberinput>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Reimbursements Description"
              :type="formErrors.reimbursementsDescription ? 'is-danger' : ''"
              :message="formErrors.reimbursementsDescription">
              <b-input v-model="reimbursementsDescription" type="text"></b-input>
            </b-field>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-button type="is-primary" @click="reportWorkerTimSheet">
            {{ "Save" }}
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
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import dayjs from "dayjs";
import { getAgencyWorkerTimeSheet, postAgencyWorkerTimeSheet } from "@/api/agencyTimeSheetApi";
import { date, time } from '@/utils/filters';

const props = defineProps<{
  workerId: any;
  workerName?: string;
  requestId: any;
}>();

const emit = defineEmits<{ (e: 'created'): void }>();

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

const emptyTime = dayjs().hour(0).minute(0).second(0).millisecond(0).toDate();

const form = useStickyForm({
  schema,
  initialValues: {
    newDate: null as Date | null,
    newDateHour: emptyTime as Date | null,
    missingHours: emptyTime as Date | null,
    missingHoursOvertime: emptyTime as Date | null,
    deductionsW: null as number | null,
    deductionsC: null as number | null,
    deductions: null as number | null,
    deductionsDes: '',
    bonus: null as number | null,
    bonusDes: '',
    reimbursements: null as number | null,
    reimbursementsDescription: '',
  },
});

const {
  newDate, newDateHour, missingHours, missingHoursOvertime,
  deductionsW, deductionsC, deductions, deductionsDes,
  bonus, bonusDes, reimbursements, reimbursementsDescription,
} = form.fields;
const formErrors = form.errors;

const maximumMissing = dayjs().hour(12).minute(0).second(0).millisecond(0).toDate();

const showOptions = ref(false);
const punchCard = ref(true);
const isLoading = ref(false);
const size = ref(30);
const currentPage = ref(1);
const rows = ref<any[]>([]);

function showTimeSheet() {
  isLoading.value = true;
  getAgencyWorkerTimeSheet(props.requestId, props.workerId)
    .then((response: any) => {
      isLoading.value = false;
      rows.value = response;
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
  punchCard.value = false;
}

function reportWorkerTimSheet() {
  form.markInteracted([
    'newDate', 'newDateHour', 'deductionsW', 'deductionsC',
    'deductions', 'deductionsDes', 'bonus', 'bonusDes',
    'reimbursements', 'reimbursementsDescription',
  ]);
  form.handleSubmit((values) => {
    submitTimeSheet(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function submitTimeSheet(values: any) {
  isLoading.value = true;
  const dateStr = dayjs(values.newDate).format('YYYY-MM-DD');
  const hours = dayjs(values.newDateHour).format('HH:mm:ss');
  const payload = {
    hours,
    timeIn: dayjs(dateStr + ' ' + hours).format('YYYY-MM-DDTHH:mm:ss'),
    missingHours: dayjs(values.missingHours).format('HH:mm:ss'),
    missingHoursOvertime: dayjs(values.missingHoursOvertime).format('HH:mm:ss'),
    missingRateWorker: values.deductionsW,
    missingRateAgency: values.deductionsC,
    deductionsOthers: values.deductions,
    deductionsOthersDescription: values.deductionsDes,
    bonusOrOthers: values.bonus,
    bonusOrOthersDescription: values.bonusDes,
    reimbursements: values.reimbursements,
    reimbursementsDescription: values.reimbursementsDescription,
  };
  postAgencyWorkerTimeSheet(props.requestId, props.workerId, payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Created');
      form.resetAll();
      emit('created');
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
