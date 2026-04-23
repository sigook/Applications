<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.company ? 'is-danger' : ''" :label="'Company'"
          :message="formErrors.company || ''">
          <b-input type="text" v-model="company" :name="'company'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.supervisor ? 'is-danger' : ''" :label="'Supervisor'"
          :message="formErrors.supervisor || ''">
          <b-input type="text" v-model="supervisor" :name="'supervisor'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :type="formErrors.duties ? 'is-danger' : ''" :label="'Duties'"
          :message="formErrors.duties || ''">
          <b-input type="textarea" v-model="duties" :name="'duties'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Current Job'">
          <b-switch v-model="workExperience.isCurrentJobPosition" :name="'isCurrentJobPosition'" :true-value="true"
            :false-value="false">
            {{ workExperience.isCurrentJobPosition ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.startDate ? 'is-danger' : ''" :label="'Start date'"
          :message="formErrors.startDate || ''">
          <b-datepicker v-model="startDate" :name="'startDate'"
            :max-date="disableStartDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!workExperience.isCurrentJobPosition">
        <b-field :type="formErrors.endDate ? 'is-danger' : ''" :label="'End date'"
          :message="formErrors.endDate || ''">
          <b-datepicker v-model="endDate" :name="'endDate'"
            :max-date="disableStartDate" :min-date="startDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
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
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { createWorkerWorkExperience, editWorkerWorkExperience } from '@/api/workerApi';

interface ExperienceForm {
  company: string;
  supervisor: string;
  duties: string;
  startDate: Date | null;
  endDate: Date | null;
}

const props = defineProps<{ workerId?: any; data?: any }>();
const emit = defineEmits<{ (e: 'updateExperience'): void }>();

const schema = yup.object({
  company: yup.string().required('Company is required')
    .min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  supervisor: yup.string().required('Supervisor is required')
    .min(2, 'Min 2 characters').max(60, 'Max 60 characters'),
  duties: yup.string().required('Duties is required').max(5000, 'Max 5000 characters'),
  startDate: yup.mixed().required('Start date is required'),
  endDate: yup.mixed().nullable(),
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
const workExperience = reactive<any>({
  isCurrentJobPosition: true,
});

function saveCreateExperience(payload: any) {
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

function saveEditExperience(payload: any) {
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
  form.handleSubmit((values: any) => {
    if (!workExperience.isCurrentJobPosition && !values.endDate) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    const payload = {
      ...workExperience,
      company: values.company,
      supervisor: values.supervisor,
      duties: values.duties,
      startDate: values.startDate,
      endDate: workExperience.isCurrentJobPosition ? null : values.endDate,
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
  Object.assign(workExperience, src);
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

appStore.getCurrentDate().then((response: any) => {
  disableStartDate.value = response;
});
if (props.data) {
  updateData();
}
</script>
