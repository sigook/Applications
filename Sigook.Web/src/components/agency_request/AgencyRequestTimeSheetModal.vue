<template>
  <div class="p-3 time-sheet-input">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered main-title"> {{ dateMonth(localEditableDay.day) }} </h2>
    <div class="has-text-centered">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="is-flex is-justify-content-space-between">
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 has-text-weight-bold is-block">Clock In</span>
              <span v-if="localEditableDay.clockIn">{{ dateHHmm(localEditableDay.clockIn) }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 has-text-weight-bold is-block">Rounded</span>
              <span>{{ dateHHmm(localEditableDay.clockInRounded) }}</span>
            </div>
          </div>
          <div class="pl-2 pr-2">
            <div>
              <span class="fz-2 has-text-weight-bold is-block">Clock Out</span>
              <span v-if="localEditableDay.clockOut">{{ dateHHmm(localEditableDay.clockOut) }}</span>
              <span v-else class="fz-1">No reported</span>
            </div>
            <div class="mt-2" v-if="localEditableDay.clockInRounded">
              <span class="fz-2 has-text-weight-bold is-block">Rounded</span>
              <span>{{ dateHHmm(localEditableDay.clockOutRounded) }}</span>
            </div>
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
    </div>
    <b-message type="is-info" v-if="localEditableDay.clockIn && !localEditableDay.clockOut" has-icon>
        The worker didn't clock out. Please enter the total hours worked in the "Hours Approved" field.
    </b-message>
    <div class="columns is-multiline">
      <div class="column is-4">
        <b-field label="Hours Approved" :type="formErrors.hoursApproved ? 'is-danger' : ''"
          :message="formErrors.hoursApproved">
          <b-timepicker v-model="hoursApproved" hour-format="24"
            :max-time="maximumDailyHours">
          </b-timepicker>
        </b-field>
      </div>
      <div class="column is-4">
        <b-field label="Missing Hours">
          <b-timepicker v-model="missinghours" hour-format="24" :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="column is-4">
        <b-field label="Missing Hours Overtime">
          <b-timepicker v-model="missingHoursOvertime" hour-format="24" :max-time="maximumMissing">
          </b-timepicker>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Missing Worker Rate" :type="formErrors.deductionsW ? 'is-danger' : ''"
          :message="formErrors.deductionsW">
          <b-numberinput v-model="deductionsW" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Missing Agency Rate" :type="formErrors.deductionsC ? 'is-danger' : ''"
          :message="formErrors.deductionsC">
          <b-numberinput v-model="deductionsC" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Deductions Others" :type="formErrors.deductions ? 'is-danger' : ''"
          :message="formErrors.deductions">
          <b-numberinput v-model="deductions" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Deductions Others Description" :type="formErrors.deductionsDes ? 'is-danger' : ''"
          :message="formErrors.deductionsDes">
          <b-input v-model="deductionsDes" type="text"></b-input>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Bonus or others" :type="formErrors.bonus ? 'is-danger' : ''"
          :message="formErrors.bonus">
          <b-numberinput v-model="bonus" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Bonus or others Description" :type="formErrors.bonusDes ? 'is-danger' : ''"
          :message="formErrors.bonusDes">
          <b-input v-model="bonusDes" type="text"></b-input>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Reimbursements" :type="formErrors.reimbursements ? 'is-danger' : ''"
          :message="formErrors.reimbursements">
          <b-numberinput v-model="reimbursements" step="0.01" controls-alignment="right">
          </b-numberinput>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Reimbursements Description" :type="formErrors.reimbursementsDes ? 'is-danger' : ''"
          :message="formErrors.reimbursementsDes">
          <b-input v-model="reimbursementsDes" type="text"></b-input>
        </b-field>
      </div>
      <div class="column is-12">
        <b-field label="Comments" :type="formErrors.comments ? 'is-danger' : ''"
          :message="formErrors.comments">
          <b-input v-model="comments" type="textarea"></b-input>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveHours">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute } from 'vue-router';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
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

function cloneEditable(src: any): any {
  if (!src) return src;
  const copy = JSON.parse(JSON.stringify(src));
  const dateFields = ['hoursApprovedToDate', 'missinghoursToDate', 'missingHoursOvertimeToDate', 'timeInApproved', 'timeOutApproved'];
  for (const key of dateFields) {
    if (copy[key]) copy[key] = new Date(copy[key]);
  }
  return copy;
}

function valuesFromEditable(src: any) {
  const clone = cloneEditable(src) || {};
  return {
    hoursApproved: clone.hoursApprovedToDate || null,
    missinghours: clone.missinghoursToDate || null,
    missingHoursOvertime: clone.missingHoursOvertimeToDate || null,
    deductionsW: clone.missingRateWorker ?? null,
    deductionsC: clone.missingRateAgency ?? null,
    deductions: clone.deductionsOthers ?? null,
    deductionsDes: clone.deductionsOthersDescription ?? '',
    bonus: clone.bonusOrOthers ?? null,
    bonusDes: clone.bonusOrOthersDescription ?? '',
    reimbursements: clone.reimbursements ?? null,
    reimbursementsDes: clone.reimbursementsDescription ?? '',
    comments: clone.comment ?? '',
  };
}

const props = defineProps<{ editableDay: any; worker: any }>();
const emit = defineEmits<{
  (e: 'update:editableDay', value: any): void;
  (e: 'updateData'): void;
}>();

const route = useRoute();

const form = useStickyForm<any>({
  schema,
  initialValues: valuesFromEditable(props.editableDay),
});
const {
  hoursApproved, missinghours, missingHoursOvertime,
  deductionsW, deductionsC, deductions, deductionsDes,
  bonus, bonusDes, reimbursements, reimbursementsDes, comments
} = form.fields;
const formErrors = form.errors;

function hydrate(src: any) {
  form.hydrate(valuesFromEditable(src));
}

const maximumMissingDate = new Date();
maximumMissingDate.setHours(12);
maximumMissingDate.setMinutes(0);
maximumMissingDate.setSeconds(0);
const maximumMissing = ref(maximumMissingDate);

const isLoading = ref(false);
const localEditableDay = ref<any>(cloneEditable(props.editableDay));

watch(() => props.editableDay, (newVal) => {
  localEditableDay.value = cloneEditable(newVal);
  hydrate(newVal);
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
  return maximumMissing.value;
});

function saveHours() {
  form.markInteracted([
    'hoursApproved', 'deductionsW', 'deductionsC', 'deductions', 'deductionsDes',
    'bonus', 'bonusDes', 'reimbursements', 'reimbursementsDes', 'comments'
  ]);
  form.handleSubmit((values: any) => {
    sendValidation(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function sendValidation(values: any) {
  isLoading.value = true;
  const timeInZero = dayjs(localEditableDay.value.timeIn).hour(0).minute(0).second(0);
  const model = {
    hours: dayjs(values.hoursApproved).format('HH:mm:ss'),
    timeIn: timeInZero.format('YYYY-MM-DDTHH:mm:ss'),
    missingHours: dayjs(values.missinghours).format("HH:mm:ss"),
    missingHoursOvertime: dayjs(values.missingHoursOvertime).format("HH:mm:ss"),
    deductionsOthers: values.deductions,
    deductionsOthersDescription: values.deductionsDes,
    bonusOrOthers: values.bonus || 0,
    bonusOrOthersDescription: values.bonusDes || '',
    comments: values.comments,
    missingRateAgency: values.deductionsC,
    missingRateWorker: values.deductionsW,
    reimbursements: values.reimbursements || 0,
    reimbursementsDescription: values.reimbursementsDes || '',
  };
  updateAgencyWorkerTimeSheet(route.params.id as string, props.worker.workerProfileId, localEditableDay.value.id, model)
    .then(() => {
      isLoading.value = false;
      const updated = {
        ...localEditableDay.value,
        hoursApprovedToDate: values.hoursApproved,
        missinghoursToDate: values.missinghours,
        missingHoursOvertimeToDate: values.missingHoursOvertime,
        missingRateWorker: values.deductionsW,
        missingRateAgency: values.deductionsC,
        deductionsOthers: values.deductions,
        deductionsOthersDescription: values.deductionsDes,
        bonusOrOthers: values.bonus,
        bonusOrOthersDescription: values.bonusDes,
        reimbursements: values.reimbursements,
        reimbursementsDescription: values.reimbursementsDes,
        comment: values.comments,
      };
      emit('update:editableDay', updated);
      showAlertSuccess('Updated');
      emit('updateData');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
