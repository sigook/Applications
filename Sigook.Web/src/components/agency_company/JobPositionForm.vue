<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div :class="isAdmin ? 'col-sm-12 col-md-6 col-lg-6 col-padding' : 'col-sm-12 col-md-12 col-lg-12 col-padding'">
        <b-field label="Position" :type="formErrors.jobPosition ? 'is-danger' : ''"
          :message="formErrors.jobPosition || ''">
          <b-input placeholder="Position" v-model="jobPosition" name="positions" />
        </b-field>
      </div>
      <div v-if="isAdmin" class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Overtime Starts After (hours)" :type="formErrors.overtimeStartsAfter ? 'is-danger' : ''"
          :message="formErrors.overtimeStartsAfter || 'Leave empty to inherit from company/province default'">
          <b-numberinput v-model="overtimeStartsAfter" name="overtimeStartsAfter" controls-alignment="right"
            step="0.5" :min="0" placeholder="44">
          </b-numberinput>
        </b-field>
      </div>
      <div v-if="isAdmin" class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Agency Rate'" :type="formErrors.rate ? 'is-danger' : ''"
          :message="formErrors.rate || ''">
          <b-numberinput v-model="rate" name="rate" controls-alignment="right" step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isAdmin ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate'" :type="formErrors.workerRate ? 'is-danger' : ''"
          :message="formErrors.workerRate || ''">
          <b-numberinput v-model="workerRate" :disabled="!isAdmin" name="workerRate"
            controls-alignment="right" step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isAdmin ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate Max'" :type="formErrors.workerRateMax ? 'is-danger' : ''"
          :message="formErrors.workerRateMax || ''">
          <b-numberinput v-model="workerRateMax" name="workerRateMax" controls-alignment="right"
            step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Description'" :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <b-input type="textarea" v-model="description" name="description">
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-if="props.currentPosition">
        <shift v-if="model.shift" :is-update="true" :current-shift="model.shift"
          @updateModel="(shift) => model.shift = shift" />
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-else>
        <shift :is-update="false" @updateModel="(shift) => model.shift = shift" />
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ props.currentPosition ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { useAdmin } from '@/composables/useAdmin';
import { createAgencyCompanyJobPosition, updateAgencyCompanyJobPosition, getAgencyCompanyJobPositionById } from "@/api/agencyCompanyApi";
import Shift from "../request/ShiftsForm.vue";

const props = defineProps<{ currentPosition?: any; profileId: any }>();
const emit = defineEmits<{ (e: 'updateContent'): void }>();

const { isAdmin } = useAdmin();

const rateRef = ref<number | null>(null);
const workerRateRef = ref<number | null>(null);

const schema = computed(() => {
  return yup.object({
    jobPosition: yup.string().required('Position is required'),
    rate: yup.number().typeError('Rate is required').required('Rate is required').min(0.1, 'Minimum is 0.1').max(999999),
    workerRate: yup
      .number()
      .typeError('Worker rate is required')
      .required('Worker rate is required')
      .min(0.1, 'Minimum is 0.1')
      .max(rateRef.value && rateRef.value > 0 ? rateRef.value : 50, 'Cannot exceed agency rate'),
    workerRateMax: yup
      .number()
      .nullable()
      .transform((v, o) => (o === '' || o === null || o === undefined ? null : v))
      .min(workerRateRef.value && workerRateRef.value > 0 ? workerRateRef.value : 0, 'Must be >= worker rate')
      .max(rateRef.value && rateRef.value > 0 ? rateRef.value : 50, 'Cannot exceed agency rate'),
    overtimeStartsAfter: yup
      .number()
      .nullable()
      .transform((v, o) => (o === '' || o === null || o === undefined ? null : v))
      .min(0, 'Must be >= 0')
      .max(168, 'Must be <= 168'),
    description: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(1000, 'Max 1000 characters'),
  });
});

const form = useStickyForm<{
  jobPosition: string;
  rate: number | null;
  workerRate: number | null;
  workerRateMax: number | null;
  overtimeStartsAfter: number | null;
  description: string;
}>({
  schema,
  initialValues: {
    jobPosition: '',
    rate: null,
    workerRate: null,
    workerRateMax: null,
    overtimeStartsAfter: null,
    description: '',
  },
});
const { jobPosition, rate, workerRate, workerRateMax, overtimeStartsAfter, description } = form.fields;
const formErrors = form.errors;

watch(rate, (v) => { rateRef.value = (v as number | null) ?? null; });
watch(workerRate, (v) => { workerRateRef.value = (v as number | null) ?? null; });

const isLoading = ref(false);
const model = ref<any>({
  id: null,
  jobPosition: null,
  rate: null,
  workerRate: null,
  description: null,
  workerRateMin: null,
  workerRateMax: null,
  shift: null,
});

function validateForm() {
  form.markInteracted();
  form.handleSubmit((values) => {
    const payload = {
      ...model.value,
      rate: values.rate,
      workerRate: values.workerRate,
      workerRateMin: values.workerRate,
      workerRateMax: values.workerRateMax,
      overtimeStartsAfter: values.overtimeStartsAfter,
      description: values.description,
      jobPosition: values.jobPosition,
    };
    if (props.currentPosition) {
      updateJobPosition(payload, props.currentPosition.id);
    } else {
      createJobPosition(payload);
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function createJobPosition(payload: any) {
  isLoading.value = true;
  createAgencyCompanyJobPosition(props.profileId, payload)
    .then(() => {
      isLoading.value = false;
      emit('updateContent');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateJobPosition(payload: any, id: any) {
  isLoading.value = true;
  updateAgencyCompanyJobPosition(props.profileId, id, payload)
    .then(() => {
      isLoading.value = false;
      emit('updateContent');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function loadJobPositionById(id: any) {
  isLoading.value = true;
  getAgencyCompanyJobPositionById(props.profileId, id)
    .then((response: any) => {
      model.value = response;
      form.hydrate({
        jobPosition: response.jobPosition || '',
        rate: response.rate ?? null,
        workerRate: response.workerRate ?? null,
        workerRateMax: response.workerRateMax ?? null,
        overtimeStartsAfter: response.overtimeStartsAfter ?? null,
        description: response.description || '',
      });
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

(async () => {
  if (props.currentPosition && props.currentPosition.id) {
    isLoading.value = true;
    loadJobPositionById(props.currentPosition.id);
    isLoading.value = false;
  }
})();
</script>
