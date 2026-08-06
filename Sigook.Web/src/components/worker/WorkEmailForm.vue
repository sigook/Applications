<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :type="formErrors.newEmail ? 'is-danger' : ''"
          :message="formErrors.newEmail || ''">
          <template #label>
            New Email <span class="has-text-danger">*</span>
          </template>
          <b-input type="email" v-model="newEmail" name="newEmail">
          </b-input>
        </b-field>
      </div>
      <div class="column is-12">
        <b-field :type="formErrors.confirmEmail ? 'is-danger' : ''"
          :message="formErrors.confirmEmail || ''">
          <template #label>
            Confirm Email <span class="has-text-danger">*</span>
          </template>
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail">
          </b-input>
        </b-field>
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
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { updateAgencyWorkerEmail } from "@/api/agencyWorkerApi";

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  newEmail: yup.string()
    .required('Email is required')
    .email('Invalid email'),
  confirmEmail: yup.string()
    .required('Confirm Email is required')
    .email('Invalid email')
    .oneOf([yup.ref('newEmail')], 'Emails must match'),
});

const form = useStickyForm<{ newEmail: string; confirmEmail: string }>({
  schema,
  initialValues: {
    newEmail: '',
    confirmEmail: '',
  },
});
const { newEmail, confirmEmail } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const worker = ref<any>({});

function updateWorkerEmail(values: any) {
  isLoading.value = true;
  updateAgencyWorkerEmail(worker.value.id, { newEmail: values.newEmail })
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values: any) => {
    updateWorkerEmail(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

if (props.data != null) {
  worker.value = Object.assign({}, props.data);
}
</script>
