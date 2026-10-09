<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-6">
        <b-field :type="formErrors.company ? 'is-danger' : ''"
          :message="formErrors.company || ''">
          <template #label>
            Company <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="company" :name="'company'" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.supervisor ? 'is-danger' : ''"
          :message="formErrors.supervisor || ''">
          <template #label>
            Supervisor
          </template>
          <b-input type="text" v-model="supervisor" :name="'supervisor'" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field :type="formErrors.duties ? 'is-danger' : ''"
          :message="formErrors.duties || ''">
          <template #label>
            Duties <span class="has-text-danger">*</span>
          </template>
          <b-input type="textarea" v-model="duties" :name="'duties'" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field :label="'Current Job'">
          <b-switch v-model="workExperience.isCurrentJobPosition" :name="'isCurrentJobPosition'" :true-value="true"
            :false-value="false">
            {{ workExperience.isCurrentJobPosition ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.startDate ? 'is-danger' : ''"
          :message="formErrors.startDate || ''">
          <template #label>
            Start date <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="startDate" :name="'startDate'"
            :max-date="disableStartDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="column is-6" v-if="!workExperience.isCurrentJobPosition">
        <b-field :type="formErrors.endDate ? 'is-danger' : ''"
          :message="formErrors.endDate || ''">
          <template #label>
            End date <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="endDate" :name="'endDate'"
            :max-date="disableStartDate" :min-date="startDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="validateAll">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import * as yup from 'yup';
import { useAppStore } from '@/app/stores/app';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError } from "@/shared/utils/toast";
import { createWorkerWorkExperience, editWorkerWorkExperience } from '@/shared/worker-profile/api';
import type { WorkerProfileJobExperienceDetail, WorkerJobExperienceModel } from '@/shared/worker-profile/types';

interface ExperienceForm {
  company: string;
  supervisor: string;
  duties: string;
  startDate: Date | null;
  endDate: Date | null;
}

const props = defineProps<{ workerId?: string; data?: WorkerProfileJobExperienceDetail }>();
const emit = defineEmits<{ (e: 'updateExperience'): void }>();

const schema = yup.object({
  company: yup.string().required('Company is required')
    .min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  supervisor: yup.string().max(50, 'Max 50 characters')
    .test('min-length', 'Min 2 characters', v => !v || v.trim().length >= 2),
  duties: yup.string().required('Duties is required')
    .min(2, 'Min 2 characters').max(5000, 'Max 5000 characters'),
  startDate: yup.mixed().required('Start date is required'),
  endDate: yup.mixed().nullable()
    .test('required-if-not-current', 'End date is required', v => workExperience.isCurrentJobPosition || !!v),
});

const form = useStickyForm<ExperienceForm>({
  schema,
  initialValues: {
    company: '',
    supervisor: '',
    duties: '',
    startDate: null,
    endDate: null,
  },
});
const { company, supervisor, duties, startDate, endDate } = form.fields;
const formErrors = form.errors;

const appStore = useAppStore();

const isLoading = ref(false);
const disableStartDate = ref<Date | null>(null);
const workExperience = reactive<{ isCurrentJobPosition: boolean }>({
  isCurrentJobPosition: true,
});

function saveCreateExperience(payload: WorkerJobExperienceModel) {
  isLoading.value = true;
  createWorkerWorkExperience(props.workerId, payload)
    .then(() => {
      isLoading.value = false;
      emit('updateExperience');
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function saveEditExperience(payload: WorkerJobExperienceModel) {
  isLoading.value = true;
  editWorkerWorkExperience(props.workerId, props.data.id, payload)
    .then(() => {
      isLoading.value = false;
      emit('updateExperience');
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values) => {
    const payload: WorkerJobExperienceModel = {
      isCurrentJobPosition: workExperience.isCurrentJobPosition,
      company: values.company,
      supervisor: values.supervisor,
      duties: values.duties,
      startDate: values.startDate?.toISOString(),
      endDate: workExperience.isCurrentJobPosition ? null : values.endDate?.toISOString() ?? null,
    };
    if (props.data) {
      saveEditExperience(payload);
    } else {
      saveCreateExperience(payload);
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function updateData() {
  const src = props.data;
  workExperience.isCurrentJobPosition = src.isCurrentJobPosition;
  const sd = new Date(src.startDate);
  const ed = src.endDate ? new Date(src.endDate) : null;
  form.hydrate({
    company: src.company || '',
    supervisor: src.supervisor || '',
    duties: src.duties || '',
    startDate: sd,
    endDate: ed,
  });
}

appStore.getCurrentDate().then((response) => {
  disableStartDate.value = response;
});
if (props.data) {
  updateData();
}
</script>
