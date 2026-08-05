<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-4">
        <b-field :label="'Do you have any health problems / allergies?'" class="has-text-weight-normal">
          <b-switch v-model="worker.haveAnyHealthProblem">
            {{ worker.haveAnyHealthProblem ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="column is-4" v-if="worker.haveAnyHealthProblem">
        <b-field :type="formErrors.healthProblem ? 'is-danger' : ''"
          :message="formErrors.healthProblem || ''">
          <template #label>
            Which ? <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="healthProblem" name="health problem">
          </b-input>
        </b-field>
      </div>
      <div class="column is-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="'Other allergies'" class="has-text-weight-normal">
          <b-input type="text" v-model="worker.otherHealthProblem">
          </b-input>
        </b-field>
      </div>
      <div class="column is-12">
        <h1 class="has-text-weight-bold">{{ 'In case of emergency notify' }}</h1>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.contactEmergencyName ? 'is-danger' : ''"
          :message="formErrors.contactEmergencyName || ''">
          <template #label>
            Name <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="contactEmergencyName" name="contact emergency">
          </b-input>
        </b-field>
      </div>
      <div class="column is-6">
        <b-field class="has-text-weight-normal"
          :type="formErrors.contactEmergencyLastName ? 'is-danger' : ''"
          :message="formErrors.contactEmergencyLastName || ''">
          <template #label>
            Last Name <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="contactEmergencyLastName" name="contact emergency lastname" expanded>
          </b-input>
        </b-field>
      </div>
      <div class="column is-12">
        <PhoneInput ref="emergencyPhoneComponent" :required="true" model="Contact Emergency Phone"
          :defaultValue="worker.contactEmergencyPhone"
          @formattedPhone="(phone) => worker.contactEmergencyPhone = phone" />
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import PhoneInput from '../PhoneInput.vue';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { createWorkerEmergencyInformation } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  healthProblem: yup.string().nullable().transform(v => v === '' ? null : v)
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  contactEmergencyName: yup.string()
    .required('Name is required')
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  contactEmergencyLastName: yup.string()
    .required('Last Name is required')
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
});

interface EmergencyForm {
  healthProblem: string;
  contactEmergencyName: string;
  contactEmergencyLastName: string;
}

const form = useStickyForm<EmergencyForm>({
  schema,
  initialValues: {
    healthProblem: '',
    contactEmergencyName: '',
    contactEmergencyLastName: '',
  },
});
const { healthProblem, contactEmergencyName, contactEmergencyLastName } = form.fields;
const formErrors = form.errors;

const worker = ref<any>({});
const isLoading = ref(false);
const emergencyPhoneComponent = ref<any>(null);

function saveEmergencyInformation(values: any) {
  isLoading.value = true;
  const payload = {
    ...worker.value,
    healthProblem: worker.value.haveAnyHealthProblem ? values.healthProblem : null,
    contactEmergencyName: values.contactEmergencyName,
    contactEmergencyLastName: values.contactEmergencyLastName,
  };
  createWorkerEmergencyInformation(props.data.id, payload)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

async function validateAll() {
  form.markInteracted();
  const phoneComponent = emergencyPhoneComponent.value;
  let phoneValid = true;
  if (phoneComponent && typeof phoneComponent.validatePhone === 'function') {
    phoneValid = await phoneComponent.validatePhone();
  }
  if (worker.value.haveAnyHealthProblem && !healthProblem.value) {
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  form.handleSubmit((values: any) => {
    if (!phoneValid) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    saveEmergencyInformation(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

if (props.data != null) {
  worker.value = Object.assign({}, props.data);
  form.hydrate({
    healthProblem: worker.value.healthProblem || '',
    contactEmergencyName: worker.value.contactEmergencyName || '',
    contactEmergencyLastName: worker.value.contactEmergencyLastName || '',
  });
}
</script>
