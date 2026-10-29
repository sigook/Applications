<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div :class="isFull ? 'column is-6' : 'column is-12'">
        <b-field label="Position" :type="formErrors.jobPosition ? 'is-danger' : ''"
          :message="formErrors.jobPosition || ''">
          <b-input placeholder="Position" v-model="jobPosition" name="positions" />
        </b-field>
      </div>
      <template v-if="isFull">
        <div class="column is-6">
          <b-field label="Overtime Starts After (hours)" :type="formErrors.overtimeStartsAfter ? 'is-danger' : ''"
            :message="formErrors.overtimeStartsAfter || 'Leave empty to inherit from company/province default'">
            <b-numberinput v-model="overtimeStartsAfter" name="overtimeStartsAfter" :controls="false"
              step="0.5" :min="0" placeholder="44">
            </b-numberinput>
          </b-field>
        </div>
        <div class="column is-4">
          <b-field :label="'Agency Rate'" :type="formErrors.rate ? 'is-danger' : ''"
            :message="formErrors.rate || ''">
            <b-numberinput v-model="rate" name="rate" :controls="false" step="0.01" placeholder="10.00">
            </b-numberinput>
          </b-field>
        </div>
        <div class="column is-4">
          <b-field :label="'Worker Rate'" :type="formErrors.workerRate ? 'is-danger' : ''"
            :message="formErrors.workerRate || ''">
            <b-numberinput v-model="workerRate" name="workerRate" :controls="false" step="0.01" placeholder="10.00">
            </b-numberinput>
          </b-field>
        </div>
        <div class="column is-4">
          <b-field :label="'Worker Rate Max'" :type="formErrors.workerRateMax ? 'is-danger' : ''"
            :message="formErrors.workerRateMax || ''">
            <b-numberinput v-model="workerRateMax" name="workerRateMax" :controls="false"
              step="0.01" placeholder="10.00">
            </b-numberinput>
          </b-field>
        </div>
      </template>
      <div class="column is-12">
        <b-field :label="'Description'" :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <b-input type="textarea" v-model="description" name="description">
          </b-input>
        </b-field>
      </div>
      <div class="column is-12" v-if="props.currentPosition">
        <shift v-if="model.shift" :is-update="true" :current-shift="model.shift"
          @updateModel="(shift) => model.shift = shift" />
      </div>
      <div class="column is-12" v-else>
        <shift :is-update="false" @updateModel="(shift) => model.shift = shift" />
      </div>

      <div class="column is-12">
        <b-button type="is-primary" @click="validateForm">
          {{ isUpdate ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError } from "@/shared/utils/toast";
import Shift from "@/shared/request-detail/ShiftsForm.vue";
import type { JobPositionModel } from '@/shared/company-profile/types';

const props = defineProps<{
  kind: 'full' | 'basic';
  create: (model: JobPositionModel) => Promise<{ id: string }>;
  update?: (id: string, model: JobPositionModel) => Promise<void>;
  load?: (id: string) => Promise<JobPositionModel>;
  currentPosition?: JobPositionModel | null;
  isDuplicate?: boolean;
}>();
const emit = defineEmits<{ (e: 'updateContent', id?: string): void }>();

const isFull = computed(() => props.kind === 'full');
const isUpdate = computed(() => !!props.currentPosition && !props.isDuplicate);

const rateRef = ref<number | null>(null);
const workerRateRef = ref<number | null>(null);

const optionalNumber = () => yup
  .number()
  .nullable()
  .transform((v, o) => (o === '' || o === null || o === undefined ? null : v));

const schema = computed(() => {
  const maxRate = rateRef.value && rateRef.value > 0 ? rateRef.value : 50;
  return yup.object({
    jobPosition: yup.string().required('Position is required'),
    rate: isFull.value
      ? yup.number().typeError('Rate is required').required('Rate is required').min(0.1, 'Minimum is 0.1').max(999999)
      : optionalNumber(),
    workerRate: isFull.value
      ? yup
        .number()
        .typeError('Worker rate is required')
        .required('Worker rate is required')
        .min(0.1, 'Minimum is 0.1')
        .max(maxRate, 'Cannot exceed agency rate')
      : optionalNumber(),
    workerRateMax: optionalNumber()
      .min(workerRateRef.value && workerRateRef.value > 0 ? workerRateRef.value : 0, 'Must be >= worker rate')
      .max(maxRate, 'Cannot exceed agency rate'),
    overtimeStartsAfter: optionalNumber()
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
const model = ref<JobPositionModel>({
  jobPosition: undefined,
  description: undefined,
  workerRateMin: null,
  workerRateMax: null,
  shift: null,
});

function validateForm() {
  form.markInteracted();
  form.handleSubmit((values) => {
    const payload: JobPositionModel = {
      ...model.value,
      jobPosition: values.jobPosition,
      description: values.description,
    };
    if (isFull.value) {
      payload.rate = values.rate ?? undefined;
      payload.workerRate = values.workerRate ?? undefined;
      payload.workerRateMin = values.workerRate;
      payload.workerRateMax = values.workerRateMax;
      payload.overtimeStartsAfter = values.overtimeStartsAfter;
    }
    if (isUpdate.value && props.currentPosition?.id && props.update) {
      save(props.update(props.currentPosition.id, payload).then(() => props.currentPosition?.id));
    } else {
      save(props.create(payload).then((response) => response?.id));
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function save(request: Promise<string | undefined>) {
  isLoading.value = true;
  request
    .then((id) => {
      isLoading.value = false;
      emit('updateContent', id);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function loadJobPosition(id: string) {
  if (!props.load) return;
  isLoading.value = true;
  props.load(id)
    .then((response) => {
      model.value = props.isDuplicate ? { ...response, id: undefined } : response;
      form.hydrate({
        jobPosition: response.jobPosition || '',
        rate: response.rate || null,
        workerRate: response.workerRate || null,
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

if (props.currentPosition?.id) {
  loadJobPosition(props.currentPosition.id);
}
</script>
